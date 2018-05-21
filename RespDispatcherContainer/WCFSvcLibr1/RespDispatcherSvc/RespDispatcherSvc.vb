Imports System.Transactions
Imports System.Messaging
Imports IRespDispatcherSvcNS
Imports IPostResponsetoSurveySvcNS
Imports System.Configuration

<ServiceBehavior(ConcurrencyMode:=ConcurrencyMode.Single, InstanceContextMode:=InstanceContextMode.Single)> _
Public Class RespDispatcherSvc
    Implements IRespDispatcherSvc


    Private Shared MyCSH As CmdSvcClassLibrary.CustomSvcHost '= CType(OperationContext.Current.Host, CmdSvcClassLibrary.CustomSvcHost)
    Private InstanceTracker As CmdSvcClassLibrary.InstanceTracker '(OperationContext.Current.Host)

    Private TargetPostingClient As IPostResponsetoSurveySvcNS.IPostResponsetoSurveySvc '(New NetMsmqBinding(NetMsmqSecurityMode.None), New EndpointAddress(MyNewUri))


    <OperationBehavior(TransactionScopeRequired:=True, TransactionAutoComplete:=True)> _
    Public Sub DispatchResponses(ByVal _RDentModel As ResponDENTModel) Implements IRespDispatcherSvc.DispatchResponses
        'it appears that in a netmsmq svc, there is no operation context during type initialization....
        'so need to initialize these variables in this operation...could also be the conncurrencymod/instancemode..

        MyCSH = CType(OperationContext.Current.Host, CmdSvcClassLibrary.CustomSvcHost)

        If InstanceTracker Is Nothing Then
            InstanceTracker = New CmdSvcClassLibrary.InstanceTracker(OperationContext.Current.Host)
        End If

        'Create a transaction scope.
        Using scope As New TransactionScope(TransactionScopeOption.Required)
            MyCSH.SvcMonitor.Update_ServiceCalls("<Dispatch Responses RDentLogInEmail='" & _RDentModel.LogInEmail & "' RspColxnCount='" & _RDentModel.ResponseColxn.Count.ToString & "'", _
                                                       DateTime.Now.ToLongTimeString)
            'DONT USE SURVEYID...IT IS LOCAL TO A CUSTOMERSURVEYMASTERDB...
            'use RDENTModel.LogInEmail...Get the CustomerID associated with LogInEmail...
            'From CustomerTable...Select the RDENTQueueURI_ID...is an ID on SurveyDataStoreTable
            'From SurveyDataStoreTable...Select AbsolutePath...set founduri below using AbsolutePath...
            'For QueueName below...Select SurveyDataStore.DatabaseName...for queues it contains the queueName....
            If PopulateTargetPostingClient(_RDentModel.LogInEmail) Then

                ' Make a queued call to submit the RDentModel
                TargetPostingClient.SubmitRespondentModel(_RDentModel)
                ' Complete the transaction.
                scope.Complete()
                MyCSH.SvcMonitor.Update_ServiceCalls("</Dispatch Complete RDentID= " & _RDentModel.ID & " RspColxnCount= " & _RDentModel.ResponseColxn.Count.ToString, _
                                                   DateTime.Now.ToLongTimeString)
                CType(TargetPostingClient, Channels.IChannel).Close()
            Else
                MyCSH.SvcMonitor.Update_ServiceCalls("</Dispatch PopulateTargetPosting = False", _
                                                       DateTime.Now.ToLongTimeString)
                '  ServiceMonitor.Update_ServiceCalls("SurveyID is not authorized for posting...", "SurveyID=<" & _RDentModel.SurveyID.ToString & "> RDentID=<" & _RDentModel.ID.ToString & ">")
            End If

        End Using
    End Sub


    'Private MSurveyDBSvcClient As IMasterSurveyDBSvcNS.IMasterSurveyDBSvc
    'Private Factory As ChannelFactory(Of IMasterSurveyDBSvcNS.IMasterSurveyDBSvc) = Nothing

#Region "SetUp MySurveyMasterDBSvc Classes"
    Private MySurveyMasterDBSvc As masterDBSvcLibr.MasterSurveyDBSvc = Nothing
    Private Function SetUpMySurveyMasterDBSvc() As Boolean
        If MySurveyMasterDBSvc Is Nothing Then
            Dim connStrings = ConfigurationManager.ConnectionStrings
            Dim dcnxnstring = connStrings("SurveyMasterConnectionString").ConnectionString

            If dcnxnstring IsNot Nothing Then
                MySurveyMasterDBSvc = New MasterDBSvcLibr.MasterSurveyDBSvc
                MySurveyMasterDBSvc.DC_ConnectionString = dcnxnstring

                Return True
            Else
                MySurveyMasterDBSvc = Nothing
                Return False
            End If
        Else
            Return True
        End If
    End Function

    'Private Function GetGlobalMasterData(ByVal _RDENT_LogInEmail As String) As MasterDBSvcLibr.GlobalMasterDataByLoginNameResult_Package
    '    Dim myGSMDBMgr As New MasterDBSvcLibr.MasterSurveyDBSvc
    '    Dim connStrings = ConfigurationManager.ConnectionStrings
    '    myGSMDBMgr.DC_ConnectionString = connStrings("SurveyMasterConnectionString").ConnectionString
    '    Dim rslt = myGSMDBMgr.GetGlobalMasterLoginDataByLoginEmail(_RDENT_LogInEmail)
    '    myGSMDBMgr = Nothing
    '    Return rslt
    'End Function

#End Region

    Private Function PopulateTargetPostingClient(ByVal _RDENT_LogInEmail As String) As Boolean
        Dim rslt As Boolean = False
        'Could look in a dxnry here...with SurveyID...shortcut DB calls...

        'DONT USE SURVEYID...IT IS LOCAL TO A CUSTOMERSURVEYMASTERDB...
       
        Dim sID As String = _RDENT_LogInEmail
        'If MSurveyDBSvcClient Is Nothing Then
        '    'MyCSH.SvcMonitor.Update_ServiceCalls(" <LogIn>", DateTime.Now.ToLongTimeString)
        '    ' MyMSDB_RemoteAddress = MyMSDB_BaseAddress & "rdispatchersvc" 'Login_andSetUpRemoteAddressfor_MSBDSvc()

        '    'If MyMSDB_RemoteAddress <> "" Then
        '    'MyCSH.SvcMonitor.Update_ServiceCalls("  MyMSDBRemoteAddress='" & MyMSDB_RemoteAddress & "'", DateTime.Now.ToLongTimeString)

        '    '
        '    Factory = New ChannelFactory(Of IMasterSurveyDBSvcNS.IMasterSurveyDBSvc)("basic_IMasterDBSvc_rdispatcher")
        '    MSurveyDBSvcClient = Factory.CreateChannel

        '    'Dim basicbinding As New BasicHttpBinding("binding1")
        '    'Dim ept As New EndpointAddress(New Uri(MyMSDB_RemoteAddress))
        '    'MSurveyDBSvcClient = ChannelFactory(Of IMasterSurveyDBSvcNS.IMasterSurveyDBSvc).CreateChannel(basicbinding, ept)

        '    CType(MSurveyDBSvcClient, Channels.IChannel).Open()

        '    Dim state = CType(MSurveyDBSvcClient, Channels.IChannel).State
        '    MyCSH.SvcMonitor.Update_ServiceCalls("  MSDBSvcClientState='" & state.ToString & "' </LogIn>", _
        '                                           DateTime.Now.ToLongTimeString)

        '    ' End If
        'End If
        If SetUpMySurveyMasterDBSvc() Then
            'If CType(MSurveyDBSvcClient, Channels.IChannel).State = CommunicationState.Opened Then
            Try

                Dim row = MySurveyMasterDBSvc.TinySurveyRow_UsingLogInEmail(_RDENT_LogInEmail)

                'ServiceMonitor.Update_ServiceCalls("    AFTERMyMSDB_RetrieveRow Row.SurveyID= " & row.SurveyID.ToString, _
                '                              DateTime.Now.ToLongTimeString)
                If row IsNot Nothing Then
                    'ServiceMonitor.Update_ServiceCalls("MyMSDB_RetrieveRow Row.SurveyID " & row.SurveyID.ToString, _
                    '                          DateTime.Now.ToLongTimeString)
                    If Not MessageQueue.Exists(row.QueueName) Then
                        Try
                            MessageQueue.Create(row.QueueName, True)
                            MyCSH.SvcMonitor.Update_ServiceCalls(" <Queue QueueName='", row.QueueName & "' RDentLogIn='" & sID & "' />")
                        Catch ex As Exception
                            MyCSH.SvcMonitor.Update_ServiceCalls(ex.Message, "  <QueueFail RDentLogIn='" & sID & "' />")
                        End Try
                    End If

                    Dim founduri = New Uri(row.QueueUri)
                    Dim newbinding = New NetMsmqBinding("NetMsmqBinding_IPostResponsetoSurveySvc")
                    TargetPostingClient = ChannelFactory(Of IPostResponsetoSurveySvc).CreateChannel(newbinding, New EndpointAddress(founduri))

                    Dim tpc_state = CType(TargetPostingClient, Channels.IChannel).State
                    MyCSH.SvcMonitor.Update_ServiceCalls(" <PostinClient> Uri='" & row.QueueUri & "' State='" & tpc_state.ToString & "'", _
                                                       DateTime.Now.ToLongTimeString & " RDentLogIn='" & sID & "' </PostinClient>")
                    rslt = True
                Else
                    TargetPostingClient = Nothing
                    MyCSH.SvcMonitor.Update_ServiceCalls("RDentLogIn Not Found", "PopulateTargetPostingClient, RDentLogIn=<" & sID & ">")
                End If
            Catch ex As Exception
                MyCSH.SvcMonitor.Update_ServiceCalls(ex.Message, " trying to PopulateTargetPostingClient, RDentLogIn=<" & sID & ">")
            End Try
            'Else
            'Dim state = CType(MSurveyDBSvcClient, Channels.IChannel).State
            'MyCSH.SvcMonitor.Update_ServiceCalls("MSDBSvcClient.State = " & state.ToString, _
            ' DateTime.Now.ToLongTimeString)
            'End If
            'close the msurveydbsvcclient....
            'CType(MSurveyDBSvcClient, Channels.IChannel).Close()
        Else
            MyCSH.SvcMonitor.Update_ServiceCalls("MSDBSvcClient is Nothing", _
                                                       DateTime.Now.ToLongTimeString)
        End If
        MyCSH.SvcMonitor.Update_ServiceCalls("Returning Result = " & rslt.ToString, _
                                                       DateTime.Now.ToLongTimeString)
        'If Factory IsNot Nothing Then
        '    Factory.Close()
        'End If

        Return rslt
    End Function


    'Private Shared MyMSDB_RemoteAddress As String = ""
    'Private Shared MyMSDB_EndPointConfigName = "BasicHttpBinding_IMasterSurveyDBSvc"
    ' Private Shared MyMSDB_BaseAddress = "http://rents/hm/MasterSurveyDB/"

    'Private Shared Function Login_andSetUpRemoteAddressfor_MSBDSvc() As String
    '    Dim LogInSvcClient As New LogInSvcNS.LoginSvcClient

    '    Dim pswdarray As String() = Array.CreateInstance(GetType(String), 1)
    '    Try
    '        LogInSvcClient.Open()
    '        Dim myloginrslt = LogInSvcClient.LogMeInPlease(New LogInSvcNS.LogInPackage With {.LogIn_Email = "rdispatchersvc", _
    '                                                                                     .PasswordHash = pswdarray})
    '        MyMSDB_RemoteAddress = BuildServiceAddress(MyMSDB_BaseAddress, myloginrslt.EndpointKeysList.ToList)
    '    Catch ex As Exception
    '        MyCSH.SvcMonitor.Update_ServiceCalls("LogInSvcClient is Nothing", _
    '                                               DateTime.Now.ToLongTimeString)
    '    End Try

    '    Return MyMSDB_RemoteAddress
    'End Function

    'Private Shared Function BuildServiceAddress(ByVal _svcBaseAddress As String, _
    '                                            ByVal _endPtKeysList As List(Of LogInSvcNS.EndPtPackage)) As String
    '    Dim rslt As String = Nothing
    '    If _endPtKeysList IsNot Nothing Then
    '        Try
    '            Dim q = From ept In _endPtKeysList _
    '                    Where _svcBaseAddress = ept.BaseAddress _
    '                    Select ept.Address
    '            q.DefaultIfEmpty(Nothing)
    '            rslt = q.SingleOrDefault
    '        Catch ex As Exception
    '            Dim x = "Blowedup"
    '            'this needs to go to logging at production time..
    '        End Try
    '    End If
    '    Return rslt

    'End Function


End Class
