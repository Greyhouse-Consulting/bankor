using System.Collections.Generic;
using System.Threading.Tasks;
using Bancor.Core;

namespace Bancor.Infrastructure.Repository
{
    public class AccountRepository 
    {


        public async Task<IList<Account>> Get(int id)
        {


            return await Task.FromResult(new List<Account>());
        }
    }
}