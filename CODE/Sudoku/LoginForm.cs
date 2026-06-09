namespace Sudoku
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GameForm banCoGame = new GameForm();
            banCoGame.Show();
            this.Hide();
        }
    }
}
