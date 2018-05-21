Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports CmdInfrastructureNS



<ServiceContract()> _
Public Interface IPgItemColxnSvc
    <OperationContract(IsOneWay:=False)> _
    Function IamWorking() As String

    <OperationContract(IsOneWay:=True)> _
    Sub SetCache(ByVal _eptSuffix As String)

    '''' <summary>
    '''' Saves  _PageItemModel to the SurveyDataStore...returns KeyValuePaier(original poco.SDS_ID,_PageITemModelID on the db...)
    '''' </summary>
    '''' <param name="_PageItemModel"></param>
    '''' <returns>KeyValuePair(of poco_SDSID, xxxID of the row returned</returns>
    '''' <remarks></remarks>
    <OperationContract()> _
    Function SavePageItemModel(ByVal _PageItemModel As PgItemModel_Package, ByVal _surveyID As Integer) _
    As POCO_ID_Pkg

    '<OperationContract()> _
    'Function RetrievePageItemModelList(ByVal _SurveyID As Integer) As List(Of PgItemModel)

    <OperationContract()> _
    Function SavePageContentModel(ByVal _PageContentModel As PgContentModel_Package, ByVal _PIMID As Integer, ByVal _SurveyID As Integer) _
    As POCO_ID_Pkg

    '<OperationContract()> _
    'Function RetrievePageContentModelList(ByVal _PgItemModelID As Integer) As List(Of PageContentModel)

    <OperationContract()> _
    Function SavePageContentElement(ByVal _PageContentElement As PCElement_Package, ByVal _PCMID As Integer, ByVal _SurveyID As Integer) _
    As POCO_ID_Pkg

    '<OperationContract()> _
    'Function RetrievePageContentElementList(ByVal _PageContentModelID As Integer) As List(Of PageContentElement)


    <OperationContract()> _
   Function Retrieve_PageBlobInfo_WithIndex(ByVal _SurveyID As Integer, ByVal _IndexOfPage As Integer) As PageBlobInfo

    <OperationContract()> _
    Function Retrieve_PageBlob_Count(ByVal _SurveyID As Integer) As Integer

    <OperationContract()> _
    Function Retrieve_PgItemModelIDsOnly(ByVal _SurveyID As Integer) As List(Of Integer)

End Interface

