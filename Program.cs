using System.Runtime.InteropServices;

namespace ExampleHttpServer
{
    internal class Program
    {
        private const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        private const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        static private extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static private extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static private extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        static private readonly HttpServer _httpServer = new HttpServer(new CustomLogger("HTTPSERVER"));
        static private ILogger _logger = new CustomLogger("PROGRAM");

        static private Dictionary<string, Action<string>> _commandHandlers = new Dictionary<string, Action<string>>()
        {
            { "restart", (string commandLine) => {
                _httpServer.Stop();
                _httpServer.Start();
            } },
            { "start", (string commandLine) => {
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
            DisableQuickEdit();

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

        static private bool DisableQuickEdit()
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            uint consoleMode;
            if (!GetConsoleMode(consoleHandle, out consoleMode))
            {
                return false;
            }

            consoleMode &= ~ENABLE_QUICK_EDIT;

            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                return false;
            }

            return true;
        }
    }
}