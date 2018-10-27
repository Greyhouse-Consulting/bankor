using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces.Grains
{
    public interface IBankAccountGrain : IGrainWithGuidKey
    {
        Task Deposit(decimal amount);
        Task Owner(string userId);
        Task<string> GetOwner();
    }
}