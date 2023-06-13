using EPDM.Interop.epdm;
using Newtonsoft.Json;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services
{

    public interface ISettingsManager
    {
        string AddInName { get; }
        bool Initialized { get; }
        IEdmVault5 Vault { get; }

        T GetSettings<T>(JsonSerializerSettings settings = null);
        T GetUserSettings<T>(int userID, JsonSerializerSettings settings = null);
        void Initialize(IEdmVault5 vault, string addInName);
        void SaveSettings<T>(T settings, JsonSerializerSettings opt = null);
        void SaveUserSettings<T>(T settings, int UserID, JsonSerializerSettings opt = null);
    }
}