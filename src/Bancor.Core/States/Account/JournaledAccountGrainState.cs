using System;
using System.Collections.Generic;
using Bancor.Core.Events.Account;

namespace Bancor.Core.States.Account
{
    public class JournaledAccountGrainState
    {
        private readonly IList<Transaction> _transactions;
        private readonly IList<Stakeholder> _stakeholderIds;
        public decimal Balance { get; private set; }

        public string Name { get; private set; }

        public bool Created { get; private set; }

        public  IReadOnlyList<Transaction> Transactions => (IReadOnlyList<Transaction>) _transactions;

        public JournaledAccountGrainState()
        {
            _transactions = new List<Transaction>();
            _stakeholderIds = new List<Stakeholder>();
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

        public JournaledAccountGrainState Apply(NewStakeholderEvent @event)
        {
            _stakeholderIds.Add(new Stakeholder{
                StakeholderId = @event.StakeholderId,
                TypeOfStakeholder = @event.TypeOfStakeholder
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

            if (@event is NewTransactionEvent newTransactionEvent)
                return Apply(newTransactionEvent);

            throw new Exception("Unhandled account event");
        }

        public JournaledAccountGrainState Apply(NewTransactionEvent @event)
        {
            Balance -= @event.Amount;
            
            _transactions.Add(new Transaction
            {
                Amount = @event.Amount,
                BookingDate = @event.BookingDate
            });

            return this;
        }
    }

    public class JournaledAccountGrainStateSnapshot : JournaledAccountGrainState
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public int LatestVersion { get; set; }

    }
}