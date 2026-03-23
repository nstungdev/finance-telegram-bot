using System.Threading.RateLimiting;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Serialization.SystemTextJson;
using FinanceTelegramBot.API;
using FinanceTelegramBot.API.Endpoints;
using FinanceTelegramBot.API.Options;
using FinanceTelegramBot.Core;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var rateLimitOptions = builder.Configuration
    .GetSection(RateLimitOptions.SectionName)
    .Get<RateLimitOptions>() ?? new RateLimitOptions();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", limiter =>
    {
        limiter.PermitLimit = rateLimitOptions.Fixed.PermitLimit;
        limiter.Window = TimeSpan.FromSeconds(rateLimitOptions.Fixed.WindowSeconds);
        limiter.QueueLimit = rateLimitOptions.Fixed.QueueLimit;
    });

    options.AddFixedWindowLimiter("webhook", limiter =>
    {
        limiter.PermitLimit = rateLimitOptions.Webhook.PermitLimit;
        limiter.Window = TimeSpan.FromSeconds(rateLimitOptions.Webhook.WindowSeconds);
        limiter.QueueLimit = rateLimitOptions.Webhook.QueueLimit;
    });
});

builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi, new SourceGeneratorLambdaJsonSerializer<AppJsonSerializerContext>());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var api = app.MapGroup("/api").WithTags("API");

api.MapGet("/health", () => Results.Ok(new HealthResponse("healthy")))
    .WithName("HealthCheck")
    .WithTags("Health");

api.MapTelegramEndpoints();

await app.RunAsync();