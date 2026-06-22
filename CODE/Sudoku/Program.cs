using System;
using System.Windows.Forms;

namespace Sudoku
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Bỏ dòng ApplicationConfiguration bị lỗi đi và dùng 2 dòng chuẩn này:
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Khởi chạy Form đăng nhập đầu tiên như cũ
            Application.Run(new LoginForm());
        }
    }
}