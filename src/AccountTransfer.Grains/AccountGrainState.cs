using System;
using Orleans.CodeGeneration;

[assembly: GenerateSerializer(typeof(AccountTransfer.Grains.AccountGrainState))]

namespace AccountTransfer.Grains
{

    [Serializable]
    public class AccountGrainState
    {
        public string Name { get; set; }

        public bool Created { get; set; }
    }
}