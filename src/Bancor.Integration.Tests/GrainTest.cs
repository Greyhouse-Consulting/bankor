using System;
using System.Threading.Tasks;
using Bancor.Infrastructure;
using Orleans.TestingHost;

namespace Bancor.Integration.Tests
{
    public class GrainTest
    {
        protected TestCluster TestCluster { get; set; }

        public async Task DeployClusterAsync()
        {
            var dbName = Guid.NewGuid().ToString();
            var db = BankorDbFactory.Create(dbName);
            BankorDbFactory.Upgrade(dbName);
            SiloConfigurator.Db = db;

            var builder = new TestClusterBuilder(1);

            builder.AddSiloBuilderConfigurator<SiloConfigurator>();

            TestCluster = builder.Build();

            await TestCluster.DeployAsync();
        }
    }
}