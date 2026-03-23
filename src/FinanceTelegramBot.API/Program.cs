using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Serialization.SystemTextJson;
using FinanceTelegramBot.API;
using FinanceTelegramBot.API.Endpoints;
using FinanceTelegramBot.Core;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi, new SourceGeneratorLambdaJsonSerializer<AppJsonSerializerContext>());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var api = app.MapGroup("/api").WithTags("API");

api.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("HealthCheck")
    .WithTags("Health");

api.MapTelegramEndpoints();

await app.RunAsync();