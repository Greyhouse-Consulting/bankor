using System;
using Bancor.Core.Grains;
using Orleans.CodeGeneration;

[assembly: GenerateSerializer(typeof(AccountGrainState))]

namespace Bancor.Core.Grains
{

    [Serializable]
    public class AccountGrainState
    {
        public string Name { get; set; }

        public bool Created { get; set; }
    }
}