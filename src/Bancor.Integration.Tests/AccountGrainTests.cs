using System;
using System.Collections.Generic;
using System.Text;
using Bancor.Core.Grains;
using Bancor.Core.Grains.Interfaces.Grains;
using Microsoft.Extensions.Configuration;
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

            var grain = TestCluster.GrainFactory.GetGrain<IAccountGrain>(2000);

            // Act Assert
            grain.GetBalance().ShouldThrow<OrleansTransactionAbortedException>();
        }

        [Fact]
        public async void Should_Make_One_Transaction()
        {
            // Arrange
            await DeployClusterAsync();

            var grain = TestCluster.GrainFactory.GetGrain<IAccountGrain>(2000);

            // Act
            await grain.HasNewName("Savings account");
            await grain.Deposit(200);

            var balance = await grain.GetBalance();
            // Assert
            balance.ShouldBe(200);
        }
    }
}
