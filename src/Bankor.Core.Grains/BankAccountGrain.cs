using System.Threading.Tasks;
using Bancor.Core.Grains.Interfaces.Grains;
using Orleans;

namespace Bancor.Core.Grains
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