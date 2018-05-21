Imports System.ComponentModel
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports CmdInfrastructureNS
Imports System.Data.SqlClient
Imports System.Collections.ObjectModel
Imports DataContextPackageNS

Public Class CustomSvcHost
    Inherits ServiceHost
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
        'If propname = "Configuration" Then
        '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
        'End If
    End Sub

    Public Sub New(ByVal servicetype As Type)
        MyBase.New(servicetype)

    End Sub
    Public Sub New(ByVal _serviceType As Type, ByVal _baseaddress As Uri)
        MyBase.New(_serviceType, _baseaddress)

    End Sub

    Public Property RDentCountTTLTime As New TimeSpan(0, 0, 29)
    Public Property RDCountTTLExpiresTime As Date = Now.Add(Me.RDentCountTTLTime)
    Private _RDentCountInLastTTLTime As Integer = 0
    Public Property RDentCountInLastTTLTime As Integer
        Get
            If Date.Now.CompareTo(Me.RDCountTTLExpiresTime) < 1 Then
                Return _RDentCountInLastTTLTime
            Else
                Return 0
            End If
        End Get
        Set(value As Integer)
            _RDentCountInLastTTLTime = value
        End Set
    End Property

#Region "Idle Timer Stuff"
    Public LastOperationDateTime As Date = DateTime.UtcNow
    Public IdleTimer As System.Windows.Threading.DispatcherTimer = Nothing

    Private Sub SetUpIdleTimer() 'Handles Me.Opened 'is not implemented....

        If Me.DxnryAddress IsNot Nothing Then
            Me.IdleTimer = New System.Windows.Threading.DispatcherTimer(Windows.Threading.DispatcherPriority.Background)
            Me.IdleTimer.Interval = New TimeSpan(0, 2, 0)

            AddHandler Me.IdleTimer.Tick, AddressOf TellWCFSvcManagerMyIamIdle
            Me.IdleTimer.Start()
        End If

    End Sub
    Private Sub TellWCFSvcManagerMyIamIdle(ByVal sender As Object, ByVal e As EventArgs)
        Dim elapsed = DateTime.UtcNow.Subtract(Me.LastOperationDateTime)
        If elapsed.Minutes >= 2 Or elapsed.Hours > 1 Then
            SharedEvents.RaiseSvcHostIsIdle(elapsed, Me.DxnryAddress, Me.EndPtSuffix)
        End If

    End Sub
    Private Sub RemoveIdleTimerHandler() Handles Me.Closed
        If Me.IdleTimer IsNot Nothing Then
            Me.IdleTimer.Stop()
            RemoveHandler Me.IdleTimer.Tick, AddressOf TellWCFSvcManagerMyIamIdle
            Me.IdleTimer.IsEnabled = False
            Me.IdleTimer = Nothing
            Me.DC_Pkg = Nothing

            'Me.CmdSvcMonitor = Nothing
            Me.SvcMonitor = Nothing
            MyBase.ReleasePerformanceCounters()
            MyBase.Finalize()

        End If
    End Sub


    Private _DxnryAddress As String
    Public Property DxnryAddress() As String
        Get
            Return _DxnryAddress
        End Get
        Set(ByVal value As String)
            _DxnryAddress = value
        End Set
    End Property
#End Region


    Public ReadOnly Property GSM_LoginID() As Integer
        Get
            Return Me.DC_Pkg.GSM_LoginID
        End Get
    End Property


    '''' <summary>
    '''' The Svc in this Host is a Client of all the Services described in this Colxn...
    '''' Use this Colxn to find the BaseAddress of a Service, for which, you want to create a ClientProxy
    '''' It is populated by EndPtDataContextSvc in its IssueExposeEndPtsCommands operation.
    '''' This colxn is accessible by WCFServicID, Name, Contract....
    '''' </summary>
    '''' <remarks></remarks>
    'Private _I_am_a_ClientOfThisServiceColxn As New List(Of ClientOFThisServiceInfo)
    'Public Property ClientOfThisServiceColxn() As List(Of ClientOFThisServiceInfo)
    '    Get
    '        Return _I_am_a_ClientOfThisServiceColxn
    '    End Get
    '    Set(ByVal value As List(Of ClientOFThisServiceInfo))
    '        _I_am_a_ClientOfThisServiceColxn = value
    '    End Set
    'End Property


    Private _SvcMonitor As ServiceMonitor
    Public Property SvcMonitor() As ServiceMonitor
        Get
            Return _SvcMonitor
        End Get
        Set(ByVal value As ServiceMonitor)
            _SvcMonitor = value
        End Set
    End Property

    'Private _CmdSvcMonitor As CommandSvcMonitor
    'Public Property CmdSvcMonitor() As CommandSvcMonitor
    '    Get
    '        Return _CmdSvcMonitor
    '    End Get
    '    Set(ByVal value As CommandSvcMonitor)
    '        _CmdSvcMonitor = value
    '    End Set
    'End Property

    'Public DC_Cnxn_IsValid As Boolean = False
    'Public Function DC_Cnxn_For_SurveyID(ByVal _SurveyID As Integer) As String 'NEED TO DEAL WITH ADD NEW CASE...
    '    Dim rslt As String = ""
    '    Me.DC_Cnxn_IsValid = False
    '    Try
    '        Dim q = From kvp In Me._DC_Pkg.Survey_DC_List _
    '           Where kvp.Key = _SurveyID _
    '           Select kvp
    '        q.DefaultIfEmpty(Nothing)
    '        If q.Any Then
    '            rslt = q.First.Valu 'CnxnString_AbsolutePath(rslt)
    '        End If
    '        'Me.DC_Cnxn_IsValid = True
    '    Catch ex As Exception
    '        'Me.DC_Cnxn_IsValid = False
    '        rslt = "IsNotSet"
    '    End Try
    '    Return rslt
    'End Function

    Public Function CnxnString_AbsolutePath(ByVal _AbsolutePath As String) As String
        Dim rslt As String = "NotSet"
        Dim builder As New SqlConnectionStringBuilder
        builder.AttachDBFilename = _AbsolutePath
        ' builder.InitialCatalog = _AbsolutePath
        builder.IntegratedSecurity = True
        builder.DataSource = ".\DEVRENTS"
        builder.UserInstance = False
        builder.LoadBalanceTimeout = 30
        rslt = builder.ConnectionString
        builder = Nothing
        Return rslt
    End Function

    Private _Cache_Pkg As Cache_Package
    Public Property Cache_Pkg() As Cache_Package
        Get
            Return _Cache_Pkg
        End Get
        Set(ByVal value As Cache_Package)
            _Cache_Pkg = value
            My_OnPropertyChanged("Cache_Pkg")
        End Set
    End Property

    Private _CreatedDateTime As DateTime = DateTime.Now
    Public ReadOnly Property CreatedDateTime() As Date
        Get
            Return _CreatedDateTime
        End Get
    End Property

    Private _FaultsColxn As New List(Of DateTime)
    Public Property FaultsColxn() As List(Of DateTime)
        Get
            Return _FaultsColxn
        End Get
        Set(ByVal value As List(Of DateTime))
            _FaultsColxn = value
        End Set

    End Property

    Private _DC_Pkg As DC_Package
    Public Property DC_Pkg() As DC_Package
        Get
            LastOperationDateTime = DateTime.UtcNow
            Return _DC_Pkg
        End Get
        Set(ByVal value As DC_Package)
            _DC_Pkg = value
            My_OnPropertyChanged("DC_Pkg")
        End Set
    End Property

    Private _EndPtSuffix As String = ""
    Public Property EndPtSuffix() As String
        Get
            Return _EndPtSuffix
        End Get
        Set(ByVal value As String)
            _EndPtSuffix = value
        End Set
    End Property

    Private _InstanceCount As Integer = 0
    Public Property InstanceCount() As Integer
        Get
            Return _InstanceCount
        End Get
        Set(ByVal value As Integer)
            _InstanceCount = value
            My_OnPropertyChanged("InstanceCount")
        End Set
    End Property

    Private _LastIntanceCreatedDateTime As Date
    Public Property LastInanceCreatedDateTime() As Date
        Get
            Return _LastIntanceCreatedDateTime
        End Get
        Set(ByVal value As Date)
            _LastIntanceCreatedDateTime = value
            My_OnPropertyChanged("LastInanceCreatedDateTime")
        End Set
    End Property

    Private _DataContextConnectionString As String
    Public Property DataContextConnectionString() As String
        Get
            LastOperationDateTime = DateTime.UtcNow
            Return _DataContextConnectionString
        End Get
        Set(ByVal value As String)
            _DataContextConnectionString = value
            My_OnPropertyChanged("DataContextConnectionString")
        End Set
    End Property

    Private _LastSurveyID As Integer
    Public Property LastSurveyID() As Integer
        Get
            Return _LastSurveyID
        End Get
        Set(ByVal value As Integer)
            _LastSurveyID = value
            My_OnPropertyChanged("LastSurveyID")
        End Set
    End Property

    Private _LastOperationKVP As KeyValuePair(Of String, String)
    Public Property LastOperationKVP() As KeyValuePair(Of String, String)
        Get
            Return _LastOperationKVP
        End Get
        Set(ByVal value As KeyValuePair(Of String, String))
            _LastOperationKVP = value
            My_OnPropertyChanged("LastOperationKVP")
        End Set
    End Property




End Class


'Public Class ServiceEptContext
'    Implements INotifyPropertyChanged
'    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

'    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
'        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
'        'If propname = "Configuration" Then
'        '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
'        'End If
'    End Sub


'    ''' <summary>
'    ''' The Svc in this Host is a Client of all the Services described in this Colxn...
'    ''' Use this Colxn to find the BaseAddress of a Service, for which, you want to create a ClientProxy
'    ''' It is populated by EndPtDataContextSvc in its IssueExposeEndPtsCommands operation.
'    ''' This colxn is accessible by WCFServicID, Name, Contract....
'    ''' </summary>
'    ''' <remarks></remarks>
'    Private _I_am_a_ClientOfThisServiceColxn As New List(Of ClientOFThisServiceInfo)
'    Public Property ClientOfThisServiceColxn() As List(Of ClientOFThisServiceInfo)
'        Get
'            Return _I_am_a_ClientOfThisServiceColxn
'        End Get
'        Set(ByVal value As List(Of ClientOFThisServiceInfo))
'            _I_am_a_ClientOfThisServiceColxn = value
'        End Set
'    End Property


'    Private _SvcMonitor As ServiceMonitor
'    Public Property SvcMonitor() As ServiceMonitor
'        Get
'            Return _SvcMonitor
'        End Get
'        Set(ByVal value As ServiceMonitor)
'            _SvcMonitor = value
'        End Set
'    End Property

'    Private _CmdSvcMonitor As CommandSvcMonitor
'    Public Property CmdSvcMonitor() As CommandSvcMonitor
'        Get
'            Return _CmdSvcMonitor
'        End Get
'        Set(ByVal value As CommandSvcMonitor)
'            _CmdSvcMonitor = value
'        End Set
'    End Property

'    Public DC_Cnxn_IsValid As Boolean = False
'    Public Function DC_Cnxn_For_SurveyID(ByVal _SurveyID As Integer) As String 'NEED TO DEAL WITH ADD NEW CASE...
'        Dim rslt As String = ""
'        Me.DC_Cnxn_IsValid = False
'        Try
'            Dim q = From kvp In Me._DC_Pkg.Survey_DC_List _
'               Where kvp.Key = _SurveyID _
'               Select kvp
'            q.DefaultIfEmpty(Nothing)
'            rslt = q.First.Valu
'            Me.DataContextConnectionString = Me.CnxnString_AbsolutePath(rslt)
'            Me.DC_Cnxn_IsValid = True
'        Catch ex As Exception
'            Me.DC_Cnxn_IsValid = False
'            Me.DataContextConnectionString = "IsNotSet"
'        End Try
'        Return rslt
'    End Function
'    Public Function CnxnString_AbsolutePath(ByVal _AbsolutePath As String) As String
'        Dim rslt As String = "NotSet"
'        Dim builder As New SqlConnectionStringBuilder
'        builder.AttachDBFilename = _AbsolutePath
'        ' builder.InitialCatalog = _AbsolutePath
'        builder.IntegratedSecurity = True
'        builder.DataSource = ".\DEVRENTS"
'        builder.UserInstance = False
'        builder.LoadBalanceTimeout = 30
'        rslt = builder.ConnectionString
'        builder = Nothing
'        Return rslt
'    End Function

'    Private _Cache_Pkg As Cache_Package
'    Public Property Cache_Pkg() As Cache_Package
'        Get
'            Return _Cache_Pkg
'        End Get
'        Set(ByVal value As Cache_Package)
'            _Cache_Pkg = value
'            My_OnPropertyChanged("Cache_Pkg")
'        End Set
'    End Property

'    Private _CreatedDateTime As DateTime = DateTime.Now
'    Public ReadOnly Property CreatedDateTime()
'        Get
'            Return _CreatedDateTime
'        End Get
'    End Property

'    Private _FaultsColxn As New List(Of DateTime)
'    Public Property FaultsColxn() As List(Of DateTime)
'        Get
'            Return _FaultsColxn
'        End Get
'        Set(ByVal value As List(Of DateTime))
'            _FaultsColxn = value
'        End Set

'    End Property

'    Private _DC_Pkg As DC_Package
'    Public Property DC_Pkg() As DC_Package
'        Get
'            Return _DC_Pkg
'        End Get
'        Set(ByVal value As DC_Package)
'            _DC_Pkg = value
'            My_OnPropertyChanged("DC_Pkg")
'        End Set
'    End Property

'    Private _EndPtSuffix As String = ""
'    Public Property EndPtSuffix() As String
'        Get
'            Return _EndPtSuffix
'        End Get
'        Set(ByVal value As String)
'            _EndPtSuffix = value
'        End Set
'    End Property

'    Private _InstanceCount As Integer = 0
'    Public Property InstanceCount() As Integer
'        Get
'            Return _InstanceCount
'        End Get
'        Set(ByVal value As Integer)
'            _InstanceCount = value
'            My_OnPropertyChanged("InstanceCount")
'        End Set
'    End Property

'    Private _LastIntanceCreatedDateTime As Date
'    Public Property LastInanceCreatedDateTime() As Date
'        Get
'            Return _LastIntanceCreatedDateTime
'        End Get
'        Set(ByVal value As Date)
'            _LastIntanceCreatedDateTime = value
'            My_OnPropertyChanged("LastInanceCreatedDateTime")
'        End Set
'    End Property

'    Private _DataContextConnectionString As String
'    Public Property DataContextConnectionString() As String
'        Get
'            Return _DataContextConnectionString
'        End Get
'        Set(ByVal value As String)
'            _DataContextConnectionString = value
'            My_OnPropertyChanged("DataContextConnectionString")
'        End Set
'    End Property

'    Private _LastSurveyID As Integer
'    Public Property LastSurveyID() As Integer
'        Get
'            Return _LastSurveyID
'        End Get
'        Set(ByVal value As Integer)
'            _LastSurveyID = value
'            My_OnPropertyChanged("LastSurveyID")
'        End Set
'    End Property

'    Private _LastOperationKVP As KeyValuePair(Of String, String)
'    Public Property LastOperationKVP() As KeyValuePair(Of String, String)
'        Get
'            Return _LastOperationKVP
'        End Get
'        Set(ByVal value As KeyValuePair(Of String, String))
'            _LastOperationKVP = value
'            My_OnPropertyChanged("LastOperationKVP")
'        End Set
'    End Property
'End Class

Public Class Cache_Package
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
        'If propname = "Configuration" Then
        '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
        'End If
    End Sub

    Public Sub New(ByVal _CacheObj As Object, ByVal _SurveyID As Integer, ByVal _LoginID As Integer)
        Me.Cache = _CacheObj
        Me._SurveyID = _SurveyID
        Me.LoginID = _LoginID
        Me._CacheDateTime = DateTime.Now
    End Sub

    Private _Cache As Object = "Empty"
    Public Property Cache() As Object
        Get
            Return _Cache
        End Get
        Set(ByVal value As Object)
            _Cache = value
            My_OnPropertyChanged("Cache")
        End Set
    End Property

    Private _CacheDateTime As Date = Nothing
    Public ReadOnly Property CacheDateTime() As Date
        Get
            Return _CacheDateTime
        End Get
        'Set(ByVal value As Date)
        '    _CacheDateTime = value
        '    My_OnPropertyChanged("CacheDateTime")
        'End Set
    End Property

    Private _SurveyID As Integer = 0
    Public Property SurveyID() As Integer
        Get
            Return _SurveyID
        End Get
        Set(ByVal value As Integer)
            _SurveyID = value
            My_OnPropertyChanged("SurveyID")
        End Set
    End Property

    Private _LoginID As Integer = 0
    Public Property LoginID() As Integer
        Get
            Return _LoginID
        End Get
        Set(ByVal value As Integer)
            _LoginID = value
            My_OnPropertyChanged("LoginID")
        End Set
    End Property
End Class

''' <summary>
''' InstanceTracker
''' </summary>
''' <remarks></remarks>
Public Class InstanceTracker
    Private OC As CustomSvcHost
    Public Sub New(ByVal _OpContext_Current_Host As CustomSvcHost)
        _OpContext_Current_Host.LastInanceCreatedDateTime = DateTime.Now
        _OpContext_Current_Host.InstanceCount += 1
        OC = _OpContext_Current_Host
    End Sub

    Public Sub TrackMethod(ByVal _Name As String, ByVal _surveyID As Integer)
        OC.LastOperationKVP = New KeyValuePair(Of String, String)(_Name, DateTime.Now.ToLongTimeString)
        If _surveyID > 0 Then
            OC.LastSurveyID = _surveyID
        End If
    End Sub
End Class
