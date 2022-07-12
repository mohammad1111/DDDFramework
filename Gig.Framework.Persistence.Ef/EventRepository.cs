using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Settings;
using Gig.Framework.Persistence.Ef.CacheKeys;
using Gig.Framework.Persistence.Ef.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Gig.Framework.Persistence.Ef;

public class EventRepository : IEventRepository
{
    private readonly IDistributeCacheManager _cacheManager;
    private readonly IDataSetting _dataSetting;
    private readonly IUnitOfWorkProvider _unitOfWorkProvider;

    public EventRepository(IDataSetting dataSetting, IUnitOfWorkProvider unitOfWorkProvider,
        IDistributeCacheManager cacheManager)
    {
        _dataSetting = dataSetting;
        _unitOfWorkProvider = unitOfWorkProvider;
        _cacheManager = cacheManager;
    }

    public async Task<bool> IsHandelEvent(Guid eventId, string type)
    {
        var isHandel = await DbContext().Set<HandleInboxEvent>()
            .AnyAsync(x => x.DomainEventId == eventId && x.HandlerType == type);
        return isHandel;
    }

    public async Task SaveInBoxEvent(Guid domainEventId, string type)
    {
        try
        {
            await using var sqlConnection = new SqlConnection(_dataSetting.WriteDataConnectionString);
            var command =
                $@"INSERT INTO {_dataSetting.MicroServiceName}.HandleInboxEvent(DomainEventId,HandlerType,UpdateDateTime)
                VALUES('{domainEventId}','{type}',GETDATE())";
            var sqlCommand = new SqlCommand(command, sqlConnection);
            await sqlConnection.OpenAsync();
            await sqlCommand.ExecuteNonQueryAsync();
            // await Set.AddAsync(new HandleInboxEvent
            // {
            //     HandlerType = type,
            //     DomainEventId = domainEventId,
            //     UpdateDateTime = DateTime.Now
            // });
            // await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task CompleteEvent(PublisherEvent @event)
    {
        try
        {
            await using var sqlConnection = new SqlConnection(_dataSetting.WriteDataConnectionString);
            await sqlConnection.OpenAsync();
            var sqlCommand = new SqlCommand($@"INSERT INTO {_dataSetting.MicroServiceName}.PublishedEvent
            (
                EventContent,
                EventTypeString,
                DomainEventId,
                UpdateDateTime
            )
            VALUES
              ('{@event.EventContent}',
                N'{@event.EventTypeString}',
                N'{@event.DomainEventId}', 
                '{DateTime.Now:yyyy-MM-dd HH:mm:ss}')", sqlConnection);
            await sqlCommand.ExecuteNonQueryAsync();
        }
        catch (Exception)
        {
            var cacheKey = new FailedEventPublishedCompleteCacheKey();
            if (_cacheManager.ExistsKey(cacheKey))
            {
                var result =
                    await _cacheManager.GetAsync<List<PublisherEvent>>(new FailedEventPublishedCompleteCacheKey());
                result.Add(@event);
                await _cacheManager.UpdateAsync(new FailedEventPublishedCompleteCacheKey(),
                    new FailedEventPublishedCompleteCacheKey());
                return;
            }

            await _cacheManager.AddAsync(new FailedEventPublishedCompleteCacheKey(),
                new FailedEventPublishedCompleteCacheKey());
        }
    }

    public async Task<List<PublisherEvent>> GetEvents()
    {
        var publisherEvents = new List<PublisherEvent>();
        var sql =
            $@"SELECT pe.DomainEventId,pe.EventContent,pe.EventTypeString,pe.CreateDateTime,Token,TransactionEventId
                FROM {_dataSetting.MicroServiceName}.PublisherEvent pe
                LEFT JOIN {_dataSetting.MicroServiceName}.PublishedEvent pe2 ON pe2.DomainEventId = pe.DomainEventId
                WHERE pe.CreateDateTime<'{DateTime.Now.AddMinutes(-2):yyyy-MM-dd hh:mm:ss}' AND pe2.DomainEventId IS NULL";
        await using var sqlConnection = new SqlConnection(_dataSetting.WriteDataConnectionString);
        await sqlConnection.OpenAsync();
        var sqlCommand = new SqlCommand(sql, sqlConnection);
        await using var dataReader = await sqlCommand.ExecuteReaderAsync();
        if (dataReader != null)
            while (dataReader.Read())
            {
                var publisherEvent = new PublisherEvent(Guid.Parse(dataReader[0].ToString()),
                    "",
                    dataReader[2].ToString(),
                    dataReader[3].ToString(),
                    DateTime.Parse(dataReader[4].ToString()),
                    string.IsNullOrEmpty(dataReader[6].ToString())
                        ? null
                        : Guid.Parse(dataReader[6].ToString())
                );
                publisherEvents.Add(publisherEvent);
            }

        return publisherEvents;
    }

    public async Task<List<PublisherEvent>> GetEvents(Guid transactionId)
    {
        var publisherEvents = new List<PublisherEvent>();
        var sql =
            $@"SELECT pe.DomainEventId,pe.EventContent,pe.EventTypeString,pe.CreateDateTime,Token,TransactionEventId
                FROM {_dataSetting.MicroServiceName}.PublisherEvent pe
                    LEFT JOIN {_dataSetting.MicroServiceName}.PublishedEvent pe2 ON pe2.DomainEventId = pe.DomainEventId
                    WHERE pe.TransactionEventId='{transactionId}'";
        await using var sqlConnection = new SqlConnection(_dataSetting.WriteDataConnectionString);
        await sqlConnection.OpenAsync();
        var sqlCommand = new SqlCommand(sql, sqlConnection);
        await using var dataReader = await sqlCommand.ExecuteReaderAsync();
        if (dataReader != null)
            while (dataReader.Read())
            {
                var publisherEvent = new PublisherEvent(Guid.Parse(dataReader[0].ToString()),
                    "",
                    dataReader[2].ToString(),
                    dataReader[3].ToString(),
                    DateTime.Parse(dataReader[4].ToString()),
                    Guid.Parse(dataReader[6].ToString())
                );
                publisherEvents.Add(publisherEvent);
            }

        return publisherEvents;
    }

    public async Task<PublisherEvent> GetEvent(Guid domainEventId)
    {
        var publisherEvents = new List<PublisherEvent>();
        return await DbContext().Set<PublisherEvent>().FirstOrDefaultAsync(x => x.DomainEventId == domainEventId);
    }

    private DbContext DbContext()
    {
        return (DbContext)_unitOfWorkProvider.GetUnitOfWork();
    }
}