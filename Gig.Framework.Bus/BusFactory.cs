using Autofac;
using Automatonymous;

namespace Gig.Framework.Bus;

public static class BusFactory
{
    public static void AddGigSagaStateMachine<TStateMachine, T>(this ContainerBuilder configurator)
        where TStateMachine : class, SagaStateMachine<T>
        where T : class, SagaStateMachineInstance
    {
        // configurator.AddGigSagaStateMachine<TStateMachine, T>();
    }
}