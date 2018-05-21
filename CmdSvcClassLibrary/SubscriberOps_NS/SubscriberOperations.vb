Imports MasterDBSvcLibr
Imports CmdInfrastructureNS

Public Class SubscriberOperations
    Implements IDisposable


    Public Event OperationFailed(ByVal sender As Object, ByVal e As EventArgs)
    Public OperationFailedName As String = Nothing
    Public MyException As Exception = Nothing

    Private Property MyGSMSvc As GlobalSurveyMasterDBSvc
    Public Sub New()
        Me.MyGSMSvc = New GlobalSurveyMasterDBSvc 'does not do a datacontext.cnxnstring in this example...
        AddHandler Me.OperationFailed, AddressOf OperationFailedHandler
        'AddHandler GlobalSurveyMasterDBSvc.GSM_DB_UnavailableEvent, AddressOf GlobalSurveyMasterDBSvcUnavailableHandler
        'AddHandler GlobalSurveyMasterDBSvc.GSM_DB_OperationFailed, AddressOf GlobalSurveyMasterDBSvcOperationFailedHandler
        'AddHandler SubscriberDBSvc.SSM_DB_UnavailableEvent, AddressOf SubscriberDBSvcUnavailableHandler
        'AddHandler SubscriberDBSvc.SSM_DB_OperationFailed, AddressOf SubscriberDBSvcOperationFailedHandler
    End Sub
    Private Sub OperationFailedHandler(sender As Object, e As EventArgs)
        Try
            Using EvLog As New EventLog()
                EvLog.Source = "SubscriberOperations"
                EvLog.Log = "Application"
                EvLog.WriteEntry(OperationFailedName & " " & MyException.Message, EventLogEntryType.Error)
            End Using
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "SubscriberOperations"
                EvLog.Log = "Application"
                EvLog.WriteEntry(OperationFailedName & " " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

    End Sub

#Region "GlobalSurveyMasterDBSvc DB Error Handlers"
    'Private Sub GlobalSurveyMasterDBSvcUnavailableHandler(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim sdbsvc = CType(sender, GlobalSurveyMasterDBSvc)
    '    'MessageBox.Show("SubscriberOperations reports GSMDBSvcUnvailable DC_CnxnString= <" & sdbsvc.DC_ConnectionString & ">")
    'End Sub

    'Private Sub GlobalSurveyMasterDBSvcOperationFailedHandler(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim sdbsvc = CType(sender, GlobalSurveyMasterDBSvc)
    '    'MessageBox.Show("SubscriberOperations reports GSMDBSvcOperationFailed .CnxnString=<" & sdbsvc.DC_ConnectionString & ">" & vbCr _
    '    '                & " Operation <" & sdbsvc.OperationFailedName & "> Exception <" & sdbsvc.MyException.Message & ">")
    'End Sub
#End Region

#Region "SubscriberDBSvc DB Error Handlers"
    'Private Sub SubscriberDBSvcUnavailableHandler(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim sdbsvc = CType(sender, SubscriberDBSvc)
    '    'MessageBox.Show("SubscriberOperations reports SubscriberDBSvcUnvailable SubscriberInfo.NormEmail= <" & sdbsvc.CurrentSubscriber.NormalizedEmailAddress & _
    '    '                "> .CnxnString=<" & sdbsvc.CurrentSubscriber.SSM_CnxnString & ">")
    'End Sub

    'Private Sub SubscriberDBSvcOperationFailedHandler(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim sdbsvc = CType(sender, SubscriberDBSvc)
    '    'MessageBox.Show("SubscriberOperations reports SubscriberDBSvcOperationFailed SubscriberInfo.NormEmail= <" & sdbsvc.CurrentSubscriber.NormalizedEmailAddress & _
    '    '                "> .CnxnString=<" & sdbsvc.CurrentSubscriber.SSM_CnxnString & ">" & vbCr _
    '    '                & " Operation <" & sdbsvc.OperationFailedName & "> Exception <" & sdbsvc.MyException.Message & ">")
    'End Sub
#End Region

#Region "Action Methods"
    Public Function AddNewSubscriber(ByVal _NewSubscriberPkg As NewSubscriberPackage) As Boolean
        Dim rslt As Boolean = False
        'If Me.ActiveLogin IsNot Nothing AndAlso Me.ActiveLogin.IsPerceptricsAdministrator Then
        If Not IsNothing(Me.MyGSMSvc) AndAlso Not IsNothing(_NewSubscriberPkg) Then
            Dim SSMSvc As SubscriberDBSvc = Nothing
            Try
                If MyGSMSvc.AddNewSubscriber(_NewSubscriberPkg) Then
                    SSMSvc = New SubscriberDBSvc(_NewSubscriberPkg.SubscrInfo)
                    rslt = SSMSvc.AddNewSubscriber(_NewSubscriberPkg)
                    'this adds a "responsesposting" logininfo...
                    Dim nlp As New NewLoginPackage(_NewSubscriberPkg.NormalizedEmailAddress & "responses")
                    nlp.IsResponsePostingLogin = True 'causes nlp.defaultprivbitmask to return a different privbitmask...
                    nlp.SubScriberInfo = _NewSubscriberPkg.SubscrInfo 'This could set PrivBitmask on LoginInfo table...doesn't need to..
                    AddNewLoginToExistingSubscriber(nlp)
                    SSMSvc.Dispose()
                    SSMSvc = Nothing
                    nlp = Nothing
                    'DisposeSSMDBSvc(SSMSvc)
                End If
            Catch ex As Exception
                Me.OperationFailedName = "AddNewSubscriber"
                Me.MyException = ex
                RaiseEvent OperationFailed("AddNewSubscriber", New EventArgs)
            Finally
                If Not IsNothing(SSMSvc) Then
                    SSMSvc.Dispose()
                    SSMSvc = Nothing
                End If
            End Try
        Else
            SharedEvents.RaiseOperationFailed("SubscriberOperations", "AddNewSubscriber - MyGSMSvc or _NewSubscriberPkg Is Nothing...")
        End If

        'Else
        'MessageBox.Show("ActiveLogin does not have required privileges for this function")
        'End If
        Return rslt
    End Function

    Public Function AddNewSurvey(ByRef _NewSurveyPkg As NewSurveyPackage) As Boolean
        Dim rslt As Boolean = False
        If Not IsNothing(_NewSurveyPkg) Then
            If Not IsNothing(Me.ActiveLogin) AndAlso Me.ActiveLogin.CanCreate Then
                _NewSurveyPkg.SubScriberInfo = Me.ActiveLogin.SubscrInfo
                Dim SSMSvc As SubscriberDBSvc = Nothing
                Try
                    SSMSvc = New SubscriberDBSvc(_NewSurveyPkg.SubScriberInfo)
                    If Not IsNothing(SSMSvc) Then
                        rslt = SSMSvc.AddNewSurvey(_NewSurveyPkg)
                        SSMSvc.Dispose()
                        SSMSvc = Nothing
                    End If
                    'DisposeSSMDBSvc(SSMSvc)
                Catch ex As Exception
                    Me.OperationFailedName = "AddNewSurvey"
                    Me.MyException = ex
                    RaiseEvent OperationFailed("AddNewSurvey", New EventArgs)
                Finally
                    If Not IsNothing(SSMSvc) Then
                        SSMSvc.Dispose()
                        SSMSvc = Nothing
                    End If
                End Try
            Else
                SharedEvents.RaiseOperationFailed("SubscriberOperations", "AddNewSurvey - Me.ActiveLogin Is Nothing or .CanCreate = False...")
                'MessageBox.Show("ActiveLogin does not have required privileges for this function")
            End If
        Else
            SharedEvents.RaiseOperationFailed("SubscriberOperations", "AddNewSurvey - _NewSurveyPkg Is Nothing...")
        End If

        Return rslt
    End Function

    Public Function PublishSurvey(ByRef _PublishSurveyPkg As PublishSurveyPackage) As Boolean
        Dim rslt As Boolean = False
        If Not IsNothing(_PublishSurveyPkg) Then
            If Me.ActiveLogin IsNot Nothing AndAlso Me.ActiveLogin.CanPublish Then
                Dim SSMSvc As SubscriberDBSvc = Nothing
                Try
                    _PublishSurveyPkg.SubScriberInfo = Me.ActiveLogin.SubscrInfo
                    SSMSvc = New SubscriberDBSvc(_PublishSurveyPkg.SubScriberInfo)
                    If SSMSvc IsNot Nothing Then
                        Dim svyid = _PublishSurveyPkg.SurveyID
                        Dim sm = SSMSvc.GetSurveyRows.Where(Function(sv) sv.SurveyID = svyid).FirstOrDefault
                        If sm IsNot Nothing Then
                            'only add RdentLoginInfo's if this survey has never been published...
                            If sm.SurveyStateID < 1 Then
                                'this is the RDentLoginInfo....added when a survey is published...should only happen when a survey is published for the first time...
                                'Dim RdentLoginString = _PublishSurveyPkg.SubScriberInfo.SSM_CustomerName & "RDent" & _PublishSurveyPkg.SurveyID.ToString
                                Dim RdentLoginString = Guid.NewGuid.ToString("N") & _PublishSurveyPkg.SurveyID.ToString
                                _PublishSurveyPkg.RDentLoginPkg = New NewLoginPackage(RdentLoginString)
                                _PublishSurveyPkg.RDentLoginPkg.IsRDentLogin = True
                                _PublishSurveyPkg.RDentLoginPkg.SubScriberInfo = _PublishSurveyPkg.SubScriberInfo
                                _PublishSurveyPkg.RDentLoginPkg.SurveyPrivileges.Add(New SurveyPrivilegeModel With {.SurveyID = _PublishSurveyPkg.SurveyID, _
                                                                                                                    .PrivBitMask = PriviligeDescrEnum.Respondent})
                                AddNewLoginToExistingSubscriber(_PublishSurveyPkg.RDentLoginPkg) 'this adds a loginInfo to GSM and SSM Db's
                            End If
                            rslt = SSMSvc.PublishSurvey(_PublishSurveyPkg)
                            sm = Nothing
                        End If
                        SSMSvc.Dispose()
                        SSMSvc = Nothing
                    End If
                    'DisposeSSMDBSvc(SSMSvc)
                Catch ex As Exception
                    Me.OperationFailedName = "PublishSurvey"
                    Me.MyException = ex
                    RaiseEvent OperationFailed("PublishSurvey", New EventArgs)
                Finally
                    If Not IsNothing(SSMSvc) Then
                        SSMSvc.Dispose()
                        SSMSvc = Nothing
                    End If
                End Try
            Else
                SharedEvents.RaiseOperationFailed("SubscriberOperations", "PublishSurvey - ActiveLogin does not have required privileges for this function")
                'MessageBox.Show("ActiveLogin does not have required privileges for this function")
            End If
        Else
            SharedEvents.RaiseOperationFailed("SubscriberOperations", "PublishSurvey - _PublishSurveyPkg Is Nothing...")
        End If

        Return rslt
    End Function

    Public Function UnPublishSurvey(ByVal _SurveyID As Integer) As Boolean
        Dim rslt As Boolean = False
        If Me.ActiveLogin IsNot Nothing AndAlso Me.ActiveLogin.CanPublish Then
            Dim SSMSvc As SubscriberDBSvc = Nothing
            Try
                SSMSvc = New SubscriberDBSvc(Me.ActiveLogin.SubscrInfo)
                If Not IsNothing(SSMSvc) Then
                    rslt = SSMSvc.UnPublishSurvey(_SurveyID)
                    SSMSvc.Dispose()
                    SSMSvc = Nothing
                End If
                'DisposeSSMDBSvc(SSMSvc)
            Catch ex As Exception
                Me.OperationFailedName = "UnPublishSurvey"
                Me.MyException = ex
                RaiseEvent OperationFailed("UnPublishSurvey", New EventArgs)
            Finally
                If Not IsNothing(SSMSvc) Then
                    SSMSvc.Dispose()
                    SSMSvc = Nothing
                End If
            End Try
        Else
            SharedEvents.RaiseOperationFailed("SubscriberOperations", "UnPublishSurvey - ActiveLogin does not have required privileges for this function")
        End If

        Return rslt

    End Function

    Public Function AddNewLoginToExistingSubscriber(ByVal _NewLoginPkg As NewLoginPackage) As Boolean
        Dim rslt As Boolean = False
        If Not IsNothing(_NewLoginPkg) Then
            If (Me.ActiveLogin IsNot Nothing AndAlso Me.ActiveLogin.IsLoginAdministrator) Or _NewLoginPkg.IsResponsePostingLogin Then
                '_NewLoginPkg.SubScriberInfo = Me.ActiveLogin.SubscrInfo
                Dim SSMSvc As SubscriberDBSvc = Nothing
                Try
                    If MyGSMSvc.AddNewLoginToExistingSubscriber(_NewLoginPkg) Then
                        SSMSvc = New SubscriberDBSvc(_NewLoginPkg.SubScriberInfo)
                        If SSMSvc IsNot Nothing Then
                            rslt = SSMSvc.AddNewLoginToExistingSubscriber(_NewLoginPkg)
                            SSMSvc.Dispose()
                            SSMSvc = Nothing
                        End If
                        'DisposeSSMDBSvc(SSMSvc)
                    End If
                Catch ex As Exception
                    Me.OperationFailedName = "AddNewLoginToExistingSubscriber"
                    Me.MyException = ex
                    RaiseEvent OperationFailed(Me, New EventArgs)
                Finally
                    If Not IsNothing(SSMSvc) Then
                        SSMSvc.Dispose()
                        SSMSvc = Nothing
                    End If
                End Try
            Else
                SharedEvents.RaiseOperationFailed("AddNewLoginToExistingSubscriber", " - ActiveLogin does not have required privileges for this function")
                'MessageBox.Show("ActiveLogin does not have required privileges for this function")
            End If
        Else
            SharedEvents.RaiseOperationFailed("AddNewLoginToExistingSubscriber", " _NewLoginPkg Is Nothing...")
        End If
       
        Return rslt
    End Function

    Public Function ModifySurveyLoginPrivileges(ByVal _SurveyPrivlgs As List(Of SurveyPrivilegeModel), ByVal _Subscriber As SubscriberInfo) As Boolean
        Dim rslt As Boolean = False
        If Not IsNothing(_SurveyPrivlgs) AndAlso Not IsNothing(_Subscriber) Then
            If Me.ActiveLogin IsNot Nothing AndAlso Me.ActiveLogin.IsLoginAdministrator Then
                Dim SSMSvc As SubscriberDBSvc = Nothing
                Try
                    SSMSvc = New SubscriberDBSvc(Me.ActiveLogin.SubscrInfo)
                    If SSMSvc IsNot Nothing Then
                        rslt = SSMSvc.ModifySurveyLoginPrivileges(_SurveyPrivlgs)
                        'TODO: THis should update the subscriber.guestlogininfo list....and surveyprivileges...?
                        SSMSvc.Dispose()
                        SSMSvc = Nothing
                    End If
                    'DisposeSSMDBSvc(SSMSvc)
                Catch ex As Exception
                    Me.OperationFailedName = "ModifySurveyLoginPrivileges"
                    Me.MyException = ex
                    RaiseEvent OperationFailed(Me, New EventArgs)
                Finally
                    If Not IsNothing(SSMSvc) Then
                        SSMSvc.Dispose()
                        SSMSvc = Nothing
                    End If
                End Try
            Else
                SharedEvents.RaiseOperationFailed("ModifySurveyLoginPrivileges", " - ActiveLogin does not have required privileges for this function")
                'MessageBox.Show("ActiveLogin does not have required privileges for this function")
            End If
        Else
            SharedEvents.RaiseOperationFailed("ModifySurveyLoginPrivileges", " _SurveyPrivlgs or _Subscriber IsNothing...")
        End If

        Return rslt
    End Function

    Public Function ModifyLoginInfoPrivBitMask(ByVal _TargetGuestLoginInfo As GuestLoginInfo) As Boolean
        Dim rslt As Boolean = False
        If Not IsNothing(_TargetGuestLoginInfo) Then
            If Me.ActiveLogin IsNot Nothing AndAlso Me.ActiveLogin.IsLoginAdministrator Then
                Dim SSMSvc As SubscriberDBSvc = Nothing
                Try
                    SSMSvc = New SubscriberDBSvc(Me.ActiveLogin.SubscrInfo)
                    If Not IsNothing(SSMSvc) Then
                        rslt = SSMSvc.ModifyLoginInfoPrivBitMask(_TargetGuestLoginInfo.SSM_LoginID, _TargetGuestLoginInfo.PrivBitMask)
                        SSMSvc.Dispose()
                        SSMSvc = Nothing
                    End If
                    'DisposeSSMDBSvc(SSMSvc)
                Catch ex As Exception
                    Me.OperationFailedName = "ModifyLoginInfoPrivBitMask"
                    Me.MyException = ex
                    RaiseEvent OperationFailed("ModifyLoginInfoPrivBitMask", New EventArgs)
                Finally
                    If Not IsNothing(SSMSvc) Then
                        SSMSvc.Dispose()
                        SSMSvc = Nothing
                    End If
                End Try
            Else
                SharedEvents.RaiseOperationFailed("ModifyLoginInfoPrivBitMask", " - ActiveLogin does not have required privileges for this function")
                'MessageBox.Show("ActiveLogin does not have required privileges for this function")
            End If
        Else
            SharedEvents.RaiseOperationFailed("ModifyLoginInfoPrivBitMask", " - _TargetGuestLoginInfo Is Nothing...")
        End If

        Return rslt
    End Function
#End Region


#Region "GetActiveLoginInfo"
    Public Function GetActiveLoginInfo(ByVal _NormalizedEmailAddress As String) As ActiveLoginInfo
        Dim rslt As ActiveLoginInfo = Nothing
        Try
            rslt = MyGSMSvc.ToActiveLoginInfo(_NormalizedEmailAddress)
            Dim SSMSvc As New SubscriberDBSvc(rslt.SubscrInfo)
            If SSMSvc IsNot Nothing Then
                Dim lirows = SSMSvc.GetLoginInfoRows
                'this _normalizedemailaddress could be an originating subscriber or an addedlogin....
                Dim liinforow = lirows.Where(Function(lir) lir.LoginEmail = rslt.NormalizedEmailAddress).FirstOrDefault
                If liinforow IsNot Nothing Then
                    rslt.SSM_LoginID = liinforow.LogInID
                    rslt.IsOriginatingSubscriber = liinforow.IsCustomerOriginator
                    rslt.PrivBitMask = liinforow.PrivBitMask
                    If liinforow.IsCustomerOriginator Then
                        rslt.SubscrInfo.SSM_LoginID = liinforow.LogInID
                        rslt.SubscrInfo.IsOriginatingSubscriber = liinforow.IsCustomerOriginator
                    Else
                        Dim subcr_liinforow = lirows.Where(Function(lir) lir.IsCustomerOriginator).FirstOrDefault
                        If subcr_liinforow IsNot Nothing Then
                            rslt.SubscrInfo.SSM_LoginID = subcr_liinforow.LogInID
                            rslt.SubscrInfo.IsOriginatingSubscriber = subcr_liinforow.IsCustomerOriginator
                        End If
                    End If
                Else
                    Dim subcr_liinforow = lirows.Where(Function(lir) lir.IsCustomerOriginator).FirstOrDefault
                    If subcr_liinforow IsNot Nothing Then
                        rslt.SubscrInfo.SSM_LoginID = subcr_liinforow.LogInID
                        rslt.SubscrInfo.IsOriginatingSubscriber = subcr_liinforow.IsCustomerOriginator
                    End If
                    rslt.SSM_LoginID = 0
                End If
                'this populates SubscriberInfo.CustomerID/Name properties
                Dim crow = SSMSvc.GetCustomerRows.FirstOrDefault
                If crow IsNot Nothing Then
                    rslt.SubscrInfo.SSM_CustomerID = crow.CustomerID
                    rslt.SubscrInfo.SSM_CustomerName = crow.CustomerName
                Else
                    rslt.SubscrInfo.SSM_CustomerID = 0
                    rslt.SubscrInfo.SSM_CustomerName = "NotSet"
                End If

                Me.ActiveLogin = rslt 'this get's set here because some of the functions below look for a subscriber info in the activelogin...

                'this populates  SubscriberInfo.SurveyPrivleges
                If crow IsNot Nothing AndAlso liinforow IsNot Nothing Then
                    rslt.SubscrInfo.SurveyPrivileges = SSMSvc.GetSurveyPrivilegeModels_List
                    'GetSurveyPrivilegeModels(_NormalizedEmailAddress, rslt.SubscrInfo)
                End If
                rslt.SubscrInfo.Guest_LoginInfo_List = (From li In lirows _
                                            Select New GuestLoginInfo With {.PrivBitMask = li.PrivBitMask, _
                                                                            .GSM_CustomerID = rslt.SubscrInfo.GSM_CustomerID, _
                                                                            .GSM_LoginID = li.LogInID_GSM, _
                                                                            .NormalizedEmailAddress = li.LoginEmail, _
                                                                            .RawEmailString = li.LoginEmailRawString, _
                                                                            .SSM_CustomerID = li.CustomerID, _
                                                                            .SSM_LoginID = li.LogInID, _
                                                                            .SurveyPrivileges = _
                                                                            (rslt.SubscrInfo.SurveyPrivileges.Where(Function(spm) spm.LoginID = li.LogInID)).ToList}).ToList
                rslt.SurveyPrivileges = (rslt.SubscrInfo.SurveyPrivileges.Where(Function(spm) spm.LoginID = rslt.SSM_LoginID)).ToList
                rslt.SubscrInfo.TinySurveyRow_List = (From sm In SSMSvc.GetSurveyRows _
                                                      Select New Tiny_Survey_Row With {.QueueName = sm.QueueName, _
                                                                                         .QueueUri = sm.QueueURI, _
                                                                                         .SurveyType = sm.SurveyType, _
                                                                                         .SurveyStateID = sm.SurveyStateID, _
                                                                                         .SurveyID = sm.SurveyID, _
                                                                                         .SurveyName = sm.SurveyDescription, _
                                                                                         .PrivilegeBitMask = 0}).ToList

            End If
            SSMSvc.Dispose()
            'DisposeSSMDBSvc(SSMSvc)
        Catch ex As Exception
            Me.ActiveLogin = Nothing
            Me.OperationFailedName = "GetActiveLoginInfo"
            Me.MyException = ex
            RaiseEvent OperationFailed(Me, New EventArgs)
        End Try
        Me.ActiveLogin = rslt
        Return rslt
    End Function

    Public Function RefreshSubscriber_SurveyPrivileges() As List(Of SurveyPrivilegeModel)
        Dim rslt As New List(Of SurveyPrivilegeModel)
        If Not IsNothing(Me.ActiveLogin) AndAlso Me.ActiveLogin.IsLoginAdministrator Then
            Dim SSMSvc As New SubscriberDBSvc(Me.ActiveLogin.SubscrInfo)
            If Not IsNothing(SSMSvc) Then
                Dim lirows = SSMSvc.GetLoginInfoRows
                Me.ActiveLogin.SubscrInfo.SurveyPrivileges = SSMSvc.GetSurveyPrivilegeModels_List
                rslt.AddRange(Me.ActiveLogin.SubscrInfo.SurveyPrivileges)
            End If
            SSMSvc.Dispose()
        End If

        Return rslt
    End Function

    'Public Function GetSurveyPrivilegeModels(ByVal _normalizedEmailAddress As String, ByVal _SubscrInfo As SubscriberInfo) As List(Of SurveyPrivilegeModel)
    '    Dim rslt As List(Of SurveyPrivilegeModel) = Nothing
    '    Dim subscrinfo As SubscriberInfo
    '    If _SubscrInfo IsNot Nothing Then
    '        subscrinfo = _SubscrInfo
    '    Else
    '        subscrinfo = Me.GetSubscriberInfo(_normalizedEmailAddress)
    '    End If
    '    Dim ema As String = subscrinfo.NormalizedEmailAddress
    '    'Dim lid As Integer = subscrinfo.SSM_LoginID
    '    Dim surveyPrivModels = From sm In GetSSMSurveyMasters(ema), lsp In GetSSMLoginSurveyPrivileges(ema) _
    '                   Where sm.SurveyID = lsp.SurveyID _
    '                   Select New SurveyPrivilegeModel With {.PrivBitMask = lsp.PrivEnumBitMask, _
    '                                                         .SurveyID = lsp.SurveyID, _
    '                                                         .SurveyDescription = sm.SurveyDescription, _
    '                                                         .LoginID = lsp.LoginID, _
    '                                                         .LoginSurveyPrivID = lsp.LoginSurveyPrivilegeID, _
    '                                                         .SurveyType = sm.SurveyType, _
    '                                                         .SurveyStateID = sm.SurveyStateID}
    '    rslt = surveyPrivModels.ToList
    '    Return rslt
    'End Function

    'Public Function GetDerivedSurveyPrivilegeModels(ByVal _normalizedEmailAddress) As List(Of SurveyPrivilegeModel)
    '    Dim rslt As List(Of SurveyPrivilegeModel) = Nothing

    '    Return rslt
    'End Function
#End Region

#Region "GetSubscriberInfo"

    Public Function GetSubscriberInfo(ByVal _NormalizedEmailAddress As String) As SubscriberInfo
        Dim rslt As SubscriberInfo = Nothing
        Try
            If Not IsNothing(Me.ActiveLogin) AndAlso Me.ActiveLogin.NormalizedEmailAddress = _NormalizedEmailAddress Then
                rslt = Me.ActiveLogin.SubscrInfo
            Else
                Me.ActiveLogin = GetActiveLoginInfo(_NormalizedEmailAddress)
                rslt = Me.ActiveLogin.SubscrInfo
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "SubscriberOperations"
                EvLog.Log = "Application"
                EvLog.WriteEntry("GetSubscriberInfo" & _NormalizedEmailAddress & " " & MyException.Message, EventLogEntryType.Error)
            End Using
        End Try
       
        Return rslt
    End Function

    Public Function RefreshSubscriber_GuestLoginInfo_List() As List(Of GuestLoginInfo)
        Dim rslt As New List(Of GuestLoginInfo)
       
        If Me.ActiveLogin IsNot Nothing AndAlso Me.ActiveLogin.IsLoginAdministrator Then
            Dim SSMSvc As SubscriberDBSvc = Nothing
            Try
                SSMSvc = New SubscriberDBSvc(Me.ActiveLogin.SubscrInfo)
                If Not IsNothing(SSMSvc) Then
                    Dim lirows = SSMSvc.GetLoginInfoRows
                    Me.ActiveLogin.SubscrInfo.Guest_LoginInfo_List = (From li In lirows _
                                                Select New GuestLoginInfo With {.PrivBitMask = li.PrivBitMask, _
                                                                                .GSM_CustomerID = Me.ActiveLogin.SubscrInfo.GSM_CustomerID, _
                                                                                .GSM_LoginID = li.LogInID_GSM, _
                                                                                .NormalizedEmailAddress = li.LoginEmail, _
                                                                                .RawEmailString = li.LoginEmailRawString, _
                                                                                .SSM_CustomerID = li.CustomerID, _
                                                                                .SSM_LoginID = li.LogInID, _
                                                                                .SurveyPrivileges = _
                                                                                (Me.ActiveLogin.SubscrInfo.SurveyPrivileges.Where(Function(spm) spm.LoginID = li.LogInID)).ToList}).ToList

                    rslt.AddRange(Me.ActiveLogin.SubscrInfo.Guest_LoginInfo_List)
                    SSMSvc.Dispose()
                    SSMSvc = Nothing
                End If
            Catch ex As Exception
                Me.OperationFailedName = "RefreshSubscriber_GuestLoginInfo_List"
                Me.MyException = ex
                RaiseEvent OperationFailed("RefreshSubscriber_GuestLoginInfo_List", New EventArgs)
            Finally
                If Not IsNothing(SSMSvc) Then
                    SSMSvc.Dispose()
                    SSMSvc = Nothing
                End If
            End Try
        Else
            SharedEvents.RaiseOperationFailed("RefreshSubscriber_GuestLoginInfo_List", " - ActiveLoginInfo Is Nothing or .IsLoginAdministator = false...")
        End If
        Return rslt
    End Function
#End Region


#Region "ChangeMyPwdPlease"
    Public Function ChangeMyPwdPlease(ByVal _PwdPkg As Password_Package) As Boolean
        If Me.MyGSMSvc IsNot Nothing Then
            Return Me.MyGSMSvc.ChangePwd(_PwdPkg)
        Else
            Me.MyGSMSvc = New GlobalSurveyMasterDBSvc
            Return Me.MyGSMSvc.ChangePwd(_PwdPkg)
        End If

    End Function
#End Region

#Region "Dispose GSM and SSM dbsvc classes"
    'Public Sub DisposeSSMDBSvc(ByVal _ssmDbsvc As SubscriberDBSvc)
    '    If _ssmDbsvc IsNot Nothing Then
    '        Try
    '            _ssmDbsvc.DisposeMe()
    '            _ssmDbsvc = Nothing
    '        Catch ex As Exception
    '            Me.OperationFailedName = "DisposeSSMDBSvc"
    '            Me.MyException = ex
    '            RaiseEvent OperationFailed(Me, New EventArgs)
    '        End Try
    '    End If
    'End Sub
#End Region

#Region "Testing REtrieval Methods"
    Public Function GetGSMLoginInfos() As List(Of MasterDBSvcLibr.LoginInfo)
        Dim rslt As List(Of MasterDBSvcLibr.LoginInfo) = Nothing
        rslt = MyGSMSvc.GetLoginInfos
        Return rslt
    End Function

    Public Function GetGSMCustomers() As List(Of MasterDBSvcLibr.Customer)
        Dim rslt As List(Of MasterDBSvcLibr.Customer) = Nothing
        rslt = MyGSMSvc.GetCustomers
        Return rslt
    End Function

    'Public Function GetSSMLoginInfos(ByVal _NormalizedEmailAddress As String) As List(Of LoginInfo)
    '    Dim rslt As New List(Of LoginInfo)

    '    Dim SSMSvc As New SubscriberDBSvc(GetSubscriberInfo(_NormalizedEmailAddress))
    '    If SSMSvc IsNot Nothing Then
    '        rslt = SSMSvc.GetLoginInfoRows
    '    End If
    '    DisposeSSMDBSvc(SSMSvc)
    '    Return rslt
    'End Function

    'Public Function GetSSMCustomers(ByVal _NormalizedEmailAddress As String) As List(Of Customer)
    '    Dim rslt As New List(Of Customer)

    '    Dim SSMSvc As New SubscriberDBSvc(GetSubscriberInfo(_NormalizedEmailAddress))
    '    If SSMSvc IsNot Nothing Then
    '        rslt = SSMSvc.GetCustomerRows
    '    End If
    '    DisposeSSMDBSvc(SSMSvc)
    '    Return rslt
    'End Function

    'Public Function GetSSMLoginSurveyPrivileges(ByVal _NormalizedEmailAddress As String) As List(Of LoginSurveyPrivilege)
    '    Dim rslt As New List(Of LoginSurveyPrivilege)

    '    Dim SSMSvc As New SubscriberDBSvc(GetSubscriberInfo(_NormalizedEmailAddress))
    '    If SSMSvc IsNot Nothing Then
    '        rslt = SSMSvc.GetLoginSurveyPrivilegeRows
    '    End If
    '    DisposeSSMDBSvc(SSMSvc)
    '    Return rslt
    'End Function

    'Public Function GetSSMSurveyMasters(ByVal _NormalizedEmailAddress As String) As List(Of SurveyMaster)
    '    Dim rslt As New List(Of SurveyMaster)

    '    Dim SSMSvc As New SubscriberDBSvc(GetSubscriberInfo(_NormalizedEmailAddress))
    '    If SSMSvc IsNot Nothing Then
    '        rslt = SSMSvc.GetSurveyRows
    '    End If
    '    DisposeSSMDBSvc(SSMSvc)
    '    Return rslt
    'End Function
#End Region

#Region "Properties"
    Private _ActiveLogin As ActiveLoginInfo
    Public Property ActiveLogin() As ActiveLoginInfo
        Get
            Return _ActiveLogin
        End Get
        Set(ByVal value As ActiveLoginInfo)
            _ActiveLogin = value
        End Set
    End Property

    Private _CurrentSubscriber As SubscriberInfo
    Public Property CurrentSubscriber() As SubscriberInfo
        Get
            Return _CurrentSubscriber
        End Get
        Set(ByVal value As SubscriberInfo)
            _CurrentSubscriber = value
        End Set
    End Property


#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                RemoveHandler Me.OperationFailed, AddressOf OperationFailedHandler
                If Not IsNothing(Me.MyGSMSvc) Then
                    Me.MyGSMSvc.Dispose()
                    Me.MyGSMSvc = Nothing
                End If
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region



End Class
