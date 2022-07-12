using System.IO.Compression;
using System.Text;

namespace Gig.Framework.Core.CompressUtilities;

public static class GigCompressUtility
{
    private static void CopyTo(Stream src,
        Stream dest)
    {
        var bytes = new byte[4096];


        int cnt;


        while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) dest.Write(bytes, 0, cnt);
    }


    public static string DeCompress(byte[] bytes)
    {
        if (bytes == null)
            return null;
        using var msi = new MemoryStream(bytes);
        using var mso = new MemoryStream();
        using (var gs = new GZipStream(msi, CompressionMode.Decompress))
        {
            CopyTo(gs, mso);
        }

        return Encoding.UTF8.GetString(mso.ToArray());
    }

    public static byte[] Compress(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        using var msi = new MemoryStream(bytes);
        using var mso = new MemoryStream();
        using (var gs = new GZipStream(mso, CompressionMode.Compress))
        {
            CopyTo(msi, gs);
        }

        return mso.ToArray();
    }
}