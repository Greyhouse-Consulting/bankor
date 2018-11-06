using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces.Grains
{
    public interface IAccoutCreatedObserverGrain : IGrainWithIntegerKey
    {
        Task StartSubscribe();
    }
}
