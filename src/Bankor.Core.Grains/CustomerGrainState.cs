using System.Collections.Generic;
using Bancor.Core.Grains.Interfaces;
using Bancor.Core.Grains.Interfaces.Grains;

namespace Bancor.Core.Grains
{
    public class CustomerGrainState
    {
        public CustomerGrainState()
        {
            AccountGrains = new List<IJournaledAccountGrain>();
        }

        public string Name { get; set; }
        public bool Created { get; set; }

        public IList<IJournaledAccountGrain> AccountGrains { get; set; }
    }
}