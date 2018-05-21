' NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in App.config.
<ServiceContract()> _
Public Interface ILoginSvc

    <OperationContract()> _
    Function LogMeInPlease(ByVal _LogInPack As LogInPackage) As LogInResult

    <OperationContract()> _
    Function LogMeOutPlease(ByVal _LogOutPack As LogOutPackage) As LogOutResult

    <OperationContract()> _
     Function ChangeMyPassword(ByVal _ChangePasswordPack As ChangePasswordPackage) As ChangePasswordResult

    <OperationContract()> _
    Function ResetMyPassword(ByVal _ResetPasswordPack As ResetPasswordPackage) As ResetPasswordResult

    <OperationContract()> _
    Function WhoIsHere(ByVal _WhoIsHerePack As WhoIsHerePackage) As WhoIsHereResult
End Interface

' Use a data contract as illustrated in the sample below to add composite types to service operations
<DataContract()> _
Public Class Srlzd_KVP

    Public Sub New(ByVal _KeyString, ByVal _ValueString)
        Me._Key = _KeyString
        Me._Valu = _ValueString
    End Sub

    Private _Key As String
    <DataMember()> _
    Public Property Key() As String
        Get
            Return Me._Key
        End Get
        Set(ByVal value As String)
            Me._Key = value
        End Set
    End Property

    Private _Valu As String
    <DataMember()> _
    Public Property Valu() As String
        Get
            Return Me._Valu
        End Get
        Set(ByVal value As String)
            Me._Valu = value
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

    Private _EndpointKeysList As List(Of EndPtDCSvc.EndPtPackage)
    <DataMember()> _
    Public Property EndpointKeysList() As List(Of EndPtDCSvc.EndPtPackage)
        Get
            Return Me._EndpointKeysList
        End Get
        Set(ByVal value As List(Of EndPtDCSvc.EndPtPackage))
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
Public Class ChangePasswordPackage

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
Public Class ChangePasswordResult

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
Public Class ResetPasswordPackage

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
Public Class ResetPasswordResult

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