Imports CmdInfrastructureNS
Imports System.Runtime.Serialization
Imports IAuthoringSvcNS
Imports System.ServiceModel
Imports System.Net.Mail
Imports System.Text

Public Class AuthoringSvc
    Implements IAuthoringSvcNS.IAuthoringSvc

    Public MyCSH As CmdSvcClassLibrary.CustomSvcHost = CType(OperationContext.Current.Host, CmdSvcClassLibrary.CustomSvcHost)
    ' Private InstanceTracker As New CmdSvcClassLibrary.InstanceTracker(OperationContext.Current.Host)
    Private MyEptSuffix As String = ""
    Private MyPgItemColxnSvc As PageItemColxnServicesLibr.PgItemColxnSvc = Nothing
    Private MyCustomerDBSvc As CustomerDBSvc.CustomerDBSvc = Nothing
    Private MySubscriberOps As SubscriberOps_NS.SubscriberOperations

    Private Sub DisposeMe()
        Try
            If Not IsNothing(Me.MyCustomerDBSvc) Then
                With Me.MyCustomerDBSvc
                    .Dispose()
                End With
                Me.MyCustomerDBSvc = Nothing
            End If
            If Not IsNothing(Me.MySubscriberOps) Then
                With Me.MySubscriberOps
                    .Dispose()
                End With
                Me.MySubscriberOps = Nothing
            End If
            If Not IsNothing(Me.MyPgItemColxnSvc) Then
                With Me.MyPgItemColxnSvc
                    .Dispose()
                End With
                Me.MyPgItemColxnSvc = Nothing
            End If

            Me.MyCSH = Nothing
            Me.MyEptSuffix = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("DisposeMe: EptSuffix " & Me.MyEptSuffix & " Returns False ", EventLogEntryType.FailureAudit)
            End Using
        End Try

    End Sub

#Region "ImageManagerMethods"
    Public Function RetrieveManagerGuidStrList(_imgsize As Integer) As List(Of String) Implements IAuthoringSvc.RetrieveManagerGuidStrList
        Dim rslt As List(Of String) = Nothing
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If SetUpCustomerDBSvc() Then
                rslt = MyCustomerDBSvc.RetrieveManagerGuidStrList(_imgsize)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RetrieveManagerGuidStrList: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try
        Return rslt
    End Function

    Public Function AddManagerImages(ByVal _mgrimgspkg As SurveyImagesPackage) As Boolean Implements IAuthoringSvc.AddManagerImages
        Dim rslt As Boolean = False
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If Not IsNothing(_mgrimgspkg) AndAlso Not IsNothing(_mgrimgspkg.ImageStorePkgList) Then
                If SetUpCustomerDBSvc() Then
                    rslt = MyCustomerDBSvc.AddManagerImages(_mgrimgspkg)
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("AddManagerImages: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try
        Return rslt
    End Function

    Public Function RemoveManagerImages(ByVal _guidstr As String) As Boolean Implements IAuthoringSvc.RemoveManagerImages
        Dim rslt As Boolean = False
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If Not IsNothing(_guidstr) Then
                If SetUpCustomerDBSvc() Then
                    rslt = MyCustomerDBSvc.RemoveManagerImages(_guidstr)
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RemoveManagerImages: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try
        Return rslt
    End Function
#End Region

#Region "SendEmailPlease"
    Public Function SendEmailPlease(ByVal _SendEmailPkg As SendEmail_Package) As Boolean Implements IAuthoringSvc.SendEmailPlease
        Dim ep As New EmailPlatform
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            ep.SendEmailPlease(_SendEmailPkg)
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SendEmailPlease: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try

        'Dim mail = New MailMessage()
        ''these are myaccounts for now....
        'mail.From = New MailAddress("wbartel@interserv.com")
        'mail.To.Add(New MailAddress("wbartel@interserv.com"))

        'mail.Subject = "Perceptrics Research "

        'mail.IsBodyHtml = False

        ''mail.Body = "This is the message in the body...the link to your survey is http://rents:88/Perceptrics/?survey=22&rdent=qwe_corprdent22"
        'Dim sbldr As New StringBuilder
        'sbldr.AppendLine("EmailFormName: " & _SendEmailPkg.EmailFormName)
        'sbldr.AppendLine("InitiatedBy: " & _SendEmailPkg.InitiatingNormalizedEmailAddr)
        'sbldr.AppendLine("ToAddress_Normalized: " & _SendEmailPkg.ToAddress_Normalized)
        'For Each skvp In _SendEmailPkg.MessageContentColxn
        '    sbldr.AppendLine(skvp.Key & ": " & skvp.Valu)
        'Next
        'mail.Body = sbldr.ToString
        'sbldr = Nothing

        'Dim mailclient = New SmtpClient()
        'mailclient.Host = "smtpauth.earthlink.net"
        'mailclient.Port = 587

        '' This is the critical part, you must enable SSL  
        'mailclient.EnableSsl = True

        '' authentication details for logging on to the smtpclient...my interserv account for now... 
        'mailclient.Credentials = New System.Net.NetworkCredential("wbartel@interserv.com", "deft5gave")
        'mailclient.Send(mail)
    End Function
#End Region

    Private ReadOnly Property MyActiveLogin() As ActiveLoginInfo
        Get
            If MySubscriberOps IsNot Nothing Then
                Return MySubscriberOps.ActiveLogin
            Else
                MySubscriberOps = New SubscriberOps_NS.SubscriberOperations
                MySubscriberOps.GetActiveLoginInfo(MyCSH.EndPtSuffix)
                Return MySubscriberOps.ActiveLogin
            End If
        End Get
    End Property

#Region "SetUp PgItemColxn and CustomerDBSvc Classes"
    Private Function SetUpPgItemColxnSvc(ByVal _SurveyId As Integer) As Boolean
        Dim rslt As Boolean = False
        Try
            If Not IsNothing(Me.MyCSH) Then
                With Me.MyCSH
                    Me.MyEptSuffix = .EndPtSuffix
                    Dim dcnxnstring = .DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyId)
                    .LastInanceCreatedDateTime = Date.Now
                    .LastOperationDateTime = Date.Now
                    .InstanceCount += 1
                    .LastSurveyID = _SurveyId
                    If dcnxnstring IsNot Nothing Then
                        'MyCSH.DataContextConnectionString = dcnxnstring
                        MyPgItemColxnSvc = New PageItemColxnServicesLibr.PgItemColxnSvc
                        MyPgItemColxnSvc.DC_ConnectionString = dcnxnstring
                        MyPgItemColxnSvc.MyCSH = MyCSH
                        rslt = True
                    Else
                        Using EvLog As New EventLog()
                            EvLog.Source = "AuthoringSvc"
                            EvLog.Log = "Application"
                            EvLog.WriteEntry("SetUpPgItemColxnSvc: EptSuffix " & Me.MyEptSuffix & " SurveyID " & _SurveyId.ToString & " Returns False ", EventLogEntryType.FailureAudit)
                        End Using
                        MyPgItemColxnSvc = Nothing
                        dcnxnstring = Nothing
                        rslt = False
                    End If
                End With
            Else
                Using EvLog As New EventLog()
                    EvLog.Source = "AuthoringSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("SetUpPgItemColxnSvc:Reports MyCSH isNothing   ", EventLogEntryType.Error)
                End Using
                MyPgItemColxnSvc = Nothing
                rslt = False
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SetUpPgItemColxnSvc: EptSuffix " & Me.MyEptSuffix & " SurveyID " & _SurveyId.ToString & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
            MyPgItemColxnSvc = Nothing
            rslt = False
        End Try
        Return rslt
    End Function

    Private Function SetUpCustomerDBSvc() As Boolean
        Dim rslt As Boolean = False
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If MySubscriberOps Is Nothing Then
                MySubscriberOps = New SubscriberOps_NS.SubscriberOperations
                MySubscriberOps.GetActiveLoginInfo(MyCSH.EndPtSuffix)
            End If

            Dim dcnxnstring = MyCSH.DataContextConnectionString
            If dcnxnstring IsNot Nothing Then
                MyCustomerDBSvc = New CustomerDBSvc.CustomerDBSvc
                MyCustomerDBSvc.DC_ConnectionString = dcnxnstring
                MyCustomerDBSvc.MyCSH = MyCSH
                rslt = True
            Else
                Using EvLog As New EventLog()
                    EvLog.Source = "AuthoringSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("SetUpCustomerDBSvc: EptSuffix " & Me.MyEptSuffix & " Returns False ", EventLogEntryType.FailureAudit)
                End Using
                MyCustomerDBSvc = Nothing
                rslt = False
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SetUpCustomerDBSvc: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
            MyCustomerDBSvc = Nothing
            rslt = False
        End Try
        Return rslt
    End Function
#End Region

#Region "Retrieve Methods"

    Public Function RetrieveResultsFilterModelImagesRFMID(ByVal _SurveyID As Integer, ByVal _RFMID As Integer) As SurveyImagesPackage Implements _
    IAuthoringSvc.RetrieveResultsFilterModelImagesRFMID
        Dim rslt As SurveyImagesPackage = Nothing
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If SetUpCustomerDBSvc() Then
                rslt = MyCustomerDBSvc.RetrieveResultsFilterModelImagesRFMID(_SurveyID, _RFMID)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RetrieveResultsFilterModelImagesRFMID: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try
        Return rslt
    End Function

    Public Function RetrieveSurveyImagesPackagePCMID(ByVal _SurveyID As Integer, ByVal _PCMID As Integer) As SurveyImagesPackage Implements IAuthoringSvc.RetrieveSurveyImagesPackagePCMID
        Dim rslt As SurveyImagesPackage = Nothing
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If SetUpCustomerDBSvc() Then
                rslt = MyCustomerDBSvc.RetrieveSurveyImagesPackagePCMID(_SurveyID, _PCMID)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RetrieveSurveyImagesPackagePCMID: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try
        Me.MyCSH = Nothing
        Return rslt
    End Function

    Public Function RetrieveSurveyImagesColxn_SequenceNumber(ByVal _SurveyID As Integer, ByVal _SequenceNumber As Integer) As SurveyImagesPackage _
    Implements IAuthoringSvc.RetrieveSurveyImagesColxn_SequenceNumber
        Dim rslt As SurveyImagesPackage = Nothing
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If SetUpCustomerDBSvc() Then
                rslt = MyCustomerDBSvc.RetrieveSurveyImagesPackageSequenceNumber(_SurveyID, _SequenceNumber)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RetrieveSurveyImagesColxn_SequenceNumber: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try
        Return rslt
    End Function

    Public Function RetrieveSurveyImagesColxnAll(ByVal _SurveyID As Integer) As SurveyImagesPackage Implements IAuthoringSvc.RetrieveSurveyImagesColxnAll
        Dim rslt As SurveyImagesPackage = Nothing
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If SetUpCustomerDBSvc() Then
                rslt = MyCustomerDBSvc.RetrieveSurveyImagesPackageALLImages(_SurveyID)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("RetrieveSurveyImagesColxnAll: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try
        Return rslt
    End Function



    Public Function Retrieve_TinySurveyRowWithModel_List(ByVal _LoginEmail As String) _
    As List(Of Tiny_Survey_RowWith_SIModel) _
    Implements IAuthoringSvc.Retrieve_TinySurveyRowWithModel_List
        Dim rslt As List(Of Tiny_Survey_RowWith_SIModel) = Nothing
        If SetUpCustomerDBSvc() Then
            rslt = New List(Of Tiny_Survey_RowWith_SIModel)
            For Each kvp In MyCSH.DC_Pkg.Survey_DC_List
                Dim tsr = MyCustomerDBSvc.RetrieveSurveyRow_WithSurveyID(kvp.Key)
                tsr.TinyRow.PrivilegeBitMask = MyCSH.DC_Pkg.Retrieve_PrivilegeBitMask(kvp.Key)
                rslt.Add(tsr)
            Next
            MyCustomerDBSvc = Nothing
        End If
        Return rslt
    End Function

    Public Function Retrieve_SurveyMetaData_withModel_List(ByVal _LoginEmail As String) As List(Of SurveyMetadataPackage) _
    Implements IAuthoringSvc.Retrieve_SurveyMetaData_withModel_List
        Dim rslt As List(Of SurveyMetadataPackage) = Nothing
        Try
            If SetUpCustomerDBSvc() Then
                rslt = New List(Of SurveyMetadataPackage)
                For Each kvp In MyCSH.DC_Pkg.Survey_DC_List
                    Dim smp = MyCustomerDBSvc.SurveyMetaData_withModel(kvp.Key)
                    smp.TinyRow.PrivilegeBitMask = MyCSH.DC_Pkg.Retrieve_PrivilegeBitMask(kvp.Key)
                    rslt.Add(smp)
                Next
                MyCustomerDBSvc.MyCSH = Nothing
                MyCustomerDBSvc = Nothing
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_SurveyMetaData_withModel_List: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try

        Return rslt
    End Function
    Public Function Retrieve_SurveyMetaData_List(ByVal _LoginEmail As String) As List(Of SurveyMetadataPackage) _
    Implements IAuthoringSvc.Retrieve_SurveyMetaData_List
        Dim rslt As List(Of SurveyMetadataPackage) = Nothing
        Try
            If SetUpCustomerDBSvc() Then
                rslt = New List(Of SurveyMetadataPackage)
                For Each kvp In MyCSH.DC_Pkg.Survey_DC_List
                    Dim smp = MyCustomerDBSvc.SurveyMetaData(kvp.Key)
                    smp.TinyRow.PrivilegeBitMask = MyCSH.DC_Pkg.Retrieve_PrivilegeBitMask(kvp.Key)
                    rslt.Add(smp)
                Next
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_SurveyMetaData_List: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try
        Return rslt
    End Function
    Public Function Retrieve_SurveyMetaData(ByVal _SurveyID As Integer) As SurveyMetadataPackage _
    Implements IAuthoringSvc.Retrieve_SurveyMetaData
        Dim rslt As SurveyMetadataPackage = Nothing
        Try
            If SetUpCustomerDBSvc() Then
                If MyCSH.DC_Pkg.Survey_DC_List.Where(Function(k) k.Key = _SurveyID).Count > 0 Then
                    rslt = MyCustomerDBSvc.SurveyMetaData(_SurveyID)
                    rslt.TinyRow.PrivilegeBitMask = MyCSH.DC_Pkg.Retrieve_PrivilegeBitMask(_SurveyID)
                    If SetUpPgItemColxnSvc(_SurveyID) Then
                        rslt.MetaDataColxn.AddRange(MyPgItemColxnSvc.Retrieve_PCMIDandPageNumbersList(_SurveyID).AsEnumerable)
                    End If
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_SurveyMetaData: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
        Return rslt
    End Function

    Public Function Retrieve_ActiveOrNewResultsSMDList(ByVal _LoginEmail As String) As List(Of SurveyMetadataPackage) Implements IAuthoringSvc.Retrieve_ActiveOrNewResultsSMDList
        Dim rslt As List(Of SurveyMetadataPackage) = Nothing
        Try
            If SetUpCustomerDBSvc() Then
                rslt = MyCustomerDBSvc.ActiveRDentsSMDList
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_ActiveRdentsSMDList: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            Me.DisposeMe()
        End Try
        Return rslt
    End Function

    Public Function Retrieve_PageBlob_Count(ByVal _SurveyID As Integer) _
    As Integer _
    Implements IAuthoringSvcNS.IAuthoringSvc.Retrieve_PageBlob_Count
        Dim rslt As Integer = 0
        Try
            If SetUpPgItemColxnSvc(_SurveyID) Then
                rslt = MyPgItemColxnSvc.Retrieve_Page_Count(_SurveyID)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_PageBlob_Count: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
        Return rslt
    End Function

    Public Function Retrieve_PageBlobInfo_WithIndex(ByVal _SurveyID As Integer, ByVal _IndexOfPage As Integer) _
    As PageBlobInfo _
    Implements IAuthoringSvcNS.IAuthoringSvc.Retrieve_PageBlobInfo_WithIndex
        Dim rslt As PageBlobInfo = Nothing
        Try
            If SetUpPgItemColxnSvc(_SurveyID) Then
                rslt = MyPgItemColxnSvc.Retrieve_PageBlobInfo_WithIndex(_SurveyID, _IndexOfPage)
                'else
                'throw...
                MyPgItemColxnSvc.MyCSH = Nothing
                MyPgItemColxnSvc = Nothing
            End If
        Catch ex As Exception
            'do some error logging here...
        End Try
        Return rslt
    End Function

    Public Function Retrieve_Page(ByVal _SurveyID As Integer, ByVal _IndexOfPage As Integer) _
    As Page_Package _
    Implements IAuthoringSvc.Retrieve_Page
        Dim rslt As Page_Package = Nothing
        Try
            If SetUpPgItemColxnSvc(_SurveyID) Then
                rslt = MyPgItemColxnSvc.Retrieve_Page(_SurveyID, _IndexOfPage) 'PgCount is populated in this method....
                rslt.SurveyID = _SurveyID
                If Not IsNothing(rslt) AndAlso SetUpCustomerDBSvc() Then
                    Dim imglist = MyCustomerDBSvc.RetrieveImagePCElemIDsList(_SurveyID, rslt.PgContentModelPkg.PCM_SDSID)
                    If Not IsNothing(imglist) Then
                        For Each pcelemid In imglist
                            Dim xpcelemid = pcelemid
                            If Not IsNothing(rslt.PCElement_Pkg_Colxn) Then
                                Dim pcelempkg = rslt.PCElement_Pkg_Colxn.Where(Function(p) p.PCE_SDSID = xpcelemid).FirstOrDefault
                                If Not IsNothing(pcelempkg) Then
                                    pcelempkg.HasImage = True
                                End If
                            End If
                        Next
                        imglist = Nothing
                    End If
                End If
            End If

        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_Page: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
        Return rslt
    End Function

    Public Function Retrieve_PCMIDandPageNumberList(ByVal _SurveyID As Integer) As List(Of Srlzd_KVP)
        Dim rslt As List(Of Srlzd_KVP) = Nothing
        Try
            If SetUpPgItemColxnSvc(_SurveyID) Then
                rslt = MyPgItemColxnSvc.Retrieve_PCMIDandPageNumbersList(_SurveyID)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_PCMIDandPageNumberList: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
        Return rslt
    End Function
#End Region

#Region "Save Methods"
    Public Sub SaveResultsFilterModelImages(ByVal _SurveyImgsPkg As SurveyImagesPackage) Implements IAuthoringSvc.SaveResultsFilterModelImages
        Try
            If SetUpCustomerDBSvc() Then
                MyCustomerDBSvc.PopulateResultsFilterModelImages(_SurveyImgsPkg)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SaveResultsFilterModelImages: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
    End Sub

    Public Function SaveSurveyItemModel(ByVal _SurveyItem_Pkg As SurveyItem_Package) _
    As POCO_ID_Pkg _
    Implements IAuthoringSvc.SaveSurveyItemModel
        Dim pocoIDPkg As POCO_ID_Pkg = Nothing
        'could be an existing survey...
        Try
            If SetUpCustomerDBSvc() Then
                If MyCSH.DC_Pkg.DC_Cnxn_For_SurveyID(_SurveyItem_Pkg.SIM_SDSID) IsNot Nothing Then

                    MyCustomerDBSvc.Update_SurveyMaster_WithModel(_SurveyItem_Pkg.SIM_SDSID, _SurveyItem_Pkg.SurveyName)
                    pocoIDPkg = New POCO_ID_Pkg With {.Survey_ID = _SurveyItem_Pkg.SIM_SDSID, _
                                                      .POCOGuid = _SurveyItem_Pkg.MyGuid, _
                                                      .DB_ID = _SurveyItem_Pkg.SIM_SDSID, _
                                                      .Original_ID = _SurveyItem_Pkg.SIM_SDSID}
                Else
                    'could be a new survey
                    If MySubscriberOps IsNot Nothing Then
                        Dim nsp As New NewSurveyPackage With {.SurveyItemPkg = _SurveyItem_Pkg, _
                                                              .SubScriberInfo = Me.MyActiveLogin.SubscrInfo}
                        MySubscriberOps.AddNewSurvey(nsp)
                        pocoIDPkg = nsp.POCO_Pkg
                        'need a DC_Cnxn_For_SurveyID to put in MyCSH.DC_Pkg.....
                        MyCSH.DC_Pkg.Survey_DC_List.Add(nsp.Survey_DC_List_Item)
                    End If
                    'pocoIDPkg = MyCustomerDBSvc.AddSurveyRow(_SurveyItem_Pkg) <<<<<THIS WAS THE BEFORE I CREATED SubscriberOperations...
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SaveSurveyItemModel: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try

        Return pocoIDPkg
    End Function

    Public Sub SaveSurveyImagesColxn(ByVal _SurveyImagesPkg As SurveyImagesPackage) Implements IAuthoringSvc.SaveSurveyImagesColxn
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If SetUpCustomerDBSvc() Then
                MyCustomerDBSvc.PopulateSurveyImages(_SurveyImagesPkg)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SaveSurveyImagesColxn: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
    End Sub

    Public Function SavePage(ByVal _Page_Pkg As Page_Package) As List(Of POCO_ID_Pkg) _
    Implements IAuthoringSvc.SavePage
        Dim ListofPOCOID_pkg As New List(Of POCO_ID_Pkg)
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If SetUpPgItemColxnSvc(_Page_Pkg.SurveyID) Then   'found a SurveyID

                Dim PIMpocoIDPkg = MyPgItemColxnSvc.SavePageItemModel(_Page_Pkg.PgItemModelPkg, _Page_Pkg.SurveyID)
                ListofPOCOID_pkg.Add(PIMpocoIDPkg)

                Dim PCMpocoIDpkg = MyPgItemColxnSvc.SavePageContentModel(_Page_Pkg.PgContentModelPkg, PIMpocoIDPkg.DB_ID, _Page_Pkg.SurveyID)
                ListofPOCOID_pkg.Add(PCMpocoIDpkg)

                Dim pcelempocoidpkgs = MyPgItemColxnSvc.SavePageContentElementsList(_Page_Pkg.PCElement_Pkg_Colxn, PCMpocoIDpkg.DB_ID, _Page_Pkg.SurveyID)
                ListofPOCOID_pkg.AddRange(pcelempocoidpkgs)

                'For Each pcelem_Pkg In _Page_Pkg.PCElement_Pkg_Colxn
                '    Dim PCElempocoIDPkg = MyPgItemColxnSvc.SavePageContentElement(pcelem_Pkg, PCMpocoIDpkg.DB_ID, _Page_Pkg.SurveyID)
                '    ListofPOCOID_pkg.Add(PCElempocoIDPkg)
                'Next

            Else
                'should log an error...
                SharedEvents.RaiseOperationFailed(Me, "SavePage..CouldNotFindSurveyID= " & _Page_Pkg.SurveyID.ToString)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SavePage: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
        Return ListofPOCOID_pkg
    End Function

    'Public Function SavePageItemModel(ByVal _PageItemModel As IPageItemsSvcNS.PgItemModel) _
    'As POCO_ID_Pkg _
    'Implements IAuthoringSvcNS.IAuthoringSvc.SavePageItemModel
    '    Dim rslt As POCO_ID_Pkg = Nothing
    '    If SetUpPgItemColxnSvc(_PageItemModel.SurveyID) Then
    '        rslt = MyPgItemColxnSvc.SavePageItemModel(_PageItemModel)
    '        'add surveyID to MyCSH.DC_Colxn 
    '        If not MyCSH.DC_Pkg.Survey_DC_List.Contains(
    '        End If
    '    End If
    '    Return rslt
    'End Function

    'Public Function SavePageContentModel(ByVal _PageContentModel As IPageItemsSvcNS.PageContentModel) _
    'As POCO_ID_Pkg _
    'Implements IAuthoringSvcNS.IAuthoringSvc.SavePageContentModel
    '    Dim rslt As POCO_ID_Pkg = Nothing
    '    'If SetUpPgItemColxnSvc(_PageContentModel.
    '    Return rslt
    'End Function

    'Public Function SavePageContentElement(ByVal _PageContentElement As IPageItemsSvcNS.PageContentElement) _
    'As POCO_ID_Pkg _
    'Implements IAuthoringSvcNS.IAuthoringSvc.SavePageContentElement
    '    Dim rslt As POCO_ID_Pkg = Nothing
    '    Return rslt
    'End Function
#End Region

#Region "Publish Methods"
    Public Function ChangeSurveyStatus(ByVal _ChangeSurveyStatePkg As ChangeSurveyStatePackage) As Integer Implements IAuthoringSvc.ChangeSurveyStatus
        Dim rslt As Integer = 9 '_ChangeSurveyStatePkg.CurrentState.
        Try
            If MyActiveLogin IsNot Nothing Then
                Select Case _ChangeSurveyStatePkg.TargetState
                    Case SurveyState.PublishedAccepting
                        If MySubscriberOps IsNot Nothing Then
                            If MyActiveLogin.CanPublish Then
                                Dim PubSrvyPkg = New PublishSurveyPackage(MyActiveLogin.SubscrInfo.NormalizedEmailAddress, _
                                                                                         _ChangeSurveyStatePkg.SurveyID, False)
                                'PubSrvyPkg.SDS_ResponseInfoList = _ChangeSurveyStatePkg.SDS_ResponseInfoList
                                If MySubscriberOps.PublishSurvey(PubSrvyPkg) Then
                                    rslt = _ChangeSurveyStatePkg.TargetState
                                End If
                            End If
                        End If
                    Case SurveyState.PublishedClosed
                        If MySubscriberOps IsNot Nothing Then
                            If MyActiveLogin.CanPublish Then
                                If MySubscriberOps.UnPublishSurvey(_ChangeSurveyStatePkg.SurveyID) Then
                                    rslt = SurveyState.PublishedClosed
                                End If
                            End If
                        End If
                End Select
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("ChangeSurveyStatus: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
        Return rslt
    End Function

    'Public Function SaveResponseModels(ByVal _ChangeSurveyStatePkg As ChangeSurveyStatePackage) As Boolean 'posting svc creates SDS_ResponseModels as required...
    '    'If _ChangeSurveyStatePkg.SDS_ResponseInfoList IsNot Nothing AndAlso _ChangeSurveyStatePkg.SDS_ResponseInfoList.Count > 0 Then
    '    '    If SetUpPgItemColxnSvc(_ChangeSurveyStatePkg.SurveyID) Then
    '    '        MyPgItemColxnSvc.
    '    '    End If
    '    'End If
    'End Function

#End Region

#Region "GuestLogin and SurveyPrivilegeMethods"
    Public Function Retrieve_SurveyPrivilegeModels_List() As List(Of SurveyPrivilegeModel) Implements IAuthoringSvc.Retrieve_SurveyPrivilegeModels_List
        Dim rslt As New List(Of SurveyPrivilegeModel)
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If MyActiveLogin IsNot Nothing Then
                If MyActiveLogin.IsLoginAdministrator Then
                    rslt.AddRange(MyActiveLogin.SubscrInfo.SurveyPrivileges)
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_SurveyPrivilegeModels_List: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try

        Return rslt
    End Function

    Public Function Update_SurveyPrivilegeModels(ByVal _SurveyPrivilegesList As List(Of SurveyPrivilegeModel)) As List(Of SurveyPrivilegeModel) Implements _
    IAuthoringSvc.Update_SurveyPrivilegeModels
        Dim rslt As New List(Of SurveyPrivilegeModel)
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If MyActiveLogin IsNot Nothing Then
                If MyActiveLogin.IsLoginAdministrator Then
                    If MySubscriberOps IsNot Nothing Then
                        MySubscriberOps.ModifySurveyLoginPrivileges(_SurveyPrivilegesList, MyActiveLogin.SubscrInfo)
                        MySubscriberOps.RefreshSubscriber_SurveyPrivileges() 'this method updates mySubscriberOps.MyActiveLogin.SubscriberInfo...
                    End If
                    rslt.AddRange(MyActiveLogin.SubscrInfo.SurveyPrivileges) 'this retrieves its value from mySubscriberOps.MyActiveLogin...
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Update_SurveyPrivilegeModels: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try

        Return rslt
    End Function

    Public Function Retrieve_GuestLoginInfo_List() As List(Of GuestLoginInfo) Implements IAuthoringSvc.Retrieve_GuestLoginInfo_List
        Dim rslt As New List(Of GuestLoginInfo)
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If MyActiveLogin IsNot Nothing Then
                If MyActiveLogin.IsLoginAdministrator Then
                    rslt.AddRange(MyActiveLogin.SubscrInfo.Guest_LoginInfo_List)
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_GuestLoginInfo_List: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
        Return rslt
    End Function

    Public Function Add_GuestLoginInfo(ByVal _EmailAddress As String) As List(Of GuestLoginInfo) Implements IAuthoringSvc.Add_GuestLoginInfo
        Dim rslt As New List(Of GuestLoginInfo)
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If MyActiveLogin IsNot Nothing Then
                If MyActiveLogin.IsLoginAdministrator Then
                    If MySubscriberOps IsNot Nothing Then
                        Dim nlp As New NewLoginPackage(_EmailAddress)
                        nlp.IsSubscriberAddedGuestLogin = True
                        nlp.SubScriberInfo = MyActiveLogin.SubscrInfo
                        MySubscriberOps.AddNewLoginToExistingSubscriber(nlp)
                        MySubscriberOps.RefreshSubscriber_GuestLoginInfo_List() 'this method updates mySubscriberOps.MyActiveLogin.SubscriberInfo...
                    End If
                    rslt.AddRange(MyActiveLogin.SubscrInfo.Guest_LoginInfo_List) 'this retrieves its value from mySubscriberOps.MyActiveLogin...
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Add_GuestLoginInfo: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try

        Return rslt
    End Function

    Public Function Update_LoginInfo_PrivBitmask(ByVal _TargetGuestLoginInfo As GuestLoginInfo) As Boolean Implements IAuthoringSvc.Update_LoginInfo_PrivBitmask
        Dim rslt As Boolean = False
        Return rslt
    End Function

    Public Function Retrieve_RDentLoginInfo_List() As List(Of GuestLoginInfo) 'Implements IAuthoringSvc.Retrieve_GuestLoginInfo_List
        Dim rslt As New List(Of GuestLoginInfo)
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If MyActiveLogin IsNot Nothing Then
                If MyActiveLogin.IsLoginAdministrator Then
                    rslt.AddRange(MyActiveLogin.SubscrInfo.RdentLoginInfo_List)
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Retrieve_RDentLoginInfo_List: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
        Return rslt
    End Function
#End Region

#Region "ChangeMyPwdPlease"
    Public Function ChangeMyPwdPlease(ByVal _PwdPack As Password_Package) As Boolean Implements IAuthoringSvc.ChangeMyPwdPlease
        Dim rslt As Boolean = False
        Try
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If MySubscriberOps IsNot Nothing Then
                rslt = MySubscriberOps.ChangeMyPwdPlease(_PwdPack)
            Else
                MySubscriberOps = New SubscriberOps_NS.SubscriberOperations
                rslt = MySubscriberOps.ChangeMyPwdPlease(_PwdPack)
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "AuthoringSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("ChangeMyPwdPlease: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
            End Using
        Finally
            DisposeMe()
        End Try
        Return rslt
    End Function
#End Region
End Class
