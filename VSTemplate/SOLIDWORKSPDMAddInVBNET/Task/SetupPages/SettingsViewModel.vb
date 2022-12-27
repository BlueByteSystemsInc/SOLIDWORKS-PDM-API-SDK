Imports System
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Namespace SOLIDWORKSPDMAddInVBNET.Task.SetupPages
    Public Class SettingsViewModel
        Implements INotifyPropertyChanged

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Private Sub NotifyPropertyChanged(
<CallerMemberName> ByVal Optional propertyName As String = "")
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
End Namespace
