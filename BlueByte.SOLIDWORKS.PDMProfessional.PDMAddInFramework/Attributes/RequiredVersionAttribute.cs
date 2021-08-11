using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes
{
    public class RequiredVersionAttribute : Attribute
    {
         
        public int Major { get; set; }
        public int Minor { get; set; }
        public RequiredVersionAttribute(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }
    }
}
