using UserManagement.Domain;
using UserManagement.Contracts.Users;
namespace UserManagement.Application.Users.Mappings;

public static class UserMappings
{
    public static UserResponse ToResponse(this User user) =>
        new(
            user.Id,
            $"{user.FullName.Name} {user.FullName.LastName} {user.FullName.SecondLastName}",
            user.Document.NumberDocument,
            user.Email.Value,
            user.BirthDate);  
}