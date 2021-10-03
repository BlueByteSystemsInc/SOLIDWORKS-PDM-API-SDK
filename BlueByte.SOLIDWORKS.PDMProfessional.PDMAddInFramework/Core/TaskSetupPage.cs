using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Diagnostics;
using EPDM.Interop.epdm;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Core
{
    /// <summary>
    /// Tasksetup page template.
    /// </summary>
    /// <typeparam name="T">ViewModel</typeparam>
    public class TaskSetupPage<T> : UserControl, ITaskSetupPage where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new instance of the task page setup.
        /// </summary>
        public TaskSetupPage() : base()
        {

        }


        /// <summary>
        /// Saves and loads data from variable
        /// </summary>
        public bool SaveLoadDataToVariable { get; set; }

        /// <summary>
        /// Gets the vault object.
        /// </summary>
        public IEdmVault5 Vault { get; private set; }

        /// <summary>
        /// Id of the variable to save and load data from.
        /// </summary>
        public int SaveLoadDataToVariableId { get; set; }

        /// <summary>
        /// Fires when data is loaded.
        /// </summary>
        public virtual void OnDataLoaded()
        {

        }
        /// <summary>
        /// Container
        /// </summary>
        public new SimpleInjector.Container Container { get; set; }

        /// <summary>
        /// ViewModel
        /// </summary>
        public T ViewModel { get; set; }
        /// <summary>
        /// Name of the setup page.
        /// </summary>
        public new string Name { get; set; }

        /// <summary>
        /// Load data.
        /// </summary>
        /// <param name="cmd">Cmd</param>
        public virtual void LoadData(ref EdmCmd cmd)//TODO change to reflect multiple pages
        {
            if (Container == null)
                throw new Exception("Please set the container property.");

            var taskProperties = cmd.mpoExtra as IEdmTaskProperties;

            this.Vault = cmd.mpoVault as IEdmVault5;

            object data = null;

            if (SaveLoadDataToVariable == false)
            {

                data = taskProperties.GetValEx($"{taskProperties.AddInName}{taskProperties.TaskName}{typeof(T).Name}");
            }

            else
            if (SaveLoadDataToVariable == true)
            {
                var vault = cmd.mpoVault as IEdmVault5;
                data = taskProperties.GetVar(SaveLoadDataToVariableId);
            }



            if (data != null)
            {
                var dataStr = data.ToString();


                var deserializedViewModel = default(T);

                if (string.IsNullOrWhiteSpace(dataStr))
                    deserializedViewModel = (T)Activator.CreateInstance(typeof(T));
                else
                    try
                    {
                        deserializedViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(dataStr);

                    }
                    catch (Exception e)
                    {
                        var logger = Container.GetInstance(typeof(ILogger)) as ILogger;
                        logger.LogToOutput(logger.OutputLocation, $"Exception occurred while serializing settings view model. {e.Message}");
                    }

                if (deserializedViewModel != null)
                {
                    this.ViewModel = deserializedViewModel;
                    OnDataLoaded();
                }
            }
        }
        /// <summary>
        /// Stores Data
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void StoreData(ref EdmCmd cmd)//TODO change to reflect multiple pages
        {
            if (cmd.mbsComment == "Cancel")
                return;

            var logger = Container.GetInstance(typeof(ILogger)) as ILogger;

            string str = string.Empty;
            try
            {
                str = Newtonsoft.Json.JsonConvert.SerializeObject(this.ViewModel);
            }
            catch (Exception e)
            {
                logger.LogToOutput(logger.OutputLocation, $"Exception occurred while serializing settings view model. {e.Message}");
            }



            if (str != null)
            {
                var taskProperties = cmd.mpoExtra as IEdmTaskProperties;
                var regex = new Regex(@"\\+");
                if (regex.IsMatch(str))
                {
                    str = regex.Replace(str, @"\\");
                }
                if (SaveLoadDataToVariable == false)
                    taskProperties.SetValEx($"{taskProperties.AddInName}{taskProperties.TaskName}{typeof(T).Name}", str);

                if (SaveLoadDataToVariable == true)
                {
                    var vault = cmd.mpoVault as IEdmVault5;
                    taskProperties.SetVar(SaveLoadDataToVariableId, str);
                }


            }
        }
    }
}
