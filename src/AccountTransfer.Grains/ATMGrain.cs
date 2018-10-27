using System;
using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using Orleans;
using Orleans.Concurrency;

namespace AccountTransfer.Grains
{
    [StatelessWorker]
    public class ATMGrain : Grain, IATMGrain
    {
        Task IATMGrain.Transfer(Guid fromAccount, Guid toAccount, uint amountToTransfer)
        {
            return Task.CompletedTask;
            //return Task.WhenAll(
            //    this.GrainFactory.GetGrain<IAccountGrain>(fromAccount).Withdraw(amountToTransfer),
            //    this.GrainFactory.GetGrain<IAccountGrain>(toAccount).Deposit(amountToTransfer));
        }
    }
}
