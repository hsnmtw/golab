
using System;
using System.Linq;
using web.Http;

namespace web
{
    public class Program
    {
        static void Main(string[]args)
        {   

            for(int i=2;i<13;i++){
                System.Console.WriteLine(new DateTime(2020,i,1).AddDays(-1).Day);
            }

            int port = 80;
            if(args.Length>0 && args[0].All(char.IsNumber))
                port = int.Parse(args[0]);
            var server = new Server();
            server.AddMiddleware((req,res)=>{
                if(req.Query("err")=="500") 
                    throw new Exception(string.Format("middleware test : {0}", req.Path ));
            });
            //server.AddMiddleware((req,res)=>{
            //    if(req.Cookie("session_id") == "" && !req.Path.StartsWith("/assets"))
            //        req.Path = "/assets/html/user/login.html";
            //});
            server.Run( "127.0.0.1", port, maxRetry: 10 );
        }
    }
}