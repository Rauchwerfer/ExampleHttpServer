using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace ExampleHttpServer
{
    public class AuthRouter : Router
    {
        public AuthRouter(string absolutePath, ILogger logger) : base(absolutePath, logger)
        {
            _endPoints.Add(
                new RouteEndPoint(absolutePath + "/login", POST, async (HttpListenerContext ctx) =>
                {
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse res = ctx.Response;

                    
                    byte[] requestBodyBytes = new byte[req.ContentLength64];
                    await req.InputStream.ReadAsync(requestBodyBytes);

                    //_logger.Log(req.ContentType);
                    //_logger.Log(Encoding.UTF8.GetString(requestBodyBytes));

                    JObject responseBody = new JObject()
                    {
                        ["userId"] = 1
                    };
                    byte[] responseBodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseBody));

                    res.ContentType = "application/json";
                    res.ContentEncoding = Encoding.UTF8;
                    res.ContentLength64 = responseBodyBytes.LongLength;

                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.AppendCookie(new Cookie("ExampleServerCookie1Key", "ExampleServerCookie1Value"));
                    await res.OutputStream.WriteAsync(responseBodyBytes);

                    res.Close();
                })
            );
        }
    }
}
