using System.Net.Http.Json;
using FinanceTelegramBot.Core.Models.Requests;
using FinanceTelegramBot.Core.Models.Responses;
using FinanceTelegramBot.Core.Options;
using Microsoft.Extensions.Options;

namespace FinanceTelegramBot.Core.HttpClients;

public interface ITelegramBotClient
{
    Task<TelegramMessage> SendMessageAsync(
        string text,
        string? chatId = null,
        string? parseMode = null,
        CancellationToken cancellationToken = default);
}

public sealed class TelegramBotClient(HttpClient httpClient, IOptions<TelegramBotOptions> options) : ITelegramBotClient
{
    private readonly TelegramBotOptions telegramBotOptions = options.Value;
    public async Task<TelegramMessage> SendMessageAsync(
        string text,
        string? chatId = null,
        string? parseMode = null,
        CancellationToken cancellationToken = default)
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
            new TelegramSendMessagePayload(targetChatId, text, parseMode),
            TelegramJsonSerializerContext.Default.TelegramSendMessagePayload,
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
