Imports System.Collections.ObjectModel
Imports System.Net.Mail
Imports System.Text


Public Class EmailPlatform

    Private MyEmFormsDBSvc As EmailFormsDBSvc


    Public Sub New()
        MyEmFormsDBSvc = New EmailFormsDBSvc
    End Sub

    'Const adminaddress As String = "admin@perceptricsresearch.com"
    'Const adminpwd As String = "Prllc6422"
    'Const mailclienthost As String = "secure.emailsrvr.com"

    'Public Sub Junk()
    '    Dim s = " <html><body><Table><tr><td>Hi,</td></tr><tr><td>Details of the Statistics :</td></tr></Table></body></html><html><body>" & "sometext" & _
    '        "</body></html><html><body><Table><tr><td> </td></tr><tr><td>NOTE: This is an automated mail. Please, do not reply.</td></tr>" & _
    '        "<tr><td>*Green coloured rows indicates temporary demos</td></tr>" & _
    '        "<tr><td>**All statistics are based on the page naming conventions Eg., 22_10_2005_</td></tr>" & _
    '        "<tr><td> </td></tr><tr><td>Regards,</td></tr><tr><td>some text,</td></tr><tr><td>some text,</td></tr>" & _
    '        "<tr><td> Some text </td></tr></table></body></html>"
    'End Sub

#Region "SendEmailPlease"
    Public Function SendEmailPlease(ByVal _SendEmailPkg As SendEmail_Package) As Boolean 'Implements IAuthoringSvc.SendEmailPlease
        Dim rslt As Boolean = False
        Try
            rslt = SendMailMessageAsynch(CreateMailfromFormName(_SendEmailPkg))

        Catch ex As Exception
            SharedEvents.RaiseOperationFailed("EmailPlatform", "EmailPlatform.SendMailPlease reports..." & ex.Message)
            ' MessageBox.Show("MailOperations.SendMail reports..." & ex.Message)
        End Try

        Return rslt
    End Function

    Private Function CreateMailfromFormName(ByVal _sendemailpkg As SendEmail_Package) As MailMessage
        Dim mail As MailMessage = Nothing
        If _sendemailpkg IsNot Nothing Then
            mail = New MailMessage() With {.IsBodyHtml = False, _
                                           .From = MyEmFormsDBSvc.DefaultFromAcct} '}New MailAddress(adminaddress)}
            mail.Bcc.Add(MyEmFormsDBSvc.DefaultFromAcct) 'New MailAddress(adminaddress))

            mail.To.Add(New MailAddress(_sendemailpkg.ToAddress_Normalized))

            mail.Subject = MyEmFormsDBSvc.SubjectLine(_sendemailpkg.EmailFormName)
            mail.Body = MyEmFormsDBSvc.MessageBody(_sendemailpkg.EmailFormName, _sendemailpkg.MessageContentColxn)

        End If
        Return mail
    End Function

    Private Function SendMailMessageAsynch(ByVal _mail As MailMessage) As Boolean
        Dim rslt As Boolean = False

        Try
            If Not IsNothing(_mail) Then
                Dim mailclient = New SmtpClient() With {.Host = MyEmFormsDBSvc.DefaultHostServer, _
                    .Port = MyEmFormsDBSvc.DefaultHostPort, _
                    .EnableSsl = MyEmFormsDBSvc.DefaultHostEnableSSl, _
                    .Credentials = New System.Net.NetworkCredential(MyEmFormsDBSvc.DefaultCredentialAcct, MyEmFormsDBSvc.DefaultCredentialPwd)}
                AddHandler mailclient.SendCompleted, AddressOf SendEmailCompleted
                mailclient.SendAsync(_mail, Nothing)
                rslt = True
            End If
        Catch ex As Exception
            SharedEvents.RaiseOperationFailed("EmailPlatform", "EmailPlatform.SendMailMessageAsnych reports..." & ex.Message)
        End Try
        Return rslt
    End Function
    Private Sub SendEmailCompleted(ByVal sender As Object, ByVal e As ComponentModel.AsyncCompletedEventArgs)
        If IsNothing(e.Error) Then
            CType(sender, SmtpClient).Dispose()
            'SharedEvents.RaiseOperationFailed("EmailPlatform", "EmailPlatform.SendEmailCompleted reports...")
            'MessageBox.Show("MailOperations.SendMail Completed...")
        Else
            SharedEvents.RaiseOperationFailed("EmailPlatform", "EmailPlatform.SendMailCompleted reports error..." & e.Error.Message)
            'MessageBox.Show("MailOperations.SendMailCompleted reports error..." & e.Error.Message)
            CType(sender, SmtpClient).Dispose()
        End If

    End Sub
#End Region

    Private ReadOnly Property MyDefaultEmailAcctInfoPoco() As EmailAcctInfoPOCO
        Get
            If MyEmFormsDBSvc IsNot Nothing Then
                Return MyEmFormsDBSvc.DefaultEmailAccount
            Else
                MyEmFormsDBSvc = New EmailFormsDBSvc
                Return MyEmFormsDBSvc.DefaultEmailAccount
            End If
        End Get
    End Property

    Public Sub SendFormPlease(ByVal _sendemailpkg As SendEmail_Package)

    End Sub
End Class



Public Enum EmailFormNames
    Comment = 0
    NewSubscriber = 1
    PublishedSurvey = 2
    UnPublishedSurvey = 3
    ChangedPassword = 4
    AddedGuestLogin = 5
    RemovedGuestLogin = 6
    CancelledSubscription = 7
    UpgradedSubscription = 8
    DownGradedSubscription = 9
    ConfirmPassword = 10
    EF11 = 11
    EF12 = 12
    EF13 = 13
    EF14 = 14
    EF15 = 15
End Enum