Imports System.Runtime.Serialization

<DataContract()> _
Public Class CommandPackage
#Region "Methods and DC_Cnxn_IsValid Field"
    Public Shared Function ToText(ByVal _this As CommandPackage) As String
        Dim rslt As String
        If _this._DC_Cnxn IsNot Nothing Then
            rslt = _this._CmdVb.ToString & " " & _this.IssueDtTime & " EndPt= " & _this._EndPointSuffix & vbCrLf & "DC_Cnxn= " & _this._DC_Cnxn
        Else
            rslt = _this._CmdVb.ToString & " " & _this.IssueDtTime & " EndPt= " & _this._EndPointSuffix
        End If

        Return rslt
    End Function
#End Region


#Region "Properties"
    Private _CmdVb As CmdVerb
    <DataMember()> _
    Public Property CmdVb() As CmdVerb
        Get
            Return _CmdVb
        End Get
        Set(ByVal value As CmdVerb)
            _CmdVb = value
        End Set
    End Property

    Private _IssueDtTime As DateTime
    <DataMember()> _
    Public Property IssueDtTime() As DateTime
        Get
            Return _IssueDtTime
        End Get
        Set(ByVal value As DateTime)
            _IssueDtTime = value
        End Set
    End Property

    Private _EndPointSuffix As String
    <DataMember()> _
    Public Property EndPtSuffix() As String
        Get
            Return _EndPointSuffix
        End Get
        Set(ByVal value As String)
            _EndPointSuffix = value
        End Set
    End Property

    Private _DC_Cnxn As String = "IsNotSet"
    <DataMember()> _
    Public Property DC_Cnxn() As String
        Get
            Return _DC_Cnxn
        End Get
        Set(ByVal value As String)
            _DC_Cnxn = value

        End Set
    End Property

    'Private _DC_Pkg As DC_Package
    '<DataMember()> _
    'Public Property DC_Pkg() As DC_Package
    '    Get
    '        Return _DC_Pkg
    '    End Get
    '    Set(ByVal value As DC_Package)
    '        _DC_Pkg = value
    '    End Set
    'End Property

    ''' <summary>
    ''' The Svc in this Host is a Client of all the Services described in this Colxn...
    ''' Use this Colxn to find the BaseAddress of a Service, for which, you want to create a ClientProxy
    ''' It is populated by EndPtDataContextSvc in its IssueExposeEndPtsCommands operation.
    ''' This colxn is accessible by WCFServicID, Name, Contract....
    ''' </summary>
    ''' <remarks></remarks>
    Private _I_am_a_ClientOfThisServiceColxn As New List(Of ClientOFThisServiceInfo)
    <DataMember()> _
    Public Property ClientOfThisServiceColxn() As List(Of ClientOFThisServiceInfo)
        Get
            Return _I_am_a_ClientOfThisServiceColxn
        End Get
        Set(ByVal value As List(Of ClientOFThisServiceInfo))
            _I_am_a_ClientOfThisServiceColxn = value
        End Set
    End Property
#End Region

End Class
<DataContract()> _
Public Enum CmdVerb
    <EnumMember()> _
    ExposeEndPoint
    <EnumMember()> _
    RetractEndPoint
    <EnumMember()> _
    AddDataContext
    <EnumMember()> _
    RemoveDataContext
    <EnumMember()> _
    ExposeWithDataContext
End Enum

''' <summary>
''' This is a tiny WCFServiceInfo that represents a WCFService...so the CustomServiceHost "knows" what services it calls as a Client..
''' Use this to find the baseAddress of a Service at runtime without making a call to the GlobalSurveyMaster
''' EndPtDataContextSvc populates this in its IssueExposeEndpointCommands operation...
''' </summary>
''' <remarks></remarks>
<DataContract()> _
Public Class ClientOFThisServiceInfo
   
    Private _WCFServiceInfoID As Integer
   
    <DataMember()> _
    Public Property WCFServiceInfoID() As Integer
        Get
            Return _WCFServiceInfoID
        End Get
        Set(ByVal value As Integer)
            _WCFServiceInfoID = value
        End Set
    End Property

    Private _WCFServiceName As String
    <DataMember()> _
    Public Property WCFServiceName() As String
        Get
            Return _WCFServiceName
        End Get
        Set(ByVal value As String)
            _WCFServiceName = value
        End Set
    End Property

    Private _Contract As String
    <DataMember()> _
    Public Property Contract() As String
        Get
            Return _Contract
        End Get
        Set(ByVal value As String)
            _Contract = value
        End Set
    End Property

    Private _BaseAddress As String
    <DataMember()> _
    Public Property BaseAddress() As String
        Get
            Return _BaseAddress
        End Get
        Set(ByVal value As String)
            _BaseAddress = value
        End Set
    End Property

End Class