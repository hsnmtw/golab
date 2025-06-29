using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace web.Http
{
    public class HttpResponse
    {
        private readonly Dictionary<string,string> _headers = new Dictionary<string,string>();        
        private readonly Stream _stream;
        public HttpResponse(Stream stream)
        {
            _stream = stream;
        }

        public void SetHeader(string name, string value)
        {
            _headers[string.Format("{0}",name).Trim().ToLower()] = value;
        }

        public void SetCookie(string name, string value, DateTime expiry)
        {
            _headers["set-cookie"] = string.Format("{0}={1}; Expires={2}; Secure; HttpOnly; Path=/",name,value,expiry);
        }

        public void Write(string content)
        {
            Write(Encoding.UTF8.GetBytes(content));
        }

        public static string GetLast(string[] items)
        {
            if(items.Length>0) return items[items.Length-1];
            return "";
        }

        public void WriteFile(string path)
        {
            string contentType = "text/plain";
            string ext = GetLast(path.Split('.'));
            switch (ext)
            {
                case "png"  :
                case "jpg"  :
                case "jpeg" :
                case "gif"  :
                case "bmp"  :
                case "tif"  :
                case "tiff" : contentType = "image/"+ext;       break;
                case "pdf"  :
                case "json" : contentType = "application/"+ext; break;
                case "htm"  :
                case "html" : contentType = "text/html";        break;
                case "css"  : contentType = "text/"+ext;        break;
                case "js"   : contentType = "text/javascript";  break;
                case "svg"  : contentType = "image/svg+xml";    break;
                case "ttf"  :
                case "eot"  :
                case "woff" :
                case "woff2": contentType = "font/"+ext;        break;
                case "zip"  : contentType = "application/octet-stream"; break;
            }
            using (var fs = File.OpenRead(path))
            {
                string headers = 
                "HTTP/1.1 200 OK\r\n"+
                "Content-Type: " + contentType + "\r\n"+
                "Content-Length: " + fs.Length + "\r\n"+
                "\r\n";
                var _buffer = Encoding.UTF8.GetBytes(headers);
                _stream.Write(_buffer,0,_buffer.Length);
                WriteTo(fs,_stream);
                _stream.Flush();
                fs.Close();
            }

        }

        private static void WriteTo(Stream source, Stream target)
        {
            byte[] buffer = new byte[0x10000];
            int n;
            while ((n = source.Read(buffer, 0, buffer.Length)) != 0)
                target.Write(buffer, 0, n);
        }


        private bool isFlused = false; 
        public void Write(byte[] buffer)
        {
            if(isFlused) return;
            if(!_headers.ContainsKey("status")) _headers["status"] = "200";
            //_headers["content-length"] = buffer.Length.ToString();
            var _buffer = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n");
            _stream.Write(_buffer,0,_buffer.Length);
            foreach(var k in _headers.Keys)
            {
                _buffer = Encoding.UTF8.GetBytes(string.Format("{0}: {1}\r\n", k, _headers[k]));
                _stream.Write(_buffer,0,_buffer.Length);
            }

            _buffer = Encoding.UTF8.GetBytes("\r\n\r\n");
            _stream.Write(_buffer,0,_buffer.Length);

            _stream.Write(buffer,0,buffer.Length);
            _stream.Flush();
            isFlused = true;
        }
    }
}