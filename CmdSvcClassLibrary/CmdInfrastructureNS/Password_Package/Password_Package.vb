Imports System.Runtime.Serialization

Public Class Password_Package
    Private _LogIn_Email As String
    <DataMember()> _
    Public Property LogIn_Email() As String
        Get
            Return Me._LogIn_Email
        End Get
        Set(ByVal value As String)
            Me._LogIn_Email = value
        End Set
    End Property

    Private _PasswordHashINT As Integer
    <DataMember()> _
    Public Property PasswordHashINT() As Integer
        Get
            Return Me._PasswordHashINT
        End Get
        Set(ByVal value As Integer)
            Me._PasswordHashINT = value
        End Set
    End Property

    Private _PasswordHash As HashSet(Of String)
    <DataMember()> _
    Public Property PasswordHash() As HashSet(Of String)
        Get
            Return Me._PasswordHash
        End Get
        Set(ByVal value As HashSet(Of String))
            Me._PasswordHash = value
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

    Private _ImagePkgList As List(Of ImageStorePackage)
    <DataMember()> _
    Public Property ImagePkg() As List(Of ImageStorePackage)
        Get
            Return _ImagePkgList
        End Get
        Set(ByVal value As List(Of ImageStorePackage))
            _ImagePkgList = value
        End Set
    End Property
End Class
