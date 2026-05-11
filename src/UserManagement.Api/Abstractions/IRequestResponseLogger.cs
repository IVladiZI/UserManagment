namespace UserManagement.Api.Abstractions;

public interface IRequestResponseLogger
{
    Task BeginCaptureAsync(HttpContext context);
    Task LogErrorAsync(HttpContext context, Exception ex, DateTimeOffset startedAt);
    Task EndCaptureAsync(HttpContext context);
}