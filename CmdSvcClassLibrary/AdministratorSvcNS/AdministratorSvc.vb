Imports IAdministratorSvcNS
Imports CmdInfrastructureNS
Imports CmdSvcClassLibrary
Imports WCFServiceManagerNS
Imports System.Configuration
Imports System.ServiceModel.Configuration
Imports System.Data.SqlClient
Imports ILogInSvc
Imports MasterDBSvcLibr
Imports DataContextPackageNS
Imports System.ServiceModel
Imports System.IdentityModel.Selectors

Public Class AdministratorSvc
    Implements IAdministratorSvc

    'Make this just like loginSvc...has to have a completely separate client web app
    'everything in this class needs to go through the validator.....doesn't expose other endpoints...althoug it could to make it more secure...
    Private MyCSH As CustomSvcHost = CType(OperationContext.Current.Host, CustomSvcHost)
    Private InstanceTracker As New CmdSvcClassLibrary.InstanceTracker(OperationContext.Current.Host)

    'Private MyEptDCClient As IEndPtDataCntxtSvc
    Dim MyGlobalMasterDataResult_Pkg As GlobalMasterDataByLoginNameResult_Package

#Region "Login and Validation"
    Private PBMFromCustDBMgr As ULong = 0
    Public Function LogMeInPlease(ByVal _LogInPack As LogInPackage) As LogInResult Implements IAdministratorSvc.LogMeInPlease
        MyCSH.SvcMonitor.Update_ServiceCalls("RegisterIN" & DateTime.Now.ToLongDateString, _
                                          "LogIn_Email=<" & _LogInPack.LogIn_Email & ">")
        Dim LogInrslt As LogInResult = New LogInResult With {.LogIn_IsSuccess = False, _
                                                             .SpiffList = New List(Of Srlzd_KVP)}
        Try
            _LogInPack.LogIn_Email = SharedMethods.EmailAddress_ToNormalized(_LogInPack.LogIn_Email)
            MyGlobalMasterDataResult_Pkg = UserCacheManager.GetGMDbpkg(_LogInPack.LogIn_Email).GMDBPkg
            'MyGlobalMasterDataResult_Pkg = Me.GetGlobalMasterData(_LogInPack)
            If MyGlobalMasterDataResult_Pkg.IsAuthenticated Then
                LogInrslt.SpiffList.Add(New Srlzd_KVP("Message", "Success.."))
                LogInrslt.SpiffList.Add(New Srlzd_KVP("PBM", PBMFromCustDBMgr))
                LogInrslt.LogIn_IsSuccess = True
                UserCacheManager.PopulateLoginIsSuccess(_LogInPack.LogIn_Email, _LogInPack.PasswordHashINT, True, False)
                SharedEvents.RaiseOperationFailed(_LogInPack.LogIn_Email, "AdministratorSvc.LogMeInPlease SUCCESS")
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
            SharedEvents.RaiseOperationFailed(_LogInPack.LogIn_Email, "AdministratorSvc.LogMeInPleaseFAILED " & ex.Message)
            LogInrslt.SpiffList.Add(New Srlzd_KVP("Message", "Failure..can't connect to Server..."))
            MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP IN  " & DateTime.Now.ToLongTimeString, _
                                                 "(LogIn_Email=<" & _LogInPack.LogIn_Email & ">" & ex.Message)
        End Try
        'MyGlobalMasterDataResult_Pkg = Nothing
        MyCSH.SvcMonitor.Update_ServiceCalls(LogInrslt.LogIn_IsSuccess.ToString & " " & DateTime.Now.ToLongTimeString, _
                                             "(LogIn_Email=<" & _LogInPack.LogIn_Email & ">")
        Return LogInrslt
    End Function
#End Region
#Region "LogMeOutPlease"
    Public Function LogMeOutPlease(ByVal _LogOutPack As ILogInSvc.LogOutPackage) As ILogInSvc.LogOutResult Implements IAdministratorSvc.LogMeOutPlease
        Dim rslt As LogOutResult = New LogOutResult With {.SpiffList = New List(Of Srlzd_KVP)}
        Try
            'WCFServiceManager.StopTheseServices(_LogOutPack.LogIn_Result.EndpointKeysList)
            rslt.SpiffList.Add(New Srlzd_KVP("Message", "closed services in logout package"))
            'this should also remove the UCI from USerCacheManager....
        Catch ex As Exception
            rslt.SpiffList.Add(New Srlzd_KVP("Message", ex.Message))
        End Try
        MyCSH.SvcMonitor.Update_ServiceCalls("LogOut" & DateTime.Now.ToLongTimeString, _
                                           "(LogIn_Email=<" & _LogOutPack.LogIn_Email & ">")
        Return rslt
    End Function
#End Region

#Region "Operations Methods"
    Public Sub ResetPassword(ByVal _PwdPkg As Password_Package) Implements IAdministratorSvc.ResetPassword
        If Not IsNothing(_PwdPkg) Then
            Try
                Dim MyGSMSvc = New GlobalSurveyMasterDBSvc
                MyGSMSvc.ChangePwd(_PwdPkg)
                SharedEvents.RaiseOperationFailed(_PwdPkg.LogIn_Email, "AdministratorSvc.ResetPassword SUCCESS")
            Catch ex As Exception
                SharedEvents.RaiseOperationFailed(_PwdPkg.LogIn_Email, "AdministratorSvc.ResetPasswordFAILED " & ex.Message)
            End Try
        Else
            SharedEvents.RaiseOperationFailed("PwdPkg is Nothing", "AdministratorSvc.ResetPassword FAILED-PwdPkg is Nothing.")
        End If
    End Sub

    Public Sub Remove_UserCacheInfo(ByVal _UCMDxnryKey As String) Implements IAdministratorSvc.Remove_UserCacheInfo
        If Not IsNothing(_UCMDxnryKey) Then
            Try
                UserCacheManager.RemoveUserCacheInfo(_UCMDxnryKey)
                SharedEvents.RaiseOperationFailed(_UCMDxnryKey, "AdministratorSvc.Remove_UserCacheInfo SUCCESS")
            Catch ex As Exception
                SharedEvents.RaiseOperationFailed(_UCMDxnryKey, "AdministratorSvc.Remove_UserCacheInfoFAILED " & ex.Message)
            End Try
        Else
            SharedEvents.RaiseOperationFailed("_UCMDxnryKey is Nothing", "AdministratorSvc.Remove_UserCacheInfo FAILED-UCMDxnryKey is Nothing.")
        End If
        UserCacheManager.RemoveUserCacheInfo(_UCMDxnryKey)
    End Sub

    Public Function Retrieve_UserCacheInfo(ByVal _UCMDxnryKey As String) As List(Of UserCacheInfo) Implements IAdministratorSvcNS.IAdministratorSvc.Retrieve_UserCacheInfo
        Dim rslt As List(Of UserCacheInfo) = Nothing
        rslt = UserCacheManager.RetrieveUserCacheInfo(_UCMDxnryKey)
        Return rslt
    End Function

    Public Function Retrieve_UserCacheManagerDxnry() As List(Of UserCacheInfo) Implements IAdministratorSvcNS.IAdministratorSvc.Retrieve_UserCacheManagerDxnry
        Dim rslt As List(Of UserCacheInfo) = Nothing
        rslt = UserCacheManager.RetrieveListofUserCacheInfo
        Return rslt
    End Function

    Public Function Retrieve_AllowedServices_List() As List(Of Srlzd_KVP) _
    Implements IAdministratorSvc.Retrieve_AllowedServices_List
        Dim rslt As List(Of Srlzd_KVP) = Nothing

        Dim configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim validsvcsBmsk = CType(configuration.AppSettings.Settings("ValidServicesBitmask").Value, UInt32)
        Dim svclist = From svc In EnumBitMaskOperations.BmskToList(Of ServiceTypeBMskEnum)(validsvcsBmsk) _
                      Select New Srlzd_KVP(validsvcsBmsk, svc)
        If svclist.Any Then
            rslt = svclist.ToList
        End If
        Return rslt
    End Function

    Public Function Retrieve_WCFSvcMgr_ServiceDxnry() _
    As List(Of Slzble_CustomSvcHostModel) _
    Implements IAdministratorSvc.Retrieve_WCFSvcMgr_ServiceDxnry
        Dim rslt As List(Of Slzble_CustomSvcHostModel) = Nothing
        Dim q = From csh In WCFServiceManager.ServicesDxnry.Values _
               Select New Slzble_CustomSvcHostModel _
               With {.EndPtURIS = (From epts In csh.Description.Endpoints _
                                 Select epts.ListenUri.AbsoluteUri).ToList, _
                    .DC_Pkg = csh.DC_Pkg, _
                    .EndPtSuffix = csh.EndPtSuffix, _
                    .LastOperationKVP = csh.LastOperationKVP, _
                    .InstanceCount = csh.InstanceCount, _
                    .LastInanceCreatedDateTime = csh.LastInanceCreatedDateTime, _
                    .LastSurveyID = csh.LastSurveyID}
        q.DefaultIfEmpty(Nothing)
        If q.Any Then
            rslt = q.ToList
        End If
        Return rslt
    End Function

    Public Function GetGlobalMasterData(ByVal _LogInPack As LogInPackage) _
    As GlobalMasterDataByLoginNameResult_Package _
    Implements IAdministratorSvc.GetGlobalMasterData
        Dim myGSMDBMgr As New MasterSurveyDBSvc
        Dim MyGlobalMasterDataResult_Pkg As GlobalMasterDataByLoginNameResult_Package
        Dim connStrings = ConfigurationManager.ConnectionStrings
        'Dim conn = New SqlConnection(connStrings("SurveyMasterConnectionString").ConnectionString)
        myGSMDBMgr.DC_ConnectionString = connStrings("SurveyMasterConnectionString").ConnectionString
        'MyGlobalMasterDataResult_Pkg = myGSMDBMgr.WithLinqStoredProcedure(_LogInPack)
        MyGlobalMasterDataResult_Pkg = myGSMDBMgr.GetGlobalMasterLoginDataByLoginName(_LogInPack)
        Return MyGlobalMasterDataResult_Pkg
    End Function

    'Private Function ServicesToStartVersion1(ByVal _LogIn_Email As String) As List(Of ServiceStartPackage)
    '    Dim rslt As New List(Of ServiceStartPackage)
    '    Dim MyCustomerDBSvc As New CustomerDBSvc.CustomerDBSvc
    '    Dim MyGlobalMasterDataResult_Pkg = GetGlobalMasterData(New LogInPackage With {.LogIn_Email = _LogIn_Email})
    '    MyCustomerDBSvc.DC_ConnectionString = MyGlobalMasterDataResult_Pkg.CustomerDatabase

    '    Dim _loginID = MyGlobalMasterDataResult_Pkg.LoginID
    '    Dim SurveyPrivDCPkg = MyCustomerDBSvc.GetPrivServiceMappings(_loginID)
    '    Dim serviceTypeBitMsk = SurveyPrivDCPkg.ServiceEnumBitMsk
    '    For Each svcEnum In EnumBitMaskOperations.BmskToValues(Of ServiceTypeBMskEnum)(serviceTypeBitMsk)
    '        Dim ssp = New ServiceStartPackage()
    '        Dim scvenumproxy = svcEnum
    '        Dim q = From row In MyGlobalMasterDataResult_Pkg.Services _
    '                   Where row.WCFSvcID = scvenumproxy _
    '                   Select row.Name
    '        q.DefaultIfEmpty("NotSet")
    '        ssp.Name = q.FirstOrDefault
    '        'ssp.Name = MyGlobalMasterDataResult_Pkg.Services.Select("WCFServiceID=" + svcEnum.ToString())(0)("Name")
    '        ssp.DataContextConnectionString = MyGlobalMasterDataResult_Pkg.CustomerDatabase
    '        ' TODO: See if we need a Clone() method here...not sure if the assignment will work
    '        ssp.DC_Pkg = SurveyPrivDCPkg.DC_Pkg
    '        ssp.EndpointSuffix = _LogIn_Email
    '        ssp.RestartonFault = False
    '        rslt.Add(ssp)
    '    Next

    '    ' TODO: Populate a list of EndpointPackages and return this
    '    Return rslt
    'End Function

    'Public Function ServicesToStartVersion2(ByVal _LogIn_Email As String) As List(Of ServiceStartPackage) _
    'Implements IAdministratorSvc.ServicesToStartVersion2
    '    Dim rslt As New List(Of ServiceStartPackage)
    '    Dim MyCustomerDBMgr As New CustomerDBOperationsNS.Manager
    '    MyCustomerDBMgr.DC_ConnectionString = MyGlobalMasterDataResult_Pkg.CustomerDatabase

    '    Dim _loginID = MyGlobalMasterDataResult_Pkg.LoginID
    '    Dim SurveyPrivDCPkg = MyCustomerDBMgr.GetPrivServiceMappings(_loginID)

    '    ' ValidServicesBitmask from config...describes Services Hosted by this WindowsServiceHostingContainer
    '    Dim ValidSvcsBmsk = Retrieve_ValidServicesBitmask()

    '    ' PermittedSvcsBitMsk is intersection of User'sPrivilegeServices and ValidSvcBmskSvcs
    '    Dim _PermittedSvcsBmsk As ULong = SurveyPrivDCPkg.ServiceEnumBitMsk And ValidSvcsBmsk

    '    ' TODO: Change TTL to something not hardcoded
    '    Dim svcstartpkgs = From se In PermittedServiceElements(_PermittedSvcsBmsk) _
    '                  Select New ServiceStartPackage _
    '                  With {.DC_Pkg = New DC_Package(_loginID, 3600, MyGlobalMasterDataResult_Pkg.CustomerDatabase), _
    '                        .Name = se.Name, _
    '                        .DataContextConnectionString = MyGlobalMasterDataResult_Pkg.CustomerDatabase, _
    '                        .EndpointSuffix = _LogIn_Email, _
    '                        .RestartonFault = False}

    '    If svcstartpkgs.Any Then
    '        rslt = svcstartpkgs.ToList
    '    End If
    '    Return rslt
    'End Function

    Private Function ServicesToStart(ByVal _LogIn_Email As String) As List(Of ServiceStartPackage) _
    Implements IAdministratorSvc.ServicesToStartVersion2
        Dim rslt As New List(Of ServiceStartPackage)
        Dim MyCustomerDBMgr As New CustomerDBOperationsNS.Manager
        Dim MyGlobalMasterDataResult_Pkg = GetGlobalMasterData(New LogInPackage With _
                                                                  {.LogIn_Email = _LogIn_Email})
        If MyGlobalMasterDataResult_Pkg IsNot Nothing Then
            MyCustomerDBMgr.DC_ConnectionString = MyGlobalMasterDataResult_Pkg.CustomerDatabase

            Dim _loginID = MyGlobalMasterDataResult_Pkg.LoginID
            Dim SurveyPrivDCPkg = MyCustomerDBMgr.GetPrivServiceMappings(_loginID)

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

            MyCustomerDBMgr = Nothing
            SurveyPrivDCPkg = Nothing
            svcstartpkg = Nothing
        End If
        Return rslt
    End Function

    Private Function PermittedServiceElements(ByVal _PermittedSvcsBmsk As ULong) As List(Of ServiceElement) _
    Implements IAdministratorSvc.PermittedServiceElements
        Dim rslt As List(Of ServiceElement) = Nothing
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
        End If
        Return rslt
    End Function

    Public Function Retrieve_ValidServicesBitmask() As ULong
        Dim rslt As ULong = 0
        Dim configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim validsvcsBmsk = CType(configuration.AppSettings.Settings("ValidServicesBitmask").Value, UInt32)
        rslt = validsvcsBmsk
        Return rslt
    End Function
#End Region
#Region "Properties"

#End Region
End Class

Public Class MyAdminSvcValidator
    Inherits UserNamePasswordValidator

    Private PBMFromCustDBMgr As ULong = 0
    Dim MyGlobalMasterDataResult_Pkg As GlobalMasterDataByLoginNameResult_Package
    Public Overrides Sub Validate(ByVal userName As String, ByVal password As String)
        If UserCacheManager.IsUserExists(SharedMethods.EmailAddress_ToNormalized(userName), password) Then
            MyGlobalMasterDataResult_Pkg = UserCacheManager.GetGMDbpkg(SharedMethods.EmailAddress_ToNormalized(userName)).GMDBPkg
            If Not IsNothing(MyGlobalMasterDataResult_Pkg) AndAlso MyGlobalMasterDataResult_Pkg.IsAuthenticated Then
                Dim MyCustomerDBMgr As New CustomerDBOperationsNS.Manager
                MyCustomerDBMgr.DC_ConnectionString = MyGlobalMasterDataResult_Pkg.CustomerDatabase

                Dim _loginID = MyGlobalMasterDataResult_Pkg.LoginID
                Dim SurveyPrivDCPkg = MyCustomerDBMgr.GetPrivServiceMappings(_loginID)

                Me.PBMFromCustDBMgr = MyCustomerDBMgr.PrivBitMask 'this is CustomerSurveyMaster.LoginInfo.PrivBitMask...third table in the storedproc...
                If (Me.PBMFromCustDBMgr And PriviligeDescrEnum.PerceptricsAdministrator) > 0 Then
                Else
                    Throw New FaultException("Unknown Username or Incorrect Password")
                End If
            Else
                Throw New FaultException("Unknown Username or Incorrect Password")
            End If
        Else
            'Throw New IdentityModel.Tokens.SecurityTokenException("Unknown Username or Incorrect Password")
            Throw New FaultException("Unknown Username or Incorrect Password")
            'SharedEvents.RaiseOperationFailed(userName, "MyValidator reports IsUserExists = false with password=" & password)
        End If
    End Sub
End Class