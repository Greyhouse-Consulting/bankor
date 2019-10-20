using System;
using System.Threading.Tasks;

namespace Bancor.Core.Grains.Interfaces.Repository
{
    public interface ICustomerRepository
    {
        Task<Guid> Insert(Customer name);
        Task<Customer> Get(int id);
        Task<Customer[]> Get();
    }
}