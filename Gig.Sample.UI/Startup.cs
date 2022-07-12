using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.EventBus;
using Gig.Framework.EventBus.Contracts;
using Gig.Sample.UI.Config;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using IEvent = Gig.Framework.EventBus.Contracts.IEvent;

namespace Gig.Sample.UI
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRabbitMq(new BusConfig
            {
                HandlerAssemblies = new[] {Assembly.GetExecutingAssembly()},
                RetryConfig = new RetryConfig
                {
                    CountRetry = 10,
                    Interval = TimeSpan.FromSeconds(10)
                },
                EndPointName = "Inventory",
                RabbitMqConfig = new RabbitMqConfig
                {
                    ServerUrl = "172.0.0.1",
                    Password = "password",
                    UserName = "username"
                }
            });
            
            
            services.AddControllers().AddNewtonsoftJson();
            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrigin",
                    builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("GigSwagger", new OpenApiInfo { Title = "Sales API", Version = "v1" });
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme.",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    Scheme = "Bearer"
                //});
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            },
                //            Scheme = "oauth2",
                //            Name = "Bearer",
                //            In = ParameterLocation.Header,
                //        },
                //        new List<string>()
                //    }
                //});
            });
        }

        public void ConfigureContainer(IWindsorContainer container)
        {
            container.Install(new ManagementSlContainerInstaller());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IWindsorContainer container
            ,IGigEventBus bus)
        {
            
            container.BeginScope();
            container.Resolve<IServiceLocator>();
            container.Resolve<IBusControl>().StartAsync();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseGigScope();
            app.UseRouting();
            
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = @"\d+" });
                endpoints.MapControllerRoute("DefaultApiWithAction", "Api/{controller}/{action}");
                endpoints.MapControllerRoute("DefaultApiGet", "Api/{controller}", new { action = "Get" });
                endpoints.MapControllerRoute("DefaultApiPost", "Api/{controller}", new { action = "Post" });
            });
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => {
                c.DefaultModelsExpandDepth(-1);
                c.SwaggerEndpoint("/swagger/GigSwagger/swagger.json", "FRAMEWORK TEST V2 API");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
