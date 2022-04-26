using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes
{
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public DescriptionAttribute(string value)
        {
            this.Description = value;
        }
    }
}
