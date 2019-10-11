using System;
using System.Threading.Tasks;
using Grains.Tests.Hosted.Cluster;
using Orleans.TestingHost;
using Xunit;

namespace Bancor.Integration.Tests
{
    public class GrainTest : IDisposable
    {

        public GrainTest()
        {
            var builder = new TestClusterBuilder(1);

            builder.AddSiloBuilderConfigurator<SiloConfigurator>();

            TestCluster = builder.Build();

            TestCluster.Deploy();

        }
        protected TestCluster TestCluster { get; set; }

        public void Dispose()
        {
            TestCluster.StopAllSilos();
        }
    }

}