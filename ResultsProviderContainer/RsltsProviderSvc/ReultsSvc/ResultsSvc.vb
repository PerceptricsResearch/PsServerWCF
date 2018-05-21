Imports System.Data.Linq

' NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
Public Class ResultsSvc
    Implements IResultsSvc


    Private MyCnxnString As String = "Data Source=LEASES\DEVRENTS;Initial Catalog=q_q_comSurveyDataStore_0;Integrated Security=True"
    Const NoFilter As String = "NoFilter"
    'Private gldb As New L2S_SurveyDataStoreDataContext
    Public Function Tellmeyouwork() As String Implements IResultsSvc.TellMeYouWork
        ServiceMonitor.Update_ServiceCalls("Results  " & DateTime.Now.ToLongTimeString, "Tellmeyouwork")

        Return "I am working..."
    End Function
    Public Function Retrieve_SDSResponseModels(ByVal _surveyID As Integer) As List(Of SDSResponseModelObject) Implements IResultsSvc.Retrieve_SDSResponseModels
        Dim rslt As List(Of SDSResponseModelObject) = Nothing
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
        End Using
        Return rslt
    End Function

    Public Function Save_ResultsFilters(ByVal _SurveyID As Integer, ByVal _ListofResultsFilter As List(Of ResultsFilterModelObject)) As Boolean _
    Implements IResultsSvc.Save_ResultsFilters
        Dim rslt As Boolean = False
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            Try
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
                Next

                Dim newrfs = From rfmo In _ListofResultsFilter _
                             Where rfmo.ID = 0 _
                             Select New ResultsFilter With {.Model = rfmo.Model, _
                                                            .SurveyID = rfmo.SurveyID}
                db.ResultsFilters.InsertAllOnSubmit(newrfs)
                db.SubmitChanges()
                rslt = True
            Catch ex As Exception
                rslt = False
            End Try
        End Using
        Return rslt
    End Function


    Public Function Retrieve_ResultsFilters(ByVal _SurveyID As Integer) As List(Of ResultsFilterModelObject) Implements IResultsSvc.Retrieve_ResultsFilters
        Dim rslt As List(Of ResultsFilterModelObject) = Nothing
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
        End Using
        Return rslt
    End Function

    Public Function RetrieveResults(ByVal _SurveyID As Integer) As List(Of ResultsProviderSummaryObject) Implements IResultsSvc.RetrieveResults
        Dim rslt As List(Of ResultsProviderSummaryObject) = Nothing
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext(MyCnxnString)
            Dim dlo = New DataLoadOptions
            dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
            db.LoadOptions = dlo
            ServiceMonitor.Update_ServiceCalls("Results  " & DateTime.Now.ToLongTimeString, "RetrieveResults(SurveyID=<" & _SurveyID.ToString & ">)")

            Dim q = From rSum In db.ResultsSummaries.AsEnumerable _
                    Where rSum.SurveyID = _SurveyID _
                    AndAlso rSum.ResultsSummaryAddress = NoFilter _
                    Select New ResultsProviderSummaryObject With {.ResultsDetailsList = ResultsProviderSummaryObject.ToSmallRsltDetails(rSum.ResultsDetails.ToList), _
                                                                  .ResultsSummaryID = rSum.ResultsSummaryID, _
                                                                  .ResultsSummarySurveyID = rSum.SurveyID, _
                                                                  .ResultsSummaryAddressKey = rSum.ResultsSummaryAddress, _
                                                                  .AllSurveyRDENTSCount = RetrieveALLRDENTSCount(_SurveyID), _
                                                                  .QuestionRDENTCountColxn = RetrieveQuestionRDENTCounts_AllRDENTS(_SurveyID)}
            q.DefaultIfEmpty(Nothing)

            rslt = q.ToList
            If rslt IsNot Nothing Then
                ServiceMonitor.Update_ServiceCalls("Results.RetrieveResults  " & DateTime.Now.ToLongTimeString, " Count=<" & rslt.Count & ">")
            Else
                ServiceMonitor.Update_ServiceCalls("Results.RetrieveResults  " & DateTime.Now.ToLongTimeString, " <Empty Query Result>")
            End If

        End Using
        Return rslt
    End Function

    Public Function RetrieveFilteredResultsWithIntegerList(ByVal _SurveyID As Integer, ByVal _AddressList As List(Of Integer)) _
    As List(Of ResultsProviderSummaryObject) _
    Implements IResultsSvc.RetrieveFilteredResultsWithIntegerList
        Dim rslt As List(Of ResultsProviderSummaryObject) = Nothing

        If RetrieveResultsSummaryRow_with_ItegerList(_SurveyID, _AddressList) Is Nothing Then
            ServiceMonitor.Update_ServiceCalls("START " & DateTime.Now.ToLongTimeString, "CREATE ResultsDETAILS (SurveyID=<" & _SurveyID.ToString & ">)")
            rslt = CreateResultDetails(_SurveyID, _AddressList)

        Else
            Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
            Using db 'As New L2S_SurveyDataStoreDataContext
                Dim dlo = New DataLoadOptions
                dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
                db.LoadOptions = dlo
                ServiceMonitor.Update_ServiceCalls("Results  " & DateTime.Now.ToLongTimeString, "RetrieveResults(SurveyID=<" & _SurveyID.ToString & ">)")

                Dim address_String = ResultsSummaryAddressList_ToString(_AddressList)
                Dim q = From rSum In db.ResultsSummaries.AsEnumerable _
                        Where rSum.SurveyID = _SurveyID _
                        AndAlso rSum.ResultsSummaryAddress = address_String _
                        Select New ResultsProviderSummaryObject With {.ResultsDetailsList = ResultsProviderSummaryObject.ToSmallRsltDetails(rSum.ResultsDetails.ToList), _
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
        If rslt IsNot Nothing Then
            ServiceMonitor.Update_ServiceCalls("Results.RetrieveResults  " & DateTime.Now.ToLongTimeString, " Count=<" & rslt.Count & ">")
        Else
            ServiceMonitor.Update_ServiceCalls("Results.RetrieveResults  " & DateTime.Now.ToLongTimeString, " <Empty Query Result>")
        End If
        Return rslt
    End Function
    Public Function CreateResultDetails(ByVal _surveyid As Integer, ByVal _addresslist As List(Of Integer)) As List(Of ResultsProviderSummaryObject)
        Dim address_String = ResultsSummaryAddressList_ToString(_addresslist)
        Dim rSum = New ResultsSummary
        rSum.SurveyID = _surveyid
        rSum.ResultsSummaryAddress = address_String
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext

            db.ResultsSummaries.InsertOnSubmit(rSum)
            'db.SubmitChanges()

        End Using
        Dim rlstdetails = PopulateResultsDetailsRDENTCount(rSum, _addresslist)
        Dim RPSO As New ResultsProviderSummaryObject With {.ResultsDetailsList = ResultsProviderSummaryObject.ToSmallRsltDetails(rlstdetails), _
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
                    Dim RsltDetail As ResultsDetail = FindResultsDetail(_RsltSummaryID, sdsrmID)
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
            ServiceMonitor.Update_ServiceCalls(ex.Message, "PopulateResultsDetailsRDENTCount encountered error..." & _RsltSummary.ResultsSummaryAddress & " SurveyID=" & _RsltSummary.SurveyID.ToString)

        End Try
        Return rslt
    End Function

    Private Function RetrieveALLRDENTSCount(ByVal _SurveyID As Integer) As Integer
        Dim rslt As Integer = 0

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





            'Dim q = From sdsrmodel In db.SDSResponseModels 
            '        Let rsp = (From rsponse In sdsrmodel.Responses _
            '                      Select rsponse.RespondentID Distinct) _
            '        Select New QuestionRDENTCountObject With {.QuestionID = sdsrmodel.QuestID, _
            '                                         .RDENTCount = rcount, _
            '                                          .SurveyID = _SurveyID} Distinct
            If q.Any Then
                rslt = q.ToList
            End If
        End Using
        Return rslt
    End Function

    Public Function RetrieveQuestionRDENTCOunts_SelectedRDENTS(ByVal _surveyID As Integer, ByVal _Filter As List(Of Integer)) As List(Of QuestionRDENTCountObject)
        Dim rslt As List(Of QuestionRDENTCountObject) = Nothing
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        'returns a list of questionID and RDENTCount for the QuestionID
        Using db 'As New L2S_SurveyDataStoreDataContext
            'Dim q = From sdsrmodel In db.SDSResponseModels _
            '        Let rcount = (From rsponse In sdsrmodel.Responses _
            '                      Where _Filter.Contains(rsponse.ResponseModelID) _
            '                      Select rsponse.RespondentID Distinct.Count) _
            '        Select New QuestionRDENTCountObject With {.QuestionID = sdsrmodel.QuestID, _
            '                                                  .RDENTCount = rcount, _
            '                                                  .SurveyID = _surveyID} Distinct
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
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            Dim rsponsemodelid_list = From sdsrspnsemodel In db.SDSResponseModels _
                                      Where sdsrspnsemodel.QuestID = _questID AndAlso sdsrspnsemodel.SurveyID = _surveyID _
                                      Select sdsrspnsemodel.SDSResponseModelID Distinct

            rslt = (From rdent In db.SDSRespondentModels _
                    Where (From rspnse In rdent.Responses _
                           Where _filter.Contains(rspnse.ResponseModelID) _
                           Select rspnse.ResponseModelID Distinct).Count = _filter.Count _
                    AndAlso (From rspnse In rdent.Responses _
                           Where rsponsemodelid_list.Contains(rspnse.ResponseModelID) _
                           Select rspnse.ResponseModelID Distinct).Count > 0 _
                    Select rdent).Count
        End Using
        Return rslt
    End Function


#Region "RFGO METHODS"

    Public Function RetrieveFilteredResults_with_ListofResultFilterGroupObject(ByVal _SurveyID As Integer, ByVal _listofRFGO As List(Of ResultsFilterGroupObject)) _
    As List(Of ResultsProviderSummaryObject) Implements IResultsSvc.RetrieveFilteredResults_with_ListofResultFilterGroupObject

        Dim rslt As List(Of ResultsProviderSummaryObject) = Nothing
        Dim RSumAddress = CreateResultSummaryAddress_From_ListofResultsFilterGroupObject(_listofRFGO)
        If RetrieveResultsSummaryRowCommaString(_SurveyID, RSumAddress) Is Nothing Then
            ServiceMonitor.Update_ServiceCalls("START " & DateTime.Now.ToLongTimeString, "CREATE RFGO ResultsDETAILS (SurveyID=<" & _SurveyID.ToString & ">)")
            rslt = CreateRFGO_ResultsDetails(_SurveyID, _listofRFGO)

        Else
            Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
            Using db 'As New L2S_SurveyDataStoreDataContext
                Dim dlo = New DataLoadOptions
                dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
                db.LoadOptions = dlo
                ServiceMonitor.Update_ServiceCalls("Results  " & DateTime.Now.ToLongTimeString, "Retrieve RFGO Results(SurveyID=<" & _SurveyID.ToString & ">)")

                Dim address_String = CreateResultSummaryAddress_From_ListofResultsFilterGroupObject(_listofRFGO)
                Dim q = From rSum In db.ResultsSummaries.AsEnumerable _
                        Where rSum.SurveyID = _SurveyID _
                        AndAlso rSum.ResultsSummaryAddress = address_String _
                        Select New ResultsProviderSummaryObject With {.ResultsDetailsList = ResultsProviderSummaryObject.ToSmallRsltDetails(rSum.ResultsDetails.ToList), _
                                                                      .ResultsSummaryID = rSum.ResultsSummaryID, _
                                                                      .ResultsSummarySurveyID = rSum.SurveyID, _
                                                                      .ResultsSummaryAddressKey = rSum.ResultsSummaryAddress, _
                                                                  .AllSurveyRDENTSCount = RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(_listofRFGO).Count, _
                                                                  .SelectedSurveyRDENTSCount = RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(_listofRFGO).Count, _
                                                                  .QuestionRDENTCountColxn = RetrieveQuestionRDENTCOunts_usingListofRFGO(_SurveyID, _listofRFGO)}
                q.DefaultIfEmpty(Nothing)


                rslt = q.ToList
            End Using

        End If
        If rslt IsNot Nothing Then
            ServiceMonitor.Update_ServiceCalls("Results.RFGO RetrieveResults  " & DateTime.Now.ToLongTimeString, " Count=<" & rslt.Count & ">")
        Else
            ServiceMonitor.Update_ServiceCalls("Results.RFGO RetrieveResults  " & DateTime.Now.ToLongTimeString, " <Empty Query Result>")
        End If
        Return rslt
    End Function

    Private Function PopulateResultsDetails_UsingRDENTSIDList(ByVal _RsltSummary As ResultsSummary, ByVal _RDENTIDsList As List(Of Integer)) As List(Of ResultsDetail)
        Dim rslt As New List(Of ResultsDetail)
        Try
            'Dim _filter As List(Of Integer) = ResultSummary_AddressToList(_RsltSummary.ResultsSummaryAddress)
            Dim _SurveyID = _RsltSummary.SurveyID
            Dim _RsltSummaryID = _RsltSummary.ResultsSummaryID
            Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
            Using db 'As New L2S_SurveyDataStoreDataContext
                For Each sdsRSm In db.SDSResponseModels
                    Dim sdsrmID = sdsRSm.SDSResponseModelID
                    Dim CountofRdentsThatPass = (From rdent In db.SDSRespondentModels _
                                                 Where rdent.SurveyID = _SurveyID _
                                                 AndAlso _RDENTIDsList.Contains(rdent.RespondentModelID) _
                                                 AndAlso (From r In rdent.Responses _
                                                         Where r.ResponseModelID = sdsrmID _
                                                         Select r.ResponseModelID).Count > 0 _
                                                Select rdent.RespondentModelID).Count

                    Dim timestamp = DateTime.Now
                    Dim RsltDetail As ResultsDetail = FindResultsDetail(_RsltSummaryID, sdsrmID)
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
            ServiceMonitor.Update_ServiceCalls(ex.Message, "PopulateResultsDetailsRDENTCount encountered error..." & _RsltSummary.ResultsSummaryAddress & " SurveyID=" & _RsltSummary.SurveyID.ToString)

        End Try
        Return rslt
    End Function

    Private Function CreateRFGO_ResultsDetails(ByVal _SurveyID As Integer, ByVal _listofRFGO As List(Of ResultsFilterGroupObject)) As List(Of ResultsProviderSummaryObject)
        Dim address_String = CreateResultSummaryAddress_From_ListofResultsFilterGroupObject(_listofRFGO)
        Dim rSum = New ResultsSummary
        rSum.SurveyID = _SurveyID
        rSum.ResultsSummaryAddress = address_String
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext

            db.ResultsSummaries.InsertOnSubmit(rSum)
            'db.SubmitChanges()

        End Using
        Dim includedRDENTSList = RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(_listofRFGO)
        Dim rlstdetails = PopulateResultsDetails_UsingRDENTSIDList(rSum, includedRDENTSList)
        Dim RPSO As New ResultsProviderSummaryObject With {.ResultsDetailsList = ResultsProviderSummaryObject.ToSmallRsltDetails(rlstdetails), _
                                                                  .ResultsSummaryID = rSum.ResultsSummaryID, _
                                                                  .ResultsSummarySurveyID = rSum.SurveyID, _
                                                                  .ResultsSummaryAddressKey = rSum.ResultsSummaryAddress, _
                                                                  .AllSurveyRDENTSCount = includedRDENTSList.Count, _
                                                                  .SelectedSurveyRDENTSCount = includedRDENTSList.Count, _
                                                                  .QuestionRDENTCountColxn = RetrieveQuestionRDENTCOunts_usingListofRFGO(_SurveyID, _listofRFGO)}
        Dim rslt As New List(Of ResultsProviderSummaryObject)
        rslt.Add(RPSO)
        Return rslt
    End Function

    Public Function RetrieveRDENTSID_Count_with_ListofResultFilterGroupObject(ByVal _ListofRFG As List(Of ResultsFilterGroupObject)) As Integer _
    Implements IResultsSvc.RetrieveRDENTSID_Count_with_ListofResultFilterGroupObject
        Return RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(_ListofRFG).Count
    End Function

    Private Function RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(ByVal _ListofRFG As List(Of ResultsFilterGroupObject)) As List(Of Integer)
        Dim rslt As List(Of Integer) = Nothing
        Dim lstOfList As New List(Of List(Of Integer))
        For Each rfg In _ListofRFG
            lstOfList.Add(RetrieveRDENTSID_List_QuestionList_OptionList(rfg.SurveyID, rfg.OptionIDList, rfg.QuestionIDList))
        Next
        Dim uniquelist = lstOfList.SelectMany(Function(lst) lst)
        rslt = (uniquelist.Distinct).ToList
        Return rslt
    End Function

    'Private Function Retrieve_UnigueRDENTSID_List_with_ListofRDentList(ByVal _ListofRDENTIDList As List(Of List(Of Integer))) As List(Of Integer)
    '    Dim rslt As List(Of Integer) = Nothing
    '    Dim uniqueRDENTIDList = _ListofRDENTIDList.SelectMany(Function(lst) lst)
    '    rslt = (uniqueRDENTIDList.Distinct).ToList
    '    Return rslt
    'End Function

    Private Function CreateResultSummaryAddress_From_ListofResultsFilterGroupObject(ByVal _listofRFGO As List(Of ResultsFilterGroupObject)) As String
        Dim rslt As String = ""
        Dim strblder As New Text.StringBuilder
        Dim ndx = 0
        Dim avgQ As Double = 0
        Dim avgOpt As Double = 0
        For Each rfgo In _listofRFGO.OrderBy(Function(rf) rf.ToAddress)
            If rfgo.QuestionIDList.Count > 0 Then
                avgQ = Math.Truncate(rfgo.QuestionIDList.Average)
            End If
            If rfgo.OptionIDList.Count > 0 Then
                avgOpt = Math.Truncate(rfgo.OptionIDList.Average)
            End If
            strblder.Append(ndx.ToString & "Qavg" & avgQ.ToString & "Oavg" & avgOpt)
        Next
        rslt = strblder.ToString
        Return rslt
    End Function

    Public Function RetrieveRDENTSCount_QuestionList_OptionList(ByVal _surveyID As Integer, ByVal _Optionfilter As List(Of Integer), ByVal _QuestionFilter As List(Of Integer)) _
                                                                 As Integer Implements IResultsSvc.RetrieveRDENTSCount_QuestionList_OptionList
        Dim rslt As Integer
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            rslt = (From rdent In db.SDSRespondentModels _
                      Where (From rspnse In rdent.Responses _
                             Where _Optionfilter.Contains(rspnse.ResponseModelID) _
                             Select rspnse.ResponseModelID Distinct).Count = _Optionfilter.Count _
                      AndAlso (From rspnse In rdent.Responses _
                             Where _QuestionFilter.Contains(rspnse.SDSResponseModel.QuestID) _
                             Select rspnse.SDSResponseModel.QuestID Distinct).Count = _QuestionFilter.Count _
                      Select rdent).Count
        End Using
        Return rslt
    End Function

    Private Function RetrieveRDENTSID_List_QuestionList_OptionList(ByVal _surveyID As Integer, ByVal _Optionfilter As List(Of Integer), ByVal _QuestionFilter As List(Of Integer)) _
                                                                As List(Of Integer)
        Dim rslt As List(Of Integer)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            rslt = (From rdent In db.SDSRespondentModels _
                      Where (From rspnse In rdent.Responses _
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
        'returns a list of questionID and RDENTCount for the QuestionID
        Dim rdentslist = RetrieveRDENTSID_List_with_ListofResultFilterGroupObject(_listofRFGO)
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            'Dim q = From sdsrmodel In db.SDSResponseModels _
            '        Let rcount = (From rsponse In sdsrmodel.Responses _
            '                      Where rdentslist.Contains(rsponse.RespondentID) _
            '                      Select rsponse.RespondentID Distinct.Count) _
            '        Select New QuestionRDENTCountObject With {.QuestionID = sdsrmodel.QuestID, _
            '                                                  .RDENTCount = rcount, _
            '                                                  .SurveyID = _surveyID} Distinct
            Dim q = From item In (From sdsrspmodel In db.SDSResponseModels _
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
        End Using
        Return rslt
    End Function

#End Region



    Private Function FindResultsDetail(ByVal _rsltSummaryID As Integer, ByVal _sdsResponseModelID As Integer) As ResultsDetail
        Dim rsltDtl As ResultsDetail = Nothing
        'need to try and find a ResultsDetail with the ResponseId and ResultSummaryID = ResultSummary.where ResultsSummarKey="NoFilter/Default"
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            Dim q = From rsltsummary In db.ResultsSummaries _
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
        End Using
        Return rslt
    End Function

    Public Function RetrieveResultsMetaData(ByVal _SurveyID As Integer) As String Implements IResultsSvc.RetrieveResultsMetaData
        Dim rslt As String = "NotImplemented"
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext

        End Using
        Return rslt
    End Function

    Public Function RetrieveResultsMetaData_SDS() As String Implements IResultsSvc.RetrieveResultsMetaData_SDS
        Dim rslt As String = "NotImplemented"
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext

        End Using
        Return rslt
    End Function

    Public Function RetrieveResultsSummaryRowCommaString(ByVal _SurveyID As Integer, ByVal _AddressCommaDelimitedString As String) As ResultsSummary Implements IResultsSvc.RetrieveResultsSummaryRowCommaString
        Dim rslt As ResultsSummary = Nothing
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

        End Using
        Return rslt
    End Function

    Public Function RetrieveResultsSummaryRowWithID(ByVal _ResultSummaryID As Integer) As ResultsSummary Implements IResultsSvc.RetrieveResultsSummaryRowWithID
        Dim rslt As ResultsSummary = Nothing
        Dim db As New L2S_SurveyDataStoreDataContext(MyCnxnString)
        Using db 'As New L2S_SurveyDataStoreDataContext
            'Dim dlo = New DataLoadOptions
            ' dlo.LoadWith(Of ResultsSummary)(Function(rs) rs.ResultsDetails)
            ' db.LoadOptions = dlo

            Dim q = From rSum In db.ResultsSummaries _
                    Where rSum.ResultsSummaryID = _ResultSummaryID _
                    Select rSum
            q.DefaultIfEmpty(rslt)
            rslt = q.FirstOrDefault
        End Using
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


End Class
