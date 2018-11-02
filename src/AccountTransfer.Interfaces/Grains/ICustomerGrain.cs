using System.Collections.Generic;
using System.Threading.Tasks;
using BankOr.Core.Models;
using Orleans;

namespace AccountTransfer.Interfaces.Grains
{
    public interface ICustomerGrain : IGrainWithIntegerKey
    {
        Task HasNewName(string name);
        Task<IList<AccountModel>> GetAccounts();

        Task CreateAccount(string name);
        Task TryInit(string name);
    }
}