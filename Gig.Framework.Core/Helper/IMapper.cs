using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Models;

namespace Gig.Framework.Core.Helper;

public static class MapperExtend
{
    public static T To<T>(this BaseModel entity, IMapper mapper)
    {
        return mapper.Map<T>(entity);
    }

    public static T To<T>(this Dto dtoModel, IMapper mapper)
    {
        return mapper.Map<T>(dtoModel);
    }

    public static T Map<T, TSource>(TSource source, IMapper mapper)
    {
        return mapper.Map<T>(source);
    }
}

public interface IMapper
{
    T Map<T>(object obj);
}