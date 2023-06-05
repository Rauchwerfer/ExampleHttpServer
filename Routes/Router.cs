using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace ExampleHttpServer.Routes
{
    public abstract class Router
    {
        public const string POST = "POST";
        public const string GET = "GET";

        public string AbsolutePath { get; }

        protected readonly List<RouteEndPoint> _endPoints = new List<RouteEndPoint>();
        public RouteEndPoint[] EndPoints => _endPoints.ToArray();

        protected ILogger _logger;

        public Router(string absolutePath, ILogger logger)
        {
            AbsolutePath = absolutePath;
            _logger = logger;
        }

        /// <summary>
        /// Optional authentication middleware.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        protected async Task<bool> AuthenticationMiddleware(HttpListenerContext ctx)
        {
            try
            {
                //HttpListenerRequest req = ctx.Request;

                //byte[] requestBodyBytes = new byte[req.ContentLength64];
                //await req.InputStream.ReadAsync(requestBodyBytes);

                //string bodyInJson = Encoding.UTF8.GetString(requestBodyBytes);
                
                //JObject body = JObject.Parse(bodyInJson);
                //if (body.Value<string>("email") == "test" && body.Value<string>("password") == "12345")
                //{
                //    return true;
                //}

                //_logger.Log(req.ContentType);
                //_logger.Log(Encoding.UTF8.GetString(requestBodyBytes));
                return false;
            }
            catch 
            {
                return false;
            }
        }
        
        protected void ResponseStatusCode(HttpListenerContext ctx, HttpStatusCode statusCode)
        {
            HttpListenerResponse resp = ctx.Response;
            resp.StatusCode = (int)statusCode; 
            resp.Close();
        }
    }
}
