
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
            new Server().Run( port );
        }
    }
}