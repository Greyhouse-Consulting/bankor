using System;
using BankOr.Infrastructure;
using Xunit;

namespace BankOr.Integration.Tests
{
    public class DbTests
    {
        [Fact]
        public void Should_Do_A_Inmem()
        {
            BankorDbFactory.Setup();
            var db = BankorDbFactory.DbFactory.GetDatabase();

        }
    }
}
