using Gig.Framework.Core.Models;
using Gig.Framework.Workflow.Contract;

namespace Gig.Framework.Workflow;

public interface ICqrsGigWorkflowEngine<in TQuery, in TCommand>:IGigWorkflowEngine<TQuery,  TCommand>
    where TCommand : WorkflowCommand
    where TQuery : WorkflowGigQuery
{

}