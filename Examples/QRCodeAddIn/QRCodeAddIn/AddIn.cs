using BlueByte.SOLIDWORKS.PDMProfessional.Extensions;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Diagnostics;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Enums;
using EPDM.Interop.epdm;
using Net.Codecrete.QrCodeGenerator;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace QRCodeAddIn
{
    public enum Commands
    {
        CommandOne = 1000
    }

    [Menu((int)Commands.CommandOne, "Generate QRCode image...")]
    [Name("QRCode AddIn")]
    [Description("Creates QRCode images from variables")]
    [CompanyName("Blue Byte Systems Inc.")]
    [AddInVersion(false, 2)]
    [IsTask(false)]
    [RequiredVersion(Year_e.PDM2019, ServicePack_e.SP2)]
    [ComVisible(true)]
    [Guid("961244da-b9c5-4395-9037-75e2404b8f4f")]
    public partial class AddIn : AddInBase
    {

        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            base.OnCmd(ref poCmd, ref ppoData);

            var errorsList = new StringBuilder();

            // handle of the window where the add-in is executing
            var windowHandle = poCmd.mlParentWnd;

            base.ForEachFile(ref ppoData, (IEdmFile5 file) => {

                var parentFolderID = file.GetParentFolderID();
                var localPath = file.GetLocalPath(parentFolderID);
                var pngPath = System.IO.Path.ChangeExtension(localPath, ".png");

                try
                {
                    var pairs = new Dictionary<string, object>();

                    pairs.Add("Description", file.GetVariableValue("Description"));
                    pairs.Add("PartNumber", file.GetVariableValue("PartNumber"));
                    pairs.Add("Author", file.GetVariableValue("Author"));

                    var variableNames = pairs.Keys.ToArray();
                    var values = pairs.Values.ToArray();

                    var Str = variableNames.Select(x=> $"&{x}={values[Array.IndexOf(variableNames,x)]}");

                    var QRStr = $"{file.Name}{Str}";

                    IEdmFolder5 parentFolder;

                    var pngFile = Vault.TryGetFileFromPath(pngPath, out parentFolder);

                    if (pngFile != null)
                    {
                        var checkOutAction = pngFile.GetRequiredCheckOutAction();

                        switch (checkOutAction)
                        {
                            case CheckoutAction.DoNothingFileCheckedOutByMe:
                                break;
                            case CheckoutAction.FileCheckedInCanBeCheckedOut:
                                pngFile.LockFile(parentFolder.ID, windowHandle, (int)EdmLockFlag.EdmLock_Simple);
                                break;
                            case CheckoutAction.CheckedOutBySomeoneElse:

                                errorsList.AppendLine($"{pngFile.Name} - File is checked out by {pngFile.LockedByUser.Name}");
                                return;
                            default:
                                break;
                        }

                        GenerateQRCode(QRStr, pngPath);

                        pngFile.UnlockFile(parentFolder.ID, $"Checked in by {Identity.Name} V{Identity.Version}" ,windowHandle);
                    }
                    else
                    {
                        GenerateQRCode(QRStr, pngPath);

                        var folderPath = (new FileInfo(localPath)).Directory.FullName;
                        var folder = Vault.TryGetFolderFromPath(folderPath);

                        if (folder != null)
                        {
                            var ID = folder.AddFile(windowHandle, pngPath);
                            pngFile = Vault.GetObject(EdmObjectType.EdmObject_File, ID) as IEdmFile5;
                            if (pngFile != null)
                            {
                                pngFile.UnlockFile(folder.ID, $"Checked in by {Identity.Name} V{Identity.Version}", windowHandle);
                            }
                            else
                                errorsList.AppendLine($"{System.IO.Path.GetFileName(pngPath)} -Could add png to vault.");
                            

                        }
                    }
                    
                }
                catch (Exception e)
                {
                    errorsList.AppendLine($"{System.IO.Path.GetFileName(pngPath)}-{e.Message}");
                }
            
            
            });

            var errors = errorsList.ToString();

            if (string.IsNullOrWhiteSpace(errors.ToString()) == false)
                Vault.MsgBox(windowHandle, errors, EdmMBoxType.EdmMbt_OKOnly, $"{Identity.Name} V{Identity.Version}");
            else
                Vault.MsgBox(windowHandle, "Success!", EdmMBoxType.EdmMbt_OKOnly, $"{Identity.Name} V{Identity.Version}");

            poCmd.mlEdmRefreshFlags = (int)EdmRefreshFlag.EdmRefresh_FileList;
        }

        private void GenerateQRCode(string qRStr, string pngPath)
        {
            var qr = QrCode.EncodeText(qRStr, QrCode.Ecc.Medium);
            qr.SaveAsPng(pngPath, 10, 3);
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