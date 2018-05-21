Imports System.ComponentModel
Imports CmdInfrastructureNS
Public Class CommandSvcMonitor

    'Public Sub Update_CommandSvcCalls(ByVal _cmdPkg As CommandPackage)
    '    _CommandIssued = _cmdPkg

    '    Dim txt = CommandPackage.ToText(_cmdPkg)
    '    Dim kvp As New KeyValuePair(Of String, String)(DateTime.Now.ToLongTimeString, txt)


    '    _CommandSvcCalls.Add(kvp) 'CmdSvcClassLibrary.CommandPackage.ToText(_cmdPkg))
    '    My_OnPropertyChanged("CommandSvcCalls")
    'End Sub

    'Private _CommandSvcCalls As New List(Of KeyValuePair(Of String, String))

    'Public ReadOnly Property CommandSvcCalls() As List(Of KeyValuePair(Of String, String))
    '    Get
    '        Return _CommandSvcCalls
    '    End Get

    'End Property

    'Private _CommandIssued As CommandPackage

    'Public ReadOnly Property CommandIssued() As CommandPackage
    '    Get
    '        Return _CommandIssued
    '    End Get

    'End Property

    'Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) 'Implements INotifyPropertyChanged.PropertyChanged

    'Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
    '    RaiseEvent PropertyChanged(Nothing, New PropertyChangedEventArgs(propname))
    '    'If propname = "Configuration" Then
    '    '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
    '    'End If
    'End Sub


End Class
