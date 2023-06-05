using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace ExampleHttpServer.Routes
{
    public class AuthRouter : Router
    {
        public AuthRouter(string absolutePath, ILogger logger) : base(absolutePath, logger)
        {
            _endPoints.Add(
                new RouteEndPoint(absolutePath + "/login", POST, async (ctx) =>
                {
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse res = ctx.Response;


                    byte[] requestBodyBytes = new byte[req.ContentLength64];
                    await req.InputStream.ReadAsync(requestBodyBytes);

                    string bodyInJson = Encoding.UTF8.GetString(requestBodyBytes);

                    JObject body = JObject.Parse(bodyInJson);


                    if (body.TryGetValue("email", out var emailEncoded) 
                        && !string.IsNullOrEmpty(emailEncoded.Value<string>())
                        && body.TryGetValue("password", out var passwordEncoded)
                        && !string.IsNullOrEmpty(passwordEncoded.Value<string>())
                        )
                    {
                        var email = emailEncoded.Value<string>();
                        var password = passwordEncoded.Value<string>();

                        // Emulating credentials check
                        bool isCredentialsCorrect = email == "test" && password == "12345";

                        if (isCredentialsCorrect) 
                        {
                            // Emulationg sending user object
                            JObject responseBody = new JObject()
                            {
                                ["userId"] = 1,
                                ["email"] = email,
                            };

                            byte[] responseBodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseBody));

                            res.ContentType = "application/json";
                            res.ContentEncoding = Encoding.UTF8;
                            res.ContentLength64 = responseBodyBytes.LongLength;
                            res.StatusCode = (int)HttpStatusCode.OK;
                            res.AppendCookie(new Cookie("ExampleUserEmail", email));
                            await res.OutputStream.WriteAsync(responseBodyBytes);

                            res.Close();
                        }
                        else
                        {
                            ResponseStatusCode(ctx, HttpStatusCode.Unauthorized);
                            return;
                        }
                    }
                    else
                    {
                        ResponseStatusCode(ctx, HttpStatusCode.UnprocessableEntity);
                        return;
                    }
                })
            );
        }
    }
}
