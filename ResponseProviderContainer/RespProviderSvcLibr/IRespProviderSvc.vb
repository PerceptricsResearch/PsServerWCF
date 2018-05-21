Imports RespProviderSvcLibr.PgItemColxnSvcNS

' NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in App.config.
<ServiceContract()> _
Public Interface IRespProviderSvc

    <OperationContract()> _
    Sub StoreRespondentModel(ByVal _RDentModel As RespDispatchSvc.ResponDENTModel)

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

End Interface



