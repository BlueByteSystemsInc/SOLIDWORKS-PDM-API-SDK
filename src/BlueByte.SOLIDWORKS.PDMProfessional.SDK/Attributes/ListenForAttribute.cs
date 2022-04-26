using EPDM.Interop.epdm;
using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    /// <summary>
    /// Listen for PDM events attribute.
    /// </summary>
    public class ListenForAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the ListenFor attribute
        /// </summary>
        /// <param name="_event"></param>
        public ListenForAttribute(EdmCmdType _event)
        {
            Event = _event;
        }

        /// <summary>
        /// PDM event to listen to
        /// </summary>
        public EdmCmdType Event { get; }
    }
}
