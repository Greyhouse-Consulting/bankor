using System;
using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using Orleans;
using Orleans.Concurrency;

namespace AccountTransfer.Grains
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
