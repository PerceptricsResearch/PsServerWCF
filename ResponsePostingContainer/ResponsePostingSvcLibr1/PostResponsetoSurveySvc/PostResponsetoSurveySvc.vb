' NOTE: If you change the class name "PostResponsetoSurveySvc" here, you must also update the reference to "PostResponsetoSurveySvc" in App.config.
Imports System.Transactions
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher
Imports IPostResponsetoSurveySvcNS
Imports CmdSvcClassLibrary
Imports IResultsProviderSvcNS
Imports CmdInfrastructureNS

<PoisonErrorBehavior()> _
Public Class PostResponsetoSurveySvc
    Implements IPostResponsetoSurveySvc

    Private MyCSH As CmdSvcClassLibrary.CustomSvcHost
    Private SvcInitialized As Boolean = False
    Private DB As L2S_SurveyDataStoreDataContext
    Private MyCustomerDBSvc As CustomerDBSvc.CustomerDBSvc = Nothing
    Private InstanceTracker As CmdSvcClassLibrary.InstanceTracker
    Dim cnxnstring As String = "NOT SET"

    Private Function SetUpCustomerDBSvc() As Boolean
        Dim dcnxnstring = MyCSH.DC_Pkg.MyCustomerDBCnxnString
        If dcnxnstring IsNot Nothing Then
            MyCustomerDBSvc = New CustomerDBSvc.CustomerDBSvc
            MyCustomerDBSvc.DC_ConnectionString = dcnxnstring
            MyCustomerDBSvc.MyCSH = MyCSH
            Return True
        Else
            MyCustomerDBSvc = Nothing
            Return False
        End If

    End Function

    <OperationBehavior(TransactionScopeRequired:=True, TransactionAutoComplete:=True)> _
    Public Sub EstablishSvc(ByVal _EstablishCommandString As String, ByVal _TargetSurveyName As String) Implements IPostResponsetoSurveySvc.EstablishSvc

    End Sub

    Private Sub RaiseOperationFailed(ByVal _MethodName As String, _Message As String)
        Using EvLog As New EventLog()
            EvLog.Source = "PostResponsetoSurveySvc"
            EvLog.Log = "Application"
            EvLog.WriteEntry("Method= " & _MethodName & "  " & _Message, EventLogEntryType.Warning)
        End Using
        'RaiseEvent OperationFailed(sender, _MethodName, New EventArgs)
    End Sub


    <OperationBehavior(TransactionScopeRequired:=True, TransactionAutoComplete:=True)> _
    Public Sub SubmitRespondentModel(ByVal _RDentModel As ResponDENTModel) Implements IPostResponsetoSurveySvc.SubmitRespondentModel


        InitializeSvc(_RDentModel.SurveyID, _RDentModel.ID)

        Using scope As New TransactionScope(TransactionScopeOption.Required)
            If SvcInitialized Then
                Dim ermsg As String = "RSLTSUM is nothing"
                'MyCSH.SvcMonitor.Update_ServiceCalls("Posting", "SurveyID=<" & _RDentModel.SurveyID & "> RdentID=<" & _RDentModel.ID & "> " & "RSPONSECount=<" & _RDentModel.ResponseColxn.Count & ">")

                'MyPostingSvcMonitor.Update_PostingSvcCalls("Posting", "SurveyID=<" & _RDentModel.SurveyID & "> RdentID=<" & _RDentModel.ID & "> " & "RSPONSECount=<" & _RDentModel.ResponseColxn.Count & ">")
                Try
                    Dim rsltsum = FindResultsSummary("NoFilter", _RDentModel.SurveyID)
                    Try
                        If IsNothing(rsltsum) Then
                            If Not IsNothing(_RDentModel) Then
                                If Not IsNothing(_RDentModel.SurveyID) Then
                                    ermsg += ", SurveyID= " & _RDentModel.SurveyID.ToString
                                Else
                                    ermsg += ", SurveyID IsNothing"
                                End If
                            Else
                                ermsg += ", _RDentModel IsNothing"
                            End If
                            Me.RaiseOperationFailed("Main", ermsg)
                        Else
                            ermsg = "SurveyID = " & _RDentModel.SurveyID.ToString
                            ermsg += ", RDentModel.ID = " & _RDentModel.ID
                        End If
                    Catch ex As Exception
                        Me.RaiseOperationFailed("Main 1", ermsg)
                    End Try

                    'this build SDSResponseModels...simulates what happens when Survey is Published...
                    'For Each rSm In _RDentModel.ResponseColxn
                    '    CreateSDSResponseModel(rSm, _RDentModel.SurveyID)
                    'Next
                    Dim SDSRDentModel = Build_SDSRespondentModel(_RDentModel)
                    Try
                        If IsNothing(SDSRDentModel) Then
                            ermsg += "SDSRDentModel is nothing"
                            If Not IsNothing(_RDentModel) Then
                                If Not IsNothing(_RDentModel.SurveyID) Then
                                    ermsg += ", SurveyID= " & _RDentModel.SurveyID.ToString
                                Else
                                    ermsg += ", SurveyID IsNothing"
                                End If
                            Else
                                ermsg += ", _RDentModel IsNothing"
                            End If
                        Else
                            ermsg += ", SDSRDentID= " & SDSRDentModel.RespondentModelID
                        End If
                    Catch ex As Exception
                        Me.RaiseOperationFailed("Main 2", ermsg)
                    End Try

                    ermsg += ", DB.SDSRDentModels.InsertOnSubmit"
                    DB.SDSRespondentModels.InsertOnSubmit(SDSRDentModel)
                    DB.SubmitChanges()

                    ermsg += ", Starting PopulateResults"
                    PopulateResults(SDSRDentModel)

                    ermsg += ", Starting DB.SubmitChanges"
                    DB.SubmitChanges()


                    Try
                        ermsg += ", Starting SetUpCustomerDBSvc"
                        If SetUpCustomerDBSvc() Then
                            Me.MyCustomerDBSvc.UpdatePostingMetaData(_RDentModel.SurveyID)
                        End If
                    Catch ex As Exception
                        Me.RaiseOperationFailed("Main 3", ermsg & ex.Message)
                    End Try
                   
                    'need a Transactionscope.Complete in here...
                    scope.Complete()
                    ermsg = ""
                    'MyCSH.SvcMonitor.Update_ServiceCalls("  Complete " & DateTime.Now.ToLongTimeString, "SurveyID=<" & _RDentModel.SurveyID & "> RdentID=<" & _RDentModel.ID & "> " & "RSPONSECount=<" & _RDentModel.ResponseColxn.Count & ">")

                Catch ex As Exception
                    Me.RaiseOperationFailed("Main 0", DateTime.Now.ToString & ermsg & ex.Message)
                    'MyCSH.SvcMonitor.Update_ServiceCalls(ex.Message, "SurveyID=<" & _RDentModel.SurveyID & "> RdentID=<" & _RDentModel.ID & "> " & "RSPONSECount=<" & _RDentModel.ResponseColxn.Count & ">")

                End Try
            Else
                Me.RaiseOperationFailed("Main Minus 1", DateTime.Now.ToString & " Not ServiceInitialized...")
            End If
        End Using
    End Sub

    Private MySurveyID As Integer

    Private Function Build_SDSRespondentModel(ByVal rm As ResponDENTModel) As SDSRespondentModel
        Dim SDSrm = New SDSRespondentModel
        MySurveyID = rm.SurveyID
        With SDSrm
            .SurveyID = rm.SurveyID
            If rm.CustomField Is Nothing Then
                rm.CustomField = ""
            End If
            .CustomField = rm.CustomField
            If rm.FirstName Is Nothing Then
                rm.FirstName = ""
            End If
            .FirstName = rm.FirstName
            If rm.LastName Is Nothing Then
                rm.LastName = ""
            End If
            .LastName = rm.LastName
            If rm.IPAddress Is Nothing Then
                rm.IPAddress = ""
            End If
            .IPAddress = rm.IPAddress
            For Each rs In rm.ResponseColxn
                .Responses.Add(Build_SDSResponse(rs, SDSrm))
            Next
        End With

        Return SDSrm
    End Function
    Private Function Build_SDSResponse(ByVal rSm As ResponseModel, ByVal _SDSrDm As SDSRespondentModel) As Response
        Dim rs = New Response
        With rs
            .SDSResponseModel = FindSDSResponseModel(rSm)
            .ResponseModelID = .SDSResponseModel.SDSResponseModelID
            .PostedTimeStamp = DateTime.Now
            .SDSRespondentModel = _SDSrDm
        End With
        DB.Responses.InsertOnSubmit(rs)
        Return rs
    End Function

    Private Function FindSDSResponseModel(ByVal rSm As ResponseModel) As SDSResponseModel
        Dim rslt As SDSResponseModel = Nothing
        Dim q = From sdsrSm In DB.SDSResponseModels _
                Where sdsrSm.QuestID = rSm.QuestID AndAlso _
                      sdsrSm.Key1 = rSm.Key1 AndAlso _
                      sdsrSm.Key2 = rSm.Key2 AndAlso _
                      sdsrSm.Key3 = rSm.Key3 _
                Select sdsrSm
        q.DefaultIfEmpty(rslt)
        If q.FirstOrDefault Is Nothing Then
            rslt = CreateSDSResponseModel(rSm, MySurveyID)
        Else
            rslt = q.FirstOrDefault
        End If

        Return rslt
    End Function
    Private Function CreateSDSResponseModel(ByVal _rSm As ResponseModel, ByVal _surveyID As Integer) As SDSResponseModel
        Dim rslt_rSm As SDSResponseModel = New SDSResponseModel
        With rslt_rSm
            .QuestID = _rSm.QuestID
            .Key1 = _rSm.Key1
            .Key2 = _rSm.Key2
            .Key3 = _rSm.Key3
            .SurveyID = _surveyID
        End With
        DB.SDSResponseModels.InsertOnSubmit(rslt_rSm)
        Return rslt_rSm
    End Function

    Private Function FindResultsDetail(ByVal _rsltSummaryID As Integer, ByVal _sdsResponseModelID As Integer) As ResultsDetail
        Dim rsltDtl As ResultsDetail = Nothing
        'need to try and find a ResultsDetail with the ResponseId and ResultSummaryID = ResultSummary.where ResultsSummarKey="NoFilter/Default"
        Dim q = From rsltsummary In DB.ResultsSummaries _
                From rd In DB.ResultsDetails _
                Where rd.SDSResponseModelID = _sdsResponseModelID AndAlso rd.ResultsSummaryID = _rsltSummaryID _
                AndAlso rsltsummary.ResultsSummaryID = _rsltSummaryID _
                Select rd
        q.DefaultIfEmpty(Nothing)
        rsltDtl = q.FirstOrDefault
        Return rsltDtl
    End Function
    Private Function FindResultsSummary(ByVal _SummaryAddress As String, ByVal _SurveyID As Integer) As ResultsSummary
        Dim rsltSumm As ResultsSummary = Nothing
        'Try
        '    Dim x = DB.ResultsSummaries
        '    Dim z = x.Count
        'Catch ex As Exception
        '    Dim y = 2
        'End Try

        Dim q = From rSumm In DB.ResultsSummaries _
                Where rSumm.ResultsSummaryAddress = _SummaryAddress _
                AndAlso rSumm.SurveyID = _SurveyID _
                Select rSumm
        q.DefaultIfEmpty(Nothing)
        rsltSumm = q.FirstOrDefault
        If rsltSumm Is Nothing Then
            rsltSumm = New ResultsSummary With {.ResultsSummaryAddress = NoFilter, _
                                                .SurveyID = _SurveyID}

            DB.ResultsSummaries.InsertOnSubmit(rsltSumm)
            DB.SubmitChanges()
        End If
        Return rsltSumm
    End Function

#Region "Preliminary Query Work - replaced with inline query"
    Private Function ResponseModelColxnPassesResultsSummaryFilter(ByVal _responseColxn As List(Of ResponseModel), ByVal _filter As List(Of Integer)) As Boolean
        Dim rslt As Boolean = False
        Dim q = (From resp In _responseColxn _
                Where _filter.Contains(FindSDSResponseModel(resp).SDSResponseModelID) _
                Select resp.ID Distinct).Count
        If q = _filter.Count Then
            rslt = True
        End If

        Return rslt
    End Function
    Private Function ResponsesListPassesResultsSummaryFilter(ByVal _responseColxn As List(Of Response), ByVal _filter As List(Of Integer)) As Boolean
        Dim rslt As Boolean = False
        Dim q = (From resp In _responseColxn _
                Where _filter.Contains(resp.SDSResponseModel.SDSResponseModelID) _
                Select resp.SDSResponseModel.SDSResponseModelID Distinct).Count
        If q = _filter.Count Then
            rslt = True
        End If

        Return rslt
    End Function
    Private Function SDSRdentPassesResultsSummaryFilter(ByVal _SDSRdent As SDSRespondentModel, ByVal _filter As List(Of Integer)) As Boolean
        Dim rslt As Boolean = False
        Dim q = (From resp In _SDSRdent.Responses _
                Where _filter.Contains(resp.SDSResponseModel.SDSResponseModelID) _
                Select resp.SDSResponseModel.SDSResponseModelID Distinct).Count
        If q = _filter.Count Then
            rslt = True
        End If

        Return rslt
    End Function
    Private Function ResponsesContainsSDSResponseModelID(ByVal _responses As System.Data.Linq.EntitySet(Of Response), ByVal _sdsRsModelId As Integer) As Boolean
        Dim rslt As Boolean = False
        Dim q = (From resp In _responses _
                 Where resp.ResponseModelID = _sdsRsModelId _
                 Select resp.SDSResponseModel.SDSResponseModelID Distinct).Count
        If q > 0 Then
            rslt = True
        End If
        Return rslt
    End Function
#End Region

#Region "ResultsSummary Methods"
    ''' <summary>
    ''' Compute Counts for all Respondents and all ResultsSummaries in a Survey...has an overload for a Singe SDSRespondentModel
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateResults()
        For Each rsltSumm In DB.ResultsSummaries
            PopulateResultsDetailsRDENTCount(rsltSumm)
        Next
    End Sub
    ''' <summary>
    ''' Update Counts for the Respondent provided...for all ResultsSummaries...where ResultsSummary.SurveyID = _Rdent.SurveyID
    ''' </summary>
    ''' <param name="_RDent"></param>
    ''' <remarks></remarks>
    Private Sub PopulateResults(ByVal _RDent As SDSRespondentModel)
        For Each rsltSumm In DB.ResultsSummaries.Where(Function(r) (r.SurveyID = _RDent.SurveyID))
            Try
                If rsltSumm.ResultsSummaryAddress = NoFilter Then
                    UpdateRsltsDetails_ForSingleRDent(_RDent, rsltSumm)
                Else
                    If Not IsDBNull(rsltSumm.RFGOList) Then
                        Dim lstofRFGO As List(Of ResultsFilterGroupObject) = DeSerializeToListOfRFGO(rsltSumm.RFGOList.ToArray) 'rsltSumm.rfgolist)
                        If RDENTPasses_ListofResultFilterGroupObject(lstofRFGO, _RDent) Then
                            UpdateRsltsDetails_ForSingleRDent(_RDent, rsltSumm)
                        End If
                    End If

                End If

            Catch ex As Exception
                CmdInfrastructureNS.SharedEvents.RaiseOperationFailed(New Object, "PostResponseToSurvey.PopulateResults(ByRef _RDent)...RDentID= " & _RDent.RespondentModelID.ToString)
            End Try
        Next
    End Sub

    Private Function PopulateResultsDetailsRDENTCount(ByVal _RsltSummary As ResultsSummary) As List(Of ResultsDetail)
        Dim rslt As New List(Of ResultsDetail)
        Try
            Dim _filter As List(Of Integer) = ResultSummary_AddressToList(_RsltSummary.ResultsSummaryAddress)
            Dim _SurveyID = _RsltSummary.SurveyID
            Dim _RsltSummaryID = _RsltSummary.ResultsSummaryID

            For Each sdsRSm In DB.SDSResponseModels
                Dim sdsrmID = sdsRSm.SDSResponseModelID
                Dim CountofRdentsThatPass = (From rdent In DB.SDSRespondentModels _
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
                        .ResultsSummaryID = _RsltSummaryID
                        '.ResultsSummary = _RsltSummary
                        .SDSResponseModel = sdsRSm
                        .LastCountTimestamp = timestamp 'this will need the ResultSummaryID...
                    End With
                    rslt.Add(RsltDetail)
                    DB.ResultsDetails.InsertOnSubmit(RsltDetail)
                Else
                    With RsltDetail
                        .RespDentCount = CountofRdentsThatPass
                        .LastCountTimestamp = timestamp
                    End With
                End If


            Next
        Catch ex As Exception
            MyCSH.SvcMonitor.Update_ServiceCalls(ex.Message, "PopulateResultsDetailsRDENTCount encountered error..." & _RsltSummary.ResultsSummaryAddress & " SurveyID=" & _RsltSummary.SurveyID.ToString)

        End Try
        Return rslt
    End Function

    'this RDentList will have one member.... 
    Private Function UpdateRsltsDetails_ForSingleRDent(ByVal _RDent As SDSRespondentModel, ByVal _RsltSummary As ResultsSummary) As List(Of ResultsDetail)
        Dim rslt As New List(Of ResultsDetail)
        Try
            'Dim _filter As List(Of Integer) = ResultSummary_AddressToList(_RsltSummary.ResultsSummaryAddress)
            'Dim _SurveyID = _RsltSummary.SurveyID
            Dim _RsltSummaryID = _RsltSummary.ResultsSummaryID

            For Each sdsRSm In DB.SDSResponseModels
                Dim sdsrmID = sdsRSm.SDSResponseModelID
                'does this rdent have a response for the SDSResponseModel with ID = sdsrmID
                Dim CountofRdentsThatPass = (From r In _RDent.Responses _
                                            Where r.ResponseModelID = sdsrmID _
                                            Select r.ResponseModelID).Count
                Dim cnt = CountofRdentsThatPass
                If cnt > 1 Then
                    CmdInfrastructureNS.SharedEvents.RaiseOperationFailed(New Object, "PostResponse cnt > 1 in UpdateRsltsDetails_forSingleRedent. " & sdsrmID.ToString)
                    cnt = 1
                End If
                Dim timestamp = DateTime.Now
                Dim RsltDetail As ResultsDetail = FindResultsDetail(_RsltSummaryID, sdsrmID)
                If RsltDetail Is Nothing Then
                    RsltDetail = New ResultsDetail
                    With RsltDetail
                        .RespDentCount = cnt 'CountofRdentsThatPass
                        .ResultsSummaryID = _RsltSummaryID
                        '.ResultsSummary = _RsltSummary
                        .SDSResponseModel = sdsRSm
                        .LastCountTimestamp = timestamp 'this will need the ResultSummaryID...
                    End With
                    rslt.Add(RsltDetail)
                    DB.ResultsDetails.InsertOnSubmit(RsltDetail)
                Else
                    Dim newcnt = RsltDetail.RespDentCount + cnt
                    With RsltDetail
                        .RespDentCount = newcnt '+= cnt 'CountofRdentsThatPass
                        .LastCountTimestamp = timestamp
                    End With
                End If
            Next
        Catch ex As Exception
            MyCSH.SvcMonitor.Update_ServiceCalls(ex.Message, "UpdateRsltsDetails_RDentList encountered error..." & _RsltSummary.ResultsSummaryAddress & " SurveyID=" & _RsltSummary.SurveyID.ToString)
        End Try

        Return rslt

    End Function

    Const NoFilter As String = "NoFilter"
    Private Function ResultSummary_AddressToList(ByVal _ResultsSummaryAddress As String) As List(Of Integer)

        Dim rslt As New List(Of Integer)
        If _ResultsSummaryAddress <> NoFilter Then
            Try
                For Each sdsRSmodelID In Strings.Split(_ResultsSummaryAddress, ",")
                    rslt.Add(CType(sdsRSmodelID, Integer))
                Next
            Catch ex As Exception
                rslt.Clear()
                MyCSH.SvcMonitor.Update_ServiceCalls(ex.Message, "ResultSummaryAddress not convertible to List(of Integer).." & _ResultsSummaryAddress)

                Throw ex
            End Try
        End If
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

#Region "RFGO Individual Rdent Methods"

    Private Function RDENTPasses_ListofResultFilterGroupObject(ByVal _ListofRFG As List(Of ResultsFilterGroupObject), _
                                                               ByRef _Rdent As SDSRespondentModel) As Boolean
        Dim rslt As Boolean = False
        If _ListofRFG IsNot Nothing AndAlso _Rdent IsNot Nothing Then
            Dim listofrslts As New List(Of Boolean)
            For Each rfg In _ListofRFG
                listofrslts.Add(RDentIDPassesThisFilter_QuestionList_OptionList(rfg.SurveyID, rfg.OptionIDList, rfg.QuestionIDList, _Rdent))
            Next
            rslt = Not listofrslts.Contains(False)
        End If

        Return rslt
    End Function
    'this evaluates  RDent.Responsemodels against a single RFGO...returns boolean
    Private Function RDentIDPassesThisFilter_QuestionList_OptionList(ByVal _surveyID As Integer, _
                                                                          ByVal _Optionfilter As List(Of Integer), _
                                                                          ByVal _QuestionFilter As List(Of Integer), _
                                                                          ByRef rdent As SDSRespondentModel) _
                                                            As Boolean
        Dim rslt As Boolean = False

        rslt = (From rspnse In rdent.Responses _
                         Where _Optionfilter.Contains(rspnse.ResponseModelID) _
                         Select rspnse.ResponseModelID Distinct).Count = _Optionfilter.Count _
                  AndAlso (From rspnse In rdent.Responses _
                         Where _QuestionFilter.Contains(rspnse.SDSResponseModel.QuestID) _
                         Select rspnse.SDSResponseModel.QuestID Distinct).Count = _QuestionFilter.Count

        Return rslt
    End Function
#End Region


#Region "ResultSummaryListOfRFGO Serialize/De methods"
    'Private Function Serialize_ListOfRFGO(ByVal _listOfRFGO As List(Of ResultsFilterGroupObject)) As Byte()
    '    Dim rslt As Byte()
    '    Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
    '    Using ms As New IO.MemoryStream
    '        bf.Serialize(ms, _listOfRFGO)
    '        ms.Seek(0, IO.SeekOrigin.Begin)
    '        rslt = ms.ToArray
    '    End Using
    '    bf = Nothing
    '    Return rslt
    'End Function

    Private Function DeSerializeToListOfRFGO(ByVal _rfgolistBarray As Byte()) As List(Of ResultsFilterGroupObject)
        Dim rslt As List(Of ResultsFilterGroupObject) = Nothing
        Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter

        Using ms As New IO.MemoryStream(_rfgolistBarray)
            rslt = bf.Deserialize(ms)
        End Using
        bf = Nothing
        Return rslt
    End Function
#End Region

#Region "Initialize Svc and FindSurveyDataStore Methods"
    Private Sub InitializeSvc(ByVal _surveyID As Integer, ByVal _RDentID As Integer)
        Dim IsCnxnValid As Boolean = False
        Dim ermsg As String = ""
        Try
            SvcInitialized = False
            If MyCSH Is Nothing Then
                If Not IsNothing(OperationContext.Current.Host) Then
                    MyCSH = CType(OperationContext.Current.Host, CustomSvcHost)
                Else
                    ermsg = "OperationContext.Current.Host is nothing."
                    Using EvLog As New EventLog()
                        EvLog.Source = "PostResponsetoSurveySvc"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry(ermsg & ", RDentID= " & _RDentID.ToString, EventLogEntryType.Error)
                    End Using
                End If

                'InstanceTracker = New InstanceTracker(MyCSH)
            End If
            If Not IsNothing(Me.MyCSH) Then
                With Me.MyCSH
                    If Not IsNothing(.DC_Pkg) Then
                        With .DC_Pkg
                            Dim prelimcnxnstring = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID)
                            If Not IsNothing(prelimcnxnstring) AndAlso prelimcnxnstring <> "IsNotSet" Then
                                cnxnstring = prelimcnxnstring
                                IsCnxnValid = True
                            Else
                                If .Refresh Then
                                    prelimcnxnstring = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID)
                                    If Not IsNothing(prelimcnxnstring) AndAlso prelimcnxnstring <> "IsNotSet" Then
                                        cnxnstring = prelimcnxnstring
                                        IsCnxnValid = True
                                    End If
                                End If
                            End If
                        End With
                    Else
                        ermsg = "DC_Pkg is nothing."
                        Using EvLog As New EventLog()
                            EvLog.Source = "PostResponsetoSurveySvc"
                            EvLog.Log = "Application"
                            EvLog.WriteEntry(ermsg & ", RDentID= " & _RDentID.ToString, EventLogEntryType.Error)
                        End Using
                    End If
                End With
            End If
            'cnxnstring = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID) 'is a function that looks at DC_Pkg in CustomServiceHost with SurveyID..sets dc_cnxn...
            'If cnxnstring.Length < 11 Then
            '    If MyCSH.DC_Pkg.Refresh Then
            '        cnxnstring = MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_surveyID)
            '    End If
            'End If
            If IsCnxnValid Then
                DB = New L2S_SurveyDataStoreDataContext(cnxnstring) 'MyCSH.DataContextConnectionString)
                'Dim x = DB.SurveyMasters
                SvcInitialized = True
            Else
                DB = Nothing
                SvcInitialized = False
            End If
            'If MyCSH.DC_Cnxn_IsValid Then
            '    DB = New L2S_SurveyDataStoreDataContext(MyCSH.DataContextConnectionString)
            'Else
            '    DB = Nothing
            'End If

            'SvcInitialized = MyCSH.DC_Cnxn_IsValid
            ' MyCSH.SvcMonitor.Update_ServiceCalls("SvcInitialized= " & SvcInitialized.ToString & "SurveyID=<" & _surveyID & "> RdentID=<" & _RDentID & ">", _
            'DateTime.Now.ToLongTimeString)
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "PostResponsetoSurveySvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry(cnxnstring & " " & ermsg & ex.Message, EventLogEntryType.Error)
            End Using

        End Try
    End Sub


#End Region

End Class

Public NotInheritable Class PoisonErrorBehaviorAttribute
    Inherits Attribute
    Implements IServiceBehavior

    Private poisonerrorHandler = New PoisonMsgErrorHandler

    Public Sub AddBindingParameters(ByVal serviceDescription As System.ServiceModel.Description.ServiceDescription, ByVal serviceHostBase As System.ServiceModel.ServiceHostBase, ByVal endpoints As System.Collections.ObjectModel.Collection(Of System.ServiceModel.Description.ServiceEndpoint), ByVal bindingParameters As System.ServiceModel.Channels.BindingParameterCollection) Implements System.ServiceModel.Description.IServiceBehavior.AddBindingParameters
        Dim q = From ep In endpoints _
                    Where ep.Binding.GetType.Equals(GetType(NetMsmqBinding)) _
                    Select ep
        q.DefaultIfEmpty(Nothing)
        Dim endpt = q.FirstOrDefault
        If endpt IsNot Nothing Then
            Dim _queueURI As String = endpt.ListenUri.OriginalString

            poisonerrorHandler.SvcPoisonQueueName = ".\private$\poisonmsgq" & endpt.Name
            poisonerrorHandler.SvcQueueName = ".\private$\" & endpt.Name
            If endpt.Binding.GetType.Equals(GetType(NetMsmqBinding)) Then
                'CType(endpt.Binding, NetMsmqBinding).RetryCycleDelay = New TimeSpan(0, 0, 5)
                'CType(endpt.Binding, NetMsmqBinding).MaxRetryCycles = 1
                Dim x = CType(endpt.Binding, NetMsmqBinding).ReceiveRetryCount
            End If

        End If
    End Sub

    Public Sub ApplyDispatchBehavior(ByVal serviceDescription As System.ServiceModel.Description.ServiceDescription, ByVal serviceHostBase As System.ServiceModel.ServiceHostBase) Implements System.ServiceModel.Description.IServiceBehavior.ApplyDispatchBehavior

        For Each cdbase In serviceHostBase.ChannelDispatchers
            Dim cd = TryCast(cdbase, ChannelDispatcher)
            cd.ErrorHandlers.Add(poisonerrorHandler)
        Next
    End Sub

    Public Sub Validate(ByVal serviceDescription As System.ServiceModel.Description.ServiceDescription, ByVal serviceHostBase As System.ServiceModel.ServiceHostBase) Implements System.ServiceModel.Description.IServiceBehavior.Validate

    End Sub
End Class