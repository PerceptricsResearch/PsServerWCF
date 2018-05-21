Imports CmdInfrastructureNS
Imports System.Configuration
Imports System.ComponentModel
Imports System.Runtime.Serialization

<DataContract()> _
Public Enum SubscriptionLevel As Integer
    Basic = 1
    Preferred = 2
    Professional = 4
    Enterprise = 8
End Enum

<DataContract()> _
Public Enum SurveyState As Integer
    <EnumMember()> _
    AuthorDesign = 0
    <EnumMember()> _
    PublishedAccepting = 1
    <EnumMember()> _
    PublishedClosed = 2
End Enum

Public Enum SurveyType As Integer
    Placeholder = 0
    SystemTemplateTraditional = 1
    SystemTemplatePerceptrics = 2
    SystemSampleTraditional = 4
    SystemSamplePerceptrics = 8
    SystemHelp = 16
    SystemMarketing = 32
    SubscriberTemplateTraditional = 64
    SubscriberTemplatePerceptrics = 128

    SubscriberBespokeTraditional = 256
    SubscriberBespokePerceptrics = 512
End Enum

Public Class SharedMethods
    Public Shared Function EmailAddress_ToNormalized(ByVal _emailAddress As String) As String
        Dim rslt As String = Nothing

        If Not String.IsNullOrEmpty(_emailAddress) Then
            Dim replaces = From c In _emailAddress.ToLowerInvariant.ToCharArray _
                           Where Not Char.IsLetterOrDigit(c) Or Char.IsPunctuation(c) Or Char.IsSymbol(c) _
                           Select c
            Dim rsltstring As String = _emailAddress
            For Each c In replaces
                Dim x = Char.ToString(c)
                rsltstring = Strings.Replace(rsltstring, Char.ToString(c), "_")
            Next

            If rsltstring.Length > 50 Then
                rslt = Left(rsltstring.ToLowerInvariant, 50)
            Else
                rslt = Left(rsltstring.ToLowerInvariant, rsltstring.Length)
            End If
        End If
        Return rslt
    End Function

    Public Shared Function GetValueFromAppConfig(ByVal _ConfigKey) As String
        Dim rslt As String = Nothing
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        rslt = config.AppSettings.Settings(_ConfigKey).Value
        'rslt = ConfigurationManager.AppSettings(_ConfigKey)
        Return rslt
    End Function

    ''' <summary>
    ''' This is a Survey level function...is the UriString for a Survey that has been published...RDent's would put this string in a browser...
    ''' </summary>
    ''' <param name="_SubscrInfo"></param>
    ''' <param name="_SurveyID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ToSurveyRDent_LinkString(ByVal _SubscrInfo As SubscriberInfo, ByVal _SurveyID As Integer) As String
        Dim rslt As String = "NotSet"

        Return rslt
    End Function

    ''' <summary>
    ''' This is a Survey level function...is the QueueName for a Survey's RDent Queue...
    ''' </summary>
    ''' <param name="_SubscrInfo"></param>
    ''' <param name="_SurveyID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ToSurveyRDent_QueueName(ByVal _SubscrInfo As SubscriberInfo, ByVal _SurveyID As Integer) As String
        Dim rslt As String = "NotSet"
        Dim qName = Replace(SharedMethods.GetValueFromAppConfig("SubscriberRDENTQueueName"), "exxon", _SubscrInfo.NormalizedEmailAddress)
        rslt = qName & _SurveyID.ToString
        Return rslt
    End Function

    ''' <summary>
    ''' This is a Survey level function...is the UriString for a Survey's RDent Queue...
    ''' </summary>
    ''' <param name="_SubscrInfo"></param>
    ''' <param name="_SurveyID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ToSurveyRDent_QueueURI(ByVal _SubscrInfo As SubscriberInfo, ByVal _SurveyID As Integer) As String
        Dim rslt As String = "NotSet"
        Dim qURI = Replace(SharedMethods.GetValueFromAppConfig("SubscriberRDENTQueueURI"), "exxon", _SubscrInfo.NormalizedEmailAddress)
        rslt = qURI & _SurveyID.ToString
        Return rslt
    End Function
End Class


Public Class ActiveLoginInfo
    Public ReadOnly Property IsSubscriber_OR_IsAddedGuest() As Boolean
        Get
            Dim rslt As Boolean = True
            If Me.PrivBitMask And PriviligeDescrEnum.PerceptricsAdministrator Then
                rslt = False
            End If
            If Me.PrivBitMask And PriviligeDescrEnum.RDispatcherSvc Then
                rslt = False
            End If
            If Me.PrivBitMask And PriviligeDescrEnum.RPostingSvc Then
                rslt = False
            End If
            If Me.PrivBitMask And PriviligeDescrEnum.Respondent Then
                rslt = False
            End If
            Return rslt
        End Get
    End Property

    Public ReadOnly Property IsPerceptricsAdministrator() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.PerceptricsAdministrator
        End Get
    End Property

    Public ReadOnly Property IsLoginAdministrator() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.LoginAdministrator
        End Get
    End Property

    Public ReadOnly Property CanPublish() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.PublishSurvey
        End Get
    End Property

    Public ReadOnly Property CanCreate() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.Create
        End Get
    End Property

    Public ReadOnly Property CanReadAny() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.ReadAny
        End Get
    End Property

    Public ReadOnly Property CanWriteAny() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.WriteAny
        End Get
    End Property

    Public ReadOnly Property CanDeleteAny() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.DeleteAny
        End Get
    End Property


    Private _PrivBitMask As ULong
    Public Property PrivBitMask() As ULong
        Get
            Return _PrivBitMask
        End Get
        Set(ByVal value As ULong)
            _PrivBitMask = value
        End Set
    End Property

    Private _SubscrInfo As SubscriberInfo
    Public Property SubscrInfo() As SubscriberInfo
        Get
            Return _SubscrInfo
        End Get
        Set(ByVal value As SubscriberInfo)
            _SubscrInfo = value
        End Set
    End Property

    Private _GSM_LoginID As Integer
    Public Property GSM_LoginID() As Integer
        Get
            Return _GSM_LoginID
        End Get
        Set(ByVal value As Integer)
            _GSM_LoginID = value
        End Set
    End Property

    Private _NormalizedEmailAddress As String
    Public Property NormalizedEmailAddress() As String
        Get
            Return _NormalizedEmailAddress
        End Get
        Set(ByVal value As String)
            _NormalizedEmailAddress = value
        End Set
    End Property

    Private _SSM_LoginID As Integer
    Public Property SSM_LoginID() As Integer
        Get
            Return _SSM_LoginID
        End Get
        Set(ByVal value As Integer)
            _SSM_LoginID = value
        End Set
    End Property

    Private _IsOrginatingSubscriber As Boolean
    Public Property IsOriginatingSubscriber() As Boolean
        Get
            Return _IsOrginatingSubscriber
        End Get
        Set(ByVal value As Boolean)
            _IsOrginatingSubscriber = value
        End Set
    End Property

    Private _SurveyPrivileges As New List(Of SurveyPrivilegeModel)
    Public Property SurveyPrivileges() As List(Of SurveyPrivilegeModel)
        Get
            Return _SurveyPrivileges
        End Get
        Set(ByVal value As List(Of SurveyPrivilegeModel))
            _SurveyPrivileges = value
        End Set
    End Property
End Class

#Region "SubscriberInfo"
Public Class SubscriberInfo
    Public Sub New()

    End Sub

#Region "GuestLoginInfoLists Is/Can"
    Public ReadOnly Property SubscriberandAddedGuest_LoginInfo_List() As List(Of GuestLoginInfo)
        Get
            Dim rslt As List(Of GuestLoginInfo) = Nothing
            If Me.Guest_LoginInfo_List IsNot Nothing Then
                rslt = (From gli In Me.Guest_LoginInfo_List _
                       Where gli.IsSubscriber_OR_IsAddedGuest _
                       Select gli).ToList
            End If
            Return rslt
        End Get
    End Property

    Public ReadOnly Property CanReadAny_LoginInfo_List() As List(Of GuestLoginInfo)
        Get
            Dim rslt As List(Of GuestLoginInfo) = Nothing
            If Me.Guest_LoginInfo_List IsNot Nothing Then
                rslt = (From gli In Me.Guest_LoginInfo_List _
                       Where gli.PrivBitMask And PriviligeDescrEnum.ReadAny _
                       Select gli).ToList
            End If
            Return rslt
        End Get
    End Property
    Public ReadOnly Property CanWriteAny_LoginInfo_List() As List(Of GuestLoginInfo)
        Get
            Dim rslt As List(Of GuestLoginInfo) = Nothing
            If Me.Guest_LoginInfo_List IsNot Nothing Then
                rslt = (From gli In Me.Guest_LoginInfo_List _
                       Where gli.PrivBitMask And PriviligeDescrEnum.WriteAny _
                       Select gli).ToList
            End If
            Return rslt
        End Get
    End Property
    Public ReadOnly Property CanRsltsViewAny_LoginInfo_List() As List(Of GuestLoginInfo)
        Get
            Dim rslt As List(Of GuestLoginInfo) = Nothing
            If Me.Guest_LoginInfo_List IsNot Nothing Then
                rslt = (From gli In Me.Guest_LoginInfo_List _
                       Where gli.PrivBitMask And PriviligeDescrEnum.RsltsViewerAny _
                       Select gli).ToList
            End If
            Return rslt
        End Get
    End Property

    Public ReadOnly Property RdentLoginInfo_List() As List(Of GuestLoginInfo)
        Get
            Dim rslt As List(Of GuestLoginInfo) = Nothing
            If Me.Guest_LoginInfo_List IsNot Nothing Then
                rslt = (From gli In Me.Guest_LoginInfo_List, spm In gli.SurveyPrivileges _
                       Where spm.PrivBitMask And PriviligeDescrEnum.Respondent _
                       Select gli).ToList
            End If
            Return rslt
        End Get
    End Property
    Public ReadOnly Property PostingLoginInfo() As GuestLoginInfo
        Get
            Dim rslt As GuestLoginInfo = Nothing
            If Me.Guest_LoginInfo_List IsNot Nothing Then
                rslt = (From gli In Me.Guest_LoginInfo_List _
                        Where (From spm In gli.SurveyPrivileges _
                            Where spm.PrivBitMask And PriviligeDescrEnum.RDispatcherSvc AndAlso spm.PrivBitMask And PriviligeDescrEnum.RPostingSvc).Count > 0 _
                       Select gli).FirstOrDefault
            End If
            Return rslt
        End Get
    End Property

    ''' <summary>
    ''' this returns a list of guestlogininfos that have the privileges described by the _IncludedPrivEnumList parameter..
    ''' </summary>
    ''' <param name="_IncludedPrivEnumList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PrivEnumLoginInfo_List(ByVal _IncludedPrivEnumList As ULong) As List(Of GuestLoginInfo)
        Dim rslt As List(Of GuestLoginInfo) = Nothing
        If Me.Guest_LoginInfo_List IsNot Nothing Then
            rslt = (From gli In Me.Guest_LoginInfo_List _
                    Where gli.PrivBitMask And _IncludedPrivEnumList _
                   Select gli).ToList
        End If
        Return rslt
    End Function
#End Region
#Region "Properties"
    Private _QueueInfo As QueueInfo
    Public Property QueueInfo() As QueueInfo
        Get
            Return _QueueInfo
        End Get
        Set(ByVal value As QueueInfo)
            _QueueInfo = value
        End Set
    End Property

    Private _PostingLoginInfo_NormalizedEmailAddr As String
    Public ReadOnly Property PostingLoginInfo_NormalizedEmailAddr() As String
        Get
            Dim rslt = From gli In Me.Guest_LoginInfo_List _
                       Where gli.IsPostingLogin _
                       Select gli.NormalizedEmailAddress
            Return rslt.FirstOrDefault
            'Return SharedMethods.EmailAddress_ToNormalized(Me.SSM_CustomerName & "responses").ToLower
        End Get
    End Property


    Private _Guest_LoginInfo_List As List(Of GuestLoginInfo)
    Public Property Guest_LoginInfo_List() As List(Of GuestLoginInfo)
        Get
            Return _Guest_LoginInfo_List
        End Get
        Set(ByVal value As List(Of GuestLoginInfo))
            _Guest_LoginInfo_List = value
        End Set
    End Property


    Private _SSM_CustomerName As String
    Public Property SSM_CustomerName() As String
        Get
            Return _SSM_CustomerName
        End Get
        Set(ByVal value As String)
            _SSM_CustomerName = value
        End Set
    End Property

    Private _GSM_LoginID As Integer
    Public Property GSM_LoginID() As Integer
        Get
            Return _GSM_LoginID
        End Get
        Set(ByVal value As Integer)
            _GSM_LoginID = value
        End Set
    End Property

    Private _GSM_CustomerID As Integer
    Public Property GSM_CustomerID() As Integer
        Get
            Return _GSM_CustomerID
        End Get
        Set(ByVal value As Integer)
            _GSM_CustomerID = value
        End Set
    End Property

    Private _SSM_CnxnString As String
    Public Property SSM_CnxnString() As String
        Get
            Return _SSM_CnxnString
        End Get
        Set(ByVal value As String)
            _SSM_CnxnString = value
        End Set
    End Property

    Private _NormalizedEmailAddress As String
    Public Property NormalizedEmailAddress() As String
        Get
            Return _NormalizedEmailAddress
        End Get
        Set(ByVal value As String)
            _NormalizedEmailAddress = value
        End Set
    End Property

    Private _SSM_LoginID As Integer
    Public Property SSM_LoginID() As Integer
        Get
            Return _SSM_LoginID
        End Get
        Set(ByVal value As Integer)
            _SSM_LoginID = value
        End Set
    End Property

    Private _SSM_CustomerID As Integer
    Public Property SSM_CustomerID() As Integer
        Get
            Return _SSM_CustomerID
        End Get
        Set(ByVal value As Integer)
            _SSM_CustomerID = value
        End Set
    End Property

    Private _IsOrginatingSubscriber As Boolean
    Public Property IsOriginatingSubscriber() As Boolean
        Get
            Return _IsOrginatingSubscriber
        End Get
        Set(ByVal value As Boolean)
            _IsOrginatingSubscriber = value
        End Set
    End Property

    Private _SurveyPrivileges As New List(Of SurveyPrivilegeModel)
    Public Property SurveyPrivileges() As List(Of SurveyPrivilegeModel)
        Get
            Return _SurveyPrivileges
        End Get
        Set(ByVal value As List(Of SurveyPrivilegeModel))
            _SurveyPrivileges = value
        End Set
    End Property

    Private _TinySurveyRow_List As New List(Of Tiny_Survey_Row)
    Public Property TinySurveyRow_List() As List(Of Tiny_Survey_Row)
        Get
            Return _TinySurveyRow_List
        End Get
        Set(ByVal value As List(Of Tiny_Survey_Row))
            _TinySurveyRow_List = value
        End Set
    End Property
#End Region
End Class
#End Region

#Region "NewSubscriberPackage"
Public Class NewSubscriberPackage
    ''' <summary>
    ''' is new for this class
    ''' </summary>
    ''' <param name="_EmailAddress">Is Raw Email Address...gets Normalized within this class</param>
    ''' <param name="_SubcriptionLvl"></param>
    ''' <param name="_AuthID">CreditCardService e.g., Authorize.Net key to authorization data for this subscriber</param>
    ''' <param name="_SubScrName"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal _EmailAddress As String, ByVal _SubcriptionLvl As CmdInfrastructureNS.SubscriptionLevel, ByVal _AuthID As Integer, ByVal _SubScrName As String)
        Me._RawEmailAddress = _EmailAddress
        Me._NormalizedEmailAddress = SharedMethods.EmailAddress_ToNormalized(_EmailAddress)
        Me._SubscriptionLevel = _SubcriptionLvl
        Me._AuthorizationID = _AuthID
        Me._SubscriberName = _SubScrName
    End Sub

#Region "Properties"
    Public Property InitialPassword As Integer

    Private _ActiveLogin As ActiveLoginInfo
    Public Property ActiveLogin() As ActiveLoginInfo
        Get
            Return _ActiveLogin
        End Get
        Set(ByVal value As ActiveLoginInfo)
            _ActiveLogin = value
        End Set
    End Property

    Private _DefaultPrivilegeBitMask As Integer
    ''' <summary>
    ''' This comes from App.config key="DefaultPrivBMask_NewSubscriber"...is the Bitmask of PrivilegeDescrEnum for an new Subscriber account
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DefaultPrivilegeBitMask() As Integer
        Get
            Dim priv As Integer = 0
            Integer.TryParse(SharedMethods.GetValueFromAppConfig("DefaultPrivBMask_NewSubscriber"), priv)
            Return priv
        End Get
    End Property

    Private _RawEmailAddress As String = "NotSet"
    Public Property RawEmailAddress() As String
        Get
            Return _RawEmailAddress
        End Get
        Set(ByVal value As String)
            _RawEmailAddress = value
        End Set
    End Property

    Private _SubscriberName As String
    Public Property SubscriberName() As String
        Get
            Return _SubscriberName
        End Get
        Set(ByVal value As String)
            _SubscriberName = value
        End Set
    End Property

    Private _CreateDBInfo As CreateDatabaseInfo
    Public Property CreateDBInfo() As CreateDatabaseInfo
        Get
            Return _CreateDBInfo
        End Get
        Set(ByVal value As CreateDatabaseInfo)
            _CreateDBInfo = value
        End Set
    End Property

    Private _SubscrInfo As SubscriberInfo = Nothing
    Public Property SubscrInfo() As SubscriberInfo
        Get
            Return _SubscrInfo
        End Get
        Set(ByVal value As SubscriberInfo)
            _SubscrInfo = value
        End Set
    End Property

    Private _AuthorizationID As Integer
    Public Property AuthorizationID() As Integer
        Get
            Return _AuthorizationID
        End Get
        Set(ByVal value As Integer)
            _AuthorizationID = value
        End Set
    End Property

    Private _SubscriptionLevel As SubscriptionLevel = SubscriptionLevel.Basic
    Public Property SubscriptionLevel() As SubscriptionLevel
        Get
            Return _SubscriptionLevel
        End Get
        Set(ByVal value As SubscriptionLevel)
            _SubscriptionLevel = value
        End Set
    End Property

    Private _NormalizedEmailAddress As String = ""
    Public Property NormalizedEmailAddress() As String
        Get
            Return _NormalizedEmailAddress
        End Get
        Set(ByVal value As String)
            _NormalizedEmailAddress = value
        End Set
    End Property

    Private _RDentQueueURI As String = ""
    Public Property RDentQueueURI() As String
        Get
            Return _RDentQueueURI
        End Get
        Set(ByVal value As String)
            _RDentQueueURI = value
        End Set
    End Property

    Private _RDentQueueName As String = ""
    Public Property RDentQueueName() As String
        Get
            Return _RDentQueueName
        End Get
        Set(ByVal value As String)
            _RDentQueueName = value
        End Set
    End Property
#End Region
End Class
#End Region

#Region "NewLoginPackage"
Public Class NewLoginPackage

    Public Sub New(ByVal _EmailAddress As String)
        Me.RawEmailAddress = _EmailAddress
        Me._NormalizedEmailAddress = SharedMethods.EmailAddress_ToNormalized(_EmailAddress)
    End Sub
    Private _IsResponsePostingLogin As Boolean = False
    Public Property IsResponsePostingLogin() As Boolean
        Get
            Return _IsResponsePostingLogin
        End Get
        Set(ByVal value As Boolean)
            _IsResponsePostingLogin = value
        End Set
    End Property

    Private _IsRDentLogin As Boolean = False
    Public Property IsRDentLogin() As Boolean
        Get
            Return _IsRDentLogin
        End Get
        Set(ByVal value As Boolean)
            _IsRDentLogin = value
        End Set
    End Property

    Private _IsSubscriberAddedGuestLogin As Boolean = False
    Public Property IsSubscriberAddedGuestLogin() As Boolean
        Get
            Return _IsSubscriberAddedGuestLogin
        End Get
        Set(ByVal value As Boolean)
            _IsSubscriberAddedGuestLogin = value
        End Set
    End Property

#Region "Properties"
    Public Property RawEmailAddress As String
    Private _DefaultPrivilegeBitMask As Integer
    ''' <summary>
    ''' This comes from App.config key="DefaultPrivBMask_AddedLogin"...is the Bitmask of PrivilegeDescrEnum for an additional login on a Subscriber account
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DefaultPrivilegeBitMask() As ULong
        Get
            If Me.IsRDentLogin Then
                Return PriviligeDescrEnum.Respondent
            ElseIf Me.IsResponsePostingLogin Then
                Return PriviligeDescrEnum.RDispatcherSvc + PriviligeDescrEnum.RPostingSvc
            ElseIf Me.IsSubscriberAddedGuestLogin Then
                Dim priv As Integer = 0
                ULong.TryParse(SharedMethods.GetValueFromAppConfig("DefaultPrivBMask_AddedLogin"), priv)
                Return priv
            End If

        End Get
    End Property

    Private _LoginID_GSM As Integer
    Public Property LoginID_GSM() As Integer
        Get
            Return _LoginID_GSM
        End Get
        Set(ByVal value As Integer)
            _LoginID_GSM = value
        End Set
    End Property

    Private _NormalizedEmailAddress As String
    ''' <summary>
    ''' This is the emailaddress of the new LoginInfo( _EmailAddress in Sub New) that is being added to the Subscriber identifed in SubscriberInfo...
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NormalizedEmailAddress() As String
        Get
            Return _NormalizedEmailAddress
        End Get
        Set(ByVal value As String)
            _NormalizedEmailAddress = value
        End Set
    End Property

    Private _SubScriberInfo As SubscriberInfo
    ''' <summary>
    ''' This is the Subscriber that "IsAdding" the new LoginInfo identified in NormalizedEmailAddress
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SubScriberInfo() As SubscriberInfo
        Get
            Return _SubScriberInfo
        End Get
        Set(ByVal value As SubscriberInfo)
            _SubScriberInfo = value
        End Set
    End Property

    Private _SurveyPrivileges As New List(Of SurveyPrivilegeModel)
    ''' <summary>
    ''' Populate this list with Surveys and Privileges you want the new login to have...If you leave it empty, this login will have default Privileges for all Survey's.
    ''' Otherwise, Survey's in this list will be given the Privilege assigned in this list...
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SurveyPrivileges() As List(Of SurveyPrivilegeModel)
        Get
            Return _SurveyPrivileges
        End Get
        Set(ByVal value As List(Of SurveyPrivilegeModel))
            _SurveyPrivileges = value
        End Set
    End Property
#End Region
End Class
#End Region

#Region "NewSurveyPackage"
Public Class NewSurveyPackage

    Private _Survey_DC_List_Item As Srlzd_KVP
    Public Property Survey_DC_List_Item() As Srlzd_KVP
        Get
            Return _Survey_DC_List_Item
        End Get
        Set(ByVal value As Srlzd_KVP)
            _Survey_DC_List_Item = value
        End Set
    End Property

    Private _SurveyItemPkg As SurveyItem_Package
    Public Property SurveyItemPkg() As SurveyItem_Package
        Get
            Return _SurveyItemPkg
        End Get
        Set(ByVal value As SurveyItem_Package)
            _SurveyItemPkg = value
        End Set
    End Property

    Private _POCO_pkg As POCO_ID_Pkg
    Public Property POCO_Pkg() As POCO_ID_Pkg
        Get
            Return _POCO_pkg
        End Get
        Set(ByVal value As POCO_ID_Pkg)
            _POCO_pkg = value
        End Set
    End Property

    Private _SurveyItemModel As String
    Public Property SurveyItemModel() As String
        Get
            Return _SurveyItemModel
        End Get
        Set(ByVal value As String)
            _SurveyItemModel = value
        End Set
    End Property

    Private _SurveyDescription As String = "NotSet"
    Public Property SurveyDescription() As String
        Get
            Return _SurveyDescription
        End Get
        Set(ByVal value As String)
            _SurveyDescription = value
        End Set
    End Property

    Private _SubScriberInfo As SubscriberInfo
    Public Property SubScriberInfo() As SubscriberInfo
        Get
            Return _SubScriberInfo
        End Get
        Set(ByVal value As SubscriberInfo)
            _SubScriberInfo = value
        End Set
    End Property

End Class
#End Region


#Region "PublishSurveyPackage"
Public Class PublishSurveyPackage
    Public Sub New(ByVal _SubscriberNormalizedEmail As String, ByVal _SurveyIDToPublish As Integer, ByVal _NewDataStore As Boolean)
        Me._SubscrNormalizedEmail = _SubscriberNormalizedEmail
        Me._SurveyID = _SurveyIDToPublish
        Me.NewDataStoreDB = _NewDataStore
    End Sub

    Private _SDS_ResponseInfoList As List(Of SDS_ResponseInfo) = Nothing
    Public Property SDS_ResponseInfoList() As List(Of SDS_ResponseInfo)
        Get
            Return _SDS_ResponseInfoList
        End Get
        Set(ByVal value As List(Of SDS_ResponseInfo))
            _SDS_ResponseInfoList = value
        End Set
    End Property

    Private _PublishedSurveyLinkString As String = "NotSet"
    Public Property PublishedSurveyLinkString() As String
        Get
            Return _PublishedSurveyLinkString
        End Get
        Set(ByVal value As String)
            _PublishedSurveyLinkString = value
        End Set
    End Property

    Private _SubscrNormalizedEmail As String
    Public ReadOnly Property SubscriberNormalizedEmail() As String
        Get
            Return _SubscrNormalizedEmail
        End Get
    End Property

    Private _SurveyID As Integer = 0
    Public ReadOnly Property SurveyID() As Integer
        Get
            Return _SurveyID
        End Get
    End Property

    Private _SurveyState As Integer = -1
    Public Property SurveyState() As Integer
        Get
            If _SurveyState < 0 Then
                If Me.SubScriberInfo IsNot Nothing AndAlso Me.SubScriberInfo.SurveyPrivileges.Count > 0 Then
                    Dim svy = (From sv In Me.SubScriberInfo.SurveyPrivileges _
                              Where sv.SurveyID = Me._SurveyID _
                              Select sv).FirstOrDefault
                    If svy IsNot Nothing Then
                        _SurveyState = svy.SurveyStateID
                    End If
                End If
            End If
            Return _SurveyState
        End Get
        Set(ByVal value As Integer)
            _SurveyState = value
        End Set
    End Property

    Private _NewDataStoreDB As Boolean = False
    ''' <summary>
    ''' Indicates whether a new DataStore database should be created for this Survey when published...
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NewDataStoreDB() As Boolean
        Get
            Return _NewDataStoreDB
        End Get
        Set(ByVal value As Boolean)
            _NewDataStoreDB = value
        End Set
    End Property

    Private _SurveyQueueName As String = "NotSet"
    Public Property SurveyQueueName() As String
        Get
            If _SurveyQueueName = "NotSet" AndAlso Me.SubScriberInfo IsNot Nothing AndAlso Me.SurveyID > 0 Then
                _SurveyQueueName = SharedMethods.ToSurveyRDent_QueueName(Me.SubScriberInfo, Me.SurveyID)
            End If
            Return _SurveyQueueName
        End Get
        Set(ByVal value As String)
            _SurveyQueueName = value
        End Set
    End Property

    Private _SurveyQueueURI As String = "NotSet"
    Public Property SurveyQueueURI() As String
        Get
            If _SurveyQueueURI = "NotSet" AndAlso Me.SubScriberInfo IsNot Nothing AndAlso Me.SurveyID > 0 Then
                _SurveyQueueURI = SharedMethods.ToSurveyRDent_QueueURI(Me.SubScriberInfo, Me.SurveyID)
            End If
            Return _SurveyQueueURI
        End Get
        Set(ByVal value As String)
            _SurveyQueueURI = value
        End Set
    End Property

    Private _RDentLoginPkg As NewLoginPackage
    ''' <summary>
    ''' This is populated automatically by SubscriberOperations.PublishSurvey Method.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This LoginInfo will reside in the GSM LoginInfos table and also in the SSM LoginInfos Table</remarks>
    Public Property RDentLoginPkg() As NewLoginPackage
        Get
            Return _RDentLoginPkg
        End Get
        Set(ByVal value As NewLoginPackage)
            _RDentLoginPkg = value
        End Set
    End Property

    Private _SubScriberInfo As SubscriberInfo
    ''' <summary>
    ''' This is the Subscriber that "IsPublishing" the survey identified by _SurveyID...
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SubScriberInfo() As SubscriberInfo
        Get
            Return _SubScriberInfo
        End Get
        Set(ByVal value As SubscriberInfo)
            _SubScriberInfo = value
        End Set
    End Property

    Private _CreateDBInfo As CreateDatabaseInfo
    ''' <summary>
    ''' This contains info about database(s) created by SqlServerOperations Methods, e.g., CreateNewSurveyDataStore,...this is the result of that method...
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CreateDBInfo() As CreateDatabaseInfo
        Get
            Return _CreateDBInfo
        End Get
        Set(ByVal value As CreateDatabaseInfo)
            _CreateDBInfo = value
        End Set
    End Property
End Class
#End Region

#Region "ChangeSurveyStatePackage"
<DataContract()> _
Public Class ChangeSurveyStatePackage

    Private _SurveyGuid As Guid
    <DataMember()> _
    Public Property SurveyGuid() As Guid
        Get
            Return _SurveyGuid
        End Get
        Set(ByVal value As Guid)
            _SurveyGuid = value
        End Set
    End Property
    'Private _SDS_ResponseInfoList As List(Of SDS_ResponseInfo) = Nothing
    '<DataMember()> _
    'Public Property SDS_ResponseInfoList() As List(Of SDS_ResponseInfo)
    '    Get
    '        Return _SDS_ResponseInfoList
    '    End Get
    '    Set(ByVal value As List(Of SDS_ResponseInfo))
    '        _SDS_ResponseInfoList = value
    '    End Set
    'End Property

    Private _SurveyID As Integer
    <DataMember()> _
    Public Property SurveyID() As Integer
        Get
            Return _SurveyID
        End Get
        Set(ByVal value As Integer)
            _SurveyID = value
        End Set
    End Property

    Private _CurrentState As SurveyState = SurveyState.AuthorDesign
    <DataMember()> _
    Public Property CurrentState() As SurveyState
        Get
            Return _CurrentState
        End Get
        Set(ByVal value As SurveyState)
            _CurrentState = value
        End Set
    End Property

    Private _TargetState As SurveyState = SurveyState.AuthorDesign
    <DataMember()> _
    Public Property TargetState() As SurveyState
        Get
            Return _TargetState
        End Get
        Set(ByVal value As SurveyState)
            _TargetState = value
        End Set
    End Property
End Class
#End Region

#Region "SDS_ResponseInfo"
<DataContract()> _
Public Class SDS_ResponseInfo
#Region "Basic Properties"
    Private _ID As String
    <DataMember()> _
    Public Property ID() As String
        Get
            Return _ID
        End Get
        Set(ByVal value As String)
            _ID = value
        End Set
    End Property

    Private _QuestID As String
    <DataMember()> _
    Public Property QuestID() As String
        Get
            Return _QuestID
        End Get
        Set(ByVal value As String)
            _QuestID = value
        End Set
    End Property

    Private _Key1 As String
    <DataMember()> _
    Public Property Key1() As String
        Get
            Return _Key1
        End Get
        Set(ByVal value As String)
            _Key1 = value
        End Set
    End Property

    Private _Key2 As String
    <DataMember()> _
    Public Property Key2() As String
        Get
            Return _Key2
        End Get
        Set(ByVal value As String)
            _Key2 = value
        End Set
    End Property

    Private _Key3 As String
    <DataMember()> _
    Public Property Key3() As String
        Get
            Return _Key3
        End Get
        Set(ByVal value As String)
            _Key3 = value
        End Set
    End Property
#End Region
End Class
#End Region

#Region "GuestLoginInfo"
<DataContract()> _
Public Class GuestLoginInfo
    Implements INotifyPropertyChanged
#Region "PropertyChanged"
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
    End Sub
#End Region

    Public Sub RaisePropertyChanged(ByVal _PropName As String) 'this is used in testing on loginwindow lsp maintenance...not necessary otherwise..
        My_OnPropertyChanged(_PropName)
    End Sub

#Region "LoginLevel PrivBitMask Is/Can Properties"
    Public ReadOnly Property IsSubscriber_OR_IsAddedGuest() As Boolean
        Get
            Dim rslt As Boolean = True
            If Me.PrivBitMask And PriviligeDescrEnum.PerceptricsAdministrator Then
                rslt = False
            End If
            If Me.PrivBitMask And PriviligeDescrEnum.RDispatcherSvc Then
                rslt = False
            End If
            If Me.PrivBitMask And PriviligeDescrEnum.RPostingSvc Then
                rslt = False
            End If
            If Me.PrivBitMask And PriviligeDescrEnum.Respondent Then
                rslt = False
            End If
            Return rslt
        End Get
    End Property

    Public ReadOnly Property IsPostingLogin() As Boolean
        Get
            Dim rslt As Boolean = False
            If Me.PrivBitMask And PriviligeDescrEnum.RPostingSvc Then
                rslt = True
            End If
            Return rslt
        End Get
    End Property

    Public ReadOnly Property IsLoginAdministrator() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.LoginAdministrator
        End Get
    End Property

    Public ReadOnly Property CanPublish() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.PublishSurvey
        End Get
    End Property

    Public ReadOnly Property CanCreate() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.Create
        End Get
    End Property

    Public ReadOnly Property CanReadAny() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.ReadAny
        End Get
    End Property

    Public ReadOnly Property CanWriteAny() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.WriteAny
        End Get
    End Property

    Public ReadOnly Property CanDeleteAny() As Boolean
        Get
            Return Me.PrivBitMask And PriviligeDescrEnum.DeleteAny
        End Get
    End Property

#End Region

#Region "Properties"
    Private _RawEmailString As String
    Public Property RawEmailString() As String
        Get
            Return _RawEmailString
        End Get
        Set(ByVal value As String)
            _RawEmailString = value
        End Set
    End Property

    Private _PrivBitMask As ULong
    <DataMember()> _
    Public Property PrivBitMask() As ULong
        Get
            Return _PrivBitMask
        End Get
        Set(ByVal value As ULong)
            _PrivBitMask = value
        End Set
    End Property

    Private _SSM_CustomerName As String
    Public Property SSM_CustomerName() As String
        Get
            Return _SSM_CustomerName
        End Get
        Set(ByVal value As String)
            _SSM_CustomerName = value
        End Set
    End Property

    Private _GSM_LoginID As Integer
    Public Property GSM_LoginID() As Integer
        Get
            Return _GSM_LoginID
        End Get
        Set(ByVal value As Integer)
            _GSM_LoginID = value
        End Set
    End Property

    Private _GSM_CustomerID As Integer
    Public Property GSM_CustomerID() As Integer
        Get
            Return _GSM_CustomerID
        End Get
        Set(ByVal value As Integer)
            _GSM_CustomerID = value
        End Set
    End Property

    Private _SSM_CnxnString As String
    Public Property SSM_CnxnString() As String
        Get
            Return _SSM_CnxnString
        End Get
        Set(ByVal value As String)
            _SSM_CnxnString = value
        End Set
    End Property

    Private _NormalizedEmailAddress As String
    <DataMember()> _
    Public Property NormalizedEmailAddress() As String
        Get
            Return _NormalizedEmailAddress
        End Get
        Set(ByVal value As String)
            _NormalizedEmailAddress = value
        End Set
    End Property

    Private _SSM_LoginID As Integer
    <DataMember()> _
    Public Property SSM_LoginID() As Integer
        Get
            Return _SSM_LoginID
        End Get
        Set(ByVal value As Integer)
            _SSM_LoginID = value
        End Set
    End Property

    Private _SSM_CustomerID As Integer
    Public Property SSM_CustomerID() As Integer
        Get
            Return _SSM_CustomerID
        End Get
        Set(ByVal value As Integer)
            _SSM_CustomerID = value
        End Set
    End Property

    Private _SurveyPrivileges As New List(Of SurveyPrivilegeModel)
    <DataMember()> _
    Public Property SurveyPrivileges() As List(Of SurveyPrivilegeModel)
        Get
            Return _SurveyPrivileges
        End Get
        Set(ByVal value As List(Of SurveyPrivilegeModel))
            _SurveyPrivileges = value
        End Set
    End Property
#End Region
End Class
#End Region

#Region "SurveyPrivilegeModel"
<DataContract()> _
Public Class SurveyPrivilegeModel
    Implements INotifyPropertyChanged
#Region "PropertyChanged"
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
    End Sub
#End Region

#Region "Properties"
    Private _GSM_LoginID As Integer
    Public Property GSM_LoginID() As Integer
        Get
            Return _GSM_LoginID
        End Get
        Set(ByVal value As Integer)
            _GSM_LoginID = value
        End Set
    End Property

    Private _SurveyType As Integer
    <DataMember()> _
    Public Property SurveyType() As Integer
        Get
            Return _SurveyType
        End Get
        Set(ByVal value As Integer)
            _SurveyType = value
        End Set
    End Property

    Private _SurveyStateID As Integer
    <DataMember()> _
    Public Property SurveyStateID() As Integer
        Get
            Return _SurveyStateID
        End Get
        Set(ByVal value As Integer)
            _SurveyStateID = value
        End Set
    End Property

    Private _LoginSurveyPrivID As Integer
    <DataMember()> _
    Public Property LoginSurveyPrivID() As Integer
        Get
            Return _LoginSurveyPrivID
        End Get
        Set(ByVal value As Integer)
            _LoginSurveyPrivID = value
        End Set
    End Property

    Private _LoginID As Integer
    <DataMember()> _
    Public Property LoginID() As Integer
        Get
            Return _LoginID
        End Get
        Set(ByVal value As Integer)
            _LoginID = value
        End Set
    End Property

    Private _PrivBitMask As ULong = 0
    <DataMember()> _
    Public Property PrivBitMask() As ULong
        Get
            Return _PrivBitMask
        End Get
        Set(ByVal value As ULong)
            _PrivBitMask = value
            PrivEnumNameList = EnumBitMaskOperations.BmskToList(Of PriviligeDescrEnum)(PrivBitMask)
            PrivilegeEnumValueList = EnumBitMaskOperations.BmskToValues(Of PriviligeDescrEnum)(PrivBitMask)
        End Set
    End Property

    Private _PrivEnumList As List(Of PriviligeDescrEnum)
    Public Property PrivEnum() As List(Of PriviligeDescrEnum)
        Get
            Return _PrivEnumList
        End Get
        Set(ByVal value As List(Of PriviligeDescrEnum))
            _PrivEnumList = value
        End Set
    End Property

    Private _PrivEnumNameList As New List(Of String)
    <DataMember()> _
    Public Property PrivEnumNameList() As List(Of String)
        Get
            Return _PrivEnumNameList
        End Get
        Set(ByVal value As List(Of String))
            _PrivEnumNameList = value
            My_OnPropertyChanged("PrivEnumNameList")
        End Set
    End Property

    Private _PrivilegeEnumValueList As New List(Of ULong)
    <DataMember()> _
    Public Property PrivilegeEnumValueList() As List(Of ULong)
        Get
            Return _PrivilegeEnumValueList
        End Get
        Set(ByVal value As List(Of ULong))
            _PrivilegeEnumValueList = value
        End Set
    End Property

    Private _SurveyDescription As String
    <DataMember()> _
    Public Property SurveyDescription() As String
        Get
            Return _SurveyDescription
        End Get
        Set(ByVal value As String)
            _SurveyDescription = value
        End Set
    End Property

    Private _SurveyID As Integer
    <DataMember()> _
    Public Property SurveyID() As Integer
        Get
            Return _SurveyID
        End Get
        Set(ByVal value As Integer)
            _SurveyID = value
        End Set
    End Property
#End Region
End Class
#End Region

#Region "CreateDatabaseInfo"
Public Class CreateDatabaseInfo
    Public Sub New(ByVal _IsScss As Boolean, ByVal _NrmlzdEmailAddress As String, ByVal _Destination_SurveyMstrDatabaseName As String, _
                   ByVal _Destination_DtStoreDatabaseName As String, ByVal _DestinationSrvrName As String, _
                   ByVal _Destination_SurveyMstr_CnxnString As String, ByVal _Destination_DtStore_CnxnString As String)
        Me._IsSuccess = _IsScss
        Me._NormalizedEmailAddress = _NrmlzdEmailAddress
        Me._Destination_SurveyMasterDatabaseName = _Destination_SurveyMstrDatabaseName
        Me._Destination_DataStoreDatabaseName = _Destination_DtStoreDatabaseName
        Me._DestinationServerName = _DestinationSrvrName
        Me._Destination_SurveyMaster_CnxnString = _Destination_SurveyMstr_CnxnString
        Me._Destination_DataStore_CnxnString = _Destination_DtStore_CnxnString
    End Sub

    Private _IsSuccess As Boolean = False
    Public ReadOnly Property IsSuccess()
        Get
            Return _IsSuccess
        End Get
    End Property

    Private _NormalizedEmailAddress As String = Nothing
    Public ReadOnly Property NormalizedEmailAddress() As String
        Get
            Return _NormalizedEmailAddress
        End Get
    End Property

    Private _Destination_SurveyMasterDatabaseName As String
    Public ReadOnly Property Destination_SurveyMasterDatabaseName() As String
        Get
            Return _Destination_SurveyMasterDatabaseName
        End Get
    End Property

    Private _Destination_DataStoreDatabaseName As String
    Public Property Destination_DataStoreDatabaseName() As String
        Get
            Return _Destination_DataStoreDatabaseName
        End Get
        Set(ByVal value As String)
            _Destination_DataStoreDatabaseName = value
        End Set
    End Property

    Private _DestinationServerName As String
    Public ReadOnly Property DestinationServerName() As String
        Get
            Return _DestinationServerName
        End Get
    End Property

    Private _Destination_SurveyMaster_CnxnString As String
    Public ReadOnly Property Destination_SurveyMaster_CnxnString() As String
        Get
            Return _Destination_SurveyMaster_CnxnString
        End Get
    End Property

    Private _Destination_DataStore_CnxnString As String
    Public ReadOnly Property Destination_DataStore_CnxnString() As String
        Get
            Return _Destination_DataStore_CnxnString
        End Get
    End Property
End Class
#End Region

Public Class QueueInfo
    Private _QueueName As String
    Public Property QueueName() As String
        Get
            Return _QueueName
        End Get
        Set(ByVal value As String)
            _QueueName = value
        End Set
    End Property

    Private _CnxnString As String
    Public Property CnxnString() As String
        Get
            Return _CnxnString
        End Get
        Set(ByVal value As String)
            _CnxnString = value
        End Set
    End Property

    Private _AbsolutePath As String
    Public Property AbsolutePath() As String
        Get
            Return _AbsolutePath
        End Get
        Set(ByVal value As String)
            _AbsolutePath = value
        End Set
    End Property
End Class