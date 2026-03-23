namespace FinanceTelegramBot.Core.Models.Requests;

public sealed record TelegramSendMessageRequest(string Text, string? ChatId = null);
