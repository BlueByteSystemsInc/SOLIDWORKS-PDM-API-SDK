using System.ComponentModel;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services.TypeConverters
{
    /// <summary>
    /// Used to enable a dropdown in a settings class. [TypeConverter(typeof(TransitionNamesConverter))]
    /// </summary>
    /// <seealso cref="System.ComponentModel.StringConverter" />
    public class TransitionNamesConverter : StringConverter
    {

        public static string[] TransitionNames { get; set; }

        public TransitionNamesConverter()
        {

        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(TransitionNames);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

