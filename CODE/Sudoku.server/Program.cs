using Sudoku.Server.Network;
using Sudoku.Server.Services;
using System.Threading;

namespace Sudoku.Server
{
    internal static class Program
    {
        /// <summary>
        /// Server entrypoint: creates services and starts the TCP listener.
        /// Usage: `dotnet run --project CODE/Sudoku.server -- [port]`
        /// </summary>
        public static async Task Main(string[] args)
        {
            int port = 8888;
            if (args.Length > 0 && int.TryParse(args[0], out var p)) port = p;

            var roomManager = new RoomManager();
            var generator = new SudokuGenerator();
            var gameService = new GameService(generator);
            roomManager.GameService = gameService;

            var server = new TcpServer(port, roomManager, gameService);

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("[Program] Shutdown requested (Ctrl+C)");
                server.Stop();
            };

            Console.WriteLine($"[Program] Starting Sudoku server on port {port}...");
            await server.StartAsync();
        }
    }
}
