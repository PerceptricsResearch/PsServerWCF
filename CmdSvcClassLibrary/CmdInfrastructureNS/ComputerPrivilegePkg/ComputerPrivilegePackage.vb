Imports System.Runtime.Serialization

''' <summary>
''' List of ComputerID/PrivilegeID Pairs
''' </summary>
''' <remarks></remarks>
''' 
<DataContract()> _
Public Class ComputerPrivilegePackage
    Private _CompPrivilegeList As New List(Of CompPrivilegeItem)
    <DataMember()> _
    Public Property CompPrivilegeList() As List(Of CompPrivilegeItem)
        Get
            Return _CompPrivilegeList
        End Get
        Set(ByVal value As List(Of CompPrivilegeItem))
            _CompPrivilegeList = value
        End Set
    End Property

    Public Class CompPrivilegeItem
        Private _ComputerID As Integer
        <DataMember()> _
        Public Property ComputerID() As Integer
            Get
                Return _ComputerID
            End Get
            Set(ByVal value As Integer)
                _ComputerID = value
            End Set
        End Property

        Private _PrivilegeID As Integer
        <DataMember()> _
        Public Property PrivilegeID() As Integer
            Get
                Return _PrivilegeID
            End Get
            Set(ByVal value As Integer)
                _PrivilegeID = value
            End Set
        End Property
    End Class


End Class

