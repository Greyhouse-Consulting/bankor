using System;
using System.Collections.Generic;
using AccountTransfer.Interfaces.Grains;

namespace AccountTransfer.Grains
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