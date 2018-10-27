using System.Collections.Generic;
using System.Threading.Tasks;
using BankOr.Core;

namespace BankOr.Infrastructure.Repository
{
    public class AccountRepository 
    {


        public async Task<IList<Account>> Get(int id)
        {


            return await Task.FromResult(new List<Account>());
        }
    }
}