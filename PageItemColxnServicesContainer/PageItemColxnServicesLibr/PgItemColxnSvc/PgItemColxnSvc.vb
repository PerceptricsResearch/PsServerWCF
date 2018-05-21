Imports IPageItemsSvcNS
Imports CmdInfrastructureNS
Imports CmdSvcClassLibrary

' NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
Public Class PgItemColxnSvc
    Implements IPgItemColxnSvc, IDisposable


    Public DC_ConnectionString As String
    Public MyCSH As CmdSvcClassLibrary.CustomSvcHost = Nothing
    'Private InstanceTracker As New CmdSvcClassLibrary.InstanceTracker(MyCSH)

    Public Function IamWorking() As String Implements IPgItemColxnSvc.IamWorking
        'InstanceTracker.TrackMethod("IamWorking", 0)
        Dim rslt = "This is pgitemscolxnsvc..." 'OperationContext.Current.EndpointDispatcher.EndpointAddress

        Return rslt 'rslt.Uri.OriginalString & DateTime.Now.ToLongTimeString '"YEs...I am here..." & DateTime.Now.ToLongTimeString
    End Function
    Public Sub SetChache(ByVal _eptSuffix As String) Implements IPgItemColxnSvc.SetCache
        If MyCSH.DC_Pkg.Survey_DC_List.Count = 1 Then
            'InstanceTracker.TrackMethod("SetChache", 0)
            Dim surveyid = CType(MyCSH.DC_Pkg.Survey_DC_List.First.Key, Integer)
            MyCSH.Cache_Pkg = New Cache_Package(Me.Retrieve_PageBlobInfo_WithIndex(surveyid, 0), surveyid, 0)
        Else
            'is a list of surveyids....not sure what to cache...maybe could do some kind of operation just to get the service spun up...
            MyCSH.Cache_Pkg = New Cache_Package(Me.IamWorking & " " & _eptSuffix, 0, 0)
        End If
    End Sub


#Region "Retrieve Methods"
    Public Function RetrievePageItemModelList(ByVal _SurveyID As Integer) _
    As List(Of PgItemModel) 'Implements IPgItemColxnSvc.RetrievePageItemModelList
        MyCSH.SvcMonitor.Update_ServiceCalls("RetrvPageITEM Model List Start" & DateTime.Now.ToLongTimeString, "(SurveyID=<" & _SurveyID.ToString & ">") 'Count=<" & rslt.Count & ">")

        Dim xxx = CType(OperationContext.Current.Host, CustomSvcHost)
        xxx.LastInanceCreatedDateTime = DateTime.Now
        Dim rslt As New List(Of PgItemModel)
        If _SurveyID > 0 Then
            Try
                Dim db As New L2S_PgItemColxnSDSDataContext
                xxx.DataContextConnectionString = db.Connection.ConnectionString
                Dim q = From pim In db.PgItemModels _
                        Where pim.SurveyID = _SurveyID _
                        Select pim
                If q.Any Then
                    rslt = q.ToList
                End If

            Catch ex As Exception
                MyCSH.SvcMonitor.Update_ServiceCalls("BLOWED UP RetrvPageITEM Model List " & DateTime.Now.ToLongTimeString, "(SurveyID=<" & _SurveyID.ToString & ">")
            End Try
        End If
        MyCSH.SvcMonitor.Update_ServiceCalls("RetrvPageITEM Model List End" & DateTime.Now.ToLongTimeString, "(SurveyID=<" & _SurveyID.ToString & "> Count=<" & rslt.Count & ">")
        Return rslt
    End Function

    Public Function RetrievePageContentElementList(ByVal _PageContentModelID As Integer) As System.Collections.Generic.List(Of PageContentElement) 'Implements IPgItemColxnSvc.RetrievePageContentElementList
        MyCSH.SvcMonitor.Update_ServiceCalls("Retrieve pgCELEMENT List" & DateTime.Now.ToLongTimeString, " from PageContentModelID=<" & _PageContentModelID.ToString & ">") 'Count=<" & rslt.Count & ">")
        Try
            Dim xxx = CType(OperationContext.Current.Host, CustomSvcHost)
        Catch ex As Exception
            Dim yyy As Integer = 2
        End Try

        Dim rslt As New List(Of PageContentElement)

        If _PageContentModelID > 0 Then
            Try
                Dim db As New L2S_PgItemColxnSDSDataContext
                Dim q = From pim In db.PageContentElements _
                        Where pim.PageContentModelID = _PageContentModelID _
                        Select pim
                If q.Any Then
                    rslt = q.ToList
                End If

            Catch ex As Exception
                MyCSH.SvcMonitor.Update_ServiceCalls("BLOWED UP pgCELEMENT List " & DateTime.Now.ToLongTimeString, " from PageContentModelID=<" & _PageContentModelID.ToString & ">")
            End Try
        End If
        MyCSH.SvcMonitor.Update_ServiceCalls("Retrieve pgCELEMENT List" & DateTime.Now.ToLongTimeString, " from PageContentModelID=<" & _PageContentModelID.ToString & "> Count=<" & rslt.Count & ">")
        Return rslt
    End Function

    Public Function RetrievePageContentModelList(ByVal _PgItemModelID As Integer) As System.Collections.Generic.List(Of PageContentModel) 'Implements IPgItemColxnSvc.RetrievePageContentModelList
        Dim rslt As New List(Of PageContentModel)
        If _PgItemModelID > 0 Then
            Try
                Dim db As New L2S_PgItemColxnSDSDataContext
                Dim q = From pim In db.PageContentModels _
                        Where pim.PgItemModelID = _PgItemModelID _
                        Select pim
                If q.Any Then
                    rslt = q.ToList
                End If

            Catch ex As Exception
                MyCSH.SvcMonitor.Update_ServiceCalls("BLOWED UP pgCONTENTModel List " & DateTime.Now.ToLongTimeString, "from PgItemModelID=<" & _PgItemModelID.ToString & ">)")
            End Try
        End If
        MyCSH.SvcMonitor.Update_ServiceCalls("Retrieve pgCONTENTModel List" & DateTime.Now.ToLongTimeString, "from PgItemModelID=<" & _PgItemModelID.ToString & "> Count=<" & rslt.Count & ">")
        Return rslt
    End Function

    Public Function Retrieve_Page_Count(ByVal _SurveyID As Integer) As Integer Implements IPgItemColxnSvc.Retrieve_PageBlob_Count
        'MyCSH.SvcMonitor.Update_ServiceCalls("Retrieve_Page_Count Start" & DateTime.Now.ToLongTimeString, _
        '                                     "SurveyID=<" & _SurveyID.ToString & ">")

        Dim rslt As Integer = 0
        If _SurveyID > 0 Then
            Try
                Dim db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString) 'MyCSH.DataContextConnectionString)
                Dim q = (From pim In db.PgItemModels _
                        Where pim.SurveyID = _SurveyID _
                        Select pim).Count

                rslt = q
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "PgItemColxnSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Retrieve_Page_Count: SurveyId " & _SurveyID.ToString & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function

    Public Function Retrieve_PageBlobInfo_WithIndex(ByVal _SurveyID As Integer, ByVal _IndexOfPage As Integer) _
    As PageBlobInfo Implements IPgItemColxnSvc.Retrieve_PageBlobInfo_WithIndex
        'InstanceTracker.TrackMethod("Retrieve_PageBlobInfo_WithIndex= " & _IndexOfPage, _SurveyID)

        'MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PageBlobInfo_WithIndex Start" & DateTime.Now.ToLongTimeString, _
        '                                   "SurveyID=<" & _SurveyID.ToString & "> IndexOfPage =<" & _IndexOfPage & ">")
        Dim rslt As PageBlobInfo = Nothing
        Try
            'If _IndexOfPage = 0 AndAlso MyCSH.Cache_Pkg IsNot Nothing Then
            '    If MyCSH.Cache_Pkg.Cache IsNot Nothing AndAlso MyCSH.Cache_Pkg.SurveyID = _SurveyID Then
            '        rslt = MyCSH.Cache_Pkg.Cache
            '    End If
            'Else
            If _SurveyID > 0 Then
                Try
                    'MyCSH.DC_Cnxn_For_SurveyID(_SurveyID)
                    'Dim db As New L2S_PgItemColxnSDSDataContext(MyCSH.DataContextConnectionString)
                    Using db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)

                        Dim q = From pim In db.PgItemModels _
                                Where pim.SurveyID = _SurveyID _
                                Select pim

                        Dim x = q.Skip(_IndexOfPage).Take(1).First

                        Dim q1 = From pcm In db.PageContentModels _
                                Where pcm.PgItemModelID = x.PageItemModelID _
                                Select New PageBlobInfo With {.PIM = x.Model, _
                                                              .PCM = pcm.Model, _
                                                              .PCE_Colxn = (From pce In db.PageContentElements _
                                                                     Where pce.PageContentModelID = pcm.PageContentModelID _
                                                                     Select pce.Model).ToList}
                        'If q.Any Then
                        rslt = q1.First
                        'End If
                        x = Nothing
                    End Using
                    Try
                        ' db.Dispose()
                    Catch ex As Exception
                        'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP PageBlobInfo_WithIndex Try db.dispose" & DateTime.Now.ToLongTimeString, _
                        '                               "(SurveyID=<" & _SurveyID.ToString & "> IndexOfPage =<" & _IndexOfPage & ">")
                    End Try
                Catch ex As Exception
                    'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP PageBlobInfo_WithIndex " & DateTime.Now.ToLongTimeString, _
                    '                                   "(SurveyID=<" & _SurveyID.ToString & "> IndexOfPage =<" & _IndexOfPage & ">")
                End Try
            End If
            'MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PageBlobInfo_WithIndex End" & DateTime.Now.ToLongTimeString, _
            '                                   "(SurveyID=<" & _SurveyID.ToString & "> IndexOfPage =<" & _IndexOfPage & ">") 'Count=<" & rslt.Count & ">")
            'End If
        Catch ex As Exception
            Dim kkk = 2
        End Try
        Return rslt
    End Function

    Public Function Retrieve_PgItemModelIDsOnly(ByVal _SurveyID As Integer) _
    As List(Of Integer) Implements IPgItemColxnSvc.Retrieve_PgItemModelIDsOnly
        MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PgItemModelIDsOnly Start" & DateTime.Now.ToLongTimeString, _
                                           "(SurveyID=<" & _SurveyID.ToString & ">") 'Count=<" & rslt.Count & ">")

        Dim xxx = CType(OperationContext.Current.Host, CustomSvcHost)
        xxx.LastInanceCreatedDateTime = DateTime.Now
        Dim rslt As New List(Of Integer)
        If _SurveyID > 0 Then
            Try
                Dim db As New L2S_PgItemColxnSDSDataContext
                xxx.DataContextConnectionString = db.Connection.ConnectionString
                Dim q = From pim In db.PgItemModels _
                        Where pim.SurveyID = _SurveyID _
                        Select pim.PageItemModelID
                If q.Any Then
                    rslt = q.ToList
                End If

            Catch ex As Exception
                MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP Retrv PgItemModelIDsOnly" & DateTime.Now.ToLongTimeString, "(SurveyID=<" & _SurveyID.ToString & ">")
            End Try
        End If
        MyCSH.SvcMonitor.Update_ServiceCalls("Retrv PgItemModelIDsOnly End" & DateTime.Now.ToLongTimeString, "(SurveyID=<" & _SurveyID.ToString & "> Count=<" & rslt.Count & ">")
        Return rslt
    End Function

    Public Function Retrieve_Page(ByVal _SurveyID As Integer, ByVal _IndexOfPage As Integer) As Page_Package
        Dim rslt As Page_Package = Nothing
        Try
            If _SurveyID > 0 Then
                Try
                    Using db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)

                        Dim q = From pim In db.PgItemModels _
                                Where pim.SurveyID = _SurveyID _
                                Order By pim.PageNumber _
                                Select pim

                        Dim x = q.Skip(_IndexOfPage).Take(1).First

                        Dim q1 = From pcm In db.PageContentModels _
                                Where pcm.PgItemModelID = x.PageItemModelID _
                                Select New Page_Package With {.PgItemModelPkg = New PgItemModel_Package _
                                                              With {.PIM = x.Model, _
                                                                    .PIM_SDSID = x.PageItemModelID, _
                                                                    .PageNumber = x.PageNumber}, _
                                                              .PgContentModelPkg = New PgContentModel_Package _
                                                              With {.PCM = pcm.Model, _
                                                                    .PCM_SDSID = pcm.PageContentModelID}, _
                                                              .PCElement_Pkg_Colxn = (From pce In db.PageContentElements _
                                                                     Where pce.PageContentModelID = pcm.PageContentModelID _
                                                                     Select New PCElement_Package _
                                                                     With {.PCE = pce.Model, _
                                                                           .PCE_SDSID = pce.PageContentElementID}).ToList}

                        rslt = q1.First
                        If Not IsNothing(rslt) Then
                            rslt.SurveyPagesCount = q.Count
                        End If
                        q = Nothing
                        q1 = Nothing
                        x = Nothing
                    End Using
                  
                Catch ex As Exception
                    Using EvLog As New EventLog()
                        EvLog.Source = "PgItemColxnSvc"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("Retrieve_Page: SurveyId " & _SurveyID.ToString & " Reports exception " & ex.Message, EventLogEntryType.Error)
                    End Using
                End Try
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "PgItemColxnSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_Page: SurveyId " & _SurveyID.ToString & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function

    Public Function Retrieve_PCMIDandPageNumbersList(ByVal _surveyID As Integer) As List(Of Srlzd_KVP)
        Dim rslt As List(Of Srlzd_KVP) = Nothing
        Try
            Using db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)

                Dim q = From pim In db.PgItemModels _
                        Where pim.SurveyID = _surveyID _
                        Order By pim.PageNumber _
                        Select pim



                Dim s = From pim In q, pcm In db.PageContentModels _
                        Where pcm.PgItemModelID = pim.PageItemModelID _
                        Select New Srlzd_KVP With {.Key = pcm.PageContentModelID, .Valu = pim.PageNumber}

                If Not IsNothing(s) Then
                    rslt = s.ToList
                End If
                q = Nothing
                s = Nothing
            End Using
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "PgItemColxnSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_PCMIDandPageNumbersList: SurveyId " & _surveyID.ToString & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function
#End Region

#Region "Save Methods"
    Public Function SavePageItemModel(ByVal _PageItemModel_Pkg As PgItemModel_Package, ByVal _SurveyID As Integer) As POCO_ID_Pkg _
    Implements IPgItemColxnSvc.SavePageItemModel

        Dim rslt As New POCO_ID_Pkg With {.POCOGuid = _PageItemModel_Pkg.MyGuid, _
                                          .Original_ID = _PageItemModel_Pkg.PIM_SDSID, _
                                          .Survey_ID = _SurveyID, _
                                          .DB_ID = -1}

        If _PageItemModel_Pkg.PIM_SDSID < 1 Then 'this must be an insert...
            If _SurveyID > 0 Then
                Dim db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)
                Try
                    Dim newpgItemModel As New PgItemModel With {.Model = _PageItemModel_Pkg.PIM, _
                                                                .SurveyID = _SurveyID, _
                                                                .PageNumber = _PageItemModel_Pkg.PageNumber, _
                                                                .PageOptions = "N", _
                                                                .Property1Value = "N", _
                                                                .SurveyName = "N"}

                    db.PgItemModels.InsertOnSubmit(newpgItemModel)
                    db.SubmitChanges()
                    rslt.DB_ID = newpgItemModel.PageItemModelID
                    'MyCSH.SvcMonitor.Update_ServiceCalls("INSERT SavePageItemModel " & DateTime.Now.ToLongTimeString, _
                    '                                     "(SurveyID=<" & _SurveyID.ToString & ">) Page=<" & _PageItemModel_Pkg.PageNumber.ToString & ">")
                Catch ex As Exception
                    SharedEvents.RaiseOperationFailed(Me, "SavePageItemModel " & ex.Message)
                    'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP INSERT SavePageItemModel " & DateTime.Now.ToLongTimeString, _
                    '                                     "(SurveyID=<" & _SurveyID.ToString & ">) Page=<" & _PageItemModel_Pkg.PageNumber.ToString & ">..." & ex.Message)
                End Try
                'db.Dispose()
            Else
                'MyCSH.SvcMonitor.Update_ServiceCalls("NOPE INSERT SavePageItemModel " & DateTime.Now.ToLongTimeString, _
                '                                     "(SurveyID=<" & _SurveyID.ToString & ">) Page=<" & _PageItemModel_Pkg.PageNumber.ToString & ">")
            End If
        Else 'this is an Update..
            Dim db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)
            Try
                Dim sdspim = From row In db.PgItemModels _
                             Where row.PageItemModelID = _PageItemModel_Pkg.PIM_SDSID _
                             Select row
                Dim ur = sdspim.Single()
                If _PageItemModel_Pkg.PIM = "Delete" Then
                    db.PgItemModels.DeleteOnSubmit(ur)
                Else
                    ur.Model = _PageItemModel_Pkg.PIM
                    ur.PageNumber = _PageItemModel_Pkg.PageNumber

                    rslt.DB_ID = ur.PageItemModelID
                End If
                db.SubmitChanges()

                'MyCSH.SvcMonitor.Update_ServiceCalls("UDate SavePageItemModel " & DateTime.Now.ToLongTimeString, _
                '                                     "(SurveyID=<" & _SurveyID.ToString & ">) Page=<" & _PageItemModel_Pkg.PageNumber.ToString & ">")
            Catch ex As Exception
                SharedEvents.RaiseOperationFailed(Me, "SavePageItemModel Is and Update/Delete... " & ex.Message)
                'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP UPDATE SavePageItemModel " & DateTime.Now.ToLongTimeString, _
                '                                     "(SurveyID=<" & _SurveyID.ToString & ">) Page=<" & _PageItemModel_Pkg.PageNumber.ToString & ">..." & ex.Message)
            End Try
            'db.Dispose()
        End If
        Return rslt
    End Function

    Public Function SavePageContentModel(ByVal _PageContentModel_pkg As PgContentModel_Package, ByVal _PIMID As Integer, ByVal _SurveyID As Integer) _
    As POCO_ID_Pkg _
    Implements IPgItemColxnSvc.SavePageContentModel

        Dim rslt As New POCO_ID_Pkg With {.POCOGuid = _PageContentModel_pkg.MyGuid, _
                                          .Original_ID = _PageContentModel_pkg.PCM_SDSID, _
                                          .Survey_ID = _SurveyID, _
                                          .DB_ID = -1}

        If _PageContentModel_pkg.PCM_SDSID < 1 Then
            'this must be an insert...
            'If _PageItemModel.SurveyID > 0 Then
            Dim db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)
            Try
                Dim newPCM As New IPageItemsSvcNS.PageContentModel With {.Model = _PageContentModel_pkg.PCM, _
                                                                        .PgItemModelID = _PIMID, _
                                                                        .HostingControlSize = "0"}

                db.PageContentModels.InsertOnSubmit(newPCM)
                db.SubmitChanges()
                rslt.DB_ID = newPCM.PageContentModelID
                'MyCSH.SvcMonitor.Update_ServiceCalls("INSERT SavePageContentModel " & DateTime.Now.ToLongTimeString, _
                '                                     "(PgItemModelID=<" & _PIMID.ToString & ">)")
            Catch ex As Exception
                SharedEvents.RaiseOperationFailed(Me, "SavePageContentModel " & ex.Message)
                'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP INSERT SavePageContentModel " & DateTime.Now.ToLongTimeString, _
                '                                     "(PgItemModelID=<" & _PIMID.ToString & ">)..." & ex.Message)
            End Try
            'db.Dispose()
            'Else
            ' MyCSH.SvcMonitor.Update_ServiceCalls("NOPE INSERT SavePageItemModel " & DateTime.Now.ToLongTimeString, "(SurveyID=<" & _PageItemModel.SurveyID.ToString & ">) Page=<" & _PageItemModel.PageNumber.ToString & ">")
            'End If
        Else 'this is an Update..
            Dim db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)
            Try
                Dim sdspim = From row In db.PageContentModels _
                         Where row.PageContentModelID = _PageContentModel_pkg.PCM_SDSID _
                         Select row
                Dim ur = sdspim.Single()
                If _PageContentModel_pkg.PCM = "Delete" Then
                    db.PageContentModels.DeleteOnSubmit(ur)
                Else
                    ur.Model = _PageContentModel_pkg.PCM
                    rslt.DB_ID = ur.PageContentModelID
                End If
                db.SubmitChanges()
                'MyCSH.SvcMonitor.Update_ServiceCalls("UDate SavePageContentModel " & DateTime.Now.ToLongTimeString, _
                '                                     "(PgItemModelID=<" & _PIMID.ToString & ">)")
            Catch ex As Exception
                SharedEvents.RaiseOperationFailed(Me, "SavePageContentModel Is and Update/Delete..." & ex.Message)
                'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP UPDATE SavePageContentModel " & DateTime.Now.ToLongTimeString, _
                '                                     "(PgItemModelID=<" & _PIMID & ">) ..." & ex.Message)
            End Try
            'db.Dispose()
        End If
        ' MyCSH.SvcMonitor.Update_ServiceCalls("SavePageContentModel " & DateTime.Now.ToLongTimeString, "(PgItemModelID=<" & _PageContentModel.PgItemModelID.ToString & ">)")
        Return rslt
    End Function

    Public Function SavePageContentElement(ByVal _PageContentElement_pkg As PCElement_Package, ByVal _PCMID As Integer, ByVal _SurveyID As Integer) _
    As POCO_ID_Pkg _
    Implements IPgItemColxnSvc.SavePageContentElement

        Dim rslt As New POCO_ID_Pkg With {.POCOGuid = _PageContentElement_pkg.MyGuid, _
                                          .Original_ID = _PageContentElement_pkg.PCE_SDSID, _
                                          .Survey_ID = _SurveyID, _
                                          .DB_ID = -1}

        If _PageContentElement_pkg.PCE_SDSID < 1 Then
            'this must be an insert...
            'If _PageItemModel.SurveyID > 0 Then
            Dim db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)
            Try
                Dim newPCE As New IPageItemsSvcNS.PageContentElement With {.PageContentModelID = _PCMID, _
                                                                            .Model = _PageContentElement_pkg.PCE, _
                                                                            .PresenterTypeName = "N"}

                db.PageContentElements.InsertOnSubmit(newPCE)
                db.SubmitChanges()
                rslt.DB_ID = newPCE.PageContentElementID
                'MyCSH.SvcMonitor.Update_ServiceCalls("INSERT SavePageContentElement " & DateTime.Now.ToLongTimeString, _
                '                                     "GUID=<" & _PageContentElement_pkg.MyGuid.ToString & ">) PageContentModelID=<" & _PCMID.ToString & ">")
            Catch ex As Exception
                SharedEvents.RaiseOperationFailed(Me, "SavePageContentElement " & ex.Message)
                'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP INSERT SavePageContentElement " & DateTime.Now.ToLongTimeString, _
                '                                     "GUID=<" & _PageContentElement_pkg.MyGuid.ToString & ">) PageContentModelID=<" & _PCMID.ToString & ">" & ex.Message)
            End Try
            'db.Dispose()
            'Else
            ' MyCSH.SvcMonitor.Update_ServiceCalls("NOPE INSERT SavePageItemModel " & DateTime.Now.ToLongTimeString, "(SurveyID=<" & _PageItemModel.SurveyID.ToString & ">) Page=<" & _PageItemModel.PageNumber.ToString & ">")
            'End If
        Else 'this is an Update..
            Dim db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)
            Try
                Dim sdspim = From row In db.PageContentElements _
                         Where row.PageContentElementID = _PageContentElement_pkg.PCE_SDSID _
                         Select row
                Dim ur = sdspim.Single()
                If _PageContentElement_pkg.PCE = "Delete" Then
                    db.PageContentElements.DeleteOnSubmit(ur)
                Else
                    With ur
                        ur.Model = _PageContentElement_pkg.PCE
                        'ur.PresenterTypeName = _PageContentElement.PresenterTypeName
                        'ur.ViewModel = _PageContentElement.ViewModel
                    End With
                    rslt.DB_ID = ur.PageContentElementID
                End If

                db.SubmitChanges()
                'MyCSH.SvcMonitor.Update_ServiceCalls("UDate SavePageContentElement " & DateTime.Now.ToLongTimeString, _
                '                                     "GUID=<" & _PageContentElement_pkg.MyGuid.ToString & ">) PageContentModelID=<" & _PCMID.ToString & ">")
            Catch ex As Exception
                SharedEvents.RaiseOperationFailed(Me, "SavePageContentElement IS an Update/Delete... " & ex.Message)
                'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP UPDATE SavePageContentElement " & DateTime.Now.ToLongTimeString, _
                '                                     "GUID=<" & _PageContentElement_pkg.MyGuid.ToString & ">) PageContentModelID=<" & _PCMID.ToString & ">" & ex.Message)
            End Try
            'db.Dispose()
        End If
        ' MyCSH.SvcMonitor.Update_ServiceCalls("SavePageContentElement " & DateTime.Now.ToLongTimeString, "(PresenterTypeName=<" & Left(_PageContentElement.PresenterTypeName, 30) & ">) PageContentModelID=<" & _PageContentElement.PageContentModelID.ToString & ">")

        Return rslt
    End Function

    Public Function SavePageContentElementsList(ByVal _PCElem_List As List(Of PCElement_Package), ByVal _PCMID As Integer, ByVal _SurveyID As Integer) As List(Of POCO_ID_Pkg)
        Dim rslt As New List(Of POCO_ID_Pkg)
        Dim db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)

        'deletes pagecontentelements that are in the database, but no longer in the PageContentModel being saved..
        Dim pcelemIDs = From pcelmpkg In _PCElem_List _
                        Select pcelmpkg.PCE_SDSID
        Dim deletethesePCElems = From pce In db.PageContentElements _
                                 Where pce.PageContentModelID = _PCMID AndAlso Not pcelemIDs.Contains(pce.PageContentElementID) _
                                 Select pce
        If deletethesePCElems.Any Then
            Try
                db.PageContentElements.DeleteAllOnSubmit(deletethesePCElems)
            Catch ex As Exception
                SharedEvents.RaiseOperationFailed(Me, "SavePageContentElementsList DeleteThesePageContentElements " & ex.Message)
            End Try
        End If

        For Each pcelem In _PCElem_List
            Dim POCO_Pkg As New POCO_ID_Pkg With {.POCOGuid = pcelem.MyGuid, _
                                          .Original_ID = pcelem.PCE_SDSID, _
                                          .Survey_ID = _SurveyID, _
                                          .DB_ID = -1}
            If pcelem.PCE_SDSID < 1 Then
                'this must be an insert...
                Try
                    Dim newPCE As New IPageItemsSvcNS.PageContentElement With {.PageContentModelID = _PCMID, _
                                                                                .Model = pcelem.PCE, _
                                                                                .PresenterTypeName = "N"}
                    db.PageContentElements.InsertOnSubmit(newPCE)
                    db.SubmitChanges()
                    POCO_Pkg.DB_ID = newPCE.PageContentElementID
                Catch ex As Exception
                    SharedEvents.RaiseOperationFailed(Me, "SavePageContentElementsList " & ex.Message)
                End Try
            Else 'this is an Update..
                Try
                    Dim pcelemSDSID = pcelem.PCE_SDSID
                    Dim sdspim = From row In db.PageContentElements _
                             Where row.PageContentElementID = pcelemSDSID _
                             Select row
                    Dim ur = sdspim.Single()
                    ur.Model = pcelem.PCE
                    POCO_Pkg.DB_ID = ur.PageContentElementID
                    db.SubmitChanges()
                Catch ex As Exception
                    SharedEvents.RaiseOperationFailed(Me, "SavePageContentElementsList IS an Update... " & ex.Message)
                End Try

            End If
            rslt.Add(POCO_Pkg)
        Next

        Return rslt
    End Function
    'Public Function SaveSDSResponseModels(ByVal _sdsRI_list As List(Of SDS_ResponseInfo)) As List(Of POCO_ID_Pkg) 'PostingSvc creates response models for you....
    '    Dim rslt = New List(Of POCO_ID_Pkg)
    '    'Dim db As New L2S_PgItemColxnSDSDataContext(DC_ConnectionString)
    '    'Try
    '    '    Dim pidpkg As New POCO_ID_Pkg With {.POCOGuid = Guid.NewGuid}
    '    '    Dim newPCM As New IPageItemsSvcNS.PageContentModel With {.Model = _PageContentModel_pkg.PCM, _
    '    '                                                            .PgItemModelID = _PIMID, _
    '    '                                                            .HostingControlSize = "0"}

    '    '    db.PageContentModels.InsertOnSubmit(newPCM)
    '    '    db.SubmitChanges()
    '    '    rslt.DB_ID = newPCM.PageContentModelID
    '    '    'MyCSH.SvcMonitor.Update_ServiceCalls("INSERT SavePageContentModel " & DateTime.Now.ToLongTimeString, _
    '    '    '                                     "(PgItemModelID=<" & _PIMID.ToString & ">)")
    '    'Catch ex As Exception
    '    '    SharedEvents.RaiseOperationFailed(Me, "SavePageContentModel " & ex.Message)
    '    '    'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP INSERT SavePageContentModel " & DateTime.Now.ToLongTimeString, _
    '    '    '                                     "(PgItemModelID=<" & _PIMID.ToString & ">)..." & ex.Message)
    '    'End Try
    '    Return rslt
    'End Function
#End Region


#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Me.MyCSH = Nothing
                Me.DC_ConnectionString = Nothing
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
