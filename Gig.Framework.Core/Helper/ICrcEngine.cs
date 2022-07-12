namespace Gig.Framework.Core.Helper;

public interface ICrcEngine
{
    string GenerateCheckSum<T>(T message) where T : class;
    string GenerateCheckSum(string message);
    bool IsValidate(string message, string checkSum);

    bool IsValidate<T>(T message, string checkSum) where T : class;


    void Validate(string message, string checkSum);

    void Validate<T>(T message, string checkSum) where T : class;
}