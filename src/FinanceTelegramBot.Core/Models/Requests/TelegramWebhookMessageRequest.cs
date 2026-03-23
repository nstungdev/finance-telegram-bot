using System.Text.Json.Serialization;

namespace FinanceTelegramBot.Core.Models.Requests;

public sealed class TelegramWebhookMessageRequest
{
    [JsonPropertyName("message_id")]
    public long? MessageId { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }

    [JsonPropertyName("chat")]
    public TelegramWebhookChatRequest? Chat { get; init; }
}
