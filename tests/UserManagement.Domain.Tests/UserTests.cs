using FluentAssertions;
using System;
using UserManagement.Domain;
using UserManagement.Domain.Errors;
using UserManagement.Domain.Exceptions;
using Xunit;

namespace UserManagement.Domain.Tests;

public class UserTests
{
    [Fact]
    public void Create_Should_Throw_BusinessException_When_Underage()
    {
        var birth = new DateOnly(DateTime.UtcNow.Year - 17, 1, 1);

        Action act = () => User.Create(
            new FullName("John", "Doe", "Smith"),
            new Document("DOC-1"),
            new Email("john@doe.com"),
            birth);

        act.Should().Throw<BusinessException>()
           .Which.ErrorCode.Should().Be(BusinessErrorCode.UnderageUser);
    }

    [Fact]
    public void Create_Should_Succeed_When_OfLegalAge()
    {
        var birth = new DateOnly(DateTime.UtcNow.Year - 25, 1, 1);

        var user = User.Create(
            new FullName("John", "Doe", "Smith"),
            new Document("DOC-1"),
            new Email("john@doe.com"),
            birth);

        user.Id.Should().NotBe(Guid.Empty);
        user.FullName.Name.Should().Be("John");
        user.Document.NumberDocument.Should().Be("DOC-1");
        user.Email.Value.Should().Be("john@doe.com");
    }
}