using System.Threading.Tasks;
using BankOr.Core;

namespace AccountTransfer.Interfaces.Repository
{
    public interface ICustomerRepository
    {
        Task<long> Insert(Customer name);
    }
}