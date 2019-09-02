using System.Threading.Tasks;
using Orleans.TestingHost;

namespace Bancor.Integration.Tests
{
    public class GrainTest
    {
        protected TestCluster TestCluster { get; set; }

        public async Task DeployClusterAsync()
        {
            var builder = new TestClusterBuilder(1);

            builder.AddSiloBuilderConfigurator<SiloConfigurator>();

            TestCluster = builder.Build();

            await TestCluster.DeployAsync();
        }
    }
}