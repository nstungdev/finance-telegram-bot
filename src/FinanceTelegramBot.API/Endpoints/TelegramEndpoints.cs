using FinanceTelegramBot.Core.HttpClients;
using FinanceTelegramBot.Core.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FinanceTelegramBot.API.Endpoints;

public static class TelegramEndpoints
{
    public static IEndpointRouteBuilder MapTelegramEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/telegram").WithTags("Telegram");

        group.MapPost("/telegram/send", async Task<Results<Ok<TelegramMessage>, BadRequest<string>>> (
            TelegramSendMessageInput input,
            ITelegramBotClient telegramBotClient,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var message = await telegramBotClient.SendMessageAsync(input.Text, input.ChatId, cancellationToken);
                return TypedResults.Ok(message);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        })
        .WithName("SendTelegramMessage");

        return endpoints;
    }
}
