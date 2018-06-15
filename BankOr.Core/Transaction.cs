using System;

namespace BankOr.Core
{
    public class Transaction
    {
        public int Id { get; set; }
                
        public DateTime BookingDate { get; set; }

        public decimal Amount { get; set; }
        public Account Account { get; set; }
    }
}