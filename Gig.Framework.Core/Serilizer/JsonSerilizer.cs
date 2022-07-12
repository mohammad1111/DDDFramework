using Newtonsoft.Json;

namespace Gig.Framework.Core.Serilizer;

public class JsonSerializer : ISerializer
{
    private readonly JsonConverter[] _types;

    public JsonSerializer(params JsonConverter[] types)
    {
        _types = types;
    }

    public T Deserialize<T>(string value)
    {
        return JsonConvert.DeserializeObject<T>(value, _types);
    }

    public string Serialize(object value)
    {
        return JsonConvert.SerializeObject(value, Formatting.Indented, _types);
    }
}