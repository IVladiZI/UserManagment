namespace UserManagement.Domain.Errors;

public enum BusinessErrorCode
{
    InvalidEmailFormat,
    ExistingEmail,
    InvalidEmailSize,
    InvalidDocumentNumber,
    ExistingDocumentNumber,
    InvalidFullName,
    UnderageUser,
    UserNotFound
}