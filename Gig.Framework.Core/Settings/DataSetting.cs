namespace Gig.Framework.Core.Settings;

public class DataSetting : IDataSetting
{
    public string GlobalDataConnectionString { get; set; } 
    public string ReadDataConnectionString { get; set; }

    public string WriteDataConnectionString { get; set; }

    public string RedisConnection { get; set; }

    public string RedisPort { get; set; }


    public string MicroServiceName { get; set; }


    public string RedisPassword { get; set; }

    public string ElasticUrl { get; set; }

    public string InventoryUri { get; set; }


    public string TokenKey { get; set; }


    public string TokenIssuer { get; set; }


    public string TokenAudience { get; set; }


    public string TokenExpirationMinutes { get; set; }


    public string SystemToken { get; set; }

    public string RuleExpireTime { get; set; }
    public bool ElasticIndexPerMonth { get; set; }
    public int AmountOfPreviousElasticIndicesUsedInAlias { get; set; }
    public string ElasticAlias { get; set; }

    public string RabbitServer { get; set; }

    public string SubSystemId { get; set; }
}