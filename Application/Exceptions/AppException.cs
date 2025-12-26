namespace Application.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    protected AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404)
    {
    }
}

public class DomainRuleViolationException : AppException
{
    public DomainRuleViolationException(string message) : base(message, 400)
    {
    }
}
