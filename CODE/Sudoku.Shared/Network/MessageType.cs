namespace Sudoku.Shared.Network;

public enum MessageType
{
    Login,
    LoginResponse,
    Register,
    RegisterResponse,
    CreateRoom,
    CreateRoomResponse,
    JoinRoom,
    JoinRoomResponse,
    FindMatch,
    FindMatchResponse,
    GameStart,
    CellUpdate,
    OpponentProgress,
    TimerSync,
    GameComplete,
    GameOver,
    Surrender,
    OpponentDisconnected,
    Error
}
