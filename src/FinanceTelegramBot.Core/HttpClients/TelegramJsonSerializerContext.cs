using System.Text.Json.Serialization;

namespace FinanceTelegramBot.Core.HttpClients;

[JsonSerializable(typeof(TelegramSendMessageRequest))]
[JsonSerializable(typeof(TelegramApiResponse<TelegramMessage>))]
internal partial class TelegramJsonSerializerContext : JsonSerializerContext
{
}
