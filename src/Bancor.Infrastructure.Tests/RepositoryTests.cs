using System;
using Bancor.Core;
using Bancor.Infrastructure.Repository;
using Shouldly;
using Xunit;

namespace Bancor.Infrastructure.Tests
{
    public class RepositoryTests
    {
        [Fact]
        public async void Create_Customer()
        {
            // Arrange 
            var dbName = Guid.NewGuid().ToString();
            var db = BankorDbFactory.Create(dbName);
            BankorDbFactory.Upgrade(dbName);

            var r = new CustomerRepository(db);

            // Act
            await r.Insert(new Customer { Name = "Kalle" });

            var c = await r.Get(0);

            // Assert
            c.ShouldNotBeNull();

        }
    }
}
