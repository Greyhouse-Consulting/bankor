using Grains.Tests.Hosted.Cluster;
using Xunit;

namespace Bancor.Integration.Tests
{
    [CollectionDefinition(nameof(ClusterCollection))]
    public class ClusterCollection : ICollectionFixture<ClusterFixture>
    {
    }
}