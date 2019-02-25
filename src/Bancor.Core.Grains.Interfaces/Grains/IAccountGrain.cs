using System.Threading.Tasks;
using Orleans;

namespace Bancor.Core.Grains.Interfaces.Grains
{
    public interface IAccountGrain : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.CreateOrJoin)]
        Task Withdraw(decimal amount, string description);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task Deposit(decimal amount, string description, TransactionType type = TransactionType.Default);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<decimal> GetBalance();

        Task  HasNewName(string name);
        Task<string> GetName();
    }
}
