using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bancor.Core.Events.Account;
using Bancor.Core.States.Account;

namespace Bancor.Core.Grains.Interfaces.Repository
{
    public interface IJournaldAccountRepository
    {

        Task<KeyValuePair<int, JournaledAccountGrainState>> GetState(Guid accountId);

        Task<bool> ApplyUpdatesToStorage(Guid accountId, IReadOnlyList<AccountEvent> updates,
            int expectedversion);
    }
}