using System;
using Gig.Framework.Application;
using Gig.Framework.Core.Exceptions;
using Gig.Framework.Domain;

namespace Gig.Framework.Workflow.Contract;

public abstract class WorkflowCommand : ICommand
{
    public StatusData Status { get; private set; }

    public Guid WorkflowInstanceId { get; set; }

    public void Initial(Guid workflowInstanceId, StatusData status = null)
    {
        Status = status;
        WorkflowInstanceId = workflowInstanceId;
    }

    public void Validate()
    {
        if (Status is null) throw new FrameworkException("Status not defined");
    }
}