using Gig.Sample.Write.Infrastructures.Persistence.Context;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gig.Sample.UI.Config
{
    public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "/App_Data/")
                .AddJsonFile("dataSettings.json")
                .Build();
            var cnn = configuration.GetSection("WriteDataConnectionString").Value;
            return new TestDbContext(cnn);
        }
    }
}
