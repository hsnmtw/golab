using System.Globalization;
using System.IO;
using System.Text;

namespace WebApp.Http;
public enum HttpRequestMethod {_,GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS, TRACE, CONNECT}
public record HttpRequestFile
(
    string Name,
    string ContentType,
    string Path
);

// public class StringKeyValue
// {
//     public StringKeyValue(string text, char sep = ':')
//     {
//         text = (""+text).Trim();
//         if(text.Length>0)
//         {
//             int eq = text.IndexOf(sep);
//             if(eq>-1)
//             {
//                 Key = text.Substring(0,eq).Trim().Trim('\r').Trim('\t').Trim('\n').Trim('\0').Trim().ToLower();
//                 Value = text.Substring(eq+1).Trim();
//             }
//         }
//     }
//     public string Key { get; private set; } = "";
//     public string Value { get; private set; } = "";
// }


public class HttpRequest
{
    private const string TOKEN = "9j21iox7";
    private HttpRequestMethod _method;
    private string _path = "";
    private string _body = "";
    private readonly Dictionary<string,string> _query   = [];
    private readonly Dictionary<string,string> _form    = [];
    private readonly Dictionary<string,string> _cookies = [];
    private readonly Dictionary<string,string> _headers = [];
    private readonly Dictionary<string,HttpRequestFile> _files   = [];

    public int ContentLength { get; private set; } = -1;
    public int ContentLengthRead { get; private set; } = 0;
    public string SessionId { get { return Cookie("session_id"); } }

    private int body_start = 0;
    public static string u8(ReadOnlySpan<byte> bytes) => System.Text.Encoding.UTF8.GetString(bytes);

    public const int BUFFER_SIZE = 1024;
    private readonly Stream _stream;



    public HttpRequest(Stream stream) {
        _stream = stream;
        Parse();
    }

    bool done = false;
    private void Read(ref ReadOnlySpan<byte> req) {
        done = ContentLength>0 && ContentLengthRead>=ContentLength;
        req = [];
        if (done) return;

        byte[] buffer = new byte[BUFFER_SIZE]; 
        int readBytes = _stream.Read(buffer, 0, BUFFER_SIZE);
        if (readBytes<1) {
            done = true;
            return;
        }
        req = readBytes < 1 ? [] : new ReadOnlySpan<byte>(buffer[..readBytes]);
        if (body_start>0) {
            ContentLengthRead+=readBytes;
        }
        done = ContentLength>0 && ContentLengthRead>=ContentLength;
        return;
    }

    private void Parse() {
        int i;
        ReadOnlySpan<byte> req = [];
        Read(ref req);
        i = ParseFirstLineHeader(req);
        body_start = ParseHeaders(ref req,i);
        ContentLengthRead = req.Length-body_start;
        // ValidateContentLength(l);
        ParseHeaderCookies();

        if (_method == HttpRequestMethod.GET) return;
        
        var (content_type, boundary) = GetHeaderContentType();
        
        switch(content_type) {
            case HeaderConstants.CTYP_FRM_URL_ENC: ParseBodyUrlEncoded(req);         break;
            case HeaderConstants.CTYP_MULTI_PART : ParseBodyMultiPart(req,boundary); break;
            case HeaderConstants.CTYP_PSV        : ParseBodyPsv(req);                break;
            default                              : ReadBodyText(req);                break;
        }
    }

    private void ReadBodyText(ReadOnlySpan<byte> req) {
        _body = u8(req[body_start..]);
        if (done) {
            return;
        }
        var sb = new StringBuilder(_body);
        int _retry = 0;
        while(!done && _retry++ < 10) {
            Read(ref req);
            //ContentLengthRead+=req.Length;
            sb.Append(u8(req));
        }
        _body = sb.ToString();
    }


    private (string content_type, string boundary) GetHeaderContentType() {
        int i,l;
        string cth,_content_type,_boundary;

        cth = Header(HeaderConstants.CONTENT_TYPE);
        if (string.IsNullOrEmpty(cth)) return ("","");

        l = cth.Length;
        for(i=0;i<l && cth[i] != ';';++i);

        _content_type = cth;
        _boundary = "";

        if(i<l && cth[i] == ';') {
            _content_type = cth[..i];
            for(;i<l && cth[i] != '=';++i);
            if(cth[i] == '=') {
                i++;
                _boundary = cth[i..];
            }
        }
        return (_content_type, _boundary);
    }

    private void ValidateContentLength(int l) {
        if(ContentLength>0 && l-body_start != ContentLength) {
            throw new Exception($"The request has content length '{ContentLength}' but server was able to read only '{l-body_start}'");
        }
    }

    private int ParseHeaders(ref ReadOnlySpan<byte> req, int header_start) {
        string key,value;
        int l = req.Length;
        int i=header_start,start;
        int _retry = 0;
        _continue_parsing_header:
        //parse headers
        for(start = ++i; i < l; ++i) {
            //System.Console.WriteLine("///////////////[{0}]",(char)req[i]);
            if ((char)req[i] is ':' or ' ') {
                key = u8(req[start..i]).ToLower();
                for(; i < l && (char)req[i] is ' ' or ':'; ++i);
                for(start = i; i < l && (char)req[i] is not('\r' or '\n'); ++i);
                value = u8(req[start..i]);
                _headers[key] = value;
                if (key.Equals(HeaderConstants.CONTENT_LENGTH, StringComparison.InvariantCultureIgnoreCase)) {
                    if (int.TryParse(value.Trim(), out int cl)) {
                        ContentLength = cl;
                    };
                }
                if(i+4<l && u8(req[i..(i+4)]) is "\r\n\r\n") break; // end of header
                for(;i<l && req[i] != '\n';++i);
                start = ++i;
            }
        }

        for(; i>=4 && u8(req[(i-4)..i]) is not "\r\n\r\n" && i < l && char.IsWhiteSpace((char)req[i]); ++i); // skip white spaces

        if (!done && _retry++<10 && (ContentLength == 0 || u8(req[(i-4)..i]) is not "\r\n\r\n")) {
        
            for(;i>=0 && (char)req[i] != '\n';--i);
            ReadOnlySpan<byte> segment = l > i+1 ? req[(i+1)..] : [];
            Read(ref req);
            req = new ReadOnlySpan<byte>([..segment,..req]);
            i=0;l=req.Length;
            goto _continue_parsing_header;
        }

        return i;
    }


    private int ParseFirstLineHeader(ReadOnlySpan<byte> req) {
        string key,value;
        int l = req.Length;
        int i,start;
        //parse method
        for(i = 0; i < l && req[i] != ' '; ++i);
        if (i >= l) throw new Exception(string.Format("l={0}, i={0}",l,i));
        switch(u8(req[..i])) {
            case "GET":     _method = HttpRequestMethod.GET;     break;
            case "PUT":     _method = HttpRequestMethod.PUT;     break;
            case "POST":    _method = HttpRequestMethod.POST;    break;
            case "PATCH":   _method = HttpRequestMethod.PATCH;   break;
            case "DELETE":  _method = HttpRequestMethod.DELETE;  break;
            case "HEAD":    _method = HttpRequestMethod.HEAD;    break;
            case "OPTIONS": _method = HttpRequestMethod.OPTIONS; break;
            case "TRACE":   _method = HttpRequestMethod.TRACE;   break;
            case "CONNECT": _method = HttpRequestMethod.CONNECT; break;
            default:        _method = HttpRequestMethod._;       break;
        }
        //parse path
        for(start = ++i; i < l && (char)req[i] is not ('?' or ' ' or '\r' or '\n'); ++i);
        _path = u8(req[start..i]).ToLower();

        if((char)req[i]=='?') {
            //parse query string
            for(start = ++i; i < l && (char)req[i] is not(' ' or '\r' or '\n'); ++i) {
                if ((char)req[i] == '=') {
                    key = u8(req[start..i]).ToLower();
                    for(start = ++i; i < l && (char)req[i] is not('&' or ' ' or '\r' or '\n'); ++i);
                    value = System.Web.HttpUtility.UrlDecode(u8(req[start..i]));
                    _query[key] = value;
                    start = ++i;
                }
            }
        }
        
        for(;i<l && (char)req[i] != '\n';++i);

        return i;
    }


    private void ParseHeaderCookies() {
        //parse cookies
        ReadOnlySpan<char> cookies = Header(HeaderConstants.COOKIE);
        int l = cookies.Length;
        int i,start;
        string key,value;

        for (start = i = 0; i < l; ++i) {
            for(;i<l && cookies[i] is not (' ' or '=');++i);
            key = cookies[start..i].ToString().ToLower();
            for(;i<l && cookies[i] is ' ' or '=';++i);
            for(start=i;i<l && cookies[i] is not (' ' or ';');++i);
            value = cookies[start..i].ToString();

            string[] ps = value.Split('_');
            if (ps.Length < 3 || !Equals(ps[ps.Length - 1], TOKEN)) continue;
            var expiryDate = ps[ps.Length - 2];
            if (!ps[ps.Length - 2].ToCharArray().All(char.IsNumber)) continue;
            DateTime expiry = DateTime.ParseExact(expiryDate, "yyMMddHHmm", CultureInfo.InvariantCulture);
            if (expiry < Utils.Now) continue;
            if (ps[0] is "DELETE") continue;

            _cookies[key] = ps[0];
        }
    }


    private void ParseBodyPsv(ReadOnlySpan<byte> req) {
        ReadBodyText(req);

        int i;            
        string[] lines = _body.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        if (lines.Length == 2) {
            string[] keys = lines[0].Split('|');
            string[] vals = lines[0].Split('|');
            if (keys.Length==vals.Length){
                for(i=0;i<keys.Length;++i){
                    _form[keys[i].ToLower()] = vals[i];
                }
            }
        }
    }


    private void ParseBodyMultiPart(ReadOnlySpan<byte> req, string boundary) {


        int start,i,l;
        string value;
        string _name,_filename,_content_type;
        int bl=boundary.Length;

        int _retry = 0;
        ReadOnlySpan<byte> body = req[body_start..];
        ContentLengthRead=req.Length-body_start;
        System.Console.WriteLine("====================================");
        System.Console.WriteLine(u8(body[..50]));
        System.Console.WriteLine("====================================");
        System.Console.WriteLine("\n[*] Reading request [{0}/{1}] {2} {3} {4}", 
            ContentLengthRead, 
            ContentLength, 
            done, 
            req.Length, body.Length);   
        while(!done && _retry++ < 100) {
            Read(ref req);
            body = new ReadOnlySpan<byte>([..body, ..req]);
            //ContentLengthRead = body.Length;

        System.Console.WriteLine("[+] Reading request [{0}/{1}] {2} {3} {4}", 
            ContentLengthRead, 
            ContentLength, 
            done, 
            req.Length, body.Length); 
                       
        }

        req = body;
        body_start=0;
        l=req.Length;

        for(i=body_start;i<l;++i){
            if(i+bl<l && !Equals(boundary,u8(req[i..(i+bl)]))) {
                continue;
            }
            i+=bl;
            if(i+2<l && (char)req[i+1]=='-' && (char)req[i+2]=='-') break; //end of body
            for(;i<l && (char)req[i] != '\n';++i);
            for(;i<l;++i){
                if((char)req[i] != '"') continue;
        
                for(start=i;i<l && (char)req[i] != '"';++i);
                _name="";_filename="";_content_type="";
                _name = u8(req[start..i]);

                for(;i<l && (char)req[i] != '"';++i);
                if((char)req[i] == '"') {
                    for(start=++i;i<l && (char)req[i] != '"';++i);
                    _filename = $"./temp/{Guid.NewGuid()}-{u8(req[start..i])}";

                    for(i++;i<l && (char)req[i] != '\n';++i);
                    for(i++;i<l && (char)req[i] !=  ':';++i);
                    for(i++;i<l && (char)req[i] ==  ' ';++i);
                    start=i;
                    for(;i<l && !char.IsWhiteSpace((char)req[i]);++i);
                    _content_type = u8(req[start..i]);
                    for(;i<l && char.IsWhiteSpace((char)req[i]);++i);
                    int w=0;
                    start = i;
                    for(;i+4+bl<l && !Equals(boundary,u8(req[(i+4)..(i+4+bl)]));++i){
                        w++;
                    }
                    using var fs = File.Create(_filename);
                    fs.Write(req[start..i].ToArray(),0,i-start);
                    fs.Flush();
                    fs.Close();

                    System.Console.WriteLine();
                    System.Console.WriteLine("written={0}",w);
                    
                    _files[_name.ToLower()] = new(_name,_content_type,_filename);
                    _form[_name.ToLower()] = _filename;
                } else {
                    for(;i<l && (char)req[i] != '\n';++i); // skip to end
                    for(;i<l && char.IsWhiteSpace((char)req[i]);++i); // skip empty line
                    for(start=i;i<l;++i) {
                        if(i+bl<l && Equals(boundary,u8(req[i..(i+bl)]))) {
                            value=u8(req[start..i]).Trim();
                            _form[_name.ToLower()] = value;   
                            break;
                        }                   
                    }
                }                         
            }                        
        }
            
    }


    private void ParseBodyUrlEncoded(ReadOnlySpan<byte> req) {
        ReadBodyText(req);
        int start,i,l = req.Length;
        string key,value;

        for(start = i = body_start; i < l; ++i) {
            if (_body[i] == '=') {
                key = _body[start..i].ToLower();
                for(start = ++i; i < l && _body[i] is not('&' or ' ' or '\r' or '\n'); ++i);
                value = System.Web.HttpUtility.UrlDecode(_body[start..i]);
                _form[key] = value;
                start = ++i;
            }
        }
    }


    // private void OldParse(string requestText)
    // {
    //     string[] lines = (""+requestText).Trim().Split('\n');
    //     //line 0 contains method and path and query
    //     for(int i=0;i<lines.Length;i++)
    //     {
    //         string line = lines[i].Trim();
    //         if(i==0) ParseFirstLine(line);
    //         else ParseOtherLines(line);
    //         if(line.Length==0)
    //         {
    //             //TODO: parse json directly to form
    //             //TODO: parse url-encoded-form to form
    //             //TODO: parse form-parts to form and extract files
    //             for(int j=i;j<lines.Length;j++)
    //                 _body += lines[j];
    //             _body = _body.Trim().Trim('\r').Trim('\n').Trim(' ').Trim('\0').Trim('\t').Trim();
    //             break;
    //         }
    //     }
    // }


    public override string ToString()
    {
        string[] view = {
        "\n method  : ", _method.ToString(),
        "\n path    : ", _path,
        "\n query   : ", string.Join("&",   _query.Select(kv => string.Format("{0}={1}",kv.Key,kv.Value))),
        "\n headers : ", string.Join("&", _headers.Select(kv => string.Format("{0}={1}",kv.Key,kv.Value))),
        "\n cookies : ", string.Join("&", _cookies.Select(kv => string.Format("{0}={1}",kv.Key,kv.Value))),
        "\n form    : ", string.Join("&",    _form.Select(kv => string.Format("{0}={1}",kv.Key,kv.Value))),
        "\n files   : ", string.Join("&",   _files.Select(kv => string.Format("{0}={1}",kv.Key,kv.Value.Path)))
        };
        return string.Join("",view);
    }

    // private void ParseCookies(string cookies)
    // {
    //     string[] parts = cookies.Split(new[]{"; "}, StringSplitOptions.RemoveEmptyEntries);
    //     for (int i = 0; i < parts.Length; i++)
    //     {
    //         try
    //         {
    //             var kv = new StringKeyValue(parts[i], '=');
    //             if (string.IsNullOrEmpty(kv.Key) || string.IsNullOrEmpty(kv.Value)) continue;
    //             string[] ps = kv.Value.Split('_');
    //             if (ps.Length < 3 || !Equals(ps[ps.Length - 1], TOKEN)) continue;
    //             var expiryDate = ps[ps.Length - 2];
    //             if (!ps[ps.Length - 2].ToCharArray().All(char.IsNumber)) continue;
    //             DateTime expiry = DateTime.ParseExact(expiryDate, "yyMMddHHmm", CultureInfo.InvariantCulture);
    //             if (expiry < Utils.Now) continue;
    //             if (ps[0] is "DELETE") continue;
    //             _cookies[kv.Key] = ps[0];
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.Error.WriteLine("[FATAL ERROR /ParseCookie/ ]: "+ex.Message);
    //         }
    //     }
    // }

    // private void ParseOtherLines(string line)
    // {
    //     var kv = new StringKeyValue(line);
    //     if(string.IsNullOrEmpty(kv.Key)) return;
    //     if(Equals(kv.Key,"cookie"))
    //     {
    //         ParseCookies(kv.Value);
    //         return;
    //     }
    //     _headers[kv.Key] = kv.Value;
    // }

    // private void ParseFirstLine(string line)
    // {
    //     string[]parts = line.Split(' ');
    //     if(parts.Length==0) return;

    //     switch(parts[0].ToUpper())
    //     {
    //         case "GET":     _method = HttpRequestMethod.GET;     break;
    //         case "PUT":     _method = HttpRequestMethod.PUT;     break;
    //         case "POST":    _method = HttpRequestMethod.POST;    break;
    //         case "PATCH":   _method = HttpRequestMethod.PATCH;   break;
    //         case "DELETE":  _method = HttpRequestMethod.DELETE;  break;
    //         case "HEAD":    _method = HttpRequestMethod.HEAD;    break;
    //         case "OPTIONS": _method = HttpRequestMethod.OPTIONS; break;
    //         case "TRACE":   _method = HttpRequestMethod.TRACE;   break;
    //         case "CONNECT": _method = HttpRequestMethod.CONNECT; break;
    //         default:        _method = HttpRequestMethod._;       break;
    //     }

    //     if(parts.Length>1)
    //     {
    //         string[] ps = parts[1].Split('?');
    //         _path = ps[0].ToLower();
    //         if(ps.Length>1)
    //         {
    //             string[] qs = ps[1].Split(new[]{"&"},StringSplitOptions.RemoveEmptyEntries);
    //             for(int j=0;j<qs.Length;j++)
    //             {
    //                 var kv = new StringKeyValue(qs[j],'=');
    //                 if(!string.IsNullOrEmpty(kv.Key)) _query[kv.Key] = System.Web.HttpUtility.UrlDecode(kv.Value);
    //             }
    //         }
    //     }
    // }

    public HttpRequestMethod Method { get { return _method; } }
    public string Path { get { return _path; } set { _path = (""+value).Trim().ToLower(); } }
    public string Body { get { return _body; } }

    public string Header(string name) { return string.IsNullOrEmpty(name) || !_headers.ContainsKey(name.ToLower()) ? "" : _headers[(""+name).ToLower()]; }
    public string Form(string name)   { return string.IsNullOrEmpty(name) || !_form.ContainsKey(name.ToLower())    ? "" : _form[(""+name).ToLower()]; }
    public string Query(string name)  { return string.IsNullOrEmpty(name) || !_query.ContainsKey(name.ToLower())   ? "" : _query[(""+name).ToLower()]; }
    public string Cookie(string name) { return string.IsNullOrEmpty(name) || !_cookies.ContainsKey(name.ToLower()) ? "" : _cookies[(""+name).ToLower()]; }
}

internal enum HttpRequestTokenType
{
    EOF,
    TEXT,
    SPACE,
    QUESTION,
    AMBERSAND,
    EQUALS,
    NEW_LINE,
    COLON,
    SEMI_COLON,
}