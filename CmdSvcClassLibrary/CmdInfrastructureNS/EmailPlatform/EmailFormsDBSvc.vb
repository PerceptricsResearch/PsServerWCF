Imports System.Net
Imports System.Net.Mail
Imports System.Configuration
Imports System.Text

Public Class EmailFormsDBSvc

    Public DC_ConnectionString As String
#Region "Private Fields"
    Private WithEvents MyDB As L2S_PerceptricsEmailDataContext
    Private MyDBAvailable As Boolean = False
    Public OperationFailedName As String = Nothing
    Public MyException As Exception = Nothing
    Private MyDB_TTL As Integer = 0 'not implemented '''would keep the MyDB datacontext around for some time...if past the time, dispose, myDB then create New l2s_DBcontext
#End Region
#Region "Custom Events"
    Public Shared Event GSM_DB_UnavailableEvent(ByVal sender As Object, ByVal e As EventArgs)
    Public Shared Event GSM_DB_OperationFailed(ByVal sender As Object, ByVal e As EventArgs)
#End Region


    Public Sub New()
        Try
            Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            Dim cnxnstring = config.ConnectionStrings.ConnectionStrings("PerceptricsEmailConnectionString").ToString
            Me.DC_ConnectionString = cnxnstring
            Me.MyDB = New L2S_PerceptricsEmailDataContext(cnxnstring)
            MyDBAvailable = True
            If IsNothing(DefaultEmailAccount) Then
                Me.MyDBAvailable = False
            End If
        Catch ex As Exception
            RaiseEvent GSM_DB_UnavailableEvent(Me, New EventArgs)
        End Try
    End Sub

    Public Property DefaultHostServer As String

    Public Property DefaultHostPort As Integer

    Public Property DefaultHostEnableSSl As Boolean

    Public Property DefaultCredentialAcct As String

    Public Property DefaultCredentialPwd As String


    Public Function DefaultEmailAccount() As EmailAcctInfoPOCO
        Dim rslt As EmailAcctInfoPOCO = Nothing
        If MyDBAvailable Then
            Try
                rslt = MyDB.EmailAcctInfos.Where(Function(eai) eai.Name = "Default").Select(Function(r) New EmailAcctInfoPOCO With _
                                                                                            {.EmailAddress = r.EmailAddress, _
                                                                                             .EnableSSL = r.EnableSSL, _
                                                                                             .HostServer = r.HostServer, _
                                                                                             .Password = r.Password, _
                                                                                             .Port = r.Port, _
                                                                                             .ID = r.ID, _
                                                                                             .Name = r.Name}).FirstOrDefault
                If Not IsNothing(rslt) Then
                    DefaultFromAcct = New MailAddress(rslt.EmailAddress)
                    DefaultHostServer = rslt.HostServer
                    DefaultCredentialAcct = rslt.EmailAddress
                    DefaultCredentialPwd = rslt.Password
                    DefaultHostEnableSSl = rslt.EnableSSL
                    DefaultHostPort = rslt.Port
                End If
            Catch ex As Exception
                Me.OperationFailedName = "DefaultEmailAccount"
                Me.MyException = ex
                RaiseEvent GSM_DB_OperationFailed(Me, New EventArgs)
            End Try
        Else
            RaiseEvent GSM_DB_UnavailableEvent(Me, New EventArgs)
        End If


        Return rslt
    End Function

    Public Property DefaultFromAcct() As MailAddress

    Function SubjectLine(ByVal _formName As String) As String
        Dim rslt As String = ""
        Dim ef = MyDB.EmailForms.Where(Function(f) f.FormName = _formName).FirstOrDefault
        Dim subjsbldr As New StringBuilder
        For Each emi In MyDB.EmailFormMessageItems.Where(Function(mi) mi.KVPKey = "subj" AndAlso mi.FormID = ef.ID)
            subjsbldr.Append(emi.KVPValue)
        Next
        rslt = subjsbldr.ToString
        subjsbldr = Nothing
        Return rslt
    End Function

    Function MessageBody(ByVal _formName As String, ByVal _pkgMsgColxn As List(Of Srlzd_KVP)) As String
        Dim rslt As String = ""
        Dim ef = MyDB.EmailForms.Where(Function(f) f.FormName = _formName).FirstOrDefault
        Dim subjsbldr As New StringBuilder
        For Each emi In MyDB.EmailFormMessageItems.Where(Function(mi) mi.KVPKey = "msg" AndAlso mi.FormID = ef.ID)
            subjsbldr.AppendLine(emi.KVPValue)
        Next
        If Not IsNothing(_pkgMsgColxn) Then
            For Each item In _pkgMsgColxn.Where(Function(k) k.Key = "msg")
                subjsbldr.AppendLine(item.Valu)
            Next
        End If
        rslt = subjsbldr.ToString
        subjsbldr = Nothing
        Return rslt
    End Function

End Class

Public Class EmailAcctInfoPOCO
    Property ID As Integer

    Property Name As String

    Property EmailAddress As String

    Property HostServer As String

    Property Password As String

    Property EnableSSL As Boolean

    Property Port As Integer

End Class