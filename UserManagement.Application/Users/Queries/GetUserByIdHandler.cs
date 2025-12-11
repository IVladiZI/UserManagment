using UserManagement.Application.Common.Mediator;
using UserManagement.Domain.Repositories;
using UserManagement.Domain.Exceptions;
using UserManagement.Domain.Errors;

namespace UserManagement.Application.Users.Queries;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResult?>
{
    private readonly IUserRepository _repository;

    public GetUserByIdHandler(IUserRepository repository) => _repository = repository;

    public async Task<GetUserByIdResult?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.Id, ct);
        return user is null
            ? throw new BusinessException(BusinessErrorCode.UserNotFound, request.Id)
            : new GetUserByIdResult(
            user.Id,
            $"{user.FullName.Name} {user.FullName.LastName} {user.FullName.SecondLastName}",
            user.Document.NumberDocument,
            user.Email.Value,
            user.BirthDate
        );
    }
}