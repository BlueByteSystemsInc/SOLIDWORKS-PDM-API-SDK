using System.ComponentModel;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services.TypeConverters
{
    /// <summary>
    /// Used to enable a dropdown in a settings class. [TypeConverter(typeof(StateNamesConverter))]
    /// </summary>
    /// <seealso cref="System.ComponentModel.StringConverter" />
    public class StateNamesConverter : StringConverter
    {
        public static string[] StateNames { get; set; }

        public StateNamesConverter()
        {

        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(StateNames);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

