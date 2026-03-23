namespace FinanceTelegramBot.Core.Models;

public readonly record struct DailyPriceQuote(
    DateTimeOffset TimestampUtc,
    decimal YesterdayClose,
    decimal CurrentPrice);
