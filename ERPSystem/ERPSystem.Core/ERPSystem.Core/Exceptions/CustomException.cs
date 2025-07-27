using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Core.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }

    public BusinessException(string message, Exception innerException)
        : base(message, innerException) { }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entity, object key)
        : base($"{entity} with key '{key}' was not found.") { }
}

public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public ValidationException(string field, string error)
        : base("Validation error occurred.")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
    }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized access.") : base(message) { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Access forbidden.") : base(message) { }
}