using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bancor.Core.Events.Account;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Infrastructure;
using Bancor.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;

namespace Bancor.SiloHost
{
    public class Program
    {

        private static readonly AutoResetEvent Closing = new AutoResetEvent(false);
        private static ISiloHost _silo;

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

                _silo = ConfigureSilo();

                Task.Run(StartSilo);

                AppDomain.CurrentDomain.ProcessExit += (object sender, EventArgs e) =>
                {
                    Console.WriteLine("ProcessExit fired");
                    Task.Run(StopSilo);
                };

                Closing.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task StopSilo()
        {
            await _silo.StopAsync();
            Console.WriteLine("Silo stopped");
            Closing.Set();
        }

        private static ISiloHost ConfigureSilo()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            var siloHostBuilder = new SiloHostBuilder();

            BsonClassMap.RegisterClassMap<AccountEvent>(cm =>
            {
                cm.AutoMap();
                cm.SetDiscriminator("AccountEvent");
            });

            IMongoDatabase database;

            switch (EnvironmentName)
            {
                case "Integration":
                    siloHostBuilder
                        .UseConsulClustering(options => { options.Address = new Uri("http://consul:8500"); })
                        .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000);
                    database = new MongoDbFactory().Create();
                    break;
                case "Development":
                    siloHostBuilder
                        .UseLocalhostClustering()
                        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback);
                    database = new MongoDbFactory().Create();
                    break;
                default:
                    throw new Exception($"Unknown environment '{EnvironmentName}'");
            }


            siloHostBuilder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "bancor";
            })
            .ConfigureServices(s => s.TryAddSingleton<IGrainStorage, MongoCustomerStorageProvider>())
            .ConfigureServices(s => s.TryAddTransient<ICustomerRepository, CustomerRepository>())
            .ConfigureServices(s => s.TryAddSingleton(database))
            .ConfigureServices(s => s.TryAddTransient<IJournaldAccountRepository, JournalAccountRepository>())
            .ConfigureServices(s =>
                s.AddSingletonNamedService<IGrainStorage>("CustomerStorageProvider",
                    (x, y) => new MongoCustomerStorageProvider(database,
                        (IGrainFactory)x.GetService(typeof(IGrainFactory)))))
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
            await _silo.StartAsync();
            Console.WriteLine("Silo started");
        }
    }

}
