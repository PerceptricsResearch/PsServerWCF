Imports CmdInfrastructureNS
Imports IPageItemsSvcNS
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web

<ServiceContract()> _
Public Interface IAuthoringSvc

    <OperationContract()> _
    Function SendEmailPlease(ByVal _SendEmailpkg As SendEmail_Package) As Boolean

    <OperationContract()> _
    Function ChangeMyPwdPlease(ByVal _PwdPkg As Password_Package) As Boolean

#Region "ImageManagerMethods"
    <OperationContract()> _
    Function RetrieveManagerGuidStrList(_imgsize As Integer) As List(Of String)
    <OperationContract()> _
    Function AddManagerImages(ByVal _mgrimgspkg As SurveyImagesPackage) As Boolean
    <OperationContract()> _
    Function RemoveManagerImages(ByVal _guidstr As String) As Boolean
#End Region

#Region "Retrieve Methods"
    '<WebGet(UriTemplate:="RFMImagesRFMID/{_SurveyID}/{_RFMID}")> _
    <OperationContract()> _
    Function RetrieveResultsFilterModelImagesRFMID(ByVal _SurveyID As Integer, ByVal _RFMID As Integer) As SurveyImagesPackage

    <OperationContract()> _
    Function RetrieveSurveyImagesPackagePCMID(ByVal _SurveyID As Integer, ByVal _PCMID As Integer) As SurveyImagesPackage

    <OperationContract()> _
    Function RetrieveSurveyImagesColxn_SequenceNumber(ByVal _SurveyID As Integer, ByVal _SequenceNumber As Integer) As SurveyImagesPackage

    <OperationContract()> _
    Function RetrieveSurveyImagesColxnAll(ByVal _SurveyID As Integer) As SurveyImagesPackage


    <OperationContract()> _
    Function Retrieve_TinySurveyRowWithModel_List(ByVal _LoginEmail As String) _
    As List(Of Tiny_Survey_RowWith_SIModel)

    <OperationContract()> _
    Function Retrieve_PageBlobInfo_WithIndex(ByVal _SurveyID As Integer, ByVal _IndexOfPage As Integer) _
    As PageBlobInfo

    <OperationContract()> _
    Function Retrieve_PageBlob_Count(ByVal _SurveyID As Integer) As Integer

    <OperationContract()> _
    Function Retrieve_Page(ByVal _SurveyID As Integer, ByVal _IndexOfPages As Integer) As Page_Package

    <OperationContract()> _
    Function Retrieve_SurveyMetaData_withModel_List(ByVal _LoginEmail As String) As List(Of SurveyMetadataPackage)

    <OperationContract()> _
    Function Retrieve_SurveyMetaData_List(ByVal _LoginEmail As String) As List(Of SurveyMetadataPackage)

    <OperationContract()> _
    Function Retrieve_ActiveOrNewResultsSMDList(ByVal _LoginEmail As String) As List(Of SurveyMetadataPackage)

    <OperationContract()> _
    Function Retrieve_SurveyMetaData(ByVal _SurveyID As Integer) As SurveyMetadataPackage

    <OperationContract()> _
    Function Retrieve_SurveyPrivilegeModels_List() As List(Of SurveyPrivilegeModel)

    <OperationContract()> _
    Function Update_SurveyPrivilegeModels(ByVal _SurveyPrivilegesList As List(Of SurveyPrivilegeModel)) As List(Of SurveyPrivilegeModel)

    <OperationContract()> _
    Function Retrieve_GuestLoginInfo_List() As List(Of GuestLoginInfo)

    <OperationContract()> _
    Function Add_GuestLoginInfo(ByVal _EmailAddress As String) As List(Of GuestLoginInfo)

    <OperationContract()> _
    Function Update_LoginInfo_PrivBitmask(ByVal _TargetGuestLoginInfo As GuestLoginInfo) As Boolean
#End Region

#Region "Save Methods"
    <OperationContract()> _
    Sub SaveResultsFilterModelImages(ByVal _SurveyImgsPkg As SurveyImagesPackage)


    <OperationContract()> _
    Function ChangeSurveyStatus(ByVal _ChangeSurveyStatePkg As ChangeSurveyStatePackage) As Integer

    <OperationContract()> _
    Sub SaveSurveyImagesColxn(ByVal _SurveyImages_Pkg As SurveyImagesPackage)

    <OperationContract()> _
    Function SaveSurveyItemModel(ByVal _SurveyItem_Pkg As SurveyItem_Package) As POCO_ID_Pkg

    <OperationContract()> _
    Function SavePage(ByVal _Page_Pkg As Page_Package) As List(Of POCO_ID_Pkg)

    '''' <summary>
    '''' Saves  _PageItemModel to the SurveyDataStore...returns KeyValuePaier(original poco.SDS_ID,_PageITemModelID on the db...)
    '''' </summary>
    '''' <param name="_PageItemModel"></param>
    '''' <returns>KeyValuePair(of poco_SDSID, xxxID of the row returned</returns>
    '''' <remarks></remarks>
    '<OperationContract()> _
    'Function SavePageItemModel(ByVal _PageItemModel As PgItemModel) As POCO_ID_Pkg

    '<OperationContract()> _
    'Function SavePageContentModel(ByVal _PageContentModel As PageContentModel) As POCO_ID_Pkg

    '<OperationContract()> _
    'Function SavePageContentElement(ByVal _PageContentElement As PageContentElement) As POCO_ID_Pkg
#End Region


End Interface
