Imports System.ComponentModel

Public Class ServiceMonitor
    'Implements INotifyPropertyChanged


    Public Sub Update_ServiceCalls(ByVal _string1 As String, ByVal _string2 As String)
        _ServiceCalls.Add(New KeyValuePair(Of String, String)(_string1, _string2))
        My_OnPropertyChanged("ServiceCalls")
    End Sub

    Private _ServiceCalls As New List(Of KeyValuePair(Of String, String))

    Public ReadOnly Property ServiceCalls() As List(Of KeyValuePair(Of String, String))
        Get
            Return _ServiceCalls
        End Get

    End Property

    Public Sub Update_ServiceInfoColxn(ByVal _SvcHostName As String, ByVal _EndPtColxn As ServiceModel.Description.ServiceEndpointCollection, _
                                              ByVal _state As ServiceModel.CommunicationState)
        Dim q = From si In _ServiceInfoColxn _
              Where si.ServiceName = _SvcHostName _
              Select si
        If q.Any Then
            For Each ept In _EndPtColxn
                q.First.EndPtColxn.Add(New KeyValuePair(Of DateTime, ServiceModel.Description.ServiceEndpoint) _
                                   (DateTime.Now, ept))
            Next
            Update_ServiceState(_SvcHostName, _state)
        Else
            Dim svcInfo As New ServiceInfo With {.ServiceName = _SvcHostName}
            svcInfo.EndPtColxn = (From ept In _EndPtColxn _
                                  Select New KeyValuePair(Of DateTime, ServiceModel.Description.ServiceEndpoint) _
                                   (DateTime.Now, ept)).ToList

            _ServiceInfoColxn.Add(svcInfo)
            Update_ServiceState(_SvcHostName, _state)
        End If
    End Sub

    Public Sub Update_ServiceState(ByVal _SvcHostName As String, ByVal _State As ServiceModel.CommunicationState)
        Dim q = From si In _ServiceInfoColxn _
                Where si.ServiceName = _SvcHostName _
                Select si.StateColxn
        If q.Any Then
            Dim kvp As New KeyValuePair(Of DateTime, ServiceModel.CommunicationState)(DateTime.Now, _State)
            q.First.Add(kvp)
        End If
        My_OnPropertyChanged("ServiceStateEvents")
    End Sub


    Private _ServiceInfoColxn As New List(Of ServiceInfo)
    Public ReadOnly Property ServiceInfoColxn() As List(Of ServiceInfo)
        Get
            Return _ServiceInfoColxn
        End Get

    End Property

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) 'Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Nothing, New PropertyChangedEventArgs(propname))
        'If propname = "Configuration" Then
        '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
        'End If
    End Sub


End Class
Public Class ServiceInfo
    Private _ServiceName As String
    Public Property ServiceName() As String
        Get
            Return _ServiceName
        End Get
        Set(ByVal value As String)
            _ServiceName = value
        End Set
    End Property
    Private _EndPtColxn As List(Of KeyValuePair(Of DateTime, ServiceModel.Description.ServiceEndpoint))
    Public Property EndPtColxn() As List(Of KeyValuePair(Of DateTime, ServiceModel.Description.ServiceEndpoint))
        Get
            Return _EndPtColxn
        End Get
        Set(ByVal value As List(Of KeyValuePair(Of DateTime, ServiceModel.Description.ServiceEndpoint)))
            _EndPtColxn = value
        End Set
    End Property


    Private _StateColxn As New List(Of KeyValuePair(Of DateTime, ServiceModel.CommunicationState))
    Public ReadOnly Property StateColxn() As List(Of KeyValuePair(Of DateTime, ServiceModel.CommunicationState))
        Get
            Return _StateColxn
        End Get

    End Property
End Class
