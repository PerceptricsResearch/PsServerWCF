Imports System.Runtime.Serialization

Public Class SharedEvents
#Region "Custom Events"

    Public Shared Event OperationFailed(ByVal sender As Object, ByVal _msg As String, ByVal e As EventArgs)
    Public Shared Sub RaiseOperationFailed(ByVal sender As Object, ByVal _MethodName As String)
        Try
            Using EvLog As New EventLog()
                EvLog.Source = "Perceptrics Server"
                EvLog.Log = "Application"
                EvLog.WriteEntry(sender.ToString & ", Method= " & _MethodName, EventLogEntryType.Warning)
            End Using
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "Perceptrics Server"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RaiseOperationFailed Could Not WriteEntry, Method= " & _MethodName & " " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

        'RaiseEvent OperationFailed(sender, _MethodName, New EventArgs)
    End Sub

    Public Shared Event SvcHostIsIdle(ByVal _timespan As TimeSpan, ByVal _dxnrykey As String, ByVal _suffix As String, ByVal e As EventArgs)
    Public Shared Sub RaiseSvcHostIsIdle(ByVal _timespan As TimeSpan, ByVal _dxnrykey As String, ByVal _suffix As String)
        'RaiseEvent SvcHostIsIdle(_timespan, _dxnrykey, _suffix, New EventArgs)
    End Sub
#End Region
End Class

#Region "SurveyClasses"
<DataContract()> _
Public Class Tiny_Survey_RowWith_SIModel
    Private _Model As String
    <DataMember()> _
    Public Property Model() As String
        Get
            Return _Model
        End Get
        Set(ByVal value As String)
            _Model = value
        End Set
    End Property

    Private _TinyRow As Tiny_Survey_Row
    <DataMember()> _
    Public Property TinyRow() As Tiny_Survey_Row
        Get
            Return _TinyRow
        End Get
        Set(ByVal value As Tiny_Survey_Row)
            _TinyRow = value
        End Set
    End Property
End Class

<DataContract()> _
Public Class Tiny_Survey_Row

    Private _SurveyID As Integer
    <DataMember()> _
    Public Property SurveyID() As Integer
        Get
            Return _SurveyID
        End Get
        Set(ByVal value As Integer)
            _SurveyID = value
        End Set
    End Property

    Private _ComputerID As Integer
    <DataMember()> _
    Public Property ComputerID() As Integer
        Get
            Return _ComputerID
        End Get
        Set(ByVal value As Integer)
            _ComputerID = value
        End Set
    End Property

    Private _SurveyDataStoreID As Integer
    <DataMember()> _
    Public Property SurveyDataStoreID() As Integer
        Get
            Return _SurveyDataStoreID
        End Get
        Set(ByVal value As Integer)
            _SurveyDataStoreID = value
        End Set
    End Property

    Private _SurveyName As String
    <DataMember()> _
    Public Property SurveyName() As String
        Get
            Return _SurveyName
        End Get
        Set(ByVal value As String)
            _SurveyName = value
        End Set
    End Property

    Private _QueueName As String
    <DataMember()> _
    Public Property QueueName() As String
        Get
            Return _QueueName
        End Get
        Set(ByVal value As String)
            _QueueName = value
        End Set
    End Property

    Private _QueueUri As String
    <DataMember()> _
    Public Property QueueUri() As String
        Get
            Return _QueueUri
        End Get
        Set(ByVal value As String)
            _QueueUri = value
        End Set
    End Property

    Private _PrivilegeBitMask As ULong
    <DataMember()> _
    Public Property PrivilegeBitMask() As ULong
        Get
            Return _PrivilegeBitMask
        End Get
        Set(ByVal value As ULong)
            _PrivilegeBitMask = value
        End Set
    End Property

    Private _SurveyStateID As Integer
    <DataMember()> _
    Public Property SurveyStateID() As Integer
        Get
            Return _SurveyStateID
        End Get
        Set(ByVal value As Integer)
            _SurveyStateID = value
        End Set
    End Property

    Private _SurveyType As Integer
    <DataMember()> _
    Public Property SurveyType() As Integer
        Get
            Return _SurveyType
        End Get
        Set(ByVal value As Integer)
            _SurveyType = value
        End Set
    End Property
End Class

<DataContract()> _
Public Class SurveyMetadataPackage
    Private _MetaDataColxn As List(Of CmdInfrastructureNS.Srlzd_KVP)
    <DataMember()> _
    Public Property MetaDataColxn() As List(Of CmdInfrastructureNS.Srlzd_KVP)
        Get
            Return _MetaDataColxn
        End Get
        Set(ByVal value As List(Of CmdInfrastructureNS.Srlzd_KVP))
            _MetaDataColxn = value
        End Set
    End Property
    Private _TinyRow As Tiny_Survey_Row
    <DataMember()> _
    Public Property TinyRow() As Tiny_Survey_Row
        Get
            Return _TinyRow
        End Get
        Set(ByVal value As Tiny_Survey_Row)
            _TinyRow = value
        End Set
    End Property
    Private _Model As SurveyImagesPackage
    <DataMember()> _
    Public Property Model() As SurveyImagesPackage
        Get
            Return _Model
        End Get
        Set(ByVal value As SurveyImagesPackage)
            _Model = value
        End Set
    End Property
End Class

<DataContract()> _
Public Class SurveyEventsPackage
    Private _EventsColxn As List(Of EventItem)
    <DataMember()> _
    Public Property EventsColxn() As List(Of EventItem)
        Get
            Return _EventsColxn
        End Get
        Set(ByVal value As List(Of EventItem))
            _EventsColxn = value
        End Set
    End Property
    Private _TinyRow As Tiny_Survey_Row
    <DataMember()> _
    Public Property TinyRow() As Tiny_Survey_Row
        Get
            Return _TinyRow
        End Get
        Set(ByVal value As Tiny_Survey_Row)
            _TinyRow = value
        End Set
    End Property
End Class

<DataContract()> _
Public Class EventItem 'have to do the rest of these properties when I figure out how it will be used...corresponds to EventTable SurveMaster
    Private _EventItemID As Integer
    <DataMember()> _
    Public Property EventItemID() As Integer
        Get
            Return _EventItemID
        End Get
        Set(ByVal value As Integer)
            _EventItemID = value
        End Set
    End Property
End Class
#End Region