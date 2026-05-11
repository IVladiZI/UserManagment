using FluentAssertions;
using System;
using UserManagement.Domain.Errors;
using UserManagement.Domain.Exceptions;
using Xunit;

namespace UserManagement.Domain.Tests;

public class BusinessExceptionTests
{
    [Fact]
    public void BusinessException_WithoutArgs_Should_Not_Print_Placeholders()
    {
        var ex = new BusinessException(BusinessErrorCode.UserNotFound);
        ex.Message.Should().NotContain("{0}");
        ex.ErrorCode.Should().Be(BusinessErrorCode.UserNotFound);
    }

    [Fact]
    public void BusinessException_WithArgs_Should_Format_Message()
    {
        var id = Guid.NewGuid();
        var ex = new BusinessException(BusinessErrorCode.UserNotFound, id);
        ex.Message.Should().Contain(id.ToString());
        ex.ErrorCode.Should().Be(BusinessErrorCode.UserNotFound);
    }
}