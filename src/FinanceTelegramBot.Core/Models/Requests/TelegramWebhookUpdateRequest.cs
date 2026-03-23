using System.Text.Json.Serialization;

namespace FinanceTelegramBot.Core.Models.Requests;

public sealed class TelegramWebhookUpdateRequest
{
    [JsonPropertyName("update_id")]
    public long? UpdateId { get; init; }

    [JsonPropertyName("message")]
    public TelegramWebhookMessageRequest? Message { get; init; }
}
