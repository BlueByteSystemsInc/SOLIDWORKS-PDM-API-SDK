using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes
{
    public class NameAttribute : Attribute
    {
        public string Name { get; set; }
        public NameAttribute(string value)
        {
            this.Name = value;
        }
    }
}
