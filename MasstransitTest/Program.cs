using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MasstransitTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) => 
                    configuration.Enrich.FromLogContext().WriteTo.Console())
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

    public class MyDBConetx :DbContext, IUnitOfWork
    {
        public Task BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PublisherEvent>> Commit(IList<GigEntityRulesResult> ruleSetIds = null)
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }
    }

    public class MyDataSetting : IDataSetting
    {
        public string ReadDataConnectionString { get; }
        public string WriteDataConnectionString { get; }
        public string RedisConnection {   get
        {
            return "172.31.0.154";
        } }

        public string RedisPort
        {
            get
            {
                return "6379";
            }
        } 

        public string MicroServiceName { get; }
        public string RedisPassword {  
            get
            {
                return "gB!3$p5ML}c49X/e";
            }
            
        }
        public string ElasticUrl { get; }
        public string InventoryUri { get; }
        public string TokenKey { get; }
        public string TokenIssuer { get; }
        public string TokenAudience { get; }
        public string TokenExpirationMinutes { get; }
        public string SystemToken { get; }
        public string RuleExpireTime { get; }
        public bool ElasticIndexPerMonth { get; }
        public int AmountOfPreviousElasticIndicesUsedInAlias { get; }
        public string ElasticAlias { get; }
        public string RabbitServer { get; }
    }
}