namespace UserManagement.Contracts.Users;

public record CreateUserRequest(
    string Name,
    string LastName,
    string SecondLastName,
    string DocumentNumber,
    string Email,
    DateOnly BirthDate);