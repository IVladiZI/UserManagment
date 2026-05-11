using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Application.Users.Queries;
using UserManagement.Domain;
using UserManagement.Domain.Errors;
using UserManagement.Domain.Exceptions;
using UserManagement.Domain.Repositories;
using Xunit;

namespace UserManagement.Application.Tests;

public class GetUserByIdHandlerTests
{
    private readonly Mock<IUserRepository> _repo = new();

    [Fact]
    public async Task Handle_Should_Return_Result_When_User_Exists()
    {
        var id = Guid.NewGuid();
        var user = User.Create(
            new FullName("John", "Doe", "Smith"),
            new Document("DOC-1"),
            new Email("john@doe.com"),
            new DateOnly(DateTime.UtcNow.Year - 25, 1, 1));

        // Forzar el Id para que coincida con la consulta
        typeof(User).GetProperty(nameof(User.Id))!.SetValue(user, id);

        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var handler = new GetUserByIdHandler(_repo.Object);

        var result = await handler.Handle(new GetUserByIdQuery(id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Email.Should().Be("john@doe.com");
    }

    [Fact]
    public async Task Handle_Should_Throw_BusinessException_UserNotFound_When_User_Does_Not_Exist()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var handler = new GetUserByIdHandler(_repo.Object);

        var act = async () => await handler.Handle(new GetUserByIdQuery(id), CancellationToken.None);

        var ex = await act.Should().ThrowAsync<BusinessException>();
        ex.Which.ErrorCode.Should().Be(BusinessErrorCode.UserNotFound);
        ex.Which.Message.Should().Contain(id.ToString());
    }
}