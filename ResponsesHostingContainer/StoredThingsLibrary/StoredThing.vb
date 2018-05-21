Imports System.Runtime.Serialization

<DataContract()> _
Public Class StoredThing
    Private _TypeName As String
    <DataMember()> _
    Public Property TypeName() As String
        Get
            Return _TypeName
        End Get
        Set(ByVal value As String)
            _TypeName = value
        End Set
    End Property
    Private _AssemblyQualifiedName As String
    <DataMember()> _
    Public Property AssemblyQualifiedName() As String
        Get
            Return _AssemblyQualifiedName
        End Get
        Set(ByVal value As String)
            _AssemblyQualifiedName = value
        End Set
    End Property
    Private _StorageKey As String
    <DataMember()> _
    Public Property StorageKey() As String
        Get
            Return _StorageKey
        End Get
        Set(ByVal value As String)
            _StorageKey = value
        End Set
    End Property
    Private _StoredValue As Object
    <DataMember()> _
    Public Property StoredValue() As Object
        Get
            Return _StoredValue
        End Get
        Set(ByVal value As Object)
            _StoredValue = value
        End Set
    End Property
End Class
