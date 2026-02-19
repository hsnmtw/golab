
using System.IO;

using System.Collections.Generic;
using System.Text;
using System.Net.Security;


namespace WebApp.Http
{
    public class HttpResponse
    {
        private const string TOKEN = "9j21iox7";
        public const string HTTP_RESPONSE_STATUS_CODE_100 = "Continue";
        public const string HTTP_RESPONSE_STATUS_CODE_101 = "Switching Protocols";
        public const string HTTP_RESPONSE_STATUS_CODE_200 = "OK";
        public const string HTTP_RESPONSE_STATUS_CODE_201 = "Created";
        public const string HTTP_RESPONSE_STATUS_CODE_202 = "Accepted";
        public const string HTTP_RESPONSE_STATUS_CODE_203 = "Non-Authoritative Information";
        public const string HTTP_RESPONSE_STATUS_CODE_204 = "No Content";
        public const string HTTP_RESPONSE_STATUS_CODE_205 = "Reset Content";
        public const string HTTP_RESPONSE_STATUS_CODE_300 = "Multiple Choices";
        public const string HTTP_RESPONSE_STATUS_CODE_301 = "Moved Permanently";
        public const string HTTP_RESPONSE_STATUS_CODE_302 = "Found";
        public const string HTTP_RESPONSE_STATUS_CODE_303 = "See Other";
        public const string HTTP_RESPONSE_STATUS_CODE_305 = "Use Proxy";
        public const string HTTP_RESPONSE_STATUS_CODE_306 = "(Unused)";
        public const string HTTP_RESPONSE_STATUS_CODE_307 = "Temporary Redirect";
        public const string HTTP_RESPONSE_STATUS_CODE_400 = "Bad Request";
        public const string HTTP_RESPONSE_STATUS_CODE_402 = "Payment Required";
        public const string HTTP_RESPONSE_STATUS_CODE_403 = "Forbidden";
        public const string HTTP_RESPONSE_STATUS_CODE_404 = "Not Found";
        public const string HTTP_RESPONSE_STATUS_CODE_405 = "Method Not Allowed";
        public const string HTTP_RESPONSE_STATUS_CODE_406 = "Not Acceptable";
        public const string HTTP_RESPONSE_STATUS_CODE_408 = "Request Timeout";
        public const string HTTP_RESPONSE_STATUS_CODE_409 = "Conflict";
        public const string HTTP_RESPONSE_STATUS_CODE_410 = "Gone";
        public const string HTTP_RESPONSE_STATUS_CODE_411 = "Length Required";
        public const string HTTP_RESPONSE_STATUS_CODE_413 = "Payload Too Large";
        public const string HTTP_RESPONSE_STATUS_CODE_414 = "URI Too Long";
        public const string HTTP_RESPONSE_STATUS_CODE_415 = "Unsupported Media Type";
        public const string HTTP_RESPONSE_STATUS_CODE_417 = "Expectation Failed";
        public const string HTTP_RESPONSE_STATUS_CODE_426 = "Upgrade Required";
        public const string HTTP_RESPONSE_STATUS_CODE_500 = "Internal Server Error";
        public const string HTTP_RESPONSE_STATUS_CODE_501 = "Not Implemented";
        public const string HTTP_RESPONSE_STATUS_CODE_502 = "Bad Gateway";
        public const string HTTP_RESPONSE_STATUS_CODE_503 = "Service Unavailable";
        public const string HTTP_RESPONSE_STATUS_CODE_504 = "Gateway Timeout";
        public const string HTTP_RESPONSE_STATUS_CODE_505 = "HTTP Version Not Supported";

        private static readonly Dictionary<string, string> _statusCodes = new Dictionary<string, string>{
            {"100",HTTP_RESPONSE_STATUS_CODE_100},
            {"101",HTTP_RESPONSE_STATUS_CODE_101},
            {"200",HTTP_RESPONSE_STATUS_CODE_200},
            {"201",HTTP_RESPONSE_STATUS_CODE_201},
            {"202",HTTP_RESPONSE_STATUS_CODE_202},
            {"203",HTTP_RESPONSE_STATUS_CODE_203},
            {"204",HTTP_RESPONSE_STATUS_CODE_204},
            {"205",HTTP_RESPONSE_STATUS_CODE_205},
            {"300",HTTP_RESPONSE_STATUS_CODE_300},
            {"301",HTTP_RESPONSE_STATUS_CODE_301},
            {"302",HTTP_RESPONSE_STATUS_CODE_302},
            {"303",HTTP_RESPONSE_STATUS_CODE_303},
            {"305",HTTP_RESPONSE_STATUS_CODE_305},
            {"306",HTTP_RESPONSE_STATUS_CODE_306},
            {"307",HTTP_RESPONSE_STATUS_CODE_307},
            {"400",HTTP_RESPONSE_STATUS_CODE_400},
            {"402",HTTP_RESPONSE_STATUS_CODE_402},
            {"403",HTTP_RESPONSE_STATUS_CODE_403},
            {"404",HTTP_RESPONSE_STATUS_CODE_404},
            {"405",HTTP_RESPONSE_STATUS_CODE_405},
            {"406",HTTP_RESPONSE_STATUS_CODE_406},
            {"408",HTTP_RESPONSE_STATUS_CODE_408},
            {"409",HTTP_RESPONSE_STATUS_CODE_409},
            {"410",HTTP_RESPONSE_STATUS_CODE_410},
            {"411",HTTP_RESPONSE_STATUS_CODE_411},
            {"413",HTTP_RESPONSE_STATUS_CODE_413},
            {"414",HTTP_RESPONSE_STATUS_CODE_414},
            {"415",HTTP_RESPONSE_STATUS_CODE_415},
            {"417",HTTP_RESPONSE_STATUS_CODE_417},
            {"426",HTTP_RESPONSE_STATUS_CODE_426},
            {"500",HTTP_RESPONSE_STATUS_CODE_500},
            {"501",HTTP_RESPONSE_STATUS_CODE_501},
            {"502",HTTP_RESPONSE_STATUS_CODE_502},
            {"503",HTTP_RESPONSE_STATUS_CODE_503},
            {"504",HTTP_RESPONSE_STATUS_CODE_504},
            {"505",HTTP_RESPONSE_STATUS_CODE_505},
        };

        public bool IsFlushed { get; private set; }
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        private readonly Stream _stream;
        public HttpResponse(Stream stream)
        {
            _stream = stream;
        }

        public void SetHeader(string name, string value)
        {
            string key = string.Format("{0}", name).Trim().ToLower();
            if (_headers.ContainsKey(key)) return;
            _headers[key] = value;
        }

        public void SetCookie(string name, string value, DateTime expiry)
        {
            //append server-specific token to end of value to prevent attack
            value = string.Format("{0}_{1}_{2}", value, expiry.ToString("yyMMddHHmm"), TOKEN);
            //HTTPS// SetHeader("Set-Cookie", string.Format("{0}={1}; Max-Age=12000; Expires={2}; Secure; HttpOnly; Path=/", name, value, expiry));
            SetHeader("Set-Cookie", string.Format("{0}={1}; Max-Age=12000; Expires={2:s}; HttpOnly; Path=/", name, value, expiry));
        }

        public void Write(string content)
        {
            Write(Encoding.UTF8.GetBytes(content));
        }

        public static string GetLast(string[] items)
        {
            if (items.Length > 0) return items[items.Length - 1];
            return "";
        }

        public static string GetContentType(string path)
        {
            string contentType = "text/plain";
            if (string.IsNullOrEmpty(path)) return contentType;
            string ext = GetLast(path.Split('.'));
            switch (ext)
            {
                case "md"   : contentType = "text/markdown"; break;
                case "png"  :
                case "jpg"  :
                case "jpeg" :
                case "gif"  :
                case "bmp"  :
                case "tif"  :
                case "tiff" : contentType = "image/"+ext;       break;
                case "svg"  : contentType = "image/svg+xml";    break;
                case "zip"  :
                case "rtf"  :
                case "pdf"  :
                case "xml"  :
                case "wasm" :
                case "json" : contentType = "application/"+ext; break;
                case "txt"  :
                case "xslt" :
                case "xhtml": 
                case "htm"  :
                case "html" : 
                case "csv"  :
                case "css"  : contentType = "text/"+ext;        break;
                case "mjs"  :
                case "js"   : contentType = "text/javascript";  break;
                case "otf"  :
                case "ttf"  :
                case "eot"  :
                case "woff" :
                case "woff2": contentType = "font/"+ext;        break;
                case "dll"  :
                case "msi"  :
                case "exe"  : contentType = "application/octet-stream"; break;
                case "doc"  :
                case "docx" : contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
                case "xls"  :
                case "xlsx" : contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break;
                case "ppt"  :
                case "pptx" : contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation"; break;
            }
            return contentType;
        }

        public void WriteFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            if (IsFlushed)
                return;
            SetHeader(HeaderConstants.STATUS,"200");
            SetHeader(HeaderConstants.CONTENT_TYPE, GetContentType(path));
            SetHeader("Last-Modified", $"{new FileInfo(path).LastWriteTimeUtc:ddd', 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' GMT'}");

            //
            string ext = path.Split('.').Last().ToLower();
            switch(ext) {
                case "mjs":
                case "js":
                {
                    SetHeader(HeaderConstants.EXPIRES, "0");
                    SetHeader(HeaderConstants.AGE, "0");
                    SetHeader(HeaderConstants.PRAGMA, "no-cache");
                    SetHeader(HeaderConstants.CACHE_CONTROL, "no-store");
                    SetHeader(HeaderConstants.ACS_CTRL_MAX_AGE, "0");
                } break;
                case "css": 
                case "html":
                case "ico":
                case "ttf":
                case "svg":
                case "otf":
                case "png":
                case "jpg":
                case "jpeg": {
                    SetHeader(HeaderConstants.EXPIRES, $"{Utils.Now.AddDays(100):ddd', 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' GMT'}");
                } break;
                default: {
                    SetHeader(HeaderConstants.EXPIRES, $"{Utils.Now.AddDays(-1):ddd', 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' GMT'}");
                } break;
            }
            SetDefaultHeaders();
            string headers =
                    string.Format("HTTP/1.1 {0} {1}\r\n{2}\r\n\r\n",
                        _headers["status"],
                        _statusCodes.ContainsKey(_headers["status"]) ? _statusCodes[_headers["status"]] : "",
                        string.Join("\r\n", _headers.Select(x => string.Format("{0}: {1}", x.Key, x.Value)))
                        );

            _stream.Write(Encoding.UTF8.GetBytes(headers));
            using var fs = File.OpenRead(path);
            fs.CopyTo(_stream);
            fs.Close();
        }

        private static void WriteTo(Stream source, Stream target)
        {
            byte[] buffer = new byte[0x10000];
            int n;
            while ((n = source.Read(buffer, 0, buffer.Length)) != 0)
                target.Write(buffer, 0, n);
        }

        public void Write(byte[] buffer)
        {
            if(IsFlushed) return;
            SetHeader(HeaderConstants.STATUS, "200");
            SetDefaultHeaders();
            SetHeader(HeaderConstants.CONTENT_TYPE, "text/html; charset=utf-8");
            // SetHeader("Content-Length", buffer.Length.ToString());
            string headers =
                    string.Format("HTTP/1.1 {0} {1}\r\n{2}\r\n\r\n",
                        _headers["status"],
                        _statusCodes.ContainsKey(_headers["status"]) ? _statusCodes[_headers["status"]] : "",
                        string.Join("\r\n", _headers.Select(x => string.Format("{0}: {1}", x.Key, x.Value)))
                        );

            _stream.Write(Encoding.UTF8.GetBytes(headers));
            _stream.Write(buffer);
        }

        public void Flush()
        {
            try
            {
                if (_stream is SslStream sslStream) sslStream.ShutdownAsync().Wait();
            }
            catch { }
            try { _stream.Flush(); _stream.Close(); } catch { }
            IsFlushed = true;
        }

        private void SetDefaultHeaders()
        {
            SetHeader("Access-Control-Allow-Origin", "*");
            SetHeader("Date", Utils.Now.ToString("ddd', 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' GMT'"));
            //SetHeader("Last-Modified", Utils.Now.ToString("ddd', 'dd' 'MMM' 'yyyy' 'HH':'mm':'ss' GMT'"));
            SetHeader("Server", "Simple Web Server By Hussain Al Mutawa");
            SetHeader("Vary", "Cookie, Accept-Encoding");
            SetHeader("X-Powered-By", "DN8");
            SetHeader("X-Content-Type-Options", "nosniff");
            SetHeader("x-frame-options", "SAMEORIGIN");	
            SetHeader("Referrer-Policy", "strict-origin");
            SetHeader("Permissions-Policy", "geolocation=()");
            SetHeader("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
		}
    }
}
