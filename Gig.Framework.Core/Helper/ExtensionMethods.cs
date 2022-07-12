using Gig.Framework.Core.Serilizer;

namespace Gig.Framework.Core.Helper;

public static class ExtensionMethods
{
    public static T DeepCopy<T>(this T obj, ISerializer serializer)
    {
        var serializeObject = serializer.Serialize(obj);
        return serializer.Deserialize<T>(serializeObject);
    }
}