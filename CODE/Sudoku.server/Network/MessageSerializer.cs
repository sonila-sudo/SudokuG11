using Sudoku.Server.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudoku.Server.Network
{
    /// <summary>
    /// Tiện ích serialize/deserialize GameMessage thành JSON string (một dòng).
    /// </summary>
    public static class MessageSerializer
    {
        private static readonly JsonSerializerOptions _opts = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        public static string Serialize(GameMessage msg) =>
            JsonSerializer.Serialize(msg, _opts);

        public static GameMessage? Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try { return JsonSerializer.Deserialize<GameMessage>(json, _opts); }
            catch { return null; }
        }
    }
}
