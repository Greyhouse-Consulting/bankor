using System;
using Bancor.Core.Grains.Interfaces;
using Grains.Tests.Hosted.Cluster;
using Shouldly;
using Xunit;

namespace Bancor.Integration.Tests
{
    [Collection(nameof(ClusterCollection))]
    public class JournaledGrainTest : GrainTest, IClassFixture<ClusterFixture>
    {
        private readonly ClusterFixture _fixture;

        public JournaledGrainTest(ClusterFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact]
        public async void Should_Store_One_Event()
        {

            // Arrange
            //await DeployClusterAsync();
            var accountId = Guid.Parse("EBD09F9C-4A99-4B8D-A581-3C93764D24B1");
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IJournaledAccountGrain>(accountId);
            await grain.HasName("asdasd");
            // Act
            await grain.Deposit(200, "deposit");
            await grain.Deposit(200, "deposit");

            // Assert
            (await grain.Balance()).ShouldBe(400);
        }


        [Fact]
        public async void Should_Calculate_Correct_Balance_When_Four_Events_Applied()
        {
            // Arrange
            //await DeployClusterAsync();
            var accountId = Guid.Parse("B274FB90-1213-4333-85A1-1A69FF268022");
            var grain = _fixture.Cluster.GrainFactory.GetGrain<IJournaledAccountGrain>(accountId);

            // Act
            await grain.HasName("Savings account");
            await grain.Deposit(200, "deposit");
            await grain.Deposit(200, "deposit");
            await grain.Withdraw(400, "deposit");
                
            // Assert
            (await grain.Balance()).ShouldBe(0);
        }
    }
}