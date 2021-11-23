using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Attributes;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Core;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Diagnostics;
using BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.Exceptions;
using EPDM.Interop.epdm;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

namespace BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework
{

    /// <summary>
    /// Taskbase class.
    /// </summary>
    public abstract class AddInBase : IEdmAddIn5
    {
        /// <summary>
        /// Task instance.
        /// </summary>
        public IEdmTaskInstance Instance { get; set; }

        /// <summary>
        /// Properties
        /// </summary>
        public IEdmTaskProperties Properties { get; set; }

        /// <summary>
        /// Action to execute before task crashes.
        /// </summary>
        public Action<Exception> BeforeYouCrash { get; set; }

        /// <summary>
        /// Stores identity of the task add-in.
        /// </summary>
        public Identity Identity;

        /// <summary>
        /// Catch all unhandled exceptions and log them.
        /// </summary>
        public bool CatchAllUnhandledException { get; set; } = false;

        /// <summary>
        /// Sets the type of the logger to be used.
        /// </summary>
        public LoggerType_e LoggerType { get; set; }

        private EdmCmd poCmd;

        /// <summary>
        /// Gets whether or not task is initialized (ie container and logger created).
        /// </summary>
        public  bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// DI Container.
        /// </summary>
        public  Container Container { get; private set; }

        /// <summary>
        /// Returns the container object.
        /// </summary>
        /// <returns></returns>
        public Container GetContainer()
        {
            return Container;
        }
        /// <summary>
        /// Fires when the application is initialized. Register types of calling assembly.
        /// </summary>
        private void RegisterPDMAddInFrameworkTypes()
        {

            if (Container == null)
            {
                Container = new Container();
                var assembly = Assembly.GetCallingAssembly();

                var registrations =
                    from type in assembly.GetExportedTypes()
                    where type.Namespace.Contains("Pages.")
                    select new { implementation = type };

                foreach (var reg in registrations)
                {
                    Container.Register(reg.implementation, reg.implementation, Lifestyle.Transient);
                }


                switch (LoggerType)
                {
                    case LoggerType_e.Console:
                        Container.RegisterSingleton<ILogger, ConsoleLogger>();
                        break;

                    case LoggerType_e.File:
                        Container.RegisterSingleton<ILogger, FileLogger>();
                        break;
                    case LoggerType_e.SQL:
                        Container.RegisterSingleton<ILogger, SQLLogger>();
                        break;
                        throw new swPDMAddInFrameworkException("Logger type was not specified.", null);
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Create a new instance of the addIn base class.
        /// </summary>
        public AddInBase()
        {
            try
            {
                var thisAssembly = new FileInfo(typeof(AddInBase).Assembly.Location);

                OnLoadAdditionalAssemblies(thisAssembly.Directory);
                OnLoggerTypeChosen(LoggerType_e.File);
                Initialize();
                OnUnhandledExceptions(false, null);
                if (IsInitialized == false)
                OnRegisterAdditionalTypes(Container);

                if (CatchAllUnhandledException)
                {
                    AppDomain.CurrentDomain.UnhandledException -= OnUnhandledExceptionThrown;
                    AppDomain.CurrentDomain.UnhandledException += OnUnhandledExceptionThrown;
                }

                Logger = Container.GetInstance<ILogger>();
                OnLoggerOutputSat(string.Empty);
                IsInitialized = true;
            }
            catch (Exception e)
            {

                MessageBox.Show($"PID: {System.Diagnostics.Process.GetCurrentProcess().Id}. Something went wrong when initializing the add-in. {e.Message}", $"{this.Identity.Name} {this.Identity.Version}");
            }
        }

        /// <summary>
        /// Allows for connection string to be sat to the SQL logger via <see cref="ILogger.StartConnection(string)"/>.
        /// </summary>
        protected virtual void OnLoggerStartConnectionWithSQLServer(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString) == false)
                Logger.SetConnectionString(connectionString);
        }

        /// <summary>
        /// Load assemblies that failed loading.
        /// </summary>
        /// <param name="addinDirectory">Directory of the add-in.</param>
        protected virtual void OnLoadAdditionalAssemblies(DirectoryInfo addinDirectory)
        {
            
        }

        /// <summary>
        /// Sets the output folder of the logger.
        /// </summary>
        /// <param name="defaultDirectory"></param>
        protected virtual void OnLoggerOutputSat(string defaultDirectory)
        { 
            if (Container != null) { 
            if (LoggerType == LoggerType_e.File)
                    if (Logger != null)
                Logger.OutputLocation = defaultDirectory;
            }
        }
        /// <summary>
        /// Sets the type of the logger.
        /// </summary>
        /// <param name="defaultType"></param>
        protected virtual void OnLoggerTypeChosen(LoggerType_e defaultType)
        {
            LoggerType = defaultType;
        }

        /// <summary>
        /// Sets how to handle unhandled exceptions
        /// </summary>
        /// <param name="catchAllExceptions"></param>
        /// <param name="logAction"></param>
        protected virtual void OnUnhandledExceptions(bool catchAllExceptions, Action<Exception> logAction = null)
        {
            CatchAllUnhandledException = catchAllExceptions;
            BeforeYouCrash = logAction;
        }

        /// <summary>
        /// Registers additional types.
        /// </summary>
        /// <param name="container"></param>
        protected virtual void OnRegisterAdditionalTypes(Container container)
        {

        }


        /// <summary>
        /// Creates a instance of the specified page type using the container.
        /// </summary>
        /// <typeparam name="T">Page type</typeparam>
        /// <param name="Page">page type</param>
        /// <returns>An instance of ITaskSetupPage</returns>
        public ITaskSetupPage CreatePageInstance<T>() where T : UserControl, ITaskSetupPage
        {
            return Container.GetInstance<T>() as T;
        }

        /// <summary>
        /// Initializes task (Registers types and creates logger).
        /// </summary>
        public virtual void Initialize()
        {
            if (IsInitialized == false)
            {
                RegisterPDMAddInFrameworkTypes();
            }
        }

        /// <summary>
        /// Attaches the debugger.
        /// </summary>
        public void AttachDebugger()
        {
            Process process = Process.GetCurrentProcess();
            if (!Debugger.IsAttached)
            {
                var information = new StringBuilder();
                information.AppendLine();
                information.AppendLine($"Process name = { process.ProcessName}");
                information.AppendLine($"Command Id   = { process.Id}");
                information.AppendLine($"Command type = { poCmd.meCmdType.ToString()}");

                if (MessageBox.Show($"Attach Debugger? {information.ToString()}", $"{Identity.Name} - {Identity.Version}", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    Debugger.Launch();
                
            }
        }

        private List<ITaskSetupPage> taskSetupPages = new List<ITaskSetupPage>();

        /// <summary>
        /// Add setup pages to task.
        /// </summary>
        /// <param name="taskSetupPages">Array of <see cref="ITaskSetupPage"/></param>
        protected void AddTaskSetupPages(ITaskSetupPage[] taskSetupPages)
        {
            Application.EnableVisualStyles();

            if (taskSetupPages == null)
                throw new NullReferenceException("taskSetupPages");

            var EdmTaskSetupPages = new List<EdmTaskSetupPage>();

            IEdmTaskProperties taskProperties = poCmd.mpoExtra as IEdmTaskProperties;
            if (taskProperties != null)
            {
                foreach (var taskSetupPage in taskSetupPages)
                {
                    var uc = taskSetupPage as UserControl;

                    if (uc == null)
                        throw new Exceptions.ITaskSetupPageNotUserControlException($"The supplied ITaskSetupPage is not a user control.", new Exception($"ITaskSetupPage name = ${taskSetupPage.Name}"));


                    if (taskSetupPage.Container == null)
                        throw new Exception("Please set the Container property.");


                    var EdmTaskSetupPage = new EdmTaskSetupPage();

                    uc.CreateControl();

                    taskSetupPage.LoadData(ref poCmd);

                    EdmTaskSetupPage.mbsPageName = taskSetupPage.Name;
                    EdmTaskSetupPage.mlPageHwnd = uc.Handle.ToInt32();
                    EdmTaskSetupPage.mpoPageImpl = uc;

                    EdmTaskSetupPages.Add(EdmTaskSetupPage);
                }

                this.taskSetupPages.Clear();
                this.taskSetupPages.AddRange(taskSetupPages);
            }

            var ar = EdmTaskSetupPages.ToArray();
            taskProperties.SetSetupPages(ar);

            Pages = taskSetupPages.ToArray();
        }


        public ITaskSetupPage[] Pages { get; set; }

        /// <summary>
        /// Add setup page to task. Use <seealso cref="CreatePageInstance{T}"/> to create instance of a page that implements <see cref="ITaskSetupPage"/>.
        /// </summary>
        /// <param name="taskSetupPage"></param>
        protected void AddTaskSetupPage(ITaskSetupPage taskSetupPage)
        {
            var l = new List<ITaskSetupPage>();
            l.Add(taskSetupPage);
            AddTaskSetupPages(l.ToArray());
         }

        
        /// <summary>
        /// Adds a context menu item to PDM.
        /// </summary>
        /// <param name="menuText">Text that will appear in the context menu.</param>
        /// <param name="statusBarHelpText">Text that will appear in the statusbar.</param>
        protected void AddContextMenu(string menuText, string statusBarHelpText)
        {
            // set task properties
            var props = poCmd.mpoExtra as IEdmTaskProperties;
            EdmTaskMenuCmd[] EdmTaskMenuCmds;
 
            props.GetMenuCmds(out EdmTaskMenuCmds);

            if (EdmTaskMenuCmds != null)
            {
                var command = new EdmTaskMenuCmd();
                int id = (new Random()).Next(1000,int.MaxValue);

                foreach (var menuCmd in EdmTaskMenuCmds)
                {
                    if (menuCmd.mbsMenuString == menuText)
                    return;
                }

                // set menu commands
                var cmds = new List<EdmTaskMenuCmd>();
                command.mbsMenuString = menuText;
                command.mbsStatusBarHelp = statusBarHelpText;
                command.mlCmdID = id;
                command.mlEdmMenuFlags = (int)EdmMenuFlags.EdmMenu_Nothing;
                cmds.Add(command);

                props.SetMenuCmds(cmds.ToArray());
            }
            
           
        }

        /// <summary>
        /// Deserialize the view model of the <see cref="TaskSetupPage{T}"/>. Use this method in the <see cref="EdmCmdType.EdmCmd_TaskRun"/> and <see cref="EdmCmdType.EdmCmd_TaskLaunch"/> to get the settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T</returns>
        public T GetSettings<T>()
        {
            var taskInstance = Instance; 
            if (taskInstance != null)
            {
                try
                {
                    var assemblyName = Path.GetFileName(this.GetType().Assembly.Location);
                    assemblyName = Path.ChangeExtension(assemblyName, "CAF");
                    var value = taskInstance.GetValEx($"{assemblyName}{taskInstance.TaskName}{typeof(T).Name}");
                    if (value != null)
                    {
                        var str = value.ToString();

                        return (T)Newtonsoft.Json.JsonConvert.DeserializeObject(str, typeof(T));

                    }
                }
                catch (Exception e)
                {
                    var exception = new SerializationException("Something went wrong while getting settings. Check inner exception.",e);
                    throw exception;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets the vault object.
        /// </summary>
        public IEdmVault5 Vault { get; private set; }

        /// <summary>
        /// Fires when an add-in is setup.
        /// </summary>
        /// <param name="poInfo">Info object.</param>
        /// <param name="poVault">Vault object.</param>
        /// <param name="poCmdMgr">Command manager object.</param>
        public virtual void GetAddInInfo(ref EdmAddInInfo poInfo, IEdmVault5 poVault, IEdmCmdMgr5 poCmdMgr)
        {
            try
            {
                bool isTask = false;
                Vault = poVault;
                // get attributes

                #region check GUID

                var guidAtt = Helper.GetFirstAttribute<GuidAttribute>(this);
                if (guidAtt == null)
                    throw new IdentityInfoException("Task class is not decorated with GuidAttribute.", null);

                #endregion check GUID

                #region check GUID

                var comVisibleAtt = Helper.GetFirstAttribute<ComVisibleAttribute>(this);
                if (comVisibleAtt == null)
                    throw new IdentityInfoException("Task class is not decorated with ComVisibleAttribute.", null);

                if(!comVisibleAtt.Value)
                    throw new IdentityInfoException("ComVisibleAttribute value is false.", null);

                #endregion check GUID

                #region set add-in name

                var addInNameAtt = Helper.GetFirstAttribute<NameAttribute>(this);
                if (addInNameAtt == null)
                    throw new IdentityInfoException("Task class is not decorated with NameAttribute.", null);

                if (string.IsNullOrWhiteSpace(addInNameAtt.Name))
                    throw new IdentityInfoException("Task add-in has an empty name.", null);

                poInfo.mbsAddInName = addInNameAtt.Name;

                #endregion set add-in name

                #region set add-in description

                var addinDescriptionAtt = Helper.GetFirstAttribute<DescriptionAttribute>(this);
                if (addinDescriptionAtt == null)
                    throw new IdentityInfoException("Task class is not decorated with DescriptionAttribute.", null);

                if (string.IsNullOrWhiteSpace(addinDescriptionAtt.Description))
                    throw new IdentityInfoException("Task add-in has an empty description.", null);

                poInfo.mbsDescription = addinDescriptionAtt.Description;

                #endregion set add-in description

                #region set add-in company name

                var addinCompanyNameAtt = Helper.GetFirstAttribute<CompanyNameAttribute>(this);
                if (addinCompanyNameAtt == null)
                    throw new IdentityInfoException("Task class is not decorated with CompanyNameAttribute.", null);

                if (string.IsNullOrWhiteSpace(addinCompanyNameAtt.CompanyName))
                    throw new IdentityInfoException("Task add-in has an empty company name.", null);

                poInfo.mbsCompany = addinCompanyNameAtt.CompanyName;

                #endregion set add-in company name

                #region set required pdm version

                var addinRequiredVersionAtt = Helper.GetFirstAttribute<RequiredVersionAttribute>(this);
                if (addinRequiredVersionAtt == null)
                    throw new IdentityInfoException("Task class is not decorated with RequiredVersionAttribute.", null);

                if (addinRequiredVersionAtt.Major == 0)
                    throw new IdentityInfoException("Task add-in version major requirement not set.", null);

                poInfo.mlRequiredVersionMajor = addinRequiredVersionAtt.Major;
                poInfo.mlRequiredVersionMinor = addinRequiredVersionAtt.Minor;

                #endregion set required pdm version

                #region set add-in version

                var addinVersionAtt = Helper.GetFirstAttribute<AddInVersionAttribute>(this);
                if (addinVersionAtt == null)
                    throw new IdentityInfoException("Task class is not decorated with AddInVersionAttribute.", null);

                if (addinVersionAtt.UseAssemblyFileRevision)
                {
                    Version version = Assembly.GetExecutingAssembly().GetName().Version;
                    poInfo.mlAddInVersion = version.Revision;
                }
                else
                {
                    if (addinVersionAtt.Version < 0)
                        throw new IdentityInfoException($"Task add-in has an incorrect version. Value ={addinVersionAtt.Version}", null);
                    else
                        poInfo.mlAddInVersion = addinVersionAtt.Version;
                }

                #endregion set add-in version

                #region get if add-in is a task

                var isTaskAtt = Helper.GetFirstAttribute<IsTaskAttribute>(this);
                if (isTaskAtt == null)
                    throw new IdentityInfoException("Task class is not decorated with IsTaskAttribute.", null);

                if (isTaskAtt != null)
                    isTask = isTaskAtt.IsTask;

                #endregion get if add-in is a task

                Identity.Name = poInfo.mbsAddInName;
                Identity.Description = poInfo.mbsDescription;
                Identity.CompanyName = poInfo.mbsCompany;
                Identity.Version = poInfo.mlAddInVersion;
                Identity.RequiredMajorVersion = poInfo.mlRequiredVersionMajor;
                Identity.RequiredMinorVersion = poInfo.mlRequiredVersionMinor;

                if (isTask)
                {
                    // hook into task commands
                    poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskDetails);
                    poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskLaunch);
                    poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskLaunchButton);
                    poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskSetupButton);
                    poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskSetup);
                    poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskRun);
                }

                #region get hooks 

              

                var hookAtts = Helper.GetAttributes<ListenForAttribute>(this);
                
                if (hookAtts != null)
                    foreach (var hookAtt in hookAtts)
                    {
                        if (hookAtt != null)
                            poCmdMgr.AddHook(hookAtt.Event);
                    }


                #endregion

                #region add commands 



                var MenuAtts = Helper.GetAttributes<MenuAttribute>(this);

                if (MenuAtts != null)
                    foreach (var MenuAtt in MenuAtts)
                    {
                        if (MenuAtt != null)
                            poCmdMgr.AddCmd(MenuAtt.ID, MenuAtt.MenuCaption, (int)MenuAtt.Flags, MenuAtt.StatusBarHelp, MenuAtt.Tooltip, MenuAtt.ToolButtonIndex, MenuAtt.ToolbarImageID);
                    }


                #endregion

            }
            catch (Exception e)
            {
                MessageBox.Show($"Error occurred while getting info. {e.Message}", "Installation error.");
            }
        }

 


        /// <summary>
        /// Sets the status of the task. Supports cancellation and suspension.
        /// </summary>
        /// <param name="status">Status type.</param>
        /// <param name="message">Message</param>
        /// <param name="beforeCancellationAction">Cleanup action before cancellation</param>
        /// <param name="cancellationAndSuspensionLogAction">Action used to log cancellation and or suspension</param>
        /// <param name="currentPosition">current position in the progress bar.</param>
        /// <exception cref="BlueByte.SOLIDWORKS.PDMProfessional.PDMAddInFramework.CancellationException">
        /// </exception>
        public virtual void SetStatus2(EdmTaskStatus status, string message, Action beforeCancellationAction, Action<string> cancellationAndSuspensionLogAction, int currentPosition = default(int))
        {


            string pauseCancellationMessage;

            if (Instance != null)
            {
                if (Instance.GetStatus() == EdmTaskStatus.EdmTaskStat_CancelPending)
                {
                    pauseCancellationMessage = "Task has been canceled.";

                    if (beforeCancellationAction != null)
                    {
                        beforeCancellationAction();
                    }

                    Instance.SetStatus(EdmTaskStatus.EdmTaskStat_DoneCancelled, 0, pauseCancellationMessage);
                    Instance.SetStatus(EdmTaskStatus.EdmTaskStat_DoneCancelled);
                    
                    if (cancellationAndSuspensionLogAction != null)
                    {
                        cancellationAndSuspensionLogAction(pauseCancellationMessage);
                    }
               

                    throw new CancellationException(pauseCancellationMessage);
                }

                if (Instance.GetStatus() == EdmTaskStatus.EdmTaskStat_SuspensionPending)
                {
                    pauseCancellationMessage = "Task has been suspended.";
                    Instance.SetStatus(EdmTaskStatus.EdmTaskStat_Suspended, 0, pauseCancellationMessage);
                    if (cancellationAndSuspensionLogAction != null)
                    {
                        cancellationAndSuspensionLogAction(pauseCancellationMessage);
                    }

                    while (Instance.GetStatus() == EdmTaskStatus.EdmTaskStat_Suspended)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                    if (Instance.GetStatus() == EdmTaskStatus.EdmTaskStat_ResumePending)
                    {
                        pauseCancellationMessage = "Task has resumed execution.";
                        Instance.SetStatus(EdmTaskStatus.EdmTaskStat_Running, 0, pauseCancellationMessage);
                        if (cancellationAndSuspensionLogAction != null)
                        {
                            cancellationAndSuspensionLogAction(pauseCancellationMessage);
                        }

                    }

                    //Check for cancellation if user cancels after pausing

                    if (Instance.GetStatus() == EdmTaskStatus.EdmTaskStat_CancelPending)
                    {
                        pauseCancellationMessage = "Task has been cancelled.";

                        if (beforeCancellationAction != null)
                        {
                            beforeCancellationAction();
                        }

                        Instance.SetStatus(EdmTaskStatus.EdmTaskStat_DoneCancelled, 0, pauseCancellationMessage);
                        Instance.SetStatus(EdmTaskStatus.EdmTaskStat_DoneCancelled);
                        if (cancellationAndSuspensionLogAction != null)
                        {
                            cancellationAndSuspensionLogAction(pauseCancellationMessage);
                        }


                        throw new CancellationException(pauseCancellationMessage);
                    }
                }

                //Instance.SetStatus(status, 0, message);

                //if (currentPosition == default(int))
                //    CurrentPosition = currentPosition;

                //else
                //    CurrentPosition = currentPosition++;

                //UpdateRange(CurrentPosition, message);
            }
        }

        /// <summary>
        /// Sets the progress range. Should be done only once at the start of the task execution
        /// </summary>
        /// <param name="range">Initialized range</param>
        /// <param name="currentPosition">Initial position</param>
        /// <param name="message">message</param>
        /// <exception cref="System.Exception"></exception>
        public void SetRange(int range, int currentPosition, string message = null)
        {
            Range = range;
            CurrentPosition = currentPosition;
            if (Range < CurrentPosition)
                throw new Exception($"{nameof(Range)} has to be superior than {nameof(CurrentPosition)}");

            Instance.SetProgressRange(Range, CurrentPosition, message);
        }

        /// <summary>
        /// Sets the progress bar position.
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="message"></param>
        public void UpdateRange(int currentPosition, string message = null)
        {
            if (Range < currentPosition)
            {
                throw new Exception($"{nameof(Range)} has to be superior than {nameof(CurrentPosition)}");
            }

            Instance.SetProgressPos(currentPosition, message);
        }
         
        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        /// <value>
        /// The current position.
        /// </value>
        public int CurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>
        /// The range.
        /// </value>
        public int Range { get; set; }

        /// <summary>
        /// Performs an action on affected data while allowing user to cancel or pause task.
        /// </summary>
        /// <param name="affectedData"></param>
        /// <param name="Action"></param>
        public void ForEachDatum(ref EdmCmdData[] affectedData, Action<EdmCmdData> Action)
        {
            var taskInstance = poCmd.mpoExtra as IEdmTaskInstance;

            foreach (var datum in affectedData)
            {
               
                Action(datum);
            }
        }

        /// <summary>
        /// Performs an action on affected file while allowing user to cancel or pause task.
        /// </summary>
        /// <param name="affectedData"></param>
        /// <param name="Action"></param>
        /// <remarks>This ignore EdmCmdData that are not files.</remarks>
        public void ForEachFile(ref EdmCmdData[] affectedData, Action<IEdmFile5> Action)
        {
            var taskInstance = poCmd.mpoExtra as IEdmTaskInstance;

            foreach (var datum in affectedData)
            {
                IEdmFile5 file = default(IEdmFile5);
                try
                {
                    file = Vault.GetObject(EdmObjectType.EdmObject_File, datum.mlObjectID1) as IEdmFile5;
                }
                catch (Exception)
                {
                }
 

                if (file != null)
                    Action(file);
            }
        }

        private void OnUnhandledExceptionThrown(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                if (BeforeYouCrash != null)
                    this.BeforeYouCrash(e.ExceptionObject as Exception);
            }

            LogExceptionBeforeCrash(Logger, e.ExceptionObject as Exception);
        }

        private void LogExceptionBeforeCrash(ILogger logger, Exception e)
        {
            if (logger is FileLogger)
            {
                if (string.IsNullOrWhiteSpace(logger.OutputLocation))
                    logger.OutputLocation = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{this.Identity.Name} {this.Identity.Version}");
            }

            var stringBuilder = new StringBuilder();
            var ex = e;
            var exceptionTypeName = ex.GetType().Name;
            StackTrace trace = new StackTrace(ex, true);

            string fileName = trace.GetFrame(0).GetFileName();
            int lineNo = trace.GetFrame(0).GetFileLineNumber();
            stringBuilder.AppendLine("An unhandled exception was thrown:");
            stringBuilder.AppendLine($" ");
            stringBuilder.AppendLine($"Exception type: {exceptionTypeName}");
            stringBuilder.AppendLine($"Exception message: {(ex).Message}");
            stringBuilder.AppendLine($"file name: {System.IO.Path.GetFileName(fileName)}");
            stringBuilder.AppendLine($"line count: {lineNo}");

            Func<Exception, Exception> findInnerMostException = (Exception x) =>
            {
                var tempE = x;
                while (tempE.InnerException != null)
                {
                    tempE = tempE.InnerException;
                }

                if (x == tempE)
                    return null;

                return tempE;
            };
            var innerMost = findInnerMostException(ex);
            if (innerMost != null)
            {
                exceptionTypeName = innerMost.GetType().Name;
                stringBuilder.AppendLine($"Inner most exception type: {exceptionTypeName}");
                stringBuilder.AppendLine($"Inner most exception message: {(innerMost).Message}");
            }

            #region add assembly name

            stringBuilder.AppendLine($" ");
            var executingAssemblyName = Assembly.GetExecutingAssembly()?.FullName;
            var callingAssemblyName = Assembly.GetCallingAssembly()?.FullName;
            var entryAssemblyName = Assembly.GetEntryAssembly()?.FullName;

            if (string.IsNullOrWhiteSpace(executingAssemblyName) == false)
                stringBuilder.AppendLine($"Executing assembly: {executingAssemblyName}");
            if (string.IsNullOrWhiteSpace(callingAssemblyName) == false)
                stringBuilder.AppendLine($"Calling assembly: {callingAssemblyName}");
            if (string.IsNullOrWhiteSpace(entryAssemblyName) == false)
                stringBuilder.AppendLine($"Entry assembly: {entryAssemblyName}");

            #endregion add assembly name

            var caption = stringBuilder.ToString();

            logger.LogToOutput($"{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.txt", caption);
        }

        /// <summary>
        ///  Fires when task details are initialized
        /// </summary>
        /// <param name="poCmd">Command</param>
        /// <param name="ppoData">Affected documents</param>
        public virtual void OnTaskDetails(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
           
            this.Initialize();
        }

        /// <summary>
        /// Fires when task is setup.
        /// </summary>
        /// <param name="poCmd"></param>
        /// <param name="ppoData"></param>
        public void OnTaskSetup(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            
            this.Properties = poCmd.mpoExtra as IEdmTaskProperties;

            var flags = Helper.GetFirstAttribute<TaskFlagsAttribute>(this);
            if (flags != null)
                this.Properties.TaskFlags = flags.Flags;

            this.Initialize();
            OnTaskDialogWindowCreated(poCmd.mpoExtra as IEdmTaskProperties);
        }

        /// <summary>
        /// Fires when the task dialog window is created. This occurs when you create a new task or edit an existing one.
        /// </summary>
        public virtual void OnTaskDialogWindowCreated(IEdmTaskProperties properties)
        {
        }

        /// <summary>
        /// Fires when a task is launched.
        /// </summary>
        /// <param name="poCmd">Command</param>
        /// <param name="ppoData">Affected documents</param>
        public virtual void OnTaskLaunch(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            
         
            this.Instance = poCmd.mpoExtra as IEdmTaskInstance;
            this.Initialize();
        }

        /// <summary>
        /// Fires when task runs on the executing machine.
        /// </summary>
        /// <param name="poCmd">Command</param>
        /// <param name="ppoData">Affected documents</param>
        public virtual void OnTaskRun(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
          
            this.Instance = poCmd.mpoExtra as IEdmTaskInstance;
            this.Initialize();
        }

        /// <summary>
        /// Fires when user clicks OK button in the task setup.
        /// </summary>
        /// <param name="poCmd">Command</param>
        /// <param name="ppoData">Affected documents</param>
        public virtual void OnTaskSetupButton(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
       
            this.Initialize();

            foreach (var taskSetup in taskSetupPages)
            {
                if (taskSetup != null)
                    taskSetup.StoreData(ref poCmd);
            }
        }

        /// <summary>
        /// Fires when user clicks on the launch button.
        /// </summary>
        /// <param name="poCmd">Command</param>
        /// <param name="ppoData">Affected documents</param>
        public virtual void OnTaskLaunchButton(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
         
            this.Initialize();
        }

        /// <summary>
        /// Fires a response to a command that the add-in is hooked too is being trigger.
        /// </summary>
        /// <param name="poCmd">PDM command</param>
        /// <param name="ppoData">Affected data</param>
        public virtual void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            Vault = poCmd.mpoVault as IEdmVault5;
            this.poCmd = poCmd;

            GetAddInIdentity();

            OnLoggerStartConnectionWithSQLServer(string.Empty);

            var isTask = false;
            var isTaskAtt = Helper.GetFirstAttribute<IsTaskAttribute>(this);
            if (isTaskAtt == null)
                throw new IdentityInfoException("Task class is not decorated with IsTaskAttribute.", null);

            if (isTaskAtt != null)
                isTask = isTaskAtt.IsTask;

            if (isTask)
            {
                switch (poCmd.meCmdType)
                {
                    case EdmCmdType.EdmCmd_TaskSetup:
                        OnTaskSetup(ref poCmd, ref ppoData);
                        break;

                    case EdmCmdType.EdmCmd_TaskSetupButton:
                        OnTaskSetupButton(ref poCmd, ref ppoData);
                        break;

                    case EdmCmdType.EdmCmd_TaskDetails:
                        OnTaskDetails(ref poCmd, ref ppoData);
                        break;

                    case EdmCmdType.EdmCmd_TaskRun:
                        OnTaskRun(ref poCmd, ref ppoData);
                        break;

                    case EdmCmdType.EdmCmd_TaskLaunch:
                        OnTaskLaunch(ref poCmd, ref ppoData);
                        break;

                    case EdmCmdType.EdmCmd_TaskLaunchButton:
                        OnTaskLaunchButton(ref poCmd, ref ppoData);
                        break;

                    default:
                        break;
                }
            }

        


        }


        private void GetAddInIdentity()
        {
            #region set add-in name

            var addInNameAtt = Helper.GetFirstAttribute<NameAttribute>(this);
            if (addInNameAtt == null)
                throw new IdentityInfoException("Task class is not decorated with NameAttribute.", null);

            if (string.IsNullOrWhiteSpace(addInNameAtt.Name))
                throw new IdentityInfoException("Task add-in has an empty name.", null);

       

            #endregion set add-in name

            #region set add-in description

            var addinDescriptionAtt = Helper.GetFirstAttribute<DescriptionAttribute>(this);
            if (addinDescriptionAtt == null)
                throw new IdentityInfoException("Task class is not decorated with DescriptionAttribute.", null);

            if (string.IsNullOrWhiteSpace(addinDescriptionAtt.Description))
                throw new IdentityInfoException("Task add-in has an empty description.", null);

      
            #endregion set add-in description

            #region set add-in company name

            var addinCompanyNameAtt = Helper.GetFirstAttribute<CompanyNameAttribute>(this);
            if (addinCompanyNameAtt == null)
                throw new IdentityInfoException("Task class is not decorated with CompanyNameAttribute.", null);

            if (string.IsNullOrWhiteSpace(addinCompanyNameAtt.CompanyName))
                throw new IdentityInfoException("Task add-in has an empty company name.", null);

    
            #endregion set add-in company name

            #region set required pdm version

            var addinRequiredVersionAtt = Helper.GetFirstAttribute<RequiredVersionAttribute>(this);
            if (addinRequiredVersionAtt == null)
                throw new IdentityInfoException("Task class is not decorated with RequiredVersionAttribute.", null);

            if (addinRequiredVersionAtt.Major == 0)
                throw new IdentityInfoException("Task add-in version major requirement not set.", null);

         
            #endregion set required pdm version

            #region set add-in version

            var addinVersionAtt = Helper.GetFirstAttribute<AddInVersionAttribute>(this);
            if (addinVersionAtt == null)
                throw new IdentityInfoException("Task class is not decorated with AddInVersionAttribute.", null);

            if (addinVersionAtt.UseAssemblyFileRevision)
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                Identity.Version = version.Revision;
            }
            else
            {
                if (addinVersionAtt.Version < 0)
                    throw new IdentityInfoException($"Task add-in has an incorrect version. Value ={addinVersionAtt.Version}", null);
            }

            #endregion set add-in version

           
            
            Identity.Name = addInNameAtt.Name;
            Identity.Description = addinDescriptionAtt.Description;
            Identity.CompanyName = addinCompanyNameAtt.CompanyName;
            if (addinVersionAtt.UseAssemblyFileRevision)
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
              }
            else 
            Identity.Version = addinVersionAtt.Version;
            Identity.RequiredMajorVersion = addinRequiredVersionAtt.Major;
            Identity.RequiredMinorVersion = addinRequiredVersionAtt.Minor;
        }
    }
}