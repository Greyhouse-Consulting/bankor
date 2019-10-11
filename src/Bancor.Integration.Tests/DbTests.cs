using System;
using Bancor.Core;
using Bancor.Infrastructure;
using Shouldly;
using Xunit;

namespace Bancor.Integration.Tests
{
    //public class DbTests
    //{
    //    [Fact]
    //    public void Should_Do_A_Inmem()
    //    {
    //        // Arrange
    //        var dbName = Guid.NewGuid().ToString();
    //        var db = BankorDbFactory.Create(dbName);
    //        BankorDbFactory.Upgrade(dbName);

    //        db.Insert(new Account { Balance = 200, Name = "Saving account" });

    //        // Act
    //        var accountTransactions = db.FetchMultiple<Account, Transaction>(
    //            "SELECT * FROM ACCOUNTS WHERE ID = @0; SELECT * FROM TRANSACTIONS WHERE AccountId = @0;", 0);

    //        // Assert
    //        accountTransactions.Item1.ShouldHaveSingleItem("Since only one item stored");

    //    }
    //}
}
