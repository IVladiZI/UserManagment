using UserManagement.Application.Users.Queries;
using UserManagement.Contracts.Users;

namespace UserManagement.Api.Mappings;

internal static class UserResponseMapper
{
    public static UserResponse ToContract(this GetUserByIdResult result) =>
        new(
            result.Id,
            result.FullName,
            result.DocumentNumber,
            result.Email,
            result.BirthDate
        );
}