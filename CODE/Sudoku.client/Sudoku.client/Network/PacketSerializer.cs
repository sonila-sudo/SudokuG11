using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sudoku.Client.Network
{
    /// <summary>
    /// Chịu trách nhiệm serialize/deserialize gói tin JSON.
    /// Dùng Newtonsoft.Json (đã có sẵn trong project .NET WinForms).
    /// </summary>
    public static class PacketSerializer
    {
        // ─── GỬI: object → JSON string → byte[] ───────────────────────────

        /// <summary>
        /// Đóng gói Action + Payload thành byte[] để gửi qua NetworkStream.
        /// Thêm ký tự '\n' ở cuối để server biết kết thúc 1 gói tin.
        /// </summary>
        public static byte[] Serialize(string action, object payload)
        {
            var message = new Message
            {
                Action = action,
                Payload = payload
            };

            string json = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(json + "\n");
        }

        // ─── NHẬN: string JSON → lấy Action và parse Payload ───────────────

        /// <summary>
        /// Đọc Action từ chuỗi JSON nhận được.
        /// </summary>
        public static string GetAction(string json)
        {
            var obj = JObject.Parse(json);
            return obj["Action"]?.ToString();
        }

        /// <summary>
        /// Parse Payload thành kiểu T cụ thể.
        /// Ví dụ: DeserializePayload<MatchStartPayload>(json)
        /// </summary>
        public static T DeserializePayload<T>(string json)
        {
            var obj = JObject.Parse(json);
            var payload = obj["Payload"];
            return payload.ToObject<T>();
        }
    }
}
