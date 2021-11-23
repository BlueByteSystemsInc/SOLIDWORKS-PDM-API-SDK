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
        public static void Main()
        {
            string licenseKey = System.IO.File.ReadAllText(@"P:\PDMFrameworkUnitTesting\BlueByteSystemsInc_licensKey.XML");

            var addinTest = new AddInTestClass(licenseKey);
            addinTest.Run();

            Console.Read();
        }
    }

    [TestVault("BlueByte")]
    public class AddInTestClass : TestableAddInBase<TestAddin.PDMFrameworkAddInSample>
    {
        public AddInTestClass(string licenseKey) : base(licenseKey)
        {

        }

        [PDMTestMethod]
        public void TestTaskSetup()
        {
            Console.WriteLine("Testing task launch");
        }
    }
}
