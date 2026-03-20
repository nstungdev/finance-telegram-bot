using System;

namespace FinanceTelegramBot.Core.Models.Requests;

public record TelegramSendMessageInput(string Text, string? ChatId = null);
