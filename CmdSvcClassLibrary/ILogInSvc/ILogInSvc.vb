Imports System.Runtime.Serialization
Imports CmdInfrastructureNS
Imports System.ServiceModel
Imports IEndPtDataCntxtSvcNS


<ServiceContract()> _
Public Interface ILogInSvc
    

    <OperationContract()> _
    Function SignMeUpPlease(ByVal _SignUpPack As SignUpPackage) As SignUpResult

    <OperationContract()> _
   Function LogMeInPlease(ByVal _LogInPack As LogInPackage) As LogInResult

    <OperationContract()> _
    Function LogMeOutPlease(ByVal _LogOutPack As LogOutPackage) As LogOutResult


    <OperationContract()> _
    Function WhoIsHere(ByVal _WhoIsHerePack As WhoIsHerePackage) As WhoIsHereResult


End Interface

<DataContract()> _
Public Class SignUpPackage
    Private _LogInPkg As LogInPackage
    <DataMember()> _
    Public Property LogInPkg() As LogInPackage
        Get
            Return _LogInPkg
        End Get
        Set(ByVal value As LogInPackage)
            _LogInPkg = value
        End Set
    End Property

    Private _OrgCompanyName As String
    <DataMember()> _
    Public Property OrgCompanyName() As String
        Get
            Return _OrgCompanyName
        End Get
        Set(ByVal value As String)
            _OrgCompanyName = value
        End Set
    End Property

    Private _GSMLoginID As Integer
    Public Property GSMLoginID() As Integer
        Get
            Return _GSMLoginID
        End Get
        Set(ByVal value As Integer)
            _GSMLoginID = value
        End Set
    End Property

    Private _Destination_SurveyMaster_CnxnString As String
    Public Property Destination_SurveyMaster_CnxnString() As String
        Get
            Return _Destination_SurveyMaster_CnxnString
        End Get
        Set(ByVal value As String)
            _Destination_SurveyMaster_CnxnString = value
        End Set
    End Property

    Private _Destination_DataStore_CnxnString As String
    Public Property Destination_DataStore_CnxnString() As String
        Get
            Return _Destination_DataStore_CnxnString
        End Get
        Set(ByVal value As String)
            _Destination_DataStore_CnxnString = value
        End Set
    End Property

    Private _NormalizedEmailAddress As String = Nothing
    Public ReadOnly Property NormalizedEmailAddress() As String
        Get
            Return _NormalizedEmailAddress
        End Get
    End Property
End Class
Public Class SignUpResult
    Private _LogInRslt As LogInResult
    Public Property LogInRslt() As LogInResult
        Get
            Return _LogInRslt
        End Get
        Set(ByVal value As LogInResult)
            _LogInRslt = value
        End Set
    End Property
End Class

<DataContract()> _
Public Class LogInPackage



    Private _LogIn_Email As String
    <DataMember()> _
    Public Property LogIn_Email() As String
        Get
            Return Me._LogIn_Email
        End Get
        Set(ByVal value As String)
            Me._LogIn_Email = value
        End Set
    End Property

    Private _PasswordHashINT As Integer
    <DataMember()> _
    Public Property PasswordHashINT() As Integer
        Get
            Return Me._PasswordHashINT
        End Get
        Set(ByVal value As Integer)
            Me._PasswordHashINT = value
        End Set
    End Property

    Private _PasswordHash As HashSet(Of String)
    <DataMember()> _
    Public Property PasswordHash() As HashSet(Of String)
        Get
            Return Me._PasswordHash
        End Get
        Set(ByVal value As HashSet(Of String))
            Me._PasswordHash = value
        End Set
    End Property

End Class
<DataContract()> _
Public Class LogInResult

    Private _LogIn_IsSuccess As Boolean
    <DataMember()> _
    Public Property LogIn_IsSuccess() As Boolean
        Get
            Return Me._LogIn_IsSuccess
        End Get
        Set(ByVal value As Boolean)
            Me._LogIn_IsSuccess = value
        End Set
    End Property

    Private _LogIn_DateTime As Date
    <DataMember()> _
    Public Property LogIn_DateTime() As Date
        Get
            Return Me._LogIn_DateTime
        End Get
        Set(ByVal value As Date)
            Me._LogIn_DateTime = value
        End Set
    End Property

    Private _EndpointKeysList As List(Of EndPtPackage)
    <DataMember()> _
    Public Property EndpointKeysList() As List(Of EndPtPackage)
        Get
            Return Me._EndpointKeysList
        End Get
        Set(ByVal value As List(Of EndPtPackage))
            Me._EndpointKeysList = value
        End Set
    End Property

    Private _SpiffList As List(Of Srlzd_KVP)
    <DataMember()> _
    Public Property SpiffList() As List(Of Srlzd_KVP)
        Get
            Return Me._SpiffList
        End Get
        Set(ByVal value As List(Of Srlzd_KVP))
            Me._SpiffList = value
        End Set
    End Property
End Class

<DataContract()> _
Public Class LogOutPackage

    Private _LogIn_Email As String
    <DataMember()> _
    Public Property LogIn_Email() As String
        Get
            Return Me._LogIn_Email
        End Get
        Set(ByVal value As String)
            Me._LogIn_Email = value
        End Set
    End Property

    Private _LogIn_Result As LogInResult
    <DataMember()> _
    Public Property LogIn_Result() As LogInResult
        Get
            Return Me._LogIn_Result
        End Get
        Set(ByVal value As LogInResult)
            Me._LogIn_Result = value
        End Set
    End Property


End Class
<DataContract()> _
Public Class LogOutResult

    Private _SpiffList As List(Of Srlzd_KVP)
    <DataMember()> _
    Public Property SpiffList() As List(Of Srlzd_KVP)
        Get
            Return Me._SpiffList
        End Get
        Set(ByVal value As List(Of Srlzd_KVP))
            Me._SpiffList = value
        End Set
    End Property

End Class

<DataContract()> _
Public Class WhoIsHerePackage

    Private boolValueField As Boolean
    Private stringValueField As String

    <DataMember()> _
    Public Property BoolValue() As Boolean
        Get
            Return Me.boolValueField
        End Get
        Set(ByVal value As Boolean)
            Me.boolValueField = value
        End Set
    End Property

    <DataMember()> _
    Public Property StringValue() As String
        Get
            Return Me.stringValueField
        End Get
        Set(ByVal value As String)
            Me.stringValueField = value
        End Set
    End Property

End Class
<DataContract()> _
Public Class WhoIsHereResult

    Private boolValueField As Boolean
    Private stringValueField As String

    <DataMember()> _
    Public Property BoolValue() As Boolean
        Get
            Return Me.boolValueField
        End Get
        Set(ByVal value As Boolean)
            Me.boolValueField = value
        End Set
    End Property

    <DataMember()> _
    Public Property StringValue() As String
        Get
            Return Me.stringValueField
        End Get
        Set(ByVal value As String)
            Me.stringValueField = value
        End Set
    End Property

End Class