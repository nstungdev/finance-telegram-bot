using System.Net.Http.Json;
using System.Text.Json.Serialization;
using FinanceTelegramBot.Core.Options;
using Microsoft.Extensions.Options;

namespace FinanceTelegramBot.Core.HttpClients;

public interface ITelegramBotClient
{
    Task<TelegramMessage> SendMessageAsync(string text, string? chatId = null, CancellationToken cancellationToken = default);
}

public sealed class TelegramBotClient(HttpClient httpClient, IOptions<TelegramBotOptions> options) : ITelegramBotClient
{
    private readonly TelegramBotOptions telegramBotOptions = options.Value;
    public async Task<TelegramMessage> SendMessageAsync(string text, string? chatId = null, CancellationToken cancellationToken = default)
    {
        var targetChatId = chatId ?? telegramBotOptions.DefaultChatId;
        if (string.IsNullOrWhiteSpace(targetChatId))
        {
            throw new ArgumentException("A chatId must be provided or TelegramBot:DefaultChatId must be configured.", nameof(chatId));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Message text is required.", nameof(text));
        }

        var requestUri = new Uri($"{telegramBotOptions.BaseUrl}/bot{telegramBotOptions.BotToken}/sendMessage");
        var response = await httpClient.PostAsJsonAsync(
            requestUri,
            new TelegramSendMessageRequest(targetChatId, text),
            TelegramJsonSerializerContext.Default.TelegramSendMessageRequest,
            cancellationToken);

        var payload = await response.Content.ReadFromJsonAsync(
            TelegramJsonSerializerContext.Default.TelegramApiResponseTelegramMessage,
            cancellationToken);
        if (!response.IsSuccessStatusCode || payload is null || !payload.Ok || payload.Result is null)
        {
            var reason = payload?.Description ?? response.ReasonPhrase ?? "Unknown Telegram API error";
            throw new HttpRequestException($"SendMessage failed: {reason}");
        }

        return payload.Result;
    }
}

internal sealed record TelegramSendMessageRequest(
    [property: JsonPropertyName("chat_id")] string ChatId,
    [property: JsonPropertyName("text")] string Text);

public sealed record TelegramApiResponse<T>(
    [property: JsonPropertyName("ok")] bool Ok,
    [property: JsonPropertyName("result")] T? Result,
    [property: JsonPropertyName("description")] string? Description);

public sealed record TelegramMessage(
    [property: JsonPropertyName("message_id")] long MessageId,
    [property: JsonPropertyName("date")] long Date,
    [property: JsonPropertyName("text")] string? Text);
