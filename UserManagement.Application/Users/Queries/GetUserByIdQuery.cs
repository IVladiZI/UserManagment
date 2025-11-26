using MediatR;

namespace UserManagement.Application.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<GetUserByIdResult?>;