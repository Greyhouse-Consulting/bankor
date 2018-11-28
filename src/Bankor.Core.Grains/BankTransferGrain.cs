using System.Threading.Tasks;
using Bancor.Core.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Concurrency;

namespace Bancor.Core.Grains
{
    [StatelessWorker]
    public class BankTransferGrain : Grain, IBankTransferGrain
    {
        Task IBankTransferGrain.Transfer(long fromAccount, long toAccount, decimal amountToTransfer)
        {
            return Task.WhenAll(
                GrainFactory.GetGrain<IAccountGrain>(fromAccount).Withdraw(amountToTransfer),
                GrainFactory.GetGrain<IAccountGrain>(toAccount).Deposit(amountToTransfer));
        }
    }
}
