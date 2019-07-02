namespace Bancor.Core.Events.Account
{
    public class DepositEvent : AccountEvent
    {
        public decimal Amount { get; protected set; }

        public DepositEvent(decimal amount, string description) : base(description)
        {
            Amount = amount;
        }
    }

    public class AccountNameEvent : AccountEvent
    {
        public string Name { get; set; }

        public AccountNameEvent(string name, string description) : base(description)
        {
            Name = name;
        }
    }
}