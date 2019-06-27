using System;
using System.Threading.Tasks;
using Bancor.Core;
using Bancor.Core.Grains.Interfaces.Repository;
using NPoco;

namespace Bancor.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDatabase _database;

        public CustomerRepository(IDatabase database)
        {
            _database = database;
        }

        public async Task<Guid> Insert(Customer customer)
        {

            var o = await _database.InsertAsync(customer);

            return customer.Id;
        }

        public async Task<Customer> Get(int id)
        {
            return await _database.SingleByIdAsync<Customer>(id);
        }
    }
}