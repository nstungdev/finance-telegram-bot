using System.Text.Json.Serialization;
using FinanceTelegramBot.Core.Models.Requests;
using FinanceTelegramBot.Core.Models.Responses;

namespace FinanceTelegramBot.Core.HttpClients;

[JsonSerializable(typeof(TelegramSendMessagePayload))]
[JsonSerializable(typeof(TelegramApiResponse<TelegramMessage>))]
internal partial class TelegramJsonSerializerContext : JsonSerializerContext
{
}
