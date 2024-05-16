# Definition

The task setup button hook gets triggered when you click on the OK or Cancel button in the following window.

The task setup hook allows you to save data into the task. It also gives the ability add context menu if your task defines that in task flags. 


# Saving task page's data

>[!WARNING]
> You must reuse the same instance from the TaskSetup hook.

- Call the method StoreData in your EdmTaskPage.
- You may also call AddContextMenu to add a right-click that triggers the task launch from File Explorer.


See the complete code example below.

# Example


# [C Sharp](#tab/cs)
```
Pages.Messaging taskSetupMessagingTab = default(Pages.Messaging);
       
        public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
        {
            base.OnCmd(ref poCmd, ref ppoData);

            int handle = poCmd.mlParentWnd;

            try
            {
                switch (poCmd.meCmdType)
                {
                    
                    case EdmCmdType.EdmCmd_TaskSetup:
                        {
                            taskSetupMessagingTab = new Pages.Messaging();
                            taskSetupMessagingTab.Container = base.Container;
                            AddTaskSetupPage(taskSetupMessagingTab);
                            // or if you have multiple pages
                            // AddTaskSetupPages(new BlueByte.SOLIDWORKS.PDMProfessional.SDK.Core.ITaskPage[] {  taskSetupMessagingTab});

                        }
                        break;
                    case EdmCmdType.EdmCmd_TaskSetupButton:
                        {
                            // add a context menu 
                            AddContextMenu($"Tasks\\{Properties.TaskName} [V{Identity.Version}]", Identity.Description);
                            
                            // save the data of the tab
                            taskSetupMessagingTab.StoreData(ref poCmd);

                        }
                        break;
                    case EdmCmdType.EdmCmd_TaskDetails:
                        break;
                    case EdmCmdType.EdmCmd_TaskRun:
                        break;
                    case EdmCmdType.EdmCmd_TaskLaunch:
                        break;
                    case EdmCmdType.EdmCmd_TaskLaunchButton:
                    default:
                        break;
                }
            }
            catch (CancellationException e)
            {

                throw;
            }
            catch (TaskFailedException e)
            {

                throw;
            }
            // this is a PDM exception
            catch (COMException e)
            {

                throw;
            }
            catch (Exception e)
            {

                throw;
            }

        }
```
# [VB](#tab/VB)
```
 Private taskSetupMessagingTab As Pages.Messaging = Nothing

    Public Overrides Sub OnCmd(ByRef poCmd As EdmCmd, ByRef ppoData() As EdmCmdData)
        MyBase.OnCmd(poCmd, ppoData)

        Dim handle As Integer = poCmd.mlParentWnd

        Try
            Select Case poCmd.meCmdType
                Case EdmCmdType.EdmCmd_TaskSetup
                    taskSetupMessagingTab = New Pages.Messaging()
                    taskSetupMessagingTab.Container = MyBase.Container
                    AddTaskSetupPage(taskSetupMessagingTab)
                    ' Or if you have multiple pages
                    ' AddTaskSetupPages(New BlueByte.SOLIDWORKS.PDMProfessional.SDK.Core.ITaskPage() {taskSetupMessagingTab})

                Case EdmCmdType.EdmCmd_TaskSetupButton
                    ' Add a context menu
                    AddContextMenu($"Tasks\{Properties.TaskName} [V{Identity.Version}]", Identity.Description)

                    ' Save the data of the tab
                    taskSetupMessagingTab.StoreData(poCmd)

                Case EdmCmdType.EdmCmd_TaskDetails
                    ' Implement task details logic here

                Case EdmCmdType.EdmCmd_TaskRun
                    ' Implement task run logic here

                Case EdmCmdType.EdmCmd_TaskLaunch
                    ' Implement task launch logic here

                Case EdmCmdType.EdmCmd_TaskLaunchButton
                    ' Implement task launch button logic here

                Case Else
                    ' Default case logic

            End Select

        Catch e As CancellationException
            Throw

        Catch e As TaskFailedException
            Throw

        ' This is a PDM exception
        Catch e As COMException
            Throw

        Catch e As Exception
            Throw

        End Try

    End Sub
```
---

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
