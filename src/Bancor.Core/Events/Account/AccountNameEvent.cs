namespace Bancor.Core.Events.Account
{
    public class AccountNameEvent : AccountEvent
    {
        public string Name { get; set; }

        public AccountNameEvent(string name, string description) : base(description)
        {
            Name = name;
        }
    }
}