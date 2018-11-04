using System;
using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces.Grains
{
    public interface IBankTransferGrain : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        Task Transfer(long fromAccount, long toAccount, decimal amountToTransfer);
    }
}
