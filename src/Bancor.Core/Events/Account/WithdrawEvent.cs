namespace Bancor.Core.Events.Account
{
    public class WithdrawEvent : AccountEvent
    {
        public decimal Amount { get; set; }
        public WithdrawEvent(decimal amount, string description) : base(description)
        {
            Amount = amount;
        }
    }
}