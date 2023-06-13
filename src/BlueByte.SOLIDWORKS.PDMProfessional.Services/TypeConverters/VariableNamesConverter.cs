using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services.TypeConverters
{
    /// <summary>
    /// Used to enable a dropdown in a settings class. [TypeConverter(typeof(VariableNamesConverter))]
    /// </summary>
    /// <seealso cref="System.ComponentModel.StringConverter" />
    public class VariableNamesConverter : StringConverter
    {

        public static string[] VariableNames { get; set; }

        public VariableNamesConverter()
        {

        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(VariableNames);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

