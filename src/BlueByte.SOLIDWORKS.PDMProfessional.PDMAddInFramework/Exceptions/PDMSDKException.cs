using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Exceptions
{
    /// <summary>
    /// Base exception.
    /// </summary>
    public class PDMSDKException : Exception
    { /// <summary>
      /// Creates a new instance of the base exception.
      /// </summary>
        public PDMSDKException()
        {
        }

        /// <summary>
        /// Creates a new instance of the base exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public PDMSDKException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
