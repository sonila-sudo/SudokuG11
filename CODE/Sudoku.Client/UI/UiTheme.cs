namespace Sudoku.Client.UI;

public static class UiTheme
{
  public static readonly Color Background = Color.FromArgb(241, 245, 249);
  public static readonly Color Header = Color.FromArgb(30, 58, 95);
  public static readonly Color Card = Color.White;
  public static readonly Color Primary = Color.FromArgb(37, 99, 235);
  public static readonly Color PrimaryHover = Color.FromArgb(29, 78, 216);
  public static readonly Color Secondary = Color.FromArgb(226, 232, 240);
  public static readonly Color SecondaryHover = Color.FromArgb(203, 213, 225);
  public static readonly Color Text = Color.FromArgb(15, 23, 42);
  public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);
  public static readonly Color Success = Color.FromArgb(22, 163, 74);
  public static readonly Color Danger = Color.FromArgb(220, 38, 38);
  public static readonly Color Border = Color.FromArgb(226, 232, 240);
  public static readonly Color InputBg = Color.FromArgb(248, 250, 252);

  public static readonly Font TitleFont = new("Segoe UI", 22F, FontStyle.Bold);
  public static readonly Font SubtitleFont = new("Segoe UI", 10F, FontStyle.Regular);
  public static readonly Font LabelFont = new("Segoe UI Semibold", 9F, FontStyle.Bold);
  public static readonly Font BodyFont = new("Segoe UI", 10F, FontStyle.Regular);
  public static readonly Font ButtonFont = new("Segoe UI Semibold", 10F, FontStyle.Bold);

  public static void ApplyFormStyle(Form form)
  {
    form.BackColor = Background;
    form.Font = BodyFont;
    form.ForeColor = Text;
  }

  public static Panel CreateCard(Rectangle bounds)
  {
    var card = new Panel
    {
      Bounds = bounds,
      BackColor = Card,
      Padding = new Padding(24)
    };
    card.Paint += (_, e) =>
    {
      using var pen = new Pen(Border, 1);
      e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
    };
    return card;
  }

  public static void StylePrimaryButton(Button button)
  {
    button.FlatStyle = FlatStyle.Flat;
    button.FlatAppearance.BorderSize = 0;
    button.BackColor = Primary;
    button.ForeColor = Color.White;
    button.Font = ButtonFont;
    button.Cursor = Cursors.Hand;
    button.MouseEnter += (_, _) => button.BackColor = PrimaryHover;
    button.MouseLeave += (_, _) => button.BackColor = Primary;
  }

  public static void StyleSecondaryButton(Button button)
  {
    button.FlatStyle = FlatStyle.Flat;
    button.FlatAppearance.BorderSize = 0;
    button.BackColor = Secondary;
    button.ForeColor = Text;
    button.Font = ButtonFont;
    button.Cursor = Cursors.Hand;
    button.MouseEnter += (_, _) => button.BackColor = SecondaryHover;
    button.MouseLeave += (_, _) => button.BackColor = Secondary;
  }

  public static void StyleDangerButton(Button button)
  {
    button.FlatStyle = FlatStyle.Flat;
    button.FlatAppearance.BorderSize = 0;
    button.BackColor = Danger;
    button.ForeColor = Color.White;
    button.Font = ButtonFont;
    button.Cursor = Cursors.Hand;
  }

  public static void StyleTextBox(TextBox textBox)
  {
    textBox.BorderStyle = BorderStyle.FixedSingle;
    textBox.BackColor = InputBg;
    textBox.Font = BodyFont;
    textBox.ForeColor = Text;
  }

  public static void StyleComboBox(ComboBox comboBox)
  {
    comboBox.FlatStyle = FlatStyle.Flat;
    comboBox.BackColor = InputBg;
    comboBox.Font = BodyFont;
    comboBox.ForeColor = Text;
  }

  public static Label CreateFieldLabel(string text, Point location)
  {
    return new Label
    {
      AutoSize = true,
      Text = text,
      Font = LabelFont,
      ForeColor = TextMuted,
      Location = location
    };
  }
}
