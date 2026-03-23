using FinanceTelegramBot.Core.HttpClients;
using FinanceTelegramBot.Core.Models.Requests;
using FinanceTelegramBot.Core.Models.Responses;
using FinanceTelegramBot.Core.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FinanceTelegramBot.API.Endpoints;

public static class TelegramEndpoints
{
    public static IEndpointRouteBuilder MapTelegramEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/telegram").WithTags("Telegram");

        group.MapPost("/telegram/send", async Task<Results<Ok<TelegramMessage>, BadRequest<string>>> (
            TelegramSendMessageRequest input,
            ITelegramBotClient telegramBotClient,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var message = await telegramBotClient.SendMessageAsync(input.Text, input.ChatId, cancellationToken: cancellationToken);
                return TypedResults.Ok(message);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        })
        .WithName("SendTelegramMessage")
        .RequireRateLimiting("fixed");

        group.MapPost("/telegram/webhook", async Task<Results<Ok, BadRequest<string>>> (
            TelegramWebhookUpdateRequest update,
            ITelegramWebhookService telegramWebhookService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                await telegramWebhookService.ProcessUpdateAsync(update, cancellationToken);

                return TypedResults.Ok();
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        })
        .WithName("TelegramWebhook")
        .WithSummary("Receives Telegram webhook updates and replies for supported commands.")
        .RequireRateLimiting("webhook");

        return endpoints;
    }
}
