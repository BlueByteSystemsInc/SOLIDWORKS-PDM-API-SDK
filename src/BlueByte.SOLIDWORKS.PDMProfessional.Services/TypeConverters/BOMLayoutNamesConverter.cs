using System.ComponentModel;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services.TypeConverters
{
    /// <summary>
    /// Used to enable a dropdown in a settings class. [TypeConverter(typeof(BOMLayoutNamesConverter))]
    /// </summary>
    /// <seealso cref="System.ComponentModel.StringConverter" />
    public class BOMLayoutNamesConverter : StringConverter
    {

        public static string[] BOMLayoutNames { get; set; }

        public BOMLayoutNamesConverter()
        {

        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(BOMLayoutNames);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

