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
}