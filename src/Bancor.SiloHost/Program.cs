using System;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bancor.Core.Grains;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Infrastructure;
using Bancor.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NPoco;
using Orleans;
using Orleans.Configuration;
using Orleans.EventSourcing.CustomStorage;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;

namespace Bancor.SiloHost
{
    public class Program
    {

        private static readonly AutoResetEvent closing = new AutoResetEvent(false);
        private static ISiloHost silo;

        public static IConfigurationRoot Configuration { get; private set; }
        public static string EnvironmentName { get; private set; }

        public static void Main(string[] args)
        {
            try
            {

                EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (EnvironmentName == "Integration")
                {
                    SqlServerDatabaseFactory.Upgrade();
                }
                else if (EnvironmentName == "Development")
                {
                    BankorDbFactory.Upgrade(BankorDbFactory.DatabaseName);
                }

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
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            IDatabase db;

            var siloHostBuilder = new SiloHostBuilder();

            switch (EnvironmentName)
            {
                case "Integration":
                    siloHostBuilder
                        .UseConsulClustering(options => { options.Address = new Uri("http://consul:8500"); })
                        .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000);
                    db = SqlServerDatabaseFactory.Create();
                    break;
                case "Development":
                    siloHostBuilder
                        .UseLocalhostClustering()
                        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback);
                    db = BankorDbFactory.Create(BankorDbFactory.DatabaseName);
                    break;
                default:
                    throw new Exception($"Unknown environment '{EnvironmentName}'");
            }

            siloHostBuilder.Configure<ClusterOptions>(options =>
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
            //.ConfigureServices(s =>
            //    s.AddSingletonNamedService<ICustomStorageInterface<JournaledAccountGrainState, AccountEvent>>("CustomStorage",
            //        (x, y) => new JournaledAccountGrain()))
            .ConfigureLogging(logging => logging.AddConsole())
            .AddMemoryGrainStorageAsDefault()
            .AddSimpleMessageStreamProvider("SMSProvider")
            .AddMemoryGrainStorage("PubSubStore")
            .AddCustomStorageBasedLogConsistencyProvider("CustomStorage")
            .UseTransactions();

            var host = siloHostBuilder.Build();

            return host;
        }


        private static async Task StartSilo()
        {
            await silo.StartAsync();
            Console.WriteLine("Silo started");
        }
    }

}
