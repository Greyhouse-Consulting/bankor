using System.Threading.Tasks;
using Orleans;

namespace Bancor.Core.Grains.Interfaces.Grains
{
    public interface IAccountGrain : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.CreateOrJoin)]
        Task Withdraw(decimal amount);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task Deposit(decimal amount);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<decimal> GetBalance();

        Task  HasNewName(string name);
        Task<string> GetName();
    }
}
