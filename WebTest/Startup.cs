using System.Reflection;
using Gig.Framework.Core.Events;
using Gig.Framework.EventBus;
using Gig.Framework.EventBus.Contracts;
using MassTransit;
using Microsoft.OpenApi.Models;

namespace WebTest;

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
        services.AddRabbitMq(new BusConfig
        {
            HandlerAssemblies = new[] { Assembly.GetExecutingAssembly() },
            EndPointName = "test",
            RabbitMqConfig = new RabbitMqConfig
            {
                ServerUrl = "rabbitmq://172.31.0.156",
                Password = "admin",
                UserName = "admin"
            }
        });
        services.AddControllers();
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebTest", Version = "v1" }); });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IGigBus bus)
    {
        bus.Start();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebTest v1"));
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public class TestEvent
    {
        public long Id { get; set; }
    }

    public class MyEvent : IEvent
    {
        public string Name { get; set; }

        public long CompanyId { get; set; }
        public long UserId { get; set; }
        public long BranchId { get; set; }
        public long LangTypeCode { get; set; }
        public long SubSystemId { get; set; }

        public bool IsAdmin { get; set; }
        public Guid CorrelationEventId { get; set; }
    }

    public class NyEventHandler : GigHandlerMessage<MyEvent>
    {
        public NyEventHandler(IGigEventBus bus) : base(bus)
        {
        }

        protected override Task Handler(MyEvent message)
        {
            return Task.CompletedTask;
        }
    }

    public class TestMohammadHandle : IConsumer<TestEvent>
    {
        public Task Consume(ConsumeContext<TestEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}