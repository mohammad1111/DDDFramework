using Gig.Framework.Application.ReadModel;

namespace Gig.Framework.Workflow.Contract;

public abstract class WorkflowGigQuery : GigQuery
{
    public Guid WorkflowInstanceId { get; private set; }

    public StatusData Status { get; private set; }

    public void Initial(Guid workflowInstanceId, StatusData status)
    {
        Status = status;
        WorkflowInstanceId = workflowInstanceId;
    }
}