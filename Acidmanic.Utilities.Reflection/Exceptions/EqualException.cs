using System;
using System.Runtime.Serialization;

namespace Acidmanic.Utilities.Reflection.Exceptions
{
    public class EqualException:Exception
    {

        public EqualException()
        {
        }

        protected EqualException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public EqualException(string message) : base(message)
        {
        }

        public EqualException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public EqualException(string expected, string actual)
        {
            
        }
    }
}