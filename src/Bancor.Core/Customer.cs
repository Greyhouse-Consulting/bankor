using System;
using System.Collections.Generic;

namespace Bancor.Core
{
    public class Customer
    {
        public Customer()
        {
            AccountsIds = new List<Guid>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public IList<Guid> AccountsIds { get; set; }
    }
}