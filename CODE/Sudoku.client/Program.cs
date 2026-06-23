using Sudoku.Client.Forms;
using Sudoku.Client.Network;

namespace Sudoku.Client;

static class Program
{
  [STAThread]
  static void Main()
  {
    ApplicationConfiguration.Initialize();
    using var client = new GameClient();
    Application.Run(new LoginForm(client));
  }
}
