using UserManagement.Domain;
using UserManagement.Contracts.Users;
namespace UserManagement.Application.Users.Mappings;

public static class UserMappings
{
    public static UserResponse ToResponse(this User user) =>
        new(
            user.Id,
            $"{user.Name.FirstName} {user.Name.PaternalSurname} {user.Name.MaternalSurname}",
            user.Document.Number,
            user.Email.Value,
            user.BirthDate);  
}