Imports System.Runtime.Serialization
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Text

<DataContract()> _
Public Class DC_Package

#Region "Private Field Declarations"
    Dim MyCustomerDBSvc As New CustomerDBSvc.CustomerDBSvc
        MyCustomerDBSvc.DC_ConnectionString
    Private _surveyRightsCache As Dictionary(Of Integer, RightsInfo)
    Private _surveyConnStringCache As SurveyConnectionStringCache
    Private _rightsLock As Object
    Private _ttl As Integer
    Private _loginID As Integer
#End Region

#Region "Constructors"
    Public Sub New()

    End Sub

    Public Sub New(ByVal LogInID As Integer, ByVal ttl As Integer)
        Me._surveyRightsCache = New Dictionary(Of Integer, RightsInfo)
        Me._surveyConnStringCache = New SurveyConnectionStringCache(ttl)
        Me._ttl = ttl
        Me._rightsLock = New Object()
        Me._loginID = LogInID
    End Sub
#End Region

#Region "DC Rights and CnxnString Methods"
    Public Function GetPrivilegeBitmask(ByVal surveyID As Integer) As UInteger

        Dim ri As RightsInfo = Nothing
        Dim bRet = False
        Dim bitmask As UInteger = 0

        If Me._surveyRightsCache.TryGetValue(surveyID, ri) Then
            If ri.ExpirationDateTime >= DateTime.UtcNow Then
                bitmask = ri.RightsBitmask
                bRet = True
            End If
        End If

        If Not bRet Then
            SyncLock (Me._rightsLock)
                _surveyRightsCache.Remove(surveyID)
            End SyncLock
            bitmask = Me.GetSurveyRightsFromDB(surveyID)
        End If

        Return bitmask
    End Function

    Public Function GetConnString(ByVal surveyID As Integer) As String
        Return Nothing
    End Function

    Private Function GetSurveyRightsFromDB(ByVal surveyID As Integer) As UInteger

        Dim conn As SqlConnection
        Dim command As SqlCommand
        Dim connString As String
        Dim rights As UInteger

        rights = 0
        connString = Me._surveyConnStringCache.GetConnString(surveyID)
        If (connString Is Nothing) Then

            Throw New ApplicationException("No connection string found for SurveyID " + surveyID.ToString())
        Else
            conn = New SqlConnection(connString)
            command = New SqlCommand("proc_GetSurveyRightsBySurveyIDLoginID", conn)
            command.CommandType = CommandType.StoredProcedure
            command.Parameters.Add("@SurveyID", System.Data.SqlDbType.Int)
            command.Parameters("@SurveyID").Value = surveyID
            command.Parameters.Add("@LoginID", System.Data.SqlDbType.Int)
            command.Parameters("@LoginID").Value = Me._loginID
            conn.Open()
            rights = CType(command.ExecuteScalar(), UInteger)

            If (rights <> 0) Then
                Me.AddSurveyRights(surveyID, rights)
            End If
            conn.Close()
            conn.Dispose()
            command.Dispose()
            conn = Nothing
            command = Nothing
        End If
        Return rights
    End Function

    Private Function AddSurveyRights(ByVal surveyID As Integer, ByVal rightsBitmask As UInteger) As Boolean
        Dim ri = New RightsInfo()
        Dim bRet = True

        Try
            ri.RightsBitmask = rightsBitmask
            ri.ExpirationDateTime = DateTime.UtcNow.AddSeconds(Me._ttl)
            If (_surveyRightsCache.ContainsKey(surveyID)) Then
                _surveyRightsCache.Remove(surveyID)
                _surveyRightsCache.Add(surveyID, ri)
            End If
        Catch ex As ArgumentException
            bRet = False
        End Try

        Return bRet
    End Function
#End Region

#Region "SurveyDCList and SurveyPrivilegeList Properties"
    Private _Survey_DC_List As New List(Of Srlzd_KVP)
    <DataMember()> _
    Public Property Survey_DC_List() As List(Of Srlzd_KVP)
        Get
            Return _Survey_DC_List
        End Get
        Set(ByVal value As List(Of Srlzd_KVP))
            _Survey_DC_List = value
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
        End Set
    End Property
#End Region

#Region "Retrieval Methods"


    Public Function DC_Cnxn_For_SurveyID(ByVal _SurveyID As Integer) As String 'NEED TO DEAL WITH ADD NEW CASE...
        Dim rslt As String = ""
        'Me.DC_Cnxn_IsValid = False
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
            'Me.DC_Cnxn_IsValid = False
            rslt = "IsNotSet"
        End Try
        Return rslt
    End Function


    Public Function Retrieve_PrivilegeBitMask(ByVal _SurveyID As Integer) As ULong
        Dim q = From kvp In Survey_Privilege_List _
                Where kvp.Key = _SurveyID _
                Select kvp.Valu
        q.DefaultIfEmpty(Nothing)
        Return q.SingleOrDefault
    End Function

#End Region

#Region "RightInfo and SurveyClasses"

    Public Class SurveyConnectionStringCache

        Private _scCache As Dictionary(Of Integer, ConnStringInfo)
        Private _lock As Object
        Private _ttl As Integer

        Public Sub New(ByVal ttl As Integer)

            Me._scCache = New Dictionary(Of Integer, ConnStringInfo)()
            Me._ttl = ttl
            Me._lock = New Object()
        End Sub

        Public Function GetConnString(ByVal surveyID As Integer) As String

            Dim csi As ConnStringInfo = Nothing
            Dim bRet As Boolean = False
            Dim connString As String = Nothing

            If (Me._scCache.TryGetValue(surveyID, csi)) Then

                If (csi.ExpirationDateTime >= DateTime.UtcNow) Then
                    connString = csi.ConnString
                    bRet = True
                End If
            End If

            If (Not bRet) Then
                SyncLock (Me._lock)
                    Me._scCache.Remove(surveyID)
                End SyncLock
                connString = Me.GetConnStringFromDB(surveyID)
            End If
            Return connString
        End Function

        'TODO: THIS IS GOING AGAINST GLOBALSURVEYMASTER>>>NEEDS TO BE AGAINST CUSTOMERSURVEYMASTER...
        Private Function GetConnStringFromDB(ByVal surveyID As Integer) As String

            Dim connStrings As ConnectionStringSettingsCollection
            Dim conn As SqlConnection
            Dim command As SqlCommand
            Dim connString As String

            connString = Nothing
            connStrings = ConfigurationManager.ConnectionStrings
            conn = New SqlConnection(connStrings("SurveyMasterConnectionString").ConnectionString)
            command = New SqlCommand("proc_GetConnStringBySurveyID", conn)
            command.CommandType = CommandType.StoredProcedure
            command.Parameters.Add("@SurveyID", System.Data.SqlDbType.Int)
            command.Parameters("@SurveyID").Value = surveyID
            conn.Open()
            connString = CType(command.ExecuteScalar(), String)

            If (connString IsNot Nothing) Then

                '// TODO: Change this when Wayne starts storing the connection string in the SurveyMaster DB
                Dim sb = New StringBuilder()
                sb.Append("Data Source=.\DEVRENTS;Database=")
                sb.Append(connString)
                sb.Append(";Integrated Security=True;Connect Timeout=30;User Instance=False")
                connString = sb.ToString()

                '// This should be all that is required once the query returns the connection string
                Me.AddConnString(surveyID, connString)
            End If


            conn.Close()
            conn.Dispose()
            command.Dispose()
            conn = Nothing
            command = Nothing

            Return connString

        End Function

        Private Function AddConnString(ByVal surveyID As Integer, ByVal connString As String) As Boolean

            Dim csi = New ConnStringInfo()
            Dim bRet = True

            Try
                csi.ConnString = connString
                csi.ExpirationDateTime = DateTime.UtcNow.AddSeconds(Me._ttl)
                If (_scCache.ContainsKey(surveyID)) Then
                    _scCache.Remove(surveyID)
                End If
                _scCache.Add(surveyID, csi)
            Catch ex As ArgumentException
                bRet = False
            End Try

            Return bRet
        End Function

    End Class

    <DataContract()> _
    Public Class ConnStringInfo
        Private _ConnString As String
        <DataMember()> _
        Public Property ConnString() As String
            Get
                Return _ConnString
            End Get
            Set(ByVal value As String)
                _ConnString = value
            End Set
        End Property

        Private _ExpirationDateTime
        <DataMember()> _
        Public Property ExpirationDateTime() As DateTime
            Get
                Return _ExpirationDateTime
            End Get
            Set(ByVal value As DateTime)
                _ExpirationDateTime = value
            End Set
        End Property
    End Class

    <DataContract()> _
    Public Class RightsInfo

        Private _RightsBitmask As UInteger
        <DataMember()> _
        Public Property RightsBitmask() As UInteger
            Get
                Return _RightsBitmask
            End Get
            Set(ByVal value As UInteger)
                _RightsBitmask = value
            End Set
        End Property

        Private _ExpirationDateTime
        <DataMember()> _
        Public Property ExpirationDateTime() As DateTime
            Get
                Return _ExpirationDateTime
            End Get
            Set(ByVal value As DateTime)
                _ExpirationDateTime = value
            End Set
        End Property
    End Class
#End Region
End Class
