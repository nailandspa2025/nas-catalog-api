namespace BuildingBlocks.RateLimiting;

public class RateLimitOptions
{
    public const string SectionName = "RateLimiting";

    public bool Enabled { get; set; } = false;

    public PolicyOptions PublicApi { get; set; } = new()
    {
        PermitLimit = 100,
        WindowSeconds = 60
    };

    public PolicyOptions Login { get; set; } = new()
    {
        PermitLimit = 10,
        WindowSeconds = 60
    };

    public PolicyOptions Admin { get; set; } = new()
    {
        PermitLimit = 50,
        WindowSeconds = 60
    };
}

public class PolicyOptions
{
    public int PermitLimit { get; set; }

    public int WindowSeconds { get; set; }

    public int QueueLimit { get; set; } = 0;
}