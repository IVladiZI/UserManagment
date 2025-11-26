namespace UserManagement.Contracts.Users;

public record CreateUserRequest(
    string FirstName,
    string PaternalSurname,
    string MaternalSurname,
    string DocumentNumber,
    string Email,
    DateOnly BirthDate);