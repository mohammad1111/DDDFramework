using System.Reflection;
using Gig.Framework.Core.Settings;
using Microsoft.Extensions.Configuration;

namespace Gig.Framework.Config;

public class DataSettingReader
{
    private readonly IConfiguration _configuration;

    public DataSettingReader(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public DataSetting Read()
    {
        DataSetting dataSetting = new DataSetting();
        _configuration.GetSection("DataSettings").Bind(dataSetting);
        return dataSetting;
    }
}