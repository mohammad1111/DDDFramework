using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using ForkJoint.Api.Components.Activities;
using Gig.Framework.Bus;
using Gig.Framework.Bus.EndpointNameFormatterExtensions;
using Gig.Framework.Bus.PipeLines;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Models;
using Gig.Framework.Core.Security;
using Gig.Framework.Core.Settings;
using GreenPipes;
using MassTransit;
using MasstransitTest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace MasstransitTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "MasstransitTest", Version = "v1"});
            });
               services.AddMassTransit(busConfiguration =>
            {
                busConfiguration.ApplyCustomMassTransitConfiguration();
                busConfiguration.AddDelayedMessageScheduler();
                var assemblies = new[] {Assembly.GetExecutingAssembly(),};
                    
                busConfiguration.AddConsumer<MyHandleMessageConsumer>();
               // busConfiguration.AddConsumer<MyHandleMessageConsumer>();
                
                busConfiguration.UsingRabbitMq((context,
                    config) =>
                {
               //     config.UseDelayedMessageScheduler();
            
                    config.AutoStart = true;
                    config.Durable = false;
                   
                    config.UseMessageRetry(x =>
                    {
                        x.Incremental(100,
                            TimeSpan.FromSeconds(1),
                            TimeSpan.FromMilliseconds(100));
                    });
                    
                    config.UseDelayedMessageScheduler();

                    var endpointName = $"GigSales_MyTestMasstransit";
                    config.UseConsumeFilter(typeof(AuthenticateConsumerFilter<>), context); 
                    config.ApplyCustomBusConfiguration();

                    config.ReceiveEndpoint(endpointName,
                        endpoint =>
                        {
                        
                        });
                    config.Host("rabbitmq://localhost",
                        host =>
                        {
                            host.Username("admin");
                            host.Password("admin");
                        });
                    config.ConfigureEndpoints(context);
                 //   config.PropagateOpenTracingContext(context);
                });
                
            });
        }
        public void ConfigureContainer(ContainerBuilder builder) {
        
            Gig.Framework.Config.DependencyConfigurator.Config(builder);
            builder.RegisterType<MySampleDep>().As<IMySampleDep>().InstancePerLifetimeScope();
            builder.RegisterType<MyDBConetx>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<MyDataSetting>().As<IDataSetting>().InstancePerLifetimeScope();

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IBusControl control,IEventRepository repository)
        {
            control.Start();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MasstransitTest v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }

    public interface IMySampleDep
    {
        Guid Id { get; }
    }

    public class MySampleDep : IMySampleDep
    {
        public Guid Id => Guid.NewGuid();
    }
    
    
    public class MyHandleMessageConsumer : IConsumer<MyTest1>
    {
        private readonly IHandleMessageDependencies _provider;

        public MyHandleMessageConsumer(IHandleMessageDependencies provider)
        {
            _provider = provider;
        }
        public Task Consume(ConsumeContext<MyTest1> context)
        {
            var value=_provider.CacheTokenService.Id;
            throw new NotImplementedException();
        }
    }

 

    public class MyHandleMessage : HandleMessageTest<MyTest1>
    {
        private readonly IHandleMessageDependencies _provider;

        public MyHandleMessage(IHandleMessageDependencies provider):base(provider)
        {
            _provider = provider;
        }
    
        protected override Task Handle(MyTest1 message)
        {
           var s= _provider.CacheTokenService.Token;

            throw new NotImplementedException();
        }
    }
        
        
   
        
}
   namespace Gig.Framework.Bus
    {


        public abstract class HandleMessageTest<TMessage> : IConsumer<TMessage> where TMessage : class, IEvent
        {
            public readonly IHandleMessageDependencies Dep;

            protected HandleMessageTest(IHandleMessageDependencies dep)
            {
                Dep = dep;
            }

            public ConsumeContext<TMessage> ConsumeContext { get; private set; }


            private void SetToken(TMessage message)
            {

            }

            public async Task Consume(ConsumeContext<TMessage> context)
            {
                try
                {
                    var eventMessage = context.Message;

                    SetToken(eventMessage);


                    if (!await CanHandel(eventMessage)) return;

                    ConsumeContext = context;

                    var value = new Tuple<Guid, string>(eventMessage.CorrelationEventId, GetType().ToString());
                    // await _dependencies.CacheManager.AddAsync(new InboxCacheKey(), value);

                    await Handle(context.Message);
                    await SaveInInBox(eventMessage);

                    // _dependencies.Logger.Information(
                    //     "The Message Type:({Type}), Handled EventId:{EventId}  TraceId:{TraceId} in Time:{HandleTime}",
                    //     GetType().Name, eventMessage.CorrelationEventId, _dependencies.RequestContext.GetUserContext().TraceId, DateTime.Now);

                }
                catch (Exception e)
                {
                    // _dependencies.Logger.Information("Handle Event {EventType} Error {Error}", GetType().ToString(),e.ToString());
                    throw;
                }
            }

            private string GetValidToken(string token)
            {
                //  var request = _dependencies.SecurityManager.ValidateWithoutExpireTime(token);
                // var model = new TokenClaimModel
                // {
                //     UserId = request.UserId,
                //     LastName = request.DisplayName,
                //     CompanyId = request.CompanyId,
                //     BranchId = request.BranchId,
                //     LangTypeCode = 1,
                //     ListOfUserCompanies = request.UserCompanies,
                //     IsAdmin = request.IsAdmin,
                // };

                var options = new BearerTokenOptionsModel
                {
                    Issuer = "Any",
                    Audience = "Any",
                    Key = "386B17FD36C9176795BF91ABEEC45",
                    AccessTokenExpirationMinutes = 360,
                    RefreshTokenExpirationMinutes = 360
                };

                return null; // _dependencies.SecurityManager.GetAccessToken(model, options);
            }

            private async Task SaveInInBox(TMessage eventMessage)
            {
                if (eventMessage is IEvent domainEvent)
                {
                    var eventId = domainEvent.CorrelationEventId;
                    // if (await CanHandel(eventMessage))
                    //   await _dependencies.EventRepository.SaveInBoxEvent(eventId, GetType().ToString());
                }
            }

            private async Task<bool> CanHandel(TMessage eventMessage)
            {
                var type = GetType().ToString();
                if (eventMessage is IEvent domainEvent)
                {
                    var eventId = domainEvent.CorrelationEventId;
                    // if (await _dependencies.EventRepository.IsHandelEvent(eventId, type))
                    // {
                    //     _dependencies.Logger.Error(
                    //         "The Message With Type:({Type}) is already Handled EventId:{EventId}  TraceId:{TraceId}",
                    //         GetType().Name,
                    //         eventMessage.CorrelationEventId, _dependencies.RequestContext.GetUserContext().TraceId);
                    //     return false;
                    // }
                }

                // _dependencies.Logger.Information(
                //     "The Message Type:({Type}) Start Handling  EventId:{EventId}  TraceId:{TraceId}", GetType().Name,
                //     eventMessage.CorrelationEventId, _dependencies.RequestContext.GetUserContext().TraceId);
                return true;
            }

            protected abstract Task Handle(TMessage message);

        }
    }
namespace ForkJoint.Api.Components.Activities
{
  
    public class MyTest1:IEvent
    {
        public Guid CorrelationEventId { get; set; } = NewId.NextGuid();
        public string Token { get; set; }
    }

   
    
}