using EPDM.Interop.epdm;
using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services
{
    internal static class SettingsManagerHelper
    {
        public static string GetUserCredentialsKey(this IEdmUser5 user)
        {
            return $"Credentials {user.ID}";
        }

        public static void SaveItem<T>(this IEdmVault5 vault, string name, string key, T item)
        {
            IEdmDictionary5 dictionary = vault.GetDictionary(name, true);

            var serialized = Extensions.Serialize(item);
            dictionary.StringSetAt(key, serialized);
        }

        public static T GetItem<T>(this IEdmVault5 vault, string Name, string key = null)
        {
            IEdmDictionary5 dictionary = vault.GetDictionary(Name, true);

            if (dictionary.StringGetAt(key, out string value))
            {
                return Extensions.Deserialize<T>(value);
            }
            else
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
        }

        public static T GetSettings<T>(this IEdmVault5 vault)
        {
                return vault.GetItem<T>(typeof(T).Name);
        }
    }
}
