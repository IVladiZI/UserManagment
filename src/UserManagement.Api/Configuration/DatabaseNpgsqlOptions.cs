namespace UserManagement.Api.Configuration;

public sealed class DatabaseNpgsqlOptions
{
    public const string SectionName = "Database:Npgsql";

    // Npgsql/EF Core settings
    public int CommandTimeoutSeconds { get; set; } = 15;
    public bool EnableRetryOnFailure { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
    public int MaxRetryDelaySeconds { get; set; } = 5;

    // Diagnostics
    public bool EnableDetailedErrors { get; set; } = false;
    public bool EnableSensitiveDataLogging { get; set; } = false;
}