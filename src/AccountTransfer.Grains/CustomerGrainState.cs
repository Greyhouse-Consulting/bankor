using System;
using System.Collections.Generic;

namespace AccountTransfer.Grains
{
    public class CustomerGrainState
    {
        public string Name { get; set; }

        public IList<Guid> AccountIds { get; set; }
    }
}