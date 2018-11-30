using Bancor.Core.Grains.Interfaces.Grains;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Infrastructure;
using Bancor.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NPoco;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;
using Orleans.TestingHost;
using Xunit;

namespace Bancor.Integration.Tests
{
    public class AccountGrainTests
    {
        [Fact]
        public async void Should_Store_One_Account()
        {
            // Arrange
            var builder = new TestClusterBuilder();
            builder.AddSiloBuilderConfigurator<SiloBuilder>();
            var cluster = builder.Build();
            cluster.Deploy();

            var grain = cluster.GrainFactory.GetGrain<IAccountGrain>(2000);

            // Act
            await grain.HasNewName("Savings account");
            var name = await grain.GetName();

            // Assert
            Assert.Equal("Savings account", name);
        }
    }

    public class SiloBuilder : ISiloBuilderConfigurator
    {
        public void Configure(ISiloHostBuilder hostBuilder)
        {
            var db = BankorDbFactory.Create();

            BankorDbFactory.Upgrade();

            hostBuilder
                .ConfigureServices(s => s.TryAddSingleton<IGrainStorage, CustomerStorageProvider>())
                .ConfigureServices(s => s.TryAddTransient<ICustomerRepository, CustomerRepository>())
                .ConfigureServices(s => s.TryAddSingleton<IDatabase>(db))
                .ConfigureServices(s =>
                    s.AddSingletonNamedService<IGrainStorage>("CustomerStorageProvider",
                        (x, y) => new CustomerStorageProvider(db,
                            (IGrainFactory) x.GetService(typeof(IGrainFactory)))))
                .ConfigureServices(s =>
                    s.AddSingletonNamedService<IGrainStorage>("AccountsStorageProvider",
                        (x, y) => new AccountsStorageProvider(db)))
                .UseTransactions();
        }
    }
}
