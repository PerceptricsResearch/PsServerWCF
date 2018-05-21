Imports IPageItemsSvcNS
Imports CmdInfrastructureNS
Imports System.ServiceModel


<ServiceContract()> _
Public Interface IRespProviderSvc
    <OperationContract()> _
    Function Retrieve_Page(ByVal _SurveyID As Integer, ByVal _IndexOfPages As Integer) As Page_Package

    <OperationContract(IsOneWay:=True)> _
  Sub SetCache(ByVal _eptSuffix As String)

    <OperationContract()> _
    Sub StoreRespondentModel(ByVal _RDentModel As IPostResponsetoSurveySvcNS.ResponDENTModel)

    <OperationContract()> _
    Function RetrievePageItemModelList(ByVal _SurveyID As Integer) As List(Of PgItemModel)

    <OperationContract()> _
    Function RetrievePageContentModelList(ByVal _PgItemModelID As Integer) As List(Of PageContentModel)


    <OperationContract()> _
    Function RetrievePageContentElementList(ByVal _PageContentModelID As Integer) As List(Of PageContentElement)

    <OperationContract()> _
    Function Retrieve_PageBlobInfo_WithIndex(ByVal _SurveyID As Integer, ByVal _IndexOfPage As Integer) As PageBlobInfo

    <OperationContract()> _
    Function Retrieve_PageBlob_Count(ByVal _SurveyID As Integer) As Integer

    <OperationContract()> _
    Function Retrieve_PgItemModelIDsOnly(ByVal _SurveyID As Integer) As List(Of Integer)

    <OperationContract()> _
  Function RetrieveSurveyImagesPackagePCMID(ByVal _SurveyID As Integer, ByVal _PCMID As Integer) As SurveyImagesPackage

End Interface
