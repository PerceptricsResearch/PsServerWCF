Imports System.Runtime.Serialization

<DataContract()> _
Public Class PageBlobInfo
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

    Private _PCE_Colxn As List(Of String)
    <DataMember()> _
    Public Property PCE_Colxn() As List(Of String)
        Get
            Return _PCE_Colxn
        End Get
        Set(ByVal value As List(Of String))
            _PCE_Colxn = value
        End Set
    End Property

End Class
