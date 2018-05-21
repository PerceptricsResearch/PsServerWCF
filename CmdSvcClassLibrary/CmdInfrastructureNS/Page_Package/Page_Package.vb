Imports System.Runtime.Serialization

<DataContract()> _
Public Class Page_Package
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

    Private _PgItemModelPkg As PgItemModel_Package
    <DataMember()> _
    Public Property PgItemModelPkg() As PgItemModel_Package
        Get
            Return _PgItemModelPkg
        End Get
        Set(ByVal value As PgItemModel_Package)
            _PgItemModelPkg = value
        End Set
    End Property

    Private _PgContentModelPkg As PgContentModel_Package
    <DataMember()> _
    Public Property PgContentModelPkg() As PgContentModel_Package
        Get
            Return _PgContentModelPkg
        End Get
        Set(ByVal value As PgContentModel_Package)
            _PgContentModelPkg = value
        End Set
    End Property

    Private _PCElement_Pkg_Colxn As List(Of PCElement_Package)
    <DataMember()> _
    Public Property PCElement_Pkg_Colxn() As List(Of PCElement_Package)
        Get
            Return _PCElement_Pkg_Colxn
        End Get
        Set(ByVal value As List(Of PCElement_Package))
            _PCElement_Pkg_Colxn = value
        End Set
    End Property

    <DataMember()> _
    Public Property SurveyPagesCount As Integer
End Class


<Serializable()> _
Public Class PgItemModel_Package

    Private _MyGuid As Guid
    <DataMember()> _
    Public Property MyGuid() As Guid
        Get
            Return _MyGuid
        End Get
        Set(ByVal value As Guid)
            _MyGuid = value
        End Set
    End Property

    Private _PageNumber As Integer
    <DataMember()> _
    Public Property PageNumber() As Integer
        Get
            Return _PageNumber
        End Get
        Set(ByVal value As Integer)
            _PageNumber = value
        End Set
    End Property

    Private _PIM As String
    <DataMember()> _
    Public Property PIM() As String
        Get
            Return _PIM
        End Get
        Set(ByVal value As String)
            _PIM = value
        End Set
    End Property

    Private _PIM_SDSID As Integer
    <DataMember()> _
    Public Property PIM_SDSID() As Integer
        Get
            Return _PIM_SDSID
        End Get
        Set(ByVal value As Integer)
            _PIM_SDSID = value
        End Set
    End Property
End Class

<Serializable()> _
Public Class PgContentModel_Package
    Private _MyGuid As Guid
    <DataMember()> _
    Public Property MyGuid() As Guid
        Get
            Return _MyGuid
        End Get
        Set(ByVal value As Guid)
            _MyGuid = value
        End Set
    End Property

    Private _PCM As String
    <DataMember()> _
    Public Property PCM() As String
        Get
            Return _PCM
        End Get
        Set(ByVal value As String)
            _PCM = value
        End Set
    End Property

    Private _PCM_SDSID As Integer
    <DataMember()> _
    Public Property PCM_SDSID() As Integer
        Get
            Return _PCM_SDSID
        End Get
        Set(ByVal value As Integer)
            _PCM_SDSID = value
        End Set
    End Property
End Class

<Serializable()> _
Public Class PCElement_Package

    Private _MyGuid As Guid
    <DataMember()> _
    Public Property MyGuid() As Guid
        Get
            Return _MyGuid
        End Get
        Set(ByVal value As Guid)
            _MyGuid = value
        End Set
    End Property

    Private _PCE As String
    <DataMember()> _
    Public Property PCE() As String
        Get
            Return _PCE
        End Get
        Set(ByVal value As String)
            _PCE = value
        End Set
    End Property

    Private _PCE_SDSID As Integer
    <DataMember()> _
    Public Property PCE_SDSID() As Integer
        Get
            Return _PCE_SDSID
        End Get
        Set(ByVal value As Integer)
            _PCE_SDSID = value
        End Set
    End Property

    <DataMember()> _
    Public Property HasImage As Boolean
End Class