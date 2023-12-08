/// <summary>
/// 	''' Custom, editable checked listbox control
/// 	''' </summary>
/// 	''' <remarks>This demo loads a comma-delimited string collection of Urls and boolean values to set the checked state for each item.</remarks>
using System;
using System.ComponentModel;
using System.Globalization;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services.TypeConverters
{
    public class StringArrayTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var propertyValue = context.PropertyDescriptor.GetValue(context.Instance);

            if (propertyValue != null)
            {
                return string.Join(", ", propertyValue as string[]);
            }
            return "No selection.";
        }
    }
}
