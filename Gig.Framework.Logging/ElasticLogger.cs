using System;
using System.Threading.Tasks;
using Gig.Framework.Core.Logging;
using Gig.Framework.Core.Settings;
using Nest;

namespace Gig.Framework.Logging
{
    public class ElasticConfig
    {

    }

    public class ElasticLogger 
    {
        private readonly ElasticClient _client;
        private readonly Guid _requestId;

        public ElasticLogger(IDataSetting dataSetting)
        {
            _requestId = Guid.NewGuid();
            var node = new Uri(dataSetting.ElasticUrl);
            var settings = new ConnectionSettings(node);
            _client = new ElasticClient(settings);
        }

        public async Task LogAsync<T>(T logData)
        {
            var logItem = new LogData<T>(logData, _requestId);
            var rs=await _client.IndexAsync(logItem, idx => idx.Index($"gig_log_doc"));
        }
    }
}