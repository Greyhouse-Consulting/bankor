using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using Orleans.Configuration;
using System.Net;
using AccountTransfer.Interfaces.Repository;
using BankOr.Infrastructure;
using BankOr.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NPoco;
using Orleans;
using Orleans.Runtime;
using Orleans.Storage;
using Orleans.Versions;
using Orleans.Versions.Compatibility;
using Orleans.Versions.Selector;

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

            var db = BankorDbFactory.Create();

            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .AddMemoryGrainStorage("DevStore")
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
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureLogging(logging => logging.AddConsole())
                .AddMemoryGrainStorageAsDefault()
                .AddSimpleMessageStreamProvider("SMSProvider")
                .AddMemoryGrainStorage("PubSubStore")
                .UseTransactions();

            var host = builder.Build();

            await host.StartAsync();
            return host;
        }
    }

}
