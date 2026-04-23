namespace Domain.Common;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}

public class DomainRuleViolationException : DomainException
{
    public DomainRuleViolationException(string message) : base(message)
    {
    }
}
