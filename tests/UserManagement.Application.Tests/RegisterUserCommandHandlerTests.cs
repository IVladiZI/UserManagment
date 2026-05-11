using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Application.Users.Commands.RegisterUser;
using UserManagement.Domain;
using UserManagement.Domain.Errors;
using UserManagement.Domain.Exceptions;
using UserManagement.Domain.Repositories;
using Xunit;

namespace UserManagement.Application.Tests;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task Handle_Should_Throw_When_Document_Exists()
    {
        var cmd = new RegisterUserCommand
        {
            Name = "John",
            LastName = "Doe",
            SecondLastName = "Smith",
            DocumentNumber = "DOC-1",
            Email = "john@doe.com",
            BirthDate = new DateOnly(DateTime.UtcNow.Year - 25, 1, 1)
        };

        _repo.Setup(r => r.ExistsByDocumentAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(true);

        var handler = new RegisterUserCommandHandler(_repo.Object, _uow.Object);

        var act = async () => await handler.Handle(cmd, CancellationToken.None);

        var ex = await act.Should().ThrowAsync<BusinessException>();
        ex.Which.ErrorCode.Should().Be(BusinessErrorCode.ExistingDocumentNumber);
        _repo.Verify(r => r.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Email_Exists()
    {
        var cmd = new RegisterUserCommand
        {
            Name = "John",
            LastName = "Doe",
            SecondLastName = "Smith",
            DocumentNumber = "DOC-1",
            Email = "john@doe.com",
            BirthDate = new DateOnly(DateTime.UtcNow.Year - 25, 1, 1)
        };

        _repo.Setup(r => r.ExistsByDocumentAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _repo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new RegisterUserCommandHandler(_repo.Object, _uow.Object);

        var act = async () => await handler.Handle(cmd, CancellationToken.None);

        var ex = await act.Should().ThrowAsync<BusinessException>();
        ex.Which.ErrorCode.Should().Be(BusinessErrorCode.ExistingEmail);
        _repo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Create_And_Persist_User_When_Unique()
    {
        var cmd = new RegisterUserCommand
        {
            Name = "John",
            LastName = "Doe",
            SecondLastName = "Smith",
            DocumentNumber = "DOC-1",
            Email = "john@doe.com",
            BirthDate = new DateOnly(DateTime.UtcNow.Year - 25, 1, 1)
        };

        _repo.Setup(r => r.ExistsByDocumentAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _repo.Setup(r => r.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new RegisterUserCommandHandler(_repo.Object, _uow.Object);

        var id = await handler.Handle(cmd, CancellationToken.None);

        id.Should().NotBe(Guid.Empty);
        _repo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}