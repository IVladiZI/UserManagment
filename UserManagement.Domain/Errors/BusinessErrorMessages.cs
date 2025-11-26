namespace UserManagement.Domain.Errors;

public static class BusinessErrorMessages
{
    public static readonly Dictionary<BusinessErrorCode, string> Messages = new()
    {
        { BusinessErrorCode.InvalidEmailFormat, "El contenido no cumple el formato de un correo electrónico." },
        { BusinessErrorCode.InvalidEmailSize, "El correo electrónico es demasiado largo" },
        { BusinessErrorCode.InvalidDocument, "El documento de identidad no es válido." },
        { BusinessErrorCode.InvalidFullName, "El nombre completo no es válido." },
        { BusinessErrorCode.UnderageUser, "El usuario debe ser mayor de edad." }
    };
}