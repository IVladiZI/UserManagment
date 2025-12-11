using UserManagement.Domain.Errors;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Domain;

public class User
{
    public Guid Id { get; private set; }
    public FullName FullName { get; private set; } = null!;
    public Document Document { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public DateOnly BirthDate { get; private set; }

    // Constructor sin parámetros para EF Core
    private User() { }

    private User(FullName fullName, Document document, Email email, DateOnly birthDate)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        Document = document;
        Email = email;
        BirthDate = birthDate;
    }

    public static User Create(FullName fullName, Document document, Email email, DateOnly birthDate)
    {
        if (!IsOfLegalAge(birthDate))
            throw new BusinessException(BusinessErrorCode.UnderageUser);

        return new User(fullName, document, email, birthDate);
    }

    private static bool IsOfLegalAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age)) age--;
        return age >= 18;
    }
}
