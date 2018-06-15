using System;
using System.Collections;
using System.Collections.Generic;

namespace BankOr.Core
{
    public class Account
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public IList<Transaction> Transactions { get; set; }
        public IList<Customer> Customers { get; set; }
    }
}
