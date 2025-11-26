using MediatR;

namespace UserManagement.Application.Users.Commands;

public record RegisterUserCommand(
    string FirstName,
    string PaternalSurname,
    string MaternalSurname,
    string DocumentNumber,
    string Email,
    DateOnly BirthDate
) : IRequest<Guid>;