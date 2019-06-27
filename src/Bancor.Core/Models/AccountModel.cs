using System;

namespace Bancor.Core.Models
{
    public class AccountModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}