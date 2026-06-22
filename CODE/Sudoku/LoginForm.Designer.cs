namespace Sudoku
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblIP = new Label();
            lblPort = new Label();
            lblPlayer = new Label();
            txtIP = new TextBox();
            txtPort = new TextBox();
            txtName = new TextBox();
            btnMatch = new Button();
            lblStatus = new Label();
            lblTitle = new Label();
            SuspendLayout();
            // 
            // lblIP
            // 
            lblIP.AutoSize = true;
            lblIP.Location = new Point(86, 143);
            lblIP.Name = "lblIP";
            lblIP.Size = new Size(69, 20);
            lblIP.TabIndex = 0;
            lblIP.Text = "IP Server:";
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Location = new Point(117, 186);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(38, 20);
            lblPort.TabIndex = 2;
            lblPort.Text = "Port:";
            // 
            // lblPlayer
            // 
            lblPlayer.AutoSize = true;
            lblPlayer.Location = new Point(45, 232);
            lblPlayer.Name = "lblPlayer";
            lblPlayer.Size = new Size(110, 20);
            lblPlayer.TabIndex = 4;
            lblPlayer.Text = "Tên người chơi:";
            // 
            // txtIP
            // 
            txtIP.BorderStyle = BorderStyle.FixedSingle;
            txtIP.Location = new Point(175, 141);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(125, 27);
            txtIP.TabIndex = 5;
            txtIP.Text = "127.0.0.1";
            // 
            // txtPort
            // 
            txtPort.BorderStyle = BorderStyle.FixedSingle;
            txtPort.Location = new Point(175, 184);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(125, 27);
            txtPort.TabIndex = 6;
            txtPort.Text = "8080";
            // 
            // txtName
            // 
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.Location = new Point(175, 230);
            txtName.Name = "txtName";
            txtName.Size = new Size(125, 27);
            txtName.TabIndex = 7;
            // 
            // btnMatch
            // 
            btnMatch.BackColor = Color.LightCyan;
            btnMatch.FlatAppearance.MouseOverBackColor = Color.Azure;
            btnMatch.FlatStyle = FlatStyle.Flat;
            btnMatch.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnMatch.Location = new Point(175, 357);
            btnMatch.Name = "btnMatch";
            btnMatch.Size = new Size(369, 65);
            btnMatch.TabIndex = 8;
            btnMatch.Text = "Tìm Trận / Match";
            btnMatch.UseVisualStyleBackColor = false;
            btnMatch.Click += button1_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblStatus.ForeColor = Color.Red;
            lblStatus.Location = new Point(291, 450);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(139, 20);
            lblStatus.TabIndex = 9;
            lblStatus.Text = "Đang chờ kết nối...";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(66, 22);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(694, 81);
            lblTitle.TabIndex = 10;
            lblTitle.Text = "SUDOKU MULTIPLAYER";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientInactiveCaption;
            ClientSize = new Size(709, 546);
            Controls.Add(lblTitle);
            Controls.Add(lblStatus);
            Controls.Add(btnMatch);
            Controls.Add(txtName);
            Controls.Add(txtPort);
            Controls.Add(txtIP);
            Controls.Add(lblPlayer);
            Controls.Add(lblPort);
            Controls.Add(lblIP);
            Cursor = Cursors.Hand;
            Name = "LoginForm";
            Text = "Form1";
            Load += LoginForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblIP;
        private Label lblPort;
        private Label lblPlayer;
        private TextBox txtIP;
        private TextBox txtPort;
        private TextBox txtName;
        private Button btnMatch;
        private Label lblStatus;
        private Label lblTitle;
    }
}
