Imports System
Imports System.IO
Imports System.Runtime.InteropServices
Imports BlueByte.SOLIDWORKS.PDMProfessional.SDK
Imports BlueByte.SOLIDWORKS.PDMProfessional.SDK.Attributes
Imports BlueByte.SOLIDWORKS.PDMProfessional.SDK.Diagnostics
Imports EPDM.Interop.epdm
Imports SimpleInjector

Namespace SOLIDWORKSPDMAddInVBNET
    Public Enum Commands
        CommandOne = 1000
    End Enum

    <Menu(CInt(Commands.CommandOne), "This is a menu item created by the SDK")>
    <Name("Sample Addin")>
    <Description("This is a description")>
    <CompanyName("Blue Byte Systems Inc")>
    <AddInVersion(False, 1)>
    <IsTask(False)>
    <RequiredVersion(10, 0)>
    <ComVisible(True)>
    <Guid("721C78F4-362E-45EC-B313-CD0E33A87D67")>
    Partial Public Class AddIn
        Inherits AddInBase

        Public Overrides Sub OnCmd(ByRef poCmd As EdmCmd, ByRef ppoData As EdmCmdData())
            MyBase.OnCmd(poCmd, ppoData)
            Throw New NotImplementedException("This addin has no implementation")
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
