Imports ILogInSvc
Imports DataContextPackageNS
Imports WCFServiceManagerNS
Imports CmdInfrastructureNS
Imports CmdSvcClassLibrary
Imports MasterDBSvcLibr
Imports IEndPtDataCntxtSvcNS
Imports System.IO
Imports System.Xml
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Data.Linq
Imports System.ServiceModel.Configuration
Imports System.ServiceModel
Imports System.IdentityModel.Selectors


Public Class LogInSvc
    Implements ILogInSvc.ILogInSvc

    Private MyCSH As CustomSvcHost = CType(OperationContext.Current.Host, CustomSvcHost)
    'Private InstanceTracker As New CmdSvcClassLibrary.InstanceTracker(OperationContext.Current.Host)

    Private MyEptDCClient As IEndPtDataCntxtSvc

    'Private _loginEmail As String

    Dim MyGlobalMasterDataResult_Pkg As GlobalMasterDataByLoginNameResult_Package

    Private Sub DisposeMe()
        Try
            Me.MyGlobalMasterDataResult_Pkg = Nothing
            Me.MyEptDCClient = Nothing
            Me.MyCSH = Nothing
            If Not IsNothing(Me.MySubscriberOps) Then
                Me.MySubscriberOps.Dispose()
                Me.MySubscriberOps = Nothing
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "LogInSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("LogMeOutPlease:DisposeMe reports error... " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

    End Sub


#Region "EmailAddress ToNormalizedEmailAddress"

    ''' <summary>
    ''' This is the same function as in SqlServerOperations...it is duplicated here...converts an email address to something we can name databases with...
    ''' </summary>
    ''' <param name="_emailAddress"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EmailAddress_ToNormalizedEmailAddress(ByVal _emailAddress As String) As String
        Dim rslt As String = Nothing

        If Not String.IsNullOrEmpty(_emailAddress) Then
            Dim replaces = From c In _emailAddress.ToLowerInvariant.ToCharArray _
                           Where Not Char.IsLetterOrDigit(c) Or Char.IsPunctuation(c) Or Char.IsSymbol(c) _
                           Select c
            Dim rsltstring As String = _emailAddress
            For Each c In replaces
                Dim x = Char.ToString(c)
                rsltstring = Strings.Replace(rsltstring, Char.ToString(c), "_")
            Next

            If rsltstring.Length > 50 Then
                rslt = Left(rsltstring.ToLowerInvariant, 50)
            Else
                rslt = Left(rsltstring.ToLowerInvariant, rsltstring.Length)
            End If
        End If
        Return rslt
    End Function

#End Region

#Region "SignMeUpPlease"
    Private MySubscriberOps As SubscriberOps_NS.SubscriberOperations

    Public Function SignMeUpPlease(ByVal _SignUpPack As SignUpPackage) As SignUpResult Implements ILogInSvc.ILogInSvc.SignMeUpPlease
        Dim rslt As New SignUpResult With {.LogInRslt = Nothing}

        Dim signupemail As String = ""
        If Not IsNothing(_SignUpPack) Then
            Try
                MySubscriberOps = New SubscriberOps_NS.SubscriberOperations
                Dim nsp As New NewSubscriberPackage(_SignUpPack.LogInPkg.LogIn_Email, SubscriptionLevel.Basic, 1, _SignUpPack.OrgCompanyName)
                nsp.InitialPassword = _SignUpPack.LogInPkg.PasswordHashINT
                If MySubscriberOps.AddNewSubscriber(nsp) Then
                    UserCacheManager.IsUserExists(nsp.NormalizedEmailAddress, _SignUpPack.LogInPkg.PasswordHashINT)
                    rslt.LogInRslt = LogMeInPlease(New LogInPackage With {.LogIn_Email = _SignUpPack.LogInPkg.LogIn_Email, _
                                                                          .PasswordHashINT = _SignUpPack.LogInPkg.PasswordHashINT})
                    rslt.LogInRslt.SpiffList.Add(New Srlzd_KVP("Message", "Success... " & nsp.NormalizedEmailAddress & ", you are subscribed."))
                    Using EvLog As New EventLog()
                        EvLog.Source = "SignUPSvc"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("SignMeUpPlease: " & signupemail & " you are subscribed.", EventLogEntryType.SuccessAudit)
                    End Using
                Else
                    rslt.LogInRslt = New LogInResult With {.LogIn_IsSuccess = False, _
                                                           .EndpointKeysList = Nothing, _
                                                           .SpiffList = New List(Of Srlzd_KVP)}
                    rslt.LogInRslt.SpiffList.Add(New Srlzd_KVP("Message", "Sorry... " & nsp.NormalizedEmailAddress & ", unable to set up your subscription."))
                    Using EvLog As New EventLog()
                        EvLog.Source = "SignUPSvc"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("SignMeUpPlease: " & signupemail & " unable to set up  subscription", EventLogEntryType.FailureAudit)
                    End Using
                End If
                nsp = Nothing
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "SignUPSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("SignMeUpPlease: " & signupemail & " " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        Else
            signupemail = "Unknown"
            Using EvLog As New EventLog()
                EvLog.Source = "SignUPSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SignMeUpPlease: " & signupemail & "SignUpPack isNothing...unable to set up  subscription", EventLogEntryType.Error)
            End Using
            rslt.LogInRslt = New LogInResult With {.LogIn_IsSuccess = False, _
                                                          .EndpointKeysList = Nothing, _
                                                          .SpiffList = New List(Of Srlzd_KVP)}
            rslt.LogInRslt.SpiffList.Add(New Srlzd_KVP("Message", "Sorry...signup info is incomplete... unable to set up your subscription."))
        End If



        Return rslt
    End Function

#End Region


#Region "LogMeInPlease"
    Private PBMFromCustDBMgr As ULong = 0
    Public Function LogMeInPlease(ByVal _LogInPack As ILogInSvc.LogInPackage) As ILogInSvc.LogInResult Implements ILogInSvc.ILogInSvc.LogMeInPlease
        MyCSH.SvcMonitor.Update_ServiceCalls("RegisterIN" & DateTime.Now.ToLongDateString, _
                                          "LogIn_Email=<" & _LogInPack.LogIn_Email & ">")

        Dim LogInrslt As LogInResult = New LogInResult With {.LogIn_IsSuccess = False, _
                                                             .SpiffList = New List(Of Srlzd_KVP)}
        Try

            _LogInPack.LogIn_Email = SharedMethods.EmailAddress_ToNormalized(_LogInPack.LogIn_Email)
            MyGlobalMasterDataResult_Pkg = UserCacheManager.GetGMDbpkg(_LogInPack.LogIn_Email).GMDBPkg
            'MyGlobalMasterDataResult_Pkg = Me.GetGlobalMasterData(_LogInPack)
            If MyGlobalMasterDataResult_Pkg.IsAuthenticated Then
                LogInrslt.EndpointKeysList = Me.StartServices(_LogInPack)
                LogInrslt.SpiffList.Add(New Srlzd_KVP("Message", "Success..You have " & LogInrslt.EndpointKeysList.Count & " endpoint keys..."))
                LogInrslt.SpiffList.Add(New Srlzd_KVP("PBM", PBMFromCustDBMgr))
                LogInrslt.LogIn_IsSuccess = True
                UserCacheManager.PopulateLoginIsSuccess(_LogInPack.LogIn_Email, _LogInPack.PasswordHashINT, True, False)
            Else
                If MyGlobalMasterDataResult_Pkg.IsLoginEmailFound Then
                    UserCacheManager.PopulateLoginIsSuccess(_LogInPack.LogIn_Email, -1, False, False)
                    LogInrslt.SpiffList.Add(New Srlzd_KVP("Message", "The password is incorrect... "))
                Else
                    UserCacheManager.PopulateLoginIsSuccess(_LogInPack.LogIn_Email, -1, False, True)
                    LogInrslt.SpiffList.Add(New Srlzd_KVP("Message", "Could not find the Login Name entered... "))
                End If
            End If
        Catch ex As Exception
            UserCacheManager.PopulateLoginIsSuccess(_LogInPack.LogIn_Email, -1, False, False)
            LogInrslt.SpiffList.Add(New Srlzd_KVP("Message", "Failure..can't connect to Server..."))
            MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP IN  " & DateTime.Now.ToLongTimeString, _
                                                 "(LogIn_Email=<" & _LogInPack.LogIn_Email & ">" & ex.Message)
        End Try

        MyGlobalMasterDataResult_Pkg = Nothing
        MyCSH.SvcMonitor.Update_ServiceCalls(LogInrslt.LogIn_IsSuccess.ToString & " " & DateTime.Now.ToLongTimeString, _
                                             "(LogIn_Email=<" & _LogInPack.LogIn_Email & ">")
        Me.DisposeMe()
        Return LogInrslt
    End Function

#Region "Support Functions LogMeInPlease"
    Private Function GetGlobalMasterData(ByVal _LogInPack As LogInPackage) As GlobalMasterDataByLoginNameResult_Package
        Dim rslt As GlobalMasterDataByLoginNameResult_Package = Nothing
        If Not IsNothing(_LogInPack) Then
            Try
                Dim myGSMDBMgr As New GlobalSurveyMasterDBSvc 'MasterSurveyDBSvc
                rslt = myGSMDBMgr.GetGlobalMasterLoginDataByLoginName(_LogInPack)
                myGSMDBMgr.Dispose()
                myGSMDBMgr = Nothing
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "LogInSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("GetGlobalMasterData: reports error... " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        Else
            Using EvLog As New EventLog()
                EvLog.Source = "LogInSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("GetGlobalMasterData: _LogInPack isNothing...unable to set up  subscription", EventLogEntryType.Error)
            End Using
        End If
        Return rslt
    End Function

    Private Function StartServices(ByVal _loginPkg As LogInPackage) As List(Of EndPtPackage)
        Dim rslt As List(Of EndPtPackage) = Nothing
        If Not IsNothing(_loginPkg) Then
            Try
                Dim ServiceStartPackages = ServicesToStart(_loginPkg.LogIn_Email)
                rslt = WCFServiceManager.StartServicesV2(ServiceStartPackages)
                ServiceStartPackages.Clear()
                ServiceStartPackages = Nothing
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "LogInSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("StartServices: reports error... " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        Else
            Using EvLog As New EventLog()
                EvLog.Source = "LogInSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("StartServices: _loginPkg isNothing...unable to set up  subscription", EventLogEntryType.Error)
            End Using
        End If
        Return rslt
    End Function

    Private Function ServicesToStart(ByVal _LogIn_Email As String) As List(Of ServiceStartPackage)
        Dim rslt As New List(Of ServiceStartPackage)
        Try
            Dim MyCustomerDBMgr As New CustomerDBOperationsNS.Manager
            MyCustomerDBMgr.DC_ConnectionString = MyGlobalMasterDataResult_Pkg.CustomerDatabase

            Dim _loginID = MyGlobalMasterDataResult_Pkg.LoginID
            Dim SurveyPrivDCPkg = MyCustomerDBMgr.GetPrivServiceMappings(_loginID)

            Me.PBMFromCustDBMgr = MyCustomerDBMgr.PrivBitMask 'this is CustomerSurveyMaster.LoginInfo.PrivBitMask...third table in the storedproc...

            ' ValidServicesBitmask from config...describes Services Hosted by this WindowsServiceHostingContainer
            Dim ValidSvcsBmsk = Retrieve_ValidServicesBitmask()

            ' PermittedSvcsBitMsk is intersection of User'sPrivilegeServices and ValidSvcBmskSvcs
            Dim _PermittedSvcsBmsk As ULong = SurveyPrivDCPkg.ServiceEnumBitMsk And ValidSvcsBmsk

            Dim dcpkg As DC_Package = New DC_Package(_loginID, 3600, MyCustomerDBMgr.DC_ConnectionString)
            If dcpkg IsNot Nothing Then
                dcpkg.Survey_DC_List = SurveyPrivDCPkg.Survey_DC_List
                dcpkg.Survey_Privilege_List = SurveyPrivDCPkg.Survey_Privilege_List
            Else
                dcpkg = New DC_Package
            End If
            ' TODO: Change TTL to something not hardcoded
            Dim svcstartpkg = From se In PermittedServiceElements(_PermittedSvcsBmsk) _
                          Select New ServiceStartPackage _
                          With {.DC_Pkg = dcpkg, _
                                .Name = se.Name, _
                                .DataContextConnectionString = MyGlobalMasterDataResult_Pkg.CustomerDatabase, _
                                .EndpointSuffix = _LogIn_Email, _
                                .RestartonFault = False, _
                                .PermittedSvcBmask = _PermittedSvcsBmsk, _
                                .SvcEnumBmask = SurveyPrivDCPkg.ServiceEnumBitMsk, _
                                .ValidSvcsBmask = ValidSvcsBmsk, _
                                .SurveyPrivDCColxnPkg = SurveyPrivDCPkg}

            If svcstartpkg.Any Then
                rslt = svcstartpkg.ToList
            End If
            If Not IsNothing(MyCustomerDBMgr) Then
                MyCustomerDBMgr.Dispose()
                MyCustomerDBMgr = Nothing
            End If
            SurveyPrivDCPkg = Nothing
            svcstartpkg = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "LogInSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("ServicesToStart: reports error... " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

        Return rslt
    End Function

    Private Function PermittedServiceElements(ByVal _PermittedSvcsBmsk As ULong) As List(Of ServiceElement)
        Dim rslt As List(Of ServiceElement) = Nothing
        Try
            Dim configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            Dim smsg = New ServiceModelSectionGroup()
            smsg = configuration.GetSectionGroup("system.serviceModel")

            'iterate through all of the ServiceModel.Services in appconfig...
            'selecting only those that are in the PermittedServicesBitMsk
            Dim q = From se As ServiceElement In smsg.Services.Services _
                    Where (EnumBitMaskOperations.GetServiceEnumByFullName(se.Name) And _PermittedSvcsBmsk) > 0 _
                    Select se
            If q.Any Then
                rslt = q.ToList
            Else
                rslt = New List(Of ServiceElement)
            End If
            q = Nothing
            smsg = Nothing
            configuration = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "LogInSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("PermittedServiceElements: reports error... " & ex.Message, EventLogEntryType.Error)
            End Using
            rslt = New List(Of ServiceElement)
        End Try

        Return rslt
    End Function

    Public Function Retrieve_ValidServicesBitmask() As ULong
        Dim rslt As ULong = 0
        Try
            Dim configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            Dim validsvcsBmsk = CType(configuration.AppSettings.Settings("ValidServicesBitmask").Value, UInt32)
            rslt = validsvcsBmsk
            configuration = Nothing
            validsvcsBmsk = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "LogInSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_ValidServicesBitmask: reports error... " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        
        Return rslt
    End Function

   

#Region "Original Support Functions"
    'Private Function Populate_EndPtKeysList(ByVal _LogIn_Email As String) As EptLoginPackage
    '    Dim rslt As New EptLoginPackage
    '    'Dim EPtDCClient As New EndPtDCSvc.EndPtDataContextSvcClient
    '    Try
    '        rslt = MyEptDCClient.Retrieve_List_of_EndPtPkg(_LogIn_Email)
    '        If rslt.IsAuthenticated Then
    '            MyEptDCClient.IssueExposeEndPtCommands(_LogIn_Email, rslt.LogIn_ID)
    '        End If
    '    Catch ex As Exception
    '        MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP EndPtKeysList  " & DateTime.Now.ToLongTimeString, _
    '                                           "LogIn_Email=<" & _LogIn_Email & ">" & ex.Message)
    '    End Try
    '    Try
    '        CType(MyEptDCClient, Channels.IChannel).Close() 'should be done with it here...
    '    Catch ex As Exception
    '        MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP TryCloseClient EndPtKeysList  " & DateTime.Now.ToLongTimeString, _
    '                                           "LogIn_Email=<" & _LogIn_Email & ">" & ex.Message)
    '        MyEptDCClient = Nothing
    '    End Try
    '    Return rslt
    'End Function

    'Private Function SpinUpCustomerDBSvc(ByVal _Login_Email As String) As Boolean
    '    Dim rslt As Boolean = False
    '    'this uses 
    '    '<client>
    '    '   <endpoint name="ws_IEndPtDataCntxtSvc" />
    '    '</client> from(app.config)
    '    Dim factory = New ChannelFactory(Of IEndPtDataCntxtSvc)("ws_IEndPtDataCntxtSvc") 'this uses <client><endpoint name=ws_IEndPtDataCntxtSvc /> from app.config
    '    MyEptDCClient = factory.CreateChannel

    '    CType(MyEptDCClient, Channels.IChannel).Open()
    '    Try
    '        MyEptDCClient.SpinUpCustomerMasterDBSvc(_Login_Email)
    '        rslt = True
    '    Catch ex As Exception
    '        MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP SpinUpCustomerDBSvc  " & DateTime.Now.ToLongTimeString, "(LogIn_Email=<" & _Login_Email & ">" & ex.Message)
    '        MyEptDCClient = Nothing 'if it blows up here, just set it to nothing...
    '    End Try
    '    'Try
    '    '    CType(EPtDCClient, Channels.IChannel).Close()
    '    'Catch ex As Exception
    '    '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP TryCloseClient SpinUpCustomerDBSvc " & DateTime.Now.ToLongTimeString, "(LogIn_Email=<" & _Login_Email & ">" & ex.Message)
    '    '    EPtDCClient = Nothing
    '    'End Try
    '    factory = Nothing
    '    Return rslt
    'End Function
#End Region

#End Region 'Support Functions LogMeInPlease
#End Region 'LogMeInPlease

#Region "LogMeOutPlease"
    Public Function LogMeOutPlease(ByVal _LogOutPack As ILogInSvc.LogOutPackage) As ILogInSvc.LogOutResult Implements ILogInSvc.ILogInSvc.LogMeOutPlease
        Dim rslt As LogOutResult = New LogOutResult With {.SpiffList = New List(Of Srlzd_KVP)}
        If Not IsNothing(_LogOutPack) Then
            Dim logoutemail As String = ""
            Try
                logoutemail = _LogOutPack.LogIn_Email
                If Not IsNothing(_LogOutPack.LogIn_Result) Then
                    WCFServiceManager.StopTheseServices(_LogOutPack.LogIn_Result.EndpointKeysList)
                    rslt.SpiffList.Add(New Srlzd_KVP("Message", "closed services in logout package"))
                Else
                    'get the endptskeys for this email.....
                    rslt.SpiffList.Add(New Srlzd_KVP("Message", "loginrslt is nothing...services still active in WCFServiceManager..."))
                End If


                'this should also remove the UCI from USerCacheManager....
                If Not IsNothing(_LogOutPack.LogIn_Email) Then
                    UserCacheManager.RemoveUserCacheInfo(SharedMethods.EmailAddress_ToNormalized(_LogOutPack.LogIn_Email))
                    rslt.SpiffList.Add(New Srlzd_KVP("Message", "removed usercacheinfo.."))
                Else
                    rslt.SpiffList.Add(New Srlzd_KVP("Message", "LoginEmail is nothing..."))
                End If

                logoutemail = Nothing
            Catch ex As Exception
                rslt.SpiffList.Add(New Srlzd_KVP("Message", ex.Message))
                Using EvLog As New EventLog()
                    EvLog.Source = "LogInSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("LogMeOutPlease: " & logoutemail & " " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
            MyCSH.SvcMonitor.Update_ServiceCalls("LogOut <" & _LogOutPack.LogIn_Email & ">", _
                                                  DateTime.Now.ToLongTimeString)
        Else
            Using EvLog As New EventLog()
                EvLog.Source = "LogInSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("LogMeOutPlease: LogOutPack is nothing... ", EventLogEntryType.Error)
            End Using
        End If

        Return rslt
    End Function
#End Region 'LogMeOutPlease



#Region "WhoIsHere"
    Public Function WhoIsHere(ByVal _WhoIsHerePack As ILogInSvc.WhoIsHerePackage) As ILogInSvc.WhoIsHereResult Implements ILogInSvc.ILogInSvc.WhoIsHere
        Dim rslt As WhoIsHereResult = New WhoIsHereResult With {.BoolValue = True, _
                                                                .StringValue = _WhoIsHerePack.StringValue & Me.MyCSH.DataContextConnectionString}
        Return rslt
    End Function
#End Region 'WhoIsHere


End Class
Public Class MyValidator
    Inherits UserNamePasswordValidator

    Public Overrides Sub Validate(ByVal userName As String, ByVal password As String)
        If UserCacheManager.IsUserExists(SharedMethods.EmailAddress_ToNormalized(userName), password) Then
        Else
            'Throw New IdentityModel.Tokens.SecurityTokenException("Unknown Username or Incorrect Password")
            Throw New FaultException("Unknown Username or Incorrect Password")
            'SharedEvents.RaiseOperationFailed(userName, "MyValidator reports IsUserExists = false with password=" & password)
        End If
    End Sub
End Class
Public Class MyOtherValidator
    Inherits UserNamePasswordValidator

    Public Overrides Sub Validate(ByVal userName As String, ByVal password As String)
        If UserCacheManager.IsLoggedIn(SharedMethods.EmailAddress_ToNormalized(userName)) Then
        Else
            'Throw New IdentityModel.Tokens.SecurityTokenException("Unknown Username or Incorrect Password")
            Throw New FaultException("Unknown Username or Incorrect Password")
            'SharedEvents.RaiseOperationFailed(userName, "MyValidator reports IsUserExists = false with password=" & password)
        End If
    End Sub
End Class

