namespace Bancor.Core.Events.Account
{
    public class DepositEvent : AccountEvent
    {
        public decimal Amount { get; }

        public DepositEvent(decimal amount)
        {
            Amount = amount;
        }
    }
}