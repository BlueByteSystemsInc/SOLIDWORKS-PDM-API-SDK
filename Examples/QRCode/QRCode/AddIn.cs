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
using System.Drawing;
using System.Drawing.Imaging;
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
    [AddInVersion(false, 1)]
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
                var bmpPath = System.IO.Path.ChangeExtension(localPath, ".png");

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


                        GenerateQRBitmap(QRStr, bmpPath);

                        bmpFile.UnlockFile(windowHandle, $"Added by {Identity.Name} {Identity.Version}", (int)EdmUnlockFlag.EdmUnlock_ForceUnlock);

                    }
                    else
                    {
                        GenerateQRBitmap(QRStr, bmpPath);

                        // assume bmp files are not added automatically
                        var folderPath = (new FileInfo(localPath)).Directory;
                        var folder = Vault.TryGetFolderFromPath(folderPath.FullName);
                        var bmpFileId = folder.AddFile(windowHandle, bmpPath, "", (int)EdmAddFlag.EdmAdd_Simple);
                        bmpFile = Vault.GetObject(EdmObjectType.EdmObject_File, bmpFileId) as IEdmFile5;
                        if (bmpFile != null)
                            bmpFile.UnlockFile(windowHandle, $"Added by {Identity.Name} {Identity.Version}", (int)EdmUnlockFlag.EdmUnlock_Simple);
                        else
                            errorsList.AppendLine($"{System.IO.Path.GetFileName(bmpPath)} - Failed to add bmp to vault. ID {bmpFileId}");

                    }




                }
                catch (Exception e)
                {
                    errorsList.AppendLine($"{System.IO.Path.GetFileName(bmpPath)} - {e.Message}");
                }


            });

            if (string.IsNullOrWhiteSpace(errorsList.ToString()) == false)
                Vault.MsgBox(windowHandle, errorsList.ToString(), EdmMBoxType.EdmMbt_OKOnly, $"Error(s) - {Identity.Name} V{Identity.Version}");
            else
                Vault.MsgBox(windowHandle, "Success!", EdmMBoxType.EdmMbt_OKOnly, $"Information - {Identity.Name} V{Identity.Version}");


            poCmd.mlEdmRefreshFlags = (int)EdmRefreshFlag.EdmRefresh_FileList;

        }

        private void GenerateQRBitmap(string QRCodeStr, string targetPath)
        {
            var qr = QrCode.EncodeText(QRCodeStr, QrCode.Ecc.Medium);
            qr.SaveAsPng(targetPath, 10, 3);
             
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


    /// <summary>
    /// <c>QrCode</c> extension for creating bitmaps using <c>System.Drawing</c> classes.
    /// <para>
    /// In .NET 6 and later versions, this extension will only work on Windows.
    /// </para>
    /// </summary>
    public static class QrCodeBitmapExtensions
    {
        /// <inheritdoc cref="ToBitmap(QrCode, int, int)"/>
        /// <param name="background">The background color.</param>
        /// <param name="foreground">The foreground color.</param>
        public static Bitmap ToBitmap(this QrCode qrCode, int scale, int border, Color foreground, Color background)
        {
            // check arguments
            if (scale <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "Value out of range");
            }
            if (border < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(border), "Value out of range");
            }

            int size = qrCode.Size;
            int dim = (size + border * 2) * scale;

            if (dim > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "Scale or border too large");
            }

            // create bitmap
            Bitmap bitmap = new Bitmap(dim, dim, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Draw(qrCode, g, scale, border, foreground, background);
            }

            return bitmap;
        }

        /// <summary>
        /// Creates a bitmap (raster image) of this QR code.
        /// <para>
        /// The <paramref name="scale"/> parameter specifies the scale of the image, which is
        /// equivalent to the width and height of each QR code module. Additionally, the number
        /// of modules to add as a border to all four sides can be specified.
        /// </para>
        /// <para>
        /// For example, <c>ToBitmap(scale: 10, border: 4)</c> means to pad the QR code with 4 white
        /// border modules on all four sides, and use 10&#xD7;10 pixels to represent each module.
        /// </para>
        /// <para>
        /// The resulting bitmap uses the pixel format <see cref="PixelFormat.Format24bppRgb"/>.
        /// If not specified, the foreground color is black (0x000000) und the background color always white (0xFFFFFF).
        /// </para>
        /// </summary>
        /// <param name="scale">The width and height, in pixels, of each module.</param>
        /// <param name="border">The number of border modules to add to each of the four sides.</param>
        /// <returns>The created bitmap representing this QR code.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="scale"/> is 0 or negative, <paramref name="border"/> is negative
        /// or the resulting image is wider than 32,768 pixels.</exception>
        public static Bitmap ToBitmap(this QrCode qrCode, int scale, int border)
        {
            return qrCode.ToBitmap(scale, border, Color.Black, Color.White);
        }

        /// <summary>
        /// Draws this QR code into the specified graphics context.
        /// <para>
        /// The QR code is drawn at offset (0, 0). Use <see cref="Graphics.TranslateTransform(float, float)"/>
        /// to draw it at a different position.
        /// </para>
        /// <para>
        /// The <paramref name="scale"/> parameter specifies the scale of the image, which is
        /// equivalent to the width and height of each QR code module. Additionally, the number
        /// of modules to add as a border to all four sides can be specified.
        /// </para>
        /// <para>
        /// For example, <c>Draw(graphics, scale: 10, border: 4)</c> means to pad the QR code with 4 white
        /// border modules on all four sides, and use 10&#xD7;10 pixels to represent each module.
        /// </para>
        /// <para>
        /// </summary>
        /// <param name="graphics">The graphics context to draw in.</param>
        /// <param name="scale">The width and height, in pixels, of each module.</param>
        /// <param name="border">The number of border modules to add to each of the four sides.</param>
        public static void Draw(this QrCode qrCode, Graphics graphics, float scale, float border)
        {
            Draw(qrCode, graphics, scale, border, Color.Black, Color.White);
        }

        /// <inheritdoc cref="Draw(QrCode, Graphics, float, float)"/>
        /// <param name="background">The background color.</param>
        /// <param name="foreground">The foreground color.</param>
        public static void Draw(this QrCode qrCode, Graphics graphics, float scale, float border, Color foreground, Color background)
        {
            if (scale <= 0 || border < 0)
            {
                return;
            }

            int size = qrCode.Size;
            float dim = (size + border * 2) * scale;

            // draw background
            if (background != null)
            {
                using(SolidBrush brush = new SolidBrush(background))
                { 
                graphics.FillRectangle(brush, 0, 0, dim, dim);
                }
            }

            // draw modules
            using (SolidBrush brush = new SolidBrush(foreground))
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        if (qrCode.GetModule(x, y))
                        {
                            graphics.FillRectangle(brush, (x + border) * scale, (y + border) * scale, scale, scale);
                        }
                    }
                }
            }
        }

        /// <inheritdoc cref="ToPng(QrCode, int, int)"/>
        /// <param name="background">The background color.</param>
        /// <param name="foreground">The foreground color.</param>
        public static byte[] ToPng(this QrCode qrCode, int scale, int border, Color foreground, Color background)
        {
            using (Bitmap bitmap = qrCode.ToBitmap(scale, border, foreground, background))
            {

                using ( MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();

                }
            }
        }

        /// <summary>
        /// Creates a PNG image of this QR code and returns it as a byte array.
        /// <para>
        /// The <paramref name="scale"/> parameter specifies the scale of the image, which is
        /// equivalent to the width and height of each QR code module. Additionally, the number
        /// of modules to add as a border to all four sides can be specified.
        /// </para>
        /// <para>
        /// For example, <c>ToPng(scale: 10, border: 4)</c> means to pad the QR code with 4 white
        /// border modules on all four sides, and use 10&#xD7;10 pixels to represent each module.
        /// </para>
        /// <para>
        /// If not specified, the foreground color is black (0x000000) und the background color always white (0xFFFFFF).
        /// </para>
        /// </summary>
        /// <param name="scale">The width and height, in pixels, of each module.</param>
        /// <param name="border">The number of border modules to add to each of the four sides.</param>
        /// <returns>The created bitmap representing this QR code.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="scale"/> is 0 or negative, <paramref name="border"/> is negative
        /// or the resulting image is wider than 32,768 pixels.</exception>
        public static byte[] ToPng(this QrCode qrCode, int scale, int border)
        {
            return qrCode.ToPng(scale, border, Color.Black, Color.White);
        }

        /// <inheritdoc cref="SaveAsPng(QrCode, string, int, int)"/>
        /// <param name="background">The background color.</param>
        /// <param name="foreground">The foreground color.</param>
        public static void SaveAsPng(this QrCode qrCode, string filename, int scale, int border, Color foreground, Color background)
        {
            using (Bitmap bitmap = qrCode.ToBitmap(scale, border, foreground, background))
            {
                bitmap.Save(filename, ImageFormat.Png);

            }
        }

        /// <summary>
        /// Saves this QR code as a PNG file.
        /// <para>
        /// The <paramref name="scale"/> parameter specifies the scale of the image, which is
        /// equivalent to the width and height of each QR code module. Additionally, the number
        /// of modules to add as a border to all four sides can be specified.
        /// </para>
        /// <para>
        /// For example, <c>SaveAsPng("qrcode.png", scale: 10, border: 4)</c> means to pad the QR code with 4 white
        /// border modules on all four sides, and use 10&#xD7;10 pixels to represent each module.
        /// </para>
        /// <para>
        /// If not specified, the foreground color is black (0x000000) und the background color always white (0xFFFFFF).
        /// </para>
        /// </summary>
        /// <param name="scale">The width and height, in pixels, of each module.</param>
        /// <param name="border">The number of border modules to add to each of the four sides.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="scale"/> is 0 or negative, <paramref name="border"/> is negative
        /// or the resulting image is wider than 32,768 pixels.</exception>
        public static void SaveAsPng(this QrCode qrCode, string filename, int scale, int border)
        {
            qrCode.SaveAsPng(filename, scale, border, Color.Black, Color.White);
        }
    }
}