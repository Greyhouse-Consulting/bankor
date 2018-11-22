using System.Threading.Tasks;

namespace Bancor.Core.Grains.Interfaces.Repository
{
    public interface ICustomerRepository
    {
        Task<long> Insert(Customer name);
    }
}