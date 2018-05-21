Imports System.Collections.ObjectModel
Imports System.Messaging
Imports System.ServiceModel
Imports MasterDBSvcLibr
Imports System.Configuration
Imports CmdInfrastructureNS
Imports WCFServiceManagerNS
Imports DataContextPackageNS
Imports System.ServiceModel.Configuration

Partial Public Class QueuesManager

    Public WithEvents MyModel As New QueuesManagerModel

    Private Sub IamLoaded() Handles Me.Loaded
        PopulateMyModel()
    End Sub

    Private Sub PopulateMyModel()
        PopulateQueuesList()
        Me.DataContext = MyModel
    End Sub

    Private Sub PopulateQueuesList()
        Dim q = From mq In MessageQueue.GetPrivateQueuesByMachine(My.Computer.Name) _
               Select New TinyMQ With {.RDENTCount = mq.GetAllMessages.Count, _
                                       .Label = mq.Label, _
                                       .LastModifyTime = mq.LastModifyTime, _
                                       .Path = mq.Path, _
                                       .QueueName = Strings.Split(mq.QueueName, "\").Last}
        Dim q1 = From tmq In q _
                 Order By tmq.RDENTCount Descending _
                 Select tmq

        MyModel.MyQueuesList = New ObservableCollection(Of TinyMQ)(q1.ToList)
    End Sub

    Private Sub MyModelPropertyChanged() Handles MyModel.PropertyChanged
        Me.DataContext = Nothing
        Me.DataContext = MyModel
    End Sub

    Private Sub StartAll_btn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'if the RDISPATCHERSvc IsHostedHere then
        ' If RDISPATCHER.State <> Opened andalso RDISPATCHER.Sate <> Openning then
        '   
        'End if
    End Sub
    Private Sub RefreshQueue_Btn(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

    End Sub

    Private Sub RefreshAll_btn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        PopulateQueuesList()
    End Sub

    Private Sub StartQueue_Btn(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim btn = CType(sender, Button)
        Dim queuename = CType(btn.Tag, String)
        If Not queuename.Contains("dispatcher") Then
            Start_SelectedQueue_ProcessingSvc(queuename)
        Else
            MessageBox.Show("This queue is processed by the ResponseDispatcherService....start it using buttons in the left panel...just like LoginSvc")
        End If
        'Dim factory = New ChannelFactory(Of ILogInSvc.ILogInSvc)("ws_ILogInSvc")
        'Dim LoginClient = factory.CreateChannel

        'Try
        '    Dim x = New Collections.Generic.HashSet(Of String)
        '    Dim loginPack As New ILogInSvc.LogInPackage With {.LogIn_Email = queuename, .PasswordHash = x}
        '    Dim xx = LoginClient.LogMeInPlease(loginPack)

        'Catch ex As Exception
        '    MessageBox.Show("QueuesManager.StartQueue_btn " & queuename & " " & ex.Message)
        'End Try
        'CType(LoginClient, Channels.IChannel).Close()

    End Sub

    Dim MyGlobalMasterDataResult_Pkg As GlobalMasterDataByLoginNameResult_Package

    Private QueueCnxnString As String = "NotSet"

    Private Sub Start_SelectedQueue_ProcessingSvc(ByVal _QueueName As String)
        If Not IsNothing(_QueueName) Then
            Try
                Dim _LogInPack As New ILogInSvc.LogInPackage With {.LogIn_Email = _QueueName}
                Dim LogIn_Email = SharedMethods.EmailAddress_ToNormalized(_QueueName)
                ''for now...
                '_LogInPack.LogIn_Email = "q_companyresponses"

                Dim GSMDbSvc As New GlobalSurveyMasterDBSvc
                Dim myActiveLogin = GSMDbSvc.ToActiveLoginInfo(_LogInPack.LogIn_Email)
                If Not IsNothing(myActiveLogin) Then
                    Dim queueCnxnString = myActiveLogin.SubscrInfo.QueueInfo.CnxnString

                    MyGlobalMasterDataResult_Pkg = Me.GetGlobalMasterData(_LogInPack)
                    If Not IsNothing(MyGlobalMasterDataResult_Pkg) Then
                        If MyGlobalMasterDataResult_Pkg.IsAuthenticated Then
                            Dim EndpointKeysList = Me.StartServices(_LogInPack, queueCnxnString)
                        End If
                    End If
                Else

                End If
                GSMDbSvc.Dispose()
                GSMDbSvc = Nothing
                myActiveLogin = Nothing
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "QueuesManager"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Start_SelectedQueue_ProcessingSvc : " & _QueueName & " " & ex.Message, EventLogEntryType.Warning)
                End Using
                MessageBox.Show("Start_SelectedQueue_ProcessingSvc reports " & _QueueName & " " & ex.Message)
            End Try
        Else
            MessageBox.Show("Start_SelectedQueue_ProcessingSvc reports QueueName is nothing....")
        End If


    End Sub

    Private Function GetGlobalMasterData(ByVal _LogInPack As ILogInSvc.LogInPackage) As GlobalMasterDataByLoginNameResult_Package
        Dim rslt As GlobalMasterDataByLoginNameResult_Package = Nothing
        If Not IsNothing(_LogInPack) Then
            Dim myGSMDBMgr As New MasterSurveyDBSvc
            Dim connStrings = ConfigurationManager.ConnectionStrings
            myGSMDBMgr.DC_ConnectionString = connStrings("SurveyMasterConnectionString").ConnectionString
            Try
                rslt = myGSMDBMgr.GetGlobalMasterLoginDataByLoginName(_LogInPack)
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "QueuesManager"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("GetGlobalMasterData : " & _LogInPack.LogIn_Email & " " & ex.Message, EventLogEntryType.Error)
                End Using
            Finally
                myGSMDBMgr.Dispose()
                myGSMDBMgr = Nothing
                connStrings = Nothing
            End Try
        Else
            Using EvLog As New EventLog()
                EvLog.Source = "QueuesManager"
                EvLog.Log = "Application"
                EvLog.WriteEntry("GetGlobalMasterData : _LogInPack IsNothing...", EventLogEntryType.Error)
            End Using
            MessageBox.Show("GetGlobalMasterData reports _LogInPack is nothing....")
        End If

        Return rslt
    End Function

    Private Function StartServices(ByVal _loginPkg As ILogInSvc.LogInPackage, ByVal _QueueCnxnString As String) As String
        Dim rslt As String = ""
        If Not IsNothing(_loginPkg) Then
            Try
                Dim ServiceStartPackages = ServicesToStart(_loginPkg.LogIn_Email)
                If Not IsNothing(ServiceStartPackages) Then
                    Dim postingsvcStartPkg = (From ssp In ServiceStartPackages _
                                         Where ssp.Name.Contains("Posting") _
                                         Select ssp).FirstOrDefault
                    rslt = WCFServiceManager.StartPostingService(postingsvcStartPkg, _QueueCnxnString)
                Else
                    Using EvLog As New EventLog()
                        EvLog.Source = "QueuesManager"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("StartServices : ServiceStartPackages IsNothing...", EventLogEntryType.Error)
                    End Using
                End If
                
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "QueuesManager"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("StartServices : " & _loginPkg.LogIn_Email & " " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        Else
            Using EvLog As New EventLog()
                EvLog.Source = "QueuesManager"
                EvLog.Log = "Application"
                EvLog.WriteEntry("StartServices : LogInPkg IsNothing...", EventLogEntryType.Error)
            End Using
        End If

       
        'ServiceStartPackages.Clear()
        'ServiceStartPackages = Nothing
        Return rslt
    End Function

    Private Function ServicesToStart(ByVal _LogIn_Email As String) As List(Of ServiceStartPackage)
        Dim rslt As New List(Of ServiceStartPackage)
        Dim MyCustomerDBMgr As New CustomerDBOperationsNS.Manager
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
        Return rslt
    End Function

    Private Function PermittedServiceElements(ByVal _PermittedSvcsBmsk As ULong) As List(Of ServiceElement)
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

End Class

Public Class TinyMQ
    Private _RDENTCount As Integer = 0
    Public Property RDENTCount() As Integer
        Get
            Return _RDENTCount
        End Get
        Set(ByVal value As Integer)
            _RDENTCount = value
        End Set
    End Property

    Private _Label As String
    Public Property Label() As String
        Get
            Return _Label
        End Get
        Set(ByVal value As String)
            _Label = value
        End Set
    End Property

    Private _QueueName As String
    Public Property QueueName() As String
        Get
            Return _QueueName
        End Get
        Set(ByVal value As String)
            _QueueName = value
        End Set
    End Property

    Private _Path As String
    Public Property Path() As String
        Get
            Return _Path
        End Get
        Set(ByVal value As String)
            _Path = value
        End Set
    End Property

    Private _LastModifyTime As Date
    Public Property LastModifyTime() As Date
        Get
            Return _LastModifyTime
        End Get
        Set(ByVal value As Date)
            _LastModifyTime = value
        End Set
    End Property
End Class