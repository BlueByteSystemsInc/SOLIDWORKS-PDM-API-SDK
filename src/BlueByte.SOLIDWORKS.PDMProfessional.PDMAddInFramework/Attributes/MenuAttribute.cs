using EPDM.Interop.epdm;
using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes
{
    /// <summary>
    /// Add command menu attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MenuAttribute : Attribute
    {
     /// <summary>
     /// Creates a command menu.
     /// </summary>
     /// <param name="iD">ID of the command.</param>
     /// <param name="menuCaption">Text that will appear in the menu.</param>
     /// <param name="flags">Where the menu will appear.</param>
     /// <param name="statusBarHelp">Help message that will appear in the status bar.</param>
     /// <param name="toolTip">Help message that appear in the Windows tooltip.</param>
     /// <param name="toolButtonIndex">Index of the command button.</param>
     /// <param name="toolbarImageID">ID of the toolbar image.</param>
        public MenuAttribute(int iD, string menuCaption, EdmMenuFlags flags = 0, string statusBarHelp = "", string toolTip = "", int toolButtonIndex = -1, int toolbarImageID = 0)
        {
            ID = iD;
            MenuCaption = menuCaption;
            Flags = flags;
            StatusBarHelp = statusBarHelp;
            Tooltip = toolTip;
            ToolButtonIndex = toolButtonIndex;
           
            ToolbarImageID = toolbarImageID;
        }
 


         

        /// <summary>
        /// ID of the command.
        /// </summary>
        public int ID { get; }
        /// <summary>
        /// Text that will appear in the menu.
        /// </summary>
        public string MenuCaption { get; }

        /// <summary>
        /// Where the menu will appear.
        /// </summary>
        public EdmMenuFlags Flags { get; }
        /// <summary>
        /// Index of the command button.
        /// </summary>
        public int ToolButtonIndex { get; }
        /// <summary>
        /// ID of the toolbar image.
        /// </summary>
        public int ToolbarImageID { get; }
        /// <summary>
        /// Help message that will appear in the status bar.
        /// </summary>
        public string StatusBarHelp { get; }
        /// <summary>
        /// Help message that appear in the Windows tooltip
        /// </summary>
        public string Tooltip { get; }
    }
}
