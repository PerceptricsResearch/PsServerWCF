Imports IEndPtDataCntxtSvcNS
Imports CmdInfrastructureNS
Imports CmdSvcClassLibrary
Imports ICustomerDBSvc

' NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
Public Class EndPtDataCntxtSvc
    Implements IEndPtDataCntxtSvc


    Private MyCSH As CustomSvcHost = CType(OperationContext.Current.Host, CustomSvcHost)
    Private InstanceTracker As New InstanceTracker(OperationContext.Current.Host)

    Private MyCustomerMasterDBSvcClient As ICustomerDBSvc.ICustomerDBSvc
    Private MyEptList As List(Of EndPtPackage)


#Region "SpinUpCustomerMasterDBsvc"

    '''' <summary>
    '''' Issues a Command on CustomerDBSvc/CommandService to expose an EndPt @ ./CustomerDBSvc/EmailSuffix
    '''' </summary>
    '''' <param name="_LoginEmail"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Function SpinUpCustomerMasterDBSvc(ByVal _LoginEmail As String) As Boolean Implements IEndPtDataCntxtSvc.SpinUpCustomerMasterDBSvc
    '    Dim rslt As Boolean = False
    '    Try
    '        Dim db As New L2S_EptSvcDCDataContext
    '        'this delivers the customerservice endpt for this email
    '        Dim q = From wcfsvc In db.WCFServiceInfos, li In db.LoginInfos, cu In db.Customers, sds In db.SurveyDataStores _
    '                Where li.LoginEmail = _LoginEmail AndAlso li.CustomerID = cu.CustomerID _
    '                    AndAlso cu.CustomerSurveyMasterID = sds.SurveyDataStoreID _
    '                    AndAlso wcfsvc.ComputerID = sds.ComputerID _
    '                    AndAlso wcfsvc.Contract = "ICustomerDBSvc.ICustomerDBSvc" _
    '                    Select wcfsvc Distinct

    '        Dim cmdsvc = q.Single
    '        If cmdsvc.Contract = "ICustomerDBSvc.ICustomerDBSvc" Then
    '            Try
    '                Dim factory = New ChannelFactory(Of ICommandSvcNS.ICommandSvc)("ws_ICustomerDBSvc_ICommandSvc")
    '                Dim cmdsvclient As ICommandSvcNS.ICommandSvc = factory.CreateChannel

    '                Dim cmdpkg As New CommandPackage With {.CmdVb = CmdVerb.ExposeWithDataContext, _
    '                                                       .DC_Pkg = Nothing, _
    '                                                       .EndPtSuffix = SuffixFromLoginEmail(_LoginEmail), _
    '                                                       .IssueDtTime = DateTime.Now, _
    '                                                       .DC_Cnxn = GetCustomerMasterDB_DC_Cnxn(_LoginEmail)} 'this is an absolutepath...
    '                MyCSH.SvcMonitor.Update_ServiceCalls("SpinUpCustomerMasterDBSvc " & _LoginEmail & " " & DateTime.Now.ToLongTimeString, _
    '                                                   CommandPackage.ToText(cmdpkg))
    '                cmdsvclient.IssueCommand(cmdpkg)
    '                rslt = True
    '                cmdsvclient = Nothing
    '                factory.Close()
    '            Catch ex As Exception
    '                MyCSH.SvcMonitor.Update_ServiceCalls("SpinUpCustomerMasterDBSvc.TryCreateCommandSvcClient " & _LoginEmail & " " & ex.Message, _
    '                                                   DateTime.Now.ToLongDateString)
    '            End Try
    '        Else
    '            MyCSH.SvcMonitor.Update_ServiceCalls("<<SpinUpCustomerMasterDBSvc " & _LoginEmail & "Could NotFind CustomerMasterDBSvc in WCFServiceInfos", _
    '                                               DateTime.Now.ToLongTimeString)
    '        End If
    '    Catch ex As Exception
    '        MyCSH.SvcMonitor.Update_ServiceCalls("SpinUpCustomerMasterDBSvc CatchAll " & _LoginEmail & " " & ex.Message, _
    '        DateTime.Now.ToLongTimeString)
    '    End Try
    '    Return rslt
    'End Function

    ''' <summary>
    ''' Retrieve the AbsolutePath of the CustomerMasterDB for this email...
    ''' </summary>
    ''' <param name="_LoginEmail"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCustomerMasterDB_DC_Cnxn(ByVal _LoginEmail As String) As String
        Dim rslt As String = "NotSet_ButTried"

        Dim db As New L2S_EptSvcDCDataContext
        Try
            Dim q = From lg In db.LoginInfos, cu In db.Customers, sds In db.SurveyDataStores _
                    Where lg.LoginEmail = _LoginEmail AndAlso lg.CustomerID = cu.CustomerID _
                    AndAlso cu.CustomerSurveyMasterID = sds.SurveyDataStoreID _
                    Select sds.AbsolutePath

            rslt = q.Single
        Catch ex As Exception
            MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP db.GetCustomerMasterDB_DC_Cnxn " & DateTime.Now.ToLongTimeString, _
                                               "(LogIn_Email=<" & _LoginEmail & ">" & ex.Message)
        End Try
        Return rslt
    End Function

    
#End Region


#Region "Retrieve EndPtPkgs List"
    Public Function Retrieve_List_of_EndPtPkg(ByVal _LogIn_Email As String) As EptLoginPackage Implements IEndPtDataCntxtSvc.Retrieve_List_of_EndPtPkg
        'ServiceMonitor.Update_ServiceCalls("Retrieve_List_of_EndPtPkg  " & DateTime.Now.ToLongTimeString, _
        '                                   "(LogIn_Email=<" & _LogIn_Email & ">")
        Dim rslt As New EptLoginPackage
        rslt.IsLoginEmailFound = False
        rslt.IsAuthenticated = False
        Dim loginID As Integer = -1
        Try
            loginID = FindLoginID(_LogIn_Email)
            rslt.LogIn_ID = loginID
            If loginID > 0 Then
                rslt.IsAuthenticated = True 'for now...do password stuff later...
                rslt.IsLoginEmailFound = True
                'For each EndPt implied by SurveyList...ie., a particular Survey is on ServerA, another Survey is on SurverB, two EndPtPackages are created...
                rslt.ListOfEndPtPackage = Populate_EndPts_List(_LogIn_Email, loginID, PopulateMyCustomerMasterDBSvcClient(_LogIn_Email))
                MyEptList = rslt.ListOfEndPtPackage
            End If
        Catch ex As Exception
            MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP Retrieve_List_of_EndPtPkg  " & DateTime.Now.ToLongTimeString, _
                                               "(LogIn_Email=<" & _LogIn_Email & ">" & ex.Message)
            If loginID > 0 Then
                rslt.IsLoginEmailFound = True
            End If
        End Try

        Return rslt
    End Function

    Private Function FindLoginID(ByVal _LoginEmail As String) As Integer
        Dim rslt As Integer = -1

        Dim db As New L2S_EptSvcDCDataContext
        Dim q = From lg In db.LoginInfos _
                Where lg.LoginEmail = _LoginEmail _
                Select lg.LogInID
        q.DefaultIfEmpty(-1)
        rslt = q.FirstOrDefault
        'Try
        '    db.Dispose()
        'Catch ex As Exception
        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP db.dispose FindLoginID " & DateTime.Now.ToLongTimeString, _
        '                                       "(LogIn_Email=<" & _LoginEmail & ">" & ex.Message)

        'End Try
        Return rslt
    End Function

    'THIS HAS TO USE THE ComputerPrivilegePackage returned from CustomerMasterDBSvcClient.Retrieve_ComputerPrivileges_Pkg(ByVal _LoginEmail As String) As ICustomerDBSvc.ComputerPrivilegePackage
    'the Package as ComputerID and Privilege ID...the query below needs to return the WCFSVCS implied by the Package...
    Private Function Populate_EndPts_List(ByVal _LogIn_Email As String, ByVal _loginID As Integer, ByVal CompPrivPkg As ComputerPrivilegePackage) As List(Of EndPtPackage)
        Dim rslt As List(Of EndPtPackage)

        'ServiceMonitor.Update_ServiceCalls("Populate_EndPts_List  CompPrivilegeList.Count= " & CompPrivPkg.CompPrivilegeList.Count, DateTime.Now.ToLongTimeString)

        Dim qcomputers = From cpi In CompPrivPkg.CompPrivilegeList _
                        Select cpi.ComputerID Distinct
        Dim computers As List(Of Integer) = qcomputers.ToList

        Dim qprivileges = From cpi In CompPrivPkg.CompPrivilegeList _
             Select cpi.PrivilegeID Distinct
        Dim privileges As List(Of Integer) = qprivileges.ToList

        Dim db As New L2S_EptSvcDCDataContext

        Dim q1 = From wcfprcnfg In db.WCFSvcPrivilegeConfigs _
                 Where privileges.Contains(wcfprcnfg.PrivilegeID) _
                 Select wcfprcnfg.WCFSvcTypeID Distinct
        Dim q1x As List(Of Integer) = q1.ToList

        Dim q2 = From wcftype In db.WCFSvCTypes _
                 Where q1x.Contains(wcftype.WCFSvcTypeID) _
                 Select wcftype.WCFSvcTypeID Distinct
        Dim q2x As List(Of Integer) = q2.ToList


        Dim qrslt = From wcfsvc In db.WCFServiceInfos _
                 Where computers.Contains(wcfsvc.ComputerID) _
                        AndAlso q2x.Contains(wcfsvc.WCFSvcTypeID) _
                 Select New EndPtPackage With {.Name = wcfsvc.Name, _
                                              .Address = wcfsvc.BaseAddress & SuffixFromLoginEmail(_LogIn_Email), _
                                              .Suffix = SuffixFromLoginEmail(_LogIn_Email), _
                                              .ServerName = wcfsvc.ComputerServerName, _
                                              .BaseAddress = wcfsvc.BaseAddress, _
                                              .WCFSvcID = wcfsvc.WCFServiceID}
        rslt = qrslt.ToList
        'ServiceMonitor.Update_ServiceCalls("Populate_EndPts_List  EndPtPkg.Count= " & rslt.Count, DateTime.Now.ToLongTimeString)
        computers = Nothing
        privileges = Nothing
        q1x = Nothing
        'Try
        '    'db.Dispose()
        'Catch ex As Exception
        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP db.dispose Populate_EndPts_List " & DateTime.Now.ToLongTimeString, _
        '                                       "LogIn_Email=<" & _LogIn_Email & ">" & ex.Message)

        'End Try
        Return rslt
    End Function

    ''' <summary>
    ''' This Creates a ClientProxy for CustomerMasterDBSvc, populates the MyCmDBscClient variable...ANDALSO returns the ComputerPrivilegePackage for an Email...
    ''' </summary>
    ''' <param name="_LoginEmail"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function PopulateMyCustomerMasterDBSvcClient(ByVal _LoginEmail As String) As ComputerPrivilegePackage
        Dim rslt As ComputerPrivilegePackage = Nothing

        'Try
        '    Dim basicbinding As New BasicHttpBinding("binding1") 'the query below is temporary...not what I want...all of these need to check computerID...
        '    Dim db As New L2S_EptSvcDCDataContext
        '    Dim q = From wsvc In db.WCFServiceInfos _
        '            Where wsvc.Contract = "ICustomerDBSvc.ICustomerDBSvc" _
        '            Select wsvc.BaseAddress
        '    Dim baddr = q.Single
        '    Dim ept As New EndpointAddress(New Uri(baddr & SuffixFromLoginEmail(_LoginEmail)))
        '    MyCustomerMasterDBSvcClient = ChannelFactory(Of ICustomerDBSvc.ICustomerDBSvc).CreateChannel(basicbinding, ept)

        '    Try
        '        rslt = MyCustomerMasterDBSvcClient.Retrieve_ComputerPrivileges_Pkg(_LoginEmail)

        '        'ServiceMonitor.Update_ServiceCalls("PopulateMyCustomerMasterDBSvcClient says x= " & rslt.CompPrivilegeList.Count, _
        '        '                                  "(LogIn_Email=<" & _LoginEmail & ">" & DateTime.Now.ToLongTimeString)
        '    Catch ex As Exception
        '        MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP PopulateMyCustomerMasterDBSvcClient " & DateTime.Now.ToLongTimeString, _
        '                                           "(LogIn_Email=<" & _LoginEmail & ">" & ex.Message)
        '    End Try
        '    basicbinding = Nothing
        '    baddr = Nothing
        '    ept = Nothing
        '    'db.Dispose()
        'Catch ex As Exception
        '    'ServiceMonitor.Update_ServiceCalls("BLOWEDUP PopulateMyCustomerMasterDBSvcClient CatchAll " & DateTime.Now.ToLongTimeString, _
        '    '                                      "(LogIn_Email=<" & _LoginEmail & ">" & ex.Message)
        'End Try
        Return rslt
    End Function
#End Region


#Region "IssueExposeEndPtCommands"
    ''' <summary>
    ''' This issues CommandPackages to the XXXXsvc/CommandSvc...tells them to expose an endpoint using the Suffix, and popultes DCnxn stuff
    ''' </summary>
    ''' <param name="_LogIn_Email"></param>
    ''' <param name="_LoginID"></param>
    ''' <remarks></remarks>
    Public Sub IssueExposeEndPtCommands(ByVal _LogIn_Email As String, ByVal _LoginID As Integer) Implements IEndPtDataCntxtSvc.IssueExposeEndPtCommands
        MyCSH.SvcMonitor.Update_ServiceCalls("IssueExposeEndPtCommands  " & DateTime.Now.ToLongTimeString, _
                                          "(LogIn_Email=<" & _LogIn_Email & ">")
        Try
            Dim db As New L2S_EptSvcDCDataContext
            For Each ept In MyEptList
                Dim varept = ept 'necessary because ept is an iteration variable...
                Dim svc_contract = (From si In db.WCFServiceInfos _
                        Where si.WCFServiceID = varept.WCFSvcID _
                        Select si.Contract).First

                Dim clientslist = GetSvcsIamaClientOf(ept.WCFSvcID, db)

                'If svc_contract = "IMasterSurveyDBSvcNS.IMasterSurveyDBSvc" Then
                '    BuildandIssueCommand("ws_IMasterSurveyDBSvcNS_ICommandSvc", clientslist, db.Connection.Database, _LogIn_Email)

                'ElseIf svc_contract = "IPageItemsSvcNS.IPgItemColxnSvc" Then
                '    BuildandIssueCommand("ws_IPageItemsSvcNS_ICommandSvc", clientslist, "NotSet", _LogIn_Email)

                'ElseIf svc_contract = "IRespProviderSvcNS.IRespProviderSvc" Then
                '    BuildandIssueCommand("ws_IRespProviderSvcNS_ICommandSvc", clientslist, "NotSet", _LogIn_Email)

                'ElseIf svc_contract = "IPostResponsetoSurveySvcNS.IPostResponsetoSurveySvc" Then
                '    BuildandIssueCommand("ws_IPostResponsetoSurveySvcNS_ICommandSvc", clientslist, GetQueueURI(_LogIn_Email), _LogIn_Email)
                'End If

                svc_contract = Nothing
                clientslist = Nothing
            Next
            'db.Dispose() 'don't do this...let gc do it....takes a lot of cycles to dispose....it is true...
        Catch ex As Exception
            MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP IssueExposeEndPtCommands  " & DateTime.Now.ToLongTimeString, _
                                               "(LogIn_Email=<" & _LogIn_Email & ">" & ex.Message)
        End Try
        MyCustomerMasterDBSvcClient = Nothing
        MyEptList = Nothing
    End Sub

    'Private Sub BuildandIssueCommand(ByVal _EndPtConfigName As String, _
    '                                 ByVal _IamAClientList As List(Of ClientOFThisServiceInfo), _
    '                                 ByVal _DC_Cnxn As String, _
    '                                 ByVal _LogIn_Email As String)

    '    Dim factory = New ChannelFactory(Of ICommandSvcNS.ICommandSvc)(_EndPtConfigName)
    '    Dim cmdsvclient As ICommandSvcNS.ICommandSvc = factory.CreateChannel

    '    Dim cmdpkg As New CommandPackage With {.CmdVb = CmdVerb.ExposeWithDataContext, _
    '                                                              .DC_Pkg = Populate_DCPkg(_LogIn_Email, 1), _
    '                                                              .EndPtSuffix = SuffixFromLoginEmail(_LogIn_Email), _
    '                                                              .IssueDtTime = DateTime.Now, _
    '                                                              .DC_Cnxn = _DC_Cnxn, _
    '                                                              .ClientOfThisServiceColxn = _IamAClientList}

    '    'ServiceMonitor.Update_ServiceCalls("IssueExposeEndPtCommands  " & DateTime.Now.ToLongTimeString, _
    '    '                                   CommandPackage.ToText(cmdpkg))
    '    cmdsvclient.IssueCommand(cmdpkg)


    '    cmdpkg = Nothing
    '    factory.Close()
    '    cmdsvclient = Nothing
    'End Sub

    ''This uses the CustomerMasterDBSvc to collect SurveyID and DataStore pairs...
    'Private Function Populate_DCPkg(ByVal _LogIn_Email As String, ByVal _computerID As Integer) As DC_Package
    '    Dim rslt As New DC_Package
    '    Try
    '        'get this from the customerDBSvc...
    '        If MyCustomerMasterDBSvcClient IsNot Nothing Then
    '            rslt = MyCustomerMasterDBSvcClient.Retrieve_DC_Package(_LogIn_Email, _computerID)
    '            MyCSH.SvcMonitor.Update_ServiceCalls("Populate_DCPkg  Survey_DC_List.Count= " & rslt.Survey_DC_List.Count, _
    '                                                  "LogIn_Email=<" & _LogIn_Email & ">" & "ComptrID= " & _computerID.ToString _
    '                                                  & " " & DateTime.Now.ToLongTimeString)
    '        Else
    '            MyCSH.SvcMonitor.Update_ServiceCalls("Populate_DCPkg  MyCustomerMasterDBSvcClient= Nothing", _
    '                                                  "LogIn_Email=<" & _LogIn_Email & ">" & "ComptrID= " & _computerID.ToString _
    '                                                  & " " & DateTime.Now.ToLongTimeString)
    '            'Probably need to try and populate mycustomerMasterDBSvc here
    '            'then wait for it..
    '            'then do the retrieve_DC_Package call...or some asynch/callback thing..argh...
    '        End If
    '    Catch ex As Exception
    '        MyCSH.SvcMonitor.Update_ServiceCalls("Populate_DCPkg " & ex.Message, _
    '                                                  "LogIn_Email=<" & _LogIn_Email & ">" & "ComptrID= " & _computerID.ToString _
    '                                                  & " " & DateTime.Now.ToLongTimeString)
    '    End Try
    '    Return rslt
    'End Function

    Private Function GetSvcsIamaClientOf(ByVal _wcfSvcID As Integer, ByVal db As L2S_EptSvcDCDataContext) As List(Of ClientOFThisServiceInfo)
        Dim rslt As New List(Of ClientOFThisServiceInfo)
        Dim clientlist = From cl In db.ClientOFWCFServiceInfos, wsinfo In db.WCFServiceInfos _
                                  Where _wcfSvcID = cl.SvcthatCallsWCFSvcInfoID _
                                  AndAlso wsinfo.WCFServiceID = cl.SvcthatGetsCalledWCFSvcInfoID _
                                  Select New ClientOFThisServiceInfo With {.WCFServiceInfoID = wsinfo.WCFServiceID, _
                                                                           .WCFServiceName = wsinfo.Name, _
                                                                           .Contract = wsinfo.Contract, _
                                                                           .BaseAddress = wsinfo.BaseAddress}
        If clientlist.Any Then
            rslt = clientlist.ToList
        End If
        Return rslt
    End Function

    Private Function GetQueueURI(ByVal _LoginEmail As String) As String
        Dim rslt As String = "NotSet"
        Dim db As New L2S_EptSvcDCDataContext
        Dim q = From li In db.LoginInfos, cu In db.Customers, sds In db.SurveyDataStores _
                   Where li.LoginEmail = _LoginEmail AndAlso cu.CustomerID = li.CustomerID _
                   AndAlso sds.SurveyDataStoreID = cu.RDENTQueueURI_ID _
                   Select sds.AbsolutePath
        'Select New Tiny_Survey_Row With {.ComputerID = sds.ComputerID, _
        '                                 .QueueName = sds.DatabaseName, _
        '                                 .QueueUri = sds.AbsolutePath, _
        '                                 .SurveyID = 0, _
        '                                 .SurveyName = "NotSet"}
        rslt = q.Single
        Return rslt
    End Function
#End Region 'IssueExposeEndPtCommands




    Private Function SuffixFromLoginEmail(ByVal _loginEmail As String)
        Return _loginEmail.ToLowerInvariant 'for now...need to remove all '.' and '@' and anything else unacceptable... 
    End Function
End Class
