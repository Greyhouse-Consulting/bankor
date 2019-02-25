using System;

namespace Bancor.Core
{
    public class Transaction
    {
        public Guid Id { get; set; }
                
        public DateTime BookingDate { get; set; }

        public decimal Amount { get; set; }
        public long AccountId { get; set; }
        public string Description { get; set; }

        public TransactionType Type { get; set; }
    }

    public enum TransactionType
    {
        Default,
        Interest        
    }
}