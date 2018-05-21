Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports CmdInfrastructureNS

<ServiceContract()> _
Public Interface IMasterSurveyDBSvc

    <OperationContract()> _
    Function Tiny_Survey_Row_List(ByVal _LoginID As Integer) As List(Of Tiny_Survey_Row)

    <OperationContract()> _
   Function Tiny_Survey_Row(ByVal _SurveyID As Integer) As List(Of Tiny_Survey_Row)






    <OperationContract()> _
   Function Tiny_Survey_Row_WithSurveyItemModel_List(ByVal _LoginID As Integer) As List(Of Tiny_Survey_RowWith_SIModel)

    <OperationContract()> _
   Function Tiny_Survey_Row_WithSurveyItemModel(ByVal _SurveyID As Integer) As List(Of Tiny_Survey_RowWith_SIModel)

    <OperationContract()> _
    Function Update_SurveyMaster_WithModel(ByVal _SurveyId As Integer, ByVal _Model As String) As Boolean

    'DONT USE SURVEYID...IT IS LOCAL TO A CUSTOMERSURVEYMASTERDB...
    'use RDENTModel.LogInEmail...Get the CustomerID associated with LogInEmail...
    'From CustomerTable...Select the RDENTQueueURI_ID...is an ID on SurveyDataStoreTable
    'From SurveyDataStoreTable...Select AbsolutePath...set founduri using AbsolutePath...
    'For QueueName ...Select SurveyDataStore.DatabaseName...for queues it contains the queueName....
    <OperationContract()> _
    Function TinySurveyRow_UsingLogInEmail(ByVal _LoginEmail As String) As Tiny_Survey_Row



    <OperationContract()> _
    Function SurveyMetaData_List(ByVal _LoginID As Integer) As List(Of SurveyMetadataPackage)

    <OperationContract()> _
    Function SurveyMetaData(ByVal _SurveyID As Integer) As SurveyMetadataPackage





    ' <OperationContract()> _
    ' Function RetrieveSurveyRow_WithSurveyID(ByVal _SurveyID As Integer) As SurveyMaster

    ' <OperationContract()> _
    'Function RetrieveSurveyRow_WithLoginId(ByVal _LoginId As Integer) As List(Of SurveyMaster)

    ' <OperationContract()> _
    ' Function AddSurveyRow(ByVal _SurveyMaster As SurveyMaster) As Integer 'is the ID of the added surveymaster...

    ' <OperationContract()> _
    'Function UpdateSurveyRow(ByVal _SurveyMaster As SurveyMaster) As Boolean

    ' <OperationContract()> _
    ' Function RemoveSurveyRow(ByVal _SurveyID As Integer) As Boolean





End Interface

