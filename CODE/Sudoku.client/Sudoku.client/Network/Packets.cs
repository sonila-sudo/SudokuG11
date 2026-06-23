using System;

namespace Sudoku.client.Network
{
    public abstract class Packet
    {
        public string Type { get; set; }

        protected Packet(string type)
        {
            Type = type;
        }
    }

    public class LoginPacket : Packet
    {
        public string Name { get; set; }

        public LoginPacket() : base("LOGIN")
        {
        }
    }
}