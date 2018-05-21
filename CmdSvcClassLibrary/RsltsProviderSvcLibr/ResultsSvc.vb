Imports System.Data.Linq
Imports IResultsProviderSvcNS
Imports System.ServiceModel

Public Class ResultsSvc
    Implements IResultsSvc

    Public MyCSH As CmdSvcClassLibrary.CustomSvcHost = CType(OperationContext.Current.Host, CmdSvcClassLibrary.CustomSvcHost)
    Private MyCnxnString As String = "Data Source=LEASES\DEVRENTS;Initial Catalog=q_q_comSurveyDataStore_0;Integrated Security=True"
    Const NoFilter As String = "NoFilter"
    Private MyEptSuffix As String = ""
    Private MyCustomerDBSvc As CustomerDBSvc.CustomerDBSvc = Nothing

    Private Function IamOk() As Boolean
        Dim rslt As Boolean = False
        If Not IsNothing(Me.MyCSH) Then
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            rslt = True
        Else
            Me.MyEptSuffix = "Unknown"
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("IamOk: EptSuffix " & Me.MyEptSuffix & " MyCSH isNothing ", EventLogEntryType.Error)
            End Using
        End If
        Return rslt
    End Function

    Private Function CountMe() As Boolean
        Dim rslt As Boolean = False
        Try
            If Not IsNothing(Me.MyCSH) Then
                With Me.MyCSH
                    Me.MyEptSuffix = .EndPtSuffix
                    .LastInanceCreatedDateTime = Date.Now
                    .LastOperationDateTime = Date.Now
                    '.LastSurveyID = _surveyID
                    .InstanceCount += 1
                    rslt = True
                End With
            Else
                Me.MyEptSuffix = "Unknown"
                Using EvLog As New EventLog()
                    EvLog.Source = "ResultsSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("CountMe: EptSuffix " & Me.MyEptSuffix & " MyCSH isNothing ", EventLogEntryType.Error)
                End Using
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("CountMe: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function

    Private Sub DisposeMe()
        Try
            If Not IsNothing(Me.MyCustomerDBSvc) Then
                With Me.MyCustomerDBSvc
                    .Dispose()
                End With
                Me.MyCustomerDBSvc = Nothing
            End If
            'If Not IsNothing(Me.MySubscriberOps) Then
            '    With Me.MySubscriberOps
            '        .Dispose()
            '    End With
            '    Me.MySubscriberOps = Nothing
            'End If
            'If Not IsNothing(Me.MyPgItemColxnSvc) Then
            '    With Me.MyPgItemColxnSvc
            '        .Dispose()
            '    End With
            '    Me.MyPgItemColxnSvc = Nothing
            'End If

            Me.MyCSH = Nothing
            Me.MyCnxnString = Nothing
            Me.MyEptSuffix = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("DisposeMe: EptSuffix " & Me.MyEptSuffix & " Returns False ", EventLogEntryType.FailureAudit)
            End Using
        End Try
    End Sub

    Public Function Tellmeyouwork() As String Implements IResultsSvc.TellMeYouWork
        'ServiceMonitor.Update_ServiceCalls("Results  " & DateTime.Now.ToLongTimeString, "Tellmeyouwork")

        Return "I am working..."
    End Function


    Private Function SetUpCustomerDBSvc() As Boolean
        Dim rslt As Boolean = False
        Try
            Dim dcnxnstring = MyCSH.DC_Pkg.MyCustomerDBCnxnString
            If dcnxnstring IsNot Nothing Then
                MyCustomerDBSvc = New CustomerDBSvc.CustomerDBSvc
                MyCustomerDBSvc.DC_ConnectionString = dcnxnstring
                MyCustomerDBSvc.MyCSH = MyCSH
                rslt = True
            Else
                Using EvLog As New EventLog()
                    EvLog.Source = "ResultsSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("SetUpCustomerDBSvc: EptSuffix " & Me.MyEptSuffix & " dcnxnstring isNothing ", EventLogEntryType.FailureAudit)
                End Using
                MyCustomerDBSvc = Nothing
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SetUpCustomerDBSvc: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function

    Public Sub PruneResultsSummaries(ByVal _SurveyIDList As List(Of Integer)) Implements IResultsSvc.PruneResultsSummaries
        If _SurveyIDList IsNot Nothing Then
            If SetUpCustomerDBSvc() Then
                For Each _SurveyID In _SurveyIDList
                    'prune just this survey....
                    RemoveOrphanRsltSummariesandDetails(_SurveyID)
                    Me.MyCustomerDBSvc.UpdateResltsViewedMetaData(_SurveyID, False)
                Next
            End If
        End If

    End Sub

    Public Function RemoveOrphanRsltSummariesandDetails(ByVal _SurveyID As Integer) As Integer
        Dim rslt As Integer = 0
        'MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
        'Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        'Using db 'As New L2S_SurveyDataStoreDataContext
        '    'want to keep only those rsltsSummaries/Details that have a ResultsFilter...is possible that Summaries/Details are created 
        '    'but not represented by a ResultsFilter saved in the db...
        '    'this method should execute when the user LogsOff?...
        '    'for each rfgo in resultsfilters...
        '    '   get the resultsummaryID's that the list of resultsfilter(_surveyID) implies...these are the keepers...
        '    'next
        '    'For each resultssummary not in the Keeperslist
        '    '   get the associated resultsdetails....deletethem...and delete the resultssummary
        '    'next
        'End Using
        Return rslt
    End Function

#Region "Core Retrieve Methods...SDSResponseModels, Results"
    Public Function Retrieve_SDSResponseModels(ByVal _surveyID As Integer) As List(Of SDSResponseModelObject) Implements IResultsSvc.Retrieve_SDSResponseModels
        Dim rslt As List(Of SDSResponseModelObject) = Nothing

        If Me.CountMe() Then
            MyCSH.LastSurveyID = _surveyID
            Try
                MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID)
                If Not IsNothing(Me.MyCnxnString) Then
                    Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
                    Using db 'As New L2S_SurveyDataStoreDataContext
                        Dim q = From row In db.SDSResponseModels _
                                Where row.SurveyID = _surveyID _
                                Select New SDSResponseModelObject With {.SDSRespModelID = row.SDSResponseModelID, _
                                                                        .QuestID = row.QuestID, _
                                                                        .Key1 = row.Key1, _
                                                                        .Key2 = row.Key2, _
                                                                        .Key3 = row.Key3}
                        q.DefaultIfEmpty(Nothing)
                        rslt = q.ToList
                        q = Nothing
                    End Using
                    db = Nothing
                Else
                    Using EvLog As New EventLog()
                        EvLog.Source = "ResultsSvc"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("Retrieve_SDSResponseModels: EptSuffix " & Me.MyEptSuffix & " MyCnxnString isNothing ", EventLogEntryType.FailureAudit)
                    End Using
                End If

            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "ResultsSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Retrieve_SDSResponseModels: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
                End Using
            Finally
                Me.DisposeMe()
            End Try
        End If
        Return rslt
    End Function

    Public Function RetrieveResults(ByVal _SurveyID As Integer) As List(Of ResultsProviderSummaryObject) Implements IResultsSvc.RetrieveResults
        Dim rslt As List(Of ResultsProviderSummaryObject) = Nothing
        If CountMe() Then
            Try
                MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
                If Not IsNothing(Me.MyCnxnString) Then
                    Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
                    Using db 'As New L2S_SurveyDataStoreDataContext(MyCnxnString)
                        Try
                            Dim dlo = New DataLoadOptions
                            dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
                            db.LoadOptions = dlo

                            Dim sdsRms = db.SDSResponseModels.Where(Function(s) s.SurveyID = _SurveyID).AsEnumerable

                            Dim q = From rSum In db.ResultsSummaries.AsEnumerable _
                                    Where rSum.SurveyID = _SurveyID _
                                    AndAlso rSum.ResultsSummaryAddress = NoFilter _
                                    Select New ResultsProviderSummaryObject With {.ResultsDetailsList = ResultsProviderSummaryObject.ToSmallRsltDetails(rSum.ResultsDetails.Where(Function(rd) (rd.ResultsSummaryID = rSum.ResultsSummaryID) AndAlso (rd.RespDentCount > 0)).ToList, sdsRms), _
                                                                                  .ResultsSummaryID = rSum.ResultsSummaryID, _
                                                                                  .ResultsSummarySurveyID = rSum.SurveyID, _
                                                                                  .ResultsSummaryAddressKey = rSum.ResultsSummaryAddress, _
                                                                                  .AllSurveyRDENTSCount = RetrieveALLRDENTSCount(_SurveyID), _
                                                                                  .QuestionRDENTCountColxn = RetrieveQuestionRDENTCounts_AllRDENTS(_SurveyID)}
                            q.DefaultIfEmpty(Nothing)
                            rslt = q.ToList
                            If SetUpCustomerDBSvc() Then
                                Me.MyCustomerDBSvc.UpdateResltsViewedMetaData(_SurveyID, True)
                            End If
                            sdsRms = Nothing
                            q = Nothing
                        Catch ex As Exception
                            Using EvLog As New EventLog()
                                EvLog.Source = "ResultsSvc"
                                EvLog.Log = "Application"
                                EvLog.WriteEntry("RetrieveResults: EptSuffix " & Me.MyEptSuffix & "Inner Try Reports Error " & ex.Message, EventLogEntryType.Error)
                            End Using
                        End Try
                    End Using
                Else
                    Using EvLog As New EventLog()
                        EvLog.Source = "ResultsSvc"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("RetrieveResults: EptSuffix " & Me.MyEptSuffix & " MyCnxnString isNothing ", EventLogEntryType.FailureAudit)
                    End Using
                End If
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "ResultsSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveResults: EptSuffix " & Me.MyEptSuffix & "Outer Try Reports Error " & ex.Message, EventLogEntryType.Error)
                End Using
            Finally
                Me.DisposeMe()
            End Try
        End If

        Return rslt
    End Function
#End Region


#Region "Results Filters Stuff"
    Public Function Save_ResultsFilters(ByVal _SurveyID As Integer, ByVal _ListofResultsFilter As List(Of ResultsFilterModelObject)) As List(Of CmdInfrastructureNS.POCO_ID_Pkg) _
    Implements IResultsSvc.Save_ResultsFilters
        Dim rslt As New List(Of CmdInfrastructureNS.POCO_ID_Pkg)
        If CountMe() Then
            Try
                MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
                Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
                Using db 'As New L2S_SurveyDataStoreDataContext
                    Dim existingrfs = From rfmo In _ListofResultsFilter _
                            Where rfmo.ID <> 0 _
                            Select rfmo
                    For Each rfmo In existingrfs
                        Dim xrfmo = rfmo
                        Dim dbrfmo = From dbrfm In db.ResultsFilters _
                                     Where dbrfm.ID = xrfmo.ID AndAlso dbrfm.SurveyID = xrfmo.SurveyID _
                                     Select dbrfm
                        Dim row = dbrfmo.Single
                        row.Model = rfmo.Model
                        row.SurveyID = rfmo.SurveyID
                        db.SubmitChanges()
                        rslt.Add(New CmdInfrastructureNS.POCO_ID_Pkg With {.DB_ID = row.ID, _
                                                       .Original_ID = rfmo.ID, _
                                                       .POCOGuid = rfmo.Guid, _
                                                       .Survey_ID = rfmo.SurveyID})
                        row = Nothing
                        xrfmo = Nothing
                        dbrfmo = Nothing
                    Next

                    For Each rfmo In _ListofResultsFilter.Where(Function(rf) rf.ID = 0)
                        Dim newrf As New ResultsFilter With {.Model = rfmo.Model, _
                                                             .SurveyID = rfmo.SurveyID}
                        db.ResultsFilters.InsertOnSubmit(newrf)
                        db.SubmitChanges()
                        rslt.Add(New CmdInfrastructureNS.POCO_ID_Pkg With {.DB_ID = newrf.ID, _
                                                                           .Original_ID = rfmo.ID, _
                                                                           .POCOGuid = rfmo.Guid, _
                                                                           .Survey_ID = rfmo.SurveyID})
                        newrf = Nothing
                    Next
                    existingrfs = Nothing
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "ResultsSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Save_ResultsFilters: EptSuffix " & Me.MyEptSuffix & "Outer Try Reports Error " & ex.Message, EventLogEntryType.Error)
                End Using
                rslt.Clear()
            Finally
                Me.DisposeMe()
            End Try
        End If
        
        

        Return rslt
    End Function

    Public Function Delete_ResultsFilters(ByVal _SurveyID As Integer, ByVal _ListofResultsFilter As LinkedList(Of ResultsFilterModelObject)) As Boolean _
    Implements IResultsSvc.Delete_ResultsFilters
        Dim rslt As Boolean = False
        If CountMe() Then
            Try
                MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
                Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
                Using db 'As New L2S_SurveyDataStoreDataContext
                    Dim existingrfs = From rfmo In _ListofResultsFilter _
                            Where rfmo.ID <> 0 _
                            Select rfmo
                    For Each rfmo In existingrfs
                        Dim xrfmo = rfmo
                        Dim dbrfmo = From dbrfm In db.ResultsFilters _
                                     Where dbrfm.ID = xrfmo.ID AndAlso dbrfm.SurveyID = xrfmo.SurveyID _
                                     Select dbrfm
                        Dim row = dbrfmo.Single
                        db.ResultsFilters.DeleteOnSubmit(row)
                        db.SubmitChanges()
                        dbrfmo = Nothing
                        xrfmo = Nothing
                    Next
                    existingrfs = Nothing
                End Using
                rslt = True
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "ResultsSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Delete_ResultsFilters: EptSuffix " & Me.MyEptSuffix & "Outer Try Reports Error " & ex.Message, EventLogEntryType.Error)
                End Using
            Finally
                Me.DisposeMe()
            End Try
        End If
        Return rslt
    End Function

    Public Function Retrieve_ResultsFilters(ByVal _SurveyID As Integer) As List(Of ResultsFilterModelObject) Implements IResultsSvc.Retrieve_ResultsFilters
        Dim rslt As List(Of ResultsFilterModelObject) = Nothing
        If CountMe() Then
            Try
                MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
                Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
                Using db 'As New L2S_SurveyDataStoreDataContext
                    Dim q = From rf In db.ResultsFilters _
                            Where rf.SurveyID = _SurveyID _
                            Select New ResultsFilterModelObject With {.ID = rf.ID, _
                                                                      .Model = rf.Model, _
                                                                      .SurveyID = rf.SurveyID}
                    If q.Any Then
                        rslt = q.ToList
                    End If
                    q = Nothing
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "ResultsSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Retrieve_ResultsFilters: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
                End Using
            Finally
                Me.DisposeMe()
            End Try
        End If
        Return rslt
    End Function

    Private Function RetrieveRFGOList_with_RFMID(ByVal _SurveyID As Integer, _RFMID As Integer) As List(Of ResultsFilterGroupObject)
        Dim rslt As List(Of ResultsFilterGroupObject) = Nothing
        If IamOk() Then
            Try
                MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
                Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
                Using db 'As New L2S_SurveyDataStoreDataContext
                    Dim q = From rf In db.ResultsFilters _
                            Where rf.SurveyID = _SurveyID AndAlso rf.ID = _RFMID _
                            Select rf.Model
                    If q.Any Then
                        'neeed to read the xmldocument that is in rf.model...
                        'extract the part that is an <RFGOLIST>....then deserialize that into a RFGOList...
                        'Dim s As New Xml.Serialization.XmlSerializer(GetType(Object))

                        'For Each m In q
                        '    Dim rfm = s.Deserialize(m)
                        'Next

                    End If
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "ResultsSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveRFGOList_with_RFMID: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If

        Return rslt
    End Function
#End Region


#Region "RFGO METHODS"

    Public Function RetrieveFilteredResults_with_ListofResultFilterGroupObject(ByVal _SurveyID As Integer, ByVal _listofRFGO As List(Of ResultsFilterGroupObject)) _
    As List(Of ResultsProviderSummaryObject) Implements IResultsSvc.RetrieveFilteredResults_with_ListofResultFilterGroupObject

        Dim rslt As List(Of ResultsProviderSummaryObject) = Nothing
        If CountMe() Then
            Try
                Dim RSumAddress = CreateResultSummaryAddress_From_ListofResultsFilterGroupObject(_listofRFGO)
                If RetrieveResultsSummaryRowCommaString(_SurveyID, RSumAddress) Is Nothing Then
                    ' ServiceMonitor.Update_ServiceCalls("START " & DateTime.Now.ToLongTimeString, "CREATE RFGO ResultsDETAILS (SurveyID=<" & _SurveyID.ToString & ">)")
                    rslt = CreateRFGO_ResultsDetails(_SurveyID, _listofRFGO)

                Else
                    MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
                    Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
                    Using db 'As New L2S_SurveyDataStoreDataContext
                        Dim dlo = New DataLoadOptions
                        dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
                        db.LoadOptions = dlo
                        'ServiceMonitor.Update_ServiceCalls("Results  " & DateTime.Now.ToLongTimeString, "Retrieve RFGO Results(SurveyID=<" & _SurveyID.ToString & ">)")

                        Dim sdsRms = db.SDSResponseModels.Where(Function(s) s.SurveyID = _SurveyID).AsEnumerable

                        Dim address_String = CreateResultSummaryAddress_From_ListofResultsFilterGroupObject(_listofRFGO)
                        Dim q = From rSum In db.ResultsSummaries.AsEnumerable _
                                Where rSum.SurveyID = _SurveyID _
                                AndAlso rSum.ResultsSummaryAddress = address_String _
                                Select New ResultsProviderSummaryObject With {.ResultsDetailsList = ResultsProviderSummaryObject.ToSmallRsltDetails(rSum.ResultsDetails.Where(Function(rd) (rd.ResultsSummaryID = rSum.ResultsSummaryID) AndAlso (rd.RespDentCount > 0)).ToList, sdsRms), _
                                                                              .ResultsSummaryID = rSum.ResultsSummaryID, _
                                                                              .ResultsSummarySurveyID = rSum.SurveyID, _
                                                                              .ResultsSummaryAddressKey = rSum.ResultsSummaryAddress, _
                                                                          .AllSurveyRDENTSCount = RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(_listofRFGO).Count, _
                                                                          .SelectedSurveyRDENTSCount = RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(_listofRFGO).Count, _
                                                                          .QuestionRDENTCountColxn = RetrieveQuestionRDENTCOunts_usingListofRFGO(_SurveyID, _listofRFGO)}
                        q.DefaultIfEmpty(Nothing)

                        rslt = q.ToList
                        dlo = Nothing
                        q = Nothing
                        address_String = Nothing
                    End Using
                End If
                RSumAddress = Nothing
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "ResultsSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveFilteredResults_with_ListofResultFilterGroupObject: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
                End Using
            Finally
                Me.DisposeMe()
            End Try
        End If
        Return rslt
    End Function

    Private Function PopulateResultsDetails_UsingRDENTSIDList(ByVal _RsltSummary As ResultsSummary, ByVal _RDENTIDsList As List(Of Integer)) As List(Of ResultsDetail)
        Dim rslt As New List(Of ResultsDetail)
        Try
            'Dim _filter As List(Of Integer) = ResultSummary_AddressToList(_RsltSummary.ResultsSummaryAddress)
            Dim _SurveyID = _RsltSummary.SurveyID
            Dim _RsltSummaryID = _RsltSummary.ResultsSummaryID

            MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
            Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
            Using db 'As New L2S_SurveyDataStoreDataContext
                For Each sdsRSm In db.SDSResponseModels.Where(Function(srsm) srsm.SurveyID = _SurveyID)
                    Dim sdsrmID = sdsRSm.SDSResponseModelID
                    Dim CountofRdentsThatPass = (From rdent In db.SDSRespondentModels _
                                                 Where rdent.SurveyID = _SurveyID _
                                                 AndAlso _RDENTIDsList.Contains(rdent.RespondentModelID) _
                                                 AndAlso (From r In rdent.Responses _
                                                         Where r.ResponseModelID = sdsrmID _
                                                         Select r.ResponseModelID).Count > 0 _
                                                Select rdent.RespondentModelID).Count

                    Dim timestamp = DateTime.Now
                    Dim RsltDetail As ResultsDetail = FindResultsDetail(_RsltSummaryID, sdsrmID, _SurveyID)
                    If RsltDetail Is Nothing Then
                        RsltDetail = New ResultsDetail
                        With RsltDetail
                            .RespDentCount = CountofRdentsThatPass
                            .ResultsSummary = _RsltSummary
                            .SDSResponseModel = sdsRSm
                            .LastCountTimestamp = timestamp 'this will need the ResultSummaryID...
                        End With
                        rslt.Add(RsltDetail)
                        db.ResultsDetails.InsertOnSubmit(RsltDetail)
                    Else
                        With RsltDetail
                            .RespDentCount = CountofRdentsThatPass
                            .LastCountTimestamp = timestamp
                        End With
                    End If
                    RsltDetail = Nothing
                    timestamp = Nothing
                Next
                db.SubmitChanges()
            End Using
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("PopulateResultsDetails_UsingRDENTSIDList: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function

    Private Function CreateRFGO_ResultsDetails(ByVal _SurveyID As Integer, ByVal _listofRFGO As List(Of ResultsFilterGroupObject)) As List(Of ResultsProviderSummaryObject)
        Dim rslt As List(Of ResultsProviderSummaryObject) = Nothing
        Try
            Dim address_String = CreateResultSummaryAddress_From_ListofResultsFilterGroupObject(_listofRFGO)
            Dim rSum = New ResultsSummary
            rSum.SurveyID = _SurveyID
            rSum.ResultsSummaryAddress = address_String
            rSum.RFGOList = Serialize_ListOfRFGO(_listofRFGO)

            MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
            Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
            Using db 'As New L2S_SurveyDataStoreDataContext

                db.ResultsSummaries.InsertOnSubmit(rSum)
                'db.SubmitChanges()


                Dim includedRDENTSList = RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(_listofRFGO)
                Dim includedRDENTSListCount = includedRDENTSList.Count
                Dim rlstdetails = PopulateResultsDetails_UsingRDENTSIDList(rSum, includedRDENTSList)
                Dim sdsRms = db.SDSResponseModels.Where(Function(s) s.SurveyID = _SurveyID).AsEnumerable

                Dim RPSO As New ResultsProviderSummaryObject With {.ResultsDetailsList = ResultsProviderSummaryObject.ToSmallRsltDetails(rlstdetails, sdsRms), _
                                                                          .ResultsSummaryID = rSum.ResultsSummaryID, _
                                                                          .ResultsSummarySurveyID = rSum.SurveyID, _
                                                                          .ResultsSummaryAddressKey = rSum.ResultsSummaryAddress, _
                                                                          .AllSurveyRDENTSCount = includedRDENTSListCount, _
                                                                          .SelectedSurveyRDENTSCount = includedRDENTSListCount, _
                                                                          .QuestionRDENTCountColxn = RetrieveQuestionRDENTCOunts_usingListofRFGO(_SurveyID, _listofRFGO)}
                rslt = New List(Of ResultsProviderSummaryObject)
                rslt.Add(RPSO)
                rSum = Nothing
                address_String = Nothing
                rSum = Nothing
                includedRDENTSList = Nothing
                rlstdetails = Nothing
                includedRDENTSListCount = Nothing
            End Using
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("CreateRFGO_ResultsDetails: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

        Return rslt
    End Function

    Public Function RetrieveRDENTSID_Count_with_ListofResultFilterGroupObject(ByVal _ListofRFG As List(Of ResultsFilterGroupObject)) As Integer _
    Implements IResultsSvc.RetrieveRDENTSID_Count_with_ListofResultFilterGroupObject
        Return RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(_ListofRFG).Count
    End Function

    Private Function RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(ByRef _ListofRFG As List(Of ResultsFilterGroupObject)) As List(Of Integer)
        Dim rslt As List(Of Integer) = Nothing
        Try
            Dim lstOfList As New List(Of List(Of Integer))
            For Each rfg In _ListofRFG
                lstOfList.Add(RetrieveRDENTSID_List_QuestionList_OptionList(rfg.SurveyID, rfg.OptionIDList, rfg.QuestionIDList))
            Next
            Dim uniquelist = lstOfList.SelectMany(Function(lst) lst)
            rslt = (uniquelist.Distinct).ToList
            lstOfList.Clear()
            lstOfList = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RetrieveRDENTSID_List_with_ListofResultFilterGroupObject: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function

    'Private Function Retrieve_UnigueRDENTSID_List_with_ListofRDentList(ByVal _ListofRDENTIDList As List(Of List(Of Integer))) As List(Of Integer)
    '    Dim rslt As List(Of Integer) = Nothing
    '    Dim uniqueRDENTIDList = _ListofRDENTIDList.SelectMany(Function(lst) lst)
    '    rslt = (uniqueRDENTIDList.Distinct).ToList
    '    Return rslt
    'End Function

    Private Function CreateResultSummaryAddress_From_ListofResultsFilterGroupObject(ByRef _listofRFGO As List(Of ResultsFilterGroupObject)) As String
        Dim rslt As String = ""
        Try
            Dim strblder As New Text.StringBuilder
            'Dim ndx = 0
            Dim avgQ As Double = 0
            Dim avgOpt As Double = 0
            If _listofRFGO.Count > 0 Then
                strblder.Append(_listofRFGO.Count.ToString)
            End If
            For Each rfgo In _listofRFGO.OrderBy(Function(rf) rf.ToAddress)
                If rfgo.QuestionIDList.Count > 0 Then
                    avgQ = Math.Round(rfgo.QuestionIDList.Average, 1)
                    strblder.Append(rfgo.QuestionIDList.Count.ToString & "Qavg" & avgQ.ToString)
                End If
                If rfgo.OptionIDList.Count > 0 Then
                    avgOpt = Math.Round(rfgo.OptionIDList.Average, 1)
                    strblder.Append(rfgo.OptionIDList.Count.ToString & "Oavg" & avgOpt.ToString)
                End If
            Next
            rslt = strblder.ToString
            strblder = Nothing
            avgOpt = Nothing
            avgQ = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("CreateResultSummaryAddress_From_ListofResultsFilterGroupObject: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

        Return rslt
    End Function

    Public Function RetrieveRDENTSCount_QuestionList_OptionList(ByVal _surveyID As Integer, ByVal _Optionfilter As List(Of Integer), ByVal _QuestionFilter As List(Of Integer)) _
                                                                 As Integer Implements IResultsSvc.RetrieveRDENTSCount_QuestionList_OptionList
        Dim rslt As Integer
        Try
            MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID)
            Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
            Using db 'As New L2S_SurveyDataStoreDataContext
                rslt = (From rdent In db.SDSRespondentModels _
                          Where rdent.SurveyID = _surveyID AndAlso _
                          (From rspnse In rdent.Responses _
                                 Where _Optionfilter.Contains(rspnse.ResponseModelID) _
                                 Select rspnse.ResponseModelID Distinct).Count = _Optionfilter.Count _
                          AndAlso (From rspnse In rdent.Responses _
                                 Where _QuestionFilter.Contains(rspnse.SDSResponseModel.QuestID) _
                                 Select rspnse.SDSResponseModel.QuestID Distinct).Count = _QuestionFilter.Count _
                          Select rdent).Count
            End Using
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RetrieveRDENTSCount_QuestionList_OptionList: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

        Return rslt
    End Function

    Private Function RetrieveRDENTSID_List_QuestionList_OptionList(ByVal _surveyID As Integer, ByVal _Optionfilter As List(Of Integer), ByVal _QuestionFilter As List(Of Integer)) _
                                                                As List(Of Integer)
        Dim rslt As List(Of Integer)

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            rslt = (From rdent In db.SDSRespondentModels _
                      Where rdent.SurveyID = _surveyID AndAlso _
                      (From rspnse In rdent.Responses _
                             Where _Optionfilter.Contains(rspnse.ResponseModelID) _
                             Select rspnse.ResponseModelID Distinct).Count = _Optionfilter.Count _
                      AndAlso (From rspnse In rdent.Responses _
                             Where _QuestionFilter.Contains(rspnse.SDSResponseModel.QuestID) _
                             Select rspnse.SDSResponseModel.QuestID Distinct).Count = _QuestionFilter.Count _
                      Select rdent.RespondentModelID Distinct).ToList
        End Using
        Return rslt
    End Function

    Public Function RetrieveQuestionRDENTCOunts_usingListofRFGO(ByVal _surveyID As Integer, ByVal _listofRFGO As List(Of ResultsFilterGroupObject)) _
    As List(Of QuestionRDENTCountObject) Implements IResultsSvc.RetrieveQuestionRDENTCOunts_usingListofRFGO
        Dim rslt As List(Of QuestionRDENTCountObject) = Nothing
        Try
            'returns a list of questionID and RDENTCount for the QuestionID
            Dim rdentslist = RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(_listofRFGO)

            MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID)
            Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
            Using db 'As New L2S_SurveyDataStoreDataContext
                Dim q = From item In (From sdsrspmodel In db.SDSResponseModels _
                                      Where sdsrspmodel.SurveyID = _surveyID _
                                    Let rsp = (From rsponse In sdsrspmodel.Responses _
                                               Where rdentslist.Contains(rsponse.RespondentID) _
                                               Select rsponse.RespondentID Distinct) _
                                    Group rsp By sdsrspmodel.QuestID Into Group) _
                        Select New QuestionRDENTCountObject With {.QuestionID = item.QuestID, _
                                                                  .RDENTCount = (item.Group.SelectMany(Function(c) c.ToList)).Distinct.Count, _
                                                                  .SurveyID = _surveyID}

                If q.Any Then
                    rslt = q.ToList
                End If
                q = Nothing
            End Using
            rdentslist = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RetrieveQuestionRDENTCOunts_usingListofRFGO: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function

#Region "ResultSummaryListOfRFGO Serialize/De methods"
    Private Function Serialize_ListOfRFGO(ByVal _listOfRFGO As List(Of ResultsFilterGroupObject)) As Byte()
        Dim rslt As Byte() = Nothing
        Try
            Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            Using ms As New IO.MemoryStream
                bf.Serialize(ms, _listOfRFGO)
                ms.Seek(0, IO.SeekOrigin.Begin)
                rslt = ms.ToArray
            End Using
            bf = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Serialize_ListOfRFGO: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function

    Private Function DeSerializeToListOfRFGO(ByVal _rfgolistBarray As Byte()) As List(Of ResultsFilterGroupObject)
        Dim rslt As List(Of ResultsFilterGroupObject) = Nothing
        Try
            Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            Using ms As New IO.MemoryStream(_rfgolistBarray)
                rslt = bf.Deserialize(ms)
            End Using
            bf = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ResultsSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("DeSerializeToListOfRFGO: EptSuffix " & Me.MyEptSuffix & " Reports Error " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
        Return rslt
    End Function
#End Region
#End Region


#Region "Find Row and Address methods...."

    Private Function FindResultsDetail(ByVal _rsltSummaryID As Integer, ByVal _sdsResponseModelID As Integer, ByVal _SurveyID As Integer) As ResultsDetail
        Dim rsltDtl As ResultsDetail = Nothing
        'need to try and find a ResultsDetail with the ResponseId and ResultSummaryID = ResultSummary.where ResultsSummarKey="NoFilter/Default"

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            Dim q = From rsltsummary In db.ResultsSummaries _
                    Where rsltsummary.SurveyID = _SurveyID _
            From rd In rsltsummary.ResultsDetails _
            Where rd.SDSResponseModelID = _sdsResponseModelID _
            AndAlso rsltsummary.ResultsSummaryID = _rsltSummaryID _
            Select rd
            q.DefaultIfEmpty(Nothing)
            rsltDtl = q.FirstOrDefault
        End Using
        Return rsltDtl
    End Function

    Public Function RetrieveResultsSummaryRow_with_CommaString(ByVal _SurveyID As Integer, ByVal _AddressCommaDelimitedString As String) As ResultsSummary Implements IResultsSvc.RetrieveResultsCommaString
        Dim rslt As ResultsSummary = Nothing

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            'Dim dlo = New DataLoadOptions
            'dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
            'db.LoadOptions = dlo

            Dim q = From rSum In db.ResultsSummaries _
                    Where rSum.SurveyID = _SurveyID _
                    AndAlso rSum.ResultsSummaryAddress = _AddressCommaDelimitedString _
                    Select rSum
            q.DefaultIfEmpty(rslt)
            rslt = q.FirstOrDefault
        End Using
        Return rslt
    End Function

    Public Function RetrieveResultsSummaryRow_with_ItegerList(ByVal _SurveyID As Integer, ByVal _AddressList As System.Collections.Generic.List(Of Integer)) As ResultsSummary Implements IResultsSvc.RetrieveResultsItegerList
        Dim rslt As ResultsSummary = Nothing

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            'Dim dlo = New DataLoadOptions
            'dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
            'db.LoadOptions = dlo

            Dim address_String = ResultsSummaryAddressList_ToString(_AddressList)
            Dim q = From rSum In db.ResultsSummaries _
                    Where rSum.SurveyID = _SurveyID _
                    AndAlso rSum.ResultsSummaryAddress = address_String _
                    Select rSum
            q.DefaultIfEmpty(rslt)
            rslt = q.FirstOrDefault
            address_String = Nothing
        End Using
        Return rslt
    End Function

    Public Function RetrieveResultsMetaData(ByVal _SurveyID As Integer) As String Implements IResultsSvc.RetrieveResultsMetaData
        Dim rslt As String = "NotImplemented"
        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext

        End Using
        Return rslt
    End Function

    Public Function RetrieveResultsMetaData_SDS() As String Implements IResultsSvc.RetrieveResultsMetaData_SDS
        Dim rslt As String = "NotImplemented"
        'Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        'Using db 'As New L2S_SurveyDataStoreDataContext

        'End Using
        Return rslt
    End Function

    Public Function RetrieveResultsSummaryRowCommaString(ByVal _SurveyID As Integer, ByVal _AddressCommaDelimitedString As String) As ResultsSummary Implements IResultsSvc.RetrieveResultsSummaryRowCommaString
        Dim rslt As ResultsSummary = Nothing

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            'Dim dlo = New DataLoadOptions
            'dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
            'db.LoadOptions = dlo

            Dim q = From rSum In db.ResultsSummaries _
                    Where rSum.SurveyID = _SurveyID _
                    AndAlso rSum.ResultsSummaryAddress = _AddressCommaDelimitedString _
                    Select rSum
            q.DefaultIfEmpty(rslt)
            rslt = q.FirstOrDefault
        End Using
        Return rslt
    End Function

    Public Function RetrieveResultsSummaryRowIntegerList(ByVal _SurveyID As Integer, ByVal _AddressList As System.Collections.Generic.List(Of Integer)) As ResultsSummary Implements IResultsSvc.RetrieveResultsSummaryRowIntegerList
        Dim rslt As ResultsSummary = Nothing

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            'Dim dlo = New DataLoadOptions
            ' dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
            ' db.LoadOptions = dlo

            Dim address_String = ResultsSummaryAddressList_ToString(_AddressList)
            Dim q = From rSum In db.ResultsSummaries _
                    Where rSum.SurveyID = _SurveyID _
                    AndAlso rSum.ResultsSummaryAddress = address_String _
                    Select rSum
            q.DefaultIfEmpty(rslt)
            rslt = q.FirstOrDefault
            address_String = Nothing
        End Using
        Return rslt
    End Function

    Public Function RetrieveResultsSummaryRowWithID(ByVal _ResultSummaryID As Integer) As ResultsSummary Implements IResultsSvc.RetrieveResultsSummaryRowWithID
        Dim rslt As ResultsSummary = Nothing

        'MyCnxnString = MyCSH.DC_Cnxn_For_SurveyID(_SurveyID)
        'Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        'Using db 'As New L2S_SurveyDataStoreDataContext
        '    'Dim dlo = New DataLoadOptions
        '    ' dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
        '    ' db.LoadOptions = dlo

        '    Dim q = From rSum In db.ResultsSummaries _
        '            Where rSum.ResultsSummaryID = _ResultSummaryID _
        '            Select rSum
        '    q.DefaultIfEmpty(rslt)
        '    rslt = q.FirstOrDefault
        'End Using
        Return rslt
    End Function


    Private Function ResultsSummaryAddressList_ToString(ByVal _ResultsSummaryList As List(Of Integer)) As String
        Dim rslt = ""

        For Each int_item In _ResultsSummaryList
            rslt += int_item.ToString
            If Not int_item.Equals(_ResultsSummaryList.Last) Then
                rslt += ","
            End If
        Next

        Return rslt
    End Function

#End Region


#Region "Non RFGO methods "
    Public Function RetrieveFilteredResultsWithIntegerList(ByVal _SurveyID As Integer, ByVal _AddressList As List(Of Integer)) _
    As List(Of ResultsProviderSummaryObject) _
    Implements IResultsSvc.RetrieveFilteredResultsWithIntegerList
        Dim rslt As List(Of ResultsProviderSummaryObject) = Nothing

        If RetrieveResultsSummaryRow_with_ItegerList(_SurveyID, _AddressList) Is Nothing Then
            'ServiceMonitor.Update_ServiceCalls("START " & DateTime.Now.ToLongTimeString, "CREATE ResultsDETAILS (SurveyID=<" & _SurveyID.ToString & ">)")
            rslt = CreateResultDetails(_SurveyID, _AddressList)

        Else
            MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
            Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
            Using db 'As New L2S_SurveyDataStoreDataContext
                Dim dlo = New DataLoadOptions
                dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
                db.LoadOptions = dlo
                'ServiceMonitor.Update_ServiceCalls("Results  " & DateTime.Now.ToLongTimeString, "RetrieveResults(SurveyID=<" & _SurveyID.ToString & ">)")
                Dim sdsRms = db.SDSResponseModels.Where(Function(s) s.SurveyID = _SurveyID).AsEnumerable
                Dim address_String = ResultsSummaryAddressList_ToString(_AddressList)
                Dim q = From rSum In db.ResultsSummaries.AsEnumerable _
                        Where rSum.SurveyID = _SurveyID _
                        AndAlso rSum.ResultsSummaryAddress = address_String _
                        Select New ResultsProviderSummaryObject With {.ResultsDetailsList = ResultsProviderSummaryObject.ToSmallRsltDetails(rSum.ResultsDetails.Where(Function(rd) rd.ResultsSummaryID = rSum.ResultsSummaryID).ToList, sdsRms), _
                                                                      .ResultsSummaryID = rSum.ResultsSummaryID, _
                                                                      .ResultsSummarySurveyID = rSum.SurveyID, _
                                                                      .ResultsSummaryAddressKey = rSum.ResultsSummaryAddress, _
                                                                  .AllSurveyRDENTSCount = RetrieveALLRDENTSCount(_SurveyID), _
                                                                  .SelectedSurveyRDENTSCount = RetrieveSelectedRDENTSCount(_SurveyID, _AddressList), _
                                                                  .QuestionRDENTCountColxn = RetrieveQuestionRDENTCOunts_SelectedRDENTS(_SurveyID, _AddressList)}
                q.DefaultIfEmpty(Nothing)


                rslt = q.ToList
            End Using

        End If
        'If rslt IsNot Nothing Then
        '    'ServiceMonitor.Update_ServiceCalls("Results.RetrieveResults  " & DateTime.Now.ToLongTimeString, " Count=<" & rslt.Count & ">")
        'Else
        '    'ServiceMonitor.Update_ServiceCalls("Results.RetrieveResults  " & DateTime.Now.ToLongTimeString, " <Empty Query Result>")
        'End If
        Return rslt

    End Function
    Public Function CreateResultDetails(ByVal _surveyid As Integer, ByVal _addresslist As List(Of Integer)) As List(Of ResultsProviderSummaryObject)
        Dim address_String = ResultsSummaryAddressList_ToString(_addresslist)
        Dim rSum = New ResultsSummary
        rSum.SurveyID = _surveyid
        rSum.ResultsSummaryAddress = address_String

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyid)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext

            db.ResultsSummaries.InsertOnSubmit(rSum)
            'db.SubmitChanges()

        End Using
        Dim rlstdetails = PopulateResultsDetailsRDENTCount(rSum, _addresslist)
        Dim sdsRms = db.SDSResponseModels.Where(Function(s) s.SurveyID = _surveyid).AsEnumerable
        Dim RPSO As New ResultsProviderSummaryObject With {.ResultsDetailsList = ResultsProviderSummaryObject.ToSmallRsltDetails(rlstdetails, sdsRms), _
                                                                  .ResultsSummaryID = rSum.ResultsSummaryID, _
                                                                  .ResultsSummarySurveyID = rSum.SurveyID, _
                                                                  .ResultsSummaryAddressKey = rSum.ResultsSummaryAddress, _
                                                                  .AllSurveyRDENTSCount = RetrieveALLRDENTSCount(_surveyid), _
                                                                  .SelectedSurveyRDENTSCount = RetrieveSelectedRDENTSCount(_surveyid, _addresslist), _
                                                                  .QuestionRDENTCountColxn = RetrieveQuestionRDENTCOunts_SelectedRDENTS(_surveyid, _addresslist)}
        Dim rslt As New List(Of ResultsProviderSummaryObject)
        rslt.Add(RPSO)
        Return rslt
    End Function
    Private Function PopulateResultsDetailsRDENTCount(ByVal _RsltSummary As ResultsSummary, ByVal _filter As List(Of Integer)) As List(Of ResultsDetail)
        Dim rslt As New List(Of ResultsDetail)
        Try
            'Dim _filter As List(Of Integer) = ResultSummary_AddressToList(_RsltSummary.ResultsSummaryAddress)
            Dim _SurveyID = _RsltSummary.SurveyID
            Dim _RsltSummaryID = _RsltSummary.ResultsSummaryID

            MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
            Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
            Using db 'As New L2S_SurveyDataStoreDataContext
                For Each sdsRSm In db.SDSResponseModels
                    Dim sdsrmID = sdsRSm.SDSResponseModelID
                    Dim CountofRdentsThatPass = (From rdent In db.SDSRespondentModels _
                                                 Where rdent.SurveyID = _SurveyID _
                                                 AndAlso (From resp In rdent.Responses _
                                                          Where _filter.Contains(resp.ResponseModelID) _
                                                          Select resp.ResponseModelID Distinct).Count = _filter.Count _
                                                AndAlso (From r In rdent.Responses _
                                                         Where r.ResponseModelID = sdsrmID _
                                                         Select r.ResponseModelID).Count > 0 _
                                                Select rdent.RespondentModelID).Count

                    Dim timestamp = DateTime.Now
                    Dim RsltDetail As ResultsDetail = FindResultsDetail(_RsltSummaryID, sdsrmID, _SurveyID)
                    If RsltDetail Is Nothing Then
                        RsltDetail = New ResultsDetail
                        With RsltDetail
                            .RespDentCount = CountofRdentsThatPass
                            .ResultsSummary = _RsltSummary
                            .SDSResponseModel = sdsRSm
                            .LastCountTimestamp = timestamp 'this will need the ResultSummaryID...
                        End With
                        rslt.Add(RsltDetail)
                        db.ResultsDetails.InsertOnSubmit(RsltDetail)
                    Else
                        With RsltDetail
                            .RespDentCount = CountofRdentsThatPass
                            .LastCountTimestamp = timestamp
                        End With
                    End If
                Next
                db.SubmitChanges()
            End Using
        Catch ex As Exception
            'ServiceMonitor.Update_ServiceCalls(ex.Message, "PopulateResultsDetailsRDENTCount encountered error..." & _RsltSummary.ResultsSummaryAddress & " SurveyID=" & _RsltSummary.SurveyID.ToString)
            CmdInfrastructureNS.SharedEvents.RaiseOperationFailed("RsltsSvc", "PopulateResultsDetailsRDENTCount")
        End Try
        Return rslt
    End Function

    Private Function RetrieveALLRDENTSCount(ByVal _SurveyID As Integer) As Integer
        Dim rslt As Integer = 0
        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            rslt = (From rdent In db.SDSRespondentModels _
                    Where rdent.SurveyID = _SurveyID _
                       Select rdent).Count
        End Using
        Return rslt
    End Function

    Private Function RetrieveSelectedRDENTSCount(ByVal _SurveyID As Integer, ByVal _filter As List(Of Integer)) As Integer
        Dim rslt As Integer = 0

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            rslt = (From rdent In db.SDSRespondentModels _
                    Where rdent.SurveyID = _SurveyID AndAlso _
                          (From rspnse In rdent.Responses _
                           Where _filter.Contains(rspnse.ResponseModelID) _
                           Select rspnse.ResponseModelID Distinct).Count = _filter.Count _
                    Select rdent).Count
        End Using
        Return rslt
    End Function

    Public Function RetrieveQuestionRDENTCounts_AllRDENTS(ByVal _SurveyID As Integer) As List(Of QuestionRDENTCountObject)
        Dim rslt As List(Of QuestionRDENTCountObject) = Nothing

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        'returns a list of questionID and RDENTCount for the QuestionID
        Using db 'As New L2S_SurveyDataStoreDataContext

            Dim q = From item In (From sdsrspmodel In db.SDSResponseModels _
                                  Where sdsrspmodel.SurveyID = _SurveyID _
                                Let rsp = (From rsponse In sdsrspmodel.Responses _
                                           Select rsponse.RespondentID Distinct) _
                                Group rsp By sdsrspmodel.QuestID Into Group) _
                    Select New QuestionRDENTCountObject With {.QuestionID = item.QuestID, _
                                                              .RDENTCount = (item.Group.SelectMany(Function(c) c.ToList)).Distinct.Count, _
                                                              .SurveyID = _SurveyID}

            If q.Any Then
                rslt = q.ToList
            End If
        End Using
        Return rslt
    End Function

    Public Function RetrieveQuestionRDENTCOunts_SelectedRDENTS(ByVal _surveyID As Integer, ByVal _Filter As List(Of Integer)) As List(Of QuestionRDENTCountObject)
        Dim rslt As List(Of QuestionRDENTCountObject) = Nothing

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        'returns a list of questionID and RDENTCount for the QuestionID
        Using db 'As New L2S_SurveyDataStoreDataContext

            Dim q = From item In (From sdsrspmodel In db.SDSResponseModels _
                                  Where sdsrspmodel.SurveyID = _surveyID _
                                Let rsp = (From rsponse In sdsrspmodel.Responses _
                                           Where _Filter.Contains(rsponse.ResponseModelID) _
                                           Select rsponse.RespondentID Distinct) _
                                Group rsp By sdsrspmodel.QuestID Into Group) _
                    Select New QuestionRDENTCountObject With {.QuestionID = item.QuestID, _
                                                              .RDENTCount = (item.Group.SelectMany(Function(c) c.ToList)).Distinct.Count, _
                                                              .SurveyID = _surveyID}

            If q.Any Then
                rslt = q.ToList
            End If
        End Using
        Return rslt
    End Function

    Private Function RetrieveAllRDENTSCount_AnsweringQuestion(ByVal _surveyID As Integer, ByVal _questID As Integer) As Integer
        Dim rslt As Integer = 0

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            Dim rsponsemodelid_list = From sdsrspnsemodel In db.SDSResponseModels _
                                      Where sdsrspnsemodel.QuestID = _questID AndAlso sdsrspnsemodel.SurveyID = _surveyID _
                                      Select sdsrspnsemodel.SDSResponseModelID Distinct

            rslt = (From rdent In db.SDSRespondentModels _
                    Where (From rspnse In rdent.Responses _
                           Where rsponsemodelid_list.Contains(rspnse.ResponseModelID) _
                           Select rspnse.ResponseModelID Distinct).Count > 0 _
                    Select rdent).Count
        End Using
        Return rslt
    End Function

    Private Function RetrieveSelectedRDENTSCount_AnsweringQuestion(ByVal _surveyID As Integer, ByVal _filter As List(Of Integer), ByVal _questID As Integer) As Integer
        Dim rslt As Integer = 0

        MyCnxnString = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            Dim rsponsemodelid_list = From sdsrspnsemodel In db.SDSResponseModels _
                                      Where sdsrspnsemodel.QuestID = _questID AndAlso sdsrspnsemodel.SurveyID = _surveyID _
                                      Select sdsrspnsemodel.SDSResponseModelID Distinct

            rslt = (From rdent In db.SDSRespondentModels _
                    Where rdent.SurveyID = _surveyID AndAlso _
                    (From rspnse In rdent.Responses _
                           Where _filter.Contains(rspnse.ResponseModelID) _
                           Select rspnse.ResponseModelID Distinct).Count = _filter.Count _
                    AndAlso (From rspnse In rdent.Responses _
                           Where rsponsemodelid_list.Contains(rspnse.ResponseModelID) _
                           Select rspnse.ResponseModelID Distinct).Count > 0 _
                    Select rdent).Count
        End Using
        Return rslt
    End Function
#End Region
End Class
