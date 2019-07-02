using System;

namespace Bancor.Core.Events.Account
{
    public class NewTransactionEvent : AccountEvent
    {
        public decimal Amount { get; set; }
        public DateTime BookingDate { get; set; }

        public NewTransactionEvent(decimal amount, DateTime bookingDate, string description) : base(description)
        {
            Amount = amount;
            BookingDate = bookingDate;
        }
    }
}