using System.Net;
using ExampleHttpServer.Routes;

namespace ExampleHttpServer
{
    public class HttpServer
    {
        private readonly HttpListener _listener = new HttpListener();

        private ILogger _logger;

        private Router[] _routers;
        private RouteEndPoint[] _endPoints;
        public RouteEndPoint[] EndPoints => _endPoints;

        private bool _runServer = false;

        public HttpServer(ILogger logger)
        {
            _logger = logger;
            _logger.Log("Creating new...");

            _routers = new Router[] 
            { 
                new RootRouter("", _logger), 
                new AuthRouter("/auth", _logger) 
            };
            _logger.Log("Registered Routers:");

            List<RouteEndPoint> routeEndPoints = new List<RouteEndPoint>();

            foreach (var router in _routers)
            {
                _logger.Log($"\t{router.GetType().Name} {router.AbsolutePath}");
                
                var endPoints = router.EndPoints;

                foreach (var endPoint in endPoints)
                {
                    _logger.Log($"\t\t{endPoint.HttpMethod} {endPoint.AbsolutePath}");
                }

                routeEndPoints.AddRange(router.EndPoints);
            }
            _endPoints = routeEndPoints.ToArray();

            _logger.Log("Created.");
            
        }

        public void Start(string url = "http://localhost:5000/")
        {            
            try
            {
                if (_listener.IsListening)
                {
                    _logger.Log("Server is already running!");
                    return;
                }
                
                _logger.Log($"Starting server on {url}...");
                _listener.Prefixes.Add(url); 
                _listener.Start();

                _runServer = true;

                //var cancellationToken = _cancellationTokensource.Token;

                Task.Factory.StartNew(async () =>
                {
                    while (_runServer) await HandleIncomingConnections();
                }, TaskCreationOptions.LongRunning);
                _logger.Log("Server is running.");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }            
        }

        public void Stop()
        {
            try
            {
                _logger.Log("Shutting down...");

                _runServer = false;
                _listener?.Stop();
                _logger.Log("Stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }            
        }

        private async Task HandleIncomingConnections()
        {
            HttpListenerContext ctx = await _listener.GetContextAsync();
            AcceptRequest(ctx);
        }

        private void AcceptRequest(HttpListenerContext ctx)
        {
            try
            {
                var endPoint = _endPoints.FirstOrDefault(
                e => e.AbsolutePath == ctx.Request.Url.AbsolutePath
                && e.HttpMethod == ctx.Request.HttpMethod, null);

                if (endPoint == null)
                {
                    HttpListenerResponse res = ctx.Response;

                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Close();

                    return;
                }
                
                endPoint.AcceptRequest(ctx);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }
    }
}
