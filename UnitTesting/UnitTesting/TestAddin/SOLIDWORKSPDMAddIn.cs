using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Diagnostics;
using EPDM.Interop.epdm;
using SimpleInjector;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace TestAddin
{
    public enum Commands
    {
        CommandOne = 1000
    }

    [Menu((int)Commands.CommandOne, "This is a menu item - PDMFramework")]
    [Name("Sample Addin - PDMFramework")]
    [Description("This is a description - PDMFramework")]
    [CompanyName("Blue Byte Systems Inc")]
    [AddInVersion(false, 1)]
    [IsTask(false)]
    [RequiredVersion(10, 0)]
    [ComVisible(true)]
    [Guid("721C78F4-362E-45EC-B313-CD0E33A87D67")]
    public partial class PDMFrameworkAddInSample : AddInBase
    {

        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            base.OnCmd(ref poCmd, ref ppoData);

            #region connection
            var connectionString = "Data Source=173.225.102.156;Initial Catalog=BlueByteSystemsLogDb;User ID=sa;Password=2BtE2v!t;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            #endregion

            var logFolderPath = System.IO.Path.Combine(this.Vault.RootFolder.LocalPath, "Logs");

        
            var table = $"TestTable";

           
            Logger.Init(this.Identity, this.Instance, connectionString);


            Logger.StartConnection();


            Logger.LogToOutput(table, "Test");

            switch (poCmd.meCmdType)
            {
                case EdmCmdType.EdmCmd_TaskSetup:
                    break;
                case EdmCmdType.EdmCmd_TaskRun:
                    break;
                case EdmCmdType.EdmCmd_TaskLaunch:
                    break;
                
                default:
                    break;
            }
        }


        protected override void OnLoggerTypeChosen(LoggerType_e defaultType)
        {
            base.OnLoggerTypeChosen(LoggerType_e.SQL);
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