using Newtonsoft.Json;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services
{
    internal static class Extensions
    {
        public static  T Deserialize<T>(this string value, JsonSerializerSettings settings = null)
        {

            return JsonConvert.DeserializeObject<T>(value, settings);
        }

        public static string Serialize<T>(this T value, JsonSerializerSettings settings = null)
        {
            if (value == null)
                return string.Empty;



            return JsonConvert.SerializeObject(value, settings);

 
        }
    }
}
