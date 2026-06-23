using Sudoku.Client.Network;
using Sudoku.Client.UI;
using Sudoku.Shared.Network;

namespace Sudoku.Client.Forms;

public partial class LobbyForm : Form
{
  private readonly GameClient _client;
  private bool _gameStarted;

  public LobbyForm(GameClient client)
  {
    _client = client;
    InitializeComponent();
    UiTheme.ApplyFormStyle(this);
    lblWelcome.Text = $"Xin chào, {_client.Username}!";
    lblWins.Text = _client.Wins.ToString();
    lblLosses.Text = _client.Losses.ToString();
    lblTotal.Text = _client.TotalGames.ToString();
    _client.MessageReceived += OnMessageReceived;
    _client.Disconnected += OnDisconnected;
  }

  private void SetStatus(string text, bool isError = false)
  {
    lblStatus.Text = text;
    lblStatus.ForeColor = isError ? UiTheme.Danger : UiTheme.TextMuted;
    pnlStatusDot.BackColor = isError ? UiTheme.Danger : UiTheme.Primary;
  }

  private void OnDisconnected()
  {
    if (IsDisposed)
      return;

    BeginInvoke(() =>
    {
      MessageBox.Show("Mất kết nối với server.", "Ngắt kết nối",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      Close();
    });
  }

  private void OnMessageReceived(NetworkMessage message)
  {
    if (IsDisposed)
      return;

    BeginInvoke(() =>
    {
      switch (message.Type)
      {
        case MessageType.FindMatchResponse:
          SetStatus(message.Message ?? string.Empty);
          if (message.Message?.Contains("tìm thấy", StringComparison.OrdinalIgnoreCase) == true)
            btnFindMatch.Enabled = false;
          break;
        case MessageType.CreateRoomResponse:
          if (message.Success)
          {
            txtRoomCode.Text = message.RoomCode ?? string.Empty;
            SetStatus(message.Message ?? "Chờ đối thủ...");
          }
          else
          {
            MessageBox.Show(message.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus(message.Message ?? "Lỗi.", isError: true);
          }
          break;
        case MessageType.JoinRoomResponse:
          if (!message.Success)
          {
            MessageBox.Show(message.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus(message.Message ?? "Lỗi.", isError: true);
          }
          else
            SetStatus(message.Message ?? "Đã vào phòng.");
          break;
        case MessageType.GameStart:
          StartGame(message);
          break;
      }
    });
  }

  private void StartGame(NetworkMessage message)
  {
    if (_gameStarted)
      return;
    _gameStarted = true;

    Hide();
    var game = new GameForm(_client, message);
    game.FormClosed += (_, _) => Close();
    game.Show();
  }

  private int GetDifficulty() =>
    cboDifficulty.SelectedIndex switch
    {
      0 => 30,
      1 => 40,
      2 => 50,
      _ => 40
    };

  private async void btnFindMatch_Click(object sender, EventArgs e)
  {
    SetStatus("Đang tìm đối thủ...");
    btnFindMatch.Enabled = false;
    try
    {
      await _client.SendAsync(new NetworkMessage
      {
        Type = MessageType.FindMatch,
        EmptyCells = GetDifficulty()
      });
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
      btnFindMatch.Enabled = true;
      SetStatus("Lỗi kết nối.", isError: true);
    }
  }

  private async void btnCreateRoom_Click(object sender, EventArgs e)
  {
    try
    {
      var response = await _client.SendAndWaitAsync(new NetworkMessage
      {
        Type = MessageType.CreateRoom,
        EmptyCells = GetDifficulty()
      }, MessageType.CreateRoomResponse);

      if (response.Success)
      {
        txtRoomCode.Text = response.RoomCode ?? string.Empty;
        SetStatus(response.Message ?? "Chờ đối thủ...");
      }
      else
      {
        MessageBox.Show(response.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        SetStatus(response.Message ?? "Lỗi.", isError: true);
      }
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }

  private async void btnJoinRoom_Click(object sender, EventArgs e)
  {
    var code = txtRoomCode.Text.Trim().ToUpperInvariant();
    if (code.Length != 6)
    {
      MessageBox.Show("Mã phòng phải có 6 ký tự.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    try
    {
      var response = await _client.SendAndWaitAsync(new NetworkMessage
      {
        Type = MessageType.JoinRoom,
        RoomCode = code
      }, MessageType.JoinRoomResponse);

      if (!response.Success)
      {
        MessageBox.Show(response.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        SetStatus(response.Message ?? "Lỗi.", isError: true);
      }
      else
        SetStatus(response.Message ?? "Đã vào phòng.");
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }

  private void LobbyForm_FormClosed(object sender, FormClosedEventArgs e)
  {
    _client.MessageReceived -= OnMessageReceived;
    _client.Disconnected -= OnDisconnected;
    Application.Exit();
  }
}
