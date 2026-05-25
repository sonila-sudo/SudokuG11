namespace Sudoku
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pageSetupDialog1 = new PageSetupDialog();
            pnlBoard = new Panel();
            lblTime = new Label();
            lblMyName = new Label();
            lblOpponentName = new Label();
            btnSurrender = new Button();
            SuspendLayout();
            // 
            // pnlBoard
            // 
            pnlBoard.Location = new Point(89, 28);
            pnlBoard.Name = "pnlBoard";
            pnlBoard.Size = new Size(450, 450);
            pnlBoard.TabIndex = 0;
            pnlBoard.Paint += pnlBoard_Paint;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTime.Location = new Point(550, 212);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(120, 54);
            lblTime.TabIndex = 0;
            lblTime.Text = "00:00";
            lblTime.Click += label1_Click;
            // 
            // lblMyName
            // 
            lblMyName.AutoSize = true;
            lblMyName.Location = new Point(550, 116);
            lblMyName.Name = "lblMyName";
            lblMyName.Size = new Size(124, 20);
            lblMyName.TabIndex = 0;
            lblMyName.Text = "Bạn: Chưa kết nối";
            // 
            // lblOpponentName
            // 
            lblOpponentName.AutoSize = true;
            lblOpponentName.Location = new Point(545, 366);
            lblOpponentName.Name = "lblOpponentName";
            lblOpponentName.Size = new Size(138, 20);
            lblOpponentName.TabIndex = 1;
            lblOpponentName.Text = "Đối thủ: Đang chờ...";
            // 
            // btnSurrender
            // 
            btnSurrender.Location = new Point(550, 460);
            btnSurrender.Name = "btnSurrender";
            btnSurrender.Size = new Size(137, 31);
            btnSurrender.TabIndex = 0;
            btnSurrender.Text = "Đầu hàng / Thoát";
            btnSurrender.UseVisualStyleBackColor = true;
            // 
            // GameForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(682, 503);
            Controls.Add(btnSurrender);
            Controls.Add(lblOpponentName);
            Controls.Add(lblMyName);
            Controls.Add(lblTime);
            Controls.Add(pnlBoard);
            Name = "GameForm";
            Text = "Form2";
            Load += GameForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PageSetupDialog pageSetupDialog1;
        private Panel pnlBoard;
        private Label lblTime;
        private Label lblMyName;
        private Label lblOpponentName;
        private Button btnSurrender;
    }
}