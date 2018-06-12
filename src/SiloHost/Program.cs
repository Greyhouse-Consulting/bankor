using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Hosting;
using Orleans.Hosting.Development;
using Orleans.Configuration;
using System.Net;
using BankOr.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NPoco;
using Orleans.Runtime;
using Orleans.Storage;

namespace OrleansSiloHost
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {

            BankorDbFactory.Setup();

            BankorDbFactory.DbFactory.Config();

            var db = BankorDbFactory.DbFactory.Build(new InMemoryDatabase(BankorDbFactory.CreateConnection()));

            var storageProvider = new BankOrStorageProvider(db);
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                //.AddMemoryGrainStorage("DevStore")
                .ConfigureServices(s => s.TryAddSingleton<IGrainStorage>(storageProvider))
                .ConfigureServices(s => s.TryAddSingleton(db))
                .ConfigureServices(s => s.AddSingletonNamedService<IGrainStorage>("DevStore", (x,y) => new BankOrStorageProvider(db)))
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureLogging(logging => logging.AddConsole())
//                .AddMemoryGrainStorageAsDefault()s                
                .UseInClusterTransactionManager()
                .UseInMemoryTransactionLog()
                .UseTransactionalState();

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
