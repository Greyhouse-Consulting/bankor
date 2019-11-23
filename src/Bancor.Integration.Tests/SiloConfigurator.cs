using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Infrastructure;
using Bancor.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mongo2Go;
using MongoDB.Driver;
using Orleans.Hosting;
using Orleans.Storage;
using Orleans.TestingHost;

namespace Bancor.Integration.Tests
{
    public class SiloConfigurator : ISiloBuilderConfigurator
    {
        public static MongoDbRunner Runner { get; set; }
        public void Configure(ISiloHostBuilder hostBuilder)
        {
            Runner = MongoDbRunner.Start();
            var client = new MongoClient(Runner.ConnectionString);
            var database = client.GetDatabase("test");

            hostBuilder
                .ConfigureServices(s => s.TryAddSingleton<IGrainStorage, MongoCustomerStorageProvider>())
                .ConfigureServices(s => s.TryAddTransient<ICustomerRepository, CustomerRepository>())
                .ConfigureServices(s => s.TryAddSingleton(database))
                .ConfigureServices(s => s.TryAddTransient<IJournaldAccountRepository, JournalAccountRepository>())
                .AddLogStorageBasedLogConsistencyProvider("CustomStorage")
                .AddMemoryGrainStorageAsDefault()
                //.UseLocalhostClustering()
                .UseTransactions();
        }
    }
}