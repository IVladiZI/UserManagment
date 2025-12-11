using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net;
using UserManagement.Domain.Exceptions;
using UserManagement.Domain.Errors;

namespace UserManagement.Api.ProblemHandling;

public interface IExceptionToProblemDetailsMapper
{
    ProblemDetails Map(HttpContext context, Exception ex);
}

public class ExceptionToProblemDetailsMapper : IExceptionToProblemDetailsMapper
{
    private readonly ProblemDetailsFactory _factory;

    public ExceptionToProblemDetailsMapper(ProblemDetailsFactory factory)
    {
        _factory = factory;
    }

    public ProblemDetails Map(HttpContext context, Exception ex)
    {
        return ex switch
        {
            TimeoutException => _factory.CreateProblemDetails(
                context, (int)HttpStatusCode.GatewayTimeout,
                "Tiempo de espera excedido",
                "El servicio de base de datos no respondió a tiempo."),
            NpgsqlException => _factory.CreateProblemDetails(
                context, (int)HttpStatusCode.BadGateway,
                "Error de conexión a la base de datos",
                "No fue posible conectar con PostgreSQL."),
            DbUpdateException => _factory.CreateProblemDetails(
                context, (int)HttpStatusCode.Conflict,
                "Error al actualizar datos",
                "Ocurrió un problema al persistir cambios."),
            BusinessException bex when bex.ErrorCode == BusinessErrorCode.UserNotFound => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Business validation error",
                Detail = bex.Message,
                Type = $"https://yourdomain.com/errors/{bex.ErrorCode}",
                Instance = context.Request.Path
            },
            BusinessException bex => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Business validation error",
                Detail = bex.Message,
                Type = $"https://yourdomain.com/errors/{bex.ErrorCode}",
                Instance = context.Request.Path
            },
            _ => _factory.CreateProblemDetails(
                context, (int)HttpStatusCode.InternalServerError,
                "Error interno del servidor",
                "Ocurrió un error inesperado.")
        };
    }
}