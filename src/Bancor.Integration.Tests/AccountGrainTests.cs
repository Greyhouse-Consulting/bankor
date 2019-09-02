using System;
using Bancor.Core;
using Bancor.Core.Grains.Interfaces;
using Bancor.Core.Grains.Interfaces.Grains;
using Orleans.Transactions;
using Shouldly;
using Xunit;

namespace Bancor.Integration.Tests
{
    public class AccountGrainTests : GrainTest
    {
        [Fact]
        public async void Should_Store_One_Account()
        {
            // Arrange
            await DeployClusterAsync();

            var grain = TestCluster.GrainFactory.GetGrain<IAccountGrain>(2000);

            // Act
            await grain.HasNewName("Savings account");
            var name = await grain.GetName();

            // Assert
            name.ShouldBe("Savings account");
        }


        [Fact]
        public async void Should_Throw_If_No_Name_Has_Been_Set()
        {
            // Arrange
            await DeployClusterAsync();

            var grain = TestCluster.GrainFactory.GetGrain<IJournaledAccountGrain>(Guid.NewGuid());

            // Act Assert
            grain.Balance().ShouldThrow<OrleansTransactionAbortedException>();
        }

        [Fact]
        public async void Should_Make_One_Transaction()
        {
            // Arrange
            await DeployClusterAsync();

            var grain = TestCluster.GrainFactory.GetGrain<IJournaledAccountGrain>(Guid.NewGuid());

            // Act
            await grain.HasName("Savings account");
            await grain.Deposit(200, "The transfer");

            var balance = await grain.Balance();
            // Assert
            balance.ShouldBe(200);
        }
    }
}
