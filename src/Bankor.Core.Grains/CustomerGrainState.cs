using System.Collections.Generic;
using Bancor.Core.Grains.Interfaces.Grains;

namespace Bankor.Core.Grains
{
    public class CustomerGrainState
    {
        public CustomerGrainState()
        {
            AccountGrains = new List<IAccountGrain>();
        }

        public string Name { get; set; }
        public bool Created { get; set; }

        public IList<IAccountGrain> AccountGrains { get; set; }
    }
}