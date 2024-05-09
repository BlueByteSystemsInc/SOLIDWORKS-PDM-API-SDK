using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Exceptions
{
    /// <summary>
    /// Fires when the supplied <see cref="Core.ITaskPage"/> is not a <see cref="System.Windows.Forms.UserControl"/>.
    /// </summary>
    public class ITaskSetupPageNotUserControlException : PDMSDKException
    {

        /// <summary>
        /// Creates a new instance of the base exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ITaskSetupPageNotUserControlException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
