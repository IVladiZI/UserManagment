namespace UserManagement.Domain.Errors;

public static class BusinessErrorMessages
{
    public static readonly Dictionary<BusinessErrorCode, string> Messages = new()
    {
        { BusinessErrorCode.InvalidEmailFormat, "El contenido no cumple el formato de un correo electrónico: {0}" },
        { BusinessErrorCode.ExistingEmail, "El correo ya fue registrado anteriormente: {0}" },
        { BusinessErrorCode.InvalidEmailSize, "El correo electrónico es demasiado largo: {0}" },
        { BusinessErrorCode.InvalidDocumentNumber, "El número de documento de identidad no es válido: {0}" },
        { BusinessErrorCode.ExistingDocumentNumber, "El número de documento de identidad ya fue registrado anteriormente: {0}" },
        { BusinessErrorCode.InvalidFullName, "El nombre completo no es válido: {0}" },
        { BusinessErrorCode.UnderageUser, "El usuario debe ser mayor de edad." },
        { BusinessErrorCode.UserNotFound, "No se tiene registrado al usuario: {0}" }
    };
}