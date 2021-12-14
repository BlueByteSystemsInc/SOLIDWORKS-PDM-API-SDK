using EPDM.Interop.epdm;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services
{

    public interface ISettingsManager
    {
        string AddInName { get; }
        bool Initialized { get; }
        IEdmVault5 Vault { get; }

        T GetSettings<T>();
        T GetUserSettings<T>(int userID);
        void Initialize(IEdmVault5 vault, string addInName);
        void SaveSettings<T>(T settings);
        void SaveUserSettings<T>(T settings, int UserID);
    }
}