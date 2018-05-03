using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankOr.Core;
using Orleans;

namespace AccountTransfer.Interfaces
{
    public interface IAccountManagerGrain : IGrainWithIntegerKey
    {
        Task<IAccountGrain> Create(string userId);
        Task<IList<Account>> GetAccounts();
    }
}