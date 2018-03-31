using System.Threading.Tasks;
using AccountTransfer.Interfaces;
using Orleans;

namespace AccountTransfer.Grains
{
    public class BankAccountGrain : Grain, IBankAccountGrain
    {
        private decimal _balance;
        private string _ownerId;

        public BankAccountGrain()
        {
            _balance = 0;
        }

        public Task Deposit(decimal amount)
        {
            _balance += amount;

            return Task.CompletedTask;
        }

        public Task Owner(string userId)
        {
            _ownerId = userId;
            return Task.CompletedTask;
        }

        public Task<string> GetOwner()
        {
            return Task.FromResult(_ownerId);
        }
    }
}