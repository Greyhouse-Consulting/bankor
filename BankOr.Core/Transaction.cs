using System;

namespace BankOr.Core
{
    public class Transaction
    {
        public int Id { get; set; }
        
        public int AccountId { get; set; }
        
        public DateTime BookingDate { get; set; }

        public decimal Amount { get; set; }
    }
}