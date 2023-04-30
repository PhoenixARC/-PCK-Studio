using System;
using System.Runtime.Serialization;

namespace PckStudio.IO.TGA
{
    [Serializable]
    internal class TGAException : Exception
    {
        public TGAException()
        {
        }

        public TGAException(string message) : base(message)
        {
        }

        public TGAException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TGAException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}