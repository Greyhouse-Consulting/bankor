using BankOr.Infrastructure.Repository;
using Xunit;

namespace BankOr.Infrastructure.Tests
{
    public class RepositoryTests
    {
        [Fact]
        public async void Create_Customer()
        {

            var db = BankorDbFactory.Create();
   
            var r = new CustomerRepository(db);


            var result = await r.Insert(new Core.Customer { Name = "Kalle" });




        }
    }
}
