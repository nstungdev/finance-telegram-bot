using System.Text.Json.Serialization;

namespace FinanceTelegramBot.Core.Models.Responses;

public sealed record TelegramMessage(
    [property: JsonPropertyName("message_id")] long MessageId,
    [property: JsonPropertyName("date")] long Date,
    [property: JsonPropertyName("text")] string? Text);
