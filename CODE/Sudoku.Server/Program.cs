using System.Net;
using Sudoku.Server.Services;

const int port = 5050;
const string dbFile = "sudoku.db";

var dbPath = Path.Combine(AppContext.BaseDirectory, dbFile);
using var database = new DatabaseService(dbPath);
using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
  e.Cancel = true;
  cts.Cancel();
};

var server = new SudokuTcpServer(IPAddress.Any, port, database);
Console.WriteLine("=================================");
Console.WriteLine("  SUDOKU MULTIPLAYER SERVER");
Console.WriteLine($"  Port: {port}");
Console.WriteLine($"  Database: {dbPath}");
Console.WriteLine("  Nhấn Ctrl+C để dừng.");
Console.WriteLine("=================================");

try
{
  await server.RunAsync(cts.Token);
}
catch (OperationCanceledException)
{
  Console.WriteLine("Server đã dừng.");
}
finally
{
  server.Stop();
}
