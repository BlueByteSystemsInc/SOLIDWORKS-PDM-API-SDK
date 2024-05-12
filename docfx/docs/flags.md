# Task features

A task can be: 
- Executed from a workflow transition as an action or a change command.
- Scheduled to execute via a command. The task is invoked in File Explorer through the file's context menu.
- Initialized from the Administration tool. 

A task can also contain a details page. The details page is accessible from the task list window.

<img src="../images/details.png"/>


All of these features are controlled by the [EdmTaskFlag](https://help.solidworks.com/2021/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.EdmTaskFlag.html) enum.


With PDMSDK, you can use the [TaskFlags](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes.TaskFlagsAttribute.html) attribute to configure your add-in like below: 

```
    [TaskFlags((int)EdmTaskFlag.EdmTask_SupportsChangeState + (int)EdmTaskFlag.EdmTask_SupportsDetails + (int)EdmTaskFlag.EdmTask_SupportsInitExec +(int)EdmTaskFlag.EdmTask_SupportsScheduling)]
    
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

 