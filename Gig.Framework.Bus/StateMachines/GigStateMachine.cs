using Automatonymous;

namespace Gig.Framework.Bus.StateMachines;

public class GigStateMachine<TInstance> : MassTransitStateMachine<TInstance>
    where TInstance : class, SagaStateMachineInstance
{
}