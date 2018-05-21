Imports System.Runtime.Serialization

<Serializable()> _
Public Class ServiceStartPackage
    Private _Name As String
    <DataMember()> _
    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Private _EndpointSuffix As String
    <DataMember()> _
    Public Property EndpointSuffix() As String
        Get
            Return _EndpointSuffix
        End Get
        Set(ByVal value As String)
            _EndpointSuffix = value
        End Set
    End Property

    Private _DataContextConnectionString As String
    <DataMember()> _
    Public Property DataContextConnectionString()
        Get
            Return _DataContextConnectionString
        End Get
        Set(ByVal value)
            _DataContextConnectionString = value
        End Set
    End Property

    Private _RestartonFault As Boolean
    <DataMember()> _
    Public Property RestartonFault() As Boolean
        Get
            Return _RestartonFault
        End Get
        Set(ByVal value As Boolean)
            _RestartonFault = value
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

    Private _SurveyPrivDCColxnPkg As CustomerDBOperationsNS.Survey_Priv_DC_Colxn_Pkg
    <DataMember()> _
    Public Property SurveyPrivDCColxnPkg() As CustomerDBOperationsNS.Survey_Priv_DC_Colxn_Pkg
        Get
            Return _SurveyPrivDCColxnPkg
        End Get
        Set(ByVal value As CustomerDBOperationsNS.Survey_Priv_DC_Colxn_Pkg)
            _SurveyPrivDCColxnPkg = value
        End Set
    End Property

    Private _ValidSvcsBmask As ULong
    <DataMember()> _
    Public Property ValidSvcsBmask() As ULong
        Get
            Return _ValidSvcsBmask
        End Get
        Set(ByVal value As ULong)
            _ValidSvcsBmask = value
        End Set
    End Property

    Private _SvcEnumBmask As ULong
    <DataMember()> _
    Public Property SvcEnumBmask() As ULong
        Get
            Return _SvcEnumBmask
        End Get
        Set(ByVal value As ULong)
            _SvcEnumBmask = value
        End Set
    End Property

    Private _PermittedSvcBmask As ULong
    <DataMember()> _
    Public Property PermittedSvcBmask() As ULong
        Get
            Return _PermittedSvcBmask
        End Get
        Set(ByVal value As ULong)
            _PermittedSvcBmask = value
        End Set
    End Property
End Class
