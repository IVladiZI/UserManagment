using UserManagement.Api.Abstractions;
using System.Text;

namespace UserManagement.Api.Observability;

public class RequestResponseLogger(ILogger<RequestResponseLogger> logger) : IRequestResponseLogger
{
    private readonly ILogger<RequestResponseLogger> _logger = logger;
    private MemoryStream? _responseBuffer;

    public async Task BeginCaptureAsync(HttpContext context)
    {
        if (context.Request.ContentLength is > 0 && context.Request.Body.CanRead)
        {
            context.Request.EnableBuffering();
        }
        _responseBuffer = new MemoryStream();
        context.Items["__origResponseBody"] = context.Response.Body;
        context.Response.Body = _responseBuffer;
        context.Items["__stopwatch"] = System.Diagnostics.Stopwatch.StartNew();
    }

    public async Task LogErrorAsync(HttpContext context, Exception ex, DateTimeOffset startedAt)
    {
        var endpointName = context.GetEndpoint()?.DisplayName ?? "UnknownEndpoint";

        string? requestBody = null;
        if (context.Request.ContentLength is > 0 && context.Request.Body.CanRead)
        {
            context.Request.Body.Position = 0;
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, 1024, true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        string? responseBody = null;
        if (_responseBuffer is { Length: > 0 })
        {
            _responseBuffer.Position = 0;
            using var rbReader = new StreamReader(_responseBuffer, Encoding.UTF8, false, 1024, true);
            responseBody = await rbReader.ReadToEndAsync();
            _responseBuffer.Position = 0;
        }

        var sw = context.Items["__stopwatch"] as System.Diagnostics.Stopwatch;
        var durationMs = (int)(sw?.Elapsed.TotalMilliseconds ?? 0);

        var payload = new
        {
            DurationMs = durationMs,
            Endpoint = endpointName,
            Http = new
            {
                Method = context.Request.Method,
                Path = context.Request.Path.Value,
                QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "",
                StatusCode = context.Response.StatusCode
            },
            Request = new
            {
                Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = requestBody
            },
            Response = new
            {
                Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = responseBody
            },
            // Solo tipo y mensaje, sin stacktrace
            Exception = new
            {
                Type = ex.GetType().FullName,
                Message = ex.Message
            }
        };

        // Importante: NO pasar 'ex' como primer argumento para evitar stacktrace
        _logger.LogError("{Error}", payload);
    }

    public async Task EndCaptureAsync(HttpContext context)
    {
        var original = context.Items["__origResponseBody"] as Stream;
        if (original != null && _responseBuffer != null)
        {
            _responseBuffer.Position = 0;
            await _responseBuffer.CopyToAsync(original);
            context.Response.Body = original;
        }
    }
}