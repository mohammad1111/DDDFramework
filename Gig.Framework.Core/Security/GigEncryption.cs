using System.Security.Cryptography;
using System.Text;

namespace Gig.Framework.Core.Security;

public class GigEncryption
{
    public static class EncryptionUtil
    {
        public static string Decrypt(string cipherText, string passPhrase)
        {
            const string initVector = "pemgail9uzpgzl88";
            const int keySize = 256;
            var initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            var cipherTextBytes = Convert.FromBase64String(cipherText);
            var password = new PasswordDeriveBytes(passPhrase, null);
            var keyBytes = password.GetBytes(keySize / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC };
            var decrypt = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decrypt, CryptoStreamMode.Read);
            var plainTextBytes = new byte[cipherTextBytes.Length];
            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }
}