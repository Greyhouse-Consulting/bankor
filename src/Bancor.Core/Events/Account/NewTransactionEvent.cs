using System;

namespace Bancor.Core.Events.Account
{
    public class NewTransactionEvent : AccountEvent
    {
        public decimal Amount { get; }
        public DateTime BookingDate { get; }

        public NewTransactionEvent(decimal amount, DateTime bookingDate)
        {
            Amount = amount;
            BookingDate = bookingDate;
        }
    }
}