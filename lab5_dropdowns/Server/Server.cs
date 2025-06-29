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
        private Dictionary<string,Action<HttpRequest,HttpResponse>> Handlers { get; set; }
        public Server()
        {
            Handlers = new Dictionary<string, Action<HttpRequest,HttpResponse>>();
            Handlers["GET:/favicon.ico"] = (req,res) => {
                LogWarning("no favicon.... just return empty response");
                res.Write(new byte[]{});
            };
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
            if(Handlers.ContainsKey(route))
            {
                Handlers[route](request,response);
                return true;
            }
            return false;
        }

        public void Run(int port = 80, int maxRetry = 100)
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
                IPAddress ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(x=> x.AddressFamily.ToString()=="InterNetwork");
                // System.Console.WriteLine(string.Join("\n|\n",Dns.GetHostEntry(Dns.GetHostName()).AddressList.Select(x=>x.ToString()+" :::: "+x.AddressFamily)));
                IPEndPoint ipLocalEndPoint = new IPEndPoint(IPAddress.Any, port);
                LogInfo(string.Format("Basic web server is running on url http://{0}:{1}/",ipAddress,ipLocalEndPoint.Port));
                server = new TcpListener(ipLocalEndPoint);
                server.Start();
                while(true)
                {
                    // Console.Write("Waiting for a connection... ");
                    using (TcpClient client = server.AcceptTcpClient())
                    {
                        // Log("Connected!");
                        using (NetworkStream stream = client.GetStream())
                        {
                            byte[] buffer = new byte[1024];
                            stream.Read(buffer,0,buffer.Length);
                            string request = Encoding.UTF8.GetString(buffer);
                            var req = new HttpRequest(request);
                            var res = new HttpResponse(stream);                            
                            if(req.Path=="/") req.Path = "/assets/html/home.html";
                            string route = string.Format("{0}:{1}",req.Method,req.Path);
                            if(!(IsStaticFile(req, res) || IsHandled(req, res)))
                            {
                                res.SetHeader("status","404");
                                // res.Write("404 Page Not Found");
                                res.WriteFile("./assets/html/errors/404.html");                                
                                LogWarning("Page not found: 404 - " + route);
                            }else{
                                LogInfo(route);
                            }
                            stream.Flush();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LogError("----------------------------------------------------------------------");
                LogError(ex.ToString());
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