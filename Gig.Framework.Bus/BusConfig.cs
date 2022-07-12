namespace Gig.Framework.Bus;

public class BusConfig
{
    public BusConfig(string endpointName)
    {
        EndpointName = endpointName;
    }


    public string BrokerUrl { get; set; }


    public string BrokerUser { get; set; }


    public string BrokerPassword { get; set; }


    public bool EnablePersistence { get; set; }


    public string PersistenceConnectionString { get; set; } =
        "Password=gDev*200;Persist Security Info=True;User ID=Develop;Initial Catalog=WpapDev;Data Source=172.31.1.200;MultipleActiveResultSets=True;";


    public string EndpointName { get; }


    public string ErrorQueue { get; set; } = "error";


    public string AuditQueue { get; set; } = "audit";


    public object ExternalContainer { get; set; }
}