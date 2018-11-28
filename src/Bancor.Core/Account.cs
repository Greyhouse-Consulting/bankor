using System.Collections.Generic;

namespace Bancor.Core
{
    public class Account
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public IList<Transaction> Transactions { get; set; }
        public IList<Customer> Customers { get; set; }
    }
}
