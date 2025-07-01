
using System;
using System.Linq;
using web.Http;

namespace web
{
    public class Program
    {
        static void Main(string[]args)
        {   
            int port = 80;
            if(args.Length>0 && args[0].All(char.IsNumber))
                port = int.Parse(args[0]);
            var server = new Server();
            server.AddMiddleware((req,res)=>{
                if(req.Query("err")=="500") 
                    throw new Exception(string.Format("middleware test : {0}", req.Path ));
            });
            server.Run( port, maxRetry: 10 );
        }
    }
}