using EPDM.Interop.epdm;
using System;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services
{
    /// <summary>
    /// Settings Manager class
    /// </summary>
    public class SettingsManager : ISettingsManager

    {


        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="SettingsManager"/> is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        public bool Initialized { get; private set; }

        /// <summary>
        /// Gets the vault.
        /// </summary>
        /// <value>
        /// The vault.
        /// </value>
        public IEdmVault5 Vault { get; private set; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsManager"/> class.
        /// </summary>
        public SettingsManager()
        {

        }
        /// <summary>
        /// Initializes the specified vault.
        /// </summary>
        /// <param name="vault">The vault.</param>
        /// <param name="addInName">AddIn Name</param>
        /// <exception cref="System.NullReferenceException">
        /// name
        /// or
        /// vault
        /// </exception>
        public void Initialize(IEdmVault5 vault, string addInName)
        {

            if (string.IsNullOrWhiteSpace(addInName))
                throw new NullReferenceException(nameof(addInName));

            if (vault == null)
                throw new NullReferenceException(nameof(vault));


            this.AddInName = addInName;

            this.Vault = vault;

            Initialized = true;
        }

        /// <summary>
        /// Gets the name of the add in.
        /// </summary>
        /// <value>
        /// The name of the add in.
        /// </value>
        public string AddInName { get; private set; }



        #endregion

        #region Public Methods        
        /// <summary>
        /// Gets the setting object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSettings<T>()
        {
            return Vault.GetItem<T>(this.AddInName, typeof(T).Name);
        }


        /// <summary>
        /// Saves the setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings">The settings.</param>
        public void SaveSettings<T>(T settings)
        {
            Vault.SaveItem<T>(this.AddInName, typeof(T).Name, settings);
        }

        /// <summary>
        /// Saves a user setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings">The settings.</param>
        /// <param name="UserID">The user identifier.</param>
        /// <exception cref="System.Exception"></exception>
        public void SaveUserSettings<T>(T settings, int UserID)
        {
            if (UserID > 0)
                throw new Exception($"{nameof(UserID)} must be valid.");

            Vault.SaveItem<T>(this.AddInName, UserID.ToString(), settings);
        }


        /// <summary>
        /// Gets a user setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userID">The user identifier.</param>
        /// <returns></returns>
        public T GetUserSettings<T>(int userID)
        {
            return Vault.GetItem<T>(this.AddInName, userID.ToString());
        }





        #endregion
    }
}
