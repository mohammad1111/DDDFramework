namespace Gig.Framework.EventBus.Contracts;

public interface ICrcEngine
{
    string GenerateCheckSum(string message);
    bool Validate(string message, string checkSum);
}