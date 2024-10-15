using System;
using Microsoft.EntityFrameworkCore;

namespace Gig.Framework.Persistence.Ef;

public interface IEntityMapper
{
    Type MapperType { get; }


    void Map(ModelBuilder modelBuilder);
}