using System;
using System.Runtime.InteropServices;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Exceptions
{
    /// <summary>
    /// Fires in <see cref="AddInBase.GetAddInInfo(ref EPDM.Interop.epdm.EdmAddInInfo, EPDM.Interop.epdm.IEdmVault5, EPDM.Interop.epdm.IEdmCmdMgr5)"/> if attribute is missing.
    /// </summary>
    public class IdentityInfoException : swPDMAddInFrameworkException
    {
        /// <summary>
        /// Creates a new instance of the base exception.
        /// </summary>
        public IdentityInfoException()
        {
        }

        /// <summary>
        /// Creates a new instance of the base exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public IdentityInfoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
