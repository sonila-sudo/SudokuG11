using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Shared.Models;

namespace Sudoku.Shared.Network;

public class NetworkMessage
{
  public MessageType Type { get; set; }
  public bool Success { get; set; } = true;
  public string? Message { get; set; }
  public string? Token { get; set; }
  public string? Username { get; set; }
  public string? Password { get; set; }
  public string? RoomCode { get; set; }
  public int PlayerId { get; set; }
  public int OpponentId { get; set; }
  public string? OpponentName { get; set; }
  public int[,]? Puzzle { get; set; }
  public int[,]? Solution { get; set; }
  public int EmptyCells { get; set; } = 40;
  public int Row { get; set; }
  public int Col { get; set; }
  public int Value { get; set; }
  public long MyElapsedMs { get; set; }
  public long OpponentElapsedMs { get; set; }
  public string? WinnerName { get; set; }
  public int Wins { get; set; }
  public int Losses { get; set; }
  public int TotalGames { get; set; }
  public int CorrectCells { get; set; }
  public int TotalCells { get; set; }
  public string? LoserName { get; set; }
  public List<MatchHistoryEntry>? History { get; set; }
  public List<LeaderboardEntry>? Leaderboard { get; set; }

  private static readonly JsonSerializerOptions Options = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = { new Int2DArrayConverter() }
  };

  public string ToJson() => JsonSerializer.Serialize(this, Options);

  public static NetworkMessage? FromJson(string json) =>
    JsonSerializer.Deserialize<NetworkMessage>(json, Options);
}

internal sealed class Int2DArrayConverter : JsonConverter<int[,]>
{
  public override int[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var list = JsonSerializer.Deserialize<int[][]>(ref reader, options);
    if (list == null || list.Length != 9)
      return null;

    var result = new int[9, 9];
    for (var r = 0; r < 9; r++)
    {
      if (list[r].Length != 9)
        return null;
      for (var c = 0; c < 9; c++)
        result[r, c] = list[r][c];
    }

    return result;
  }

  public override void Write(Utf8JsonWriter writer, int[,] value, JsonSerializerOptions options)
  {
    var rows = new int[9][];
    for (var r = 0; r < 9; r++)
    {
      rows[r] = new int[9];
      for (var c = 0; c < 9; c++)
        rows[r][c] = value[r, c];
    }

    JsonSerializer.Serialize(writer, rows, options);
  }
}
