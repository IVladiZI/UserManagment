using System;
using UserManagement.Domain.Errors;

namespace UserManagement.Domain.Exceptions;

public class BusinessException(BusinessErrorCode errorCode) : Exception(BusinessErrorMessages.Messages[errorCode])
{
    public BusinessErrorCode ErrorCode { get; } = errorCode;
}


