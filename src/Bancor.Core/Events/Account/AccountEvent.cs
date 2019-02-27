using System;

namespace Bancor.Core.Events.Account
{
    public class AccountEvent
    {
        public Guid Id { get; set; }
        public AccountEvent()
        {
            Created = DateTime.Now;
            Id = Guid.NewGuid();
        }
        public DateTime Created { get; set;}

    }
}