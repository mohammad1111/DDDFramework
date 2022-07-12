using Gig.Framework.Domain;

namespace Gig.Framework.Workflow.Contract.Exceptions;

public class WorkflowQueryCommandNotDefinedException : DomainException
{
    public WorkflowQueryCommandNotDefinedException() : base("کوئری جمع اوری دیتا تعریف نشده است")
    {
    }
}

public class WorkflowNotValidStateException : DomainException
{
    public WorkflowNotValidStateException() : base("وضعیت معتبری برای این گردش کار وجود ندارد")
    {
    }
}