using System.Collections.Generic;
using System.Threading.Tasks;
using BankOr.Core;
using Orleans;

namespace AccountTransfer.Interfaces
{
    public interface ICustomerGrain : IGrainWithIntegerKey
    {
        Task HasNewName(string name);
        Task<IList<string>> GetAccounts();

        Task CreateAccount(string name);
    }
}