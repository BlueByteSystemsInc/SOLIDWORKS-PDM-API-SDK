# Definition

The task launch hook gets triggered when your task by launched. A task can be launched by one computer and executed (ran) on another one. It is very common for the executing computer to be the PDM server.

Almost all of your task implementation in the OnCmd method will be handled by the TaskRun hook.



>[!NOTE]
>Use TaskLaunch to show any necessary UI to get the user input.

# IEdmTaskInstance object

You may need, depending on your OnCmd implementation, to access the task instance. 

In PDMSDK, use [Instance](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_Instance) to get access to the IEdmTaskInstance object. This property returns a value in TaskLaunch, launch and, Details.

 
# Cancellation and suspension 


You cannot suspend a task during its launch. 

You are able to cancel the task by means setting the mpoCancel field on the EdmCmd parameter just like you would do with any hook.

# Transfering data from task launch to task run

>[!IMPORTANT]
>It is critical to understand that your add-in will invoked by two different processes one during TaskLaunch and one during TaskRun. Every process will have its own ID. 

When a task a launched, it is launched with TaskLauncher.exe and when it is run, it runs in the TaskExecuter.exe process. You can find out more about these processes by using the AttachDebugger method.


This leaves data transfer  from TaskLaunch to TaskRun only possible via the GetVarEx and SetVarEx methods part of the IEdmTaskInstance interface.




# Getting saved data 

To get access to the data you have saved in the task setup, call the method [GetSettings<T>](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_GetSettings__1) where T is the view model type. Example:


# [C Sharp](#tab/cs)
```
var messagingViewModel = base.GetSettings<MessagingViewModel>();
```
# [VB](#tab/VB)
```
Dim messagingViewModel As MessagingViewModel = MyBase.GetSettings(Of MessagingViewModel)()
```
---
