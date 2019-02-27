using System;
using Bancor.Core.Events.Account;

namespace Bancor.Infrastructure.Containers
{
    public class DepositEventContainer : AccountEventContainer
    {
        public decimal Amount { get; set; }
    }

    public class AccountEventContainer
    {
        public Guid Id { get; set; }
    }
}