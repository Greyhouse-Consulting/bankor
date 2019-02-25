using Bancor.Core;
using Bancor.Core.Grains.Interfaces;
using Bancor.Core.Grains.Interfaces.Grains;
using Orleans.Transactions;
using Shouldly;
using Xunit;

namespace Bancor.Integration.Tests
{

    public class JournaledGrainTest : GrainTest
    {
        [Fact]
        public async void Should_Store_One_Event()
        {
            // Arrange
            await DeployClusterAsync();

            var grain = TestCluster.GrainFactory.GetGrain<IJournaledAccountGrain>(2000);
                
            // Act
            await grain.Deposit(200);
            await grain.Deposit(200);

            // Assert
            (await grain.Balance()).ShouldBe(500);
        }
    }
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
            await grain.Deposit(200, "The transfer", TransactionType.Default);

            var balance = await grain.GetBalance();
            // Assert
            balance.ShouldBe(200);
        }
    }
}
  