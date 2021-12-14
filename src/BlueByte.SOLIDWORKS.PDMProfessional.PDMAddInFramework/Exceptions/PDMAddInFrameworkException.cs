using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Exceptions
{
    /// <summary>
    /// Base exception.
    /// </summary>
    public class PDMAddInFrameworkException : Exception
    { /// <summary>
      /// Creates a new instance of the base exception.
      /// </summary>
        public PDMAddInFrameworkException()
        {
        }

        /// <summary>
        /// Creates a new instance of the base exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public PDMAddInFrameworkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
