namespace FinanceTelegramBot.Core.Options;

public sealed class TelegramBotOptions
{
    public const string SectionName = "TelegramBot";

    public string BaseUrl { get; set; } = "https://api.telegram.org";

    public string BotToken { get; set; } = string.Empty;

    public string? DefaultChatId { get; set; }
}
