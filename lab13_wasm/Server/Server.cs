using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

namespace web.Http
{
    public class Server
    {
        public Action<HttpRequest,HttpResponse> PageNotFoundHandler { get; set; }
        public Action<HttpRequest,HttpResponse> ServerErrorHandler { get; set; }
        private Dictionary<string,Func<HttpRequest,HttpResponse,bool>> Handlers { get; set; }
        private readonly Middleware middleware;
        public Server()
        {
            PageNotFoundHandler = (req,res) => {
                res.SetHeader("status","404");
                res.WriteFile("./assets/html/errors/404.html");                                
                LogWarning("Page not found: 404 - " + req.Path);
            };
            ServerErrorHandler = (req,res) => {
                res.SetHeader("status","500");
                res.WriteFile("./assets/html/errors/500.html");                                
                LogError("Server Error: 500 - " + req.Path);
            };
            middleware = new Middleware();
            Handlers = new Dictionary<string, Func<HttpRequest,HttpResponse,bool>>();
            Handlers["GET:/favicon.ico"] = (req,res) => {
                LogWarning("no favicon.... just return empty response");
                res.Write(new byte[]{});
                return true;
            };
        }

        public void AddMiddleware(Action<HttpRequest,HttpResponse> action)
        {
            middleware.Actions.Add(action);
        }

        public void AddRoute(string name, Func<HttpRequest,HttpResponse,bool> predicate)
        {
            Handlers[name] = predicate;
        }

        public static void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.Write("\n{0:d'/'M hh':'mm':'ss}",DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Green;
            System.Console.Write(" [inf] ");
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.Write(message);
            System.Console.Out.Flush();
        }
        public static void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.Write("\n{0:d'/'M hh':'mm':'ss}",DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.Write(" [wrn] ");
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.Write(message);
            System.Console.Out.Flush();
        }
        public static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.Error.Write("\n{0:d'/'M hh':'mm':'ss}",DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Error.Write(" [err] ");
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.Error.Write(message);
            System.Console.Error.Flush();
        }

        private bool IsStaticFile(HttpRequest req, HttpResponse response)
        {
            if(req.Path.StartsWith("/assets/") && File.Exists("."+req.Path))
            {
                response.WriteFile("."+req.Path);
                return true;
            }
            return false;
        }
        private bool IsHandled(HttpRequest request, HttpResponse response)
        {
            var route = string.Format("{0}:{1}",request.Method,request.Path);
            if(!Handlers.ContainsKey(route)) return false;
            if(string.IsNullOrEmpty(request.SessionId))
            {
                request.Path = "/assets/html/user/login.html";
                return IsStaticFile(request,response);
            }
            return Handlers[route](request,response);
        }

        private static object _sync = new object();

        public void Run(string address="127.0.0.1",int port = 80, int maxRetry = 100)
        {
            if(port==0) port=80;
            Start:
            if(maxRetry--<0)
            {
                LogError("Exceeded number of max-retry, shutting down");
                return;
            }
            TcpListener server = null;
            try
            {
                IPAddress ipAddress = IPAddress.Parse(address);
                IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, port);
                LogInfo(string.Format("Basic web server is running on url http://{0}:{1}/",ipLocalEndPoint.Address,ipLocalEndPoint.Port));
                server = new TcpListener(ipLocalEndPoint);
                server.Start();
                while(true)
                {
                    // Console.Write("Waiting for a connection... ");
                    using (TcpClient client = server.AcceptTcpClient())
                    {
                        lock(_sync)
                        {
                            // Log("Connected!");
                            using (NetworkStream stream = client.GetStream())
                            {

                                HttpRequest req = null;
                                HttpResponse res = null;

                                try
                                {
                                    byte[] buffer = new byte[1024];
                                    stream.Read(buffer, 0, buffer.Length);
                                    string request = Encoding.UTF8.GetString(buffer);
                                    req = new HttpRequest(request);
                                    res = new HttpResponse(stream, req.SessionId);

                                    foreach (var action in middleware.Actions)
                                        action(req, res);

                                    if (req.Path == "/") req.Path = "/assets/html/home.html";
                                    string route = string.Format("{0}:{1}", req.Method, req.Path);
                                    LogInfo(route);
                                    if (!(IsStaticFile(req, res) || IsHandled(req, res))) {
                                        PageNotFoundHandler(req, res);
                                    }
                                }
                                catch(Exception ex)
                                {
                                    LogError(string.Format("TCP/LISTENER ERROR: {0} : {1}",req.Path,ex.ToString()));
                                    ServerErrorHandler(req,res);
                                }
                                finally
                                {
                                    stream.Flush();
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LogError("----------------------------------------------------------------------");
                LogError("SERVER ERROR: "+ex.ToString());
                LogError("----------------------------------------------------------------------");
            }
            finally
            {
                try { server.Stop(); } catch {}
            }
            goto Start;
        }
    }
}