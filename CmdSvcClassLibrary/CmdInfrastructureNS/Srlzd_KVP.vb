Imports System.Runtime.Serialization

<DataContract()> _
Public Class Srlzd_KVP
    Public Sub New()

    End Sub

    Public Sub New(ByVal _KeyString, ByVal _ValueString)
        Me._Key = _KeyString
        Me._Valu = _ValueString
    End Sub

    Private _Key As String = Nothing
    <DataMember()> _
    Public Property Key() As String
        Get
            Return Me._Key
        End Get
        Set(ByVal value As String)
            Me._Key = value
        End Set
    End Property

    Private _Valu As String = Nothing
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
