using System.Text.Json.Serialization;

namespace FinanceTelegramBot.Core.Models.Requests;

internal sealed record TelegramSendMessagePayload(
    [property: JsonPropertyName("chat_id")] string ChatId,
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("parse_mode")] string? ParseMode = null);
