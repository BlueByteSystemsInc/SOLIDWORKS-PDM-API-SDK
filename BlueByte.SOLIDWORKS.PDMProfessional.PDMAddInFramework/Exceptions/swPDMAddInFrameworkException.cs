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
    public class swPDMAddInFrameworkException : Exception
    { /// <summary>
      /// Creates a new instance of the base exception.
      /// </summary>
        public swPDMAddInFrameworkException()
        {
        }

        /// <summary>
        /// Creates a new instance of the base exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public swPDMAddInFrameworkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
