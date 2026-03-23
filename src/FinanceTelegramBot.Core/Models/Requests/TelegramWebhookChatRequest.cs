using System.Text.Json.Serialization;

namespace FinanceTelegramBot.Core.Models.Requests;

public sealed class TelegramWebhookChatRequest
{
    [JsonPropertyName("id")]
    public long? Id { get; init; }
}
