Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports CmdInfrastructureNS

<ServiceContract()> _
Public Interface ICustomerDBSvc

    '<OperationContract(IsOneWay:=True)> _
    'Sub SetCache(ByVal value As String)

    '<OperationContract()> _
    'Function Retrieve_QueueInfo(ByVal _SurveyID As Integer) As String

    '<OperationContract()> _
    'Function Retrieve_ComputerPrivileges_Pkg(ByVal LoginEmail As String) As ComputerPrivilegePackage

    '<OperationContract()> _
    'Function Retrieve_DC_Package(ByVal _LogIn_Email As String, ByVal _computerID As Integer) As DC_Package

    <OperationContract()> _
    Function Tiny_Survey_Row_List_UsingLogInEmail(ByVal _LoginEmail As String, ByVal _PrivColxn As List(Of PriviligeDescrEnum)) _
    As List(Of Tiny_Survey_RowWith_SIModel)

    <OperationContract()> _
    Function RetrieveSurveyRow_WithSurveyID(ByVal _SurveyID As Integer) As Tiny_Survey_RowWith_SIModel

    <OperationContract()> _
    Function AddSurveyRow(ByVal _SurveyItem_Pkg As SurveyItem_Package) As POCO_ID_Pkg

    <OperationContract()> _
    Function Update_SurveyMaster_WithModel(ByVal _SurveyId As Integer, ByVal _Model As String) As Boolean

    <OperationContract()> _
    Function RemoveSurveyRow(ByVal _SurveyID As Integer) As Boolean


    <OperationContract()> _
    Function SurveyMetaData_List(ByVal _LoginID As Integer) As List(Of SurveyMetadataPackage)

    <OperationContract()> _
    Function SurveyMetaData(ByVal _SurveyID As Integer) As SurveyMetadataPackage





    



    
    ' <OperationContract()> _
    'Function UpdateSurveyRow(ByVal _SurveyMaster As SurveyMaster) As Boolean

   

End Interface


