using System.Text.RegularExpressions;
using UserManagement.Domain.Errors;

namespace UserManagement.Domain.Exceptions;

public partial class BusinessException : Exception
{
    public BusinessErrorCode ErrorCode { get; }

    public BusinessException(BusinessErrorCode errorCode)
        : base(CleanPlaceholders(BusinessErrorMessages.Messages[errorCode]))
    {
        ErrorCode = errorCode;
    }

    public BusinessException(BusinessErrorCode errorCode, string message)
        : base(string.IsNullOrWhiteSpace(message)
            ? CleanPlaceholders(BusinessErrorMessages.Messages[errorCode])
            : message)
    {
        ErrorCode = errorCode;
    }

    // Constructor con parámetros: formatea usando la plantilla del diccionario
    public BusinessException(BusinessErrorCode errorCode, params object[] args)
        : base(FormatMessage(errorCode, args))
    {
        ErrorCode = errorCode;
    }

    private static string FormatMessage(BusinessErrorCode errorCode, object[] args)
    {
        var template = BusinessErrorMessages.Messages.TryGetValue(errorCode, out var msg)
            ? msg
            : "Error de negocio";

        if (args is { Length: > 0 })
        {
            // Si hay argumentos, se formatea normalmente
            return string.Format(template, args);
        }

        // Sin argumentos: limpia los placeholders {0}, {1}, ...
        return CleanPlaceholders(template);
    }

    private static string CleanPlaceholders(string template)
    {
        // Remueve cualquier placeholder {n} y espacios dobles
        var cleaned = PlaceholderRegex().Replace(template, string.Empty);
        cleaned = DoubleSpaceRegex().Replace(cleaned, " ").Trim();
        return cleaned;
    }

    [GeneratedRegex(@"\{\d+\}")]
    private static partial Regex PlaceholderRegex();

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex DoubleSpaceRegex();
}


