namespace ExampleHttpServer
{
    internal class Program
    {
        static private readonly HttpServer _httpServer = new HttpServer(new CustomLogger("HTTPSERVER"));
        static private ILogger _logger = new CustomLogger("PROGRAM");

        static private Dictionary<string, Action<string>> _commandHandlers = new Dictionary<string, Action<string>>()
        {
            { "restart", (string commandLine) => {
                _httpServer.Stop();
                //_httpServer = new HttpServer(new CustomLogger("HTTPSERVER"));
                _httpServer.Start();
            } },
            { "start", (string commandLine) => {
                //_httpServer = new HttpServer(new CustomLogger("HTTPSERVER"));
                _httpServer.Start();
            } },
            { "help", (string commandLine) => {
                _logger.Log("Available commands:");
                _logger.Log("\tstart\tStarts HttpServer.");
                _logger.Log("\trestart\tRestarts HttpServer.");
                _logger.Log("\texit\tStops HttpServer and exit.");
            } },
        };

        static void Main(string[] args)
        {
            _httpServer.Start();

            string? input = Console.ReadLine();

            while (input != "exit")
            {
                HandleCommand(input);

                input = Console.ReadLine();
            }

            _httpServer.Stop();
        }
        static private void HandleCommand(string? command)
        {
            if (string.IsNullOrEmpty(command)) return;

            foreach (var commandHandler in _commandHandlers)
            {
                if (command.StartsWith(commandHandler.Key, StringComparison.OrdinalIgnoreCase))
                {
                    commandHandler.Value.Invoke(command);

                    return;
                }
            }
        }
    }
}