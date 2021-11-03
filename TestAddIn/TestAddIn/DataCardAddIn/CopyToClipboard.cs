using EPDM.Interop.epdm;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DataCardAddIn
{
    [ComVisible(true)]
    [Guid("E21C78F4-362E-45EC-B313-CD0E33A87D67")]
    public class CopyToClipboard : IEdmAddIn5
    {
        private const string addinName = "Old DataCard AddIn";

        public void GetAddInInfo(ref EdmAddInInfo poInfo, IEdmVault5 poVault, IEdmCmdMgr5 poCmdMgr)
        {
            poInfo.mbsAddInName = addinName;
            poInfo.mbsCompany = "Blue Byte Systems, Inc.";
            poInfo.mlAddInVersion = 1;
            poInfo.mbsDescription = "This is a description.";
            poInfo.mlRequiredVersionMajor = 10;
            poInfo.mlRequiredVersionMinor = 0;

            poCmdMgr.AddCmd(100, "Copy paths to clipboard...",(int)EdmMenuFlags.EdmMenu_OnlyInContextMenu);

        }

        public void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {

            var vault = poCmd.mpoVault as IEdmVault21;

            if (poCmd.meCmdType == EdmCmdType.EdmCmd_Menu)
            {
                if (poCmd.mlCmdID == 100)
                {
                    var stringBuilder = new StringBuilder();

                    foreach (var data in ppoData)
                    {
                        var id = data.mlObjectID1;
                        try
                        {
                            var file = vault.GetObject(EdmObjectType.EdmObject_File, id) as IEdmFile10;
                            var localPath = file.GetLocalPath(file.GetNextFolder(file.GetFirstFolderPosition()).ID);

                            stringBuilder.AppendLine(localPath);

                        }
                        catch (Exception)
                        {

                        }
                        
                    }


                    
                    if (string.IsNullOrEmpty(stringBuilder.ToString()) == false)
                    {
                        Clipboard.SetText(stringBuilder.ToString());
                        vault.MsgBox(poCmd.mlParentWnd, "Copied to clipboard", EdmMBoxType.EdmMbt_OKOnly, "Message");
                    }
                }
            }
        }
    }
}
