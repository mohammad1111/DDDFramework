using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace Gig.Framework.Persistence.Ef.Converts;

public class PointConvert : JsonConverter<Point>
{
    public override void WriteJson(JsonWriter writer, Point value, JsonSerializer serializer)
    {
        writer.WriteValue($"{value.X},{value.Y}");
    }

    public override Point ReadJson(JsonReader reader, Type objectType, Point existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var location = (string)reader.Value;
        Point point = null;
        if (!string.IsNullOrEmpty(location))
        {
            var locationPoint = location.Split(',');
            point = new Point(double.Parse(locationPoint[0]), double.Parse(locationPoint[1]));
        }

        return point;
    }
}