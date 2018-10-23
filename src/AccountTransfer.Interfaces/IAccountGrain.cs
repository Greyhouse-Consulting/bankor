using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces
{
    public interface IAccountGrain : IGrainWithGuidKey
    {
        [Transaction(TransactionOption.Create)]
        Task Withdraw(uint amount);

        [Transaction(TransactionOption.Create)]
        Task Deposit(uint amount);

        Task<decimal> GetBalance();

        Task Owner(string userId);
    }
}
