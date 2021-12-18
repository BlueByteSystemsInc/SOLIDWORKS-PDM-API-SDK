using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Enums;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Diagnostics;
using EPDM.Interop.epdm;
using SimpleInjector;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CopyToClipboardAddInPDMFramework
{
    public enum Commands
    {
        CopyToClipboardCommand = 1000
    }

    [Menu((int)Commands.CopyToClipboardCommand, "Copy to clipboard using PDMFramework")]
    [Name("CopyToClipboard - PDMFramework")]
    [Description("This is a description - PDMFramework")]
    [CompanyName("Blue Byte Systems Inc")]
    [AddInVersion(false, 1)]
    [IsTask(false)]
    [RequiredVersion(Year_e.PDM2006, ServicePack_e.SP0)]
    [ComVisible(true)]
    [Guid("B6241720-A37B-4D9C-AF4D-7F6F12EA5D2B")]
    public partial class CopyToClipboardAddInPDMFramework : AddInBase
    {

        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            base.OnCmd(ref poCmd, ref ppoData);

            var logger = base.Logger;

            logger.Init(this.Identity, Instance, string.Empty);

            logger.OutputLocation = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            logger.LogToOutput("Log.txt", "Logged this value.");

            var stringBuilder = new StringBuilder();

            ForEachFile(ref ppoData, (IEdmFile5 file) => {

                var localPath = file.GetLocalPath(file.GetNextFolder(file.GetFirstFolderPosition()).ID);

                stringBuilder.AppendLine(localPath);

            });


            if (string.IsNullOrEmpty(stringBuilder.ToString()) == false)
            {
                Clipboard.SetText(stringBuilder.ToString());
                Vault.MsgBox(poCmd.mlParentWnd, "Copied to clipboard!", EdmMBoxType.EdmMbt_OKOnly, $"{Identity.Name} V{Identity.Version}");
            }
        }


        protected override void OnLoggerTypeChosen(LoggerType_e defaultType)
        {
            base.OnLoggerTypeChosen(LoggerType_e.File);
        }

        protected override void OnRegisterAdditionalTypes(Container container)
        {
            // register types with the container 
        }

        protected override void OnLoggerOutputSat(string defaultDirectory)
        {
            // set the logger default directory - ONLY USE IF YOU ARE NOT LOGGING TO PDM
        }
        protected override void OnLoadAdditionalAssemblies(DirectoryInfo addinDirectory)
        {
            base.OnLoadAdditionalAssemblies(addinDirectory);
        }

        protected override void OnUnhandledExceptions(bool catchAllExceptions, Action<Exception> logAction = null)
        {
            this.CatchAllUnhandledException = false;

            logAction = (Exception e) =>
            {

                throw new NotImplementedException();
            };


            base.OnUnhandledExceptions(catchAllExceptions, logAction);
        }
    }
}