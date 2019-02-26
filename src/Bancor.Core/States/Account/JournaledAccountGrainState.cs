using System;
using System.Collections.Generic;
using Bancor.Core.Events.Account;

namespace Bancor.Core.States.Account
{
    public class JournaledAccountGrainState
    {
        private readonly IList<Transaction> _transactions;
        public decimal Balance { get; private set; }

        public string Name { get; private set; }

        public bool Created { get; private set; }

        public  IReadOnlyList<Transaction> Transactions => (IReadOnlyList<Transaction>) _transactions;

        public JournaledAccountGrainState()
        {
            _transactions = new List<Transaction>();
        }

        public JournaledAccountGrainState Apply(DepositEvent @event)
        {
            Balance += @event.Amount;
            
            _transactions.Add(new Transaction
            {
                Amount = @event.Amount,
                BookingDate = DateTime.Now
            });
            
            return this;
        }

        public JournaledAccountGrainState Apply(AccountNameEvent @event)
        {
            Name += @event.Name;

            Created = true;
            return this;
        }


        public JournaledAccountGrainState Apply(WithdrawEvent @event)
        {
            Balance -= @event.Amount;
            
            _transactions.Add(new Transaction
            {
                Amount = @event.Amount,
                BookingDate = DateTime.Now
            });

            return this;
        }

        public JournaledAccountGrainState Apply(AccountEvent @event)
        {
            if (@event is DepositEvent depositEvent)
                return Apply(depositEvent);

            if (@event is WithdrawEvent withdrawEvent)
                return Apply(withdrawEvent);

            if (@event is AccountNameEvent accountNameEvent)
                return Apply(accountNameEvent);

            throw new Exception("Unhandled account event");
        }
    }

    public class JournaledAccountGrainStateSnapshot : JournaledAccountGrainState
    {
        public int AccountId { get; set; }
        public int LatestVersion { get; set; }

    }
}