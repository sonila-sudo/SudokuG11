using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Sudoku.Server.Services;
using Sudoku.Server.Network;
using Sudoku.Server.Models;

namespace Sudoku.Server
{
    class Program
    {
        // Tạm thời COMMENT hoặc XÓA bỏ chế độ ẩn ShowWindow để chúng ta dễ kiểm soát luồng chạy
        /*
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        const int SW_HIDE = 0;
        */

        // Đổi hàm Main thành async Task để chạy bất đồng bộ đồng bộ với TcpServer của bạn
        static async Task Main(string[] args)
        {
            // Để hiện cửa sổ theo dõi log, không chạy ngầm giấu đi nữa
            // IntPtr handle = GetConsoleWindow();
            // ShowWindow(handle, SW_HIDE);

            Console.OutputEncoding = System.Text.Encoding.UTF8; // In chữ tiếng Việt chuẩn chỉnh không lỗi font
            Console.WriteLine("=========================================================");
            Console.WriteLine("[Server] ĐANG KHỞI ĐỘNG HỆ THỐNG SUDOKU MULTIPLAYER...");
            Console.WriteLine("=========================================================");

            // 1. Khởi tạo dịch vụ tạo đề bài Sudoku và quản lý logic game
            SudokuGenerator generator = new SudokuGenerator();
            GameService gameService = new GameService(generator);
            RoomManager roomManager = new RoomManager();

            // Cấu hình chéo liên kết tránh circular dependency giống như cấu trúc của bạn
            roomManager.GameService = gameService;

            // 2. CẤU HÌNH PORT 9999: Đồng bộ 100% với ô Port nhập trên LoginForm của Client
            int port = 9999;

            // 3. Khởi chạy thông qua lớp TcpServer xịn mà bạn đã viết sẵn
            TcpServer server = new TcpServer(port, roomManager, gameService);

            // Kích hoạt lắng nghe, lúc này màn hình đen sẽ hiện rõ trạng thái hoạt động
            await server.StartAsync();
        }
    }
}