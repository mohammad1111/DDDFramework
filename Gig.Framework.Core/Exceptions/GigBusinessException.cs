namespace Gig.Framework.Core.Exceptions;

public class GigBusinessException : Exception
{
    public GigBusinessException(string message, string code) : base(message)
    {
        Code = code;
    }

    public string Code { get; set; }
}