using System.Threading.Tasks;
using Orleans;

namespace Bancor.Core.Grains.Interfaces
{
    public interface IJournaledAccountGrain : IGrainWithIntegerKey
    {
        Task Deposit(decimal amount);
        Task<decimal> Balance();
    }
}