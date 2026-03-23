using FinanceTelegramBot.Core.HttpClients;
using FinanceTelegramBot.Core.Options;
using FinanceTelegramBot.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace FinanceTelegramBot.Core;

public static class DependencyInjection
{
	public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddOptions<TelegramBotOptions>()
			.Bind(configuration.GetRequiredSection(TelegramBotOptions.SectionName))
			.Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps,
				$"{TelegramBotOptions.SectionName}:BaseUrl must be a valid absolute HTTPS URL.")
			.Validate(options => !string.IsNullOrWhiteSpace(options.BotToken),
				$"{TelegramBotOptions.SectionName}:BotToken is required.")
			.ValidateOnStart();

		services.AddHttpClient<ITelegramBotClient, TelegramBotClient>(client =>
			{
				client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int?>("TelegramBot:TimeoutSeconds") ?? 180);
				client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
			.AddStandardResilienceHandler(ConfigureStandardResilience);

		services.AddSingleton<ITelegramPriceMessageFormatter, TelegramPriceMessageFormatter>();
		services.AddScoped<ITelegramWebhookService, TelegramWebhookService>();

		return services;
	}

	private static void ConfigureStandardResilience(HttpStandardResilienceOptions options)
	{
		options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
		options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(180);
		options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
	}
}
