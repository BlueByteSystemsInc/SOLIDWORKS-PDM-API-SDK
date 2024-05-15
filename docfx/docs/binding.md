# Background 

EdmTaskPage inherits from the WinForms UserControl class.

The idea behind using the MVVM architecture is to minimize the amount of codebehind in EdmTaskPages.

The EdmTaskPage class contains a property called ViewModel. When you create a EdmTaskPage, you are required to specify the ViewModel's type as part of the generic class definition of the page.

# [C Sharp](#tab/cs)
```
public class myViewModelType : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    public string FileName {get;set;}
}

public  partial class myEdmTaskPage : EdmTaskPage<myViewModelType>
{

        // this line to demo the logic - add controls through the usercontrol designer
        private System.Windows.Forms.TextBox FileNameTextBox;

         public myEdmTaskPage()
        {
            this.Name = "myEdmTaskPage";

            InitializeComponent();
        }
}
```
# [VB](#tab/VB)
```
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

Public Class myViewModelType
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = "")
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private _fileName As String
    Public Property FileName As String
        Get
            Return _fileName
        End Get
        Set(value As String)
            If _fileName <> value Then
                _fileName = value
                OnPropertyChanged()
            End If
        End Set
    End Property
End Class

Public Partial Class myEdmTaskPage
    Inherits EdmTaskPage(Of myViewModelType)

    ' This line to demo the logic - add controls through the user control designer
    Private FileNameTextBox As TextBox

    Public Sub New()
        Me.Name = "myEdmTaskPage"
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.FileNameTextBox = New TextBox()
        ' Additional initialization logic can be added here
    End Sub
End Class
```
---


>[!NOTE]
>Your view model type must implemented the .NET Component Model interface INotifyPropertyChanged. 


# How binding works?

In the view model, override the method OnDataLoaded. You can provide the binding between the control and the view model property. Example:

# [C Sharp](#tab/cs)
```
  public override void OnDataLoaded()
        {
            base.OnDataLoaded();

            FileNameTextBox.DataBindings.Add(new Binding("Text", ViewModel, nameof(ViewModel.Enabled), false, DataSourceUpdateMode.OnPropertyChanged));
        }
```
# [VB](#tab/VB)
```
Public Overrides Sub OnDataLoaded()
    MyBase.OnDataLoaded()

    FileNameTextBox.DataBindings.Add(New Binding("Text", ViewModel, NameOf(ViewModel.Enabled), False, DataSourceUpdateMode.OnPropertyChanged))
End Sub
```
---

All changes in the UI will propagate to the view model and no more code-behind is needed. 


# ViewModel requirements

- Do not use internal properties.
- Do not use private setters.
- Always make sure the view model has a parameterless constructor.



>[!NOTE]
>These requirements are necessary because EdmTaskPage uses Newtonsoft.Json to load and save the view model.


# How do I save the view model?

Call the method [StoreData](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.Core.TaskPage-1.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_Core_TaskPage_1_StoreData_EPDM_Interop_epdm_EdmCmd__) during the TaskSetupButton hook. See full code example in the TaskSetup section.




