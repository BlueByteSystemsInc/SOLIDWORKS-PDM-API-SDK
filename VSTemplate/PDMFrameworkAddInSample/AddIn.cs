/*
MIT License

Copyright (c) 2022 Blue Byte Systems Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
BUT NOT LIMITED TO >THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
INNO EVENT SHALL >THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF >CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
OR THE USE OR OTHER >DEALINGS IN THE SOFTWARE.*/

using System;
using System.IO;
using System.Runtime.InteropServices;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Diagnostics;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Enums;
using EPDM.Interop.epdm;
using SimpleInjector;

namespace SOLIDWORKSPDMAddIn
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
    [RequiredVersion(Year_e.PDM2018, ServicePack_e.SP0)]
    [ComVisible(true)]
    [Guid("721C78F4-362E-45EC-B313-CD0E33A87D67")]
    public partial class AddIn : AddInBase
    {

        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            base.OnCmd(ref poCmd, ref ppoData);

            // todo: add implementation  
            throw new NotImplementedException("This addin has no implementation");
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