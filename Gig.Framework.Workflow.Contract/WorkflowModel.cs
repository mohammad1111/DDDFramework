namespace Gig.Framework.Workflow.Contract;

public abstract class WorkflowExecuteModel
{
    protected WorkflowExecuteModel(StatusData status, Guid workflowInstanceId)
    {
        Status = status;
        WorkflowInstanceId = workflowInstanceId;
    }

    public StatusData Status { get;  }

    public Guid WorkflowInstanceId { get;  }

}