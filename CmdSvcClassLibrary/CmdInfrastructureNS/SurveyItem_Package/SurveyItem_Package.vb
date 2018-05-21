Imports System.Runtime.Serialization
<DataContract()> _
Public Class SurveyItem_Package
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
    Private _SIM As SurveyImagesPackage
    <DataMember()> _
    Public Property SIM() As SurveyImagesPackage
        Get
            Return _SIM
        End Get
        Set(ByVal value As SurveyImagesPackage)
            _SIM = value
        End Set
    End Property

    Private _SIM_SDSID As Integer
    <DataMember()> _
    Public Property SIM_SDSID() As Integer
        Get
            Return _SIM_SDSID
        End Get
        Set(ByVal value As Integer)
            _SIM_SDSID = value
        End Set
    End Property

    Private _SurveyName As String
    <DataMember()> _
    Public Property SurveyName() As String
        Get
            Return _SurveyName
        End Get
        Set(ByVal value As String)
            _SurveyName = value
        End Set
    End Property
End Class

<DataContract()> _
Public Class SurveyImagesPackage
    Private _SIM_SDSID As Integer
    <DataMember()> _
    Public Property SIM_SDSID() As Integer
        Get
            Return _SIM_SDSID
        End Get
        Set(ByVal value As Integer)
            _SIM_SDSID = value
        End Set
    End Property

    Private _ImageStorePkgList As List(Of ImageStorePackage)
    <DataMember()> _
    Public Property ImageStorePkgList() As List(Of ImageStorePackage)
        Get
            Return _ImageStorePkgList
        End Get
        Set(ByVal value As List(Of ImageStorePackage))
            _ImageStorePkgList = value
        End Set
    End Property
End Class

<DataContract()> _
Public Class ImageStorePackage
    Private _ImageID As Integer
    <DataMember()> _
    Public Property ImageID() As Integer
        Get
            Return _ImageID
        End Get
        Set(ByVal value As Integer)
            _ImageID = value
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

    Private _PCMID As Integer
    <DataMember()> _
    Public Property PCMID() As Integer
        Get
            Return _PCMID
        End Get
        Set(ByVal value As Integer)
            _PCMID = value
        End Set
    End Property

    Private _Height As Integer
    <DataMember()> _
    Public Property Height() As Integer
        Get
            Return _Height
        End Get
        Set(ByVal value As Integer)
            _Height = value
        End Set
    End Property

    Private _Width As Integer
    <DataMember()> _
    Public Property Width() As Integer
        Get
            Return _Width
        End Get
        Set(ByVal value As Integer)
            _Width = value
        End Set
    End Property

    Private _ImgFormat As Integer
    <DataMember()> _
    Public Property ImgFormat() As Integer
        Get
            Return _ImgFormat
        End Get
        Set(ByVal value As Integer)
            _ImgFormat = value
        End Set
    End Property

    Private _PermPCMGuidString As String
    <DataMember()> _
    Public Property PermPCMGuidString() As String
        Get
            Return _PermPCMGuidString
        End Get
        Set(ByVal value As String)
            _PermPCMGuidString = value
        End Set
    End Property

    Private _SeqNumber As Integer
    <DataMember()> _
    Public Property SeqNumber() As Integer
        Get
            Return _SeqNumber
        End Get
        Set(ByVal value As Integer)
            _SeqNumber = value
        End Set
    End Property

    Private _PCElemID As Integer
    <DataMember()> _
    Public Property PCElemID() As Integer
        Get
            Return _PCElemID
        End Get
        Set(ByVal value As Integer)
            _PCElemID = value
        End Set
    End Property

    Private _ByteArray As Byte()
    <DataMember()> _
    Public Property ByteArray() As Byte()
        Get
            Return _ByteArray
        End Get
        Set(ByVal value As Byte())
            _ByteArray = value
        End Set
    End Property
End Class