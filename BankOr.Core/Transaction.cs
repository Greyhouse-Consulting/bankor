using System;

namespace BankOr.Core
{
    public class Transaction
    {
        public Guid Id { get; set; }
                
        public DateTime BookingDate { get; set; }

        public decimal Amount { get; set; }
        public long AccountId { get; set; }
    }
}