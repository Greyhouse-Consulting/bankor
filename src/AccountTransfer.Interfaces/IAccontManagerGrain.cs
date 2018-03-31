using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankOr.Core;
using Orleans;

namespace AccountTransfer.Interfaces
{
    public interface IAccontManagerGrain : IGrainWithIntegerKey
    {
        Task<IBankAccountGrain> Create(string userId);
        Task<IList<BankAccountModel>> GetAccounts();
    }
}