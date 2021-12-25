using BlueByte.SOLIDWORKS.PDMProfessional.Extensions;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Diagnostics;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Enums;
using EPDM.Interop.epdm;
using QRCoder;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AddIn
{
    public enum Commands
    {
        CommandOne = 1000
    }

    [Menu((int)Commands.CommandOne, "Generate QRCode Bitmap...")]
    [Name("QRCode")]
    [Description("Converts filenames and variables to QRCode")]
    [CompanyName("Blue Byte Systems Inc")]
    [AddInVersion(false, 4)]
    [IsTask(false)]
    [RequiredVersion(Year_e.PDM2020, ServicePack_e.SP0)]
    [ComVisible(true)]
    [Guid("44c8aefc-6c1f-4a78-a16d-3e0e891f8437")]
    public partial class AddIn : AddInBase
    {

        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            base.OnCmd(ref poCmd, ref ppoData);

            var errorsList = new StringBuilder();

            var windowHandle = poCmd.mlParentWnd;


            this.ForEachFile(ref ppoData, (IEdmFile5 file) =>
            {
                var parentFolderID = file.GetParentFolderID();
                var localPath = file.GetLocalPath(parentFolderID);
                var bmpPath = System.IO.Path.ChangeExtension(localPath, ".bmp");

                try
                {


                    if (file.IsPart() == false)
                    {
                        errorsList.Append($"{file.Name} is not a part document.");
                        return;
                    }


                    var pairs = new Dictionary<string, object>();

                    pairs.Add("Description", file.GetVariableValue("Description"));
                    pairs.Add("PartNumber", file.GetVariableValue("PartNumber"));
                    pairs.Add("Author", file.GetVariableValue("Author"));

                    var variableNames = pairs.Keys.ToArray();
                    var values = pairs.Keys.ToArray();

                    var str = variableNames.Select(x => $"&{x}={values[Array.IndexOf(variableNames, x)]}");

                    var QRStr = $"{file.Name}-{str}";

                    var QRbmp = GenerateQRBitmap(QRStr);


                  

                    IEdmFolder5 parentFolder;

                    var bmpFile = this.Vault.TryGetFileFromPath(bmpPath, out parentFolder);

                    if (bmpFile != null)
                    {
                        var checkoutAction = bmpFile.GetRequiredCheckOutAction();

                        switch (checkoutAction)
                        {
                            case CheckoutAction.DoNothingFileCheckedOutByMe:
                            case CheckoutAction.FileCheckedInCanBeCheckedOut:
                                bmpFile.LockFile(parentFolder.ID, windowHandle, (int)EdmLockFlag.EdmLock_Simple);
                                break;
                            case CheckoutAction.CheckedOutBySomeoneElse:
                                errorsList.AppendLine($"{bmpFile.Name} - Could not save bmp. {bmpFile.Name} is checked out by {bmpFile.LockedByUser.Name}");
                                return;
                            default:
                                break;
                        }


                        QRbmp.Save(bmpPath);
                        bmpFile.UnlockFile(windowHandle, $"Added by {Identity.Name} {Identity.Version}", (int)EdmUnlockFlag.EdmUnlock_ForceUnlock);

                    }
                    else
                    {
                        QRbmp.Save(bmpPath);
                        // assume bmp files are not added automatically
                        var folderPath = (new FileInfo(localPath)).Directory;
                        var folder = Vault.TryGetFolderFromPath(folderPath.FullName);
                        var bmpFileId = folder.AddFile(windowHandle, bmpPath);
                        bmpFile = Vault.GetObject(EdmObjectType.EdmObject_File, bmpFileId) as IEdmFile5;
                        if (bmpFile != null)
                            bmpFile.UnlockFile(windowHandle, $"Added by {Identity.Name} {Identity.Version}", (int)EdmUnlockFlag.EdmUnlock_ForceUnlock);
                        else
                            errorsList.AppendLine($"{System.IO.Path.GetFileName(bmpPath)} - Failed to add bmp to vault. ID {bmpFileId}");

                    }




                }
                catch (Exception e)
                {
                    errorsList.AppendLine($"{System.IO.Path.GetFileName(bmpPath)}{e.Message}");
                }


            });

            if (string.IsNullOrWhiteSpace(errorsList.ToString()) == false)
                Vault.MsgBox(windowHandle, errorsList.ToString(), EdmMBoxType.EdmMbt_OKOnly, $"Error(s) - {Identity.Name} V{Identity.Version}");
            else
                Vault.MsgBox(windowHandle, "Success!", EdmMBoxType.EdmMbt_OKOnly, $"Information - {Identity.Name} V{Identity.Version}");


            poCmd.mlEdmRefreshFlags = (int)EdmRefreshFlag.EdmRefresh_FileList;

        }

        private static Bitmap GenerateQRBitmap(string QRCodeStr)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(QRCodeStr, QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return qrCodeImage;
        }

        protected override void OnLoggerTypeChosen(LoggerType_e defaultType)
        {
            base.OnLoggerTypeChosen(LoggerType_e.File);
        }

        protected override void OnRegisterAdditionalTypes(Container container)
        {
   
        }

        protected override void OnLoggerOutputSat(string defaultDirectory)
        {
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