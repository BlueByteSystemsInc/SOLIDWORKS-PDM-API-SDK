using EPDM.Interop.epdm;
using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes
{
    /// <summary>
    /// Add command menu attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class HandlesAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the HandlesAttribute
        /// </summary>
        /// </param>
        /// <param name="eventName">Invoke the callback method when this event occurs in PDM</param>
        public HandlesAttribute(EdmCmdType eventName)
        {
          
            this.EventName = eventName;
        }

        

      
        /// <summary>
        /// Event name.
        /// </summary>
        public EdmCmdType EventName { get;  set; }


        
    }
}
