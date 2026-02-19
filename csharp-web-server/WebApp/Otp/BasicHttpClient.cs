

using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace HOS.Otp;

public static class BasicHttpClient
{
    private static readonly string[] TRUSTED = ["CN=*.telegram.org"];
    private static readonly SslClientAuthenticationOptions sslOptions = new SslClientAuthenticationOptions
    {
        AllowRenegotiation = true,
        AllowTlsResume = true,
        CertificateRevocationCheckMode = X509RevocationMode.Online,
        RemoteCertificateValidationCallback = (object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) =>
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true; // Certificate is valid according to default checks
            }

            if (certificate is not null
                && !string.IsNullOrEmpty(certificate.Subject)
                && TRUSTED.Contains(certificate.Subject, StringComparer.InvariantCultureIgnoreCase))
                return true;

            System.Console.WriteLine("[sslPolicyErrors = '{0}' ]",sslPolicyErrors);
            System.Console.WriteLine("[subj = '{0}' ]", certificate?.Subject);

            return false;
        },
        EncryptionPolicy = EncryptionPolicy.RequireEncryption,
    };
    public static string Curl(string url, (string key, string val)[] args, string method = "GET")
    {
        System.Console.WriteLine("going to : '{0}'", url);

        Match match = Regex.Match(url, "^(?<protocol>http[s]{0,1}):[/][/](?<host>[^/:]+)([:](?<port>[0-9]+)){0,1}(?<resource>.*)$");
        if (!match.Success) return "1/ CURL ERROR: incorrect url provided: " + url;
        string? host     = match.Groups["host"]?.Value;
        string? protocol = match.Groups["protocol"]?.Value.ToLower();
        string? resource = match.Groups["resource"]?.Value;
        int port         = int.Parse("0"+match.Groups["port"]?.Value);
        port = port == 0 ? (protocol is "http" ? 80 : 443) : port;

        
        // System.Console.WriteLine( "host     = '{0}'" , host );     
        // System.Console.WriteLine( "protocol = '{0}'" , protocol );
        // System.Console.WriteLine( "resource = '{0}'" , resource ); 
        // System.Console.WriteLine( "port     = '{0}'" , port );
        
        if (string.IsNullOrEmpty(host)) return "2/ CURL ERROR : unknown host : " + url;
        if (string.IsNullOrEmpty(resource) || resource.Trim().Length == 0) resource = "/"; 

        // System.Console.WriteLine("curl [req:parse] : {0}://{1}:{2}/{3}", protocol, host, port, resource);

        byte[] request = Encoding.UTF8.GetBytes(
        $"""
        {method} {resource} HTTP/1.1
        Connection: close
        Host: {host}
        User-Agent: curl/8.13.0
        Accept: */*

        
        """);

        using TcpClient client = new();
        client.Connect(host, port);
        if (!client.Connected) return "3/ CURL ERROR : disconnected from " + host + ":" + port;
        byte[] buffer = new byte[2048];
        int readBytes = 0;

        if (protocol is "https")
        {
            SslStream stream = new SslStream(client.GetStream(), false);
            stream.AuthenticateAsClient(sslOptions);
            stream.Write(request, 0, request.Length);
            stream.Flush();

            readBytes = stream.Read(buffer, 0, buffer.Length);
        }
        else
        {
            NetworkStream stream = client.GetStream();// : ;
            stream.Write(request, 0, request.Length);
            stream.Flush();
            readBytes = stream.Read(buffer, 0, buffer.Length);
        }
        
        if (readBytes < 1) return "4/ CURL ERROR: unable to get response";

        string response = Encoding.UTF8.GetString(buffer, 0, readBytes);
        int bodyIndex = response.IndexOf("\r\n\r\n");
        if (bodyIndex < 0) bodyIndex = response.IndexOf("\n\n");
        if (bodyIndex < 0) return "5/ CURL ERROR: unknown response : " + response;

        return response[bodyIndex..].Trim();


    }
}