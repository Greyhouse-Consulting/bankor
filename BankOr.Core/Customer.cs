using System.Collections;
using System.Collections.Generic;

namespace BankOr.Core
{
    public class Customer
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public IList<Account> Accounts { get; set; }
    }
}