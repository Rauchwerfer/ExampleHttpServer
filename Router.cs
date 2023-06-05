namespace ExampleHttpServer
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
    }
}
