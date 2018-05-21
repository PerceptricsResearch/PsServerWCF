Imports CmdInfrastructureNS
Imports MasterDBSvcLibr

Public Class SubscriberDBSvc
    Implements IDisposable


#Region "Private Fields"
    Private MyCurrentSubscriber As SubscriberInfo
    Private WithEvents MyDB As L2S_SubscriberSurveyMasterDataContext
    Private MyDBAvailable As Boolean = False
    Public OperationFailedName As String = Nothing
    Public MyException As Exception = Nothing
    Private MyDB_TTL As Integer = 0 'not implemented '''would keep the MyDB datacontext around for some time...if past the time, dispose, myDB then create New l2s_DBcontext
#End Region

#Region "New and DisposeMe"
    Public Sub New(ByVal _SubscriberInfo As SubscriberInfo)
        If Not IsNothing(_SubscriberInfo) Then
            Me.MyCurrentSubscriber = _SubscriberInfo
            AddHandler Me.SSM_DB_OperationFailed, AddressOf OperationFailedHandler
            AddHandler Me.SSM_DB_UnavailableEvent, AddressOf DBUnavailableHandler
            Try
                Me.MyDB = New L2S_SubscriberSurveyMasterDataContext(_SubscriberInfo.SSM_CnxnString)
                MyDBAvailable = True
            Catch ex As Exception
                Me.OperationFailedName = "New"
                Me.MyException = ex
                RaiseEvent SSM_DB_UnavailableEvent("New", New EventArgs)
            End Try
        Else
            SharedEvents.RaiseOperationFailed("SubscriberDBSvc.New", " - _SubscriberInfo Is Nothing...")
        End If
    End Sub

    Public Sub DisposeMe()
        Me.Dispose()
    End Sub
#End Region

#Region "Action Methods"
    Public Function AddNewSubscriber(ByRef _NewSubscriberPkg As NewSubscriberPackage) As Boolean
        Dim rslt As Boolean = False
        If MyDBAvailable Then
            Try
                'this cleans the "template surveyMaster db ...
                MyDB.Customers.DeleteAllOnSubmit(MyDB.Customers.AsEnumerable)
                MyDB.LoginInfos.DeleteAllOnSubmit(MyDB.LoginInfos.AsEnumerable)
                MyDB.LoginSurveyPrivileges.DeleteAllOnSubmit(MyDB.LoginSurveyPrivileges.AsEnumerable)
                MyDB.SurveyDataStores.DeleteAllOnSubmit(MyDB.SurveyDataStores.AsEnumerable)
                'Dim firstlen = Len(_NewSubscriberPkg.CreateDBInfo.Destination_DataStore_CnxnString)
                'Dim secondlen = Len(_NewSubscriberPkg.CreateDBInfo.Destination_DataStoreDatabaseName)
                Dim primarySDS As New SurveyDataStore With {.CnxnString = _NewSubscriberPkg.CreateDBInfo.Destination_DataStore_CnxnString, _
                                                            .DatabaseName = _NewSubscriberPkg.CreateDBInfo.Destination_DataStoreDatabaseName, _
                                                            .AbsolutePath = _NewSubscriberPkg.CreateDBInfo.Destination_DataStore_CnxnString, _
                                                            .ComputerID = 1}
                Dim primSDSID = InsertNewSurveyDataStore(primarySDS)
                Dim custrow As New Customer With {.CreatedDate = DateAndTime.Now, _
                                                  .CreatingLoginID_GSM = _NewSubscriberPkg.SubscrInfo.GSM_LoginID, _
                                                  .CurrentAuthorizationID = _NewSubscriberPkg.AuthorizationID, _
                                                  .CustomerID_GSM = _NewSubscriberPkg.SubscrInfo.GSM_CustomerID, _
                                                  .PrimaryDataStoreCnxnString = _NewSubscriberPkg.CreateDBInfo.Destination_DataStore_CnxnString, _
                                                  .CustomerName = _NewSubscriberPkg.SubscriberName, _
                                                  .RDentQueueURI_String = _NewSubscriberPkg.RDentQueueURI, _
                                                  .RDentQueueName = _NewSubscriberPkg.RDentQueueName, _
                                                  .SubscriptionLevelBitmask = _NewSubscriberPkg.SubscriptionLevel, _
                                                  .PrimaryDataStoreID = primSDSID}
                Dim crowId = InsertNewCustomerRow(custrow)
                _NewSubscriberPkg.SubscrInfo.SSM_CustomerID = crowId
                Dim liInfo As New LoginInfo With {.CustomerID = crowId, _
                                                  .IsCustomerOriginator = True, _
                                                  .LoginEmailRawString = _NewSubscriberPkg.RawEmailAddress, _
                                                  .IsLoggedIn = False, _
                                                  .LastLoginDate = Date.Now, _
                                                  .LoginEmail = _NewSubscriberPkg.NormalizedEmailAddress, _
                                                  .LogInID_GSM = _NewSubscriberPkg.SubscrInfo.GSM_LoginID, _
                                                  .PasswordHash = 0, _
                                                  .PasswordLastSetDate = Date.Now, _
                                                  .PrivBitMask = 8128}
                Dim liinfoID = InsertNewLoginInfoRow(liInfo)
                Dim notusedprivid = MyDB.Privileges.FirstOrDefault.PriviligeID
                Dim lsprows As New List(Of LoginSurveyPrivilege)
                For Each sm In MyDB.SurveyMasters
                    lsprows.Add(New LoginSurveyPrivilege With {.GlobalSMLoginID = _NewSubscriberPkg.SubscrInfo.GSM_LoginID, _
                                                                    .LoginID = liinfoID, _
                                                                    .PrivEnumBitMask = _NewSubscriberPkg.DefaultPrivilegeBitMask, _
                                                                    .PrivilegeID = notusedprivid, _
                                                                    .SurveyID = sm.SurveyID})
                    sm.SurveyDataStoreID = primSDSID
                    sm.LoginID = liinfoID
                Next

                MyDB.LoginSurveyPrivileges.InsertAllOnSubmit(lsprows)
                MyDB.SubmitChanges()
                rslt = True
            Catch ex As Exception
                Me.OperationFailedName = "AddNewSubscriber"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("AddNewSubscriber", New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent("AddNewSubscriber", New EventArgs)
        End If
        Return rslt
    End Function

    ''' <summary>
    ''' This adds a survey...Note that the _NewSurveyPkg is returned to the calling method with updated POCO_Pkg and a Survey_DC_List_Item...
    ''' this method also adds loginsurveyprivilege rows so that other Login's within the subscriber can see the new survey...
    ''' </summary>
    ''' <param name="_NewSurveyPkg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddNewSurvey(ByRef _NewSurveyPkg As NewSurveyPackage) As Boolean
        Dim rslt As Boolean = False
        If MyDBAvailable Then
            Try
                Dim crow As Customer = MyDB.Customers.FirstOrDefault
                Dim newSMrow As New SurveyMaster With {.ActiveRespondentsCount = 0, _
                                                       .CreatedDate = DateAndTime.Now, _
                                                       .LastModifiedDate = DateAndTime.Now, _
                                                       .LoginID = _NewSurveyPkg.SubScriberInfo.SSM_LoginID, _
                                                       .SurveyDataStoreComputer = "NotSet", _
                                                       .QueueComputer = "NotSet", _
                                                       .QueueName = crow.RDentQueueName & "_S_", _
                                                       .QueueURI = crow.RDentQueueURI_String & "_S_", _
                                                       .SurveyStateID = 0, _
                                                       .SurveyType = 1, _
                                                       .Model = "", _
                                                       .SurveyDescription = _NewSurveyPkg.SurveyItemPkg.SurveyName, _
                                                       .SurveyDataStoreID = crow.PrimaryDataStoreID}
                Dim smID = InsertNewSurveyMasterRow(newSMrow)
                Dim orginalID As Integer = _NewSurveyPkg.SurveyItemPkg.SIM_SDSID
                _NewSurveyPkg.POCO_Pkg = New POCO_ID_Pkg With {.Survey_ID = smID, _
                                         .Original_ID = orginalID, _
                                         .DB_ID = smID, _
                                         .POCOGuid = _NewSurveyPkg.SurveyItemPkg.MyGuid}
                Dim priv = MyDB.Privileges.FirstOrDefault.PriviligeID
                'create loginsurveypriv rows so that other logins can see this survey...
                Dim svylist As New List(Of SurveyMaster)
                svylist.Add(newSMrow)
                For Each lsp In CreateLoginSurveyPrivRows(svylist, _NewSurveyPkg.SubScriberInfo.SubscriberandAddedGuest_LoginInfo_List)
                    InsertNewLoginSurveyPrivilegeRow(lsp)
                Next
                _NewSurveyPkg.Survey_DC_List_Item = GetSurveyDC_List.Where(Function(kvp) kvp.Key = smID).FirstOrDefault
                rslt = True
            Catch ex As Exception
                Me.OperationFailedName = "AddNewSurvey"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("AddNewSurvey", New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent("AddNewSurvey", New EventArgs)
        End If
        Return rslt
    End Function

    Public Function PublishSurvey(ByRef _PublSurvPkg As PublishSurveyPackage) As Boolean
        Dim rslt As Boolean = False
        If MyDBAvailable Then
            Try
                Dim svyID = _PublSurvPkg.SurveyID
                ' PublSurveyPkg populate its properties...th
                Dim sm = MyDB.SurveyMasters.Where(Function(sv) sv.SurveyID = svyID).FirstOrDefault
                If sm IsNot Nothing Then
                    If PrepareSurveyForRDENTS() Then 'doesn't do anything...is a placeholder for stuff that might need to be done...
                        If sm.SurveyStateID < 1 Then
                            sm.FirstPublishedDate = Date.Now
                            AddPostingLoginSurveyPrivilege(svyID) 'this gives the posting svc classes access to the survey databases...
                        End If
                        sm.SurveyStateID = 1
                        sm.QueueName = _PublSurvPkg.SurveyQueueName
                        sm.QueueURI = _PublSurvPkg.SurveyQueueURI

                        sm.LastPublishedDate = Date.Now
                        '_PublSurvPkg.PublishedSurveyLinkString
                        MyDB.SubmitChanges()
                        rslt = True
                    End If
                End If
            Catch ex As Exception
                Me.OperationFailedName = "PublishSurvey"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("PublishSurvey", New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent("PublishSurvey", New EventArgs)
        End If

        Return rslt
    End Function


    Public Function UnPublishSurvey(_SurveyID As Integer) As Boolean
        Dim rslt As Boolean = False
        If MyDBAvailable Then
            Try
                Dim sm = MyDB.SurveyMasters.Where(Function(sv) sv.SurveyID = _SurveyID).FirstOrDefault
                If Not IsNothing(sm) Then
                    If UnPrepareSurveyForRDENTS() Then 'doesn't do anything...is a placeholder for stuff that might need to be done...

                        sm.SurveyStateID = SurveyState.PublishedClosed

                        MyDB.SubmitChanges()
                        rslt = True
                    End If
                    sm = Nothing
                End If
            Catch ex As Exception
                Me.OperationFailedName = "UnPublishSurvey"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("UnPublishSurvey", New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent("UnPublishSurvey", New EventArgs)
        End If

        Return rslt
    End Function

    ''' <summary>
    ''' The posting svc creates SDS_ResponseModels as they are required....so this is only a placeholder now...just in case we need to do something
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function PrepareSurveyForRDENTS() As Boolean
        Dim rslt As Boolean = False
        rslt = True
        Return rslt
    End Function

    Private Function UnPrepareSurveyForRDENTS() As Boolean
        Return True
    End Function

    ''' <summary>
    ''' This adds LoginInfo row...and LoginSurveyPrivilege rows for all Surveys...using a default privilege in the app.config...unless
    ''' the NewLoginPkg.SurveyPrivileges list contains any members. In that case this method will create LoginSurveyPrivilege rows for only those members using the associated Privilege.
    ''' </summary>
    ''' <param name="_NewLoginPkg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddNewLoginToExistingSubscriber(ByVal _NewLoginPkg As NewLoginPackage) As Boolean
        Dim rslt As Boolean = False
        If MyDBAvailable Then
            Try

                Dim liInfo As New LoginInfo With {.IsCustomerOriginator = False, _
                                                  .LoginEmail = _NewLoginPkg.NormalizedEmailAddress, _
                                                  .CustomerID = _NewLoginPkg.SubScriberInfo.SSM_CustomerID, _
                                                  .LogInID_GSM = _NewLoginPkg.LoginID_GSM, _
                                                  .PasswordHash = 0, _
                                                  .PasswordLastSetDate = DateTime.Now, _
                                                  .IsLoggedIn = False, _
                                                  .LastLoginDate = DateAndTime.Now, _
                                                  .PrivBitMask = _NewLoginPkg.DefaultPrivilegeBitMask}
                Dim liinfoID = InsertNewLoginInfoRow(liInfo)
                Dim notusedprivid = MyDB.Privileges.FirstOrDefault.PriviligeID
                Dim lsprows As New List(Of LoginSurveyPrivilege) 'this is the parameter to the InsertAllOnSubmit function below...
                If _NewLoginPkg.IsSubscriberAddedGuestLogin Then
                    For Each sm In MyDB.SurveyMasters
                        lsprows.Add(New LoginSurveyPrivilege With {.GlobalSMLoginID = _NewLoginPkg.LoginID_GSM, _
                                                                        .LoginID = liinfoID, _
                                                                        .PrivEnumBitMask = _NewLoginPkg.DefaultPrivilegeBitMask, _
                                                                        .PrivilegeID = notusedprivid, _
                                                                        .SurveyID = sm.SurveyID})
                    Next
                ElseIf _NewLoginPkg.IsRDentLogin Then
                    For Each spm In _NewLoginPkg.SurveyPrivileges
                        Dim spmsvyID = spm.SurveyID
                        Dim sm = (MyDB.SurveyMasters.Where(Function(sr) sr.SurveyID = spmsvyID)).FirstOrDefault
                        lsprows.Add(New LoginSurveyPrivilege With {.GlobalSMLoginID = _NewLoginPkg.LoginID_GSM, _
                                                .LoginID = liinfoID, _
                                                .PrivEnumBitMask = spm.PrivBitMask, _
                                                .PrivilegeID = notusedprivid, _
                                                .SurveyID = sm.SurveyID})
                    Next
                End If

                MyDB.LoginSurveyPrivileges.InsertAllOnSubmit(lsprows)
                MyDB.SubmitChanges()
                rslt = True
            Catch ex As Exception
                Me.OperationFailedName = "AddNewLoginToExistingSubscriber"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("AddNewLoginToExistingSubscriber", New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent("AddNewLoginToExistingSubscriber", New EventArgs)
        End If
        Return rslt
    End Function

    Public Function ModifySurveyLoginPrivileges(ByVal _SurveyPrivList As List(Of SurveyPrivilegeModel)) As Boolean
        Dim rslt As Boolean = False
        If MyDBAvailable Then
            Try
                Dim lsprows = Me.GetLoginSurveyPrivilegeRows()
                For Each spm In _SurveyPrivList
                    rslt = False
                    Dim xspm = spm
                    Dim lsp = (lsprows.Where(Function(l) l.LoginSurveyPrivilegeID = xspm.LoginSurveyPrivID AndAlso l.SurveyID = xspm.SurveyID)).FirstOrDefault
                    If lsp IsNot Nothing Then
                        lsp.PrivEnumBitMask = spm.PrivBitMask
                        MyDB.SubmitChanges()
                        rslt = True
                    Else
                        Dim newlsp As New LoginSurveyPrivilege With {.LoginID = spm.LoginID, _
                                                                     .GlobalSMLoginID = spm.GSM_LoginID, _
                                                                  .SurveyID = spm.SurveyID, _
                                                                  .PrivEnumBitMask = spm.PrivBitMask}
                        Dim ndx = InsertNewLoginSurveyPrivilegeRow(newlsp)
                        If ndx > 0 Then
                            rslt = True
                        End If
                    End If
                Next
            Catch ex As Exception
                Me.OperationFailedName = "ModifySurveyLoginPrivileges"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("ModifySurveyLoginPrivileges", New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent(Me, New EventArgs)
        End If
        Return rslt
    End Function

    ''' <summary>
    ''' This adds a LoginSurveyPrivilege row for the PostingSvc logininfo and the published survey with _SurveyID...
    ''' </summary>
    ''' <param name="_SurveyID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddPostingLoginSurveyPrivilege(ByVal _SurveyID As Integer) As Boolean
        Dim rslt As Boolean = False
        Dim postingLoginName = Me.MyCurrentSubscriber.PostingLoginInfo_NormalizedEmailAddr
        Dim rpli = (From li In Me.GetLoginInfoRows _
                   Where li.LoginEmail = postingLoginName _
                   Select li).FirstOrDefault
        If rpli IsNot Nothing Then
            Dim lsp As New LoginSurveyPrivilege With {.GlobalSMLoginID = rpli.LogInID_GSM, _
                                                    .LoginID = rpli.LogInID, _
                                                    .PrivEnumBitMask = 48, _
                                                    .PrivilegeID = 0, _
                                                    .SurveyID = _SurveyID}
            rslt = InsertNewLoginSurveyPrivilegeRow(lsp)
        Else
            Me.OperationFailedName = "AddPostingLoginSurveyPrivilege"
            Me.MyException = New Exception With {.Source = "AddPostingLoginSurveyPrivilege CouldNotFind <" & Me.MyCurrentSubscriber.SSM_CustomerName & "responses>"}
            RaiseEvent SSM_DB_OperationFailed("AddPostingLoginSurveyPrivilege", New EventArgs)
        End If
        Return rslt
    End Function

    Public Function ModifyLoginInfoPrivBitMask(ByVal _SSMLoginID As Integer, ByVal _PrivBitmask As ULong) As Boolean
        Dim rslt As Boolean = False
        If MyDBAvailable Then
            Try
                Dim lirow = (From li In MyDB.LoginInfos _
                            Where li.LogInID = _SSMLoginID _
                            Select li).FirstOrDefault
                If lirow IsNot Nothing Then
                    lirow.PrivBitMask = _PrivBitmask
                    MyDB.SubmitChanges()
                    rslt = True
                End If
            Catch ex As Exception
                Me.OperationFailedName = "ModifyLoginInfoPrivBitMask"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("ModifyLoginInfoPrivBitMask", New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent("ModifyLoginInfoPrivBitMask", New EventArgs)
        End If
        Return rslt
    End Function
#End Region

#Region "Insert Methods"
    Public Function InsertNewCustomerRow(ByVal _newCustomerRow) As Integer
        Dim rslt As Integer = Nothing
        If MyDBAvailable Then
            Try
                MyDB.Customers.InsertOnSubmit(_newCustomerRow)
                MyDB.SubmitChanges()
                rslt = _newCustomerRow.CustomerID
            Catch ex As Exception
                Me.OperationFailedName = "InsertNewCustomerRow"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("InsertNewCustomerRow", New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent("InsertNewCustomerRow", New EventArgs)
        End If
        Return rslt
    End Function

    Public Function InsertNewLoginInfoRow(ByVal _newLoginInfoRow As LoginInfo) As Integer
        Dim rslt As Integer = Nothing
        If MyDBAvailable Then
            Try
                MyDB.LoginInfos.InsertOnSubmit(_newLoginInfoRow)
                MyDB.SubmitChanges()
                rslt = _newLoginInfoRow.LogInID
            Catch ex As Exception
                Me.OperationFailedName = "InsertNewLoginInfoRow"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed(Me, New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent(Me, New EventArgs)
        End If
        Return rslt
    End Function

    Public Function InsertNewSurveyMasterRow(ByVal _newSurvMstrRow As SurveyMaster) As Integer
        Dim rslt As Integer = Nothing
        If MyDBAvailable Then
            Try
                MyDB.SurveyMasters.InsertOnSubmit(_newSurvMstrRow)
                MyDB.SubmitChanges()
                rslt = _newSurvMstrRow.SurveyID
            Catch ex As Exception
                Me.OperationFailedName = "InsertNewSurveyMasterRow"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed(Me, New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent(Me, New EventArgs)
        End If
        Return rslt
    End Function

    Public Function InsertNewLoginSurveyPrivilegeRow(ByVal _newLSPRow As LoginSurveyPrivilege) As Integer
        Dim rslt As Integer = 0
        If MyDBAvailable Then
            Try
                MyDB.LoginSurveyPrivileges.InsertOnSubmit(_newLSPRow)
                MyDB.SubmitChanges()
                rslt = _newLSPRow.LoginSurveyPrivilegeID
            Catch ex As Exception
                Me.OperationFailedName = "InsertNewLoginSurveyPrivilegeRow"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed(Me, New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent(Me, New EventArgs)
        End If
        Return rslt
    End Function

    Public Function InsertNewSurveyDataStore(ByVal _newSDSRow As SurveyDataStore) As Integer
        Dim rslt As Integer = Nothing
        If MyDBAvailable Then
            Try
                MyDB.SurveyDataStores.InsertOnSubmit(_newSDSRow)
                MyDB.SubmitChanges()
                rslt = _newSDSRow.SurveyDataStoreID
            Catch ex As Exception
                Me.OperationFailedName = "InsertNewSDSRow"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed(Me, New EventArgs)
            End Try
        Else
            RaiseEvent SSM_DB_UnavailableEvent(Me, New EventArgs)
        End If
        Return rslt
    End Function

    'Public Function InsertAllNewSDS_ResponseModel(_listofSDSResponseModel As List(Of SDS_Responsemodel) as 

#End Region

    Public Function CreateLoginSurveyPrivRows(ByVal _SurveyRows As List(Of SurveyMaster), ByVal _LoginInfoRows As List(Of GuestLoginInfo)) As List(Of LoginSurveyPrivilege)
        Dim lsprows = (From sm In _SurveyRows, li In _LoginInfoRows _
                       Select New LoginSurveyPrivilege With {.LoginID = li.SSM_LoginID, _
                                                             .GlobalSMLoginID = li.GSM_LoginID, _
                                                             .SurveyID = sm.SurveyID, _
                                                             .PrivEnumBitMask = li.PrivBitMask, _
                                                             .PrivilegeID = 0}).ToList
        Return lsprows
    End Function
    Public Function DerivePrivBitmask(ByVal _PrivBitmask As ULong) As ULong
        Dim rslt As ULong = 0
        If _PrivBitmask And PriviligeDescrEnum.ReadAny Then
            rslt += PriviligeDescrEnum.AuthorRead
        End If
        If _PrivBitmask And PriviligeDescrEnum.WriteAny Then
            rslt += PriviligeDescrEnum.AuthorRead
            rslt += PriviligeDescrEnum.AuthorWrite
        End If
        If _PrivBitmask And PriviligeDescrEnum.RsltsViewerAny Then
            rslt += PriviligeDescrEnum.RsltsViewer
        End If
        Return rslt
    End Function

#Region "UpdateMethods"

#End Region

#Region "Retrieval Methods"
    Public Function GetCustomerRows() As List(Of Customer)
        Dim rslt As New List(Of Customer)
        If MyDBAvailable Then
            Try
                rslt = MyDB.Customers.ToList
            Catch ex As Exception
                Me.OperationFailedName = "GetCustomerRows"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("GetCustomerRows", New EventArgs)
            End Try
        End If
        Return rslt
    End Function

    Public Function GetLoginInfoRows() As List(Of LoginInfo)
        Dim rslt As New List(Of LoginInfo)
        If MyDBAvailable Then
            Try
                rslt = MyDB.LoginInfos.ToList
            Catch ex As Exception
                Me.OperationFailedName = "GetLoginInfoRows"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("GetLoginInfoRows", New EventArgs)
            End Try
        End If
        Return rslt
    End Function

    Public Function GetLoginSurveyPrivilegeRows() As List(Of LoginSurveyPrivilege)
        Dim rslt As New List(Of LoginSurveyPrivilege)
        If MyDBAvailable Then
            Try
                rslt = MyDB.LoginSurveyPrivileges.ToList
            Catch ex As Exception
                Me.OperationFailedName = "GetLoginSurveyPrivilegeRows"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("GetLoginSurveyPrivilegeRows", New EventArgs)
            End Try
        End If
        Return rslt
    End Function

    Public Function GetSurveyRows() As List(Of SurveyMaster)
        Dim rslt As New List(Of SurveyMaster)
        If MyDBAvailable Then
            Try
                rslt = MyDB.SurveyMasters.ToList
            Catch ex As Exception
                Me.OperationFailedName = "GetSurveyRows"
                Me.MyException = ex
                RaiseEvent SSM_DB_OperationFailed("GetSurveyRows", New EventArgs)
            End Try
        End If
        Return rslt
    End Function

    Public Function GetSurveyPrivilegeModels_List() As List(Of SurveyPrivilegeModel)
        Dim rslt As List(Of SurveyPrivilegeModel) = Nothing
        Dim surveyPrivModels = From sm In MyDB.SurveyMasters, lsp In MyDB.LoginSurveyPrivileges _
               Where sm.SurveyID = lsp.SurveyID _
               Select New SurveyPrivilegeModel With {.PrivBitMask = lsp.PrivEnumBitMask, _
                                                     .SurveyID = lsp.SurveyID, _
                                                     .SurveyDescription = sm.SurveyDescription, _
                                                     .LoginID = lsp.LoginID, _
                                                     .LoginSurveyPrivID = lsp.LoginSurveyPrivilegeID, _
                                                     .SurveyType = sm.SurveyType, _
                                                     .SurveyStateID = sm.SurveyStateID}
        rslt = surveyPrivModels.ToList
        rslt.AddRange(GetDerivedSurveyPrivilegeModels(rslt))
        Return rslt
    End Function
    Public Function GetDerivedSurveyPrivilegeModels(ByVal _explicitSPMs As List(Of SurveyPrivilegeModel)) As List(Of SurveyPrivilegeModel)
        Dim rslt As New List(Of SurveyPrivilegeModel)

        Dim lirowswithAnyPrivileges = From li In MyDB.LoginInfos _
                                      Where li.PrivBitMask And PriviligeDescrEnum.ReadAny Or li.PrivBitMask And PriviligeDescrEnum.WriteAny Or li.PrivBitMask And PriviligeDescrEnum.RsltsViewerAny _
                                      Select li
        For Each li In lirowswithAnyPrivileges
            Dim lid = li.LogInID
            Dim translatedpriv As ULong = 0
            If li.PrivBitMask And PriviligeDescrEnum.ReadAny Then
                translatedpriv += PriviligeDescrEnum.AuthorRead
            End If
            If li.PrivBitMask And PriviligeDescrEnum.WriteAny Then
                translatedpriv += PriviligeDescrEnum.AuthorWrite
            End If
            If li.PrivBitMask And PriviligeDescrEnum.RsltsViewerAny Then
                translatedpriv += PriviligeDescrEnum.RsltsViewer
            End If
            Dim spms = From sm In MyDB.SurveyMasters _
                       Select New SurveyPrivilegeModel With {.PrivBitMask = translatedpriv, _
                                                     .SurveyID = sm.SurveyID, _
                                                     .SurveyDescription = sm.SurveyDescription, _
                                                     .LoginID = lid, _
                                                     .LoginSurveyPrivID = -1, _
                                                     .SurveyType = sm.SurveyType, _
                                                     .SurveyStateID = sm.SurveyStateID}
            For Each spm In spms
                Dim newspm = spm
                Dim existingspm = _explicitSPMs.Where(Function(xspm) xspm.LoginID = newspm.LoginID AndAlso xspm.SurveyID = newspm.SurveyID).FirstOrDefault
                If existingspm IsNot Nothing Then
                    If existingspm.PrivBitMask And spm.PrivBitMask Then
                        'has this priv already...
                        Dim x = 2
                    Else
                        Dim x = 2
                    End If
                Else
                    rslt.Add(spm)
                End If

            Next
        Next
        Return rslt
    End Function

    Public Function GetSurveyDC_List() As List(Of Srlzd_KVP)
        Dim rslt As New List(Of Srlzd_KVP)
        rslt.AddRange(From sm In MyDB.SurveyMasters, sds In MyDB.SurveyDataStores _
                           Where sm.SurveyDataStoreID = sds.SurveyDataStoreID _
                           Select New Srlzd_KVP(sm.SurveyID, sds.CnxnString))
        Return rslt
    End Function
#End Region

#Region "Properties"
    Public ReadOnly Property CurrentSubscriber() As SubscriberInfo
        Get
            Return Me.MyCurrentSubscriber
        End Get
    End Property
#End Region

#Region "Custom Events"
    Public Event SSM_DB_UnavailableEvent(ByVal sender As Object, ByVal e As EventArgs)
    Public Event SSM_DB_OperationFailed(ByVal sender As Object, ByVal e As EventArgs)
#End Region

#Region "Event Handlers"
    Private Sub OperationFailedHandler(sender As Object, e As EventArgs)
        Try
            Using EvLog As New EventLog()
                EvLog.Source = "SubscriberDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry(OperationFailedName & " " & MyException.Message, EventLogEntryType.Error)
            End Using
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "SubscriberDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry(OperationFailedName & " " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
    End Sub
    Private Sub DBUnavailableHandler(sender As Object, e As EventArgs)
        Try
            Using EvLog As New EventLog()
                EvLog.Source = "SubscriberDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("DBUnavailable " & sender.ToString & " " & Me.CurrentSubscriber.NormalizedEmailAddress, EventLogEntryType.Error)
            End Using
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "SubscriberDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("DBUnavailable " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try
    End Sub
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Try
                    If Not IsNothing(MyDB) Then
                        MyDB.Dispose()
                        MyDB = Nothing
                    End If

                Catch ex As Exception

                End Try
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