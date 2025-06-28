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
                Console.WriteLine("no favicon.... just return empty response");
                res.Write(new byte[]{});
            };
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

        public void Run()
        {
            Start:
            TcpListener server = null;
            try
            {
                System.Console.WriteLine("Basic web server");
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);
                server.Start();
                while(true)
                {
                    // Console.Write("Waiting for a connection... ");
                    using (TcpClient client = server.AcceptTcpClient())
                    {
                        // Console.WriteLine("Connected!");
                        using (NetworkStream stream = client.GetStream())
                        {
                            byte[] buffer = new byte[1024];
                            stream.Read(buffer,0,buffer.Length);
                            string request = Encoding.UTF8.GetString(buffer);
                            var req = new HttpRequest(request);
                            var res = new HttpResponse(stream);                            
                            if(req.Path=="/") req.Path = "/assets/html/home.html";
                            System.Console.WriteLine("{0} [inf] {1}:{2}",DateTime.Now,req.Method,req.Path);
                            if(!(IsStaticFile(req, res) || IsHandled(req, res)))
                            {
                                res.SetHeader("status","404");
                                res.Write("404 Page Not Found");
                            }
                            stream.Flush();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine("----------------------------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(ex);
                Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine("----------------------------------------------------------------------");
                System.Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally
            {
                try { server.Stop(); } catch {}
            }
            goto Start;
        }
    }
}