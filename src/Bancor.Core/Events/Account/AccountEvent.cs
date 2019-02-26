using System;

namespace Bancor.Core.Events.Account
{
    public class AccountEvent
    {
        public AccountEvent()
        {
            Created = DateTime.Now;
        }
        public DateTime Created { get; set;}

    }

    public class WithdrawEvent : AccountEvent
    {
        public decimal Amount { get; set; }

        public WithdrawEvent(decimal amount)
        {
            Amount = amount;
        }
    }
}