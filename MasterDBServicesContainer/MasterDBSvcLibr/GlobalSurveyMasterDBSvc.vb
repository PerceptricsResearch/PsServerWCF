Imports CmdInfrastructureNS
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Text
Imports ILogInSvc
Imports System.Security.Cryptography

Public Class GlobalSurveyMasterDBSvc
    Implements IDisposable


    Public DC_ConnectionString As String
#Region "Private Fields"
    Private WithEvents MyDB As L2S_MasterSurveyDBDataContext
    Private MyDBAvailable As Boolean = False
    Public OperationFailedName As String = Nothing
    Public MyException As Exception = Nothing
    Private MyDB_TTL As Integer = 0 'not implemented '''would keep the MyDB datacontext around for some time...if past the time, dispose, myDB then create New l2s_DBcontext
#End Region

    Public Sub New()
        Try
            AddHandler Me.GSM_DB_UnavailableEvent, AddressOf DBUnavailableHandler
            AddHandler Me.GSM_DB_OperationFailed, AddressOf DBOperationFailedHandler

            Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            Dim cnxnstring = config.ConnectionStrings.ConnectionStrings("SurveyMasterConnectionString").ToString
            Me.DC_ConnectionString = cnxnstring
            Me.MyDB = New L2S_MasterSurveyDBDataContext(cnxnstring)
            MyDBAvailable = True
            config = Nothing
            cnxnstring = Nothing
        Catch ex As Exception
            RaiseEvent GSM_DB_UnavailableEvent("New", New EventArgs)
        End Try
    End Sub

    Public Function ToActiveLoginInfo(ByVal _NormalizedEmailAddress As String) As ActiveLoginInfo
        Dim rslt As ActiveLoginInfo = Nothing
        If MyDBAvailable Then
            Try
                rslt = (From li In MyDB.LoginInfos, c In MyDB.Customers, sds In MyDB.SurveyDataStores _
                              Where li.LoginEmail = _NormalizedEmailAddress AndAlso li.CustomerID = c.CustomerID AndAlso sds.SurveyDataStoreID = c.CustomerSurveyMasterID _
                              Select New ActiveLoginInfo With {.GSM_LoginID = li.LogInID, _
                                                              .NormalizedEmailAddress = _NormalizedEmailAddress}).FirstOrDefault
                rslt.SubscrInfo = ToSubscriberInfo(_NormalizedEmailAddress)
            Catch ex As Exception
                Me.OperationFailedName = "ToActiveLoginInfo"
                Me.MyException = ex
                RaiseEvent GSM_DB_OperationFailed("ToActiveLoginInfo", New EventArgs)
            End Try
        Else
            RaiseEvent GSM_DB_UnavailableEvent("ToActiveLoginInfo", New EventArgs)
        End If
        Return rslt
    End Function

    Public Function ToSubscriberInfo(ByVal _NormalizedEmailAddress As String) As SubscriberInfo
        Dim rslt As SubscriberInfo = Nothing
        If MyDBAvailable Then
            Try
                rslt = (From li In MyDB.LoginInfos, c In MyDB.Customers, sds In MyDB.SurveyDataStores _
                              Where li.LoginEmail = _NormalizedEmailAddress AndAlso li.CustomerID = c.CustomerID AndAlso sds.SurveyDataStoreID = c.CustomerSurveyMasterID _
                              Select New SubscriberInfo With {.GSM_CustomerID = c.CustomerID, _
                                                              .GSM_LoginID = li.LogInID, _
                                                              .NormalizedEmailAddress = _NormalizedEmailAddress, _
                                                              .SSM_CnxnString = sds.CnxnString}).FirstOrDefault
                If rslt IsNot Nothing Then
                    rslt.QueueInfo = (From li In MyDB.LoginInfos, c In MyDB.Customers, sds In MyDB.SurveyDataStores _
                              Where li.LoginEmail = _NormalizedEmailAddress AndAlso li.CustomerID = c.CustomerID AndAlso sds.SurveyDataStoreID = c.RDENTQueueURI_ID _
                              Select New QueueInfo With {.AbsolutePath = sds.AbsolutePath, _
                                                         .CnxnString = sds.CnxnString, _
                                                         .QueueName = sds.DatabaseName}).FirstOrDefault
                End If

                '.SSM_CnxnString = sds.CnxnString}).FirstOrDefault
                '.SSM_CnxnString = GetTemporaryCnxnString()}).FirstOrDefault
            Catch ex As Exception
                Me.OperationFailedName = "ToSubscriberInfo"
                Me.MyException = ex
                RaiseEvent GSM_DB_OperationFailed("ToSubscriberInfo", New EventArgs)
            End Try
        Else
            RaiseEvent GSM_DB_UnavailableEvent("ToSubscriberInfo", New EventArgs)
        End If
        Return rslt
    End Function

#Region "Action Methods - NewSubscriber, NewLoginToExistingSubscriber"
    Public Function AddNewSubscriber(ByRef _NewSubscriberPkg As NewSubscriberPackage) As Boolean
        Dim rslt As Boolean = False
        Try
            Dim sqlops As New SqlServerOperations(SqlServerOperations.DatabaseCreateAction.CreateNewSubscriberDatabases)
            Dim cdbi = sqlops.CreateNewSubscriberDatabases(_NewSubscriberPkg.NormalizedEmailAddress)
            _NewSubscriberPkg.CreateDBInfo = cdbi
            Dim newCustomerRow As New Customer With {.CustomerName = _NewSubscriberPkg.SubscriberName, _
                                                     .PrimaryContactInfoID = 1, _
                                                     .CurrentAuthorizationID = _NewSubscriberPkg.AuthorizationID, _
                                                     .PrimaryServerID = 1}
            'insert sds rows...should be three of them...get their id's...need them  to populate fields in the Customer row...
            Dim ssmSDSrow As New SurveyDataStore With {.DatabaseName = cdbi.Destination_SurveyMasterDatabaseName, _
                                                       .CnxnString = cdbi.Destination_SurveyMaster_CnxnString, _
                                                       .AbsolutePath = cdbi.DestinationServerName, _
                                                       .ComputerID = 1}

            Dim primaryDataStore_SDSrow As New SurveyDataStore With {.DatabaseName = cdbi.Destination_DataStoreDatabaseName, _
                                                                     .CnxnString = cdbi.Destination_DataStore_CnxnString, _
                                                                     .AbsolutePath = cdbi.DestinationServerName, _
                                                                     .ComputerID = 1}
            Dim qURI = Replace(SharedMethods.GetValueFromAppConfig("SubscriberRDENTQueueURI"), "exxon", cdbi.NormalizedEmailAddress)
            Dim qName = Replace(SharedMethods.GetValueFromAppConfig("SubscriberRDENTQueueName"), "exxon", cdbi.NormalizedEmailAddress)
            Dim rdentQueue_SDSrow As New SurveyDataStore With {.AbsolutePath = qURI, _
                                                               .DatabaseName = qName, _
                                                               .CnxnString = qURI, _
                                                               .ComputerID = 1}
            _NewSubscriberPkg.RDentQueueURI = qURI
            _NewSubscriberPkg.RDentQueueName = qName
            'insert the Customer row...get it's id...need it for the loginInfo row
            newCustomerRow.CustomerSurveyMasterID = InsertNewSDSRow(ssmSDSrow)
            newCustomerRow.PrimaryDataStoreID = InsertNewSDSRow(primaryDataStore_SDSrow)
            newCustomerRow.RDENTQueueURI_ID = InsertNewSDSRow(rdentQueue_SDSrow)
            Dim pwdpgk = GlobalMasterDataByLoginNameResult_Package.EncodeNew(_NewSubscriberPkg.InitialPassword)
            Dim newLoginInfoRow As New LoginInfo With {.CustomerID = InsertNewCustomerRow(newCustomerRow), _
                                                       .IsLoggedIn = False, _
                                                       .LoginEmail = _NewSubscriberPkg.NormalizedEmailAddress, _
                                                       .PasswordHash = pwdpgk.PwdHashINT, _
                                                       .LastSaltByteArray = pwdpgk.Salt, _
                                                       .PasswordLastSetDate = DateAndTime.Now, _
                                                       .LastLoginDate = DateAndTime.Now}

            'insert the logininfo row...
            InsertNewLoginInfoRow(newLoginInfoRow)
            _NewSubscriberPkg.SubscrInfo = Me.ToSubscriberInfo(_NewSubscriberPkg.NormalizedEmailAddress)
            _NewSubscriberPkg.ActiveLogin = Me.ToActiveLoginInfo(_NewSubscriberPkg.NormalizedEmailAddress)
            Using EvLog As New EventLog()
                EvLog.Source = "GlobalSurveyMasterDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("AddNewSubscriber" & _NewSubscriberPkg.NormalizedEmailAddress, EventLogEntryType.SuccessAudit)
            End Using
            rslt = True
        Catch ex As Exception
            Me.OperationFailedName = "AddNewSubscriber"
            Me.MyException = ex
            Using EvLog As New EventLog()
                EvLog.Source = "GlobalSurveyMasterDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("AddNewSubscriber " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function
    Public Function AddNewLoginToExistingSubscriber(ByRef _newLoginPkg As NewLoginPackage) As Boolean
        Dim rslt As Boolean = False
        Try
            'this is the same code that is in the Client SmartPwdPkg function....it replicates what happens on the client for salting passwords....
            Dim pwdb = Text.Encoding.UTF8.GetBytes(_newLoginPkg.RawEmailAddress)
            Dim pwdfromclientPWDPkg = GlobalMasterDataByLoginNameResult_Package.Encode(_newLoginPkg.RawEmailAddress, pwdb.Take(Math.Min(pwdb.Length, 16)).ToArray)
            'insert new loginInfo row
            Dim pwdpkg = GlobalMasterDataByLoginNameResult_Package.EncodeNew(pwdfromclientPWDPkg.PwdHashINT)
            Dim liInfo As New LoginInfo With {.CustomerID = _newLoginPkg.SubScriberInfo.GSM_CustomerID, _
                                              .IsLoggedIn = False, _
                                              .LastLoginDate = DateTime.Now, _
                                              .LoginEmail = _newLoginPkg.NormalizedEmailAddress, _
                                              .PasswordHash = pwdpkg.PwdHashINT, _
                                              .LastSaltByteArray = pwdpkg.Salt, _
                                              .PasswordLastSetDate = DateTime.Now}
            _newLoginPkg.LoginID_GSM = InsertNewLoginInfoRow(liInfo)
            Using EvLog As New EventLog()
                EvLog.Source = "GlobalSurveyMasterDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("AddNewLoginToExistingSubscriber" & _newLoginPkg.RawEmailAddress, EventLogEntryType.SuccessAudit)
            End Using
            rslt = True
        Catch ex As Exception
            Me.OperationFailedName = "AddNewLoginToExistingSubscriber"
            Me.MyException = ex
            RaiseEvent GSM_DB_OperationFailed("AddNewLoginToExistingSubscriber", New EventArgs)
        End Try

        Return rslt
    End Function
#End Region

#Region "ChangePwd"
    Public Function ChangePwd(ByVal _pwdpkgIN As Password_Package) As Boolean
        Dim rslt As Boolean = False
        If MyDBAvailable Then
            Try
                Dim normalizedemail = SharedMethods.EmailAddress_ToNormalized(_pwdpkgIN.LogIn_Email)
                Dim linfo = MyDB.LoginInfos.Where(Function(li) li.LoginEmail = normalizedemail).FirstOrDefault
                Dim pwdpkg = GlobalMasterDataByLoginNameResult_Package.EncodeNew(_pwdpkgIN.PasswordHashINT)
                linfo.PasswordHash = pwdpkg.PwdHashINT
                linfo.PasswordLastSetDate = Date.Now
                linfo.LastSaltByteArray = pwdpkg.Salt
                MyDB.SubmitChanges()
                'this should update the UserCacheManager.Dxnry.UCI....the UCI should have the new pwdint in it....
                'or the uci should be deleted...and the user has to log back in...
                Using EvLog As New EventLog()
                    EvLog.Source = "GlobalSurveyMasterDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("ChangePwd " & normalizedemail, EventLogEntryType.SuccessAudit)
                End Using
                rslt = True
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "GlobalSurveyMasterDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("ChangePwd " & _pwdpkgIN.LogIn_Email & " " & ex.Message, EventLogEntryType.Error)
                End Using
                'SharedEvents.RaiseOperationFailed(_pwdpkgIN, "GlobalSurveyMasterDBSvc.ChangePwd")
            End Try
        Else
            Using EvLog As New EventLog()
                EvLog.Source = "GlobalSurveyMasterDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("ChangePwd " & _pwdpkgIN.LogIn_Email & "MyDBAvailable = false... ", EventLogEntryType.Error)
            End Using
            RaiseEvent GSM_DB_UnavailableEvent(Me, New EventArgs)
        End If
        Return rslt
    End Function
#End Region

#Region "Insert Methods"
    Public Function InsertNewCustomerRow(ByVal _newCustomerRow As Customer) As Integer
        Dim rslt As Integer = Nothing
        If MyDBAvailable Then
            Try
                MyDB.Customers.InsertOnSubmit(_newCustomerRow)
                MyDB.SubmitChanges()
                rslt = _newCustomerRow.CustomerID
            Catch ex As Exception
                Me.OperationFailedName = "InsertNewCustomerRow"
                Me.MyException = ex
                RaiseEvent GSM_DB_OperationFailed("InsertNewCustomerRow", New EventArgs)
            End Try
        Else
            RaiseEvent GSM_DB_UnavailableEvent("InsertNewCustomerRow", New EventArgs)
        End If
        Return rslt
    End Function

    Public Function InsertNewLoginInfoRow(ByVal _newLoginInfoRow As LoginInfo) As Integer
        Dim rslt As Integer = Nothing
        If MyDBAvailable Then
            Try
                MyDB.LoginInfos.InsertOnSubmit(_newLoginInfoRow)
                MyDB.SubmitChanges()
                rslt = _newLoginInfoRow.LogInID
            Catch ex As Exception
                Me.OperationFailedName = "InsertNewLoginInfoRow"
                Me.MyException = ex
                RaiseEvent GSM_DB_OperationFailed("InsertNewLoginInfoRow", New EventArgs)
            End Try
        Else
            RaiseEvent GSM_DB_UnavailableEvent("InsertNewLoginInfoRow", New EventArgs)
        End If
        Return rslt
    End Function

    Public Function InsertNewSDSRow(ByVal _newSDSrow As SurveyDataStore) As Integer
        Dim rslt As Integer = Nothing
        If MyDBAvailable Then
            Try
                MyDB.SurveyDataStores.InsertOnSubmit(_newSDSrow)
                MyDB.SubmitChanges()
                rslt = _newSDSrow.SurveyDataStoreID
            Catch ex As Exception
                Me.OperationFailedName = "InsertNewSDSRow"
                Me.MyException = ex
                RaiseEvent GSM_DB_OperationFailed("InsertNewSDSRow", New EventArgs)
            End Try
        Else
            RaiseEvent GSM_DB_UnavailableEvent("InsertNewSDSRow", New EventArgs)
        End If
        Return rslt
    End Function
#End Region


#Region "GlobalSurveyMaster Methods"
    Public Function WithLinqStoredProcedure(ByVal _LogInPack As LogInPackage) As GlobalMasterDataByLoginNameResult_Package
        Dim rslt As New GlobalMasterDataByLoginNameResult_Package
        Dim db As New L2S_MasterSurveyDBDataContext(DC_ConnectionString)
        Dim x = db.proc_GetGlobalMasterLoginDataByLoginName(_LogInPack.LogIn_Email, _LogInPack.PasswordHashINT)

        Return (rslt)
    End Function
    Public Function GetGlobalMasterLoginDataByLoginName(ByVal _LogInPack As LogInPackage) As GlobalMasterDataByLoginNameResult_Package
        Dim rslt As New GlobalMasterDataByLoginNameResult_Package
       
        Try
            Dim conn = New SqlConnection(DC_ConnectionString) '(connStrings("SurveyMasterConnectionString").ConnectionString)
            Dim da = New SqlDataAdapter("proc_GetGlobalMasterLoginDataByLoginName", conn)
            Dim ds = New DataSet()
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Add("@LoginEMail", System.Data.SqlDbType.VarChar)
            da.SelectCommand.Parameters("@LoginEMail").Value = _LogInPack.LogIn_Email
            da.SelectCommand.Parameters.Add("@PasswordHash", System.Data.SqlDbType.Int)
            ' TODO: Pull the integer value out that we need
            'da.SelectCommand.Parameters["@PasswordHash"].Value = _LogInPack.PasswordHash;
            da.SelectCommand.Parameters("@PasswordHash").Value = _LogInPack.PasswordHashINT

            da.Fill(ds)

            rslt.CustomerDatabase = ds.Tables(0).Rows(0)("CnxnString")

            rslt.LoginID = ds.Tables(1).Rows(0)("LoginID")

            If rslt.LoginID > 0 Then
                rslt.IsLoginEmailFound = True
                Dim Pwd_onDB As Integer = ds.Tables(1).Rows(0)("PasswordHash")
                Dim lstSltBA = ds.Tables(1).Rows(0)("LastSaltByteArray")
                If lstSltBA IsNot Nothing AndAlso Not IsDBNull(lstSltBA) Then
                    rslt.LastPwdSalt = lstSltBA
                    rslt.IsAuthenticated = rslt.Verify(Pwd_onDB.ToString, _LogInPack.PasswordHashINT, lstSltBA)
                Else
                    If Pwd_onDB = 0 Then
                        '    ChangePwd(New Password_Package With {.LogIn_Email = SharedMethods.EmailAddress_ToNormalized(_LogInPack.LogIn_Email), _
                        '                                         .PasswordHashINT = _LogInPack.PasswordHashINT})
                        rslt.IsAuthenticated = True
                    End If

                    '    rslt.LastPwdSalt = Nothing 'could generate a new one here...
                End If
                If Not rslt.IsAuthenticated Then
                    rslt.PwdDB = Pwd_onDB
                    rslt.LastPwdSalt = lstSltBA
                End If
            End If

            Dim svcsrows = From dr In ds.Tables(3).DefaultView _
                           Let WCFSvcID = dr("WCFServiceID") _
                           Let Name = dr("Name") _
                           Let Cntrct = dr("Contract") _
                           Select WCFSvcID, Name, Cntrct
            rslt.Services = svcsrows.AsQueryable 'ds.Tables(3).Clone()
            conn.Close()
            conn.Dispose()
            da.Dispose()
            ds.Dispose()
            conn = Nothing
            da = Nothing
            ds = Nothing
        Catch ex As Exception
            'SharedEvents.RaiseOperationFailed("GlobalSurveyMasterDBSvc", "GlobalSurveyMasterDBSvc.GetGlobalMasterLoginDataByLoginName..." & ex.Message)
            Using EvLog As New EventLog()
                EvLog.Source = "GlobalSurveyMasterDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("GetGlobalMasterLoginDataByLoginName " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

        Return rslt
    End Function
#End Region


#Region "TestingMEthods"
    Private Function GetTemporaryCnxnString() As String
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim cnxnstring = config.ConnectionStrings.ConnectionStrings("SubscriberTest.MySettings.ExxonSurveyMasterConnectionString").ToString
        Return cnxnstring
    End Function
    Public Function GetLoginInfos() As List(Of LoginInfo)
        Return MyDB.LoginInfos.ToList
    End Function
    Public Function GetCustomers() As List(Of Customer)
        Return MyDB.Customers.ToList
    End Function
#End Region
#Region "Custom Events"
    Public Event GSM_DB_UnavailableEvent(ByVal sender As Object, ByVal e As EventArgs)
    Public Event GSM_DB_OperationFailed(ByVal sender As Object, ByVal e As EventArgs)
#End Region



    Private Sub DBOperationFailedHandler(sender As Object, e As EventArgs)
        Try
            Using EvLog As New EventLog()
                EvLog.Source = "GlobalSurveyMasterDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry(OperationFailedName & " " & MyException.Message, EventLogEntryType.Error)
            End Using
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "GlobalSurveyMasterDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry(OperationFailedName & " " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
    End Sub
    Private Sub DBUnavailableHandler(sender As Object, e As EventArgs)
        Try
            Using EvLog As New EventLog()
                EvLog.Source = "GlobalSurveyMasterDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("DBUnavailable " & sender.ToString & " " & Me.DC_ConnectionString, EventLogEntryType.Error)
            End Using
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "GlobalSurveyMasterDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("DBUnavailable " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Try
                    RemoveHandler Me.GSM_DB_UnavailableEvent, AddressOf DBUnavailableHandler
                    RemoveHandler Me.GSM_DB_OperationFailed, AddressOf DBOperationFailedHandler
                    If Not IsNothing(Me.MyDB) Then
                        Me.MyDB.Dispose()
                        Me.MyDB = Nothing
                    End If
                    Me.MyException = Nothing
                    Me.MyDB_TTL = Nothing
                    Me.DC_ConnectionString = Nothing
                Catch ex As Exception
                    Using EvLog As New EventLog()
                        EvLog.Source = "GlobalSurveyMasterDBSvc"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("Dispose " & ex.Message, EventLogEntryType.Error)
                    End Using
                End Try
                
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region


End Class

<DataContract()> _
Public Class GlobalMasterDataByLoginNameResult_Package
#Region "Fields...Shared"
    Private Shared ReadOnly Rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider
    Private Shared HA As SHA256Managed = New SHA256Managed
    Public Shared ReadOnly DefaultSaltSize As Integer = 16
#End Region

#Region "EncodeNew, Encode....are Shared/Static Methods"
    Public Shared Function EncodeNew(ByVal _pswd As String) As PWD_Pkg
        Dim rngba As Byte() = New Byte(DefaultSaltSize) {}
        Rng.GetBytes(rngba)
        Return Encode(_pswd, rngba)
    End Function

    Public Shared Function Encode(ByVal _pswd As String, ByVal _slt As Byte()) As PWD_Pkg
        Using ms As New System.IO.MemoryStream
            Dim pwd_in_bytearray = Text.Encoding.UTF8.GetBytes(_pswd)
            ms.Write(_slt, 0, _slt.Length)
            ms.Write(pwd_in_bytearray, 0, pwd_in_bytearray.Length)
            ms.Seek(0, IO.SeekOrigin.Begin)
            Return New PWD_Pkg(_slt, HA.ComputeHash(ms))
        End Using
    End Function
#End Region

#Region "Verify...is Instance method"
    Public Function Verify(ByVal _PwdOnDB As Integer, ByVal _loginPk_PwdINT As Integer, ByVal _LastSalt As Byte()) As Boolean
        Return Encode(_loginPk_PwdINT, _LastSalt).PwdHashINT = _PwdOnDB
    End Function
#End Region

#Region "Internal Class PWD_Pkg"
    Public Class PWD_Pkg
#Region "Fields and New"
        Public ReadOnly Salt As Byte()
        Public ReadOnly PassWordHash As Byte()

        Public Sub New(ByVal _Salt As Byte(), ByVal _PwdHash As Byte())
            Me.Salt = _Salt
            Me.PassWordHash = _PwdHash
        End Sub
#End Region

#Region "PwdHashINT"
        Public ReadOnly Property PwdHashINT() As Integer
            Get
                Return BitConverter.ToInt32(PassWordHash, 0)
            End Get
        End Property
#End Region

    End Class
#End Region

#Region "Properties"
    Public Property LastPwdSalt As Byte()

    Private _customerDatabase As String
    ''' <summary>
    ''' ConnectionString for CustomerSurveyMaster, e.g. ExxonSurveyMaster connection string
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()> _
    Public Property CustomerDatabase() As String
        Get
            Return _customerDatabase
        End Get
        Set(ByVal value As String)
            _customerDatabase = value
        End Set
    End Property

    Private _loginID As Integer = 0 '= (int)ds.Tables[1].Rows[0]["LoginID"];
    <DataMember()> _
    Public Property LoginID() As Integer
        Get
            Return _loginID
        End Get
        Set(ByVal value As Integer)
            _loginID = value
        End Set
    End Property

    Private _privilegeServiceMappings As IEnumerable ' = ds.Tables[2].Clone();
    '<DataMember()> _
    Public Property PrivilegeServiceMappings() As IEnumerable
        Get
            Return _privilegeServiceMappings
        End Get
        Set(ByVal value As IEnumerable)
            _privilegeServiceMappings = value
        End Set
    End Property

    Private _services As IEnumerable '= ds.Tables[3].Clone();
    '<DataMember()> _
    Public Property Services() As IEnumerable
        Get
            Return _services
        End Get
        Set(ByVal value As IEnumerable)
            _services = value
        End Set
    End Property

    Private _surveyDatabaseMappings ' = ds.Tables[4].Clone();
    '<DataMember()> _
    Public Property SurveyDatabaseMappings() As Object
        Get
            Return _surveyDatabaseMappings
        End Get
        Set(ByVal value As Object)
            _surveyDatabaseMappings = value
        End Set
    End Property

    Private _IsAuthenticated As Boolean = False
    <DataMember()> _
    Public Property IsAuthenticated() As Boolean
        Get
            Return _IsAuthenticated
        End Get
        Set(ByVal value As Boolean)
            _IsAuthenticated = value
        End Set
    End Property

    Private _IsLoginEmailFound As Boolean = False
    <DataMember()> _
    Public Property IsLoginEmailFound() As Boolean
        Get
            Return _IsLoginEmailFound
        End Get
        Set(ByVal value As Boolean)
            _IsLoginEmailFound = value
        End Set
    End Property

    Private _PwdDB As Integer
    Public Property PwdDB As Integer
        Get
            Return _PwdDB
        End Get
        Set(ByVal value As Integer)
            _PwdDB = value
        End Set
    End Property
#End Region
End Class

Public Class UserCacheManager
    Private Shared Dxnry As New Dictionary(Of String, UserCacheInfo)
    Public Shared Function IsLoggedIn(ByVal _userName As String) As Boolean
        Dim uci As UserCacheInfo = Nothing
        Dim rslt As Boolean = False
        SyncLock CType(Dxnry, ICollection).SyncRoot
            Dxnry.TryGetValue(_userName, uci)
        End SyncLock
        If uci IsNot Nothing AndAlso uci.IsOk Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Shared Function IsUserExists(ByVal _userName As String, ByVal _password As String) As Boolean
        Dim uci As UserCacheInfo = Nothing
        Dim rslt As Boolean = False
        SyncLock CType(Dxnry, ICollection).SyncRoot
            Dxnry.TryGetValue(_userName, uci)
        End SyncLock
        If uci IsNot Nothing Then
            rslt = uci.IsOk
            Dim pwdint As Integer
            If rslt Then
                If uci.IsLoggedIn Then

                    If Integer.TryParse(_password, pwdint) Then
                        If uci.PasswordInt = pwdint Then
                            rslt = True
                        Else
                            uci.Attempts += 1
                            rslt = False
                        End If
                    Else
                        If uci.PasswordInt = 0 Then
                            rslt = True
                        Else
                            SharedEvents.RaiseOperationFailed(_userName, "UserCacheManager.IsUserExists cannot parse password to integer")
                        End If
                    End If
                Else
                    uci.Attempts += 1
                End If
            Else
                If Not IsNothing(uci.GMDBPkg) Then
                    If Not IsNothing(uci.GMDBPkg.LastPwdSalt) Then
                        If uci.GMDBPkg.IsLoginEmailFound Then
                            'have to allow the first cut at creating uci be invalid...and then if subsequent attempts to login
                            'provide the right password, have to allow the login..
                            'as it is now, if first attempt is bad, uci is always bad...and never get to login...
                            If Integer.TryParse(_password, pwdint) Then
                                rslt = uci.GMDBPkg.Verify(uci.GMDBPkg.PwdDB.ToString, pwdint, uci.GMDBPkg.LastPwdSalt)
                                If rslt Then
                                    uci.GMDBPkg.PwdDB = -1
                                    uci.GMDBPkg.IsAuthenticated = True
                                End If
                            End If


                        End If
                    End If

                End If

            End If
            Return rslt
        Else
            Return AddUser(_userName, _password)
        End If
    End Function

    Public Shared Function AddUser(ByVal _userName As String, ByVal _password As String) As Boolean
        Dim raisopfailed As Boolean = False
        SyncLock CType(Dxnry, ICollection).SyncRoot
            Dim uci As UserCacheInfo = Nothing
            Dxnry.TryGetValue(_userName, uci)
            If uci Is Nothing Then
                uci = New UserCacheInfo(_userName, _password)
                Dxnry.Add(_userName, uci)
                Return uci.GMDBPkg.IsAuthenticated
            Else
                raisopfailed = True
            End If
        End SyncLock
        If raisopfailed Then
            SharedEvents.RaiseOperationFailed(_userName, "UserCacheManager.AddUser...already in Dxnry")
            Return False
        End If
    End Function

    Public Shared Function GetGMDbpkg(ByVal _username As String) As UserCacheInfo
        Dim uci As UserCacheInfo = Nothing
        SyncLock CType(Dxnry, ICollection).SyncRoot
            Dxnry.TryGetValue(_username, uci)
        End SyncLock
        Return uci
    End Function

    Public Shared GSMDbSvc As New GlobalSurveyMasterDBSvc

    Public Shared Sub PopulateLoginIsSuccess(ByVal _userName As String, ByVal _pwdint As Integer, ByVal _loginIsSuccess As Boolean, ByVal _restrict As Boolean)
        Dim uci As UserCacheInfo = Nothing
        SyncLock CType(Dxnry, ICollection).SyncRoot
            Dxnry.TryGetValue(_userName, uci)
        End SyncLock
        If uci IsNot Nothing Then
            uci.IsLoggedIn = _loginIsSuccess
            uci.IsFailedLogin = Not _loginIsSuccess
            uci.IsRestricted = _restrict
            'uci.Attempts += 1
            If uci.IsLoggedIn Then
                uci.ExpiresAt.AddMinutes(30)
                uci.PasswordInt = _pwdint
                'uci.GMDBPkg = Nothing
            ElseIf _restrict Then
                uci.GMDBPkg = Nothing
            End If

        Else
            SharedEvents.RaiseOperationFailed(_userName, "UserCacheManager.PopulateLoginIsSucess...No ucinfo")
        End If
    End Sub

    Public Shared Function RetrieveListofUserCacheInfo() As List(Of UserCacheInfo)
        Return Dxnry.Values.ToList
    End Function

    Public Shared Function RetrieveUserCacheInfo(ByVal _DxnryKey As String) As List(Of UserCacheInfo)
        Dim rslt As New List(Of UserCacheInfo)
        Try
            Dim uci As UserCacheInfo = Nothing
            SyncLock CType(Dxnry, ICollection).SyncRoot
                If Dxnry.TryGetValue(_DxnryKey, uci) Then
                    rslt.Add(uci)
                End If
            End SyncLock
        Catch ex As Exception
            SharedEvents.RaiseOperationFailed(_DxnryKey, "UserCacheManager.RetrieveUserCacheInfo..." & ex.Message)
        End Try

        Return rslt
    End Function

    Public Shared Sub RemoveUserCacheInfo(ByVal _DxnryKey As String)
        Try
            SyncLock CType(Dxnry, ICollection).SyncRoot
                Dim uci As UserCacheInfo = Nothing
                Dxnry.TryGetValue(_DxnryKey, uci)
                If Not IsNothing(uci) Then
                    Dxnry.Remove(_DxnryKey)
                End If
            End SyncLock
        Catch ex As Exception
            SharedEvents.RaiseOperationFailed(_DxnryKey, "UserCacheManager.RemoveUserCacheInfo...not in Dxnry")
        End Try

    End Sub

End Class

<DataContract()> _
Public Class UserCacheInfo

    Public Sub New(ByVal _username As String, ByVal _password As String)
        Me.ExpiresAt = Date.Now.AddMinutes(1)
        Me.DxnryKey = _username
        Dim trypwdINT As Integer = -1
        If Integer.TryParse(_password, trypwdINT) Then
            Me.GMDBPkg = Me.GetGlobalMasterData(New LogInPackage With {.LogIn_Email = _username, _
                                                     .PasswordHashINT = trypwdINT})
        End If

    End Sub
    Private Function GetGlobalMasterData(ByVal _LogInPack As LogInPackage) As GlobalMasterDataByLoginNameResult_Package
        'Dim myGSMDBMgr As New GlobalSurveyMasterDBSvc 'MasterSurveyDBSvc
        'Dim connStrings = ConfigurationManager.ConnectionStrings
        'myGSMDBMgr.DC_ConnectionString = connStrings("SurveyMasterConnectionString").ConnectionString

        Dim rslt = UserCacheManager.GSMDbSvc.GetGlobalMasterLoginDataByLoginName(_LogInPack)
        ' myGSMDBMgr = Nothing
        Return rslt
    End Function
    Public Function IsOk() As Boolean
        Return Me.IsLoggedIn And (Not Me.IsRestricted) And (Me.Attempts < 5) 'And (Me.ExpiresAt.CompareTo(Date.Now) > 0)
    End Function
    <DataMember()> _
    Public Property GMDBPkg As GlobalMasterDataByLoginNameResult_Package = Nothing
    <DataMember()> _
    Public Property Attempts As Integer = 0
    <DataMember()> _
    Public Property ExpiresAt As Date
    <DataMember()> _
    Public Property PasswordInt As Integer
    <DataMember()> _
    Public Property IsLoggedIn As Boolean = False
    <DataMember()> _
    Public Property IsFailedLogin As Boolean = False
    <DataMember()> _
    Public Property IsRestricted As Boolean = False
    <DataMember()> _
    Public Property DxnryKey As String
End Class