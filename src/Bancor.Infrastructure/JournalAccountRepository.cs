using System;
using Bancor.Core.Events.Account;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Core.States.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bancor.Infrastructure
{
    public class JournalAccountRepository : IJournaldAccountRepository
    {
        public Task< KeyValuePair<int, JournaledAccountGrainState>> GetState(Guid accountId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ApplyUpdatesToStorage(Guid accountId, IReadOnlyList<AccountEvent> updates,
            int expectedversion)
        {
            throw new System.NotImplementedException();
        }
    }

    public class JournaledAccountGrainStateSnapshotTest  : JournaledAccountGrainStateSnapshot
    {
        public JournaledAccountGrainStateSnapshotTest()
        {
            Apply(new AccountNameEvent("Savings account"));
        }
    }
}