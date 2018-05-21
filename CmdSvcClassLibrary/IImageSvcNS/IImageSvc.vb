Imports System.Runtime.Serialization
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Web
Imports System.ServiceModel
Imports System.IO
Imports IResultsProviderSvcNS
Imports CmdInfrastructureNS

<ServiceContract()> _
Public Interface IImageSvc

#Region "ImageManagerMethods"
    <OperationContract()> _
<WebGet(UriTemplate:="mgrimgguidstrlist/{email}", ResponseFormat:=WebMessageFormat.Json)> _
    Function ManagerGuidStringsPlease(ByVal email As String) As List(Of String)

    <OperationContract()> _
<WebGet(UriTemplate:="mgrimg/{email}/{guidstr}")> _
    Function ManagerImagePlease(ByVal email As String, ByVal guidstr As String) As Stream

    <OperationContract()> _
<WebGet(UriTemplate:="mgrimglarge/{email}/{guidstr}")> _
    Function ManagerLargeImagePlease(ByVal email As String, ByVal guidstr As String) As Stream

    <OperationContract()> _
<WebGet(UriTemplate:="mgrimgsmall/{email}/{guidstr}")> _
    Function ManagerSmallImagePlease(ByVal email As String, ByVal guidstr As String) As Stream

    <OperationContract()> _
<WebGet(UriTemplate:="mgrimgthumb/{email}/{guidstr}")> _
    Function ManagerThumbnailImagePlease(ByVal email As String, ByVal guidstr As String) As Stream
#End Region

#Region "Preliminary proof of concept contracts"
    <OperationContract()> _
   <WebGet(UriTemplate:="testme")> _
    Function TestMe() As String

    <OperationContract()> _
    <WebGet(UriTemplate:="login/{email}/{sid}/{sqn}")> _
    Function LogInPlease(ByVal email As String, ByVal sid As String, ByVal sqn As String) As String

    <OperationContract()> _
  <WebGet(UriTemplate:="image/{email}/{sid}/{sqn}")> _
    Function ImagePlease(ByVal email As String, ByVal sid As String, ByVal sqn As String) As Stream

    <OperationContract()> _
<WebGet(UriTemplate:="imagepcelem/{email}/{sid}/{pcmid}/{pcelemid}")> _
    Function ImagePCElemPlease(ByVal email As String, ByVal sid As String, ByVal pcmid As String, ByVal pcelemid As String) As Stream

    <OperationContract()> _
<WebGet(UriTemplate:="imagepcelemidlist/json/{email}/{sid}/{pcmid}", ResponseFormat:=WebMessageFormat.Json)> _
    Function ImagePCElemIDsList(ByVal email As String, ByVal sid As String, ByVal pcmid As String) As List(Of Integer)
#End Region

#Region "ResultsSvc webhttp contracts"
    <OperationContract()> _
<WebGet(UriTemplate:="sdsresponsemodels/json/{email}/{sid}", ResponseFormat:=WebMessageFormat.Json)> _
    Function Retrieve_SDSResponseModels(email As String, ByVal sid As String) As List(Of SDSResponseModelObject)

    <OperationContract()> _
 <WebGet(UriTemplate:="results/json/{email}/{sid}", ResponseFormat:=WebMessageFormat.Json)> _
    Function RetrieveResults(ByVal email As String, ByVal sid As String) As List(Of ResultsProviderSummaryObject)

    <OperationContract()> _
<WebGet(UriTemplate:="filters/json/{email}/{sid}", ResponseFormat:=WebMessageFormat.Json)> _
    Function Retrieve_ResultsFilters(email As String, ByVal sid As String) As List(Of ResultsFilterModelObject)

    <OperationContract()> _
<WebGet(UriTemplate:="resultsfilteredrfmid/json/{email}/{sid}/{rfmid}", ResponseFormat:=WebMessageFormat.Json)> _
    Function RetrieveFilteredResults_with_RFMID(ByVal email As String, ByVal sid As String, ByVal rfmid As String) _
    As List(Of ResultsProviderSummaryObject)
#End Region

#Region "AuthoringSvc webhttp contracts"
    <OperationContract()> _
<WebGet(UriTemplate:="authoring/json/{email}", ResponseFormat:=WebMessageFormat.Json)> _
    Function Retrieve_SurveyMetaData_List(ByVal email As String) As List(Of SurveyMetadataPackage)

    <OperationContract()> _
<WebGet(UriTemplate:="authoring/json/surveymetadata/{email}/{sid}", _
        ResponseFormat:=WebMessageFormat.Json)> _
    Function Retrieve_SurveyMetaData(ByVal email As String, ByVal sid As String) As SurveyMetadataPackage

    <OperationContract()> _
<WebGet(UriTemplate:="authoring/json/pageblob_count/{email}/{sid}", _
    ResponseFormat:=WebMessageFormat.Json)> _
    Function Retrieve_PageBlob_Count(ByVal email As String, ByVal sid As String) _
    As Integer

    <OperationContract()> _
<WebGet(UriTemplate:="authoring/json/page/{email}/{sid}/{ndx}", _
ResponseFormat:=WebMessageFormat.Json)> _
    Function Retrieve_Page(ByVal email As String, ByVal sid As String, ByVal ndx As String) _
    As Page_Package
#End Region

End Interface


