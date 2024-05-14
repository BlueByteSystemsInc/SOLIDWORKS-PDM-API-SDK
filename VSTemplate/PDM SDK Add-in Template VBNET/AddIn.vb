Imports System
Imports System.IO
Imports System.Runtime.InteropServices
Imports BlueByte.SOLIDWORKS.PDMProfessional.SDK
Imports BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes
Imports BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics
Imports EPDM.Interop.epdm
Imports SimpleInjector

Namespace $safeprojectname$
    Public Enum Commands
        CommandOne = 1234
    End Enum

    <Menu(CInt(Commands.CommandOne), "Click Me!")>
    <ListenFor(EdmCmdType.EdmCmd_Menu)>
    <Name("$addinname$")>
    <Description("$addindescription$")>
    <CompanyName("$addincompanyname$")>
    <AddInVersion(False, 1)>
    <IsTask($addintask$)>
    <RequiredVersion(Year_e.PDM2014, ServicePack_e.SP0)>
    <ComVisible(True)>
    <Guid("$guid1$")>
    Partial Public Class AddIn
        Inherits AddInBase

        Public Overrides Sub OnCmd(ByRef poCmd As EdmCmd, ByRef ppoData As EdmCmdData())
            MyBase.OnCmd(poCmd, ppoData)
           

           If poCmd.mlCmdID <> CInt(Commands.CommandOne) Then Return
           
        Try
            ForEachFile(ppoData, Function(ByVal file As IEdmFile5)
                                    MyBase.Vault.MsgBox(handle, $"You clicked on {file.Name}", EdmMBoxType.EdmMbt_OKOnly, Identity.ToCaption())
                                End Function)
        Catch e As CancellationException
            Throw
        Catch e As TaskFailedException
            Throw
        Catch e As COMException
            Throw
        Catch e As Exception
            Throw
        End Try


        End Sub

        Protected Overrides Sub OnLoggerTypeChosen(ByVal defaultType As LoggerType_e)
            MyBase.OnLoggerTypeChosen(LoggerType_e.File)
        End Sub

        Protected Overrides Sub OnRegisterAdditionalTypes(ByVal container As Container)
        End Sub

        Protected Overrides Sub OnLoggerOutputSat(ByVal defaultDirectory As String)
        End Sub

        Protected Overrides Sub OnLoadAdditionalAssemblies(ByVal addinDirectory As DirectoryInfo)
            MyBase.OnLoadAdditionalAssemblies(addinDirectory)
        End Sub

        Protected Overrides Sub OnUnhandledExceptions(ByVal catchAllExceptions As Boolean, ByVal Optional logAction As Action(Of Exception) = Nothing)
            Me.CatchAllUnhandledException = False
            logAction = Sub(ByVal e As Exception)
                            Throw New NotImplementedException()
                        End Sub

            MyBase.OnUnhandledExceptions(catchAllExceptions, logAction)
        End Sub
    End Class
End Namespace
