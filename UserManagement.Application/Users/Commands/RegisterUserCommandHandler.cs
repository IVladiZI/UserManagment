using MediatR;
using UserManagement.Domain;
using UserManagement.Domain.Repositories;
using UserManagement.Domain.Exceptions;
using UserManagement.Domain.Errors;

namespace UserManagement.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler(IUserRepository userRepository) : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new BusinessException(BusinessErrorCode.InvalidEmailFormat);

        var user = new User(
            new FullName(request.FirstName, request.PaternalSurname, request.MaternalSurname),
            new Document(request.DocumentNumber),
            email,
            request.BirthDate
        );

        await _userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}