using EPDM.Interop.epdm;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Core
{
    /// <summary>
    /// Task setup page.
    /// </summary>
    public interface ITaskSetupPage
    {

        /// <summary>
        /// Gets or sets the container. This is used in <see cref="LoadData(ref EdmCmd)"/> and <see cref="StoreData(ref EdmCmd)"/>
        /// </summary>
        SimpleInjector.Container Container { get; set; }

        /// <summary>
        /// Name of the setup page.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Loads data from the task.
        /// </summary>
        /// <param name="cmd">EdmCmd specific to the <see cref="EdmCmdType.EdmCmd_TaskSetup"/> hook.</param>
        /// <param name="identity">Use because IEdmTaskInstance guid returns empty guid.</param>
        void LoadData(ref EdmCmd cmd);
        /// <summary>
        /// Stores data back into the task.
        /// <param name="cmd">EdmCmd specific to the <see cref="EdmCmdType.EdmCmd_TaskSetup"/> hook.</param>
      
        void StoreData(ref EdmCmd cmd);
    }
}
