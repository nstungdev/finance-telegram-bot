using System.Globalization;
using System.Text;
using FinanceTelegramBot.Core.Models;

namespace FinanceTelegramBot.Core.Services;

public interface ITelegramPriceMessageFormatter
{
    string FormatPriceUpdate(PriceAsset asset, DailyPriceQuote quote);

    string BuildTemplate(PriceAsset asset);
}

public sealed class TelegramPriceMessageFormatter : ITelegramPriceMessageFormatter
{
    public string FormatPriceUpdate(PriceAsset asset, DailyPriceQuote quote)
    {
        var timestampUtc = quote.TimestampUtc.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss'Z'", CultureInfo.InvariantCulture);
        var yesterdayClose = FormatPriceValue(quote.YesterdayClose, GetUnit(asset));
        var currentPrice = FormatPriceValue(quote.CurrentPrice, GetUnit(asset));
        var trendDisplay = GetTrendDisplay(quote.YesterdayClose, quote.CurrentPrice);

        var builder = new StringBuilder(capacity: 256);
        builder.Append('*').Append(GetAssetEmoji(asset)).Append(' ').Append(GetAssetTitle(asset)).AppendLine(" Price Update*");
        builder.AppendLine();
        builder.Append("- ⏱️ *Timestamp:* ").AppendLine(timestampUtc);
        builder.Append("- 📌 *Yesterday Close:* ").AppendLine(yesterdayClose);
        builder.Append("- 💵 *Current Price:* ").AppendLine(currentPrice);
        builder.Append("- 📊 *Trend:* ").Append(trendDisplay);

        return builder.ToString();
    }

    public string BuildTemplate(PriceAsset asset)
    {
        var (yesterdayPlaceholder, currentPlaceholder, trendPlaceholder) = asset switch
        {
            PriceAsset.Gold => ("{GoldYesterdayCloseVndPerTael}", "{GoldCurrentPriceVndPerTael}", "{GoldTrend}"),
            PriceAsset.Usd => ("{UsdYesterdayCloseVndPerUsd}", "{UsdCurrentPriceVndPerUsd}", "{UsdTrend}"),
            _ => throw new ArgumentOutOfRangeException(nameof(asset), asset, "Unsupported asset type.")
        };

        var builder = new StringBuilder(capacity: 256);
        builder.Append('*').Append(GetAssetEmoji(asset)).Append(' ').Append(GetAssetTitle(asset)).AppendLine(" Price Update*");
        builder.AppendLine();
        builder.AppendLine("- ⏱️ *Timestamp:* {TimestampUtc}");
        builder.Append("- 📌 *Yesterday Close:* ").AppendLine(yesterdayPlaceholder);
        builder.Append("- 💵 *Current Price:* ").AppendLine(currentPlaceholder);
        builder.Append("- 📊 *Trend:* ").Append(trendPlaceholder);

        return builder.ToString();
    }

    private static string GetAssetEmoji(PriceAsset asset) => asset switch
    {
        PriceAsset.Gold => "🥇",
        PriceAsset.Usd => "💱",
        _ => throw new ArgumentOutOfRangeException(nameof(asset), asset, "Unsupported asset type.")
    };

    private static string GetAssetTitle(PriceAsset asset) => asset switch
    {
        PriceAsset.Gold => "Gold",
        PriceAsset.Usd => "USD",
        _ => throw new ArgumentOutOfRangeException(nameof(asset), asset, "Unsupported asset type.")
    };

    private static string GetUnit(PriceAsset asset) => asset switch
    {
        PriceAsset.Gold => "VND/tael",
        PriceAsset.Usd => "VND/USD",
        _ => throw new ArgumentOutOfRangeException(nameof(asset), asset, "Unsupported asset type.")
    };

    private static string GetTrendDisplay(decimal yesterdayClose, decimal currentPrice)
    {
        if (currentPrice > yesterdayClose)
        {
            return "⬆️ increasing";
        }

        if (currentPrice < yesterdayClose)
        {
            return "⬇️ decreasing";
        }

        return "➡️ unchanged";
    }

    private static string FormatPriceValue(decimal value, string unit)
    {
        return string.Create(CultureInfo.InvariantCulture, $"{value:N0} {unit}");
    }
}
