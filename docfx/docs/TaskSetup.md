#Definition

The task setup hook gets triggered when you select the add-in from the dropdown in the task dialog or you edit an add-in's task.

The task setup hook allows you to load and save data into the task. It also gives the ability to add custom tabs to the task setup dialog.

# Adding a EdmTaskPage

>[!TIP]
> You can completely ignore the EdmTaskSetup hook if you do not intend on storing data in the task. Most of the time, task contain data such logging path, export path or any specific settings that needed as part of its implementation.

Consult the Task Sample from the SOLIDWORKS PDM Professional API to see the default implementation of this hook. We highly recommend you use PDMSDK's because it saves a lot of time.

You can one of many tabs. In PDMSDK terminogly, we can them TaskPage. PDMSK has two types: 

- EdmTaskPage: It is a generic wrapper around the Winforms UserControl with built support for saving and loading data in the task. The data is serialized and deserialized. The data object must be a serialized class. the EdmTaskPage takes care of creating or loading an instance of your data class as well as saving into the task.
- ITaskPage: An interface that the EdmTaskPage implements.

>[!TIP]
> We highly recommend you use the PDMSDK Visual Studio Template because they come ready with a boilerplate EdmTaskPage item to be added to your project.


To add an EdmTaskSetup:
- Right-click on the *Project* in the *Solution Explorer* and click Add *New Item...*
- Look for EdmTaskSetupPage or EdmTaskPage and click Add.
- By default, two items are added: The EdmTaskPage and the data object which is called SettingsViewModel.
- An EdmTaskPage is a UserControl. You can use the Visual Studio Toolbox to add and remove WinForms controls to it.
- Requirement: In the OnDataMethod in the EdmTasksPage, you can bind your properties from the data object to your controls in EdmTaskPage.

# OnCmd

In the OnCmd implementation:

- Create instances of your task pages.
- Setup the Container property from EdmTaskSetup to that of the AddInBase.
- Call the method AddTaskSetupPages and supply it with an array of your task pages.

# Debugging 

>[!IMPORTANT]
> You cannot debug EdmTaskSetup with *Debug Add-ins...*


- Make sure you have a fresh connection to PDM. Generally speaking this is done by clicking Conisioadmin.exe, edmserver.exe and explorer.exe and then restarting explorer.exe. This process can be done from the Windows task manager.
- Call AttachDebugger in your OnCmd.
- Compile code.
- Add project files to your vault from the administration tool. Right-click on *Add-ins* and click *New Add-in...*
- Accept all dialogs.
- Right-click on *Tasks* and click *New Task...*
- Choose the add-in that you just added from the dropdown.
- AttachDebugger will called immediately and you can attach Visual Studio to start debugging through your code.


>[!TIP]
> You can use the directives #if DEBUG #endif to conditionally compile the AttachDebugger in the debug configuration. This will ignore in the Release configuration which you can release to production. Be aware that release configuration does not automatically generate pdb files so your stack traces will not be printed in your error or logs.
