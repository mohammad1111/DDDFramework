using System.ComponentModel;

namespace Gig.Framework.Domain;

public abstract class RuleWarningException : Exception
{
    protected RuleWarningException()
    {
    }

    protected RuleWarningException([Localizable(true)] string message) : base(message)
    {
    }

    protected RuleWarningException([Localizable(true)] string message, Exception innerException) : base(message,
        innerException)
    {
    }
}