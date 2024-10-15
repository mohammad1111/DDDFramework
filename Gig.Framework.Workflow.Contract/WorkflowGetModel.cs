using System;

namespace Gig.Framework.Workflow.Contract;

public abstract class WorkflowGetModel
{
    protected WorkflowGetModel(Guid workflowInstanceId, StatusData status)
    {
        WorkflowInstanceId = workflowInstanceId;
        Status = status;
    }

    public Guid WorkflowInstanceId { get; }

    public StatusData Status { get;  }
}