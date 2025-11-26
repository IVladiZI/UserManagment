namespace UserManagement.Application.Users.Queries;

public record GetUserByIdResult(
    Guid Id,
    string FullName,
    string DocumentNumber,
    string Email,
    DateOnly BirthDate);