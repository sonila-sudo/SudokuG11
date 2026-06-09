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
            lblTimer = new Label();
            lblMyName = new Label();
            lblOpponentName = new Label();
            btnSurrender = new Button();
            lblGameStatus = new Label();
            SuspendLayout();
            // 
            // pnlBoard
            // 
            pnlBoard.Location = new Point(29, 36);
            pnlBoard.Name = "pnlBoard";
            pnlBoard.Size = new Size(450, 450);
            pnlBoard.TabIndex = 0;
            pnlBoard.Paint += pnlBoard_Paint;
            // 
            // lblTimer
            // 
            lblTimer.AutoSize = true;
            lblTimer.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTimer.Location = new Point(523, 133);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(120, 54);
            lblTimer.TabIndex = 0;
            lblTimer.Text = "05:00";
            lblTimer.Click += label1_Click;
            // 
            // lblMyName
            // 
            lblMyName.AutoSize = true;
            lblMyName.Location = new Point(550, 25);
            lblMyName.Name = "lblMyName";
            lblMyName.Size = new Size(74, 20);
            lblMyName.TabIndex = 0;
            lblMyName.Text = "Bạn: [Tên]";
            // 
            // lblOpponentName
            // 
            lblOpponentName.AutoSize = true;
            lblOpponentName.Location = new Point(526, 277);
            lblOpponentName.Name = "lblOpponentName";
            lblOpponentName.Size = new Size(98, 20);
            lblOpponentName.TabIndex = 1;
            lblOpponentName.Text = "Đối thủ: [Tên]";
            // 
            // btnSurrender
            // 
            btnSurrender.BackColor = Color.Salmon;
            btnSurrender.FlatAppearance.MouseOverBackColor = Color.MistyRose;
            btnSurrender.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSurrender.ForeColor = Color.Black;
            btnSurrender.Location = new Point(0, 516);
            btnSurrender.Name = "btnSurrender";
            btnSurrender.Size = new Size(150, 31);
            btnSurrender.TabIndex = 0;
            btnSurrender.Text = "Đầu hàng / Thoát";
            btnSurrender.UseVisualStyleBackColor = false;
            btnSurrender.Click += btnSurrender_Click;
            // 
            // lblGameStatus
            // 
            lblGameStatus.AutoSize = true;
            lblGameStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblGameStatus.ForeColor = Color.Red;
            lblGameStatus.Location = new Point(510, 398);
            lblGameStatus.Name = "lblGameStatus";
            lblGameStatus.Size = new Size(161, 20);
            lblGameStatus.TabIndex = 0;
            lblGameStatus.Text = "Trận đấu đang diễn ra";
            lblGameStatus.Click += lblGameStatus_Click;
            // 
            // GameForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientInactiveCaption;
            ClientSize = new Size(709, 546);
            Controls.Add(lblGameStatus);
            Controls.Add(btnSurrender);
            Controls.Add(lblOpponentName);
            Controls.Add(lblMyName);
            Controls.Add(lblTimer);
            Controls.Add(pnlBoard);
            Name = "GameForm";
            Text = "Form2";
            FormClosed += GameForm_FormClosed;
            Load += GameForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PageSetupDialog pageSetupDialog1;
        private Panel pnlBoard;
        private Label lblTimer;
        private Label lblMyName;
        private Label lblOpponentName;
        private Button btnSurrender;
        private Label lblGameStatus;
    }
}