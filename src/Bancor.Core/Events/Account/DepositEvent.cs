namespace Bancor.Core.Events.Account
{
    public class DepositEvent : AccountEvent
    {
        public decimal Amount { get; set; }

        public DepositEvent(decimal amount)
        {
            Amount = amount;
        }
    }

    public class AccountNameEvent : AccountEvent
    {
        public string Name { get; set; }

        public AccountNameEvent(string name)
        {
            Name = name;
        }
    }
}