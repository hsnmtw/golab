using System.Collections.Generic;
using System.IO;

using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using System.Diagnostics;

using HOS.App.Features.Security;
using WebApp.Otp;

namespace WebApp.Http;
public class Server
{
    public const string IP_REGEX = @"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.|$)){4}\b";
    public static DateTime MaxDate { get; set; }
    public static string? LicencedTo { get; set; }
    public Action<Context> PageNotFoundHandler { get; set; }
    public Action<Context> ServerErrorHandler { get; set; }
    private Dictionary<string, Func<Context, bool>> Handlers { get; set; }
    private readonly Middleware middleware;
    public const string LONG_DATE_FORMAT_GMT = "dddd', 'dd' 'MMMM' 'yyyy' 'HH':'mm':'ss' GMT'";
    public Server()
    {
        PageNotFoundHandler = (context) =>
        {
            if (context.Request.Method != HttpRequestMethod.GET && Utils.Now > Server.MaxDate)
            {
                context.Response.WriteFile("./assets/html/errors/EXP.html");
                Logger.Warning("Expired License: EXP - " + context.Request.Path, context.Username, context.IP);
                return;
            }
            context.Response.SetHeader(HeaderConstants.STATUS, "404");
            context.Response.WriteFile("./assets/html/errors/404.html");
            Logger.Warning("Page not found: 404 - " + context.Request.Path, context.Username, context.IP);
        };

        ServerErrorHandler = (context) =>
        {
            context.Response.SetHeader(HeaderConstants.STATUS, "500");
            context.Response.WriteFile("./assets/html/errors/500.html");
            Logger.Error("Server Error: 500 - " + context.Request.Path);
        };
        middleware = new Middleware();
        Handlers = new Dictionary<string, Func<Context, bool>>(StringComparer.InvariantCultureIgnoreCase);
    }
    

    public void AddMiddleware(Action<Context> action)
    {
        middleware.Actions.Add(action);
    }

    public void AddRoute(string name, Func<Context, bool> predicate)
    {
        Handlers[name] = predicate;
    }

    public bool IsStaticFile(Context context) {
        HttpRequest req = context.Request;
        HttpResponse response = context.Response;

        if (req.Path.StartsWith("/assets/") && File.Exists("." + req.Path))
        {
            //Expires: <day-name>, <day> <month> <year> <hour>:<minute>:<second> GMT
            response.SetHeader(HeaderConstants.EXPIRES, Utils.Now.AddDays(1).ToString(LONG_DATE_FORMAT_GMT));
            response.WriteFile("." + req.Path);
            return true;
        }
        return false;
    }

    public bool AuthorizationHandled(Context context) {
        if (!string.IsNullOrEmpty(context.SessionId) && !string.IsNullOrEmpty(context.Username)) return false;

        HttpRequest request = context.Request;
        HttpResponse response = context.Response;

        var route = $"{request.Method}:{request.Path}";
        System.Console.WriteLine($"\n///////////[{route}]////////////\n");
        switch (route)
        {
            case "GET:/login":
            case "POST:/login":
            case "POST:/resend/otp":
            case "POST:/reset/password":
            case "GET:/logout":
            case "GET:/home":
            case "GET:/":
                return false;
        }

        //if (request.Method == HttpRequestMethod.POST && request.Path.Contains("/")) return false;

        if (string.IsNullOrEmpty(context.SessionId))
        {
            response.Write("Session expired ...");
            return true;
        }
        return false;
    }

    public bool IsHandled(Context context) {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;

        var route = string.Format("{0}:{1}", request.Method, request.Path);
        if (!Handlers.ContainsKey(route)) return false;
        if (AuthorizationHandled(context)) return true;
        return Handlers[route](context);
    }

    // private const string CSC = @"c:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe";
    private static readonly string WORKING_DIRECTORY = new DirectoryInfo(".\\assets").FullName;

    private static string? ShellExecute(string args) {
        try {
            var processInfo = new ProcessStartInfo() {
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = @"c:\windows\system32\cmd.exe",
                ArgumentList = { "/c", args },
                ErrorDialog = false,
            };
            var process = new System.Diagnostics.Process();
            process.StartInfo = processInfo;
            process.Start();
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        } catch(Exception ex) {
            Logger.Error("Error when executing shell command : " + ex.Message );
        }
        return default;
    }

    public static string GetBIOS() {
        // if (!OperatingSystem.IsWindows())
        //     return "H0S";
        return string.Format("{0}", ShellExecute("wmic bios get serialnumber /value")).Trim().Split('=').Last().Trim().ToUpper();
    }

    public static string GetIP() {
        // if (!OperatingSystem.IsWindows())
        //     return "82.25.35.194"; //
        return string.Format("{0}", ShellExecute("ipconfig|findstr IPv4")).Trim().Split('\n').First().Split(':').Last().Trim();
    }

    public void Run(int port = 80, int maxRetry = 100, bool build = false, Action? then = null) {
        ServicePointManager.DefaultConnectionLimit = 125;
        bool _thenInvoked = false;
        if (port == 0) port = 80;
        //AddCompiledScriptsRoutes(build, string.Format("{0}\\pages", WORKING_DIRECTORY));
        for (;maxRetry>0;--maxRetry) {

            LingerOption lingerOption    = new(true, 100);
            IPEndPoint   ipLocalEndPoint = new(IPAddress.Any, port);
            TcpListener  listener        = new(ipLocalEndPoint);

            Logger.Info(string.Format("Basic web server is started on usrl/port http://{0}:{1}/", ipLocalEndPoint.Address, ipLocalEndPoint.Port));

            listener.Start();
            if (then != null && !_thenInvoked) then.Invoke();
            _thenInvoked = true;
            bool isLocked = false;
            int errors = 0;
            while (errors < 1000) {
                if (!isLocked && Utils.Now > Server.MaxDate) {
                    isLocked = true;
                    string except = "POST:/login";
                    string[] keys = [.. Handlers.Keys.Where(x => x.Trim().ToUpper().StartsWith("POST:") && !Equals(except, x))];
                    foreach (var key in keys) Handlers.Remove(key);
                }
                try {
                    TcpClient client = listener.AcceptTcpClient();
                    client.NoDelay = true;
                    client.LingerState = lingerOption;
                    client.SendTimeout = 5000;
                    client.Client.Ttl = 100;
                    ThreadPool.UnsafeQueueUserWorkItem(new HttpsConnectionHandler(port==443, client, this, middleware), preferLocal: false);
                } catch (Exception ex) {
                    Logger.Error("*---------------------------------------------------------------------");
                    Logger.Error("| SERVER ERROR: " + ex.ToString());
                    Logger.Error("*---------------------------------------------------------------------");
                    ++errors;
                }
            }
            try { listener.Stop(); } catch { }
        }
        Logger.Error("Exceeded number of max-retry, shutting down ...");
    }

    
    internal class HttpsConnectionHandler(bool secured,TcpClient client, Server server, Middleware middleware) : IThreadPoolWorkItem
    {
        public void Execute() {
            if (!client.Connected
            || client.Client is null
            || !client.Client.IsBound
            // || client.Client.Available < 1
            ) { return; }

            Context context = default;

            try {
                
                using NetworkStream networkStream = client.GetStream();
                using Stream stream = secured ? SecureWithSsl(networkStream) : networkStream;
                // stream.ReadTimeout = 250;
                // byte[] buffer = new byte[409600]; // TODO: need to make this number adjustable
                // int readBytes = stream.Read(buffer, 0, buffer.Length);

                // if (readBytes < 1) { return; }

                // var req       = new HttpRequest(buffer.AsSpan()[..readBytes]);
                var req       = new HttpRequest(stream);
                var res       = new HttpResponse(stream);
                var sessionId = req.SessionId;
                var user      = SessionRepository.GetUser(sessionId); //TODO: remove this nasty dependency
                
                // System.Console.WriteLine("///\n'{0}'\n///",sessionId);

                if (user is null) sessionId = null;
                string ipAddress = string.Format("{0}", client.Client.RemoteEndPoint).Split(':')[0].Trim();

                context = new Context() {
                    Response = res,
                    Request = req,
                    SessionId = string.Format("{0}", sessionId),
                    Username = user?.Name ?? "Guest",
                    Authorization = user?.Authorization,
                    IP = ipAddress
                };

                foreach (var action in middleware.Actions) {
                    action(context);
                }

                if (req.Path == "/favicon.ico") {
                    res.SetHeader(HeaderConstants.EXPIRES, Utils.Today.AddDays(300).ToString(LONG_DATE_FORMAT_GMT));
                    req.Path = "/assets/favicon.ico";
                }

                if (req.Path == "/") {
                    res.SetHeader(HeaderConstants.EXPIRES, Utils.Today.AddDays(10).ToString(LONG_DATE_FORMAT_GMT));
                    req.Path = "/assets/html/home.html";
                }

                string route = string.Format("{0}:{1}", req.Method, req.Path);

                Logger.Info(route, context.Username, context.IP);
                // System.Console.WriteLine();
                // System.Console.WriteLine("----------------------------------------------------");
                // string request = Encoding.UTF8.GetString(buffer, 0, readBytes);
                // System.Console.WriteLine(request);
                // System.Console.WriteLine("----------------------------------------------------");
                // System.Console.WriteLine(req);
                // System.Console.WriteLine("----------------------------------------------------");

                if (server.Handlers.TryGetValue("*", out var handler)) {
                    handler(context);
                }

                else if (!(server.IsStaticFile(context) || server.IsHandled(context)))
                    server.PageNotFoundHandler(context);

                try {
                    //without it, we get CURL error : curl: (56) schannel: server closed abruptly (missing close_notify)
                    if (stream is SslStream sslStream) {
                        sslStream.ShutdownAsync().Wait();
                        stream.Read(new byte[128], 0, 128);
                    }
                } catch (Exception ex) {
                    Logger.Error("Error when sutting down SSL stream : " + ex.Message);
                }

            } catch (IOException ioex) {
                Logger.Error($"[FATAL ERROR/IO] : {ioex.Message}");
                try { context.Response?.SetHeader(HeaderConstants.STATUS, "500"); } catch { }
            } catch (System.Security.Authentication.AuthenticationException sslEx) {
                Logger.Error($"[FATAL ERROR/SSL] : {sslEx.Message}");
            } catch (Exception ex) {
                Logger.Error("CLIENT CONNECTION THREAD: " + ex.Message);
                if(Environment.MachineName == "DESKTOP-8PVQSE3") {
                    System.Console.Error.WriteLine(ex);
                } else {
                    OtpManager.Instance.SendMessageAdmin($"[HOS] EXCEPTION: `{ex}`");
                }
            }

            try { context.Response?.Flush(); } catch { }
            try { client.Close();            } catch { } 
        }

        private static readonly SslServerAuthenticationOptions sslOptions = new(){
            EnabledSslProtocols =
                    System.Security.Authentication.SslProtocols.None
                | System.Security.Authentication.SslProtocols.Tls12
                | System.Security.Authentication.SslProtocols.Tls13,
            ServerCertificate = new X509Certificate2("../ssl/cert.pfx", "_P@ssw0rd"),
            ClientCertificateRequired = false,
            ApplicationProtocols = [
                SslApplicationProtocol.Http11,
                SslApplicationProtocol.Http2,
                SslApplicationProtocol.Http3
            ],
            AllowRenegotiation = true,
            AllowTlsResume = true,
            CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
            EncryptionPolicy = EncryptionPolicy.RequireEncryption,
        };

        private SslStream SecureWithSsl(Stream stream) {
            SslStream sslStream = new SslStream(stream, false);
            sslStream.AuthenticateAsServer(sslOptions);
            return sslStream;
        }
    }        
}