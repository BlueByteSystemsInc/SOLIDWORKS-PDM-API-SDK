using EPDM.Interop.epdm;
using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes
{
    /// <summary>
    /// Command visiblity attribute.
    /// </summary>
    /// <remarks><see cref="AddInBase"/> will ignore multiple attributes with the same menu id and will only pick one.</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CommandVisibilityAttribute : Attribute
    {

        /// <summary>
        /// ID of the affected command.
        /// </summary>
        public int CommandID { get; set; }
        /// <summary>
        /// Sets or gets the permissions of the users to whom to show the command.
        /// </summary>
        public EdmSysPerm OnlyShowToUsersWithThesePermissions { get; set; }

        /// <summary>
        /// Creates new instance of this class.
        /// </summary>
        public CommandVisibilityAttribute()
        {

        }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="commandId">Command ID.</param>
        public CommandVisibilityAttribute(int commandId)
        {
            CommandID = commandId;  
        }

        /// <summary>
        /// Hide from these user or group names.
        /// </summary>
        public string[] HideFromTheseUserOrGroupNames { get; set; }

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="hideFromTheseUserOrGroupNames"></param>
        public CommandVisibilityAttribute(int commandId, string[] hideFromTheseUserOrGroupNames)
        {
            CommandID = commandId;
            HideFromTheseUserOrGroupNames = hideFromTheseUserOrGroupNames;
        }
         
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="commandId">Command id.</param>
        /// <param name="onlyShowToUsersWithThesePermissions">Only show this command to users who have these permissions.</param>
        public CommandVisibilityAttribute(int commandId, EdmSysPerm onlyShowToUsersWithThesePermissions)
        {
            CommandID = commandId;
            OnlyShowToUsersWithThesePermissions = onlyShowToUsersWithThesePermissions;
        }



         



    }
}
