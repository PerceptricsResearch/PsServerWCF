Imports System.Runtime.Serialization
Imports System.ServiceModel

<ServiceContract()> _
Public Interface IEndPtDataCntxtSvc
    ' <OperationContract()> _
    'Function SpinUpCustomerMasterDBSvc(ByVal _LoginEmail As String) As Boolean


    ''' <summary>
    ''' CustomerLoginSvc calls this operation to populate its LoginResult.EndpointKeysList
    ''' 
    ''' </summary>
    ''' <param name="_LogIn_Email"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <OperationContract()> _
    Function Retrieve_List_of_EndPtPkg(ByVal _LogIn_Email As String) As EptLoginPackage


    ''' <summary>
    ''' Issues ExposeEndPtWithDataContextCommands to MasterSurveyDBSvc.CommandSvc, PgItemsColxnSvc.CommandSvc, and ResultsProviderSvc.CommandSvc
    ''' CustomerLogInSvc is the Client that uses this Operation...
    ''' To get loginId call the Retrieve_list_of_endPtPkg operation first...it is returned in the LoginPackage...
    ''' </summary>
    ''' <param name="_LogIn_Email"></param>
    ''' <param name="_LoginID"></param>
    ''' <remarks>Provide a _LoginID as a string..</remarks>
    <OperationContract()> _
    Sub IssueExposeEndPtCommands(ByVal _LogIn_Email As String, ByVal _LoginID As Integer)
End Interface


<DataContract()> _
Public Class EptLoginPackage
    Private _LogIn_ID As Integer = -1
    <DataMember()> _
    Public Property LogIn_ID() As Integer
        Get
            Return _LogIn_ID
        End Get
        Set(ByVal value As Integer)
            _LogIn_ID = value
        End Set
    End Property

    Private _IsAuthenticated As Boolean = False
    <DataMember()> _
    Public Property IsAuthenticated() As Boolean
        Get
            Return _IsAuthenticated
        End Get
        Set(ByVal value As Boolean)
            _IsAuthenticated = value
        End Set
    End Property

    Private _ListOfEndPtPackage As New List(Of EndPtPackage)
    <DataMember()> _
    Public Property ListOfEndPtPackage() As List(Of EndPtPackage)
        Get
            Return _ListOfEndPtPackage
        End Get
        Set(ByVal value As List(Of EndPtPackage))
            _ListOfEndPtPackage = value
        End Set
    End Property

    Private _IsLoginEmailFound As Boolean = False
    <DataMember()> _
    Public Property IsLoginEmailFound() As Boolean
        Get
            Return _IsLoginEmailFound
        End Get
        Set(ByVal value As Boolean)
            _IsLoginEmailFound = value
        End Set
    End Property
End Class

<DataContract()> _
Public Class EndPtPackage

    Private _Name As String
    <DataMember()> _
    Public Property Name() As String
        Get
            Return Me._Name
        End Get
        Set(ByVal value As String)
            Me._Name = value
        End Set
    End Property

    Private _ServerName As String
    <DataMember()> _
    Public Property ServerName() As String
        Get
            Return Me._ServerName
        End Get
        Set(ByVal value As String)
            Me._ServerName = value
        End Set
    End Property

    Private _Address As String
    <DataMember()> _
    Public Property Address() As String
        Get
            Return Me._Address
        End Get
        Set(ByVal value As String)
            Me._Address = value
        End Set
    End Property

    Private _Suffix As String
    <DataMember()> _
    Public Property Suffix() As String
        Get
            Return Me._Suffix
        End Get
        Set(ByVal value As String)
            Me._Suffix = value
        End Set
    End Property

    Private _BaseAddress As String
    <DataMember()> _
    Public Property BaseAddress() As String
        Get
            Return Me._BaseAddress
        End Get
        Set(ByVal value As String)
            Me._BaseAddress = value
        End Set
    End Property

    Private _WCFSvcID As Integer
    '<DataMember()> _
    Public Property WCFSvcID() As Integer
        Get
            Return Me._WCFSvcID
        End Get
        Set(ByVal value As Integer)
            Me._WCFSvcID = value
        End Set
    End Property
End Class