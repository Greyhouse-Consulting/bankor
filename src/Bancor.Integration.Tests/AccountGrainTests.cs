using System;
using Bancor.Core;
using Bancor.Core.Exceptions;
using Bancor.Core.Grains.Interfaces;
using Bancor.Core.Grains.Interfaces.Grains;
using Grains.Tests.Hosted.Cluster;
using Orleans.TestingHost;
using Orleans.Transactions;
using Shouldly;
using Xunit;

namespace Bancor.Integration.Tests
{

    [Collection(nameof(ClusterCollection))]
    public class AccountGrainTests : GrainTest, IClassFixture<ClusterFixture>
    {
        private readonly TestCluster  _cluster;

        public AccountGrainTests(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public async void Should_Store_One_Account()
        {
            // Arrange
            var grain = TestCluster.GrainFactory.GetGrain<IJournaledAccountGrain>(Guid.NewGuid());

            // Act
            await grain.HasName("Savings account");
            var name = await grain.Name();

            // Assert
            name.ShouldBe("Savings account");
        }


        [Fact]
        public async void Should_Throw_If_No_Name_Has_Been_Set()
        {
            // Arrange
            var grain = TestCluster.GrainFactory.GetGrain<IJournaledAccountGrain>(Guid.NewGuid());

            // Act Assert
            grain.Balance().ShouldThrow<GrainDoesNotExistException>();
        }

        [Fact]
        public async void Should_Make_One_Transaction()
        {
            // Arrange
            var grain = _cluster.GrainFactory.GetGrain<IJournaledAccountGrain>(Guid.NewGuid());

            // Act
            await grain.HasName("Savings account");
            await grain.Deposit(200, "The transfer");

            var balance = await grain.Balance();
            // Assert
            balance.ShouldBe(200);
        }
    }
}
