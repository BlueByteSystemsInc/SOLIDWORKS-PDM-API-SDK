using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes
{
    public class CompanyNameAttribute : Attribute
    {
        public string CompanyName { get; set; }
        public CompanyNameAttribute(string value)
        {
            this.CompanyName = value;
        }
    }
}
