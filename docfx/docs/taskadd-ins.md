# Definition

Task add-ins are the network distributed version PDM add-ins. You can create instances of the add-in called tasks and execute them on a specific computer in your PDM network.

Programmtivally, The main difference is that a task add-in observes 5 different hooks which are:

- [EdmTaskSetup](https://help.solidworks.com/2023/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.EdmCmdData.html)
- [EdmTaskSetupButton](https://help.solidworks.com/2023/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.EdmCmdData.html)
- [EdmTaskLaunch](https://help.solidworks.com/2023/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.EdmCmdData.html)
- [EdmTaskRun](https://help.solidworks.com/2023/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.EdmCmdData.html)
- [EdmTaskDetails](https://help.solidworks.com/2023/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.EdmCmdData.html)

>[!NOTE]
> Every task add-in is a PDM add-in but not every add-in is a PDM task add-in. 


# Creating a task add-in

Conventially, you need your command manager [IEdmCmdMgr5](https://help.solidworks.com/2018/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.IEdmCmdMgr5.html) to add the hooks above in the implemention of your GetAddInInfo. This is how it is code using the code from the task sample from the SOLIDWORKS PDM Professional API help page.

```
Public Sub GetAddInInfo(ByRef poInfo As EdmAddInInfo, ByVal poVault As IEdmVault5, ByVal poCmdMgr As IEdmCmdMgr5) Implements IEdmAddIn5.GetAddInInfo
On Error GoTo ErrHand
  ' Fill in the add-in's description
  poInfo.mbsAddInName = "Task Test Add-in"
  poInfo.mbsCompany = "SOLIDWORKS"
  poInfo.mbsDescription = "Add-in used to test the task execution system"
  poInfo.mlAddInVersion = 1

  ' Minimum SOLIDWORKS PDM Professional version needed for VB.NET add-ins is 2010
  poInfo.mlRequiredVersionMajor = 10
  poInfo.mlRequiredVersionMinor = 0

  'Register this add-in as a task add-in
  poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskRun)
  'Register this add-in as being able to append its own property pages in the Administration tool
  poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskSetup)
  'Register this add-in to be called when the task is launched on the client computer
  poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskLaunch)
  'Register this add-in to provide extra details in the Details dialog box in the task list in the Administration tool
  poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskDetails)
  'Register this add-in to be called when the launch dialog box is closed
  poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskLaunchButton)
  'Register this add-in to be called when the set-up wizard is closed
  poCmdMgr.AddHook(EdmCmdType.EdmCmd_TaskSetupButton)

Exit Sub

ErrHand:
Dim v11 As IEdmVault11
v11 = poVault
MsgBox(v11.GetErrorMessage(Err.Number))
End Sub
```

# Using PDMSDK

The class that implements the [AddInBase](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html) needs to be decorated with [IsTask](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes.IsTaskAttribute.html) attribute. 


>[!NOTE]
> This attribute is already defined for you if you have used the [PDM SDK Template](https://marketplace.visualstudio.com/items?itemName=BlueByteSystemsInc.ID) to create your add-in project. 


# [C Sharp](#tab/cs)
```
    [IsTask(true)]
    [RequiredVersion(10, 0)]
    [ComVisible(true)]
    [Guid("00000-00-4745-97F6-2029AFB70716")]

    public partial class AddIn : AddInBase
    {
        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {}
    }
```
# [VB](#tab/VB)
```
Imports System.Runtime.InteropServices
<IsTask(True)>
<RequiredVersion(10, 0)>
<ComVisible(True)>
<Guid("00000-00-4745-97F6-2029AFB70716")>
Public Partial Class AddIn
    Inherits AddInBase

    Public Overrides Sub OnCmd(ByRef poCmd As EdmCmd, ByRef ppoData() As EdmCmdData)
        ' Implement the method logic here
    End Sub
End Class
```
---


>[!WARNING]
> Do not delete this attribute otherwise you will get an error dialog box when you add the add-in files in the vault. This attribute **is required**.