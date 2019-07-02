using System.Threading.Tasks;
using Orleans;

namespace Bancor.Core.Grains.Interfaces
{
    public interface IJournaledAccountGrain : IGrainWithGuidKey
    {
        Task Deposit(decimal amount, string description);
        Task Withdraw(decimal amount, string description);
        Task<decimal> Balance();

        Task HasName(string name);
        Task<string> Name();
        Task AddTransaction(Transaction transaction);
    }
}