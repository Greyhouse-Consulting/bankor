using System.Threading.Tasks;
using AccountTransfer.Interfaces.Repository;
using BankOr.Core;
using NPoco;

namespace BankOr.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDatabase _database;

        public CustomerRepository(IDatabase database)
        {
            _database = database;
        }

        public async Task<long> Insert(Customer customer)
        {

            var o = await _database.InsertAsync(customer);

            return customer.Id;
        }
    }
}