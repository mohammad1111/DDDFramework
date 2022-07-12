using AutoMapper;

namespace Gig.Framework.Core.Helper;

public class GigMapper : IMapper
{
    private readonly AutoMapper.IMapper _mapper;

    public GigMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMissingTypeMaps = true;
            cfg.ValidateInlineMaps = false;
            // other configurations
        });
        _mapper = config.CreateMapper();
    }

    public T Map<T>(object obj)
    {
        return _mapper.Map<T>(obj);
    }
}