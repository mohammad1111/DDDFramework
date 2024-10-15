using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gig.Framework.Core;
using Gig.Framework.Core.DataProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Gig.Framework.ReadModel;

public abstract class ReadDbContext : DbContext
{
    public readonly IReadDbContextDependencies ReadDbContextDependencies;


    protected ReadDbContext(IReadDbContextDependencies readDbContextDependencies)
    {
        ReadDbContextDependencies = readDbContextDependencies;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected ReadDbContext(DbContextOptions options, IReadDbContextDependencies readDbContextDependencies)
        : base(options)
    {
        ReadDbContextDependencies = readDbContextDependencies;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>()
            .UseSqlServer(ReadDbContextDependencies.DataSetting.ReadDataConnectionString, x =>
            {
                x.UseNetTopologySuite();
                x.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    $"{ReadDbContextDependencies.DataSetting.MicroServiceName}Query"
                );
            });
        ;
        base.OnConfiguring(optionsBuilder);
    }

    public override int SaveChanges()
    {
        FillProperty();
        return base.SaveChanges();
    }

    private void FillProperty()
    {
        var userContext = ReadDbContextDependencies.RequestContext;
        FillPropertyInAddedMode(userContext);
        FillPropertyInUpdateMode(userContext);
    }

    private void FillPropertyInUpdateMode(IRequestContext requestContext)
    {
        var userContext = requestContext.GetUserContext();
        foreach (var entity in ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).Select(y => y.Entity)
                     .OfType<BaseModel>().Where(x => x.ModifiedBy == 0))
        {
            entity.ModifiedBy = userContext.UserId;
            entity.ModifiedOn = DateTime.Now;
        }
    }

    private void FillPropertyInAddedMode(IRequestContext requestContext)
    {
        var userContext = requestContext.GetUserContext();

        foreach (var model in ChangeTracker.Entries().Where(x => x.State == EntityState.Added)
                     .Select(y => y.Entity).OfType<BaseModel>().Where(x => x.CreatedBy == 0))
        {
            model.ModifiedOn = DateTime.Now;
            model.ModifiedBy = userContext.UserId;
            model.CompanyId = userContext.CompanyId;
            model.BranchId = userContext.BranchId;
            model.CreatedBy = userContext.UserId;
            model.OwnerId = userContext.UserId;
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        FillProperty();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new()
    )
    {
        FillProperty();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        FillProperty();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema($"{ReadDbContextDependencies.DataSetting.MicroServiceName}Query");
        
        Console.WriteLine($"Entity count : {modelBuilder.Model.GetEntityTypes().Count()}");

        modelBuilder.ApplyRowVersionSettingOnAllEntities();
        modelBuilder.DisableIdentitySettingOnAllEntities();

    }
}