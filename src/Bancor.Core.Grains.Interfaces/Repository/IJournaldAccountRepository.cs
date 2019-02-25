using System.Collections.Generic;
using System.Threading.Tasks;
using Bancor.Core.Events.Account;
using Bancor.Core.States.Account;

namespace Bancor.Core.Grains.Interfaces.Repository
{
    public interface IJournaldAccountRepository
    {

        Task<KeyValuePair<int, JournaledAccountGrainState>> GetState(long accountId);

        Task<bool> ApplyUpdatesToStorage(long accountId, IReadOnlyList<AccountEvent> updates,
            int expectedversion);
    }
}