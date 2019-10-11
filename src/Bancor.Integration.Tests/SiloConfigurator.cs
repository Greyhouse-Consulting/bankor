using System.Collections.Generic;
using System.Net;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Infrastructure;
using Bancor.Infrastructure.Abstractions;
using Bancor.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mongo2Go;
using MongoDB.Driver;
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
        public void Configure(ISiloHostBuilder hostBuilder)
        {
            var runner = MongoDbRunner.Start();
            var client = new MongoClient(runner.ConnectionString);
            var database = client.GetDatabase("test");

            hostBuilder
                .ConfigureServices(s => s.TryAddSingleton<IGrainStorage, MongoCustomerStorageProvider>())
                .ConfigureServices(s => s.TryAddTransient<ICustomerRepository, CustomerRepository>())
                .ConfigureServices(s => s.TryAddSingleton(database))
                .ConfigureServices(s => s.TryAddTransient<IJournaldAccountRepository, JournalAccountRepository>())
                //    .ConfigureServices(s =>
                //        s.AddSingletonNamedService<IGrainStorage>("CustomerStorageProvider",
                //            (x, y) => new MongoCustomerStorageProvider(database,
                //                (IGrainFactory)x.GetService(typeof(IGrainFactory)))))
                .AddMemoryGrainStorageAsDefault()
                //    //.AddSimpleMessageStreamProvider("SMSProvider")
                //.AddMemoryGrainStorage("PubSubStore")
                .AddLogStorageBasedLogConsistencyProvider("CustomStorage")
                .AddMemoryGrainStorageAsDefault()
                //.UseLocalhostClustering()
                .UseTransactions();



        }
    }
}