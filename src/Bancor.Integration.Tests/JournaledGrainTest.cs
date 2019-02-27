using System;
using Bancor.Core.Grains.Interfaces;
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
            var accountId = Guid.Parse("EBD09F9C-4A99-4B8D-A581-3C93764D24B1");
            var grain = TestCluster.GrainFactory.GetGrain<IJournaledAccountGrain>(accountId);

            // Act
            await grain.Deposit(200);
            await grain.Deposit(200);

            // Assert
            (await grain.Balance()).ShouldBe(500);
        }


        [Fact]
        public async void Should_Calculate_Correct_Balance_When_Four_Events_Applied()
        {
            // Arrange
            await DeployClusterAsync();
            var accountId = Guid.Parse("B274FB90-1213-4333-85A1-1A69FF268022");
            var grain = TestCluster.GrainFactory.GetGrain<IJournaledAccountGrain>(accountId);

            // Act
            await grain.HasName("Savings account");
            await grain.Deposit(200);
            await grain.Deposit(200);
            await grain.Withdraw(400);
                
            // Assert
            (await grain.Balance()).ShouldBe(0);
        }
    }
}