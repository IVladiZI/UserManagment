using UserManagement.Domain.Errors;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Domain;

public class User
{
    public Guid Id { get; private set; }
    public FullName Name { get; private set; }
    public Document Document { get; private set; }
    public Email Email { get; private set; }
    public DateOnly BirthDate { get; private set; }

    public User(FullName name, Document document, Email email, DateOnly birthDate)
    {
        if (!IsOfLegalAge(birthDate))
            throw new BusinessException(BusinessErrorCode.UnderageUser);

        Id = Guid.NewGuid();
        Name = name;
        Document = document;
        Email = email;
        BirthDate = birthDate;
    }

    private static bool IsOfLegalAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age)) age--;
        return age >= 18;
    }
}
