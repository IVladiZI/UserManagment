using UserManagement.Api.Abstractions;
using UserManagement.Api.Observability;
using UserManagement.Api.ProblemHandling;

namespace UserManagement.Api.Middlewares;

public class ProblemDetailsMiddleware : IMiddleware
{
    private readonly IExceptionToProblemDetailsMapper _mapper;
    private readonly IErrorResponseWriter _writer;
    private readonly IRequestResponseLogger _logger;

    public ProblemDetailsMiddleware(
        IExceptionToProblemDetailsMapper mapper,
        IErrorResponseWriter writer,
        IRequestResponseLogger logger)
    {
        _mapper = mapper;
        _writer = writer;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var startedAt = DateTimeOffset.Now; // usa hora local si prefieres evitar UTC
        await _logger.BeginCaptureAsync(context);

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var pd = _mapper.Map(context, ex);
            // El logger interno ya evita pasar la excepción al provider
            await _logger.LogErrorAsync(context, ex, startedAt);
            await _writer.WriteAsync(context, pd);
        }
        finally
        {
            await _logger.EndCaptureAsync(context);
        }
    }
}