using EPDM.Interop.epdm;

namespace BlueByte.SOLIDWORKS.PDMProfessional.SDK.Core
{
    /// <summary>
    /// task page page.
    /// </summary>
    public interface ITaskPage
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the container. This is used in <see cref="LoadData(ref EdmCmd)"/> and <see cref="StoreData(ref EdmCmd)"/>
        /// </summary>
        SimpleInjector.Container Container { get; set; }

        /// <summary>
        /// Name of the setup page.
        /// </summary>
        string Name { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads data from the task.
        /// </summary>
        /// <param name="cmd">EdmCmd specific to the <see cref="EdmCmdType.EdmCmd_TaskSetup"/> or <see cref="EdmCmdType.EdmCmd_TaskDetails"/>hook.</param>
        /// <param name="identity">Use because IEdmTaskInstance guid returns empty guid.</param>
        void LoadData(ref EdmCmd cmd);

        /// <summary>
        /// Loads task settings from a file
        /// </summary>
        void LoadSettings();

        /// <summary>
        /// Saves task settings to a file
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Stores data back into the task.
        /// <param name="cmd">EdmCmd specific to the <see cref="EdmCmdType.EdmCmd_TaskSetup"/> hook.</param>
        void StoreData(ref EdmCmd cmd);

        #endregion
    }
}