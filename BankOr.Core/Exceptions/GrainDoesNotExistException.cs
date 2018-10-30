using System;
using System.Runtime.Serialization;

namespace BankOr.Core.Exceptions
{

    [Serializable]
    public class GrainDoesNotExistException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public GrainDoesNotExistException()
        {
        }

        public GrainDoesNotExistException(string message) : base(message)
        {
        }

        public GrainDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }

        protected GrainDoesNotExistException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}