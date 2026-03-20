using System;
using System.Text.Json.Serialization;
using FinanceTelegramBot.Core.HttpClients;
using FinanceTelegramBot.Core.Models.Requests;

namespace FinanceTelegramBot.API;

[JsonSerializable(typeof(TelegramSendMessageInput))]
[JsonSerializable(typeof(TelegramMessage))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}