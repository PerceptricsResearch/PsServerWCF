Imports System.Runtime.Serialization

Public Class SendEmail_Package
    <DataMember()> _
    Public Property InitiatingNormalizedEmailAddr As String
    <DataMember()> _
    Public Property ToAddress_Normalized As String
    <DataMember()> _
    Public Property EmailFormName As String
    <DataMember()> _
    Public Property MessageContentColxn As List(Of Srlzd_KVP)
End Class
