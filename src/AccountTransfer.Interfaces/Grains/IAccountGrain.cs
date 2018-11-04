using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces.Grains
{
    public interface IAccountGrain : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.CreateOrJoin)]
        Task Withdraw(decimal amount);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task Deposit(decimal amount);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<decimal> GetBalance();

        Task Owner(string userId);

        Task  HasNewName(string name);
        Task<string> GetName();
    }
}
