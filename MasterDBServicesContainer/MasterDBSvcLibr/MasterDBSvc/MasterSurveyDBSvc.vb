Imports System.Data.Linq
Imports IMasterSurveyDBSvcNS
Imports ILogInSvc
Imports CmdSvcClassLibrary
Imports CmdInfrastructureNS
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Text

Public Class MasterSurveyDBSvc
    Implements IMasterSurveyDBSvc, IDisposable


    Public DC_ConnectionString As String
    Private MyCSH As CmdSvcClassLibrary.CustomSvcHost = Nothing 'CType(OperationContext.Current.Host, CmdSvcClassLibrary.CustomSvcHost)
    Private InstanceTracker As CmdSvcClassLibrary.InstanceTracker '(MyCSH)
#Region "GlobalSurveyMaster Methods"
    'Public Function WithLinqStoredProcedure(ByVal _LogInPack As LogInPackage) As GlobalMasterDataByLoginNameResult_Package
    '    Dim rslt As New GlobalMasterDataByLoginNameResult_Package
    '    Dim db As New L2S_MasterSurveyDBDataContext(DC_ConnectionString)
    '    Dim x = db.proc_GetGlobalMasterLoginDataByLoginName(_LogInPack.LogIn_Email, _LogInPack.PasswordHashINT)

    '    Return rslt
    'End Function
    Public Function GetGlobalMasterLoginDataByLoginName(ByVal _LogInPack As LogInPackage) As GlobalMasterDataByLoginNameResult_Package
        Dim rslt As New GlobalMasterDataByLoginNameResult_Package
        Dim loginpackemail As String = ""
        'Dim connStrings = ConfigurationManager.ConnectionStrings
        Dim conn = New SqlConnection(DC_ConnectionString) '(connStrings("SurveyMasterConnectionString").ConnectionString)
        Dim da = New SqlDataAdapter("proc_GetGlobalMasterLoginDataByLoginName", conn)
        Try
            loginpackemail = _LogInPack.LogIn_Email
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Add("@LoginEMail", System.Data.SqlDbType.VarChar)
            da.SelectCommand.Parameters("@LoginEMail").Value = _LogInPack.LogIn_Email
            da.SelectCommand.Parameters.Add("@PasswordHash", System.Data.SqlDbType.Int)
            ' TODO: Pull the integer value out that we need
            'da.SelectCommand.Parameters["@PasswordHash"].Value = _LogInPack.PasswordHash;
            da.SelectCommand.Parameters("@PasswordHash").Value = _LogInPack.PasswordHashINT
            Dim ds = New DataSet()
            da.Fill(ds)

            ' TODO: Change this to be the connection string itself when this gets stored in the DB
            'Dim sb = New StringBuilder()
            'sb.Append("Data Source=.\DEVRENTS;Database=")
            'sb.Append(ds.Tables(0).Rows(0)("DatabaseName").ToString())
            'sb.Append(";Integrated Security=True;Connect Timeout=30;User Instance=False")
            rslt.CustomerDatabase = ds.Tables(0).Rows(0)("CnxnString") 'sb.ToString()

            ' sb = Nothing
            rslt.LoginID = ds.Tables(1).Rows(0)("LoginID")
            If rslt.LoginID > 0 Then
                rslt.IsAuthenticated = True
                rslt.IsLoginEmailFound = True
            End If

            'Dim psm = From dr In ds.Tables(2).DefaultView _
            '          Let PrivId = dr("PrivilegeID") _
            '          Let WCFSvcID = dr("WCFServiceID") _
            '          Select PrivId, WCFSvcID
            'rslt.PrivilegeServiceMappings = psm 'ds.Tables(2).Clone()

            Dim svcsrows = From dr In ds.Tables(3).DefaultView _
                           Let WCFSvcID = dr("WCFServiceID") _
                           Let Name = dr("Name") _
                           Let Cntrct = dr("Contract") _
                           Select WCFSvcID, Name, Cntrct
            rslt.Services = svcsrows.AsQueryable 'ds.Tables(3).Clone()

            'rslt.SurveyDatabaseMappings = ds.Tables(4).Clone()
            ds.Dispose()
            ds = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "MasterSurveyDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("GetGlobalMasterLoginDataByLoginName : Exception " & loginpackemail & " " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Try
                conn.Close()
                conn.Dispose()
                da.Dispose()
                conn = Nothing
                da = Nothing
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "MasterSurveyDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("GetGlobalMasterLoginDataByLoginName.Finally : Exception " & loginpackemail & " " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
            
        End Try
        Return rslt
    End Function
    'Public Function GetGlobalMasterLoginDataByLoginEmail(ByVal _LogIn_Email As String) As GlobalMasterDataByLoginNameResult_Package
    '    Dim rslt As New GlobalMasterDataByLoginNameResult_Package

    '    'Dim connStrings = ConfigurationManager.ConnectionStrings
    '    Dim conn = New SqlConnection(DC_ConnectionString) '(connStrings("SurveyMasterConnectionString").ConnectionString)
    '    Dim da = New SqlDataAdapter("proc_GetGlobalMasterLoginDataByLoginName", conn)
    '    da.SelectCommand.CommandType = CommandType.StoredProcedure
    '    da.SelectCommand.Parameters.Add("@LoginEMail", System.Data.SqlDbType.VarChar)
    '    da.SelectCommand.Parameters("@LoginEMail").Value = _LogIn_Email
    '    da.SelectCommand.Parameters.Add("@PasswordHash", System.Data.SqlDbType.Int)
    '    ' TODO: Pull the integer value out that we need
    '    'da.SelectCommand.Parameters["@PasswordHash"].Value = _LogInPack.PasswordHash;
    '    da.SelectCommand.Parameters("@PasswordHash").Value = 0
    '    Dim ds = New DataSet()
    '    da.Fill(ds)

    '    ' TODO: Change this to be the connection string itself when this gets stored in the DB
    '    Dim sb = New StringBuilder()
    '    sb.Append("Data Source=.\DEVRENTS;Database=")
    '    sb.Append(ds.Tables(0).Rows(0)("DatabaseName").ToString())
    '    sb.Append(";Integrated Security=True;Connect Timeout=30;User Instance=False")
    '    rslt.CustomerDatabase = sb.ToString()

    '    sb = Nothing
    '    rslt.LoginID = ds.Tables(1).Rows(0)("LoginID")
    '    If rslt.LoginID > 0 Then
    '        rslt.IsAuthenticated = True
    '        rslt.IsLoginEmailFound = True
    '    End If

    '    'Dim psm = From dr In ds.Tables(2).DefaultView _
    '    '          Let PrivId = dr("PrivilegeID") _
    '    '          Let WCFSvcID = dr("WCFServiceID") _
    '    '          Select PrivId, WCFSvcID
    '    'rslt.PrivilegeServiceMappings = psm 'ds.Tables(2).Clone()

    '    Dim svcsrows = From dr In ds.Tables(3).DefaultView _
    '                   Let WCFSvcID = dr("WCFServiceID") _
    '                   Let Name = dr("Name") _
    '                   Let Cntrct = dr("Contract") _
    '                   Select WCFSvcID, Name, Cntrct
    '    rslt.Services = svcsrows.AsQueryable 'ds.Tables(3).Clone()

    '    'rslt.SurveyDatabaseMappings = ds.Tables(4).Clone()

    '    conn.Close()
    '    conn.Dispose()
    '    da.Dispose()
    '    ds.Dispose()
    '    conn = Nothing
    '    da = Nothing
    '    ds = Nothing
    '    Return rslt
    'End Function


#End Region

#Region "Method Called By ResponseDispatcher to GetPostingClient"
    Function TinySurveyRow_UsingLogInEmail(ByVal _LoginEmail As String) As Tiny_Survey_Row Implements IMasterSurveyDBSvc.TinySurveyRow_UsingLogInEmail
        Dim rslt As Tiny_Survey_Row = Nothing
        'InstanceTracker.TrackMethod("TinySurveyRow_UsingLogInEmai LoginEmail=<" & _LoginEmail & "> ", 0)
        'Dim db As New L2S_MasterSurveyDBDataContext 'context should always be set to GlobalSurveyMaster...
        Dim db As New L2S_MasterSurveyDBDataContext(DC_ConnectionString)
        Try
            Dim q = From li In db.LoginInfos, cu In db.Customers, sds In db.SurveyDataStores _
                    Where li.LoginEmail = _LoginEmail AndAlso cu.CustomerID = li.CustomerID _
                    AndAlso sds.SurveyDataStoreID = cu.RDENTQueueURI_ID _
                    Select New Tiny_Survey_Row With {.ComputerID = sds.ComputerID, _
                                                     .QueueName = sds.DatabaseName, _
                                                     .QueueUri = sds.AbsolutePath, _
                                                     .SurveyID = 0, _
                                                     .SurveyName = "NotSet"}
            rslt = q.Single
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "MasterSurveyDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("TinySurveyRow_UsingLogInEmail : Exception " & _LoginEmail & " " & ex.Message, EventLogEntryType.Error)
            End Using
            SharedEvents.RaiseOperationFailed("could not find email in SurveyMaster " & _LoginEmail, "MasterDBSvc.TinySurveyRow_UsingLoginEmail")
            'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP TinySurveyRow_UsingLogInEmail" & ex.Message, _
            '       "LoginEmail=<" & _LoginEmail & "> " & DateTime.Now.ToLongTimeString)
        End Try
        'MyCSH.SvcMonitor.Update_ServiceCalls("TinySurveyRow_UsingLogInEmail" & "LoginEmail=<" & _LoginEmail & "> ", _
        'DateTime.Now.ToLongTimeString)
        Return rslt
    End Function
#End Region

    'Public Function ChangePwd(ByVal _loginpkg As LogInPackage, ByVal _oldPwdHash As Integer) As Boolean
    '    Dim rslt As Boolean = False
    '    Try
    '        Dim db As New L2S_MasterSurveyDBDataContext(DC_ConnectionString)
    '        Dim linfo = db.LoginInfos.Where(Function(li) li.LoginEmail = _loginpkg.LogIn_Email).FirstOrDefault
    '        linfo.PasswordHash = _loginpkg.LogIn_Email
    '        linfo.PasswordLastSetDate = Date.Now
    '        db.SubmitChanges()
    '        rslt = True
    '    Catch ex As Exception
    '        SharedEvents.RaiseOperationFailed(_loginpkg, "ChangePwd")
    '    End Try
    '    Return rslt
    'End Function



#Region "Add New Customer with LoginInfo"
    ''' <summary>
    ''' This Inserts Customer and LoginInfo rows in the GlobalSurveyMaster database...is part of the SignMeUp process.
    ''' </summary>
    ''' <param name="_CustomerName"></param>
    ''' <param name="_EmailAddress">NormalizedEmailAddress</param>
    ''' <param name="_SurvMstrName">ShortName of the CustomerSurveyMasterDatabase...eg, Exxon_SurveyMaster</param>
    ''' <param name="_DataStoreName">ShortName of the CustomerSurveyDataStore...eg, Exxon_SurveyDataStore_0</param>
    ''' <param name="_CustSurvMstrCnxnString">SqlConnection.Connection string to CustomerSurveyMasterDatabase</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddNewCustomer_and_LoginInfo(ByVal _CustomerName As String, _
                                                 ByVal _EmailAddress As String, _
                                                 ByVal _SurvMstrName As String, _
                                                 ByVal _DataStoreName As String, _
                                                 ByVal _CustSurvMstrCnxnString As String, ByVal _SgnUpPkg As SignUpPackage) As SignUpPackage
        Dim rslt As SignUpPackage = _SgnUpPkg
        Try
            Dim CustId As Integer = AddCustomer(_CustomerName, _SurvMstrName, _DataStoreName, _CustSurvMstrCnxnString)
            If CustId > 0 Then
                Dim loginID As Integer = AddLoginInfo(_EmailAddress, CustId)
                rslt.GSMLoginID = loginID
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "MasterSurveyDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("AddNewCustomer_and_LoginInfo : Exception " & _EmailAddress & " " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

        Return rslt
    End Function
#End Region

#Region "Add LoginInfo to LoginInfo Table"
    Public Function AddLoginInfo(ByVal _NormalizedEmailAddress As String, ByVal _CustomerID As Integer) As Integer
        Dim rslt As Integer = Nothing
        Try
            Dim db As New L2S_MasterSurveyDBDataContext(DC_ConnectionString)
            Dim newlogin As New LoginInfo With {.CustomerID = _CustomerID, _
                                                .LoginEmail = _NormalizedEmailAddress, _
                                                .PasswordHash = 0}
            db.LoginInfos.InsertOnSubmit(newlogin)
            db.SubmitChanges()
            rslt = newlogin.LogInID
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "MasterSurveyDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("AddLoginInfo : Exception " & _NormalizedEmailAddress & " " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function
#End Region
#Region "AddCustomer to Customer Table"
    Public Function AddCustomer(ByVal _CustomerName As String, _
                                ByVal _SurvMstrName As String, _
                                ByVal _DataStoreName As String, _
                                ByVal _CustSurvMstrCnxnString As String) As Integer
        Dim rslt As Integer = Nothing
        Dim db As New L2S_MasterSurveyDBDataContext(DC_ConnectionString)
        Dim newSDSrow As New SurveyDataStore With {.AbsolutePath = "PAth...should be ConnectionString to xxxCustomerSurveyMAster...", _
                                                   .ComputerID = 0, _
                                                   .DatabaseName = _SurvMstrName}
        db.SurveyDataStores.InsertOnSubmit(newSDSrow)
        Dim newCustrow As New Customer With {.CustomerName = _CustomerName, _
                                          .CustomerSurveyMasterID = newSDSrow.SurveyDataStoreID} 'CustomerSurveyMasterID is the ID of a GlobalSurveyMaster.SurveyDataStore Row...that points to the
        'connectionstring/location of the "ExxonSurveyMaster" database....the SurveyDataStore row also has the databaseName in it...
        db.Customers.InsertOnSubmit(newCustrow)
        Try
            db.SubmitChanges()
            rslt = newCustrow.CustomerID
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "MasterSurveyDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("AddCustomer : Exception " & _CustomerName & " " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try


        Return rslt
    End Function
#End Region



#Region "OLD METHODS"



    'Public Function AddSurveyRow(ByVal _SurveyMaster As SurveyMaster) As Integer Implements IMasterSurveyDBSvc.AddSurveyRow

    '    Dim rslt As Integer = -1
    '    Dim db As New L2S_MasterSurveyDBDataContext
    '    ServiceMonitor.Update_ServiceCalls("AddSurveyRow  " & DateTime.Now.ToLongTimeString, _
    '                                       "SurveyID=<" & _SurveyMaster.SurveyID.ToString & ">")
    '    Try
    '        db.SurveyMasters.InsertOnSubmit(_SurveyMaster)
    '        db.SubmitChanges()
    '        rslt = _SurveyMaster.SurveyID
    '        InstanceTracker.TrackMethod("AddSurveyRow", _SurveyMaster.SurveyID)

    '    Catch ex As Exception
    '        ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.AddSurveyRow reports exception..." & DateTime.Now.ToLongTimeString, _
    '                                           "SurveyRow(SurveyID=<" & _SurveyMaster.SurveyID.ToString & ">)" & ex.Message)
    '    End Try
    '    'Try
    '    '    db.Dispose()
    '    'Catch ex As Exception
    '    '    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.AddSurveyRow Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, "SurveyRow(SurveyID=<" & _SurveyMaster.SurveyID.ToString & ">)" & ex.Message)
    '    db = Nothing
    '    'End Try

    '    Return rslt
    'End Function

    'Public Function UpdateSurveyRow(ByVal _SurveyMaster As SurveyMaster) As Boolean Implements IMasterSurveyDBSvc.UpdateSurveyRow
    '    InstanceTracker.TrackMethod("UpdateSurveyRow", _SurveyMaster.SurveyID)

    '    Dim rslt As Boolean = False
    '    Dim db As New L2S_MasterSurveyDBDataContext
    '    ServiceMonitor.Update_ServiceCalls("UpdateSurveyRow  " & DateTime.Now.ToLongTimeString, _
    '                                       "SurveyID=<" & _SurveyMaster.SurveyID.ToString & ">")
    '    Try
    '        Dim _SurveyID = _SurveyMaster.SurveyID
    '        Dim q = From si In db.SurveyMasters _
    '                Where si.SurveyID = _SurveyID _
    '                Select si
    '        Dim dr = q.Single
    '        db.SubmitChanges()
    '        rslt = True
    '    Catch ex As Exception
    '        ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.UpdateSurveyRow reports more than one...  " & DateTime.Now.ToLongTimeString, "SurveyRow(SurveyID=<" & _SurveyMaster.SurveyID.ToString & ">)")
    '    End Try
    '    'Try
    '    '    db.Dispose()
    '    'Catch ex As Exception
    '    '    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.UpdateSurveyRow Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, "SurveyRow(SurveyID=<" & _SurveyMaster.SurveyID.ToString & ">)" & ex.Message)
    '    db = Nothing
    '    'End Try
    '    Return rslt
    'End Function

    'Public Function RemoveSurveyRow(ByVal _SurveyID As Integer) As Boolean Implements IMasterSurveyDBSvc.RemoveSurveyRow
    '    InstanceTracker.TrackMethod("RemoveSurveyRow", _SurveyID)

    '    Dim rslt As Boolean = False
    '    Dim db As New L2S_MasterSurveyDBDataContext
    '    ServiceMonitor.Update_ServiceCalls("RemoveSurveyRow  " & DateTime.Now.ToLongTimeString, _
    '                                      "SurveyID=<" & _SurveyID.ToString & ">")
    '    Try
    '        Dim q = From si In db.SurveyMasters _
    '            Where si.SurveyID = _SurveyID _
    '            Select si
    '        Dim dr = q.Single
    '        db.SurveyMasters.DeleteOnSubmit(dr)
    '        db.SubmitChanges()
    '        rslt = True
    '    Catch ex As Exception
    '        ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.RemoveSurveyRow reports more than one...  " & DateTime.Now.ToLongTimeString, _
    '                                           "SurveyRow(SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)
    '    End Try
    '    'Try
    '    '    db.Dispose()
    '    'Catch ex As Exception
    '    '    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.RemoveSurveyRow Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, _
    '    '                                       "SurveyRow(SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)
    '    db = Nothing
    '    'End Try
    '    Return rslt
    'End Function

    'Public Function RetrieveSurveyRow_WithSurveyID(ByVal _SurveyID As Integer) As SurveyMaster Implements IMasterSurveyDBSvc.RetrieveSurveyRow_WithSurveyID
    '    Dim rslt As SurveyMaster = Nothing
    '    Dim db As New L2S_MasterSurveyDBDataContext
    '    'Dim dlo As New DataLoadOptions
    '    'dlo.LoadWith(Of SurveyMaster)(Function(sm As SurveyMaster) sm.SurveyID = sm.SurveyID)
    '    'db.LoadOptions = Nothing
    '    Try
    '        ServiceMonitor.Update_ServiceCalls("RetrieveSurveyRow_WithSurveyID" & DateTime.Now.ToLongTimeString, _
    '                                           "SurveyID=<" & _SurveyID.ToString & ">)")
    '        Dim q = From si In db.SurveyMasters _
    '            Where si.SurveyID = _SurveyID _
    '            Select si
    '        q.DefaultIfEmpty(rslt)
    '        rslt = q.Single
    '        ServiceMonitor.Update_ServiceCalls("Finished RetrieveSurveyRow_WithSurveyID" & DateTime.Now.ToLongTimeString, _
    '                                          "rslt.SurveyID=<" & rslt.SurveyID.ToString & ">)")
    '    Catch ex As Exception
    '        ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.RetrieveSurveyRow_WithSurveyID reports exception..." & DateTime.Now.ToLongTimeString, _
    '                                           "SurveyRow(SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)
    '    End Try
    '    'Try
    '    '    'db.Dispose()
    '    'Catch ex As Exception
    '    '    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.RetrieveSurveyRow_WithSurveyID Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, _
    '    '                                       "SurveyRow(SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)
    '    '    'db = Nothing
    '    'End Try
    '    ServiceMonitor.Update_ServiceCalls("<<ReturningResult RetrieveSurveyRow_WithSurveyID" & DateTime.Now.ToLongTimeString, _
    '                                          "rslt.SurveyID=<" & rslt.SurveyID.ToString & ">)")

    '    Return rslt
    'End Function

    'Public Function RetrieveSurveyRow_WithLoginId(ByVal _LoginId As Integer) As List(Of SurveyMaster) Implements IMasterSurveyDBSvc.RetrieveSurveyRow_WithLoginId
    '    Dim rslt As List(Of SurveyMaster) = Nothing
    '    Dim db As New L2S_MasterSurveyDBDataContext
    '    ServiceMonitor.Update_ServiceCalls("RetrieveSurveyRow_WithLoginId" & DateTime.Now.ToLongTimeString, _
    '                                          "LoginId =<" & _LoginId.ToString & ">)")
    '    Try
    '        ServiceMonitor.Update_ServiceCalls("RetrieveSurveyRow_WithLoginId  " & DateTime.Now.ToLongTimeString, _
    '                                           "LoginID=<" & _LoginId & ">")
    '        Dim q = From si In db.SurveyMasters _
    '            Where si.LoginID = _LoginId _
    '            Select si
    '        rslt = q.ToList
    '    Catch ex As Exception
    '        ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.RetrieveSurveyRow_WithLoginId reports exception..." & DateTime.Now.ToLongTimeString, _
    '                                           "SurveyRow(LoginId =<" & _LoginId.ToString & ">)" & ex.Message)
    '    End Try
    '    'Try
    '    '    db.Dispose()
    '    'Catch ex As Exception
    '    '    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.RetrieveSurveyRow_WithLoginId Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, _
    '    '                                       "SurveyRow(LoginId =<" & _LoginId.ToString & ">)" & ex.Message)
    '    '    db = Nothing
    '    'End Try
    '    Return rslt
    'End Function

    Public Function SurveyMetaData(ByVal _SurveyID As Integer) As SurveyMetadataPackage Implements IMasterSurveyDBSvc.SurveyMetaData
        InstanceTracker.TrackMethod("SurveyMetaData", _SurveyID)

        Dim rslt As SurveyMetadataPackage = Nothing
        Dim db As New L2S_MasterSurveyDBDataContext
        MyCSH.SvcMonitor.Update_ServiceCalls("SurveyMetaData" & DateTime.Now.ToLongTimeString, _
                                             "SurveyID=<" & _SurveyID.ToString & ">)")
        'use reflection.propertyinfor where type is datetime...
        'put the property name, and the value in a cmdSvcLibr.srlzd_KVP...
        'Try
        '    db.Dispose()
        'Catch ex As Exception
        '    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.SurveyMetaData Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, _
        '                                       "SurveyRow(SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)
        db = Nothing
        'End Try
        Return rslt
    End Function

    Public Function SurveyMetaData_List(ByVal _LoginID As Integer) As System.Collections.Generic.List(Of SurveyMetadataPackage) Implements IMasterSurveyDBSvc.SurveyMetaData_List
        InstanceTracker.TrackMethod("SurveyMetaData_List LoginID", 0)

        Dim rslt As List(Of SurveyMetadataPackage) = Nothing
        Dim db As New L2S_MasterSurveyDBDataContext
        MyCSH.SvcMonitor.Update_ServiceCalls("SurveyMetaData" & DateTime.Now.ToLongTimeString, _
                                            "LoginID=<" & _LoginID.ToString & ">)")
        Try
            'use reflection.propertyinfor where type is datetime...
            'put the property name, and the value in a cmdSvcLibr.srlzd_KVP...
        Catch ex As Exception

        End Try

        'Try
        '    db.Dispose()
        'Catch ex As Exception
        '    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.SurveyMetaData Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, _
        '                                       "LoginID=<" & _LoginID.ToString & ">)" & ex.Message)
        db = Nothing
        'End Try
        Return rslt
    End Function

    Public Function Tiny_Survey_Row(ByVal _SurveyID As Integer) As System.Collections.Generic.List(Of Tiny_Survey_Row) Implements IMasterSurveyDBSvc.Tiny_Survey_Row
        InstanceTracker.TrackMethod("Tiny_Survey_Row", _SurveyID)

        Dim rslt As List(Of Tiny_Survey_Row) = Nothing
        'Dim db As New L2S_MasterSurveyDBDataContext
        'MyCSH.SvcMonitor.Update_ServiceCalls("Tiny_Survey_Row" & DateTime.Now.ToLongTimeString, _
        '                                      "SurveyID=<" & _SurveyID.ToString & ">)")
        'Dim dlo As New DataLoadOptions
        'dlo.LoadWith(Of SurveyDataStore)(Function(sd As SurveyDataStore) sd.SurveyMasters)
        'db.LoadOptions = dlo
        'Try
        '    Dim q = From sds In db.SurveyDataStores, si In sds.SurveyMasters _
        '            Where si.SurveyID = _SurveyID _
        '            Select New Tiny_Survey_Row With {.ComputerID = sds.ComputerID, _
        '                                       .SurveyDataStoreID = si.SurveyDataStoreID, _
        '                                       .SurveyID = si.SurveyID, _
        '                                       .SurveyName = si.SurveyDescription, _
        '                                       .QueueName = si.QueueName, _
        '                                       .QueueUri = si.QueueURI}
        '    rslt = q.ToList
        'Catch ex As Exception
        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.Tiny_Survey_Row reports exception..." & DateTime.Now.ToLongTimeString, _
        '                                       "SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)

        'End Try

        'Try
        '    db.Dispose()
        'Catch ex As Exception
        '    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.RetrieveSurveyRow Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, _
        '                                       "SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)
        'db = Nothing
        'End Try
        Return rslt
    End Function

    Public Function Tiny_Survey_Row_List(ByVal _LoginID As Integer) As System.Collections.Generic.List(Of Tiny_Survey_Row) Implements IMasterSurveyDBSvc.Tiny_Survey_Row_List
        InstanceTracker.TrackMethod("Tiny_Survey_Row_List LoginID", 0)

        Dim rslt As List(Of Tiny_Survey_Row) = Nothing
        'Dim db As New L2S_MasterSurveyDBDataContext
        'Try
        '    Dim q = From sds In db.SurveyDataStores, si In sds.SurveyMasters _
        '            Where si.LoginID = _LoginID _
        '            Select New Tiny_Survey_Row With {.ComputerID = sds.ComputerID, _
        '                                       .SurveyDataStoreID = si.SurveyDataStoreID, _
        '                                       .SurveyID = si.SurveyID, _
        '                                       .SurveyName = si.SurveyDescription}
        '    rslt = q.ToList
        'Catch ex As Exception
        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.Tiny_Survey_Row_List reports exception..." & DateTime.Now.ToLongTimeString, "LoginID=<" & _LoginID.ToString & ">)" & ex.Message)
        'End Try
        ''Try
        ''    db.Dispose()
        ''Catch ex As Exception
        ''    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.Tiny_Survey_Row_List Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, "LoginID=<" & _LoginID.ToString & ">)" & ex.Message)
        'db = Nothing
        'End Try
        Return rslt
    End Function

    Public Function Tiny_Survey_Row_WithSurveyItemModel(ByVal _SurveyID As Integer) As List(Of Tiny_Survey_RowWith_SIModel) Implements IMasterSurveyDBSvc.Tiny_Survey_Row_WithSurveyItemModel
        InstanceTracker.TrackMethod("Tiny_Survey_Row_WithSurveyItemModel", _SurveyID)

        Dim rslt As List(Of Tiny_Survey_RowWith_SIModel) = Nothing
        'Dim db As New L2S_MasterSurveyDBDataContext
        'MyCSH.SvcMonitor.Update_ServiceCalls("Tiny_Survey_Row_WithSurveyItemModel" & DateTime.Now.ToLongTimeString, _
        '                                      "SurveyID=<" & _SurveyID.ToString & ">)")
        'Try
        '    Dim q = From sds In db.SurveyDataStores, si In sds.SurveyMasters _
        '            Where si.SurveyID = _SurveyID _
        '            Select New Tiny_Survey_RowWith_SIModel With {.Model = si.Model, _
        '                                                    .TinyRow = New Tiny_Survey_Row With {.ComputerID = sds.ComputerID, _
        '                                                                                         .SurveyDataStoreID = si.SurveyDataStoreID, _
        '                                                                                         .SurveyID = si.SurveyID, _
        '                                                                                         .SurveyName = si.SurveyDescription}}
        '    rslt = q.ToList
        'Catch ex As Exception
        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.Tiny_Survey_Row_WithSurveyItemModel reports exception..." & DateTime.Now.ToLongTimeString, _
        '                                       "SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)
        'End Try

        ''Try
        ''    db.Dispose()
        ''Catch ex As Exception
        ''    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.Tiny_Survey_Row_WithSurveyItemModel_List Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, "SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)
        'db = Nothing
        ''End Try
        Return rslt
    End Function

    Public Function Tiny_Survey_Row_WithSurveyItemModel_List(ByVal _LoginID As Integer) As System.Collections.Generic.List(Of Tiny_Survey_RowWith_SIModel) Implements IMasterSurveyDBSvc.Tiny_Survey_Row_WithSurveyItemModel_List
        InstanceTracker.TrackMethod("Tiny_Survey_Row_WithSurveyItemModel_List  LoginID", 0)

        Dim rslt As List(Of Tiny_Survey_RowWith_SIModel) = Nothing
        'Dim db As New L2S_MasterSurveyDBDataContext
        'MyCSH.SvcMonitor.Update_ServiceCalls("Tiny_Survey_Row_WithSurveyItemModel_List" & DateTime.Now.ToLongTimeString, _
        '                                     "LoginID=<" & _LoginID.ToString & ">)")
        'Try
        '    Dim q = From sds In db.SurveyDataStores, si In sds.SurveyMasters _
        '            Where si.LoginID = _LoginID _
        '            Select New Tiny_Survey_RowWith_SIModel With {.Model = si.Model, _
        '                                                     .TinyRow = New Tiny_Survey_Row With {.ComputerID = sds.ComputerID, _
        '                                                                                          .SurveyDataStoreID = si.SurveyDataStoreID, _
        '                                                                                          .SurveyID = si.SurveyID, _
        '                                                                                          .SurveyName = si.SurveyDescription}}
        '    rslt = q.ToList
        'Catch ex As Exception
        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.Tiny_Survey_Row_WithSurveyItemModel_List reports exception..." & DateTime.Now.ToLongTimeString, _
        '                                       "LoginID=<" & _LoginID.ToString & ">)" & ex.Message)

        'End Try
        ''Try
        ''    db.Dispose()
        ''Catch ex As Exception
        ''    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.Tiny_Survey_Row_WithSurveyItemModel_List Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, _
        ''                                       "LoginID=<" & _LoginID.ToString & ">)" & ex.Message)
        'db = Nothing
        ''End Try
        Return rslt
    End Function

    Public Function Update_SurveyMaster_WithModel(ByVal _SurveyId As Integer, ByVal _Model As String) As Boolean Implements IMasterSurveyDBSvc.Update_SurveyMaster_WithModel
        InstanceTracker.TrackMethod("Update_SurveyMaster_WithModel  LoginID", 0)
        Dim rslt As Boolean = False
        Dim db As New L2S_MasterSurveyDBDataContext
        MyCSH.SvcMonitor.Update_ServiceCalls("Update_SurveyMaster_WithModel" & DateTime.Now.ToLongTimeString, _
                                            "SurveyId=<" & _SurveyId.ToString & ">)")
        Try
            Dim q = From si In db.SurveyMasters _
                    Where si.SurveyID = _SurveyId _
                    Select si
            Dim ur = q.Single
            ur.Model = _Model
            db.SubmitChanges()
        Catch ex As Exception
            MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.Update_SurveyMaster_WithModel reports exception..." & DateTime.Now.ToLongTimeString, _
                                               "SurveyID=<" & _SurveyId.ToString & ">)" & ex.Message)
        End Try

        'Try
        '    db.Dispose()
        'Catch ex As Exception
        '    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.Update_SurveyMaster_WithModel Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, _
        '                                       "SurveyID=<" & _SurveyId.ToString & ">)" & ex.Message)
        db = Nothing
        'End Try
        Return rslt
    End Function
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
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
