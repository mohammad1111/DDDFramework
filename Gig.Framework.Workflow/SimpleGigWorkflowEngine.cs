using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gig.Framework.Core;
using Gig.Framework.Core.Exceptions;
using Gig.Framework.Core.Models;
using Gig.Framework.Workflow.Contract;
using Gig.Framework.Workflow.Contract.Exceptions;

namespace Gig.Framework.Workflow;

public class SimpleGigWorkflowEngine<TGet, TExecute> : ISimpleGigWorkflowEngine<TGet, TExecute>
    where TGet : WorkflowGetModel
    where TExecute : WorkflowExecuteModel

{
    private readonly IRequestContext _requestContext;

    public SimpleGigWorkflowEngine(IRequestContext requestContext)
    {
        _requestContext = requestContext;
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


    public async Task<GigCommonResultBase> Execute(TExecute query)
    {
        if (ExecuteFunc == null)
        {
            throw new FrameworkException("");
        }

        var result = await ExecuteFunc(query);
        return result;

    }

    public async Task<GigCommonResult<IList<StatusData>>> GetAvailableStatus(TGet query)
    {
        
        var statusData = await GetFunc(query);
        var validStatusData = ValidateResult(statusData, _requestContext);
        return validStatusData;
    }

    public Func<TGet, Task<IList<StatusData>>> GetFunc { get; set; }
    public Func<TExecute, Task<GigCommonResultBase>> ExecuteFunc { get; set; }
}