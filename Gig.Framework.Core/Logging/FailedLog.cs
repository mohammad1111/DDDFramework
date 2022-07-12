namespace Gig.Framework.Core.Logging;

public class FailedLog<T>
{
    public FailedLog(T logModel, string logType)
    {
        LogModel = logModel;
        LogType = logType;
    }


    public T LogModel { get; set; }

    public string LogType { get; set; }
}