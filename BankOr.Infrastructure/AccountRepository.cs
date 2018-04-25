using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankOr.Core;
using NPoco;

namespace BankOr.Infrastructure
{
    public class AccountRepository 
    {
        public async Task<IList<Account>> Get(int id)
        {

            var inmemoryDatabase = new InMemoryDatabase();

            using (IDatabase db = new Database(inmemoryDatabase.Connection)) 
            {
                var users = db.Fetch<Account>("select id, name from Accounts");
            }

            return await Task.FromResult(new List<Account>());
        }
    }
}