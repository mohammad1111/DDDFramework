using Gig.Framework.Application;
using Gig.Framework.Application.ReadModel;
using Gig.Framework.Core;
using Gig.Framework.Core.Models;
using Gig.Framework.Workflow.Contract;
using Gig.Framework.Workflow.Contract.Exceptions;

namespace Gig.Framework.Workflow;

public class CqrsGigWorkflowEngine<TQuery, TCommand> : ICqrsGigWorkflowEngine<TQuery, TCommand>
    where TCommand : WorkflowCommand
    where TQuery : WorkflowGigQuery
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryBus _queryBus;
    private readonly IRequestContext _requestContext;

    public CqrsGigWorkflowEngine(IQueryBus queryBus, ICommandBus commandBus, IRequestContext requestContext)
    {
        _queryBus = queryBus;
        _commandBus = commandBus;
        _requestContext = requestContext;
    }

    public async Task<GigCommonResult<IList<StatusData>>> GetAvailableStatus(TQuery query)
    {
        ValidateQueryCommandDefined(query);
        var statusData = await _queryBus.DispatchAsync<TQuery, IList<StatusData>>(query);
        var validStatusData = ValidateResult(statusData, _requestContext);
        return validStatusData;
    }

    public async Task<GigCommonResultBase> Execute(TCommand command)
    {
        var result = await _commandBus.DispatchAsync(command);
        return result;
    }

    private static GigCommonResult<IList<StatusData>> ValidateResult(IList<StatusData> result
        , IRequestContext requestContext)
    {
        var currentUserId = requestContext.GetUserContext().UserId;
        var finalResult = new List<StatusData>();
        foreach (var statusData in result)
        {
            if (statusData.UserIds.Any())
            {
                if (statusData.UserIds.Contains(currentUserId)) finalResult.Add(statusData);

                continue;
            }

            finalResult.Add(statusData);
        }

        if (!result.Any())
        {
            var exception = new WorkflowNotValidStateException();
            return new GigCommonResult<IList<StatusData>>
            {
                Data = null,
                HasError = true,
                FriendlyMessages = new List<BusinessErrorMessage>
                {
                    new()
                    {
                        Exception = exception,
                        Message = exception.Message,
                        ErrorCode = "1015"
                    }
                }
            };
        }

        return new GigCommonResult<IList<StatusData>>
        {
            Data = result,
            HasError = false
        };
    }

    private static void ValidateQueryCommandDefined(TQuery query)
    {
        if (query is null) throw new WorkflowQueryCommandNotDefinedException();
    }


}