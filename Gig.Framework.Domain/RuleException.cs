using System.ComponentModel;

namespace Gig.Framework.Domain;

public abstract class RuleException : Exception
{
    protected RuleException()
    {
    }

    protected RuleException([Localizable(true)] string message) : base(message)
    {
    }

    protected RuleException([Localizable(true)] string message, Exception innerException) : base(message,
        innerException)
    {
    }
}