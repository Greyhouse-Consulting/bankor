using System;

namespace BankOr.Core
{
 

    public sealed class StreamIdGenerator
    {
        private static readonly StreamIdGenerator instance = new StreamIdGenerator();

        public static Guid StreamId { get; }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static StreamIdGenerator()
        {
            StreamId = Guid.NewGuid();
        }

        private StreamIdGenerator()
        {
        }

        public static StreamIdGenerator Instance
        {
            get
            {
                return instance;
            }
        }
    }
}