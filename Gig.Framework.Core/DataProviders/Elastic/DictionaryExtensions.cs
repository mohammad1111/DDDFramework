using System.Dynamic;

namespace Gig.Framework.Core.DataProviders.Elastic;

public static class DictionaryExtensions
{
    public static dynamic ToAnonymousObject(this Dictionary<string, object> dictionary)
    {
        var eo = new ExpandoObject();
        var eoColl = (ICollection<KeyValuePair<string, object>>)eo;

        foreach (var kvp in dictionary) eoColl.Add(kvp);

        dynamic eoDynamic = eo;
        return eoDynamic;
    }

    public static object ToDynamicObject(this Dictionary<string, object> dictionary)
    {
        dynamic obj = dictionary.Aggregate(new ExpandoObject() as IDictionary<string, object>,
            (a, p) =>
            {
                a.Add(p.Key, p.Value);
                return a;
            });
        return obj;
    }
}