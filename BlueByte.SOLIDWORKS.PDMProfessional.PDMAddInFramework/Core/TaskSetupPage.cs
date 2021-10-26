using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Diagnostics;
using EPDM.Interop.epdm;
using Microsoft.WindowsAPICodePack.Dialogs;
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
        #region Public Properties

        /// <summary>
        /// Container
        /// </summary>
        public new SimpleInjector.Container Container { get; set; }

        /// <summary>
        /// Name of the setup page.
        /// </summary>
        public new string Name { get; set; }

        /// <summary>
        /// Saves and loads data from variable
        /// </summary>
        public bool SaveLoadDataToVariable { get; set; }

        /// <summary>
        /// Id of the variable to save and load data from.
        /// </summary>
        public int SaveLoadDataToVariableId { get; set; }

        /// <summary>
        /// Gets the vault object.
        /// </summary>
        public IEdmVault5 Vault { get; private set; }

        /// <summary>
        /// ViewModel
        /// </summary>
        public T ViewModel { get; private set; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Creates a new instance of the task page setup.
        /// </summary>
        public TaskSetupPage() : base()
        {
        }

        #endregion

        #region Public Methods

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
        /// Loads task settings from a file
        /// </summary>
        public void LoadSettings()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "Browse to task settings";
            dialog.Filters.Add(new CommonFileDialogFilter("PDM Task Settings", ".edmtdf"));
            dialog.DefaultDirectory = Vault.RootFolderPath;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var fileName = dialog.FileName;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return;
                }

                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                var serializedViewModel = System.IO.File.ReadAllText(fileName);
                try
                {
                    ViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(serializedViewModel, settings);
                    OnDataLoaded();
                }
                catch (Exception ex)
                {
                    Vault.MsgBox(Handle.ToInt32(), $"Failed to load task definition. {ex.Message} - {Environment.NewLine} {ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Fires when data is loaded. Clear all bindings from all controls by invoking <see cref="ClearDataBindings"/>
        /// </summary>
        public virtual void OnDataLoaded()
        {
            ClearDataBindings(this);
        }

        /// <summary>
        /// Clears data bindings all of children.
        /// </summary>
        /// <param name="rootControl">The root control.</param>
        public void ClearDataBindings(Control rootControl)
        {
            foreach (Control control in rootControl.Controls)
            {
                control.DataBindings.Clear();
                if (control.Controls != null)
                {
                    ClearDataBindings(control);
                }
            }
        }

        /// <summary>
        /// Saves task settings to a file
        /// </summary>
        public void SaveSettings()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save task settings";
            dialog.Filter = "PDM Task Settings (*.edmtdf)|*.edmtdf|All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileName = dialog.FileName;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return;
                }

                var serializedViewModel = string.Empty;
                try
                {
                    serializedViewModel = Newtonsoft.Json.JsonConvert.SerializeObject(ViewModel);
                    System.IO.File.WriteAllText(fileName, serializedViewModel);
                }
                catch (Exception ex)
                {
                    Vault.MsgBox(Handle.ToInt32(), $"Failed to save task definition. {ex.Message} - {Environment.NewLine} {ex.StackTrace}");
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

        #endregion
    }
}