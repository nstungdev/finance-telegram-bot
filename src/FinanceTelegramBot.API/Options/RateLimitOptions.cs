namespace FinanceTelegramBot.API.Options;

public sealed class RateLimitPolicyOptions
{
    public int PermitLimit { get; set; }
    public int WindowSeconds { get; set; }
    public int QueueLimit { get; set; }
}

public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    public RateLimitPolicyOptions Fixed { get; set; } = new() { PermitLimit = 30, WindowSeconds = 60, QueueLimit = 0 };
    public RateLimitPolicyOptions Webhook { get; set; } = new() { PermitLimit = 60, WindowSeconds = 60, QueueLimit = 0 };
}
