using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Enums
{
    /// <summary>
    /// Advanced version of the <see cref="EdmMenuFlags"/>
    /// </summary>
    public enum PDMSDKEdmMenuFlags
    {
        /// <summary>
        /// The PDMSDK edm menu only files in file explorer
        /// </summary>
        PDMSDK_EdmMenu_OnlyFilesInFileExplorer = 43,
        /// <summary>
        /// The edm menu nothing
        /// </summary>
        EdmMenu_Nothing = 0,
        /// <summary>
        /// The edm menu must have selection
        /// </summary>
        EdmMenu_MustHaveSelection = 1,
        /// <summary>
        /// The edm menu only files
        /// </summary>
        EdmMenu_OnlyFiles = 2,
        /// <summary>
        /// The edm menu only folders
        /// </summary>
        EdmMenu_OnlyFolders = 4,
        /// <summary>
        /// The edm menu only single selection
        /// </summary>
        EdmMenu_OnlySingleSelection = 8,
        /// <summary>
        /// The edm menu only multiple selection
        /// </summary>
        EdmMenu_OnlyMultipleSelection = 16,
        /// <summary>
        /// The edm menu only in context menu
        /// </summary>
        EdmMenu_OnlyInContextMenu = 32,
        /// <summary>
        /// The edm menu never in context menu
        /// </summary>
        EdmMenu_NeverInContextMenu = 64,
        /// <summary>
        /// The edm menu has toolbar button
        /// </summary>
        EdmMenu_HasToolbarButton = 128,
        /// <summary>
        /// The edm menu owner draw toolbar button
        /// </summary>
        EdmMenu_OwnerDrawToolbarButton = 256,
        /// <summary>
        /// The edm menu administration
        /// </summary>
        EdmMenu_Administration = 512,
        /// <summary>
        /// The edm menu context menu item
        /// </summary>
        EdmMenu_ContextMenuItem = 1024,
        /// <summary>
        /// The edm menu context menu item folder
        /// </summary>
        EdmMenu_ContextMenuItemFolder = 2048,
        /// <summary>
        /// The edm menu item tools menu
        /// </summary>
        EdmMenu_ItemToolsMenu = 4096,
        /// <summary>
        /// The edm menu hast item toolbar button
        /// </summary>
        EdmMenu_HastItemToolbarButton = 8192,
        /// <summary>
        /// The edm menu show in menu bar action
        /// </summary>
        EdmMenu_ShowInMenuBarAction = 16384,
        /// <summary>
        /// The edm menu show in menu bar modify
        /// </summary>
        EdmMenu_ShowInMenuBarModify = 32768,
        /// <summary>
        /// The edm menu show in menu bar display
        /// </summary>
        EdmMenu_ShowInMenuBarDisplay = 65536,
        /// <summary>
        /// The edm menu show in menu bar tools
        /// </summary>
        EdmMenu_ShowInMenuBarTools = 131072
    }
}
