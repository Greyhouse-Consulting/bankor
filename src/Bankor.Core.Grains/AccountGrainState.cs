using System;
using Bankor.Core.Grains;
using Orleans.CodeGeneration;

[assembly: GenerateSerializer(typeof(AccountGrainState))]

namespace Bankor.Core.Grains
{

    [Serializable]
    public class AccountGrainState
    {
        public string Name { get; set; }

        public bool Created { get; set; }
    }
}