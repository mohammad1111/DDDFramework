using Nest;

namespace Gig.Framework.Core.Helper;

public static class GuidHelper
{
    public static long ToLong(this Guid id)
    {
        var byteArray = id.ToByteArray();
        return BitConverter.ToInt64(byteArray,0);
    }
}