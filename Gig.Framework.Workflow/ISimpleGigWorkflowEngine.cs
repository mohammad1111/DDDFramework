using Gig.Framework.Core.Models;
using Gig.Framework.Workflow.Contract;

namespace Gig.Framework.Workflow;

public interface ISimpleGigWorkflowEngine< TGetModel,TExecuteModel>:IGigWorkflowEngine<TGetModel,TExecuteModel>
    where TGetModel :  WorkflowGetModel
    where TExecuteModel : WorkflowExecuteModel
{
    Func<TGetModel,Task<IList<StatusData>>> GetFunc { get; set; }

    Func<TExecuteModel,Task<GigCommonResultBase>> ExecuteFunc  { get; set; }
}