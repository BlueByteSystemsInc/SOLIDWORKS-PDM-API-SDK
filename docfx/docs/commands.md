# Definition
Commands in SOLIDWORKS PDM can be triggered from the toolbar menus, buttons, or the right-click menu. Conventionally, you add commands using the command manager [IEdmCmdMgr5](https://help.solidworks.com/2018/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.IEdmCmdMgr5.html) in the GetAddInfo implementation. 


# Commands in PDMSDK
Since PDMSDK hides the GetAddInInfo implementation, we use the  [MenuAttribute](../api/SOLIDWORKS-PDM-API-SDK/api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes.MenuAttribute.html) to add commands capbilitites to your add-in.

To add a command as part of the right-click menu: 

- Start by defining an Enum for your commands. For example 
```
// this piece of code defines two commands. Make sure your commands values are unique.
 public enum Commands_e
 {
   Settings = 15615,
   ProcessFile = 475
 }
```
- Decorate your add-in class with the [MenuAttribute](../api/SOLIDWORKS-PDM-API-SDK/api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes.MenuAttribute.html). Example: 
```
[Menu((int)Commands_e.ProcessFile, "MyPDMAddIn\\Process File(s)", 35, "","",-1,0)]
```

>[!TIP]
> Use `\\` in the command caption to create sub menus. See the example above.


>[!TIP]
> Please make you read the MenuAttribute constructors paramrters. The flags parameter defines the visibility of the command. The flags are a sum of the [EdmMenuFlags](https://help.solidworks.com/2014/english/api/epdmapi/epdm.interop.epdm~epdm.interop.epdm.edmmenuflags.html) enum.

For the flags: 

- Use 35 if you want to show the command in the RMB for files with support for single and multi-selection.
- Use 43 if you want to show the command in the RMB for files with support for single selection only.
- You can cast the value of EdmMenuFlags into an integer as a flag. For example (int)EdmMenuFlags.EdmMenu_Administration will add the menu to the right-click menu of the add-in the administration tool.

# OnCmd Implementation

Commands trigger the OnCmd method. Make sure to approprietly check for the invoked command ID with poCmd.meCmdID  and the command type with poCmd.meCmdType (In this case, it will EdmCmdType.EdmCmdMenu). 


# Command Visibility 

With PDMSDK, you restrict commands to be visible to specific users, groups or users with specific permissions. 

You can do that via the [CommandVisibilityAttribute](../api/SOLIDWORKS-PDM-API-SDK/api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes.CommandVisibilityAttribute.html) attribute. Decorate your add-in class with the CommandVisibility attribute. The example below shows the settings command only the users with edit addins permission.

```
 [CommandVisibility((int)Commands_e.Settings, EdmSysPerm.EdmSysPerm_EditAddins)]
 
```

# Code sample


# [C Sharp](#tab/cs)
```
 public enum Commands_e
 {
   Settings = 15615,
   ProcessFile = 475
 }

    [Name("MyAddIn")]
    [Description("MyAddIn Description")]
    [CompanyName("Blue Byte Systems Inc.")]
    [ListenFor(EdmCmdType.EdmCmd_Menu)]
    [Menu((int)Commands_e.ProcessFile, "MyAddIn\\ProcessFile", 35, "","",-1,0)]
    [Menu((int)Commands_e.Settings, "Settings", (int)EdmMenuFlags.EdmMenu_Administration, "","",-1,0)]
    [CommandVisibility((int)Commands_e.Settings, EdmSysPerm.EdmSysPerm_EditAddins)]
    [AddInVersion(false, 1)]
    [IsTask(false)]
    [RequiredVersion(10, 0)]
    [ComVisible(true)]
    [Guid("2968ACED-D471-4704-AC44-68B28549F8295")]
    public partial class AddIn : AddInBase
    {
        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            base.OnCmd(ref poCmd, ref ppoData);

            try 
            {
                
            switch(poCmd.meCmdType)
            {
                case EdmCmdType.EdmCmd_Menu:

                if (poCmd.mlCmdID == (int)Commands_e.ProcessFile)
                {
                     foreach(var ppoDatum in ppoData)
                    {
                        // Do something to each file before it is checked-in
                    }

                    return;
                }

                if (poCmd.mlCmdID == (int)Commands_e.Settings)
                {
                     foreach(var ppoDatum in ppoData)
                    {
                        // show settings
                    }
                }

                   
               break;
            }

            }
            catch(TaskFailedException e)
            {
                
            }
            catch(CancellationException e)
            {

            }
            catch(COMException e)
            {

            }
            catch(Exception e)
            {

            }
            
        }
    }

```
# [VB](#tab/VB)
```
Public Enum Commands_e
    Settings = 15615
    ProcessFile = 475
End Enum

<Name("MyAddIn")>
<Description("MyAddIn Description")>
<CompanyName("Blue Byte Systems Inc.")>
<ListenFor(EdmCmdType.EdmCmd_Menu)>
<Menu(CType(Commands_e.ProcessFile, Integer), "MyAddIn\ProcessFile", 35, "", "", -1, 0)>
<Menu(CType(Commands_e.Settings, Integer), "Settings", CType(EdmMenuFlags.EdmMenu_Administration, Integer), "", "", -1, 0)>
<CommandVisibility(CType(Commands_e.Settings, Integer), EdmSysPerm.EdmSysPerm_EditAddins)>
<AddInVersion(False, 1)>
<IsTask(False)>
<RequiredVersion(10, 0)>
<ComVisible(True)>
<Guid("2968ACED-D471-4704-AC44-68B28549F8295")>
Public Partial Class AddIn
    Inherits AddInBase

    Public Overrides Sub OnCmd(ByRef poCmd As EdmCmd, ByRef ppoData As EdmCmdData())
        MyBase.OnCmd(poCmd, ppoData)

        Try
            Select Case poCmd.meCmdType
                Case EdmCmdType.EdmCmd_Menu
                    If poCmd.mlCmdID = CInt(Commands_e.ProcessFile) Then
                        For Each ppoDatum In ppoData
                            ' Do something to each file before it is checked-in
                        Next
                        Return
                    End If

                    If poCmd.mlCmdID = CInt(Commands_e.Settings) Then
                        For Each ppoDatum In ppoData
                            ' Show settings
                        Next
                    End If
            End Select

        Catch e As TaskFailedException

        Catch e As CancellationException

        Catch e As COMException

        Catch e As Exception

        End Try
    End Sub
End Class
```
---
