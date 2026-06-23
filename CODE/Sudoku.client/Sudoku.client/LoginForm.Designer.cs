// 1. SỬA CHỖ NÀY: Thay đổi namespace thành của dự án Sudoku.client mới
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Sudoku.client
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            txtIP = new TextBox();
            txtPort = new TextBox();
            txtName = new TextBox();
            btnConnect = new Button();
            lblStatus = new Label();
            lblTitle = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(86, 143);
            label1.Name = "label1";
            label1.Size = new Size(69, 20);
            label1.TabIndex = 0;
            label1.Text = "IP Server:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(117, 186);
            label2.Name = "label2";
            label2.Size = new Size(38, 20);
            label2.TabIndex = 2;
            label2.Text = "Port:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(45, 232);
            label3.Name = "label3";
            label3.Size = new Size(110, 20);
            label3.TabIndex = 4;
            label3.Text = "Tên người chơi:";
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
            // 2. SỬA CHỖ NÀY: Thay "8080" thành cổng của Server (Ví dụ: "9999" trùng với NetworkManager)
            txtPort.Text = "9999";
            // 
            // txtName
            // 
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.Location = new Point(175, 230);
            txtName.Name = "txtName";
            txtName.Size = new Size(125, 27);
            txtName.TabIndex = 7;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.LightCyan;
            btnConnect.FlatAppearance.MouseOverBackColor = Color.Azure;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Font = new System.Drawing.Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnConnect.Location = new Point(175, 357);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(369, 65);
            btnConnect.TabIndex = 8;
            btnConnect.Text = "Tìm Trận / Match";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += button1_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
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
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(3, 9);
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
            Controls.Add(btnConnect);
            Controls.Add(txtName);
            Controls.Add(txtPort);
            Controls.Add(txtIP);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Cursor = Cursors.Hand;
            Name = "LoginForm";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtIP;
        private TextBox txtPort;
        private TextBox txtName;
        private Button btnConnect;
        private Label lblStatus;
        private Label lblTitle;
    }
}