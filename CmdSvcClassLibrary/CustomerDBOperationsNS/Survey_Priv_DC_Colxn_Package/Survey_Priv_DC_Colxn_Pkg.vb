Imports System.Runtime.Serialization
Imports CmdInfrastructureNS

Public Class Survey_Priv_DC_Colxn_Pkg
    Private _ServiceEnumBitMsk As ULong = 0
    Public Property ServiceEnumBitMsk() As ULong
        Get
            Return _ServiceEnumBitMsk
        End Get
        Set(ByVal value As ULong)
            _ServiceEnumBitMsk = value
        End Set
    End Property
#Region "SurveyDCList and SurveyPrivilegeList Properties"
    Private _Survey_DC_List As New List(Of Srlzd_KVP)

    Public Property Survey_DC_List() As List(Of Srlzd_KVP)
        Get
            Return _Survey_DC_List
        End Get
        Set(ByVal value As List(Of Srlzd_KVP))
            _Survey_DC_List = value
        End Set
    End Property

    ''' <summary>
    ''' List of SurveyID and PrivilegeBitMask of PrivilegeDescrEnum
    ''' </summary>
    ''' <remarks></remarks>
    Private _Survey_Privilege_List As New List(Of Srlzd_KVP)

    Public Property Survey_Privilege_List() As List(Of Srlzd_KVP)
        Get
            Return _Survey_Privilege_List
        End Get
        Set(ByVal value As List(Of Srlzd_KVP))
            _Survey_Privilege_List = value
        End Set
    End Property
#End Region

    'Private _DC_Pkg As DC_Package
    'Public Property DC_Pkg() As DC_Package
    '    Get
    '        Return _DC_Pkg
    '    End Get
    '    Set(ByVal value As DC_Package)
    '        _DC_Pkg = value
    '    End Set
    'End Property
End Class
