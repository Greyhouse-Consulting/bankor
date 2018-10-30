using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces.Grains
{
    public interface IAccountGrain : IGrainWithIntegerKey
    {
        Task Withdraw(decimal amount);

        Task Deposit(decimal amount);

        Task<decimal> GetBalance();

        Task Owner(string userId);
        Task  SetName(string name);
        Task<string> GetName();
    }
}
