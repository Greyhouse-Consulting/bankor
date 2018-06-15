using System;
using BankOr.Core;
using BankOr.Infrastructure;
using Xunit;

namespace BankOr.Integration.Tests
{
    public class DbTests
    {
        [Fact]
        public void Should_Do_A_Inmem()
        {

            // Arrange
            BankorDbFactory.Setup();
            var db = BankorDbFactory.DbFactory.GetDatabase();
            db.Insert(new Account { Balance = 200, Name = "Saving account"});

            // Act
            var accountTransactions = db.FetchMultiple<Account, Transaction>(
                "SELECT * FROM ACCOUNTS WHERE ID = @0; SELECT * FROM TRANSACTIONS WHERE AccountId = @0;", 2);

            // Assert
            Assert.Equal(2, accountTransactions.Item1.Count);
        }
    }
}
