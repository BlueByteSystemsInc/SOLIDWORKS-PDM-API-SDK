﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using BlueByte.SOLIDWORKS.PDMProfessional.SDK;
using BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes;
using BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics;
using BlueByte.SOLIDWORKS.PDMProfessional.SDK.Enums;
using EPDM.Interop.epdm;
using SimpleInjector;

namespace SOLIDWORKSPDMAddIn
{

    public enum Commands
    {
        CommandOne = 1234
    }
    
    [Menu((int)Commands.CommandOne, "Click Me!", 43)]
    [ListenFor(EdmCmdType.EdmCmd_Menu)]
    [Name("Sample Addin")]
    [Description("This is a description")]
    [CompanyName("Blue Byte Systems Inc")]
    [AddInVersion(false, 1)]
    [IsTask(false)]
    [RequiredVersion(Year_e.PDM2014, ServicePack_e.SP0)]
    [ComVisible(true)]
    [Guid("721C78F4-362E-45EC-B313-CD0E33A87D67")]
    public partial class AddIn : AddInBase
    {

        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            base.OnCmd(ref poCmd, ref ppoData);

            int handle = poCmd.mlParentWnd;

            try
            {
                ForEachFile(ref ppoData, (IEdmFile5 file) => {

                    base.Vault.MsgBox(handle, $"You clicked on {file.Name}", EdmMBoxType.EdmMbt_OKOnly, Identity.ToCaption());

                });
            }
            catch (CancellationException e)
            {

                throw;
            }
            catch (TaskFailedException e)
            {

                throw;
            }
            // this is a PDM exception
            catch (COMException e)
            {

                throw;
            }
            catch (Exception e)
            {

                throw;
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

            logAction = (Exception e) => {

                throw new NotImplementedException();
            };


            base.OnUnhandledExceptions(catchAllExceptions, logAction);
        }
    }
}