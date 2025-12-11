using UserManagement.Application.Common.Mediator;

namespace UserManagement.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserCommand : IRequest<Guid>
{
    public string Name { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string SecondLastName { get; init; } = null!;
    public string DocumentNumber { get; init; } = null!;
    public string Email { get; init; } = null!;
    public DateOnly BirthDate { get; init; }
}