namespace Bancor.Core.Events.Account
{
    public class AccountEvent
    {

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