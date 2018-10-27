using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces.Grains
{
    public interface IAccountGrain : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        Task Withdraw(uint amount);

        [Transaction(TransactionOption.Create)]
        Task Deposit(uint amount);

        Task<decimal> GetBalance();

        Task Owner(string userId);
        Task  SetName(string name);
        Task<string> GetName();
    }
}
