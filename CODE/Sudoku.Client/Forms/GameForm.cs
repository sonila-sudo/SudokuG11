using System.Diagnostics;
using Sudoku.Client.Network;
using Sudoku.Client.UI;
using Sudoku.Shared.Network;
using Sudoku.Shared.Sudoku;

namespace Sudoku.Client.Forms;

public partial class GameForm : Form
{
  private const int CellSize = 62;
  private const int MaxMistakes = 3;

  private readonly GameClient _client;
  private readonly NetworkMessage _startMessage;
  private readonly SudokuLogic _logic = new();
  private readonly TextBox[,] _sudokuCells = new TextBox[9, 9];
  private readonly Stopwatch _stopwatch = new();
  private readonly System.Windows.Forms.Timer _timer = new() { Interval = 1000 };
  private readonly HashSet<(int Row, int Col)> _givenCells = new();
  private readonly HashSet<(int Row, int Col)> _solvedCells = new();
  private int[,] _initialPuzzle = new int[9, 9];

  private bool _gameEnded;
  private bool _awaitingGameResult;
  private bool _endGameHandled;
  private bool _suppressTextChange;
  private int _totalEmptyCells;
  private int _myCorrectCells;
  private int _opponentCorrectCells;
  private int _mistakeCount;
  private string _opponentName = "Đối thủ";

  private readonly Color _colorDefaultBack = Color.White;
  private readonly Color _colorAltBack = Color.FromArgb(248, 250, 252);
  private readonly Color _colorHighlightLine = Color.FromArgb(232, 239, 247);
  private readonly Color _colorSameNumber = Color.FromArgb(205, 220, 240);
  private readonly Color _colorFocus = Color.FromArgb(180, 210, 245);
  private readonly Color _colorSolved = Color.FromArgb(219, 234, 254);

  public GameForm(GameClient client, NetworkMessage startMessage)
  {
    _client = client;
    _startMessage = startMessage;   
    _opponentName = startMessage.OpponentName ?? "Đối thủ";
    _totalEmptyCells = startMessage.TotalCells > 0 ? startMessage.TotalCells : startMessage.EmptyCells;
    InitializeComponent();
    pnlSidebar.Paint += (_, e) =>
    {
      using var pen = new Pen(UiTheme.Border);
      e.Graphics.DrawRectangle(pen, 0, 0, pnlSidebar.Width - 1, pnlSidebar.Height - 1);
    };
    UiTheme.ApplyFormStyle(this);
    lblOpponent.Text = _opponentName;
    _client.MessageReceived += OnMessageReceived;
    _client.Disconnected += OnDisconnected;
    _timer.Tick += (_, _) => UpdateTimerLabels();
  }

  private void GameForm_Load(object sender, EventArgs e)
  {
    CreateSudokuGrid();
    StartFromNetwork(_startMessage);
    UpdateProgressLabels();
    _stopwatch.Start();
    _timer.Start();
  }

  public void StartFromNetwork(NetworkMessage message)
  {
    if (message.Puzzle == null || message.Solution == null)
      return;

    _logic.LoadPuzzle(message.Puzzle, message.Solution);
    _initialPuzzle = (int[,])message.Puzzle.Clone();
    _givenCells.Clear();
    _solvedCells.Clear();
    _myCorrectCells = 0;
    _opponentCorrectCells = 0;
    _mistakeCount = 0;
    UpdateMistakeLabel();
    progressMy.Maximum = Math.Max(1, _totalEmptyCells);
    progressOpponent.Maximum = Math.Max(1, _totalEmptyCells);

    for (var r = 0; r < 9; r++)
    for (var c = 0; c < 9; c++)
    {
      var val = _logic.Board[r, c];
      var cell = _sudokuCells[r, c];
      DetachCellHandlers(cell);
      if (val != 0)
      {
        cell.Text = val.ToString();
        cell.ReadOnly = true;
        cell.ForeColor = Color.FromArgb(30, 41, 59);
        cell.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
        _givenCells.Add((r, c));
      }
      else
      {
        cell.Text = string.Empty;
        cell.ReadOnly = false;
        cell.ForeColor = UiTheme.Primary;
        cell.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
      }
      AttachCellHandlers(cell);
      ResetCellBackground(r, c, cell);
    }
  }

  private void CreateSudokuGrid()
  {
    const int borderThickness = 4;
    var boardSize = 9 * CellSize + 9 + 9 + borderThickness * 2;
    pnlBoard.BackColor = Color.FromArgb(30, 58, 95);
    pnlBoard.Size = new Size(boardSize, boardSize);
    pnlBoard.Controls.Clear();

    for (var row = 0; row < 9; row++)
    {
      for (var col = 0; col < 9; col++)
      {
        var txt = new TextBox
        {
          Size = new Size(CellSize, CellSize),
          Multiline = true,
          Font = new Font("Segoe UI", 24F, FontStyle.Bold),
          ForeColor = UiTheme.Primary,
          TextAlign = HorizontalAlignment.Center,
          MaxLength = 1,
          BorderStyle = BorderStyle.None,
          Tag = new[] { row, col }
        };

        txt.Location = new Point(
          col * CellSize + col + col / 3 * 3 + borderThickness,
          row * CellSize + row + row / 3 * 3 + borderThickness);

        ResetCellBackground(row, col, txt);
        AttachCellHandlers(txt);
        _sudokuCells[row, col] = txt;
        pnlBoard.Controls.Add(txt);
      }
    }

    pnlSidebar.Location = new Point(pnlBoard.Right + 24, pnlBoard.Top);
    ClientSize = new Size(pnlSidebar.Right + 24, Math.Max(pnlBoard.Bottom, pnlSidebar.Bottom) + 24);
  }

  private void AttachCellHandlers(TextBox txt)
  {
    txt.Enter += Cell_Enter;
    txt.KeyPress += Txt_KeyPress;
    txt.TextChanged += Cell_TextChanged;
    txt.Click += Cell_Click;
  }

  private void DetachCellHandlers(TextBox txt)
  {
    txt.Enter -= Cell_Enter;
    txt.KeyPress -= Txt_KeyPress;
    txt.TextChanged -= Cell_TextChanged;
    txt.Click -= Cell_Click;
  }

  private void Cell_Click(object? sender, EventArgs e)
  {
    if (sender is TextBox tb && !tb.ReadOnly)
      tb.SelectAll();
  }

  private void Cell_Enter(object? sender, EventArgs e)
  {
    if (sender is not TextBox currentTxt || currentTxt.Tag is not int[] pos)
      return;

    if (!currentTxt.ReadOnly)
      currentTxt.SelectAll();
    ApplyHighlight(pos[0], pos[1]);
  }

  private void ApplyHighlight(int row, int col)
  {
    var targetValue = _sudokuCells[row, col].Text;

    for (var r = 0; r < 9; r++)
    {
      for (var c = 0; c < 9; c++)
      {
        if (_solvedCells.Contains((r, c)))
        {
          _sudokuCells[r, c].BackColor = _colorSolved;
          continue;
        }

        var isAlt = (r / 3 + c / 3) % 2 != 0;
        if (r == row || c == col)
          _sudokuCells[r, c].BackColor = _colorHighlightLine;
        else
          _sudokuCells[r, c].BackColor = isAlt ? _colorAltBack : _colorDefaultBack;

        if (!string.IsNullOrEmpty(targetValue) && _sudokuCells[r, c].Text == targetValue)
          _sudokuCells[r, c].BackColor = _colorSameNumber;
      }
    }

    _sudokuCells[row, col].BackColor = _colorFocus;
  }

  private void ResetCellBackground(int row, int col, TextBox? txt = null)
  {
    txt ??= _sudokuCells[row, col];
    if (_solvedCells.Contains((row, col)))
    {
      txt.BackColor = _colorSolved;
      return;
    }

    var isAlt = (row / 3 + col / 3) % 2 != 0;
    txt.BackColor = isAlt ? _colorAltBack : _colorDefaultBack;
  }

  private async void Cell_TextChanged(object? sender, EventArgs e)
  {
    if (_suppressTextChange || _gameEnded || _awaitingGameResult)
      return;

    if (sender is not TextBox txt || txt.Tag is not int[] pos)
      return;

    var r = pos[0];
    var c = pos[1];
    if (_givenCells.Contains((r, c)) || _solvedCells.Contains((r, c)))
      return;

    if (!int.TryParse(txt.Text, out var num))
    {
      _logic.Board[r, c] = 0;
      return;
    }

    if (!_logic.IsCorrectMove(r, c, num))
    {
      RejectWrongMove(txt, r, c);
      return;
    }

    _logic.Board[r, c] = num;
    _solvedCells.Add((r, c));
    txt.ReadOnly = true;
    txt.ForeColor = UiTheme.Primary;
    txt.BackColor = _colorSolved;
    _myCorrectCells = _logic.CountCorrectPlayerCells(_initialPuzzle);
    UpdateProgressLabels();
    ApplyHighlight(r, c);

    try
    {
      await _client.SendAsync(new NetworkMessage
      {
        Type = MessageType.CellUpdate,
        Row = r,
        Col = c,
        Value = num
      });
    }
    catch
    {
      // ignore transient network errors
    }

    if (_logic.IsBoardComplete())
      await TryCompleteGameAsync();
  }

  private void RejectWrongMove(TextBox txt, int row, int col)
  {
    _mistakeCount++;
    UpdateMistakeLabel();

    _suppressTextChange = true;
    DetachCellHandlers(txt);
    txt.Text = string.Empty;
    AttachCellHandlers(txt);
    _logic.Board[row, col] = 0;
    _suppressTextChange = false;

    txt.BackColor = Color.FromArgb(254, 226, 226);
    lblGameStatus.Text = $"● Số sai — {_mistakeCount}/{MaxMistakes} lỗi!";
    lblGameStatus.ForeColor = UiTheme.Danger;

    if (_mistakeCount >= MaxMistakes)
    {
      _ = TryEliminateByMistakesAsync();
      return;
    }

    var revertTimer = new System.Windows.Forms.Timer { Interval = 350 };
    revertTimer.Tick += (_, _) =>
    {
      revertTimer.Stop();
      revertTimer.Dispose();
      ResetCellBackground(row, col, txt);
      if (!_gameEnded && !_awaitingGameResult)
      {
        lblGameStatus.Text = "● Đang chơi...";
        lblGameStatus.ForeColor = UiTheme.Success;
      }
    };
    revertTimer.Start();
  }

  private void UpdateMistakeLabel()
  {
    lblMistakes.Text = $"{_mistakeCount} / {MaxMistakes}";
    lblMistakes.ForeColor = _mistakeCount >= MaxMistakes - 1 ? UiTheme.Danger : UiTheme.Text;
  }

  private async Task TryEliminateByMistakesAsync()
  {
    if (_gameEnded || _awaitingGameResult)
      return;

    _awaitingGameResult = true;
    _gameEnded = true;
    _stopwatch.Stop();
    _timer.Stop();
    lblGameStatus.Text = "● Đang xác nhận kết quả...";
    lblGameStatus.ForeColor = UiTheme.Danger;
    DisableBoardInput();

    try
    {
      await _client.SendAsync(new NetworkMessage
      {
        Type = MessageType.TooManyMistakes,
        MyElapsedMs = _stopwatch.ElapsedMilliseconds
      });
    }
    catch (Exception ex)
    {
      _awaitingGameResult = false;
      MessageBox.Show($"Không gửi được kết quả: {ex.Message}", "Lỗi",
        MessageBoxButtons.OK, MessageBoxIcon.Error);
      ShowLocalLossDialog($"{_opponentName} đã THẮNG vì bạn sai quá 3 lỗi.");
    }
  }

  private void ShowLocalLossDialog(string reason)
  {
    if (_endGameHandled)
      return;

    _endGameHandled = true;
    _awaitingGameResult = false;
    _gameEnded = true;
    _stopwatch.Stop();
    _timer.Stop();
    DisableBoardInput();
    lblGameStatus.Text = "● Bạn thua!";
    lblGameStatus.ForeColor = UiTheme.Danger;

    MessageBox.Show(
      $"Bạn đã THUA.\n{_opponentName} đã THẮNG.\n\nLý do: {reason}",
      "Kết thúc trận đấu",
      MessageBoxButtons.OK,
      MessageBoxIcon.Warning);
    Close();
  }

  private void ShowLocalWinDialog(string reason)
  {
    if (_endGameHandled)
      return;

    _endGameHandled = true;
    _awaitingGameResult = false;
    _gameEnded = true;
    _stopwatch.Stop();
    _timer.Stop();
    DisableBoardInput();
    lblGameStatus.Text = "● Bạn thắng!";
    lblGameStatus.ForeColor = UiTheme.Success;

    MessageBox.Show(
      $"Chúc mừng! Bạn đã THẮNG.\n{_opponentName} đã THUA.\n\nLý do: {reason}",
      "Kết thúc trận đấu",
      MessageBoxButtons.OK,
      MessageBoxIcon.Information);
    Close();
  }

  private void DisableBoardInput()
  {
    for (var r = 0; r < 9; r++)
    for (var c = 0; c < 9; c++)
    {
      if (!_givenCells.Contains((r, c)))
        _sudokuCells[r, c].ReadOnly = true;
    }
    btnSurrender.Enabled = false;
  }

  private async Task TryCompleteGameAsync()
  {
    if (_gameEnded || _awaitingGameResult)
      return;

    _awaitingGameResult = true;
    _stopwatch.Stop();
    lblGameStatus.Text = "● Đang xác nhận kết quả...";
    lblGameStatus.ForeColor = UiTheme.Primary;

    try
    {
      await _client.SendAsync(new NetworkMessage
      {
        Type = MessageType.GameComplete,
        Puzzle = _logic.BuildFullBoard(),
        MyElapsedMs = _stopwatch.ElapsedMilliseconds
      });
    }
    catch (Exception ex)
    {
      _awaitingGameResult = false;
      _stopwatch.Start();
      lblGameStatus.Text = "● Đang chơi...";
      lblGameStatus.ForeColor = UiTheme.Success;
      MessageBox.Show($"Không gửi được kết quả: {ex.Message}", "Lỗi",
        MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }

  private void Txt_KeyPress(object? sender, KeyPressEventArgs e)
  {
    if (sender is TextBox tb && tb.ReadOnly)
    {
      e.Handled = true;
      return;
    }

    if (!char.IsDigit(e.KeyChar) || e.KeyChar == '0')
    {
      if (e.KeyChar != (char)8)
        e.Handled = true;
    }
  }

  private void UpdateTimerLabels()
  {
    lblMyTime.Text = FormatTime(_stopwatch.Elapsed);
  }

  private void UpdateProgressLabels()
  {
    lblMyProgress.Text = $"Bạn: {_myCorrectCells}/{_totalEmptyCells} ô";
    lblOpponentProgress.Text = $"Đối thủ: {_opponentCorrectCells}/{_totalEmptyCells} ô";
    progressMy.Value = Math.Min(progressMy.Maximum, _myCorrectCells);
    progressOpponent.Value = Math.Min(progressOpponent.Maximum, _opponentCorrectCells);
  }

  private static string FormatTime(TimeSpan time) =>
    $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";

  private void OnMessageReceived(NetworkMessage message)
  {
    if (IsDisposed)
      return;

    BeginInvoke(() =>
    {
      switch (message.Type)
      {
        case MessageType.OpponentProgress:
          _opponentCorrectCells = message.CorrectCells;
          if (message.TotalCells > 0)
            _totalEmptyCells = message.TotalCells;
          UpdateProgressLabels();
          break;
        case MessageType.GameOver:
          EndGame(message);
          break;
        case MessageType.Error:
          if (_awaitingGameResult && !_gameEnded)
          {
            _awaitingGameResult = false;
            _stopwatch.Start();
            _timer.Start();
            lblGameStatus.Text = "● Đang chơi...";
            lblGameStatus.ForeColor = UiTheme.Success;
            MessageBox.Show(message.Message ?? "Bảng chưa hợp lệ.", "Chưa thắng",
              MessageBoxButtons.OK, MessageBoxIcon.Warning);
          }
          else if (_awaitingGameResult && _gameEnded)
          {
            ShowLocalLossDialog(message.Message ?? "Server không xác nhận được kết quả.");
          }
          break;
        case MessageType.OpponentDisconnected:
          if (_endGameHandled)
            break;

          _endGameHandled = true;
          _gameEnded = true;
          _awaitingGameResult = false;
          _stopwatch.Stop();
          _timer.Stop();
          DisableBoardInput();
          lblGameStatus.Text = "● Đối thủ đã rời trận";
          lblGameStatus.ForeColor = UiTheme.Danger;
          MessageBox.Show(message.Message ?? "Đối thủ đã rời trận đấu.", "Kết thúc",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
          Close();
          break;
      }
    });
  }

  private void EndGame(NetworkMessage message)
  {
    if (_endGameHandled)
      return;

    _endGameHandled = true;
    _awaitingGameResult = false;
    _gameEnded = true;
    _stopwatch.Stop();
    _timer.Stop();
    DisableBoardInput();

    var won = message.WinnerName == _client.Username;
    lblGameStatus.Text = won ? "● Bạn thắng!" : "● Bạn thua!";
    lblGameStatus.ForeColor = won ? UiTheme.Success : UiTheme.Danger;

    _client.ApplyStats(message.Wins, message.Losses, message.TotalGames);

    var winnerName = message.WinnerName ?? "—";
    var loserName = message.LoserName ?? message.OpponentName ?? _opponentName;
    var reason = message.Message ?? "Kết thúc trận đấu.";

    MessageBox.Show(
      won
        ? $"Chúc mừng! Bạn đã THẮNG.\n{loserName} đã THUA.\n\nLý do: {reason}\n\nThống kê: {message.Wins} thắng / {message.Losses} thua / {message.TotalGames} trận."
        : $"Bạn đã THUA.\n{winnerName} đã THẮNG.\n\nLý do: {reason}\n\nThống kê: {message.Wins} thắng / {message.Losses} thua / {message.TotalGames} trận.",
      "Kết thúc trận đấu",
      MessageBoxButtons.OK,
      won ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

    Close();
  }

  private void OnDisconnected()
  {
    if (IsDisposed || _gameEnded)
      return;

    BeginInvoke(() =>
    {
      MessageBox.Show("Mất kết nối với server.", "Ngắt kết nối",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
      Close();
    });
  }

  private async void btnSurrender_Click(object sender, EventArgs e)
  {
    if (_gameEnded || _awaitingGameResult)
      return;

    var result = MessageBox.Show(
      "Bạn có chắc muốn đầu hàng?",
      "Xác nhận",
      MessageBoxButtons.YesNo,
      MessageBoxIcon.Warning);

    if (result != DialogResult.Yes)
      return;

    _awaitingGameResult = true;
    _gameEnded = true;
    _stopwatch.Stop();
    _timer.Stop();
    DisableBoardInput();
    lblGameStatus.Text = "● Đang xác nhận đầu hàng...";
    lblGameStatus.ForeColor = UiTheme.Danger;

    try
    {
      await _client.SendAsync(new NetworkMessage
      {
        Type = MessageType.Surrender,
        MyElapsedMs = _stopwatch.ElapsedMilliseconds
      });
    }
    catch
    {
      // server may already be gone
    }
  }

  private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    if (_endGameHandled)
      return;

    try
    {
      var task = _client.SendAsync(new NetworkMessage { Type = MessageType.LeaveGame });
      task.Wait(TimeSpan.FromSeconds(2));
    }
    catch
    {
      // connection may already be gone
    }
  }

  private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
  {
    _timer.Stop();
    _client.MessageReceived -= OnMessageReceived;
    _client.Disconnected -= OnDisconnected;
  }
}
