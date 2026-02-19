namespace WebApp.Http
{
    public struct Context
    {
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public string Username { get; set; }
        public string? Authorization { get; set; }
        public string SessionId { get; set; }
        public string IP { get; set; }
    }
}