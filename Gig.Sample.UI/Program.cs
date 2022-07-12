using System;
using System.Transactions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Gig.Sample.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
          
            var s = JsonConvert.SerializeObject(new MyMainObject());
            var ret = JsonConvert.DeserializeObject(s,typeof(ISer));
              CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var tr=Transaction.Current;
            using (var t = new TransactionScope(TransactionScopeOption.Required,TransactionScopeAsyncFlowOption.Enabled))
            {
                var tr1 = Transaction.Current;
                

            }
            return Host.CreateDefaultBuilder(args)
                .UseWindsorContainerServiceProvider()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }

    public interface ISer
    {
        MyRec GetMyRec();
    }


    public class MyMainObject:ISer
    {
        public Guid Id { get; set; }=Guid.NewGuid();


        public MyRec Rec { get; set; }= new MyRec();


        public MyRec GetMyRec()
        {
            return Rec;
        }
    }

    public class MyRec
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

}

