using System.Text.RegularExpressions;
using UserManagement.Domain.Errors;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Domain;

public partial record Email
{
    public const int MaxLength = 254;
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !EmailRegex().IsMatch(value))
            throw new BusinessException(BusinessErrorCode.InvalidEmailFormat);

        if (value.Length > MaxLength)
            throw new BusinessException(BusinessErrorCode.InvalidEmailFormat);

        Value = value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
