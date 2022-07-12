using Gig.Framework.Core.DataProviders.Elastic;
using Gig.Framework.Data.Elastic;
using Gig.Framework.ReadModel.Models;

namespace Gig.Framework.Api.Controllers.AuditLog;

public class WebApplicationLogViewModel : ViewModel
{
    public string Level { get; set; }
    public string MessageTemplate { get; set; }
    public string Message { get; set; }
    public string ActionId { get; set; }
    public string ActionName { get; set; }
    public string RequestId { get; set; }
    public string RequestPath { get; set; }
    public string ConnectionId { get; set; }
    public string MachineName { get; set; }
    public string Environment { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public static WebApplicationLogViewModel FromElasticDocument(WebApplicationLogElasticDocument document)
    {
        //todo: find a way to get typed result from serilog logs
        var fields = document.Fields.ToAnonymousObject();
        return new WebApplicationLogViewModel
        {
            ActionId = fields.ActionId,
            ActionName = fields.ActionName,
            RequestId = fields.RequestId,
            RequestPath = fields.RequestPath,
            ConnectionId = fields.ConnectionId,
            MachineName = fields.MachineName,
            Environment = fields.Environment,
            Level = document.Level,
            MessageTemplate = document.MessageTemplate,
            Message = document.Message,
            Timestamp = document.Timestamp
        };
    }
}