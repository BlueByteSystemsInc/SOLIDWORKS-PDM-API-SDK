
# Implementing OnCmd

> [!TIP] 
> You **do not** need to be provide an implementation or override the GetAddInInfo method. PDMSDK takes care of setting the add-in information through the definition attributes discussed previously.

The OnCmd gets called by PDM whenever a menu command or a hooked event is triggered. There are two arguments passed into this method:


1. `EdmCmd poCmd`

   This object contains information about the event that triggered this call as well as attributes for the vault and vault view that triggered the event.

> [!TIP]
> For complete information about `EdmCmd`, refer to this [page](https://help.solidworks.com/2023/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.EdmCmd.html)

2. `EdmCmdData[] ppoData`

   This object contains data about every object (file, folder, etc.) that this command will potentially affect.

   Common attributes that are passed through the `EdmCmdData` structure are:

   - File path
   - Id of affected file
   - Id of affected folder, or parent folder of file
   - Additional command specific information may be passed into this structure.

> [!TIP]
> For complete information about `EdmCmdData` structure for all command types, refer to this [page](https://help.solidworks.com/2023/english/api/epdmapi/EPDM.Interop.epdm~EPDM.Interop.epdm.EdmCmdData.html)

# Working with EdmCmdData

A common structure for to utilize `OnCmd` and its data will be to perform actions based on the command type using a switch statement. The command type is retrieved from `poCmd.meCmdType`

A separate switch case can be added for each command type in order to keep the logic separate.

> [!NOTE]
> To perform actions on a specific command type, that command must first be hooked using the `[AddListener()]` attribute. See this help [page](./addinbase_structure.md) for more information.

```
public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
{
    base.OnCmd(ref poCmd, ref ppoData);

    switch(poCmd.meCmdType)
    {
        case EdmCmdType.EdmCmd_PreUnlock:
            foreach(var ppoDatum in ppoData)
            {
                // Do something to each file before it is checked-in
            }
            break;
    }
}
```

> [!TIP]
> To access the vault object, utilize `this.Vault`. If the vault object needs to implement additional features from a specific vault interface, it can be cast in this manner.
>
> `var edmVault21 = this.Vault as IEdmVault21`

# Accessing Files and Folders

Files and folders can be accessed by utilizing the IDs passed into the `ppoData[]` structure. The example below obtains the file object from the vault and can perform actions or checks on it prior to it being checked in.

```
case EdmCmdType.EdmCmd_PreUnlock:
    foreach (var ppoDatum in ppoData)
    {
        var filePath = ppoDatum.mbsStrData1;
        var fileId = ppoDatum.mlObjectID1;
        var folderId = ppoDatum.mlObjectID2;

        var edmFile = Vault.TryGetFileFromPath(filePath, out var edmFolder);

        // Do something with the file in question
    }
    break;
```

# Canceling Actions

If the OnCmd function needs to abort the current action, such as check-in or state change, the `poCmd.mbCancel` flag can be set by the user. Once the add-in finishes with the OnCmd method, it will allow or cancel the current action based on the value of this command.

Example to cancel check-in:

```
case EdmCmdType.EdmCmd_PreUnlock:
    foreach (var ppoDatum in ppoData)
    {
        var filePath = ppoDatum.mbsStrData1;
        var fileId = ppoDatum.mlObjectID1;
        var folderId = ppoDatum.mlObjectID2;

        var edmFile = Vault.TryGetFileFromPath(filePath, out var edmFolder);

        if (edmFile.CurrentRevision == "") // if the revision is not blank, cancel the check-in
        {
            poCmd.mbCancel = Convert.ToInt16(true); // Sets to integer value of 1
            break;
        }
    }
    break;
```

> [!NOTE]
> This cancellation applies to all files in the current command. It is not possible to allow some files to proceed while halting the action on others. 

> [!IMPORTANT]
> Tasks support suspension and cancellation during the EdmTaskRun hook. PDMSDK provides a specific way to suspend and cancel tasks. Consult the task section for more information.


# Exception handling

We highly recommend that you wrap your OnCmd implementation in a try catch block. Please catch the following exceptions when working with PDMSDK in this order:

- [TaskFailedException](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.TaskFailedException.html#constructors): Thrown when a task fails.
- [CancellationException](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.CancellationException.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_CancellationException__ctor): Thrown when a task is canceled by the user.
- COMException: Thrown when PDM (or another third-party application such as SOLIDWORKS ) throws an exception.
- Exception: To catch any .NET exception.


```
public override void OnCmd(ref EdmCmd poCmd, ref EdmCmdData[] ppoData)
{
    base.OnCmd(ref poCmd, ref ppoData);

    try 
    {
        
     switch(poCmd.meCmdType)
     {
        case EdmCmdType.EdmCmd_PreUnlock:
            foreach(var ppoDatum in ppoData)
            {
                // Do something to each file before it is checked-in
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
```