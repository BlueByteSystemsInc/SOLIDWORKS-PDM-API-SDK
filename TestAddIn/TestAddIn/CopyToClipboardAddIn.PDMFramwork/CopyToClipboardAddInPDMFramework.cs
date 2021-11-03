using System;
using System.Runtime.InteropServices;
using System.Text;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes;
using EPDM.Interop.epdm;

namespace CopyToClipboardAddIn.PDMFramwork
{
    public enum Menus
    {
        CopyToClipboard = 1000
    }

    [Menu((int)Menus.CopyToClipboard,"Copy to clipboard - PDMFramework")]
    [Name("Copy to clipboard - PDMFramework")]
    [Description("This is a description - PDMFramework")]
    [CompanyName("Blue Byte Systems Inc")]
    [AddInVersion(false,1)]
    [IsTask(false)]
    [RequiredVersion(10, 0)]
    [ComVisible(true)]
    [Guid("721C78F4-362E-45EC-B313-CD0E33A87D67")]
    public class CopyToClipboardAddInPDMFramework : AddInBase
    {

        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            base.OnCmd(ref poCmd, ref ppoData);


            if (poCmd.mlCmdID != (int)Menus.CopyToClipboard)
                return;

            var stringBuilder = new StringBuilder();

            ForEachFile(ref ppoData, (IEdmFile5 x) => {

                var path = x.GetLocalPath(x.GetNextFolder(x.GetFirstFolderPosition()).ID);

                stringBuilder.AppendLine(path);


            
            });


            if (string.IsNullOrEmpty(stringBuilder.ToString()) == false)
            {
                System.Windows.Forms.Clipboard.SetText(stringBuilder.ToString());
                Vault.MsgBox(poCmd.mlParentWnd, "Copied to clipboard", EdmMBoxType.EdmMbt_OKOnly, "Message - PDMFramework");
            }
        }
    }
}
