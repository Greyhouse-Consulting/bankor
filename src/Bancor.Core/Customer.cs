using System.Collections.Generic;

namespace Bancor.Core
{
    public class Customer
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public IList<Account> Accounts { get; set; }
        public bool Created { get; set; }
    }
}