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
        private Dictionary<string,Action<HttpRequest,Stream>> Handlers { get; set; }
        public Server()
        {
            Handlers = new Dictionary<string, Action<HttpRequest,Stream>>();
        }

        private bool IsStaticFile(HttpRequest req, Stream stream)
        {
            if(req.Path.StartsWith("/assets/") && File.Exists("."+req.Path))
            {
                using (var fs = File.OpenRead("."+req.Path))
                {
                    string contentType = "text/plain";
                    string ext = GetLast(req.Path.Split('.'));
                    switch (ext)
                    {
                        case "png":
                        case "jpg":
                        case "jpeg":
                        case "gif":
                        case "bmp":
                        case "tif":
                        case "tiff": contentType = "image/"+ext; break;
                        case "pdf":
                        case "json": contentType = "application/"+ext; break;
                        case "htm":
                        case "html": contentType = "text/html"; break;
                        case "css": contentType = "text/"+ext; break;
                        case "js": contentType = "text/javascript"; break;
                        case "ttf":
                        case "zip": contentType = "application/octet-stream"; break;
                    }
                    byte[] buffer = Encoding.UTF8.GetBytes(String.Format("HTTP/1.1 200 OK\r\nContent-Length: {0}\r\nContent-Type: {1}\r\n\r\n",fs.Length,contentType));
                    stream.Write(buffer,0,buffer.Length);
                    WriteTo(fs,stream);
                    fs.Close();
                }
                return true;
            }
            return false;
        }
        private bool IsHandled(HttpRequest request, Stream stream)
        {
            var route = string.Format("{0}:{1}",request.Method,request.Path);
            if(Handlers.ContainsKey(route))
            {
                Handlers[route](request,stream);
                return true;
            }
            return false;
        }

        public static void WriteTo(Stream sourceStream, Stream targetStream)
        {
            byte[] buffer = new byte[0x10000];
            int n;
            while ((n = sourceStream.Read(buffer, 0, buffer.Length)) != 0)
                targetStream.Write(buffer, 0, n);
        }

        public static string GetLast(string[] items)
        {
            if(items.Length>0) return items[items.Length-1];
            return "";
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
                    Console.Write("Waiting for a connection... ");
                    using (TcpClient client = server.AcceptTcpClient())
                    {
                        Console.WriteLine("Connected!");
                        using (NetworkStream stream = client.GetStream())
                        {
                            byte[] buffer = new byte[1024];
                            stream.Read(buffer,0,buffer.Length);
                            string request = Encoding.UTF8.GetString(buffer);
                            System.Console.WriteLine("------------------------------------");
                            var req = new HttpRequest(request);
                            if(req.Path=="/") req.Path = "/assets/html/home.html";
                            System.Console.WriteLine(req);
                            System.Console.WriteLine("------------------------------------");
                            if(!(IsStaticFile(req, stream) || IsHandled(req, stream)))
                            {
                                buffer = Encoding.UTF8.GetBytes("HTTP/1.1 404 NOT FOUND\r\nStatus: 404\r\n\r\nrequested:"+req.Path +" is not found");
                                stream.Write(buffer,0,buffer.Length);
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