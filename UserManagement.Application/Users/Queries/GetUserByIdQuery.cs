using UserManagement.Application.Common.Mediator;

namespace UserManagement.Application.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<GetUserByIdResult?>;