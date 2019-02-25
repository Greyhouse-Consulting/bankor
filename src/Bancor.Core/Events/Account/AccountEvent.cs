using System;

namespace Bancor.Core.Events.Account
{
    public class AccountEvent
    {
        public AccountEvent()
        {
            Created = DateTime.Now;
        }
        public DateTime Created { get; }

    }

    public class WithdrawEvent : AccountEvent
    {
        public decimal Amount { get; }

        public WithdrawEvent(decimal amount)
        {
            Amount = amount;
        }
    }

    public class AccountEventLog
    {
        public long AccountId { get; }
        public AccountEvent AccountEvent { get; }
        public int AccountVersion { get; set; }


        public AccountEventLog(long accountId, AccountEvent accountEvent, int version)
        {
            AccountId = accountId;
            AccountEvent = accountEvent;
            AccountVersion = version;
        }
    }
}