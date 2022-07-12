using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Gig.Framework.Api.Security;
using Gig.Framework.Application;
using Gig.Framework.Application.ReadModel;
using Gig.Framework.Config;
using Gig.Framework.Core;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.FacadServices;
using Gig.Framework.Core.Security;
using Gig.Sample.Read.Facade.User;
using Gig.Sample.Read.ReadModels.Services.User;
using Gig.Sample.Write.Applications.Application.User;
using Gig.Sample.Write.Facades.Facade.Services.User;
using Gig.Sample.Write.Infrastructures.Persistence.Repositories.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Gig.Framework.Bus;
using Gig.Framework.Core.Events;
using Gig.Framework.Domain;
using Gig.Framework.Persistence.Ef;
using Gig.Framework.Scheduling;
using Gig.Sample.Write.Domains.Domain.Product.Services;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Quartz;
using Serilog;

namespace Gig.Sample.UI.Config
{
    public static class MyConsumerExtensions
    {
        public static void AddConsume<T>(IRabbitMqReceiveEndpointConfigurator endpoint)
            where T : class, IConsumer, new()
        {
            endpoint.Consumer<T>();
        }
    }

    public static class DependencyConfigurators
    {
        public static void Config(IWindsorContainer builder)
        {
            DependencyConfigurator.Config(builder);
            // var t1 = typeof(UserRepository);
            var t6 = typeof(Sample.Config.DependencyConfigurator);
            var currentAssembly = typeof(Startup).Assembly;
            var assemblies = currentAssembly
                .GetReferencedAssemblies()
                .Select(Assembly.Load).ToArray();

            var types = assemblies.SelectMany(x => x.GetTypes())
                .Where(x => typeof(IConfig).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
            foreach (var type in types)
            {
                ((IConfig) Activator.CreateInstance(type)).Config(builder);
            }
            //builder.Register(Classes.FromAssemblyInThisApplication(typeof(DependencyConfigurators).Assembly)
            // .Where(x => x.Name.EndsWith("EventHandler")).WithServiceAllInterfaces().LifestyleScoped());

            builder.Register(Component.For<IHttpContextAccessor>().ImplementedBy(typeof(HttpContextAccessor))
                .LifestyleScoped());

            builder.Register(Component.For<IIdGenerator>().ImplementedBy(typeof(IdGenerator))
                .LifestyleScoped());

            builder.Register(Component.For<IEventRepository>().ImplementedBy(typeof(EventRepository))
                .LifestyleScoped());

            // 
            builder.Register(Component.For<ISecurityToken>().ImplementedBy(typeof(GigSecurityToken)).LifestyleScoped());

            builder.Register(Classes.FromAssemblyInThisApplication(typeof(DependencyConfigurators).Assembly)
                    .Where(x => x.Name.EndsWith("Job") && x.GetInterfaces().Contains(typeof(IGigJob))).WithServiceSelf().LifestyleScoped());
           
            builder.Register(Classes.FromAssemblyInThisApplication(typeof(DependencyConfigurators).Assembly)
                .Where(x => x.Name.EndsWith("GigScheduleJob") && x.GetInterfaces().Contains(typeof(IGigSchedule))).WithServiceAllInterfaces().LifestyleSingleton());

            //
            builder.Register(Classes.FromAssemblyContaining(typeof(UserQueryCommandHandler))
                .BasedOn<CommandQueryHandler>().WithServiceAllInterfaces().LifestyleScoped());
            

            //builder.Register(Classes.FromAssemblyContaining(typeof(UserRepository))
            //  .BasedOn<IUnitOfWork>()
            //  .WithServiceAllInterfaces().LifestyleScoped());

            builder.Register(Classes.FromAssemblyContaining(typeof(UserCommandHandler))
                .BasedOn<ICommandHandler>().WithServiceAllInterfaces().LifestyleScoped());

            builder.Register(Classes.FromAssemblyContaining(typeof(UserRepository))
                .BasedOn(typeof(IRepository<>))
                .WithServiceAllInterfaces().LifestyleScoped());

            //builder.Register(Classes.FromAssemblyContaining(typeof(UserRepository))
            //  .BasedOn<IUnitOfWork>()
            //  .WithServiceAllInterfaces().LifestyleScoped());


            //builder.Register(Component.For<ReadDbContext>().ImplementedBy(typeof(WpapDevContext)).LifestyleTransient());




            //builder.Register(Classes.FromAssemblyInThisApplication(typeof(DependencyConfigurators).Assembly)
            //    .Where(x => x.Name.EndsWith("QueryHandler")).WithServiceAllInterfaces().LifestyleScoped());

            //builder.Register(Classes.FromAssemblyInThisApplication(typeof(DependencyConfigurators).Assembly)
            //   .Where(x => x.Name.EndsWith("CommandHandler")).WithServiceAllInterfaces().LifestyleScoped());

            builder.Register(Classes.FromAssemblyContaining(typeof(UserQueryFacadeService)).BasedOn<IFacadeService>()
                .WithServiceAllInterfaces().LifestyleScoped());
            builder.Register(Classes.FromAssemblyContaining(typeof(UserFacadeService)).BasedOn<IFacadeService>()
                .WithServiceAllInterfaces().LifestyleScoped());
            builder.Register(Classes.FromAssemblyInThisApplication(typeof(DependencyConfigurators).Assembly)
                .Where(x => x.Name.EndsWith("DomainService")).WithServiceAllInterfaces().LifestyleScoped());

            builder.Register(Component.For<IFakeRegister>().ImplementedBy(typeof(FakeRegister)).LifestyleScoped());

            //Configurations.EndPointName)
            var ass=assemblies.ToList();
            ass.Add(Assembly.GetExecutingAssembly());
            builder.CreateMassTransitBus(new BusConfig("FakeTest")
            {
                ExternalContainer = builder,
                BrokerUrl = "rabbitmq://172.31.0.156",
                BrokerUser = "admin",
                BrokerPassword = "admin"
            }, ass.ToArray());

             builder.StartGigScheduler();
        }
    }

    public class FakeJob : GigJobBase
    {
        private readonly IProductRepository _repository;

        public FakeJob(IProductRepository repository,ILogger logger):base(logger)
        {
            _repository = repository;
        }

        protected override Task Execute(IJobExecutionContext gigJobExecutionContext)
        {
            return Task.CompletedTask;
        }
    }

    public class FakeGigScheduleJob : GigScheduleJob<FakeJob>
    {
        public override ITrigger GetSchedule(TriggerBuilder builder)
        {
            return builder.WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever()).Build();
        }
    }

    public interface IFakeRegister
    {
        long Id { get; set; }

    }
    public class FakeRegister:IFakeRegister
    {
        public long Id { get; set; } = 100;
    }

    public static class ReflectionExtensions
    {
        public static IEnumerable<MethodInfo> GetExtensionMethods(this Type type, Assembly extensionsAssembly)
        {
            var query = from t in extensionsAssembly.GetTypes()
                        where !t.IsGenericType && !t.IsNested
                        from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                        where m.GetParameters()[0].ParameterType == type
                        select m;

            return query;
        }

        public static MethodInfo GetExtensionMethod(this Type type, Assembly extensionsAssembly, string name)
        {
            return type.GetExtensionMethods(extensionsAssembly).FirstOrDefault(m => m.Name == name);
        }

        public static MethodInfo GetExtensionMethod(this Type type, Assembly extensionsAssembly, string name, Type[] types)
        {
            var methods = (from m in type.GetExtensionMethods(extensionsAssembly)
                           where m.Name == name
                           && m.GetParameters().Count() == types.Length + 1 // + 1 because extension method parameter (this)
                           select m).ToList();

            if (!methods.Any())
            {
                return default(MethodInfo);
            }

            if (methods.Count() == 1)
            {
                return methods.First();
            }

            foreach (var methodInfo in methods)
            {
                var parameters = methodInfo.GetParameters();

                bool found = true;
                for (byte b = 0; b < types.Length; b++)
                {
                    found = true;
                    if (parameters[b].GetType() != types[b])
                    {
                        found = false;
                    }
                }

                if (found)
                {
                    return methodInfo;
                }
            }

            return default(MethodInfo);
        }
    }
}