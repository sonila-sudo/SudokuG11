using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sudoku.client
{
    public partial class LoginForm : Form
    {
        // Sử dụng đường dẫn trực tiếp (Fully Qualified Name) để không sợ lỗi namespace
        private Sudoku.client.Network.NetworkManager _networkManager;

        public LoginForm()
        {
            InitializeComponent();
            lblStatus.Text = "Sẵn sàng kết nối.";
            lblStatus.ForeColor = Color.Black;

            _networkManager = new Sudoku.client.Network.NetworkManager();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên người chơi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ip = string.IsNullOrWhiteSpace(txtIP.Text) ? "127.0.0.1" : txtIP.Text.Trim();

            if (!int.TryParse(txtPort.Text.Trim(), out int port))
            {
                MessageBox.Show("Cổng Port phải là một số nguyên hợp lệ!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                lblStatus.Text = "Đang kết nối tới server...";
                lblStatus.ForeColor = Color.Blue;
                lblStatus.Refresh();

                bool isConnected = await _networkManager.ConnectAsync(ip, port);

                if (isConnected)
                {
                    lblStatus.Text = "Đang tìm kiếm trận đấu...";
                    lblStatus.ForeColor = Color.Green;

                    btnConnect.Enabled = false;
                    txtName.Enabled = false;
                    txtIP.Enabled = false;
                    txtPort.Enabled = false;

                    if (_networkManager.Handler != null)
                    {
                        // Sửa lỗi gạch đỏ CS0123: Ép delegate nhận đúng tham số dạng chuỗi string
                        _networkManager.Handler.OnMatchStart -= ThucHienChuyenForm;
                        _networkManager.Handler.OnMatchStart += ThucHienChuyenForm;
                    }

                    string playerName = txtName.Text.Trim();
                    _networkManager.SendLogin(playerName);
                }
                else
                {
                    lblStatus.Text = "Kết nối thất bại!";
                    lblStatus.ForeColor = Color.Red;
                    MessageBox.Show("Không thể kết nối tới Server. Hãy kiểm tra lại IP/Port!", "Lỗi Kết Nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi hệ thống!";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnConnect.Enabled = true;
            }
        }

        private void ThucHienChuyenForm(string rawMessage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ThucHienChuyenForm(rawMessage)));
                return;
            }

            this.Hide();

            GameForm gameBoard = new GameForm();
            gameBoard.FormClosed += (s, args) => this.Close();
            gameBoard.Show();
        }
    }
}