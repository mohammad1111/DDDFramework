using System.Security.Cryptography;
using System.Text;
using Gig.Framework.EventBus.Contracts;

namespace Gig.Framework.EventBus.Configs;

public class CrcEngine : ICrcEngine
{
    private const ushort Polynomial = 0xA001;
    private readonly ushort[] _table = new ushort[256];

    public CrcEngine()
    {
        ushort value;
        ushort temp;
        for (ushort i = 0; i < _table.Length; ++i)
        {
            value = 0;
            temp = i;
            for (byte j = 0; j < 8; ++j)
            {
                if (((value ^ temp) & 0x0001) != 0)
                    value = (ushort)((value >> 1) ^ Polynomial);
                else
                    value >>= 1;

                temp >>= 1;
            }

            _table[i] = value;
        }
    }

    public string GenerateCheckSum(string message)
    {
        var key = Encoding.Default.GetBytes("956#$%hYu9");
        using var hmac = new HMACSHA512(key);
        var stream = new MemoryStream(Encoding.Default.GetBytes(message));
        var bytes = hmac.ComputeHash(stream);
        var s = ComputeChecksum(bytes).ToString("x2");
        return s;
    }

    public bool Validate(string message, string checkSum)
    {
        return GenerateCheckSum(message) == checkSum;
    }

    private static byte[] HexToBytes(string input)
    {
        var result = new byte[input.Length / 2];
        for (var i = 0; i < result.Length; i++) result[i] = Convert.ToByte(input.Substring(2 * i, 2), 16);

        return result;
    }

    private ushort ComputeChecksum(IReadOnlyList<byte> bytes)
    {
        ushort crc = 0;
        for (var i = 0; i < bytes.Count; ++i)
        {
            var index = (byte)(crc ^ bytes[i]);
            crc = (ushort)((crc >> 8) ^ _table[index]);
        }

        return crc;
    }
}