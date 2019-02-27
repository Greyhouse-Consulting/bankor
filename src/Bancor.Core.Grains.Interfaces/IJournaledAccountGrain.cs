using System.Threading.Tasks;
using Orleans;

namespace Bancor.Core.Grains.Interfaces
{
    public interface IJournaledAccountGrain : IGrainWithGuidKey
    {
        Task Deposit(decimal amount);
        Task Withdraw(decimal amount);
        Task<decimal> Balance();

        Task HasName(string name);
    }
}