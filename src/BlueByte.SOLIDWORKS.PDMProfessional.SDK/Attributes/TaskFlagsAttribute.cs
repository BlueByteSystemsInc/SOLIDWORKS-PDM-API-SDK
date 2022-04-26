using EPDM.Interop.epdm;
using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes
{
    /// <summary>
    /// Task flags enum.
    /// </summary>
    public class TaskFlagsAttribute : Attribute
    {
        
        /// <summary>
        /// Task flag enum.
        /// </summary>
        /// <param name="flags">Sum of all flags</param>
        public TaskFlagsAttribute(int flags)
        {
            Flags = flags;
        }

        public int Flags { get; }
    }
}
