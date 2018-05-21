Imports System.Runtime.Serialization

<Serializable()> _
Public Class POCO_ID_Pkg

    Private _POCOGuid As Guid
    <DataMember()> _
    Public Property POCOGuid() As Guid
        Get
            Return _POCOGuid
        End Get
        Set(ByVal value As Guid)
            _POCOGuid = value
        End Set
    End Property

    Private _Original_ID As Integer
    <DataMember()> _
    Public Property Original_ID() As Integer
        Get
            Return _Original_ID
        End Get
        Set(ByVal value As Integer)
            _Original_ID = value
        End Set
    End Property

    Private _DB_ID As Integer
    <DataMember()> _
    Public Property DB_ID() As Integer
        Get
            Return _DB_ID
        End Get
        Set(ByVal value As Integer)
            _DB_ID = value
        End Set
    End Property

    Private _Survey_ID As Integer
    <DataMember()> _
    Public Property Survey_ID() As Integer
        Get
            Return _Survey_ID
        End Get
        Set(ByVal value As Integer)
            _Survey_ID = value
        End Set
    End Property
End Class
