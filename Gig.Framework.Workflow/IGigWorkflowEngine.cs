using Gig.Framework.Core.Models;
using Gig.Framework.Workflow.Contract;

namespace Gig.Framework.Workflow;

public interface IGigWorkflowEngine<in TGetModel, in TExecuteModel>
{
    Task<GigCommonResultBase> Execute(TExecuteModel query);
    
    Task<GigCommonResult<IList<StatusData>>> GetAvailableStatus(TGetModel query);
}