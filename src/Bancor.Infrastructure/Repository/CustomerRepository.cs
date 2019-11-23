using System;
using System.Linq;
using System.Threading.Tasks;
using Bancor.Core;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Infrastructure.MongoEntites;
using MongoDB.Driver;


namespace Bancor.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoDatabase _database;

        public CustomerRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<Guid> Insert(Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task<Customer> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Customer[]> Get()
        {
            try
            {
                var customers = await _database.GetCollection<MongoCustomer>("Customers").Find(Builders<MongoCustomer>.Filter.Empty).ToListAsync();
                return customers.Select(c => new Customer { Id = c.Customer.Id, Name = c.Customer.Name }).ToArray();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}