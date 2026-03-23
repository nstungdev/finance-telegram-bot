using System;
using System.Text.Json.Serialization;
using FinanceTelegramBot.Core.Models.Requests;
using FinanceTelegramBot.Core.Models.Responses;

namespace FinanceTelegramBot.API;

[JsonSerializable(typeof(TelegramSendMessageRequest))]
[JsonSerializable(typeof(TelegramWebhookUpdateRequest))]
[JsonSerializable(typeof(TelegramMessage))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}