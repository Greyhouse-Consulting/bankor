using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Infrastructure;
using Bancor.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NPoco;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;

namespace Bancor.SiloHost
{
    public class Program
    {

        private static readonly AutoResetEvent closing = new AutoResetEvent(false);
        private static ISiloHost silo;

        public static void Main(string[] args)
        {
            try
            {
                SqlServerDatabaseFactory.Upgrade();

                silo = ConfigureSilo();

                Task.Run(StartSilo);

                AppDomain.CurrentDomain.ProcessExit += (object sender, EventArgs e) =>
                {
                    Console.WriteLine("ProcessExit fired");
                    Task.Run(StopSilo);
                };

                closing.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task StopSilo()
        {
            await silo.StopAsync();
            Console.WriteLine("Silo stopped");
            closing.Set();
        }

        private static ISiloHost ConfigureSilo()
        {

            var db = SqlServerDatabaseFactory.Create();

            var builder = new SiloHostBuilder()
//                .UseLocalhostClustering()
                .UseConsulClustering(options =>
                {
                    options.Address = new Uri("http://consul:8500");
                })
                //.UseAdoNetClustering(options =>
                //    {
                //        options.ConnectionString = "Server=db;Database=master;User=sa;Password=MyPassword001;";
                //        options.Invariant = "System.Data.SqlClient";
                //    })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "bancor";
                })
                .ConfigureServices(s => s.TryAddSingleton<IGrainStorage, CustomerStorageProvider>())
                .ConfigureServices(s => s.TryAddTransient<ICustomerRepository, CustomerRepository>())
                .ConfigureServices(s => s.TryAddSingleton<IDatabase>(db))
                .ConfigureServices(s =>
                    s.AddSingletonNamedService<IGrainStorage>("CustomerStorageProvider",
                        (x, y) => new CustomerStorageProvider(db,
                            (IGrainFactory)x.GetService(typeof(IGrainFactory)))))
                .ConfigureServices(s =>
                    s.AddSingletonNamedService<IGrainStorage>("AccountsStorageProvider",
                        (x, y) => new AccountsStorageProvider(db)))
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Any)
                .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                .ConfigureLogging(logging => logging.AddConsole())
                .AddMemoryGrainStorageAsDefault()
                .AddSimpleMessageStreamProvider("SMSProvider")
                .AddMemoryGrainStorage("PubSubStore")
                .UseTransactions();

            var host = builder.Build();

            return host;
        }


        private static async Task StartSilo()
        {
            await silo.StartAsync();
            Console.WriteLine("Silo started");
        }
    }

}
