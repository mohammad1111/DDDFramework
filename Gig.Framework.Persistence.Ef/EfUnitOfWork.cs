using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.DataProviders.CacheKeys;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Exceptions;
using Gig.Framework.Core.Logging;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.Domain;
using Gig.Framework.Persistence.Ef.Models;
using Gig.Framework.RuleEngine.Contract.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gig.Framework.Persistence.Ef;

public abstract class EfUnitOfWork : GigDbContext
{
    private const string EntityString = "Entity";
    private readonly string _connectionString;

    private readonly IEntityFrameWorkDependencies _entityFrameWorkDependencies;
    private readonly string _schema;

    private IDbContextTransaction _transaction;

    protected EfUnitOfWork(string connectionString, IEntityFrameWorkDependencies entityFrameWorkDependencies,DbContextOptions<EfUnitOfWork> options)
        :base(options)
    {
        _connectionString = connectionString;
        _entityFrameWorkDependencies = entityFrameWorkDependencies;
        _schema = _entityFrameWorkDependencies.DataSetting.MicroServiceName;
    }

    private List<PublisherEvent> Events { get; set; }

    private bool IsAmbientTransaction => Transaction.Current != null;

    public override async Task BeginTransaction()
    {
        if (!IsAmbientTransaction) _transaction = await Database.BeginTransactionAsync();
    }

    public override async Task<IEnumerable<PublisherEvent>> Commit(IList<GigEntityRulesResult> ruleSetIds = null)
    {
        var userContext = _entityFrameWorkDependencies.RequestContext.GetUserContext();
        var entityCommitRuleSetId = new List<Guid>();
        if (ruleSetIds != null)
        {
            var recGuidIds = ChangeTracker.Entries().Select(x => x.Entity).OfType<Entity>().Select(x => x.RecGuid)
                .ToList();
            entityCommitRuleSetId = ruleSetIds
                .Where(x => recGuidIds.Any(recGuidId => x.RecGuid == recGuidId))
                .Select(x => x.RuleSetId).ToList();
        }

        SetBaseFieldInAddedMode();
        SetUpdateFields();
        var currentEventHandler = await GetPublisherEvent();
        var aggregateRoots = ChangeTracker.Entries().Select(x => x.Entity).OfType<AggregateRoot>().ToList();
        Events = SaveEventIntegrationEvents(aggregateRoots).ToList();
        var ruleEvents = await GetRuleEvents(entityCommitRuleSetId);

        if (ruleEvents.commitEvents.Any()) Events.AddRange(ruleEvents.commitEvents);

        var auditEntries = await OnBeforeSaveChanges();

        try
        {
            await SaveOutBoxEvent(Events.Where(x => x.Type == EventType.Both || x.Type == EventType.Integrate)
                .ToList());
            await SaveInBoxEvent(currentEventHandler);
            SaveRuleEngine(entityCommitRuleSetId);
            await SaveChangesAsync();
            Auditing(auditEntries);

            var currentEvents = Events.ToList();
            Events.Clear();
            await RemovePublisherEvent();
            _entityFrameWorkDependencies.Logger.Information("Save Data in  TraceId: {TraceId}",
                _entityFrameWorkDependencies.RequestContext.GetUserContext().TraceId);
            ChangeTracker.Clear();
            return currentEvents;
        }
        catch (DbUpdateConcurrencyException exception)
        {
            await RollbackTransactionAsync();
            _entityFrameWorkDependencies.Logger.Error(exception,
                "DbUpdateConcurrencyException in  TraceId: {TraceId}",
                userContext.TraceId);
            ChangeTracker.Clear();
            throw new FrameworkException("اطلاعات تغییر کرده است", exception);
        }
        catch (Exception e)
        {
            await RollbackTransactionAsync();

            if (EfExceptionHelper.IsUniqueConstraintViolation(e))
            {
                var exception = new FrameworkException("اطلاعات تکراری است", e);
                _entityFrameWorkDependencies.Logger.Error(e, "Duplicate Data in TraceId: {TraceId}",
                    userContext.TraceId);

                ChangeTracker.Clear();
                throw exception;
            }

            _entityFrameWorkDependencies.Logger.Error(e, "UnHandledException  TraceId: {TraceId}",
                userContext.TraceId);
            ChangeTracker.Clear();
            throw;
        }
    }

    public override void Rollback()
    {
    }

    private async Task RemovePublisherEvent()
    {
        await _entityFrameWorkDependencies.RequestMemoryCacheManager.RemoveAsync(new InboxCacheKey());
    }

    private async Task<Tuple<Guid, string>> GetPublisherEvent()
    {
        if (_entityFrameWorkDependencies.RequestMemoryCacheManager.ExistsKey(new InboxCacheKey()))
        {
            var domainEventId =
                await _entityFrameWorkDependencies.RequestMemoryCacheManager.GetAsync<Tuple<Guid, string>>(
                    new InboxCacheKey());
            return domainEventId;
        }

        return null;
    }

    private void Auditing(List<AuditEntry> auditEntries)
    {
        if (IsAmbientTransaction)
        {
            
        }
        if (auditEntries == null) return;
        var userContext = _entityFrameWorkDependencies.RequestContext.GetUserContext();
        foreach (var entry in auditEntries)
        {
            _entityFrameWorkDependencies.Logger.Information("Audit in  TraceId: {TraceId}, Audit: {@Audit}",
                userContext.TraceId, entry.ToAudit());
       }
    }

    public async Task<Guid?> GetTransactionId()
    {
        if (await _entityFrameWorkDependencies.RequestMemoryCacheManager.Exists(new TransactionIdCacheKey()))
            return await _entityFrameWorkDependencies.RequestMemoryCacheManager.GetAsync<Guid>(
                new TransactionIdCacheKey());

        return null;
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null) await _transaction.RollbackAsync();
    }

    private void SaveRuleEngine(IList<Guid> entityCommitRuleSetId)
    {
        if (entityCommitRuleSetId.Any())
            foreach (var ruleSetId in entityCommitRuleSetId)
            {
                var ruleSet = new RunningGigRuleSetEntity
                {
                    RuleSetId = ruleSetId,
                    HandelRule = false
                };
                Attach(ruleSet);
                ruleSet.HandelRule = true;
            }
    }

    private async Task SaveOutBoxEvent(IList<PublisherEvent> events)
    {
        Guid? transactionId = null;
        if (IsAmbientTransaction) transactionId = await GetTransactionId();

        if (events.Any())
            foreach (var x in events)
                Add(new PublisherEventEntity
                {
                    UserContext = x.UserContext,
                    EventContent = x.EventContent,
                    CreateDateTime = DateTime.Now,
                    DomainEventId = x.DomainEventId,
                    EventTypeString = x.EventTypeString,
                    TransactionEventId = transactionId
                });
    }

    private async Task SaveInBoxEvent(Tuple<Guid, string> currentHandler)
    {
        if (currentHandler != null)
            Add(new HandleInboxEvent
            {
                HandlerType = currentHandler.Item2,
                DomainEventId = currentHandler.Item1,
                UpdateDateTime = DateTime.Now
            });
    }


    private void SetBaseFieldInAddedMode()
    {
        var userContext = _entityFrameWorkDependencies.RequestContext.GetUserContext();

        foreach (var entity in ChangeTracker.Entries().Where(x => x.State == EntityState.Added).Select(y => y.Entity)
                     .OfType<Entity>())
        {
            entity.CompanyId = userContext.CompanyId;
            entity.BranchId = userContext.BranchId;
            entity.ModifiedBy = userContext.UserId;
            entity.CreatedBy = userContext.UserId;
            entity.OwnerId = userContext.UserId;
        }
    }

    private void SetUpdateFields()
    {
        var userContext = _entityFrameWorkDependencies.RequestContext.GetUserContext();

        foreach (var o in ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).Select(y => y.Entity)
                     .OfType<Entity>())
        {
            o.ModifiedBy = userContext.UserId;
            o.ModifiedOn = DateTime.Now;
        }
    }

    private IEnumerable<PublisherEvent> SaveEventIntegrationEvents(List<AggregateRoot> aggregateRoots)
    {
        var userContext = _entityFrameWorkDependencies.RequestContext.GetUserContext();

        var savingEvents = new List<PublisherEvent>();
        var aggregateRootIds = aggregateRoots.Select(x => x.Id).ToArray();
        aggregateRoots.AddRange(ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged)
            .Select(x => x.Entity).OfType<AggregateRoot>()
            .Where(x => !aggregateRootIds.Contains(x.Id) && x.Changes.Any()).ToList());
        foreach (var root in aggregateRoots)
        foreach (var domainEvent in root.Changes)
        {
            domainEvent.BranchId = userContext.BranchId;
            domainEvent.CompanyId = userContext.CompanyId;
            domainEvent.UserId = userContext.UserId;
            domainEvent.IsAdmin = userContext.IsAdmin;
            domainEvent.LangTypeCode = 1;
            domainEvent.SubSystemId = 6;
            savingEvents.Add(
                new PublisherEvent(domainEvent,
                    domainEvent.GetType().AssemblyQualifiedName,
                    root.GetType().Name,
                    _entityFrameWorkDependencies.Serializer));
        }

        return savingEvents;
    }

    private async Task<(IList<PublisherEvent> commitEvents, IList<PublisherEvent> rollbackEvents)> GetRuleEvents(
        IList<Guid> ruleSetIds)
    {
        var userContext = _entityFrameWorkDependencies.RequestContext.GetUserContext();
        var serializeUserContext = _entityFrameWorkDependencies.Serializer.Serialize(userContext);
        var commitEvents = new List<PublisherEvent>();
        var rollbackEvents = new List<PublisherEvent>();

        if (ruleSetIds.Any())
        {
            var results = (await _entityFrameWorkDependencies.RuleRepository.GetRulesResult(ruleSetIds)).ToList();
            var user = _entityFrameWorkDependencies.RequestContext.GetUserContext();
            foreach (var rule in results)
            {
                var methodInfo = typeof(ISerializer).GetMethod("Deserialize")
                    ?.MakeGenericMethod(Type.GetType(rule.TypeOfData));
                var result = (GigRuleResultModel)methodInfo.Invoke(_entityFrameWorkDependencies.Serializer,
                    new[] { rule.RuleContent });

                foreach (var resultEvent in result.Events)
                {
                    resultEvent.EngineEvent.CompanyId = user.CompanyId;
                    resultEvent.EngineEvent.BranchId = user.BranchId;
                    resultEvent.EngineEvent.SubSystemId = user.SubSystemId;
                    resultEvent.EngineEvent.LangTypeCode = user.LangTypeCode;
                    resultEvent.EngineEvent.UserId = user.UserId;
                    resultEvent.EngineEvent.IsAdmin = user.IsAdmin;
                }

                commitEvents.AddRange(result.Events.Where(x => x.RuleEventType == RuleEventType.Commit).Select(
                    x =>
                        new PublisherEvent(result.RuleId, rule.TypeOfData,
                            _entityFrameWorkDependencies.Serializer.Serialize(x.EngineEvent),
                            x.RuleType, DateTime.Now,
                            Transaction.Current != null
                                ? Transaction.Current.TransactionInformation.DistributedIdentifier
                                : (Guid?)null)));

                rollbackEvents.AddRange(result.Events.Where(x => x.RuleEventType == RuleEventType.Rollback)
                    .Select(x =>
                        new PublisherEvent(result.RuleId, rule.TypeOfData,
                            _entityFrameWorkDependencies.Serializer.Serialize(x.EngineEvent),
                            x.RuleType, DateTime.Now,
                            Transaction.Current != null
                                ? Transaction.Current.TransactionInformation.DistributedIdentifier
                                : (Guid?)null)));
            }
        }

        return (commitEvents, rollbackEvents);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>()
            .UseLazyLoadingProxies()
            .UseSqlServer(_connectionString, x =>
            {
                x.UseNetTopologySuite();
                x.MinBatchSize(5);
                x.MaxBatchSize(100);
                x.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    _entityFrameWorkDependencies.DataSetting.MicroServiceName
                );
            });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(_schema);
        
        var mappers =_entityFrameWorkDependencies.ServiceLocator.Current.ResolveAll<IEntityMapper>();
        foreach (var mapper in mappers)
        {
            mapper.Map(modelBuilder);
        }
        
        Console.WriteLine($"Mappers Count : {mappers.Count()}");
        Console.WriteLine($"Entity count : {modelBuilder.Model.GetEntityTypes().Count()}");

        modelBuilder.SetIdAsKeyOnAllEntities();
        modelBuilder.SetSoftFilterOnAllEntities();
        modelBuilder.IgnorePropertyOnAllEntities(nameof(AggregateRoot.Changes));
        modelBuilder.ApplyRowVersionSettingOnAllEntities();
        modelBuilder.DisableIdentitySettingOnAllEntities();

        ConfigureHandleInboxEvent(modelBuilder);
        ConfigurePublishedEventEntity(modelBuilder);
        ConfigurePublisherEventEntity(modelBuilder);
        ConfigureRunningGigRuleSetEntity(modelBuilder);
    }

    private void ConfigureRunningGigRuleSetEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RunningGigRuleSetEntity>(
            entity =>
            {
                entity.ToTable(nameof(RunningGigRuleSetEntity).Replace(EntityString, string.Empty), _schema);
                entity.HasKey(inbox => new { inbox.RuleSetId });
                entity.Property(x => x.RuleSetId).HasColumnType(nameof(SqlDbType.UniqueIdentifier)).IsRequired();
                entity.Property(x => x.HandelRule).HasColumnType(nameof(SqlDbType.Bit));
            });
    }

    private void ConfigurePublisherEventEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublisherEventEntity>(
            entity =>
            {
                entity.ToTable(nameof(PublisherEventEntity).Replace(EntityString, string.Empty), _schema);
                entity.HasKey(inbox => new { inbox.DomainEventId });
                entity.Property(x => x.CreateDateTime).HasColumnType(nameof(SqlDbType.DateTime2));
                entity.Property(x => x.EventContent).HasColumnType("varchar(max)");
                entity.Property(x => x.EventTypeString).HasColumnType(nameof(SqlDbType.NVarChar)).HasMaxLength(500);
                entity.Property(x => x.UserContext).HasColumnName("Token").HasColumnType(nameof(SqlDbType.NVarChar))
                    .HasMaxLength(4000);
                entity.Property(x => x.DomainEventId).HasColumnType(nameof(SqlDbType.UniqueIdentifier)).IsRequired();
                entity.Property(x => x.TransactionEventId).HasColumnType(nameof(SqlDbType.UniqueIdentifier));
            });
    }

    private void ConfigurePublishedEventEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublishedEventEntity>(
            entity =>
            {
                entity.ToTable(nameof(PublishedEventEntity).Replace(EntityString, string.Empty), _schema);
                entity.HasKey(x => new { x.DomainEventId });
                entity.Property(x => x.UpdateDateTime).HasColumnType(nameof(SqlDbType.DateTime2));
                entity.Property(x => x.EventContent).HasColumnType(nameof(SqlDbType.NVarChar)).HasMaxLength(4000);
                entity.Property(x => x.EventTypeString).HasColumnType(nameof(SqlDbType.NVarChar)).HasMaxLength(500);
                entity.Property(x => x.DomainEventId).HasColumnType(nameof(SqlDbType.UniqueIdentifier)).IsRequired();
            });
    }

    private void ConfigureHandleInboxEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HandleInboxEvent>(
            entity =>
            {
                entity.ToTable(nameof(HandleInboxEvent), _schema);
                entity.HasKey(inbox => new { inbox.DomainEventId, inbox.HandlerType });

                entity.Property(x => x.UpdateDateTime).HasColumnType(nameof(SqlDbType.DateTime2));
                entity.Property(x => x.HandlerType).HasColumnType(nameof(SqlDbType.NVarChar)).HasMaxLength(500).IsRequired();
                entity.Property(x => x.DomainEventId).HasColumnType(nameof(SqlDbType.UniqueIdentifier)).IsRequired();
            });
    }

    private async Task<List<AuditEntry>> OnBeforeSaveChanges()
    {
        var userContext = _entityFrameWorkDependencies.RequestContext.GetUserContext();
        var cacheManager = _entityFrameWorkDependencies.DistributeCacheManager;
        if (cacheManager.ExistsKey(
                new AuditingEntityCacheKey(_entityFrameWorkDependencies.DataSetting.MicroServiceName.ToLower(),
                    _entityFrameWorkDependencies.RequestContext.GetUserContext().CompanyId)))
        {
            var auditingEntities =
                await cacheManager.GetAsync<List<string>>(
                    new AuditingEntityCacheKey(_entityFrameWorkDependencies.DataSetting.MicroServiceName.ToLower(),
                        _entityFrameWorkDependencies.RequestContext.GetUserContext().CompanyId));
            if (auditingEntities == null || !auditingEntities.Any()) return null;

            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                var entityName = entry.Entity.GetType().Name.RemoveSuffix("Proxy").ToLower();
                if (entry.Entity is Audit || entry.State == EntityState.Detached ||
                    entry.State == EntityState.Unchanged || auditingEntities.All(x => x != entityName))
                    continue;

                var auditEntry = new AuditEntry(entry, _entityFrameWorkDependencies.Serializer)
                {
                    TableName = entityName,
                    UserId = userContext.UserId
                };
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    var propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }

                            break;
                    }
                }
            }

            return auditEntries;
        }

        return null;
    }
}
