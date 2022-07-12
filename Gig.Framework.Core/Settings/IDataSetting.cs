namespace Gig.Framework.Core.Settings;

public interface IDataSetting
{
    string GlobalDataConnectionString { get; }
    
    string ReadDataConnectionString { get; }

    string WriteDataConnectionString { get; }

    string RedisConnection { get; }

    string RedisPort { get; }

    string MicroServiceName { get; }


    string RedisPassword { get; }

    string ElasticUrl { get; }

    string InventoryUri { get; }

    string TokenKey { get; }

    string TokenIssuer { get; }

    string TokenAudience { get; }

    string TokenExpirationMinutes { get; }

    string SystemToken { get; }

    string RuleExpireTime { get; }

    bool ElasticIndexPerMonth { get; }

    int AmountOfPreviousElasticIndicesUsedInAlias { get; }

    string ElasticAlias { get; }

    string RabbitServer { get; }

    string SubSystemId { get; }
}