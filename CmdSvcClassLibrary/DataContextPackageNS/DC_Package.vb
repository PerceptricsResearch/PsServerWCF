Imports CmdInfrastructureNS
Imports CustomerDBOperationsNS
Imports System.Runtime.Serialization
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Text

<DataContract()> _
Public Class DC_Package
    ''' <summary>
    ''' This is the GlobalSurveyMaster_LoginID provided to the constructor of this instance...
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GSM_LoginID() As Integer
        Get
            Return _loginID
        End Get
    End Property

#Region "Private Field Declarations"
    'Private _surveyRightsCache As Dictionary(Of Integer, RightsInfo)
    'Private _surveyConnStringCache As SurveyConnectionStringCache
    'Private _rightsLock As Object
    Private _ttl As Integer
    Private _loginID As Integer
#End Region

#Region "Constructors"
    Public Sub New()

    End Sub

    Public Sub New(ByVal LogInID As Integer, ByVal ttl As Integer, ByVal _CustomerDBCnxnString As String)
        'Me._surveyRightsCache = New Dictionary(Of Integer, RightsInfo)
        'Me._surveyConnStringCache = New SurveyConnectionStringCache(ttl, _CustomerDBCnxnString, LogInID)
        Me._ttl = ttl
        'Me._rightsLock = New Object()
        Me._loginID = LogInID
        MyCustomerDBCnxnString = _CustomerDBCnxnString
    End Sub

#End Region 'Constructors

#Region "Public Properties SurveyDCList and SurveyPrivilegeList "
    Private ExpirationDateTime_SurveyPrivList As DateTime
    Private ExpirationDateTime_Survey_DC_List As DateTime

    Private _Survey_DC_List As New List(Of Srlzd_KVP)
    <DataMember()> _
    Public Property Survey_DC_List() As List(Of Srlzd_KVP)
        Get
            Return _Survey_DC_List
        End Get
        Set(ByVal value As List(Of Srlzd_KVP))
            _Survey_DC_List = value
            Me.ExpirationDateTime_Survey_DC_List = DateTime.UtcNow.AddSeconds(Me._ttl)
        End Set
    End Property

    ''' <summary>
    ''' List of SurveyID and PrivilegeBitMask of PrivilegeDescrEnum
    ''' </summary>
    ''' <remarks></remarks>
    Private _Survey_Privilege_List As New List(Of Srlzd_KVP)
    <DataMember()> _
    Public Property Survey_Privilege_List() As List(Of Srlzd_KVP)
        Get
            Return _Survey_Privilege_List
        End Get
        Set(ByVal value As List(Of Srlzd_KVP))
            _Survey_Privilege_List = value
            Me.ExpirationDateTime_SurveyPrivList = DateTime.UtcNow.AddSeconds(Me._ttl)
        End Set
    End Property
#End Region

#Region "Public Retrieval Methods - DC_Cnxn_for_SurveyID, Retrieve_PrivilegeBitMask"
    Public Function DC_Cnxn_For_SurveyID(ByVal _SurveyID As Integer) As String 'NEED TO DEAL WITH ADD NEW CASE...
        Dim rslt As String = Nothing
        If Me.ExpirationDateTime_Survey_DC_List < DateTime.UtcNow Then
            Dim spdcpkg = Me.Retrieve_SurveyPrivilegeDCColxn_Pkg
            Me.Survey_Privilege_List = spdcpkg.Survey_Privilege_List
            Me.Survey_DC_List = spdcpkg.Survey_DC_List
        End If

        Try

            Dim q = From kvp In Me.Survey_DC_List _
               Where kvp.Key = _SurveyID _
               Select kvp
            q.DefaultIfEmpty(Nothing)
            If q.Any Then
                rslt = q.First.Valu 'CnxnString_AbsolutePath(rslt)
            End If
            'Me.DC_Cnxn_IsValid = True
        Catch ex As Exception
            rslt = "IsNotSet"
            Using EvLog As New EventLog()
                EvLog.Source = "DC_Cnxn_For_SurveyID"
                EvLog.Log = "Application"
                EvLog.WriteEntry("GSM_LoginID= " & Me.GSM_LoginID & ", SurveyID= " & _SurveyID.ToString & " " & ex.Message, EventLogEntryType.Error)
            End Using

        End Try
        Return rslt
    End Function

    Public Function Retrieve_PrivilegeBitMask(ByVal _SurveyID As Integer) As ULong
        If Me.ExpirationDateTime_SurveyPrivList < DateTime.UtcNow Then
            Dim spdcpkg = Me.Retrieve_SurveyPrivilegeDCColxn_Pkg
            Me.Survey_Privilege_List = spdcpkg.Survey_Privilege_List
            Me.Survey_DC_List = spdcpkg.Survey_DC_List
        End If
        Dim q = From kvp In Survey_Privilege_List _
                Where kvp.Key = _SurveyID _
                Select kvp.Valu
        q.DefaultIfEmpty(Nothing)
        Return q.SingleOrDefault
    End Function

    Private Function Retrieve_SurveyPrivilegeDCColxn_Pkg() As Survey_Priv_DC_Colxn_Pkg
        Dim rslt As Survey_Priv_DC_Colxn_Pkg = Nothing
        If SetUpCustomerDBsvc() Then
            Try
                rslt = MyCustomerDBMgr.GetPrivServiceMappings(_loginID)
            Catch ex As Exception
                SharedEvents.RaiseOperationFailed(Me, "DCPkg.Retrieve_SurveyPrivilegeDCColxn_Pkg - refresh reports..." & ex.Message)
            End Try
        End If
        Return rslt
    End Function

    Public Function Refresh() As Boolean
        Dim rslt As Boolean = False
        Try
            Dim spdcpkg = Me.Retrieve_SurveyPrivilegeDCColxn_Pkg
            Me.Survey_Privilege_List = spdcpkg.Survey_Privilege_List
            Me.Survey_DC_List = spdcpkg.Survey_DC_List
            rslt = True
        Catch ex As Exception
            CmdInfrastructureNS.SharedEvents.RaiseOperationFailed(New Object, "DCPackage.Refresh reports " & ex.Message)
        End Try
        Return rslt
    End Function
#End Region

#Region "MyCustomerDBSvc - SetUp and CnxnString Property"
#Region " SetUpCustomerDBSvc"
    Private Function SetUpCustomerDBsvc() As Boolean
        Dim rslt As Boolean = False
        If MyCustomerDBMgr Is Nothing Then
            Try
                MyCustomerDBMgr = New CustomerDBOperationsNS.Manager
                MyCustomerDBMgr.DC_ConnectionString = MyCustomerDBCnxnString
                rslt = True
            Catch ex As Exception
                MyCustomerDBMgr = Nothing
                rslt = False
            End Try
        Else
            MyCustomerDBMgr.DC_ConnectionString = MyCustomerDBCnxnString
            rslt = True
        End If

        Return rslt
    End Function
#End Region

#Region "Properties -  MyCustomerDBSvc and CnxnString "
    Private _MyCustomerDBMgr As CustomerDBOperationsNS.Manager
    Public Property MyCustomerDBMgr() As CustomerDBOperationsNS.Manager
        Get
            Return _MyCustomerDBMgr
        End Get
        Set(ByVal value As CustomerDBOperationsNS.Manager)
            _MyCustomerDBMgr = value
        End Set
    End Property

    Private _MyCustomerDBCnxnString As String
    Public Property MyCustomerDBCnxnString() As String
        Get
            Return _MyCustomerDBCnxnString
        End Get
        Set(ByVal value As String)
            _MyCustomerDBCnxnString = value
        End Set
    End Property
#End Region
#End Region




    ' NOT USED STUFF...
#Region "RightInfo and SurveyConnectinStringCache Classes...cache is in SurveyDCList property"
    '    <Serializable()> _
    '    Public Class SurveyConnectionStringCache

    '        Private _scCache As Dictionary(Of Integer, ConnStringInfo)
    '        Private _lock As Object
    '        Private _ttl As Integer
    '        Private _loginid As Integer

    '        Public Sub New(ByVal ttl As Integer, ByVal _CustomerDBCnxnString As String, ByVal _LoginID As Integer)
    '            Me.MyCustomerDBCnxnString = _CustomerDBCnxnString
    '            Me._loginid = _LoginID
    '            Me._scCache = New Dictionary(Of Integer, ConnStringInfo)()
    '            Me._ttl = ttl
    '            Me._lock = New Object()
    '        End Sub

    '        Public Function DC_Cnxn_For_SurveyID(ByVal _SurveyID As Integer) As String 'NEED TO DEAL WITH ADD NEW CASE...
    '            Dim rslt As String = ""
    '            'Me.DC_Cnxn_IsValid = False
    '            Try
    '                Dim q = From kvp In Me.Survey_DC_List _
    '                   Where kvp.Key = _SurveyID _
    '                   Select kvp
    '                q.DefaultIfEmpty(Nothing)
    '                If q.Any Then
    '                    rslt = q.First.Valu 'CnxnString_AbsolutePath(rslt)
    '                End If
    '                'Me.DC_Cnxn_IsValid = True
    '            Catch ex As Exception
    '                'Me.DC_Cnxn_IsValid = False
    '                rslt = "IsNotSet"
    '            End Try
    '            Return rslt
    '        End Function

    '        Public Function GetConnString(ByVal surveyID As Integer) As String

    '            Dim csi As ConnStringInfo = Nothing
    '            Dim bRet As Boolean = False
    '            Dim connString As String = Nothing

    '            If (Me._scCache.TryGetValue(surveyID, csi)) Then

    '                If (csi.ExpirationDateTime >= DateTime.UtcNow) Then
    '                    connString = csi.ConnString
    '                    bRet = True
    '                End If
    '            End If

    '            If (Not bRet) Then
    '                SyncLock (Me._lock)
    '                    Me._scCache.Remove(surveyID)
    '                End SyncLock
    '                connString = Me.GetConnStringFromDB(surveyID)
    '            End If
    '            Return connString
    '        End Function

    '        Private Function GetConnStringFromDB(ByVal _surveyID As Integer) As String
    '            Dim connString As String = Nothing

    '            If SetUpCustomerDBsvc() Then
    '                Me.Survey_DC_List.Clear()
    '                Me.Survey_DC_List = Retrieve_SurveyPrivilegeDCColxn_Pkg().Survey_DC_List
    '                connString = DC_Cnxn_For_SurveyID(_surveyID)
    '            End If

    '            If (connString IsNot Nothing) Then

    '                '// TODO: Change this when Wayne starts storing the connection string in the SurveyMaster DB
    '                Dim sb = New StringBuilder()
    '                sb.Append("Data Source=.\DEVRENTS;Database=")
    '                sb.Append(connString)
    '                sb.Append(";Integrated Security=True;Connect Timeout=30;User Instance=False")
    '                connString = sb.ToString()

    '                '// This should be all that is required once the query returns the connection string
    '                Me.AddConnString(_surveyID, connString)
    '            End If


    '            Return connString

    '        End Function

    '        Private Function AddConnString(ByVal surveyID As Integer, ByVal connString As String) As Boolean

    '            Dim csi = New ConnStringInfo()
    '            Dim bRet = True

    '            Try
    '                csi.ConnString = connString
    '                csi.ExpirationDateTime = DateTime.UtcNow.AddSeconds(Me._ttl)
    '                If (_scCache.ContainsKey(surveyID)) Then
    '                    _scCache.Remove(surveyID)
    '                End If
    '                _scCache.Add(surveyID, csi)
    '            Catch ex As ArgumentException
    '                bRet = False
    '            End Try

    '            Return bRet
    '        End Function

    '#Region "MyCustomerDBSvc Stuff"

    '#Region "MyCustomerDBSvc Methods"
    '        Private Function SetUpCustomerDBsvc() As Boolean
    '            Dim rslt As Boolean = False
    '            If MyCustomerDBMgr Is Nothing Then
    '                Try
    '                    MyCustomerDBMgr = New CustomerDBOperationsNS.Manager
    '                    MyCustomerDBMgr.DC_ConnectionString = MyCustomerDBCnxnString
    '                    rslt = True
    '                Catch ex As Exception
    '                    MyCustomerDBMgr = Nothing
    '                    rslt = False
    '                End Try
    '            Else
    '                MyCustomerDBMgr.DC_ConnectionString = MyCustomerDBCnxnString
    '                rslt = True
    '            End If

    '            Return rslt
    '        End Function

    '        Private Function Retrieve_SurveyPrivilegeDCColxn_Pkg() As Survey_Priv_DC_Colxn_Pkg
    '            Dim rslt As Survey_Priv_DC_Colxn_Pkg = Nothing
    '            If SetUpCustomerDBsvc() Then
    '                Try
    '                    rslt = MyCustomerDBMgr.GetPrivServiceMappings(_loginid)
    '                Catch ex As Exception
    '                    Dim x = 2
    '                End Try
    '            End If
    '            Return rslt
    '        End Function
    '#End Region

    '#Region "MyCustomerDBSvc and CnxnString Properties"
    '        Private _MyCustomerDBMgr As CustomerDBOperationsNS.Manager
    '        Public Property MyCustomerDBMgr() As CustomerDBOperationsNS.Manager
    '            Get
    '                Return _MyCustomerDBMgr
    '            End Get
    '            Set(ByVal value As CustomerDBOperationsNS.Manager)
    '                _MyCustomerDBMgr = value
    '            End Set
    '        End Property

    '        Private _MyCustomerDBCnxnString As String
    '        <DataMember()> _
    '        Public Property MyCustomerDBCnxnString() As String
    '            Get
    '                Return _MyCustomerDBCnxnString
    '            End Get
    '            Set(ByVal value As String)
    '                _MyCustomerDBCnxnString = value
    '            End Set
    '        End Property
    '#End Region

    '#End Region

    '#Region "SurveyDCList and SurveyPrivilegeList Properties"
    '        Private _Survey_DC_List As New List(Of Srlzd_KVP)
    '        <DataMember()> _
    '        Public Property Survey_DC_List() As List(Of Srlzd_KVP)
    '            Get
    '                Return _Survey_DC_List
    '            End Get
    '            Set(ByVal value As List(Of Srlzd_KVP))
    '                _Survey_DC_List = value
    '            End Set
    '        End Property

    '        ''' <summary>
    '        ''' List of SurveyID and PrivilegeBitMask of PrivilegeDescrEnum
    '        ''' </summary>
    '        ''' <remarks></remarks>
    '        Private _Survey_Privilege_List As New List(Of Srlzd_KVP)
    '        <DataMember()> _
    '        Public Property Survey_Privilege_List() As List(Of Srlzd_KVP)
    '            Get
    '                Return _Survey_Privilege_List
    '            End Get
    '            Set(ByVal value As List(Of Srlzd_KVP))
    '                _Survey_Privilege_List = value
    '            End Set
    '        End Property
    '#End Region
    '    End Class

    '    <DataContract()> _
    '    Public Class ConnStringInfo
    '        Private _ConnString As String
    '        <DataMember()> _
    '        Public Property ConnString() As String
    '            Get
    '                Return _ConnString
    '            End Get
    '            Set(ByVal value As String)
    '                _ConnString = value
    '            End Set
    '        End Property

    '        Private _ExpirationDateTime
    '        <DataMember()> _
    '        Public Property ExpirationDateTime() As DateTime
    '            Get
    '                Return _ExpirationDateTime
    '            End Get
    '            Set(ByVal value As DateTime)
    '                _ExpirationDateTime = value
    '            End Set
    '        End Property
    '    End Class

    '    <DataContract()> _
    '    Public Class RightsInfo

    '        Private _RightsBitmask As UInteger
    '        <DataMember()> _
    '        Public Property RightsBitmask() As UInteger
    '            Get
    '                Return _RightsBitmask
    '            End Get
    '            Set(ByVal value As UInteger)
    '                _RightsBitmask = value
    '            End Set
    '        End Property

    '        Private _ExpirationDateTime
    '        <DataMember()> _
    '        Public Property ExpirationDateTime() As DateTime
    '            Get
    '                Return _ExpirationDateTime
    '            End Get
    '            Set(ByVal value As DateTime)
    '                _ExpirationDateTime = value
    '            End Set
    '        End Property
    '    End Class
#End Region

#Region "DC Rights and SurveyCnxnString Methods commented out...not used..."
    'Public Function GetPrivilegeBitmask(ByVal surveyID As Integer) As UInteger

    '    Dim ri As RightsInfo = Nothing
    '    Dim bRet = False
    '    Dim bitmask As UInteger = 0

    '    If Me._surveyRightsCache.TryGetValue(surveyID, ri) Then
    '        If ri.ExpirationDateTime >= DateTime.UtcNow Then
    '            bitmask = ri.RightsBitmask
    '            bRet = True
    '        End If
    '    End If

    '    If Not bRet Then
    '        SyncLock (Me._rightsLock)
    '            _surveyRightsCache.Remove(surveyID)
    '        End SyncLock
    '        bitmask = Me.GetSurveyRightsFromDB(surveyID)
    '    End If

    '    Return bitmask
    'End Function

    'Public Function GetConnString(ByVal surveyID As Integer) As String
    '    Return Nothing
    'End Function

    'Private Function GetSurveyRightsFromDB(ByVal surveyID As Integer) As UInteger

    '    Dim conn As SqlConnection
    '    Dim command As SqlCommand
    '    Dim connString As String
    '    Dim rights As UInteger

    '    rights = 0
    '    connString = Me._surveyConnStringCache.GetConnString(surveyID)
    '    If (connString Is Nothing) Then

    '        Throw New ApplicationException("No connection string found for SurveyID " + surveyID.ToString())
    '    Else
    '        conn = New SqlConnection(connString)
    '        command = New SqlCommand("proc_GetSurveyRightsBySurveyIDLoginID", conn)
    '        command.CommandType = CommandType.StoredProcedure
    '        command.Parameters.Add("@SurveyID", System.Data.SqlDbType.Int)
    '        command.Parameters("@SurveyID").Value = surveyID
    '        command.Parameters.Add("@LoginID", System.Data.SqlDbType.Int)
    '        command.Parameters("@LoginID").Value = Me._loginID
    '        conn.Open()
    '        rights = CType(command.ExecuteScalar(), UInteger)

    '        If (rights <> 0) Then
    '            Me.AddSurveyRights(surveyID, rights)
    '        End If
    '        conn.Close()
    '        conn.Dispose()
    '        command.Dispose()
    '        conn = Nothing
    '        command = Nothing
    '    End If
    '    Return rights
    'End Function

    'Private Function AddSurveyRights(ByVal surveyID As Integer, ByVal rightsBitmask As UInteger) As Boolean
    '    Dim ri = New RightsInfo()
    '    Dim bRet = True

    '    Try
    '        ri.RightsBitmask = rightsBitmask
    '        ri.ExpirationDateTime = DateTime.UtcNow.AddSeconds(Me._ttl)
    '        If (_surveyRightsCache.ContainsKey(surveyID)) Then
    '            _surveyRightsCache.Remove(surveyID)
    '            _surveyRightsCache.Add(surveyID, ri)
    '        End If
    '    Catch ex As ArgumentException
    '        bRet = False
    '    End Try

    '    Return bRet
    'End Function
#End Region
End Class
