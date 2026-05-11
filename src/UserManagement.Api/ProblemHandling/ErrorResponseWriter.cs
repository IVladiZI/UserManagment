using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace UserManagement.Api.ProblemHandling;

public interface IErrorResponseWriter
{
    Task WriteAsync(HttpContext context, ProblemDetails problemDetails);
}

public class ErrorResponseWriter : IErrorResponseWriter
{
    public async Task WriteAsync(HttpContext context, ProblemDetails problemDetails)
    {
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}