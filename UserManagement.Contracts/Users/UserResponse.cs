namespace UserManagement.Contracts.Users;

public record UserResponse(
    Guid Id,
    string FullName,
    string DocumentNumber,
    string Email,
    DateOnly BirthDate);