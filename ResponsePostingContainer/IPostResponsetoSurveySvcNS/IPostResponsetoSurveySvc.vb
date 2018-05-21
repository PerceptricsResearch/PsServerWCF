Imports System.ServiceModel
Imports System.Runtime.Serialization

' NOTE: If you change the class name "IPostResponsetoSurveySvc" here, you must also update the reference to "IPostResponsetoSurveySvc" in App.config.
<ServiceContract()> _
Public Interface IPostResponsetoSurveySvc

    <OperationContract(IsOneWay:=True)> _
    Sub EstablishSvc(ByVal _EstablishCommandString As String, ByVal _TargetSurveyName As String)

    <OperationContract(IsOneWay:=True)> _
    Sub SubmitRespondentModel(ByVal _RDentModel As ResponDENTModel)

End Interface
'RESPONDENT MODEL and others SHOULD be in a Common Class Library where things like this can be shared/have a common definition throughout...
'is especially important insofar as this DataContract gets used in SLResponsePortal, ResponseProvider, Dispatcher, and ResponsePosting... 

<DataContract()> _
Public Class ResponDENTModel

    Private _SurveyID As Integer = 0
    <DataMember()> _
    Public Property SurveyID() As Integer
        Get
            Return Me._SurveyID
        End Get
        Set(ByVal value As Integer)
            Me._SurveyID = value
        End Set
    End Property

    Private _LogInEmail As String = "NotSet"
    ''' <summary>
    ''' This is the Global LogInEmail that was used by the RDENT to get access to the Survey...look this up in GlobalSurveyMaster...
    ''' This tells us which CustomMasterDB this RDENT and SurveyID belong to...SurveyID's are local to a CustomerMasterSurveyDB
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()> _
    Public Property LogInEmail() As String
        Get
            Return _LogInEmail
        End Get
        Set(ByVal value As String)
            _LogInEmail = value
        End Set
    End Property

    Private _ID As Integer = 0
    <DataMember()> _
    Public Property ID() As Integer
        Get
            Return Me._ID
        End Get
        Set(ByVal value As Integer)
            Me._ID = value
        End Set
    End Property

    Private _ResponseColxn As New List(Of ResponseModel)
    <DataMember()> _
    Public Property ResponseColxn() As List(Of ResponseModel)
        Get
            Return Me._ResponseColxn
        End Get
        Set(ByVal value As List(Of ResponseModel))
            Me._ResponseColxn = value
        End Set
    End Property

    Private _CustomField As String = ""
    <DataMember()> _
    Public Property CustomField() As String
        Get
            Return Me._CustomField
        End Get
        Set(ByVal value As String)
            Me._CustomField = value
        End Set
    End Property

    Private _FirstName As String = ""
    <DataMember()> _
    Public Property FirstName() As String
        Get
            Return Me._FirstName
        End Get
        Set(ByVal value As String)
            Me._FirstName = value
        End Set
    End Property

    Private _LastName As String = ""
    <DataMember()> _
    Public Property LastName() As String
        Get
            Return Me._LastName
        End Get
        Set(ByVal value As String)
            Me._LastName = value
        End Set
    End Property

    Private _IPAddress As String = ""
    <DataMember()> _
    Public Property IPAddress() As String
        Get
            Return Me._IPAddress
        End Get
        Set(ByVal value As String)
            Me._IPAddress = value
        End Set
    End Property
End Class


<DataContract()> _
Public Class ResponseModel
    Private _ID As Integer = 0

    <DataMember()> _
    Public Property ID() As Integer
        Get
            Return Me._ID
        End Get
        Set(ByVal value As Integer)
            Me._ID = value
        End Set
    End Property

    Private _QuestID As Integer = 0
    <DataMember()> _
    Public Property QuestID() As Integer
        Get
            Return Me._QuestID
        End Get
        Set(ByVal value As Integer)
            Me._QuestID = value
        End Set
    End Property

    Private _Key1 As Integer = 0
    <DataMember()> _
    Public Property Key1() As Integer
        Get
            Return Me._Key1
        End Get
        Set(ByVal value As Integer)
            Me._Key1 = value
        End Set
    End Property

    Private _Key2 As Integer = 0
    <DataMember()> _
    Public Property Key2() As Integer
        Get
            Return Me._Key2
        End Get
        Set(ByVal value As Integer)
            Me._Key2 = value
        End Set
    End Property

    Private _Key3 As Integer = 0
    <DataMember()> _
    Public Property Key3() As Integer
        Get
            Return Me._Key3
        End Get
        Set(ByVal value As Integer)
            Me._Key3 = value
        End Set
    End Property
End Class
