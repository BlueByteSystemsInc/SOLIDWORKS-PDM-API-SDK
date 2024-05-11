# Logger

PDMSDK has a logger property that implements the ILogger interface called [Logger](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_Logger). It allows for logging capabilites in your add-in. 

There are three built-in types of logger:

- File logger: This logs output to a text file. You must define the output location.
- SQL logger: This logs output to a table in a database. This logger requires a connection string and a table name.
- Console logger: If your add-in dll is called from a console application, you can use this logger to print output directly to the console. This logger does not require any initialization and is very useful in unit and integration tests. 

>[!IMPORTANT]
> When working with the TaskDetails hook, some developers prefer to save the output of their logs in the task instance object. To access the log, the user needs to be have the proper task permissions. We highly recommend you use the [Logger](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_Logger) and save your logs to the vault file system instead.


# Configuration

You can define the type of the logger by overriding the method [OnLoggerTypeChosen](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_OnLoggerTypeChosen_BlueByte_SOLIDWORKS_PDMProfessional_SDK_Diagnostics_LoggerType_e_) and setting the logger type.

# [C Sharp](#tab/cs)
```
 protected override void OnLoggerTypeChosen(LoggerType_e defaultType)
        {
            base.OnLoggerTypeChosen(LoggerType_e.File);
        }
```
# [VB](#tab/VB)
```
To be provided. 
// if you see this, please contribute to our repo by providing a translation of C# to VB.NET
```
---

>[!NOTE]
> You can implement your own logger by implementing the ILogger interface and register it using the [Container](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_Container) in the [RegisterAdditionalTypes](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_RegisterAdditionalTypes) method. In this case, you can ignore the [OnLoggerTypeChosen](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_OnLoggerTypeChosen) method.

# File Logger

To use the file logger:

- Set the output location via the [OutputLocation](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics.ILogger.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_Diagnostics_ILogger_OutputLocation) property.
- Call [LogToOutput](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics.ILogger.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_Diagnostics_ILogger_LogToOutput) to log to output. Your target must be the name of your log file with its extension.

# SQL logger

With the SQL logger, you must:
-  Call the [Init](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics.ILogger.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_Diagnostics_ILogger_Init_BlueByte_SOLIDWORKS_PDMProfessional_SDK_Identity_EPDM_Interop_epdm_IEdmTaskInstance_System_String_) method in the implementation of the OnCmd method.
- Call [StartConnection](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics.ILogger.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_Diagnostics_ILogger_StartConnection) to start your connection with your SQL server.
- Call [LogToOutput](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics.ILogger.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_Diagnostics_ILogger_LogToOutput) to log to output with target being the name of the table in your SQL table.
- Call [EndConnection](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics.ILogger.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_Diagnostics_ILogger_EndConnection) to end your connection with your SQL server.
