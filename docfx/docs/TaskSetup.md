# Definition

The task setup hook gets triggered when you select the add-in from the dropdown in the task dialog or you edit an add-in's task.

The task setup hook allows you to load and save data into the task. It also gives the ability to add custom tabs to the task setup dialog. This tabs are WinForms UserControls.

# Adding a EdmTaskPage

>[!TIP]
> If your task does not have any settings, you can completely ignore the EdmTaskSetup hook. There is no need to store data in the task.

Consult the [Task Sample](https://help.solidworks.com/2017/english/api/epdmapi/TaskSample.htm) from the SOLIDWORKS PDM Professional API help to see the default implementation of this hook. 

We highly recommend you use PDMSDK's because it saves a lot of time.

You can add one of many custom tabs. In PDMSDK terminogly, we call them task page. A task page is presented by the type EdmTaskPage.

You need to be aware of two types: 

- EdmTaskPage: It is a generic wrapper around the WinForms UserControl with built-in support for saving and loading data in the task. The data is serialized and deserialized. The data object must be a serialized class. the EdmTaskPage takes care of creating or loading an instance of your data class as well as saving into the task. Your data object is property called ViewModel in the EdmTaskPage.

- ITaskPage: An interface that the EdmTaskPage implements. 

>[!TIP]
> We highly recommend you use the PDMSDK Visual Studio Template because it comes ready with a boilerplate EdmTaskPage item to be added to your project.


To add an EdmTaskSetup:
- Right-click on the *Project* in the *Solution Explorer* and click Add *New Item...*
- Look for EdmTaskSetupPage or EdmTaskPage and click Add.
- By default, two items are added: The EdmTaskPage and the data object which is called EdmTaskPageViewModel.
- An EdmTaskPage is a UserControl. You can use the Visual Studio Toolbox to add and remove WinForms controls to and from it.
- Requirement: In the OnDataLoaded method in the EdmTasksPage, you can bind your the data object properties to the controls in the EdmTaskPage.

# OnCmd

In the OnCmd implementation:

- Create instances of your task pages.
- Set the Container property in the EdmTaskSetup to that of the AddInBase. We're passing the Container from the AddInBase to the EdmTaskPage. 
- Call the method AddTaskSetupPages and supply it with an array of your task pages.

# Debugging 

>[!IMPORTANT]
> You cannot debug task setup pages with *Debug Add-ins...*


- Make sure you have a fresh connection to PDM. Generally speaking, this is done by killing Conisioadmin.exe, edmserver.exe and explorer.exe and then restarting explorer.exe. This process can be done from the Windows task manager.
- Call AttachDebugger in your OnCmd.
- Compile code.
- Add the project files to your vault from the administration tool. Right-click on *Add-ins* and click *New Add-in...*
- Accept all dialogs.
- Right-click on *Tasks* and click *New...*
- Choose the add-in that you just added from the dropdown.
- AttachDebugger will be called immediately and you can attach Visual Studio to start debugging through your code.


>[!TIP]
> You can use the directives #if DEBUG #endif to conditionally compile the AttachDebugger in the debug configuration. This will ignore it in the Release configuration which you can use in production. Be aware that release configurations do not automatically generate pdb files so your stack traces will not print in your error or logs.
