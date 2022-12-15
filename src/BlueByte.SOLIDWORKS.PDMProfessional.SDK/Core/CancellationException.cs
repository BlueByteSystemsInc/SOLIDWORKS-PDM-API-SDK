using System;
using System.Runtime.Serialization;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK
{


    [Serializable]
    public class CancellationException : Exception
    {
        public CancellationException()
        {
        }

        public CancellationException(string message) : base(message)
        {
        }

        public CancellationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CancellationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}