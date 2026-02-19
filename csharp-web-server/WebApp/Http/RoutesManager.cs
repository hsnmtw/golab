

using HOS.App.Features.Security;
using WebApp.Http.Routes;

namespace WebApp.Http;

public static class RoutesManager
{
    public static void Initialize(Server server)
    {
        server.AddRoute("GET:/login", (context) =>
        {
            var res = context.Response;
            var req = context.Request;

            if (!string.IsNullOrEmpty(context.SessionId) && !string.IsNullOrEmpty(context.Username))
            {
                req.Path = "/home";
                return server.IsHandled(context);
            }
            req.Path = "/assets/html/user/login.html";
            return server.IsStaticFile(context);
        });
    }
}