Imports System.Transactions
Imports System.Messaging
Imports IRespProviderSvcNS
Imports IPageItemsSvcNS
Imports CmdSvcClassLibrary
Imports CmdInfrastructureNS

Public Class RespProviderSvc
    Implements IRespProviderSvc



    Private RespDispatchSvcClient As IRespDispatcherSvcNS.IRespDispatcherSvc 'New RespDispatchSvc.RespDispatcherSvcClient
    Private MyCSH As CmdSvcClassLibrary.CustomSvcHost = CType(OperationContext.Current.Host, CmdSvcClassLibrary.CustomSvcHost)
    'Private InstanceTracker As New CmdSvcClassLibrary.InstanceTracker(OperationContext.Current.Host)
    Private MyEptSuffix As String = "" 'MyCSH.EndPtSuffix
    Private MyPgItemColxnSvc As PageItemColxnServicesLibr.PgItemColxnSvc = Nothing
    Private MyCustomerDBSvc As CustomerDBSvc.CustomerDBSvc = Nothing

#Region "SetUp PgItemColxn and CustomerDBSvc Classes"
    Private Function SetUpPgItemColxnSvc(ByVal _SurveyId As Integer) As Boolean
        Dim rslt As Boolean = False
        Try
            If Not IsNothing(Me.MyCSH) Then
                With Me.MyCSH
                    Me.MyEptSuffix = .EndPtSuffix
                    Dim dcnxnstring = .DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyId)
                    .LastInanceCreatedDateTime = Date.Now
                    .LastOperationDateTime = Date.Now
                    .InstanceCount += 1
                    .LastSurveyID = _SurveyId
                    If dcnxnstring IsNot Nothing Then
                        MyCSH.DataContextConnectionString = dcnxnstring
                        MyPgItemColxnSvc = New PageItemColxnServicesLibr.PgItemColxnSvc
                        MyPgItemColxnSvc.DC_ConnectionString = dcnxnstring
                        MyPgItemColxnSvc.MyCSH = MyCSH

                        'MyPgItemColxnSvc.SetChache(MyEptSuffix)
                        dcnxnstring = Nothing
                        rslt = True
                    Else
                        Using EvLog As New EventLog()
                            EvLog.Source = "RespProviderSvc"
                            EvLog.Log = "Application"
                            EvLog.WriteEntry("SetUpPgItemColxnSvc: EptSuffix " & Me.MyEptSuffix & " SurveyID " & _SurveyId.ToString & " Returns False ", EventLogEntryType.FailureAudit)
                        End Using
                        MyPgItemColxnSvc = Nothing
                        dcnxnstring = Nothing
                        rslt = False
                    End If
                End With
            Else
                Using EvLog As New EventLog()
                    EvLog.Source = "RespProviderSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("SetUpPgItemColxnSvc:Reports MyCSH isNothing   ", EventLogEntryType.Error)
                End Using
                MyPgItemColxnSvc = Nothing
                rslt = False
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "RespProviderSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SetUpPgItemColxnSvc: EptSuffix " & Me.MyEptSuffix & " SurveyID " & _SurveyId.ToString & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
            MyPgItemColxnSvc = Nothing
            rslt = False
        End Try
        Return rslt
    End Function

    Private Function SetUpCustomerDBSvc() As Boolean
        Dim rslt As Boolean = False
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            Dim dcnxnstring = MyCSH.DC_Pkg.MyCustomerDBCnxnString
            If dcnxnstring IsNot Nothing Then
                MyCustomerDBSvc = New CustomerDBSvc.CustomerDBSvc
                MyCustomerDBSvc.DC_ConnectionString = dcnxnstring
                MyCustomerDBSvc.MyCSH = MyCSH
                dcnxnstring = Nothing
                rslt = True
            Else
                Using EvLog As New EventLog()
                    EvLog.Source = "RespProviderSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("SetUpCustomerDBSvc: EptSuffix " & Me.MyEptSuffix & " Returns False ", EventLogEntryType.FailureAudit)
                End Using
                MyCustomerDBSvc = Nothing
                rslt = False
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "RespProviderSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SetUpCustomerDBSvc: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
            MyCustomerDBSvc = Nothing
            rslt = False
        End Try
        Return rslt
    End Function
#End Region


    Sub SetCache(ByVal _eptSuffix As String) Implements IRespProviderSvc.SetCache
        If Not IsNothing(Me.MyCSH) Then
            With Me.MyCSH
                .RDentCountInLastTTLTime += 1
                If .RDCountTTLExpiresTime.CompareTo(Date.Now) < 1 Then
                    .RDCountTTLExpiresTime = Date.Now.Add(.RDentCountTTLTime)
                End If
                .LastOperationDateTime = Date.Now
            End With
            Me.MyCSH = Nothing
            Me.MyEptSuffix = Nothing
            _eptSuffix = Nothing
        End If
    End Sub

    Sub StoreRespondentModel(ByVal _RDentModel As IPostResponsetoSurveySvcNS.ResponDENTModel) Implements IRespProviderSvc.StoreRespondentModel
        Try
            If Not IsNothing(_RDentModel) Then
                If Not IsNothing(Me.MyCSH) Then
                    With Me.MyCSH
                        Me.MyEptSuffix = .EndPtSuffix
                        .LastOperationDateTime = Date.Now
                        .LastInanceCreatedDateTime = Date.Now
                        .InstanceCount += 1
                        If Not IsNothing(_RDentModel.SurveyID) Then
                            .LastSurveyID = _RDentModel.SurveyID
                        Else
                            Using EvLog As New EventLog()
                                EvLog.Source = "RespProviderSvc"
                                EvLog.Log = "Application"
                                EvLog.WriteEntry("StoreRespondentModel: EptSuffix " & Me.MyEptSuffix & " Reports _RDentModel.SurveyID isNothing   ", EventLogEntryType.Error)
                            End Using
                        End If
                    End With
                    Using scope As New TransactionScope(TransactionScopeOption.Required)
                        If IsNothing(RespDispatchSvcClient) Then
                            Dim msmqbinding As New NetMsmqBinding("NetMsmqBinding_IRespDispatcherSvc")
                            Dim ept As New EndpointAddress(New Uri("net.msmq://localhost/private/dispatcherresponses"))
                            RespDispatchSvcClient = ChannelFactory(Of IRespDispatcherSvcNS.IRespDispatcherSvc).CreateChannel(msmqbinding, ept)
                            'Using EvLog As New EventLog()
                            '    EvLog.Source = "RespProviderSvc"
                            '    EvLog.Log = "Application"
                            '    EvLog.WriteEntry("StoreRDentModel: EptSuffix " & Me.MyEptSuffix & "Created RespDispatchSvcClient on endpoint " & ept.Uri.OriginalString, EventLogEntryType.SuccessAudit)
                            'End Using
                            msmqbinding = Nothing
                            ept = Nothing
                            'RespDispatchSvcClient = New RespDispatchSvc.RespDispatcherSvcClient
                            'RespDispatchSvcClient.Open()
                        Else
                            If CType(RespDispatchSvcClient, Channels.IChannel).State <> CommunicationState.Opened Then
                                Dim msmqbinding As New NetMsmqBinding("NetMsmqBinding_IRespDispatcherSvc")
                                Dim ept As New EndpointAddress(New Uri("net.msmq://localhost/private/dispatcherresponses"))
                                RespDispatchSvcClient = ChannelFactory(Of IRespDispatcherSvcNS.IRespDispatcherSvc).CreateChannel(msmqbinding, ept)
                                Using EvLog As New EventLog()
                                    EvLog.Source = "RespProviderSvc"
                                    EvLog.Log = "Application"
                                    EvLog.WriteEntry("StoreRDentModel: EptSuffix " & Me.MyEptSuffix & " ReBuild UnOpenned RespDispSvcClient " & ept.Uri.OriginalString, EventLogEntryType.Warning)
                                End Using
                                msmqbinding = Nothing
                                ept = Nothing
                            End If
                        End If

                        ' Make a queued call to submit the RDentModel
                        RespDispatchSvcClient.DispatchResponses(_RDentModel)
                        If SetUpCustomerDBSvc() Then
                            MyCustomerDBSvc.RDentCompletedSurvey(_RDentModel.SurveyID)
                        End If
                        ' Complete the transaction.
                        scope.Complete()
                    End Using
                Else
                    Using EvLog As New EventLog()
                        EvLog.Source = "RespProviderSvc"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("StoreRespondentModel:Reports MyCSH isNothing   ", EventLogEntryType.Error)
                    End Using
                End If
            Else
                Using EvLog As New EventLog()
                    EvLog.Source = "RespProviderSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("StoreRespondentModel:Reports _RDentModel isNothing   ", EventLogEntryType.Error)
                End Using
            End If
           
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "RespProviderSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("StoreRDentModel: Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            If Not IsNothing(Me.MyPgItemColxnSvc) Then
                With Me.MyPgItemColxnSvc
                    .Dispose()
                End With
                Me.MyPgItemColxnSvc = Nothing
            End If
            If Not IsNothing(Me.MyCustomerDBSvc) Then
                With Me.MyCustomerDBSvc
                    .Dispose()
                End With
                Me.MyCustomerDBSvc = Nothing
            End If
            Me.RespDispatchSvcClient = Nothing
            Me.MyCSH = Nothing
        End Try

    End Sub


    'Const MyEndpointConfigName = "BasicHttpBinding_IPgItemColxnSvc"

    Private MyBaseAddress As String = "NotSet"
    'Private Sub SetBaseAddressForPgITemsColxn()
    '    MyBaseAddress = (From client In MyCSH.ClientOfThisServiceColxn _
    '                        Where client.Contract = "IPageItemsSvcNS.IPgItemColxnSvc" _
    '                        Select client.BaseAddress).Single
    'End Sub

    Public Function RetrievePageContentElementList(ByVal _PageContentModelID As Integer) As List(Of IPageItemsSvcNS.PageContentElement) Implements IRespProviderSvc.RetrievePageContentElementList
        Dim rslt As List(Of IPageItemsSvcNS.PageContentElement) = Nothing

        ''Dim csh As CustomSvcHost = OperationContext.Current.Host
        ''Dim myremoteaddress = MyBaseAddress & csh.EndPtSuffix 'kluge...see note above...
        ''Dim MyPgItemsColxnClient = New PgItemColxnSvcNS.PgItemColxnSvcClient(MyEndpointConfigName, myremoteaddress)
        'SetBaseAddressForPgITemsColxn()
        'Dim basicbinding As New BasicHttpBinding("binding1")
        'Dim ept As New EndpointAddress(New Uri(MyBaseAddress & MyEptSuffix))
        'Dim MyPgItemsColxnClient = ChannelFactory(Of IPageItemsSvcNS.IPgItemColxnSvc).CreateChannel(basicbinding, ept)
        'Try
        '    ' rslt = MyPgItemsColxnClient.RetrievePageContentElementList(_PageContentModelID).ToList
        '    MyCSH.SvcMonitor.Update_ServiceCalls("RetrievePage ContentELEMENT List " & DateTime.Now.ToLongTimeString, "PageContentModelID=<" & _PageContentModelID.ToString.ToString & "> Count=<" & rslt.Count & ">")
        '    MyPgItemsColxnClient = Nothing
        'Catch ex As Exception
        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWED UP RetrievePage ContentELEMENT List " & DateTime.Now.ToLongTimeString, "PageContentModelID=<" & _PageContentModelID.ToString & ">")

        'End Try
        Return rslt
    End Function

    Public Function RetrievePageContentModelList(ByVal _PgItemModelID As Integer) As List(Of PageContentModel) Implements IRespProviderSvc.RetrievePageContentModelList
        Dim rslt As List(Of IPageItemsSvcNS.PageContentModel) = Nothing

        ''Dim csh As CustomSvcHost = OperationContext.Current.Host
        ''Dim myremoteaddress = MyBaseAddress & csh.EndPtSuffix 'kluge...see note above...
        ''Dim MyPgItemsColxnClient = New PgItemColxnSvcNS.PgItemColxnSvcClient(MyEndpointConfigName, myremoteaddress)
        'SetBaseAddressForPgITemsColxn()
        'Dim basicbinding As New BasicHttpBinding("binding1")
        'Dim ept As New EndpointAddress(New Uri(MyBaseAddress & MyEptSuffix))
        'Dim MyPgItemsColxnClient = ChannelFactory(Of IPageItemsSvcNS.IPgItemColxnSvc).CreateChannel(basicbinding, ept)
        'Try
        '    'rslt = MyPgItemsColxnClient.RetrievePageContentModelList(_PgItemModelID).ToList
        '    MyCSH.SvcMonitor.Update_ServiceCalls("RetrievePage CONTENTMODEL List " & DateTime.Now.ToLongTimeString, _
        '                                         "_PgItemModelID=<" & _PgItemModelID.ToString.ToString & "> Count=<" & rslt.Count & ">")
        '    MyPgItemsColxnClient = Nothing
        'Catch ex As Exception
        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWED UP RetrievePage CONTENTMODEL List " & DateTime.Now.ToLongTimeString, _
        '                                         "_PgItemModelID=<" & _PgItemModelID.ToString & ">")

        'End Try
        Return rslt
    End Function

    Public Function RetrievePageItemModelList(ByVal _SurveyID As Integer) As List(Of PgItemModel) Implements IRespProviderSvc.RetrievePageItemModelList
        'MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PageITEM Model List " & DateTime.Now.ToLongTimeString, _
        '                                     "(SurveyID=<" & _SurveyID.ToString & ">") 'Count=<" & rslt.Count & ">")

        Dim rslt As List(Of PgItemModel) = Nothing

        ''Dim csh As CustomSvcHost = OperationContext.Current.Host
        ''Dim myremoteaddress = MyBaseAddress & csh.EndPtSuffix 'kluge...see note above...
        ''Dim MyPgItemsColxnClient = New PgItemColxnSvcNS.PgItemColxnSvcClient(MyEndpointConfigName, myremoteaddress)
        'SetBaseAddressForPgITemsColxn()
        'Dim basicbinding As New BasicHttpBinding("binding1")
        'Dim ept As New EndpointAddress(New Uri(MyBaseAddress & MyEptSuffix))
        'Dim MyPgItemsColxnClient = ChannelFactory(Of IPageItemsSvcNS.IPgItemColxnSvc).CreateChannel(basicbinding, ept)
        'Try
        '    'rslt = MyPgItemsColxnClient.RetrievePageItemModelList(_SurveyID).Take(1).ToList
        '    MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PageITEM Model List " & DateTime.Now.ToLongTimeString, _
        '                                         "(SurveyID=<" & _SurveyID.ToString & "> Count=<" & rslt.Count & ">")
        '    MyPgItemsColxnClient = Nothing
        'Catch ex As Exception

        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWED UP RetrvPageITEM Model List " & DateTime.Now.ToLongTimeString, "(SurveyID=<" & _SurveyID.ToString & ">")
        'End Try


        Return rslt
    End Function

    Public Function Retrieve_PageBlob_Count(ByVal _SurveyID As Integer) As Integer Implements IRespProviderSvc.Retrieve_PageBlob_Count
        'MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PageBlob_Count " & DateTime.Now.ToLongTimeString, _
        '                                   "SurveyID=<" & _SurveyID.ToString & ">")

        Dim rslt As Integer = Nothing
       
        If SetUpPgItemColxnSvc(_SurveyID) Then
            Try
                rslt = MyPgItemColxnSvc.Retrieve_Page_Count(_SurveyID)
                If SetUpCustomerDBSvc() Then
                    Me.MyCustomerDBSvc.RDentStartedSurvey(_SurveyID)
                End If
                'MyPgItemsColxnClient = Nothing
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "RespProviderSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Retrieve_PageBlob_Count: EptSuffix " & Me.MyEptSuffix & " SurveyID " & _SurveyID.ToString & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            Finally
                If Not IsNothing(Me.MyPgItemColxnSvc) Then
                    With Me.MyPgItemColxnSvc
                        .Dispose()
                    End With
                    Me.MyPgItemColxnSvc = Nothing
                End If
                If Not IsNothing(Me.MyCustomerDBSvc) Then
                    With Me.MyCustomerDBSvc
                        .Dispose()
                    End With
                    Me.MyCustomerDBSvc = Nothing
                End If
            End Try
        End If
        Me.MyCSH = Nothing
        Return rslt
    End Function

    Public Function Retrieve_PageBlobInfo_WithIndex(ByVal _SurveyID As Integer, ByVal _IndexOfPage As Integer) _
    As PageBlobInfo Implements IRespProviderSvc.Retrieve_PageBlobInfo_WithIndex

        Dim rslt As PageBlobInfo = Nothing

        If _IndexOfPage = 0 AndAlso MyCSH.Cache_Pkg IsNot Nothing AndAlso MyCSH.Cache_Pkg.Cache IsNot Nothing Then
            If MyCSH.Cache_Pkg.Cache IsNot Nothing AndAlso MyCSH.Cache_Pkg.SurveyID = _SurveyID Then
                'InstanceTracker.TrackMethod("FROMCACHE Retrieve_PageBlobInfo_WithIndex IndexofPage= " & _IndexOfPage.ToString, _SurveyID)

                'MyCSH.SvcMonitor.Update_ServiceCalls("FROMCACHE Retrv_PageBlobInfo_WithIndex " & DateTime.Now.ToLongTimeString, _
                '                              "(SurveyID=<" & _SurveyID.ToString & "> IndexOfPage=<" & _IndexOfPage & ">")
                rslt = MyCSH.Cache_Pkg.Cache
            End If

        Else
            'InstanceTracker.TrackMethod("Retrieve_PageBlobInfo_WithIndex IndexofPage= " & _IndexOfPage.ToString, _SurveyID)

            'MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PageBlobInfo_WithIndex " & DateTime.Now.ToLongTimeString, _
            '                                   "(SurveyID=<" & _SurveyID.ToString & "> IndexOfPage=<" & _IndexOfPage & ">")

            'SetBaseAddressForPgITemsColxn()
            'Dim basicbinding As New BasicHttpBinding("binding1")
            'Dim ept As New EndpointAddress(New Uri(MyBaseAddress & MyEptSuffix))
            'Dim MyPgItemsColxnClient = ChannelFactory(Of IPageItemsSvcNS.IPgItemColxnSvc).CreateChannel(basicbinding, ept)
            If SetUpPgItemColxnSvc(_SurveyID) Then
                Try
                    'Dim s1 = CType(MyPgItemsColxnClient, Channels.IChannel).State
                    rslt = MyPgItemColxnSvc.Retrieve_PageBlobInfo_WithIndex(_SurveyID, _IndexOfPage)

                Catch ex As Exception
                    'Dim s1 = CType(MyPgItemsColxnClient, Channels.IChannel).State
                    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWED UP PageBlobInfo_WithIndex " & DateTime.Now.ToLongTimeString, _
                                                         "(SurveyID=<" & _SurveyID.ToString & ">")
                End Try

                'MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PageBlobInfo_WithIndex " & DateTime.Now.ToLongTimeString, _
                '                                   "(SurveyID=<" & _SurveyID.ToString & "> IndexOfPage=<" & _IndexOfPage & ">")
            End If
            'MyPgItemsColxnClient = Nothing
            'basicbinding = Nothing
            'ept = Nothing
            End If
            Return rslt
    End Function

    Public Function Retrieve_Page(ByVal _SurveyID As Integer, ByVal _IndexOfPage As Integer) _
    As Page_Package _
    Implements IRespProviderSvc.Retrieve_Page
        Dim rslt As Page_Package = Nothing
     
        If SetUpPgItemColxnSvc(_SurveyID) Then
            Try
                rslt = MyPgItemColxnSvc.Retrieve_Page(_SurveyID, _IndexOfPage)
                rslt.SurveyID = _SurveyID
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "RespProviderSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Retrieve_Page: EptSuffix " & Me.MyEptSuffix & " SurveyID " & _SurveyID.ToString & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using

            Finally
                If Not IsNothing(Me.MyPgItemColxnSvc) Then
                    With Me.MyPgItemColxnSvc
                        .Dispose()
                    End With
                    Me.MyPgItemColxnSvc = Nothing
                End If
                If Not IsNothing(Me.MyCustomerDBSvc) Then
                    With Me.MyCustomerDBSvc
                        .Dispose()
                    End With
                    Me.MyCustomerDBSvc = Nothing
                    Me.MyCSH = Nothing
                End If
            End Try
        End If
        Me.MyCSH = Nothing
        Return rslt
    End Function

    Public Function Retrieve_PgItemModelIDsOnly(ByVal _SurveyID As Integer) _
    As List(Of Integer) Implements IRespProviderSvc.Retrieve_PgItemModelIDsOnly

        MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PgItemModelIDsOnly " & DateTime.Now.ToLongTimeString, _
                                           "SurveyID=<" & _SurveyID.ToString & ">") ' IndexOfPage=<" & _IndexOfPage & ">")

        Dim rslt As List(Of Integer) = Nothing

        ''Dim csh As CustomSvcHost = OperationContext.Current.Host
        ''Dim myremoteaddress = MyBaseAddress & csh.EndPtSuffix 'kluge...see note above...
        ''Dim MyPgItemsColxnClient = New PgItemColxnSvcNS.PgItemColxnSvcClient(MyEndpointConfigName, myremoteaddress)
        ''SetBaseAddressForPgITemsColxn()
        ''Dim basicbinding As New BasicHttpBinding("binding1")
        ''Dim ept As New EndpointAddress(New Uri(MyBaseAddress & MyEptSuffix))
        ''Dim MyPgItemsColxnClient = ChannelFactory(Of IPageItemsSvcNS.IPgItemColxnSvc).CreateChannel(basicbinding, ept)
        'If SetUpPgItemColxnSvc(_SurveyID) Then
        '    Try
        '        rslt = MyPgItemColxnSvc.Retrieve_PgItemModelIDsOnly(_SurveyID).ToList
        '        'MyPgItemsColxnClient = Nothing
        '    Catch ex As Exception

        '        MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP Retrv PgItemModelIDsOnly " & DateTime.Now.ToLongTimeString, _
        '                                              "SurveyID=<" & _SurveyID.ToString & ">")
        '    End Try
        'End If
        'MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PgItemModelIDsOnly " & DateTime.Now.ToLongTimeString, _
        '                                   "SurveyID=<" & _SurveyID.ToString & "> Count=<" & rslt.Count & ">")

        Return rslt
    End Function

    Public Function RetrieveSurveyImagesPackagePCMID(ByVal _SurveyID As Integer, ByVal _PCMID As Integer) As SurveyImagesPackage Implements IRespProviderSvc.RetrieveSurveyImagesPackagePCMID
        Dim rslt As SurveyImagesPackage = Nothing
        If Not IsNothing(Me.MyCSH) Then
            With Me.MyCSH
                .LastOperationDateTime = Date.Now
                .LastSurveyID = _SurveyID
                .LastInanceCreatedDateTime = Date.Now
                .InstanceCount += 1
                Me.MyEptSuffix = .EndPtSuffix
            End With
            If SetUpCustomerDBSvc() Then
                Try
                    rslt = MyCustomerDBSvc.RetrieveSurveyImagesPackagePCMID(_SurveyID, _PCMID)
                Catch ex As Exception
                    Using EvLog As New EventLog()
                        EvLog.Source = "RespProviderSvc"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("RetrieveSurveyImagesPackagePCMID:: EptSuffix " & Me.MyEptSuffix & " SurveyID " & _SurveyID.ToString & " Reports exception " & ex.Message, EventLogEntryType.Error)
                    End Using
                Finally
                    If Not IsNothing(Me.MyCustomerDBSvc) Then
                        With Me.MyCustomerDBSvc
                            .Dispose()
                        End With
                        Me.MyCustomerDBSvc = Nothing
                    End If
                End Try
            End If
            Me.MyCSH = Nothing
        Else
            Using EvLog As New EventLog()
                EvLog.Source = "RespProviderSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RetrieveSurveyImagesPackagePCMID:Reports MyCSH isNothing   ", EventLogEntryType.Error)
            End Using
        End If

        Return rslt
    End Function
End Class
