using UserManagement.Application.Common.Mediator;
using UserManagement.Domain;
using UserManagement.Domain.Errors;
using UserManagement.Domain.Exceptions;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var document = new Document(request.DocumentNumber);

        if (await _userRepository.ExistsByDocumentAsync(document, cancellationToken))
            throw new BusinessException(BusinessErrorCode.ExistingDocumentNumber, document.NumberDocument);

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new BusinessException(BusinessErrorCode.ExistingEmail, email.Value);

        var user = User.Create(
            new FullName(request.Name, request.LastName, request.SecondLastName),
            document,
            email,
            request.BirthDate
        );

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}