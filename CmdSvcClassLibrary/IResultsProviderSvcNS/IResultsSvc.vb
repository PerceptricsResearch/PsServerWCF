Imports System.ServiceModel
Imports System.Runtime.Serialization

<ServiceContract()> _
Public Interface IResultsSvc

    <OperationContract()> _
   Function TellMeYouWork() As String

    <OperationContract()> _
    Sub PruneResultsSummaries(ByVal _SurveyIDList As List(Of Integer))

    <OperationContract()> _
    Function Retrieve_SDSResponseModels(ByVal _SurveyID As Integer) As List(Of SDSResponseModelObject)

    <OperationContract()> _
    Function Save_ResultsFilters(ByVal _SurveyID As Integer, ByVal _ListofResultsFilter As List(Of ResultsFilterModelObject)) As List(Of CmdInfrastructureNS.POCO_ID_Pkg)

    <OperationContract()> _
    Function Delete_ResultsFilters(ByVal _SurveyID As Integer, ByVal _ListofResultsFilter As LinkedList(Of ResultsFilterModelObject)) As Boolean

    <OperationContract()> _
    Function Retrieve_ResultsFilters(ByVal _SurveyID As Integer) As List(Of ResultsFilterModelObject)



    ''' <summary>
    ''' ResultsSummary with ResultsDetails property populated...implied is the "NoFilter" default value for ResultsSummaryAddress...
    ''' </summary>
    ''' <param name="_SurveyID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <OperationContract()> _
    Function RetrieveResults(ByVal _SurveyID As Integer) As List(Of ResultsProviderSummaryObject)

    <OperationContract()> _
    Function RetrieveFilteredResultsWithIntegerList(ByVal _SurveyID As Integer, ByVal _AddressList As System.Collections.Generic.List(Of Integer)) As List(Of ResultsProviderSummaryObject)

    ''' <summary>
    ''' ResultsSummary with ResultsDetails property populated...provide a List(of Integer) for ResultsSummaryAddress...
    ''' </summary>
    ''' <param name="_SurveyID"></param>
    ''' <param name="_AddressList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <OperationContract()> _
    Function RetrieveResultsItegerList(ByVal _SurveyID As Integer, ByVal _AddressList As List(Of Integer)) As ResultsSummary

    ''' <summary>
    ''' ResultsSummary with ResultsDetails property populated...provide a CommaDelimitedString for ResultsSummaryAddress...
    ''' </summary>
    ''' <param name="_SurveyID"></param>
    ''' <param name="_AddressCommaDelimitedString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <OperationContract()> _
    Function RetrieveResultsCommaString(ByVal _SurveyID As Integer, ByVal _AddressCommaDelimitedString As String) As ResultsSummary

    ''' <summary>
    ''' Is the ResultsSummaryRow only...the ResultsDetails Property is not populated...
    ''' </summary>
    ''' <param name="_SurveyID"></param>
    ''' <param name="_AddressList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <OperationContract()> _
  Function RetrieveResultsSummaryRowIntegerList(ByVal _SurveyID As Integer, ByVal _AddressList As List(Of Integer)) As ResultsSummary

    <OperationContract()> _
Function RetrieveResultsSummaryRowCommaString(ByVal _SurveyID As Integer, ByVal _AddressCommaDelimitedString As String) As ResultsSummary

    <OperationContract()> _
 Function RetrieveResultsSummaryRowWithID(ByVal _ResultSummaryID As Integer) As ResultsSummary

    <OperationContract()> _
   Function RetrieveResultsMetaData(ByVal _SurveyID As Integer) As String

    <OperationContract()> _
   Function RetrieveResultsMetaData_SDS() As String

    <OperationContract()> _
    Function RetrieveFilteredResults_with_ListofResultFilterGroupObject(ByVal _SurveyID As Integer, ByVal _listofRFGO As List(Of ResultsFilterGroupObject)) _
    As List(Of ResultsProviderSummaryObject)

    <OperationContract()> _
     Function RetrieveRDENTSCount_QuestionList_OptionList(ByVal _surveyID As Integer, ByVal _Optionfilter As List(Of Integer), ByVal _QuestionFilter As List(Of Integer)) _
                                                                 As Integer
    <OperationContract()> _
    Function RetrieveRDENTSID_Count_with_ListofResultFilterGroupObject(ByVal _ListofRFG As List(Of ResultsFilterGroupObject)) As Integer

    <OperationContract()> _
    Function RetrieveQuestionRDENTCOunts_usingListofRFGO(ByVal _surveyID As Integer, ByVal _listofRFGO As List(Of ResultsFilterGroupObject)) As List(Of QuestionRDENTCountObject)

End Interface
<DataContract()> _
Public Class ResultsProviderSummaryObject

    Public Shared Function ToSmallRsltDetails(ByVal _listofResultsDetail As List(Of ResultsDetail), ByVal _sdsRms As IEnumerable(Of SDSResponseModel)) As List(Of SmallRsltDetail)
        Dim rslt = New List(Of SmallRsltDetail)
        Dim q = From rd In _listofResultsDetail, sdsrm In _sdsRms.Where(Function(s) s.SDSResponseModelID = rd.SDSResponseModelID) _
                Select New SmallRsltDetail With {.RespDentCount = rd.RespDentCount, _
                                                 .LastCountTimeStamp = rd.LastCountTimestamp, _
                                                 .RsltSummID = rd.ResultsSummaryID, _
                                                 .QuestID = sdsrm.QuestID, _
                                                 .RMKey1 = sdsrm.Key1, _
                                                 .RMKey2 = sdsrm.Key2, _
                                                 .RMKey3 = sdsrm.Key3, _
                                                 .SDSRespModelID = rd.SDSResponseModelID}
        rslt = q.ToList
        Return rslt
    End Function


    Private _ResultsSummaryID As Integer
    <DataMember()> _
    Public Property ResultsSummaryID() As Integer
        Get
            Return _ResultsSummaryID
        End Get
        Set(ByVal value As Integer)
            _ResultsSummaryID = value
        End Set
    End Property
    Private _ResultsSummarySurveyID As Integer
    <DataMember()> _
    Public Property ResultsSummarySurveyID() As Integer
        Get
            Return _ResultsSummarySurveyID
        End Get
        Set(ByVal value As Integer)
            _ResultsSummarySurveyID = value
        End Set
    End Property
    Private _ResultsSummaryAddressKey As String
    <DataMember()> _
    Public Property ResultsSummaryAddressKey() As String
        Get
            Return _ResultsSummaryAddressKey
        End Get
        Set(ByVal value As String)
            _ResultsSummaryAddressKey = value
        End Set
    End Property

    Private _ResultsDetailsList As List(Of SmallRsltDetail)
    <DataMember()> _
    Public Property ResultsDetailsList() As List(Of SmallRsltDetail)
        Get
            Return _ResultsDetailsList
        End Get
        Set(ByVal value As List(Of SmallRsltDetail))
            _ResultsDetailsList = value
        End Set
    End Property

    Private _AllSurveyRDENTSCount As Integer
    <DataMember()> _
    Public Property AllSurveyRDENTSCount() As Integer
        Get
            Return _AllSurveyRDENTSCount
        End Get
        Set(ByVal value As Integer)
            _AllSurveyRDENTSCount = value
        End Set
    End Property
    Private _SelectedSurveyRDENTSCount As Integer
    <DataMember()> _
    Public Property SelectedSurveyRDENTSCount() As Integer
        Get
            Return _SelectedSurveyRDENTSCount
        End Get
        Set(ByVal value As Integer)
            _SelectedSurveyRDENTSCount = value
        End Set
    End Property

    Private _QuestionRDENTCountColxn As List(Of QuestionRDENTCountObject)
    <DataMember()> _
    Public Property QuestionRDENTCountColxn() As List(Of QuestionRDENTCountObject)
        Get
            Return _QuestionRDENTCountColxn
        End Get
        Set(ByVal value As List(Of QuestionRDENTCountObject))
            _QuestionRDENTCountColxn = value
        End Set
    End Property

End Class
<DataContract()> _
Public Class SmallRsltDetail
    Private _RespDentCount As Integer
    <DataMember()> _
    Public Property RespDentCount() As Integer
        Get
            Return _RespDentCount
        End Get
        Set(ByVal value As Integer)
            _RespDentCount = value
        End Set
    End Property

    Private _SDSRespModelID As Integer
    <DataMember()> _
    Public Property SDSRespModelID() As Integer
        Get
            Return _SDSRespModelID
        End Get
        Set(ByVal value As Integer)
            _SDSRespModelID = value
        End Set
    End Property

    Private _RsltSummID As Integer
    <DataMember()> _
    Public Property RsltSummID() As Integer
        Get
            Return _RsltSummID
        End Get
        Set(ByVal value As Integer)
            _RsltSummID = value
        End Set
    End Property

    Private _LastCountTimeStamp As DateTime
    <DataMember()> _
    Public Property LastCountTimeStamp() As DateTime
        Get
            Return _LastCountTimeStamp
        End Get
        Set(ByVal value As DateTime)
            _LastCountTimeStamp = value
        End Set
    End Property

    <DataMember()> _
    Public Property QuestID As Integer
    <DataMember()> _
    Public Property RMKey1 As Integer
    <DataMember()> _
    Public Property RMKey2 As Integer
    <DataMember()> _
    Public Property RMKey3 As Integer
End Class

<DataContract()> _
Public Class SDSResponseModelObject
    Private _SDSRespModelID As Integer
    <DataMember()> _
    Public Property SDSRespModelID() As Integer
        Get
            Return _SDSRespModelID
        End Get
        Set(ByVal value As Integer)
            _SDSRespModelID = value
        End Set
    End Property
    Private _QuestID As Integer
    <DataMember()> _
    Public Property QuestID() As Integer
        Get
            Return _QuestID
        End Get
        Set(ByVal value As Integer)
            _QuestID = value
        End Set
    End Property
    Private _Key1 As Integer
    <DataMember()> _
    Public Property Key1() As Integer
        Get
            Return _Key1
        End Get
        Set(ByVal value As Integer)
            _Key1 = value
        End Set
    End Property
    Private _Key2 As Integer
    <DataMember()> _
    Public Property Key2() As Integer
        Get
            Return _Key2
        End Get
        Set(ByVal value As Integer)
            _Key2 = value
        End Set
    End Property
    Private _Key3 As Integer
    <DataMember()> _
    Public Property Key3() As Integer
        Get
            Return _Key3
        End Get
        Set(ByVal value As Integer)
            _Key3 = value
        End Set
    End Property
End Class

<DataContract()> _
Public Class QuestionRDENTCountObject
    Private _SurveyID As Integer
    <DataMember()> _
    Public Property SurveyID() As Integer
        Get
            Return _SurveyID
        End Get
        Set(ByVal value As Integer)
            _SurveyID = value
        End Set
    End Property
    Private _QuestionID As Integer
    <DataMember()> _
    Public Property QuestionID() As Integer
        Get
            Return _QuestionID
        End Get
        Set(ByVal value As Integer)
            _QuestionID = value
        End Set
    End Property
    Private _RDENTCount As Integer
    <DataMember()> _
    Public Property RDENTCount() As Integer
        Get
            Return _RDENTCount
        End Get
        Set(ByVal value As Integer)
            _RDENTCount = value
        End Set
    End Property
End Class

<DataContract()> _
<Serializable()> _
Public Class ResultsFilterGroupObject 'this is only one row in a ResultsFilterModel....
    Private Const NoFilter = "NoFilter"
#Region "Methods"
    Public Function ToAddress() As String
        Dim rslt As String = NoFilter
        Dim strbldr As New Text.StringBuilder
        For Each q As Integer In QuestionIDList.OrderBy(Function(qint) qint)
            strbldr.Append("Q" & q.ToString)
        Next
        For Each opt As Integer In OptionIDList.OrderBy(Function(optint) optint)
            strbldr.Append("O" & opt.ToString)
        Next
        rslt = strbldr.ToString
        Return rslt
    End Function
#End Region

#Region "Properties"
    Private _SurveyID As Integer
    <DataMember()> _
    Public Property SurveyID() As Integer
        Get
            Return _SurveyID
        End Get
        Set(ByVal value As Integer)
            _SurveyID = value
        End Set
    End Property
    Private _QuestionIDList As List(Of Integer)
    <DataMember()> _
    Public Property QuestionIDList() As List(Of Integer)
        Get
            Return _QuestionIDList
        End Get
        Set(ByVal value As List(Of Integer))
            _QuestionIDList = value
        End Set
    End Property
    Private _OptionIDList As List(Of Integer)
    <DataMember()> _
    Public Property OptionIDList() As List(Of Integer)
        Get
            Return _OptionIDList
        End Get
        Set(ByVal value As List(Of Integer))
            _OptionIDList = value
        End Set
    End Property
#End Region
End Class

<DataContract()> _
Public Class ResultsFilterModelObject
    Private _Guid As Guid
    <DataMember()> _
    Public Property Guid() As Guid
        Get
            Return _Guid
        End Get
        Set(ByVal value As Guid)
            _Guid = value
        End Set
    End Property

    Private _ID As Integer
    <DataMember()> _
    Public Property ID() As Integer
        Get
            Return _ID
        End Get
        Set(ByVal value As Integer)
            _ID = value
        End Set
    End Property

    Private _SurveyID As Integer
    <DataMember()> _
    Public Property SurveyID() As Integer
        Get
            Return _SurveyID
        End Get
        Set(ByVal value As Integer)
            _SurveyID = value
        End Set
    End Property

    Private _Model As String
    <DataMember()> _
    Public Property Model() As String
        Get
            Return _Model
        End Get
        Set(ByVal value As String)
            _Model = value
        End Set
    End Property
End Class
