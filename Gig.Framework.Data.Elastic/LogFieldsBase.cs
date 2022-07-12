namespace Gig.Framework.Data.Elastic;

public class LogFieldsBase
{
    public string ActionId { get; set; }
    public string ActionName { get; set; }
    public string RequestId { get; set; }
    public string RequestPath { get; set; }
    public string ConnectionId { get; set; }
    public string MachineName { get; set; }
    public string Environment { get; set; }
}