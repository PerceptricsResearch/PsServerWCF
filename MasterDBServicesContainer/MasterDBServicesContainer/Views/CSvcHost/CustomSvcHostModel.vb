Imports System.ComponentModel

Public Class CustomSvcHostModel
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
        'If propname = "Configuration" Then
        '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
        'End If
    End Sub

    Private WithEvents _MyCustomSvcHost As MasterDBSvcLibr.CustomSvcHost
    Public Property MyCustomSvcHost() As MasterDBSvcLibr.CustomSvcHost
        Get
            Return _MyCustomSvcHost
        End Get
        Set(ByVal value As MasterDBSvcLibr.CustomSvcHost)
            _MyCustomSvcHost = value
            My_OnPropertyChanged("MyCustomSvcHost")

        End Set
    End Property

    Private Sub MyCustSvcHostPropChanged() Handles _MyCustomSvcHost.PropertyChanged
        My_OnPropertyChanged("MyCustomSvcHost")
    End Sub
End Class
