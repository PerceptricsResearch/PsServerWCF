Imports System.ServiceModel
Imports System.Windows
Imports System.Data.SqlClient
Imports CmdInfrastructureNS

''' <summary>
''' CommandSupport for WPF Services Container T is a Service...T must have only one Interface...resolvable to I'T...'
''' </summary>
''' <remarks></remarks>
Public Class CommandSupport

    Private TName As String = "NotSet"

    Public SvcHostDxnry As New Dictionary(Of String, CustomSvcHost)

    Public MyBaseAddress As String = "ignatz"
    Public MySvcType As Type = Nothing

    Public Function ToIType(ByVal _ClassType As Type) As Type
        Dim rslt As Type = Nothing
        Dim classtypename As String = ""
        Try
            Dim q = From itype In _ClassType.GetInterfaces() _
                Select itype
            rslt = q.Single
            MySvcType = _ClassType
            TName = MySvcType.Name
        Catch ex As Exception
            MessageBox.Show("The ClassType<" & _ClassType.Name & "> provided has more than one interface defined..." & ex.Message)
        End Try
        Return rslt
    End Function


    Private MyEptSuffix As String 'this is ultimately an email...need conversion Suffix.ToEmail, Email.ToSuffix...
    'Public Sub StartCmd(ByVal _cmdPkg As CommandPackage) 'ByVal _EptSuffix As String)
    '    Dim _EptSuffix = _cmdPkg.EndPtSuffix
    '    MyEptSuffix = _cmdPkg.EndPtSuffix
    '    If SvcHostDxnry.ContainsKey(_EptSuffix) Then 'TryGetValue(_queueUri, xxxsvchost) Then
    '        If SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Opening AndAlso SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Opened Then
    '            Try
    '                SvcHostDxnry.Item(_EptSuffix) = Nothing

    '                Dim newPSHost = New CustomSvcHost(MySvcType)
    '                newPSHost.EndPtSuffix = _EptSuffix
    '                If _cmdPkg.CmdVb = CmdVerb.AddDataContext Or _cmdPkg.CmdVb = CmdVerb.ExposeWithDataContext Then
    '                    'newPSHost.DC_Pkg = _cmdPkg.DC_Pkg
    '                    newPSHost.DataContextConnectionString = CnxnString_AbsolutePath(_cmdPkg.DC_Cnxn)
    '                    newPSHost.SvcMonitor = New ServiceMonitor
    '                    newPSHost.SvcMonitor.Update_ServiceCalls("<<Cmd Start Contains DC_Cnxn>>" & DateTime.Now.ToLongTimeString, _
    '                                         "_cmdPkg.DC_Cnxn=<" & _cmdPkg.DC_Cnxn & ">)")

    '                End If
    '                newPSHost.Description.Name = CreateSvcName(_EptSuffix)
    '                Dim endpt As Description.ServiceEndpoint = Nothing
    '                If ToIType(MySvcType).Equals(GetType(IPostResponsetoSurveySvcNS.IPostResponsetoSurveySvc)) Then
    '                    newPSHost.DataContextConnectionString = _cmdPkg.DC_Cnxn
    '                    Dim newbinding = New NetMsmqBinding("NetMsmqBinding_IPostResponsetoSurveySvc")
    '                    endpt = newPSHost.AddServiceEndpoint(ToIType(MySvcType), newbinding, _cmdPkg.DC_Cnxn)
    '                Else
    '                    endpt = newPSHost.AddServiceEndpoint(ToIType(MySvcType), New BasicHttpBinding("binding1"), _
    '                                                         MyBaseAddress & _EptSuffix)
    '                End If
    '                endpt.Name = newPSHost.Description.Name '& "altered"

    '                'Dim tcpendpt = newPSHost.AddServiceEndpoint(GetType(IMasterSurveyDBSvc), New NetTcpBinding(), MyBaseAddress & _EptSuffix)

    '                SvcHostDxnry.Item(_EptSuffix) = newPSHost
    '                'AddHandler newPSHost.Faulted, AddressOf SvcFaulted_Handler

    '                '<<<<<<THIS IS WHERE A CLIENT IS CREATED FOR THIS SERVICE>>>
    '                '<<<AND FOR CERTAIN TYPES, OPERATIONS CAN BE INVOKED>>>
    '                AddHandler newPSHost.Opened, AddressOf OnNewPSHost_Opened

    '                newPSHost.Open()
    '                newPSHost.SvcMonitor.Update_ServiceCalls("CMD Start Contains" & DateTime.Now.ToLongTimeString, _
    '                                          "EptSuffix=<" & _EptSuffix & ">)")

    '            Catch ex As Exception
    '                MessageBox.Show("MasterSurveyDBSvcContainer.Window1.StartCmd encountered error..." & ex.Message)
    '            End Try
    '        End If
    '    Else
    '        Try
    '            Dim newpshost = New CustomSvcHost(MySvcType)
    '            newpshost.SvcMonitor = New ServiceMonitor

    '            newpshost.Description.Name = CreateSvcName(_EptSuffix)

    '            newpshost.EndPtSuffix = _EptSuffix

    '            If _cmdPkg.CmdVb = CmdVerb.AddDataContext Or _cmdPkg.CmdVb = CmdVerb.ExposeWithDataContext Then
    '                'newpshost.DC_Pkg = _cmdPkg.DC_Pkg
    '                newpshost.LastOperationKVP = New KeyValuePair(Of String, String)(DateTime.Now.ToLongTimeString, "CMD Start")
    '                newpshost.DataContextConnectionString = CnxnString_AbsolutePath(_cmdPkg.DC_Cnxn)
    '                newpshost.ClientOfThisServiceColxn = _cmdPkg.ClientOfThisServiceColxn
    '                newpshost.SvcMonitor.Update_ServiceCalls("<<Cmd Start Add DC_Cnxn>>" & DateTime.Now.ToLongTimeString, _
    '                                         "_cmdPkg.DC_Cnxn=<" & _cmdPkg.DC_Cnxn & ">)")

    '            End If

    '            Dim endpt As Description.ServiceEndpoint = Nothing
    '            If ToIType(MySvcType).Equals(GetType(IPostResponsetoSurveySvcNS.IPostResponsetoSurveySvc)) Then
    '                newpshost.DataContextConnectionString = _cmdPkg.DC_Cnxn
    '                Dim newbinding = New NetMsmqBinding("NetMsmqBinding_IPostResponsetoSurveySvc")
    '                endpt = newpshost.AddServiceEndpoint(ToIType(MySvcType), newbinding, _cmdPkg.DC_Cnxn)
    '            Else
    '                endpt = newpshost.AddServiceEndpoint(ToIType(MySvcType), New BasicHttpBinding("binding1"), _
    '                                                     MyBaseAddress & _EptSuffix)
    '            End If

    '            endpt.Name = newpshost.Description.Name '& "altered"

    '            'Dim tcpendpt = newPSHost.AddServiceEndpoint(GetType(IMasterSurveyDBSvc), New NetTcpBinding(), MyBaseAddress & _EptSuffix)

    '            SvcHostDxnry.Add(_EptSuffix, newpshost)
    '            'AddHandler newPSHost.Faulted, AddressOf SvcFaulted_Handler

    '            '<<<<<<THIS IS WHERE A CLIENT IS CREATED FOR THIS SERVICE>>>
    '            '<<<AND FOR CERTAIN TYPES, OPERATIONS CAN BE INVOKED>>>
    '            AddHandler newpshost.Opened, AddressOf OnNewPSHost_Opened

    '            newpshost.Open()
    '            newpshost.SvcMonitor.Update_ServiceCalls("CMD Start Add to SvcDxnry" & DateTime.Now.ToLongTimeString, _
    '                                         "EptSuffix=<" & _EptSuffix & ">)")

    '        Catch ex As Exception
    '            MessageBox.Show(TName & " Container.StartCmd encountered error..." & ex.Message)
    '        End Try
    '    End If
    'End Sub
   


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

    'Private Sub OnNewPSHost_Opened(ByVal sender As Object, ByVal e As EventArgs)
    '    If ToIType(MySvcType).Equals(GetType(ICustomerDBSvc.ICustomerDBSvc)) Then
    '        Try
    '            Dim basicbinding As New BasicHttpBinding("binding1")
    '            Dim ept As New EndpointAddress(New Uri(MyBaseAddress & MyEptSuffix))
    '            Dim clientproxy = ChannelFactory(Of ICustomerDBSvc.ICustomerDBSvc).CreateChannel(basicbinding, ept)

    '            clientproxy.SetCache(MyEptSuffix) 'call retrieve computer privilege(_email)
    '            'CType(sender, CustomSvcHost).SvcMonitor.Update_ServiceCalls("CommandSupport.OnNewPSHost_Opened  ICustomerDBSvc.Retrieve_ComputerPrivileges_Pkg " & DateTime.Now.ToLongTimeString, _
    '            '                             "MySvcType=<" & MySvcType.ToString & ">")
    '            clientproxy = Nothing
    '        Catch ex As Exception
    '            CType(sender, CustomSvcHost).SvcMonitor.Update_ServiceCalls("BLOWEDUP CommandSupport.OnNewPSHost_Opened reports " & DateTime.Now.ToLongTimeString, _
    '                                         "MySvcType=<" & MySvcType.ToString & ">)" & ex.Message)
    '        End Try
    '    End If
    '    If ToIType(MySvcType).Equals(GetType(IPageItemsSvcNS.IPgItemColxnSvc)) Then
    '        Try
    '            Dim basicbinding As New BasicHttpBinding("binding1")
    '            Dim ept As New EndpointAddress(New Uri(MyBaseAddress & MyEptSuffix))
    '            Dim clientproxy = ChannelFactory(Of IPageItemsSvcNS.IPgItemColxnSvc).CreateChannel(basicbinding, ept)

    '            clientproxy.SetCache(MyEptSuffix)

    '            'CType(sender, CustomSvcHost).SvcMonitor.Update_ServiceCalls("CommandSupport.OnNewPSHost_Opened  IPgItemColxnSvc.SetCache " & DateTime.Now.ToLongTimeString, _
    '            '                            "MySvcType=<" & MySvcType.ToString & ">")
    '            clientproxy = Nothing
    '        Catch ex As Exception
    '            CType(sender, CustomSvcHost).SvcMonitor.Update_ServiceCalls("BLOWEDUP CommandSupport.OnNewPSHost_Opened reports " & DateTime.Now.ToLongTimeString, _
    '                                         "MySvcType=<" & MySvcType.ToString & ">)" & ex.Message)
    '        End Try
    '    End If
    '    If ToIType(MySvcType).Equals(GetType(IRespProviderSvcNS.IRespProviderSvc)) Then
    '        Try
    '            Dim basicbinding As New BasicHttpBinding("binding1")
    '            Dim ept As New EndpointAddress(New Uri(MyBaseAddress & MyEptSuffix))
    '            Dim clientproxy = ChannelFactory(Of IRespProviderSvcNS.IRespProviderSvc).CreateChannel(basicbinding, ept)

    '            clientproxy.SetCache(MyEptSuffix)
    '            'CType(sender, CustomSvcHost).SvcMonitor.Update_ServiceCalls("CommandSupport.OnNewPSHost_Opened  IRespProviderSvc.SetCache " & DateTime.Now.ToLongTimeString, _
    '            '                             "MySvcType=<" & MySvcType.ToString & ">")
    '            clientproxy = Nothing
    '        Catch ex As Exception
    '            CType(sender, CustomSvcHost).SvcMonitor.Update_ServiceCalls("BLOWEDUP CommandSupport.OnNewPSHost_Opened reports " & DateTime.Now.ToLongTimeString, _
    '                                         "MySvcType=<" & MySvcType.ToString & ">)" & ex.Message)
    '        End Try
    '    End If
    '    'If ToIType(MySvcType).Equals(GetType(ITesterSvc.ITesterSvc)) Then
    '    '    Try
    '    '        Dim factory = New ChannelFactory(Of ITesterSvc.ITesterSvc)("BasicHttpBinding_ICacheTesterSvc")
    '    '        Dim client As ITesterSvc.ITesterSvc = factory.CreateChannel

    '    '        client.GoDoWhatISaid()
    '    '        CType(sender, CustomSvcHost).SvcMonitor.Update_ServiceCalls("CommandSupport.OnNewPSHost_Opened  Client.GoDoWhatISaid() " & DateTime.Now.ToLongTimeString, _
    '    '                                     "MySvcType=<" & MySvcType.ToString & ">")
    '    '    Catch ex As Exception
    '    '        CType(sender, CustomSvcHost).SvcMonitor.Update_ServiceCalls("BLOWEDUP CommandSupport.OnNewPSHost_Opened reports " & DateTime.Now.ToLongTimeString, _
    '    '                                     "MySvcType=<" & MySvcType.ToString & ">)" & ex.Message)
    '    '    End Try
    '    'End If
    'End Sub

    Public Sub StopCmd(ByVal _EptSuffix As String)
        'Dim xxxsvchost As ServiceHost = Nothing
        If SvcHostDxnry.ContainsKey(_EptSuffix) Then 'TryGetValue(_queueUri, xxxsvchost) Then
            Try
                SvcHostDxnry.Item(_EptSuffix).SvcMonitor.Update_ServiceCalls("CMD Stop " & DateTime.Now.ToLongTimeString, _
                                             "EptSuffix=<" & _EptSuffix & ">)")

                If SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Closing AndAlso SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Closed Then
                    SvcHostDxnry.Item(_EptSuffix).Close()
                End If
            Catch ex As Exception
                MessageBox.Show(TName & " Container.StopCmd encountered error..." & ex.Message)
            End Try

        End If

    End Sub


    Public ReadOnly Property ActiveServicesList() As List(Of KeyValuePair(Of String, CustomSvcHost))
        Get
            Return FnActiveServicesList()
        End Get
    End Property

    Public Function FnActiveServicesList() As List(Of KeyValuePair(Of String, CustomSvcHost))
        Dim rslt = From kvp In SvcHostDxnry _
                   Where kvp.Value.State = CommunicationState.Opened Or kvp.Value.State = CommunicationState.Opening _
                   Select kvp 'New KeyValuePair(Of String, CustomSvcHost)(kvp.Key & " " & kvp.Value.State.ToString, _
        'kvp.Value)
        'Select New CSHButton With {.Content = kvp.Key & " " & kvp.Value.State.ToString, _
        '                                                           .Tag = kvp.Value}
        'Select New KeyValuePair(Of CSHButton, String)(New CSHButton With {.Content = kvp.Key, _
        '                                                            .Tag = kvp.Value}, kvp.Value.State.ToString)
        rslt.DefaultIfEmpty(New KeyValuePair(Of String, CustomSvcHost)("", Nothing))
        'New KeyValuePair(Of CSHButton, String)(New CSHButton With {.Content = "NotSet", _
        '                                                                       .Tag = "NotSet"}, ""))
        Return rslt.ToList
    End Function

    Public Function DormantServicesList() As List(Of KeyValuePair(Of String, String))
        Dim rslt = From kvp In SvcHostDxnry _
                   Where kvp.Value.State = CommunicationState.Closed Or kvp.Value.State = CommunicationState.Closing _
                   Select New KeyValuePair(Of String, String)(kvp.Key, kvp.Value.State.ToString)
        rslt.DefaultIfEmpty(New KeyValuePair(Of String, String)("", ""))
        Return rslt.ToList
    End Function

    ''' <summary>
    ''' Returns the _EptSuffix...not necessary to do this as a function...left it this way for consistency with prior code...
    ''' </summary>
    ''' <param name="_EptSuffix"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CreateSvcName(ByVal _EptSuffix As String) As String
        Return _EptSuffix
        'Return Strings.Split(_EptSuffix, "/").Last
    End Function


    Private Sub SvcFaulted_Handler(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim svchost As CustomSvcHost = CType(sender, CustomSvcHost)
        svchost.FaultsColxn.Add(DateTime.Now)
        'sv.Update_CommandSvcCalls("ServiceHostFaulted " & svchost.Description.Endpoints.FirstOrDefault.Name, DateTime.Now.ToShortTimeString)
        'RemoveHandler svchost.Faulted, AddressOf SvcFaulted_Handler 'not sure if I need this....
        Dim _EptSuffix As String = svchost.EndPtSuffix
        CType(sender, CustomSvcHost).SvcMonitor.Update_ServiceCalls("<<Faulted>>" & DateTime.Now.ToLongTimeString, _
                                             "EptSuffix=<" & _EptSuffix & ">)")

        Dim _Dc_Cnxn As String = svchost.DataContextConnectionString
        Dim _DC_Pkg = svchost.DC_Pkg
        Dim _FaultsColxn = svchost.FaultsColxn
        If SvcHostDxnry.TryGetValue(_EptSuffix, svchost) Then
            Try
                'svchost = New CustomSvcHost(GetType(MasterSurveyDBSvc))
                'svchost.EndPtSuffix = _EptSuffix
                'svchost.DataContextConnectionString = _Dc_Cnxn
                'svchost.DC_Pkg = _DC_Pkg
                'svchost.FaultsColxn = _FaultsColxn

                'svchost.Description.Name = CreateSvcName(_EptSuffix)
                'Dim endpt = svchost.AddServiceEndpoint(GetType(IMasterSurveyDBSvc), _
                '                                       New BasicHttpBinding(BasicHttpSecurityMode.Message), MyBaseAddress & _EptSuffix)
                'endpt.Name = svchost.Description.Name '& "altered"
                'Dim tcpendpt = svchost.AddServiceEndpoint(GetType(IMasterSurveyDBSvc), New NetTcpBinding(), MyBaseAddress & _EptSuffix)

                'SvcHostDxnry.Item(_EptSuffix) = svchost
                'AddHandler svchost.Faulted, AddressOf SvcFaulted_Handler

                'svchost.Open()

            Catch ex As Exception
                MessageBox.Show(TName & " Container.SvcFaultedHandler encountered error..." & ex.Message)
            End Try
        End If
    End Sub


End Class


