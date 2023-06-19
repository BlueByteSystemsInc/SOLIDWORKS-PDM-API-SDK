using System.ComponentModel;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services.TypeConverters
{
    /// <summary>
    /// Used to enable a dropdown in a settings class. [TypeConverter(typeof(WorkflowNamesConverter))]
    /// </summary>
    /// <seealso cref="System.ComponentModel.StringConverter" />
    public class WorkflowNamesConverter : StringConverter
    {

        public static string[] WorkflowNames { get; set; }

        public WorkflowNamesConverter()
        {

        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(WorkflowNames);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

