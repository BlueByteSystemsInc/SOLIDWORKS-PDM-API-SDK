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
        /// <!--/ <param name="CommandID">CommandID</param>  
        public HandlesAttribute(EdmCmdType eventName, int commandID = -1)
        {
          
            this.EventName = eventName;
            this.CommandID = commandID; 
        }

        

      
        /// <summary>
        /// Event name.
        /// </summary>
        public EdmCmdType EventName { get;  set; }


        /// <summary>
        /// command ID
        /// </summary>
        public int CommandID { get; set; }

        internal bool EvaluateCommandID(int commandID)
        {
            if (commandID == -1 || CommandID == -1)
                return true;

            return commandID == CommandID;
        }
    }
}
