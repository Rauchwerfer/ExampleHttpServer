using System.Net;

namespace ExampleHttpServer
{
    public class RouteEndPoint
    {
        public string AbsolutePath { get; } = string.Empty;
        public string HttpMethod { get; } = string.Empty;


        private Action<HttpListenerContext> _acceptRequest = (HttpListenerContext ctx) => 
        {
            HttpListenerRequest req = ctx.Request;
            HttpListenerResponse resp = ctx.Response;

            resp.StatusCode = (int)HttpStatusCode.NotAcceptable;

            resp.Close();
        };

        public RouteEndPoint(string absolutePath, string httpMethod, Action<HttpListenerContext> acceptRequest)
        {
            AbsolutePath = absolutePath;
            HttpMethod = httpMethod;
            _acceptRequest = acceptRequest;
        }

        public void AcceptRequest(HttpListenerContext ctx)
        {
            _acceptRequest?.Invoke(ctx);
        }
    }
}
