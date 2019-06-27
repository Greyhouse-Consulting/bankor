using System.Net;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Infrastructure;
using Bancor.Infrastructure.Abstractions;
using Bancor.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NPoco;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;
using Orleans.TestingHost;

namespace Bancor.Integration.Tests
{
    public class SiloConfigurator : ISiloBuilderConfigurator
    {
        public static IDatabase Db { get; set; }
        public void Configure(ISiloHostBuilder hostBuilder)
        {

            //hostBuilder
            //    .ConfigureServices(s => s.TryAddSingleton<IGrainStorage, MongoCustomerStorageProvider>())
            //    .ConfigureServices(s => s.TryAddTransient<ICustomerRepository, CustomerRepository>())
            //    .ConfigureServices(s => s.TryAddSingleton<IDatabase>(Db))
            //    .ConfigureServices(s => s.TryAddSingleton<IJournaldAccountRepository, JournalAccountRepositoryInMemory>())
            //    .ConfigureServices(s => s.TryAddSingleton<IMongoDbFactory, MongoDbInmemoryFactory>())
            //    .ConfigureServices(s => s.AddSingleton(x => x.GetService<IMongoDbFactory>().Create()))
            //    .ConfigureServices(s =>
            //        s.AddSingletonNamedService<IGrainStorage>("CustomerStorageProvider",
            //            (x, y) => new MongoCustomerStorageProvider(Db,
            //                (IGrainFactory)x.GetService(typeof(IGrainFactory)))))
            //    .ConfigureServices(s =>
            //        s.AddSingletonNamedService<IGrainStorage>("AccountsStorageProvider",
            //            (x, y) => new AccountsStorageProvider(Db)))
            //    .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
            //    .AddCustomStorageBasedLogConsistencyProvider("CustomStorage")
            //    .UseTransactions();
        }
    }
}