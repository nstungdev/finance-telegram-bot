using System.Globalization;
using FinanceTelegramBot.Core.HttpClients;
using FinanceTelegramBot.Core.Models;
using FinanceTelegramBot.Core.Models.Requests;
using FinanceTelegramBot.Core.Options;
using Microsoft.Extensions.Options;

namespace FinanceTelegramBot.Core.Services;

public interface ITelegramWebhookService
{
    Task ProcessUpdateAsync(TelegramWebhookUpdateRequest update, CancellationToken cancellationToken = default);
}

public sealed class TelegramWebhookService(
    ITelegramBotClient telegramBotClient,
    ITelegramPriceMessageFormatter priceMessageFormatter,
    IOptions<TelegramBotOptions> telegramBotOptions) : ITelegramWebhookService
{
    public async Task ProcessUpdateAsync(TelegramWebhookUpdateRequest update, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(update.Message?.Text))
        {
            return;
        }

        if (!TryResolveAsset(update.Message.Text, out var asset))
        {
            return;
        }

        var chatId = ResolveTargetChatId(update);
        if (string.IsNullOrWhiteSpace(chatId))
        {
            return;
        }

        var quote = BuildDailyQuote(asset);
        var markdownMessage = priceMessageFormatter.FormatPriceUpdate(asset, quote);

        await telegramBotClient.SendMessageAsync(
            markdownMessage,
            chatId,
            parseMode: "Markdown",
            cancellationToken: cancellationToken);
    }

    private string? ResolveTargetChatId(TelegramWebhookUpdateRequest update)
    {
        if (update.Message?.Chat?.Id is not null)
        {
            return update.Message.Chat.Id.Value.ToString(CultureInfo.InvariantCulture);
        }

        return telegramBotOptions.Value.DefaultChatId;
    }

    private static bool TryResolveAsset(string input, out PriceAsset asset)
    {
        var command = input.Trim();
        if (command.StartsWith('/'))
        {
            command = command[1..];
        }

        var firstToken = command.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(firstToken))
        {
            asset = default;
            return false;
        }

        var normalized = firstToken.Split('@', 2, StringSplitOptions.TrimEntries)[0].ToLowerInvariant();

        switch (normalized)
        {
            case "gold":
                asset = PriceAsset.Gold;
                return true;
            case "usd":
                asset = PriceAsset.Usd;
                return true;
            default:
                asset = default;
                return false;
        }
    }

    private static DailyPriceQuote BuildDailyQuote(PriceAsset asset)
    {
        var now = DateTimeOffset.UtcNow;

        return asset switch
        {
            PriceAsset.Gold => new DailyPriceQuote(
                TimestampUtc: now,
                YesterdayClose: 125_500_000m,
                CurrentPrice: 126_100_000m),
            PriceAsset.Usd => new DailyPriceQuote(
                TimestampUtc: now,
                YesterdayClose: 25_500m,
                CurrentPrice: 25_430m),
            _ => throw new ArgumentOutOfRangeException(nameof(asset), asset, "Unsupported asset type.")
        };
    }
}
