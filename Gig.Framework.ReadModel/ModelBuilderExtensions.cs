using System.Reflection;
using Gig.Framework.Core.DataProviders;
using Microsoft.EntityFrameworkCore;

namespace Gig.Framework.ReadModel;

public static class ModelBuilderExtensions
{
    private static void DisableIdentity(ModelBuilder modelBuilder, Type entityType)
    {
        var method = typeof(ModelBuilderExtensions)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(t => t.Name == nameof(DisableIdentityGeneric));
        method.MakeGenericMethod(entityType)
            .Invoke(null, new object[] { modelBuilder });
    }

    private static void DisableIdentityGeneric<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseModel
    {
        modelBuilder.Entity<TEntity>().Property(p => p.Id).ValueGeneratedNever();
    }

    public static void DisableIdentitySettingOnAllEntities(this ModelBuilder modelBuilder)
    {
        foreach (var type in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseModel).IsAssignableFrom(type.ClrType))
            {
                DisableIdentity(modelBuilder, type.ClrType);
            }
        }
    }

    
    private static void ApplyRowVersion(ModelBuilder modelBuilder, Type entityType)
    {
        var method = typeof(ModelBuilderExtensions)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(t => t.Name == nameof(ApplyRowVersionGeneric));
        method.MakeGenericMethod(entityType)
            .Invoke(null, new object[] { modelBuilder });
    }

    private static void ApplyRowVersionGeneric<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseModel
    {
        modelBuilder.Entity<TEntity>().Property(c => c.RowVersion)
            .IsRequired()
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();
    }
    
    public static void ApplyRowVersionSettingOnAllEntities(this ModelBuilder modelBuilder)
    {
        foreach (var type in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseModel).IsAssignableFrom(type.ClrType))
            {
                ApplyRowVersion(modelBuilder, type.ClrType);
            }
        }
    }
}