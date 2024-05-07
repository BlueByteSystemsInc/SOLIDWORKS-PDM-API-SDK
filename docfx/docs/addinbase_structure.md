# AddInBase structure

>[!NOTE]
> For the complete API reference of the AddInBase class, please refer to this [page](https://bluebytesystemsinc.github.io/SOLIDWORKS-PDM-API-SDK/api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html).

In the SOLIDWORKS PDM API, your add-in's class must implement the [IEdmAddIn5](https://help.solidworks.com/2023/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.IEdmAddIn5.html) interface. In PDMSDK, AddInBase is the **only** class that has to be inherited in order to create a working PDM add-in. This abstract class does implement the [IEdmAddIn5](https://help.solidworks.com/2023/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.IEdmAddIn5.html) interface and packs a lot of useful functionlities.  

PDM add-ins can be broken down into two different types:

- Standard add-in

  - Can hook into various PDM events such as check-in/check-out, state changes, adding/deleting files, etc...

- Task add-in

  - Can be run as current or another specified user
  - Can be run on current PC or another specified PC
  - Can be scheduled to run at a specific time
  - Can be set to execute as part of a document workflow

While add-ins follow the simple client-server model, task add-ins are slightly more complex. A PDM workload can distributed to several task machines. The overall archiecture can be described by the following diagram: 

<img src="https://help.solidworks.com/2021/english/EnterprisePDM/Admin/ems1450377335497.image"/>


>[!TIP]
> SOLIDWORKS Corp defines tasks as feature in the administration tool that lets you configure, run, and monitor tasks that you perform frequently on SOLIDWORKS PDM files.. For more information, please visit this [page](https://help.solidworks.com/2021/english/EnterprisePDM/Admin/c_Print_Plot_Convert.htm).

#  Definition attributes

To configure an add-in and specify its functionality, attributes are used to decorate the class that implements AddInBase.

The following attributes define what is shown in the Administration UI Properties of the add-in:

- `[Name("Sample PDM Add-In")]`
- `[CompanyName("My Company Name")]`
- `[AddInVersion(false, 1)]`
- `[RequiredVersion(29, 0)]`
- `[Description("This is the description of what this add-in will do.")]`
  <img src="../images/Sample Add-In Administration Properties.png"/>

Task add-in specific attributes:

- `[IsTask(true)]` This defines whether the add-in is a task add in (true) or not (false).
- `[TaskFlags(EdmTaskFlag flag)]` These agruments indicate how the add-in can be incorporated into PDM.

> [!Note]
> The framework automatically hooks into all task events when `[IsTask()]` is set to `true`, no need to use the `[ListenFor(EdmCmdType event)]` attribute].

Standard add-in attributes:

- `[ListenFor(EdmCmdType event)]` tells the add-in which PDM event to hook into. Each time a hooked event is triggered within PDM, it will call OnCmd().

Required Windows attributes:

- `[ComVisible(true)]` This allows PDM to see the add-in.
- `[Guid("")]` A unique ID generated that is specific to your project. 

>[!Note]
>In Visual Studio, you can generate a new guid by going to Tools -> Create GUID.

# Common members

Look up the help page for these members using the [documentation](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.html) link above.

| Property/Method      | Purpose |
| ----------- | ----------- |
| [Logger](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_Logger)      | Returns the ILogger interface.  |
| [Properties](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_Properties)  | Returns the [IEdmProperties](https://help.solidworks.com/2020/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.IEdmTaskProperties.html) object. Be aware that this is only available for EdmTaskSetupButton and EdmTaskSetup hooks. Otherwise, this property will turn null or nothing.        |
|[Instance](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_Instance)| Returns the [IEdmTaskInstance](https://help.solidworks.com/2020/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.IEdmTaskInstance.html) object. Be aware that this is only available for EdmTaskLaunch, Run and Details hooks. Otherwise, this property will return null or nothing.|
|[Identity](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_Identity)| This structure captures the values used in the definition attributes. Use the method ToCaption() to return the name of the add-in and its version without using string concatenation. This particularly useful for setting the title of a form.|
|[Container](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_Container)| This dependency injection container will resolve any types that registered in the [RegisterAdditionalTypes](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_RegisterAdditionalTypes) method.|
|[ForEach](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_ForEach)| Provides a mechanism to apply a lambda expression to the affected items. Be aware that this is only valid for files (IEdmFile5). Other items affected by the command that are not files will be ignored.

>[!NOTE]
> AddInBase contains properties and methods that sare pecific to task add-ins and are covered in further details in the next sections.
# Example

# [C Sharp](#tab/cs)
```
namespace MyPdmAddIn
{
    [Name("Sample PDM Add-In")]
    [CompanyName("My Company Name")]
    [AddInVersion(false, 1)]
    [RequiredVersion(29, 0)]
    [Description("This is the description of what this add-in will do.")]

    [IsTask(true)]
    [TaskFlags((int)EdmTaskFlag.EdmTask_SupportsChangeState + (int)EdmTaskFlag.EdmTask_SupportsDetails + (int)EdmTaskFlag.EdmTask_SupportsInitExec + (int)EdmTaskFlag.EdmTask_SupportsScheduling)]


    [ComVisible(true)]
    [Guid("0DF94D2F-9FCE-40E2-85E9-CCD5B9DE0DC6")]
    public class AddIn : AddInBase
    {
        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            base.OnCmd(ref poCmd, ref ppoData);

            // Add your add-in code here
        }
    }
}
```
# [VB](#tab/VB)
```
To be provided. 
// if you see this, please contribute to our repo but providing a translation of C# to VB.NET
```
---

