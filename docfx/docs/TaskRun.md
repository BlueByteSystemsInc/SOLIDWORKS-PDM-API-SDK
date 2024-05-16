# Definition

The task run hook gets triggered when your task gets transfered from the launch machine to the run. A task can be launched by one computer and executed (ran) on another one. It is very common for the executing computer to be the PDM server.

Almost all of your task implementation in the OnCmd method will be handled by the TaskRun hook.

# IEdmTaskInstance object

You may need, depending on your OnCmd implementation, to access the task instance. 

In PDMSDK, use [Instance](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_Instance) to get access to the IEdmTaskInstance object. This property returns a value in TaskLaunch, Run and, Details.

# Updating the progressbar 

To update the progress bar in the tasklist use these two methods.

- [SetRange](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_SetRange_System_Int32_System_Int32_System_String_)
- [UpdateTaskMessage](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_UpdateTaskMessage_System_Int32_System_String_)

>[!Warning]
>[UpdateTaskMessage](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_UpdateTaskMessage_System_Int32_System_String_) will throw an exception if the provided position is out of bounds of the specified range.


# Cancellation and suspension 

Whenever you want to give your implementation a chance to consider if the user has requested a suspension or a cancellation call [UpdateTaskMessage](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_UpdateTaskMessage_System_Int32_System_String_). 

>[!Warning]
>In case of a cancellation, [UpdateTaskMessage](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_UpdateTaskMessage_System_Int32_System_String_) will throw a CancellationException that you need to handle. 


>[!NOTE]
>[UpdateTaskMessage](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_UpdateTaskMessage_System_Int32_System_String_) handles cancellation, suspension and update the progressbar and the message in the tasklist.

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

