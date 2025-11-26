using System.Text.Json;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Api.Middlewares
{
    public class ProblemDetailsMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/problem+json";

                var problemDetails = new
                {
                    type = $"https://yourdomain.com/errors/{ex.ErrorCode}",
                    title = "Business validation error",
                    status = 400,
                    detail = ex.Message,
                    instance = context.Request.Path
                };

                var json = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(json);
            }
        }
    }
}