namespace Gig.Framework.Data.Elastic;

public class CrudAuditLogElasticDocument : SerilogElasticDocumentBase
{
    //todo: find a way to get typed result from serilog logs
    // public CrudAuditLogFields Fields { get; set; }
    public Dictionary<string, object> Fields { get; set; }
}