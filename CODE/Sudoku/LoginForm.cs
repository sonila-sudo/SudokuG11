namespace Sudoku
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            ApplyModernTheme();
        }

        private void ApplyModernTheme()
        {
            this.BackColor = Color.FromArgb(44, 47, 51);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.ForeColor = Color.White;

                    if (lbl.Font.Size > 20)
                    {
                        lbl.ForeColor = Color.FromArgb(114, 137, 218);
                        lbl.Font = new Font("Segoe UI Black", 28, FontStyle.Bold);
                    }

                    if (lbl.Text.Contains("Đang chờ"))
                    {
                        lbl.ForeColor = Color.FromArgb(250, 173, 20);
                    }
                }

                if (ctrl is TextBox txt)
                {
                    txt.BackColor = Color.FromArgb(35, 39, 42);
                    txt.ForeColor = Color.White;
                    txt.BorderStyle = BorderStyle.FixedSingle;
                    txt.Font = new Font("Segoe UI", 12, FontStyle.Regular);
                    txt.AutoSize = false;
                    txt.Height = 35;
                }

                if (ctrl is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackColor = Color.FromArgb(88, 101, 242);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                    btn.Cursor = Cursors.Hand;
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 110, 248);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GameForm banCoGame = new GameForm();
            banCoGame.Show();
            this.Hide();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
        }
    }
}