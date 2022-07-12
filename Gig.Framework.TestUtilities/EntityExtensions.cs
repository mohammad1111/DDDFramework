using Gig.Framework.Core.DataProviders;
using Gig.Framework.Domain;

namespace Gig.Framework.TestUtilities;

public static class EntityExtensions
{
    public static T AsInUpdateMode<T>(this T instance)
        where T : Entity
    {
        return instance.With(
            m => m.IsInUpdateMode,
            true
        );
    }

    public static T WithId<T>(this T instance, long id)
        where T : Entity
    {
        return instance.With(
            m => m.Id,
            id
        );
    }
}