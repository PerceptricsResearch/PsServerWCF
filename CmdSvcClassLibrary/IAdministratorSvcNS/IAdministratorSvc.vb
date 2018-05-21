Imports System.ServiceModel
Imports CmdSvcClassLibrary
Imports CmdInfrastructureNS
Imports System.Runtime.Serialization
Imports ILogInSvc
Imports MasterDBSvcLibr
Imports System.ServiceModel.Configuration
Imports DataContextPackageNS
Imports System.ServiceModel.Web

<ServiceContract()> _
Public Interface IAdministratorSvc

    <OperationContract()> _
    Function LogMeInPlease(ByVal _LogInPack As LogInPackage) As LogInResult

    <OperationContract()> _
    Function LogMeOutPlease(ByVal _LogOutPack As LogOutPackage) As LogOutResult


    <OperationContract()> _
    Sub ResetPassword(ByVal _PwdPkg As Password_Package)

    <OperationContract()> _
    Sub Remove_UserCacheInfo(ByVal _UCMDxnryKey As String)

    <OperationContract()> _
    Function Retrieve_UserCacheInfo(ByVal _UCMDxnryKey As String) As List(Of UserCacheInfo)

    <OperationContract()> _
       <WebGet(UriTemplate:="administrator/ucmdxnry", ResponseFormat:=WebMessageFormat.Json)> _
    Function Retrieve_UserCacheManagerDxnry() As List(Of UserCacheInfo)

    <OperationContract()> _
        <WebGet(UriTemplate:="administrator/wcfsvcmgrdxnry", ResponseFormat:=WebMessageFormat.Json)> _
    Function Retrieve_WCFSvcMgr_ServiceDxnry() As List(Of Slzble_CustomSvcHostModel)

    <OperationContract()> _
    Function Retrieve_AllowedServices_List() As List(Of Srlzd_KVP)

    <OperationContract()> _
    Function ServicesToStartVersion2(ByVal _LogIn_Email As String) As List(Of ServiceStartPackage)

    <OperationContract()> _
    Function GetGlobalMasterData(ByVal _LogInPack As LogInPackage) As GlobalMasterDataByLoginNameResult_Package

    <OperationContract()> _
    Function PermittedServiceElements(ByVal _PermittedSvcsBmsk As ULong) As List(Of ServiceElement)

End Interface
<DataContract()> _
Public Class Slzble_CustomSvcHostModel
    Private _EndPtURIS As List(Of String)
    <DataMember()> _
    Public Property EndPtURIS() As List(Of String)
        Get
            Return _EndPtURIS
        End Get
        Set(ByVal value As List(Of String))
            _EndPtURIS = value
        End Set
    End Property

    Private _DC_Pkg As DC_Package
    <DataMember()> _
    Public Property DC_Pkg() As DC_Package
        Get
            Return _DC_Pkg
        End Get
        Set(ByVal value As DC_Package)
            _DC_Pkg = value
        End Set
    End Property

    Private _EndPtSuffix As String = ""
    <DataMember()> _
    Public Property EndPtSuffix() As String
        Get
            Return _EndPtSuffix
        End Get
        Set(ByVal value As String)
            _EndPtSuffix = value
        End Set
    End Property

    Private _InstanceCount As Integer = 0
    <DataMember()> _
    Public Property InstanceCount() As Integer
        Get
            Return _InstanceCount
        End Get
        Set(ByVal value As Integer)
            _InstanceCount = value

        End Set
    End Property

    Private _LastIntanceCreatedDateTime As Date
    <DataMember()> _
    Public Property LastInanceCreatedDateTime() As Date
        Get
            Return _LastIntanceCreatedDateTime
        End Get
        Set(ByVal value As Date)
            _LastIntanceCreatedDateTime = value

        End Set
    End Property

    Private _DataContextConnectionString As String
    <DataMember()> _
    Public Property DataContextConnectionString() As String
        Get
            Return _DataContextConnectionString
        End Get
        Set(ByVal value As String)
            _DataContextConnectionString = value

        End Set
    End Property

    Private _LastSurveyID As Integer
    <DataMember()> _
    Public Property LastSurveyID() As Integer
        Get
            Return _LastSurveyID
        End Get
        Set(ByVal value As Integer)
            _LastSurveyID = value

        End Set
    End Property

    Private _LastOperationKVP As KeyValuePair(Of String, String)
    Public Property LastOperationKVP() As KeyValuePair(Of String, String)
        Get
            Return _LastOperationKVP
        End Get
        Set(ByVal value As KeyValuePair(Of String, String))
            _LastOperationKVP = value

        End Set
    End Property
End Class