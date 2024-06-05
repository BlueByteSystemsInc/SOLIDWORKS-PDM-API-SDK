using BlueByte.SOLIDWORKS.PDMProfessional.UnitTesting;
using BlueByte.SOLIDWORKS.PDMProfessional.UnitTesting.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    class Program
    {
        [STAThreadAttribute]
        public static void Main()
        {
            string licenseKey = System.IO.File.ReadAllText(@"P:\Blue Byte Systems, Inc\PDMFrameworkUnitTesting\BlueByteSystemsInc_licensKey.XML");

            var addinTest = new AddInTestClass(licenseKey);
            addinTest.Run();

            Console.Read();
        }
    }

    [TestVault("BlueByte")]
    public class AddInTestClass : TestableAddInBase<TestAddin.TestAddIn>
    {
        public AddInTestClass(string licenseKey) : base(licenseKey)
        {

        }

        [PDMTestMethod]
        public void TestLogger()
        {
            EPDM.Interop.epdm.EdmCmdData[] ppData = null;
            EPDM.Interop.epdm.EdmCmd poCmd = MockEdmCmd.EdmCmd_Menu.Mock(5);
            this.AddInObject.OnCmd(ref poCmd, ref ppData);
        }
    }
}
