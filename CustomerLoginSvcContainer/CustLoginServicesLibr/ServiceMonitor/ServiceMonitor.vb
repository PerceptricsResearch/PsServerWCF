Imports System.ComponentModel

Public Class ServiceMonitor
    'Implements INotifyPropertyChanged


    Public Shared Sub Update_ServiceCalls(ByVal _string1 As String, ByVal _string2 As String)

        _ServiceCalls.Add(New KeyValuePair(Of String, String)(_string1, _string2))
        ServiceMonitor.My_OnPropertyChanged("ServiceCalls")
    End Sub

    Private Shared _ServiceCalls As New List(Of KeyValuePair(Of String, String))

    Public Shared ReadOnly Property ServiceCalls() As List(Of KeyValuePair(Of String, String))
        Get
            Return _ServiceCalls.ToList
        End Get
        'Set(ByVal value As Dictionary(Of String, String))
        '    _ServiceCalls = value
        'End Set
    End Property

    Public Shared Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) 'Implements INotifyPropertyChanged.PropertyChanged

    Protected Shared Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Nothing, New PropertyChangedEventArgs(propname))
        'If propname = "Configuration" Then
        '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
        'End If
    End Sub


End Class
