
using System.Collections.Generic;

namespace WebApp.Http
{
    public class Middleware
    {
        public List<Action<Context>> Actions { get; private set; }

        public Middleware()
        {
            Actions = new List<Action<Context>>();
        }
    }
}