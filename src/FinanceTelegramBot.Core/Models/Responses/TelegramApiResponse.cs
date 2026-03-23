using System.Text.Json.Serialization;

namespace FinanceTelegramBot.Core.Models.Responses;

public sealed record TelegramApiResponse<T>(
    [property: JsonPropertyName("ok")] bool Ok,
    [property: JsonPropertyName("result")] T? Result,
    [property: JsonPropertyName("description")] string? Description);
