using System;
using System.Collections.Generic;
using System.Linq;
using IdGen;

namespace Gig.Framework.Domain.IdGenerators;

public class GigIdGenerator : IGigIdGenerator
{
    
    
    
    private readonly IdGenerator _generator;

    public GigIdGenerator()
    {
        var epoch = new DateTime(2020, 4, 1, 0, 0, 0, DateTimeKind.Utc);
        
        var structure = new IdStructure(41, 10, 12);
        
        var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));

        _generator = new IdGenerator(0, options);


    }
    
    public long NewId()
    {
        var id = _generator.Take(100);
        return _generator.CreateId();
    }

    public IEnumerable<long> Take(int count)
    {
        return _generator.Take(count);
    }
}