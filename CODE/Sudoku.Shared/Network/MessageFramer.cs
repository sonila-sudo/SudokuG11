using System.Net.Sockets;
using System.Text;

namespace Sudoku.Shared.Network;

public static class MessageFramer
{
  private const int MaxMessageBytes = 1024 * 1024;

  public static async Task SendAsync(NetworkStream stream, NetworkMessage message, CancellationToken ct = default)
  {
    var payload = Encoding.UTF8.GetBytes(message.ToJson() + "\n");
    await stream.WriteAsync(payload, ct);
    await stream.FlushAsync(ct);
  }

  public static async Task<NetworkMessage?> ReadAsync(NetworkStream stream, StringBuilder buffer, CancellationToken ct = default)
  {
    while (true)
    {
      var text = buffer.ToString();
      var newlineIndex = text.IndexOf('\n');
      if (newlineIndex >= 0)
      {
        var line = text[..newlineIndex].Trim();
        buffer.Remove(0, newlineIndex + 1);
        if (string.IsNullOrWhiteSpace(line))
          continue;
        return NetworkMessage.FromJson(line);
      }

      var chunk = new byte[4096];
      var read = await stream.ReadAsync(chunk, ct);
      if (read == 0)
        return null;

      if (buffer.Length + read > MaxMessageBytes)
        throw new InvalidOperationException("Message quá lớn.");

      buffer.Append(Encoding.UTF8.GetString(chunk, 0, read));
    }
  }
}
