using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace ExampleHttpServer.Routes
{
    public class RootRouter : Router
    {
        public RootRouter(string absolutePath, ILogger logger) : base(absolutePath, logger)
        {
            _endPoints.Add(
                new RouteEndPoint(absolutePath + "/", GET, async (ctx) =>
                {
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse res = ctx.Response;

                    JObject responseBody = new JObject()
                    {
                        ["serverStatus"] = "running"
                    };
                    byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseBody));

                    res.ContentType = "application/json";
                    res.ContentEncoding = Encoding.UTF8;
                    res.ContentLength64 = data.LongLength;

                    await res.OutputStream.WriteAsync(data);


                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Close();
                })
            );
            _endPoints.Add(
                new RouteEndPoint(absolutePath + "/time", GET, async (ctx) =>
                {
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse res = ctx.Response;

                    JObject responseBody = new JObject()
                    {
                        ["serverTime"] = DateTime.Now.ToString()
                    };
                    byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseBody));

                    res.ContentType = "application/json";
                    res.ContentEncoding = Encoding.UTF8;
                    res.ContentLength64 = data.LongLength;

                    await res.OutputStream.WriteAsync(data);


                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Close();
                })
            );
        }
    }
}
