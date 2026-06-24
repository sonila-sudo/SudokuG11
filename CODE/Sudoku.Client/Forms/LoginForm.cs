using Sudoku.Client.Network;
using Sudoku.Client.UI;

namespace Sudoku.Client.Forms;

public partial class LoginForm : Form
{
  private readonly GameClient _client;
  private bool _connected;

  public LoginForm(GameClient client)
  {
    _client = client;
    InitializeComponent();
    pnlCard.Paint += (_, e) =>
    {
      using var pen = new Pen(UiTheme.Border);
      e.Graphics.DrawRectangle(pen, 0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
    };

    pnlServer.Paint += (_, e) =>
    {
      using var pen = new Pen(UiTheme.Border);
      e.Graphics.DrawRectangle(pen, 0, 0, pnlServer.Width - 1, pnlServer.Height - 1);
    };
    UiTheme.ApplyFormStyle(this);
  }

  private async void LoginForm_Load(object sender, EventArgs e)
  {
    await TryConnectAsync();
  }

  private async void btnRetry_Click(object sender, EventArgs e)
  {
    await TryConnectAsync();
  }

  private async Task TryConnectAsync()
  {
    if (!int.TryParse(txtServerPort.Text.Trim(), out var port) || port is < 1 or > 65535)
    {
      SetConnectionState(false, "Cổng server không hợp lệ (1-65535).");
      return;
    }

    _client.ServerHost = txtServerHost.Text.Trim();
    _client.ServerPort = port;

    btnLogin.Enabled = false;
    btnRegister.Enabled = false;
    btnRetry.Enabled = false;
    lblStatus.ForeColor = UiTheme.TextMuted;
    lblStatus.Text = "Đang kết nối server...";

    try
    {
      await _client.ConnectAsync();
      _connected = true;
      SetConnectionState(true, $"Đã kết nối {_client.ServerHost}:{_client.ServerPort}");
    }
    catch (Exception ex)
    {
      _connected = false;
      var hint = ex.Message.Contains("actively refused", StringComparison.OrdinalIgnoreCase) ||
                 ex.Message.Contains("No connection", StringComparison.OrdinalIgnoreCase)
        ? " Hãy chạy server trước: dotnet run --project Sudoku.Server"
        : string.Empty;
      SetConnectionState(false, $"Không kết nối được server.{hint}");
    }
    finally
    {
      btnRetry.Enabled = true;
      UpdateActionButtons();
    }
  }

  private void SetConnectionState(bool connected, string message)
  {
    pnlStatusIndicator.BackColor = connected ? UiTheme.Success : UiTheme.Danger;
    lblStatus.Text = message;
    lblStatus.ForeColor = connected ? UiTheme.Success : UiTheme.Danger;
  }

  private void UpdateActionButtons()
  {
    btnLogin.Enabled = _connected;
    btnRegister.Enabled = _connected;
  }

  private async void btnLogin_Click(object sender, EventArgs e)
  {
    await AuthenticateAsync(isRegister: false);
  }

  private async void btnRegister_Click(object sender, EventArgs e)
  {
    await AuthenticateAsync(isRegister: true);
  }

  private async Task<bool> EnsureConnectedAsync()
  {
    if (_client.IsConnected)
      return true;

    await TryConnectAsync();
    return _connected;
  }

  private async Task AuthenticateAsync(bool isRegister)
  {
    if (!await EnsureConnectedAsync())
    {
      MessageBox.Show(
        "Chưa kết nối được server.\n\n1. Mở terminal\n2. Chạy: dotnet run --project Sudoku.Server\n3. Nhấn \"Thử lại kết nối\"",
        "Chưa kết nối server",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning);
      return;
    }

    var username = txtUsername.Text.Trim();
    var password = txtPassword.Text;

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
      MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.", "Thiếu thông tin",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    btnLogin.Enabled = false;
    btnRegister.Enabled = false;
    lblStatus.ForeColor = UiTheme.TextMuted;
    lblStatus.Text = isRegister ? "Đang đăng ký..." : "Đang đăng nhập...";

    try
    {
      if (isRegister)
      {
        var (ok, message) = await _client.RegisterAsync(username, password);
        MessageBox.Show(message, ok ? "Thành công" : "Lỗi",
          MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        return;
      }

      var (loggedIn, loginMessage) = await _client.LoginAsync(username, password);
      if (!loggedIn)
      {
        MessageBox.Show(loginMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      Hide();
      var lobby = new LobbyForm(_client);
      lobby.FormClosed += (_, _) => Close();
      lobby.Show();
    }
    catch (Exception ex)
    {
      _connected = false;
      SetConnectionState(false, "Mất kết nối với server.");
      MessageBox.Show(ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
      UpdateActionButtons();
      if (_connected)
      {
        lblStatus.ForeColor = UiTheme.Success;
        lblStatus.Text = "Sẵn sàng.";
      }
    }
  }

  private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    _client.Dispose();
  }
}
