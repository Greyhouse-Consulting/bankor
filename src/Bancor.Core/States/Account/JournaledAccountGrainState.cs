using System;
using Bancor.Core.Events.Account;

namespace Bancor.Core.States.Account
{
    public class JournaledAccountGrainState
    {
        public decimal Balance { get; private set; }

        public JournaledAccountGrainState Apply(DepositEvent @event)
        {
            Balance += @event.Amount;
            return this;
        }

        public JournaledAccountGrainState Apply(AccountEvent @event)
        {
            if (@event is DepositEvent depositEvent)
                return Apply(depositEvent);

            throw new Exception("Unhandled account event");
        }
    }

    public class JournaledAccountGrainStateSnapshot : JournaledAccountGrainState
    {
        public int AccountId { get; set; }
        public int LatestVersion { get; set; }

    }
}