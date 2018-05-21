Imports System.Data.SqlClient
Imports CmdInfrastructureNS
Imports ICustomerDBSvc
Imports CmdSvcClassLibrary
Imports WCFServiceManagerNS


Public Class CustomerDBSvc
    Implements ICustomerDBSvc.ICustomerDBSvc, IDisposable

    Public Property DC_ConnectionString As String
    Public Property MyCSH As CmdSvcClassLibrary.CustomSvcHost = Nothing 'CType(OperationContext.Current.Host, CmdSvcClassLibrary.CustomSvcHost)
    Private Property MyEptSuffix As String = ""
    'Private InstanceTracker As CmdSvcClassLibrary.InstanceTracker = Nothing '(MyCSH)

    'Public Sub SetCache(ByVal _LoginEmail As String) Implements ICustomerDBSvc.ICustomerDBSvc.SetCache
    '    InstanceTracker.TrackMethod("SetCache", 0)
    '    MyCSH.SvcMonitor.Update_ServiceCalls("SetCache... " & _LoginEmail, DateTime.Now.ToLongTimeString)

    '    MyCSH.Cache_Pkg = New CmdSvcClassLibrary.Cache_Package(Retrieve_ComputerPrivileges_Pkg(_LoginEmail), 0, 0)

    'End Sub
    '#Region "PrivilegeServiceMappings"
    '    ''' <summary>
    '    ''' Returns a Survey_Priv_DC_Colxn_Pkg that contains the ServiceTypeEnumBitMask and the DC_Package
    '    ''' </summary>
    '    ''' <param name="_LoginID"></param>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public Function GetPrivServiceMappings(ByVal _LoginID As Integer) As Survey_Priv_DC_Colxn_Pkg
    '        Dim conn = New SqlConnection(DC_ConnectionString)
    '        Dim da = New SqlDataAdapter("proc_GetSurveyIDsAndPrivilegeIDsByLoginID", conn)
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.Add("@LoginID", SqlDbType.Int)
    '        da.SelectCommand.Parameters("@LoginID").Value = _LoginID
    '        conn.Open()
    '        Dim ds = New DataSet()
    '        da.Fill(ds)

    '        Dim _surveyPrivilegeMappings = ds.Tables(0)
    '        'da.Fill(_surveyPrivilegeMappings)
    '        Dim rslt As New Survey_Priv_DC_Colxn_Pkg

    '        'ServiceEnumBitMsk
    '        Dim ServiceEnumBitMask As ULong = 0
    '        Dim privEnumlist As New List(Of ULong)
    '        For Each dr In _surveyPrivilegeMappings.DefaultView.ToTable(True, "PrivilegeID").Rows
    '            privEnumlist.Add(dr("PrivilegeID")) 'this should be a privilegebitmask
    '        Next

    '        rslt.ServiceEnumBitMsk = EnumBitMaskOperations.EvaluatePrivilegeUsingListofUlong(privEnumlist)

    '        Dim _surveyID_DbName = ds.Tables(1)
    '        rslt.Survey_DC_List = (From dr In _surveyID_DbName.DefaultView _
    '                                                            Let Cnxstr = CnxnString_DatabaseName(dr("DatabaseName")) _
    '                                                            Let SurvID = dr("SurveyID") _
    '                                                            Select New Srlzd_KVP With {.Key = SurvID, _
    '                                                                                       .Valu = Cnxstr}).ToList
    '        rslt.Survey_Privilege_List = (From dr1 In _surveyPrivilegeMappings.DefaultView _
    '                                    Select New Srlzd_KVP With {.Key = dr1("SurveyID"), _
    '                                                               .Valu = dr1("PrivilegeID")}).ToList


    '        conn.Close()
    '        conn.Dispose()
    '        da.Dispose()
    '        conn = Nothing
    '        da = Nothing
    '        Return rslt
    '    End Function
    '#End Region



    'Public Function Retrieve_QueueInfo(ByVal _SurveyID As Integer) As String _
    'Implements ICustomerDBSvc.ICustomerDBSvc.Retrieve_QueueInfo
    '    Dim rslt As String = "NotSet"
    '    InstanceTracker.TrackMethod("Retrieve_QueueInfo", _SurveyID)
    '    Return rslt
    'End Function

    'Public Function Retrieve_ComputerPrivileges_Pkg(ByVal _LoginEmail As String) _
    'As ComputerPrivilegePackage _
    'Implements ICustomerDBSvc.ICustomerDBSvc.Retrieve_ComputerPrivileges_Pkg

    '    Dim rslt As New ComputerPrivilegePackage
    '    If MyCSH.Cache_Pkg IsNot Nothing AndAlso _
    '     MyCSH.Cache_Pkg.Cache.GetType.Equals(GetType(ComputerPrivilegePackage)) Then
    '        InstanceTracker.TrackMethod("Retrieve_ComputerPrivileges_Pkg FROM CACHE", 0)
    '        'this should look at email...make sure its the right cache...should be on this endpoint...
    '        Return MyCSH.Cache_Pkg.Cache
    '    Else
    '        InstanceTracker.TrackMethod("Retrieve_ComputerPrivileges_Pkg", 0)

    '        Try
    '            Dim db As New L2S_CustomerSurveyMasterDBDataContext(CnxnStringFromCSH)
    '            'MyCSH.SvcMonitor.Update_ServiceCalls("Retrieve_ComputerPrivileges_Pkg " & _LoginEmail, _
    '            '                                     DateTime.Now.ToLongTimeString)
    '            Dim q = From lsp In db.LoginSurveyPrivileges _
    '                    Where lsp.LoginID = (From li In db.LoginInfos _
    '                                         Where li.LoginEmail = _LoginEmail _
    '                                         Select li.LogInID).Single _
    '                    Select lsp.SurveyID, lsp.PrivilegeID

    '            Dim q1 = From item In q, sm In db.SurveyMasters _
    '                     Where item.SurveyID = sm.SurveyID _
    '                     Select sm.SurveyDataStoreID, item.PrivilegeID Distinct

    '            Dim q2 = From item In q1.Distinct, sds In db.SurveyDataStores _
    '                     Where item.SurveyDataStoreID = sds.SurveyDataStoreID _
    '                     Select sds.ComputerID, item.PrivilegeID Distinct

    '            '<<<<THESE PRIVILEGEIDS AND THESE COMPUTERIDS HAVE TO BE THE SAME IDS AS ON GLOBALSURVEY MASTER...
    '            'unless we do this query...crosswalks CustomerMasterDB ID's to GlobalSurveyMasterID's
    '            Dim q3 = From item In q2, comp In db.ComputerInfos, priv In db.Privileges _
    '                     Where item.ComputerID = comp.ComputerID AndAlso item.PrivilegeID = priv.PriviligeID _
    '                     Select comp.GlobalComputerID, priv.GlobalPrivilegeID Distinct

    '            rslt.CompPrivilegeList = (From item In q3 _
    '                            Select New ComputerPrivilegePackage.CompPrivilegeItem _
    '                            With {.ComputerID = item.GlobalComputerID, _
    '                                  .PrivilegeID = item.GlobalPrivilegeID}).ToList
    '            'MyCSH.SvcMonitor.Update_ServiceCalls("Retrieve_ComputerPrivileges_Pkg " & _LoginEmail, _
    '            '                                     "ListCount = " & rslt.CompPrivilegeList.Count & DateTime.Now.ToLongTimeString)
    '            Try
    '                'db.Dispose()
    '            Catch ex As Exception
    '                MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP Retrieve_ComputerPrivileges_Pkg Try db.dispose " & _LoginEmail & ex.Message, _
    '                                               DateTime.Now.ToLongTimeString)
    '            End Try

    '        Catch ex As Exception
    '            MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP Retrieve_ComputerPrivileges_Pkg " & _LoginEmail & ex.Message, _
    '                                                 DateTime.Now.ToLongTimeString)
    '        End Try
    '    End If
    '    Return rslt
    'End Function

    ''this needs to return absolutepaths for every survey this login email has privileges to....see EndPtDataContextService...
    'Public Function Retrieve_DC_PackageByVal(ByVal _LogIn_Email As String, ByVal _computerID As Integer) As DC_Package _
    'Implements ICustomerDBSvc.ICustomerDBSvc.Retrieve_DC_Package

    '    Dim rslt As New DC_Package
    '    Dim db As New L2S_CustomerSurveyMasterDBDataContext(CnxnStringFromCSH)
    '    'this needs to look at LSP...all the surveys this loginemail has privileges to...

    '    Dim q = From lsp In db.LoginSurveyPrivileges _
    '                  Where lsp.LoginID = (From li In db.LoginInfos _
    '                                       Where li.LoginEmail = _LogIn_Email _
    '                                       Select li.LogInID).Single _
    '                  Select lsp.SurveyID, lsp.PrivilegeID

    '    Dim q1 = From item In q, sm In db.SurveyMasters _
    '                    Where item.SurveyID = sm.SurveyID _
    '                    Select sm.SurveyDataStoreID, sm.SurveyID Distinct

    '    Dim q2 = From item In q1, sds In db.SurveyDataStores _
    '                    Where item.SurveyDataStoreID = sds.SurveyDataStoreID _
    '                    AndAlso sds.ComputerID = _computerID _
    '                    Select New Srlzd_KVP(item.SurveyID, sds.AbsolutePath) Distinct

    '    rslt.Survey_DC_List = q2.ToList

    '    Try
    '        db.Dispose()
    '    Catch ex As Exception
    '        MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP db.dispose Populate_DCPkg " & DateTime.Now.ToLongTimeString, _
    '                                             "(LogIn_Email=<" & _LogIn_Email & ">" & ex.Message)

    '    End Try
    '    Return rslt
    'End Function
    Private Function IamOk() As Boolean
        Dim rslt As Boolean = False
        If Not IsNothing(Me.MyCSH) Then
            Me.MyEptSuffix = Me.MyCSH.EndPtSuffix
            If Not IsNothing(Me.DC_ConnectionString) Then
                rslt = True
            Else
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("IamOk: EptSuffix " & Me.MyEptSuffix & " DC_ConnectionString isNothing ", EventLogEntryType.Error)
                End Using
            End If

        Else
            Me.MyEptSuffix = "Unknown"
            Using EvLog As New EventLog()
                EvLog.Source = "CustomerDBSvc"
                EvLog.Log = "Application"
                EvLog.WriteEntry("IamOk: EptSuffix " & Me.MyEptSuffix & " MyCSH isNothing ", EventLogEntryType.Error)
            End Using
        End If
        Return rslt
    End Function

#Region "ImageManagerMethods"
    Public Function RetrieveManagerImage(ByVal _guidstr As String, _imgsize As Integer) As Byte()
        Dim rslt As Byte() = Nothing
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim ba = (From imgrow In db.ImagesStores _
                            Where imgrow.PermPCMGuidString = _guidstr AndAlso imgrow.Format = _imgsize AndAlso imgrow.SurveyID = 0 AndAlso imgrow.PCMID = 0 _
                            Select imgrow.ByteArray).FirstOrDefault
                    If Not IsNothing(ba) Then
                        rslt = ba.ToArray()
                    End If
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveManagerImage: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function

    Public Function RetrieveManagerGuidStrList(_imgsize As Integer) As List(Of String)
        Dim rslt As List(Of String) = Nothing
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim imgguidstrs = From imgrow In db.ImagesStores _
                            Where imgrow.Format = _imgsize AndAlso imgrow.SurveyID = 0 AndAlso imgrow.PCMID = 0 _
                            Select imgrow.PermPCMGuidString
                    If Not IsNothing(imgguidstrs) Then
                        rslt = imgguidstrs.ToList
                    End If
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveManagerImage: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function

    Public Function AddManagerImages(ByVal _mgrimgspkg As SurveyImagesPackage) As Boolean
        Dim rslt As Boolean = False
        If IamOk() Then
            Try
                If Not IsNothing(_mgrimgspkg) AndAlso Not IsNothing(_mgrimgspkg.ImageStorePkgList) Then
                    Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                        db.ImagesStores.InsertAllOnSubmit(From ipkitem In _mgrimgspkg.ImageStorePkgList _
                                     Select New ImagesStore With {.ByteArray = ipkitem.ByteArray, _
                                                                  .Format = ipkitem.ImgFormat, _
                                                                  .Height = ipkitem.Height, _
                                                                  .PCElementID = ipkitem.PCElemID, _
                                                                  .PCMID = ipkitem.PCMID, _
                                                                  .PermPCMGuidString = ipkitem.PermPCMGuidString, _
                                                                  .SequenceNumber = ipkitem.SeqNumber, _
                                                                  .SurveyID = ipkitem.SurveyID, _
                                                                  .Width = ipkitem.Width})
                        db.SubmitChanges()
                    End Using
                    rslt = True
                End If
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("PopulateSurveyImages: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function
    Public Function RemoveManagerImages(ByVal _guidstr As String) As Boolean
        Dim rslt As Boolean = False
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim imgrows = (From imgrow In db.ImagesStores _
                            Where imgrow.PermPCMGuidString = _guidstr AndAlso imgrow.SurveyID = 0 AndAlso imgrow.PCMID = 0 _
                            Select imgrow)
                    If Not IsNothing(imgrows) Then
                        db.ImagesStores.DeleteAllOnSubmit(imgrows.AsEnumerable)
                        rslt = True
                    End If
                End Using
                rslt = True
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RemoveManagerImage: EptSuffix " & Me.MyEptSuffix & " _guidstr= " & _guidstr & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function
#End Region

#Region "ResultsFilterModelImages Stuff"
    Public Function RetrieveResultsFilterModelImagesRFMID(ByVal _SurveyID As Integer, ByVal _RFMID As Integer) As SurveyImagesPackage
        Dim rslt As New SurveyImagesPackage With {.SIM_SDSID = _SurveyID, _
                                            .ImageStorePkgList = New List(Of ImageStorePackage)}
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    rslt.ImageStorePkgList.AddRange(From imgrow In db.ImagesStores _
                                                    Where imgrow.SurveyID = _SurveyID AndAlso imgrow.PCMID = _RFMID AndAlso imgrow.PCElementID = -2 _
                                                    Select New ImageStorePackage With {.ByteArray = imgrow.ByteArray.ToArray, _
                                                                                       .PCElemID = imgrow.PCElementID, _
                                                                        .Height = imgrow.Height, _
                                                                        .ImageID = imgrow.ImageID, _
                                                                        .ImgFormat = imgrow.Format, _
                                                                        .PCMID = imgrow.PCMID, _
                                                                        .PermPCMGuidString = imgrow.PermPCMGuidString, _
                                                                        .SeqNumber = imgrow.SequenceNumber, _
                                                                        .SurveyID = imgrow.SurveyID, _
                                                                        .Width = imgrow.Width})
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveResultsFilterModelImagesRFMID: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If

        Return rslt
    End Function

    Public Function PopulateResultsFilterModelImages(ByVal _SurveyImgsPkg As SurveyImagesPackage) As Boolean
        Dim rslt As Boolean = False
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim existingimgrows As IQueryable(Of ImagesStore)
                    Dim pcmid = _SurveyImgsPkg.ImageStorePkgList.FirstOrDefault.PCMID 'this is resultsfiltermodel.ID...
                    existingimgrows = From imgrow In db.ImagesStores _
                                      Where imgrow.SurveyID = _SurveyImgsPkg.SIM_SDSID AndAlso imgrow.PCMID = pcmid AndAlso imgrow.PCElementID = -2 _
                                      Select imgrow

                    For Each imgrow In existingimgrows
                        Dim ximgrow = imgrow
                        Dim ipkitem = _SurveyImgsPkg.ImageStorePkgList.Where(Function(ipk) ipk.SeqNumber = ximgrow.SequenceNumber).FirstOrDefault
                        If ipkitem IsNot Nothing Then
                            ximgrow.ByteArray = ipkitem.ByteArray
                            ximgrow.Height = ipkitem.Height
                            ximgrow.Width = ipkitem.Width
                            ximgrow.SequenceNumber = ipkitem.SeqNumber
                            _SurveyImgsPkg.ImageStorePkgList.Remove(ipkitem)
                        Else
                            db.ImagesStores.DeleteOnSubmit(imgrow)
                        End If
                        ximgrow = Nothing
                        ipkitem = Nothing
                    Next
                    db.ImagesStores.InsertAllOnSubmit(From ipkitem In _SurveyImgsPkg.ImageStorePkgList _
                                 Select New ImagesStore With {.ByteArray = ipkitem.ByteArray, _
                                                              .Format = ipkitem.ImgFormat, _
                                                              .Height = ipkitem.Height, _
                                                              .PCElementID = -2, _
                                                              .PCMID = ipkitem.PCMID, _
                                                              .PermPCMGuidString = ipkitem.PermPCMGuidString, _
                                                              .SequenceNumber = ipkitem.SeqNumber, _
                                                              .SurveyID = _SurveyImgsPkg.SIM_SDSID, _
                                                              .Width = ipkitem.Width})
                    db.SubmitChanges()
                    existingimgrows = Nothing
                    pcmid = Nothing
                End Using
                rslt = True
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("PopulateResultsFilterModelImages: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function
#End Region

#Region "SurveyImagesStuff"
    Public Function RetrieveImagePCElemIDsList(_SurveyID As Integer, _PCMID As Integer) As List(Of Integer)
        Dim rslt As New List(Of Integer)
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    rslt.AddRange(From imgrow In db.ImagesStores _
                                                    Where imgrow.SurveyID = _SurveyID AndAlso imgrow.PCMID = _PCMID AndAlso imgrow.PCElementID > 0 _
                                                    Select imgrow.PCElementID.GetValueOrDefault)
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveImagePCElemIDsList: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function


    Public Function RetrieveImagePCElem(ByVal _SurveyID As Integer, ByVal _PCMID As Integer, _PCElemID As Integer) As SurveyImagesPackage
        Dim rslt As New SurveyImagesPackage With {.SIM_SDSID = _SurveyID, _
                                                  .ImageStorePkgList = New List(Of ImageStorePackage)}
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    rslt.ImageStorePkgList.AddRange(From imgrow In db.ImagesStores _
                                                    Where imgrow.SurveyID = _SurveyID AndAlso imgrow.PCMID = _PCMID AndAlso imgrow.PCElementID = _PCElemID _
                                                    Select New ImageStorePackage With {.ByteArray = imgrow.ByteArray.ToArray, _
                                                                                       .PCElemID = imgrow.PCElementID, _
                                                                        .Height = imgrow.Height, _
                                                                        .ImageID = imgrow.ImageID, _
                                                                        .ImgFormat = imgrow.Format, _
                                                                        .PCMID = imgrow.PCMID, _
                                                                        .PermPCMGuidString = imgrow.PermPCMGuidString, _
                                                                        .SeqNumber = imgrow.SequenceNumber, _
                                                                        .SurveyID = imgrow.SurveyID, _
                                                                        .Width = imgrow.Width})
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveImagePCElem: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function


    Public Function RetrieveSurveyImagesPackagePCMID(ByVal _SurveyID As Integer, ByVal _PCMID As Integer) As SurveyImagesPackage
        Dim rslt As New SurveyImagesPackage With {.SIM_SDSID = _SurveyID, _
                                                  .ImageStorePkgList = New List(Of ImageStorePackage)}
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    rslt.ImageStorePkgList.AddRange(From imgrow In db.ImagesStores _
                                                    Where imgrow.SurveyID = _SurveyID AndAlso imgrow.PCMID = _PCMID AndAlso imgrow.PCElementID > -1 _
                                                    Select New ImageStorePackage With {.ByteArray = imgrow.ByteArray.ToArray, _
                                                                                       .PCElemID = imgrow.PCElementID, _
                                                                        .Height = imgrow.Height, _
                                                                        .ImageID = imgrow.ImageID, _
                                                                        .ImgFormat = imgrow.Format, _
                                                                        .PCMID = imgrow.PCMID, _
                                                                        .PermPCMGuidString = imgrow.PermPCMGuidString, _
                                                                        .SeqNumber = imgrow.SequenceNumber, _
                                                                        .SurveyID = imgrow.SurveyID, _
                                                                        .Width = imgrow.Width})
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveSurveyImagesPackagePCMID: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function

    Public Function RetrieveSurveyImagesPackageSequenceNumber(ByVal _SurveyID As Integer, ByVal _SequenceNumber As Integer) As SurveyImagesPackage
        Dim rslt As New SurveyImagesPackage With {.SIM_SDSID = _SurveyID, _
                                                  .ImageStorePkgList = New List(Of ImageStorePackage)}
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    rslt.ImageStorePkgList.Add((From imgrow In db.ImagesStores _
                                                    Where imgrow.SurveyID = _SurveyID AndAlso imgrow.SequenceNumber = _SequenceNumber AndAlso imgrow.PCElementID = -1 _
                                                    Select New ImageStorePackage With {.ByteArray = imgrow.ByteArray.ToArray, _
                                                                        .Height = imgrow.Height, _
                                                                        .ImageID = imgrow.ImageID, _
                                                                        .ImgFormat = imgrow.Format, _
                                                                        .PCMID = imgrow.PCMID, _
                                                                        .PermPCMGuidString = imgrow.PermPCMGuidString, _
                                                                        .SeqNumber = imgrow.SequenceNumber, _
                                                                        .SurveyID = imgrow.SurveyID, _
                                                                        .Width = imgrow.Width}).FirstOrDefault)
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveSurveyImagesPackageSequenceNumber: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function

    Public Function RetrieveSurveyImagesPackageALLImages(ByVal _SurveyID As Integer) As SurveyImagesPackage
        Dim rslt As New SurveyImagesPackage With {.SIM_SDSID = _SurveyID, _
                                                  .ImageStorePkgList = New List(Of ImageStorePackage)}
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    rslt.ImageStorePkgList.AddRange(From imgrow In db.ImagesStores _
                                                    Where imgrow.SurveyID = _SurveyID AndAlso imgrow.PCElementID = -1 _
                                                    Select New ImageStorePackage With {.ByteArray = imgrow.ByteArray.ToArray, _
                                                                        .Height = imgrow.Height, _
                                                                        .ImageID = imgrow.ImageID, _
                                                                        .ImgFormat = imgrow.Format, _
                                                                        .PCMID = imgrow.PCMID, _
                                                                        .PermPCMGuidString = imgrow.PermPCMGuidString, _
                                                                        .SeqNumber = imgrow.SequenceNumber, _
                                                                        .SurveyID = imgrow.SurveyID, _
                                                                        .Width = imgrow.Width})
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveSurveyImagesPackageALLImages: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function

    Public Function PopulateSurveyImages(ByVal _SurveyImgsPkg As SurveyImagesPackage) As Boolean
        Dim rslt As Boolean = False
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim firstimg = 0  '-1 indicates surveyimages, value > 0 is a pagecontentelement imageview...
                    If _SurveyImgsPkg.ImageStorePkgList.FirstOrDefault IsNot Nothing Then
                        firstimg = _SurveyImgsPkg.ImageStorePkgList.FirstOrDefault.PCElemID
                    End If

                    Dim existingimgrows As IQueryable(Of ImagesStore)
                    If firstimg < 0 Then
                        existingimgrows = From imgrow In db.ImagesStores _
                                          Where imgrow.SurveyID = _SurveyImgsPkg.SIM_SDSID AndAlso imgrow.PCElementID = -1 _
                                          Select imgrow
                    Else
                        Dim pcmid = 0 ' _SurveyImgsPkg.ImageStorePkgList.FirstOrDefault.PCMID
                        If _SurveyImgsPkg.ImageStorePkgList.FirstOrDefault IsNot Nothing Then
                            pcmid = _SurveyImgsPkg.ImageStorePkgList.FirstOrDefault.PCMID
                        End If
                        existingimgrows = From imgrow In db.ImagesStores _
                                          Where imgrow.SurveyID = _SurveyImgsPkg.SIM_SDSID AndAlso imgrow.PCMID = pcmid AndAlso imgrow.PCElementID > -1 _
                                          Select imgrow

                    End If

                    For Each imgrow In existingimgrows
                        Dim ximgrow = imgrow
                        Dim ipkitem = _SurveyImgsPkg.ImageStorePkgList.Where(Function(ipk) ipk.PermPCMGuidString = ximgrow.PermPCMGuidString AndAlso ximgrow.PCElementID = ipk.PCElemID).FirstOrDefault
                        If ipkitem IsNot Nothing Then
                            ximgrow.ByteArray = ipkitem.ByteArray
                            ximgrow.Height = ipkitem.Height
                            ximgrow.Width = ipkitem.Width
                            ximgrow.SequenceNumber = ipkitem.SeqNumber
                            ximgrow.Format = ipkitem.ImgFormat
                            _SurveyImgsPkg.ImageStorePkgList.Remove(ipkitem)
                        Else
                            db.ImagesStores.DeleteOnSubmit(imgrow)
                        End If
                        ximgrow = Nothing
                    Next
                    db.ImagesStores.InsertAllOnSubmit(From ipkitem In _SurveyImgsPkg.ImageStorePkgList _
                                                      Where ipkitem.ByteArray IsNot Nothing _
                                 Select New ImagesStore With {.ByteArray = ipkitem.ByteArray, _
                                                              .Format = ipkitem.ImgFormat, _
                                                              .Height = ipkitem.Height, _
                                                              .PCElementID = ipkitem.PCElemID, _
                                                              .PCMID = ipkitem.PCMID, _
                                                              .PermPCMGuidString = ipkitem.PermPCMGuidString, _
                                                              .SequenceNumber = ipkitem.SeqNumber, _
                                                              .SurveyID = _SurveyImgsPkg.SIM_SDSID, _
                                                              .Width = ipkitem.Width})
                    db.SubmitChanges()
                    existingimgrows = Nothing
                    firstimg = Nothing
                End Using
                rslt = True
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("PopulateSurveyImages: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If

        Return rslt
    End Function
#End Region

    Public Function Tiny_Survey_Row_List_UsingLogInEmail(ByVal _LoginEmail As String, ByVal _PrivColxn As List(Of PriviligeDescrEnum)) _
    As List(Of Tiny_Survey_RowWith_SIModel) _
    Implements ICustomerDBSvc.ICustomerDBSvc.Tiny_Survey_Row_List_UsingLogInEmail

        Dim rslt As List(Of Tiny_Survey_RowWith_SIModel) = Nothing
        '' InstanceTracker.TrackMethod("Tiny_Survey_Row_List_UsingLogInEmai LoginEmail=<" & _LoginEmail & "> ", 0)
        'Dim db As New L2S_CustomerSurveyMasterDBDataContext 'context should always be set to GlobalSurveyMaster...
        'Try

        '    Dim q = From lsp In db.LoginSurveyPrivileges, priv In db.Privileges _
        '            Where lsp.LoginID = (From li In db.LoginInfos _
        '                                  Where li.LoginEmail = _LoginEmail _
        '                                  Select li.LogInID).Single _
        '            AndAlso _PrivColxn.Contains(priv.PrivilegeDescription) _
        '            Select lsp.SurveyID, lsp.PrivilegeID

        '    Dim q1 = From item In q, sm In db.SurveyMasters _
        '                    Where item.SurveyID = sm.SurveyID _
        '                    Select sm.SurveyDataStoreID, sm.SurveyID, sm.SurveyDescription, sm.SurveyStateID, sm.SurveyType Distinct

        '    Dim q2 = From item In q1, sds In db.SurveyDataStores _
        '                    Where item.SurveyDataStoreID = sds.SurveyDataStoreID _
        '                    Select New Tiny_Survey_RowWith_SIModel With {.Model = New Object, _
        '                                                                 .TinyRow = New Tiny_Survey_Row _
        '                                                                 With {.ComputerID = sds.ComputerID, _
        '                                                                       .QueueName = "NotSet", _
        '                                                                       .QueueUri = "NotSet", _
        '                                                                       .SurveyID = item.SurveyID, _
        '                                                                       .SurveyName = item.SurveyDescription, _
        '                                                                       .SurveyStateID = item.SurveyStateID, _
        '                                                                       .SurveyType = item.SurveyType}}
        '    rslt = q2.ToList
        'Catch ex As Exception
        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP TinySurveyRow_UsingLogInEmail" & ex.Message, _
        '                                     "LoginEmail=<" & _LoginEmail & "> " & DateTime.Now.ToLongTimeString)
        'End Try
        'MyCSH.SvcMonitor.Update_ServiceCalls("TinySurveyRow_UsingLogInEmail" & "LoginEmail=<" & _LoginEmail & "> ", _
        '                                     DateTime.Now.ToLongTimeString)


        Return rslt
    End Function

    Public Function RetrieveSurveyRow_WithSurveyID(ByVal _SurveyID As Integer) _
    As Tiny_Survey_RowWith_SIModel _
    Implements ICustomerDBSvc.ICustomerDBSvc.RetrieveSurveyRow_WithSurveyID
        Dim rslt As Tiny_Survey_RowWith_SIModel = Nothing
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)

                    Dim q1 = From sm In db.SurveyMasters _
                             Where sm.SurveyID = _SurveyID _
                             Select sm.SurveyDataStoreID, sm.SurveyID, sm.SurveyDescription, sm.Model, sm.SurveyStateID, sm.SurveyType Distinct
                    Dim q2 = From item In q1, sds In db.SurveyDataStores _
                             Where item.SurveyDataStoreID = sds.SurveyDataStoreID _
                             Select New Tiny_Survey_RowWith_SIModel With {.Model = item.Model, _
                                                                          .TinyRow = New Tiny_Survey_Row _
                                                                                     With {.ComputerID = sds.ComputerID, _
                                                                                           .QueueName = "NotSet", _
                                                                                           .QueueUri = "NotSet", _
                                                                                           .SurveyID = item.SurveyID, _
                                                                                           .SurveyName = item.SurveyDescription, _
                                                                                           .SurveyStateID = item.SurveyStateID, _
                                                                                           .SurveyType = item.SurveyType}}
                    rslt = q2.Single
                    q1 = Nothing
                    q2 = Nothing
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RetrieveSurveyRow_WithSurveyID: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function

    Public Function Update_SurveyMaster_WithModel(ByVal _SurveyId As Integer, ByVal _SurveyName As String) _
    As Boolean _
    Implements ICustomerDBSvc.ICustomerDBSvc.Update_SurveyMaster_WithModel
        Dim rslt As Boolean = False
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim q = From si In db.SurveyMasters _
                            Where si.SurveyID = _SurveyId _
                            Select si
                    Dim ur = q.Single
                    ur.LastModifiedDate = Date.Now
                    ur.SurveyDescription = _SurveyName
                    db.SubmitChanges()
                    q = Nothing
                    ur = Nothing
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Update_SurveyMaster_WithModel: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function


    Public Function AddSurveyRow(ByVal _SurveyItem_Pkg As SurveyItem_Package) As POCO_ID_Pkg _
    Implements ICustomerDBSvc.ICustomerDBSvc.AddSurveyRow

        Dim rslt As POCO_ID_Pkg = Nothing
        'If IamOk() Then

        'End If
        'Try
        '    Dim orginalID As Integer = _SurveyItem_Pkg.SIM_SDSID
        '    Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
        '        Dim _SurveyMaster As New SurveyMaster With {.Model = "", _
        '                                                    .SurveyDescription = _SurveyItem_Pkg.SurveyName, _
        '                                                    .CreatedDate = Date.Now, _
        '                                                    .LastModifiedDate = Date.Now, _
        '                                                    .QueueComputer = 0, _
        '                                                    .QueueName = "NotSet", _
        '                                                    .QueueURI = "NotSet"}

        '        db.SurveyMasters.InsertOnSubmit(_SurveyMaster)

        '        db.SubmitChanges()
        '        rslt = New POCO_ID_Pkg With {.Survey_ID = _SurveyMaster.SurveyID, _
        '                                     .Original_ID = orginalID, _
        '                                     .DB_ID = _SurveyMaster.SurveyID, _
        '                                     .POCOGuid = _SurveyItem_Pkg.MyGuid}
        '        'Dim newLSP As New LoginSurveyPrivilege With {.SurveyID = _SurveyMaster.SurveyID, .PrivilegeID = 7, .LoginID = 8}
        '        ''AddLoginSurveyPrivilegeRow(_SurveyMaster.SurveyID)
        '        'db.LoginSurveyPrivileges.InsertOnSubmit(newLSP)
        '        'db.SubmitChanges()
        '    End Using
        'Catch ex As Exception
        '    'MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.AddSurveyRow reports exception..." & DateTime.Now.ToLongTimeString, _
        '    '                                   "SurveyRow(SurveyID=<" & _SurveyItem_Pkg.MyGuid.ToString & ">)" & ex.Message)
        'End Try
        Return rslt
    End Function
    'Private Function AddLoginSurveyPrivilegeRow(ByVal _SurveyID As Integer) As Boolean
    '    Dim rslt As Boolean = False
    '    Dim db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
    '    'Dim localloginid = From li In db.LoginInfos _
    '    '                  Where li.gsmloginID = MyCSH.GSM_LoginID _  <<<<<NEED GSMLoginID in LoginInfos...
    '    '                  Select li

    '    Dim newLSP As New LoginSurveyPrivilege With {.SurveyID = _SurveyID, .PrivilegeID = 7, .LoginID = 8}
    '    Return rslt
    'End Function


    Public Function RemoveSurveyRow(ByVal _SurveyID As Integer) As Boolean _
    Implements ICustomerDBSvc.ICustomerDBSvc.RemoveSurveyRow

        ' InstanceTracker.TrackMethod("RemoveSurveyRow", _SurveyID)

        Dim rslt As Boolean = False
        'Dim db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
        'MyCSH.SvcMonitor.Update_ServiceCalls("RemoveSurveyRow  " & DateTime.Now.ToLongTimeString, _
        '                                  "SurveyID=<" & _SurveyID.ToString & ">")
        'Try
        '    Dim q = From si In db.SurveyMasters _
        '        Where si.SurveyID = _SurveyID _
        '        Select si
        '    Dim dr = q.Single
        '    db.SurveyMasters.DeleteOnSubmit(dr)
        '    db.SubmitChanges()
        '    rslt = True
        'Catch ex As Exception
        '    MyCSH.SvcMonitor.Update_ServiceCalls("BLOWEDUP CustomerDBSvc.RemoveSurveyRow reports more than one...  " & DateTime.Now.ToLongTimeString, _
        '                                       "SurveyRow(SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)
        'End Try
        ''Try
        ''    db.Dispose()
        ''Catch ex As Exception
        ''    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.RemoveSurveyRow Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, _
        ''                                       "SurveyRow(SurveyID=<" & _SurveyID.ToString & ">)" & ex.Message)
        'db = Nothing
        ''End Try
        Return rslt
    End Function
    'DOES NOT RETRIEVE IMAGES....
    Public Function SurveyMetaData(ByVal _SurveyID As Integer) _
    As SurveyMetadataPackage _
    Implements ICustomerDBSvc.ICustomerDBSvc.SurveyMetaData
        Dim rslt As SurveyMetadataPackage = Nothing
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim q1 = (From sm In db.SurveyMasters _
                             Where sm.SurveyID = _SurveyID _
                             Select sm).FirstOrDefault
                    If Not IsNothing(q1) Then
                        rslt = New SurveyMetadataPackage
                        rslt.MetaDataColxn = (From prop In q1.GetType.GetProperties _
                                             Where prop.PropertyType Is GetType(Date) Or prop.PropertyType Is GetType(Date?) Or prop.PropertyType Is GetType(Integer) Or prop.PropertyType Is GetType(Boolean) _
                                             Select New Srlzd_KVP With {.Key = prop.Name, .Valu = prop.GetValue(q1, Nothing)}).ToList
                        If q1.SurveyStateID = SurveyState.PublishedAccepting Then
                            Dim rdentlogin = (From li In db.LoginInfos, lsp In db.LoginSurveyPrivileges _
                                                Where li.PrivBitMask = PriviligeDescrEnum.Respondent AndAlso (lsp.SurveyID = _SurveyID AndAlso lsp.LoginID = li.LogInID) _
                                                Select li.LoginEmail).FirstOrDefault
                            If Not IsNothing(rdentlogin) Then
                                Dim kvp = Me.GetActiveRespondents(rdentlogin)
                                rslt.MetaDataColxn.Add(kvp)
                                If kvp.Valu > 0 Then
                                    rslt.MetaDataColxn.Add(New Srlzd_KVP With {.Key = "HasActiveRDents", .Valu = kvp.Valu})
                                End If
                            End If
                            rdentlogin = Nothing
                        Else
                            rslt.MetaDataColxn.Add(New Srlzd_KVP With {.Key = "ActiveRDentsComputed", .Valu = 0})
                        End If
                        Dim LastRdentPosted = q1.LastRespondentPostedDate
                        Dim LastRsltView = q1.LastStatisticsViewerDate
                        Dim isNewResult As Boolean = False
                        If Not IsNothing(LastRdentPosted) Then
                            If Not IsNothing(LastRsltView) Then
                                If Date.Compare(LastRdentPosted, LastRsltView) > 0 Then
                                    isNewResult = True
                                End If
                            Else
                                isNewResult = True
                            End If
                        End If
                        If isNewResult Then
                            rslt.MetaDataColxn.Add(New Srlzd_KVP With {.Key = "HasNewResults", .Valu = 1})
                        End If
                        rslt.TinyRow = New Tiny_Survey_Row With {.SurveyID = _SurveyID, .SurveyName = q1.SurveyDescription, .SurveyStateID = q1.SurveyStateID, .SurveyType = q1.SurveyType}
                        q1 = Nothing
                    End If
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("SurveyMetaData: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function

    Private Const RespProviderDxnryKey As String = "https://leases/hm/RespProviderSvcLibr.RespProviderSvc/"
    Private Function GetActiveRespondents(_email As String) As Srlzd_KVP
        Dim rslt As New Srlzd_KVP With {.Key = "ActiveRDentsComputed", .Valu = 0}
        If Not IsNothing(_email) Then
            Try
                If Not IsNothing(WCFServiceManagerNS.WCFServiceManager.ServicesDxnry) Then
                    With WCFServiceManagerNS.WCFServiceManager.ServicesDxnry
                        Dim RespProvmycsh As CustomSvcHost = Nothing
                        If Not IsNothing(Me.MyCSH) Then
                            Dim dxnrykey = Nothing
                            dxnrykey = RespProviderDxnryKey & _email

                            If Not IsNothing(dxnrykey) Then
                                If WCFServiceManager.ServicesDxnry.TryGetValue(dxnrykey, RespProvmycsh) Then
                                    rslt.Valu = RespProvmycsh.RDentCountInLastTTLTime
                                End If
                            End If
                            dxnrykey = Nothing
                        End If
                        RespProvmycsh = Nothing
                    End With
                Else
                    Using EvLog As New EventLog()
                        EvLog.Source = "CustomerDBSvc"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("GetActiveRespondents: EptSuffix " & Me.MyEptSuffix & " Reports WCFSvcMgr.ServiceDxnry isNothing... ", EventLogEntryType.Error)
                    End Using
                End If
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("GetActiveRespondents: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function

    Public Function ActiveRDentsSMDList() As List(Of SurveyMetadataPackage)
        Dim rslt As List(Of SurveyMetadataPackage) = Nothing
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    For Each sm In db.SurveyMasters.Where(Function(s) s.SurveyStateID = SurveyState.PublishedAccepting)
                        Dim xsm = sm
                        Dim LastRdentPosted = sm.LastRespondentPostedDate
                        Dim LastRsltView = sm.LastStatisticsViewerDate
                        Dim isNewResult As Boolean = False
                        If Not IsNothing(LastRdentPosted) Then
                            If Not IsNothing(LastRsltView) Then
                                If Date.Compare(LastRdentPosted, LastRsltView) > 0 Then
                                    isNewResult = True
                                End If
                            Else
                                isNewResult = True
                            End If
                        End If
                        Dim hasActiveRdents As Boolean = False
                        Dim _SurveyID = sm.SurveyID

                        Dim rdentlogin = (From li In db.LoginInfos, lsp In db.LoginSurveyPrivileges _
                                            Where li.PrivBitMask = PriviligeDescrEnum.Respondent AndAlso (lsp.SurveyID = _SurveyID AndAlso lsp.LoginID = li.LogInID) _
                                            Select li.LoginEmail).FirstOrDefault
                        Dim skvp As Srlzd_KVP = Nothing
                        If Not IsNothing(rdentlogin) Then
                            skvp = Me.GetActiveRespondents(rdentlogin)
                            If Not IsNothing(skvp) AndAlso skvp.Valu > 0 Then
                                hasActiveRdents = True
                            End If
                            rdentlogin = Nothing
                        End If
                        If isNewResult Or hasActiveRdents Then
                            Dim smdPkg = New SurveyMetadataPackage
                            smdPkg.MetaDataColxn = New List(Of Srlzd_KVP)
                            'smdPkg.MetaDataColxn = (From prop In sm.GetType.GetProperties _
                            '                     Where prop.PropertyType Is GetType(Date) Or prop.PropertyType Is GetType(Date?) Or prop.PropertyType Is GetType(Integer) Or prop.PropertyType Is GetType(Boolean) _
                            '                     Select New Srlzd_KVP With {.Key = prop.Name, .Valu = prop.GetValue(xsm, Nothing)}).ToList
                            smdPkg.TinyRow = New Tiny_Survey_Row With {.SurveyID = _SurveyID, .SurveyName = xsm.SurveyDescription, .SurveyStateID = xsm.SurveyStateID, .SurveyType = xsm.SurveyType}

                            If hasActiveRdents AndAlso Not IsNothing(skvp) Then
                                smdPkg.MetaDataColxn.Add(skvp)
                                smdPkg.MetaDataColxn.Add(New Srlzd_KVP With {.Key = "HasActiveRDents", .Valu = skvp.Valu})
                            Else
                                smdPkg.MetaDataColxn.Add(New Srlzd_KVP With {.Key = "ActiveRDentsComputed", .Valu = 0})
                            End If
                            If isNewResult Then
                                smdPkg.MetaDataColxn.Add(New Srlzd_KVP With {.Key = "HasNewResults", .Valu = 1})
                            End If
                            If IsNothing(rslt) Then
                                rslt = New List(Of SurveyMetadataPackage)
                            End If
                            rslt.Add(smdPkg)
                        End If
                        xsm = Nothing
                    Next
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("ActiveRDentsSMDList: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function


    Public Function SurveyMetaData_withModel(ByVal _SurveyID As Integer) As SurveyMetadataPackage
        Dim rslt As SurveyMetadataPackage = Nothing
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim q1 = (From sm In db.SurveyMasters _
                             Where sm.SurveyID = _SurveyID _
                             Select sm).FirstOrDefault
                    If Not IsNothing(q1) Then
                        rslt = New SurveyMetadataPackage
                        rslt.MetaDataColxn = (From prop In q1.GetType.GetProperties _
                                             Where prop.PropertyType Is GetType(Date) Or prop.PropertyType Is GetType(Date?) Or prop.PropertyType Is GetType(Integer) Or prop.PropertyType Is GetType(Boolean) _
                                             Select New Srlzd_KVP With {.Key = prop.Name, .Valu = prop.GetValue(q1, Nothing)}).ToList
                        rslt.TinyRow = New Tiny_Survey_Row With {.SurveyID = _SurveyID, .SurveyName = q1.SurveyDescription, .SurveyStateID = q1.SurveyStateID, .SurveyType = q1.SurveyType}
                        rslt.Model = Me.RetrieveSurveyImagesPackageALLImages(_SurveyID)
                    End If
                    q1 = Nothing
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("SurveyMetaData_withModel: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
        Return rslt
    End Function

    Public Function SurveyMetaData_List(ByVal _LoginID As Integer) _
    As List(Of SurveyMetadataPackage) _
    Implements ICustomerDBSvc.ICustomerDBSvc.SurveyMetaData_List

        Dim rslt As List(Of SurveyMetadataPackage) = Nothing
        'If IamOk() Then
        '    Dim db As New L2S_CustomerSurveyMasterDBDataContext
        '    MyCSH.SvcMonitor.Update_ServiceCalls("SurveyMetaData" & DateTime.Now.ToLongTimeString, _
        '                                        "LoginID=<" & _LoginID.ToString & ">)")
        '    Try
        '        'use reflection.propertyinfor where type is datetime...
        '        'put the property name, and the value in a cmdSvcLibr.srlzd_KVP...
        '    Catch ex As Exception

        '    End Try

        '    'Try
        '    '    db.Dispose()
        '    'Catch ex As Exception
        '    '    ServiceMonitor.Update_ServiceCalls("BLOWEDUP MasterDBSvc.SurveyMetaData Try db.Dispose reports exception..." & DateTime.Now.ToLongTimeString, _
        '    '                                       "LoginID=<" & _LoginID.ToString & ">)" & ex.Message)
        '    db = Nothing
        '    'End Try
        'End If

        Return rslt
    End Function


#Region "Update MetaDataStuff from PostingSvc"
    Public Sub RDentStartedSurvey(ByVal _SurveyID As Integer)
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim sm = db.SurveyMasters.Where(Function(s) s.SurveyID = _SurveyID).FirstOrDefault
                    If Not IsNothing(sm) Then
                        'sm.ActiveRespondentsCount = sm.ActiveRespondentsCount + 1
                        sm.Model = Me.MyCSH.EndPtSuffix
                        sm.RespondenStartedtCount = sm.RespondenStartedtCount + 1
                        sm.LastRespondentStartedDate = Date.Now
                        db.SubmitChanges()
                    End If
                    sm = Nothing
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RDentStartedSurvey: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
    End Sub

    Public Sub RDentCompletedSurvey(ByVal _SurveyID As Integer)
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim sm = db.SurveyMasters.Where(Function(s) s.SurveyID = _SurveyID).FirstOrDefault
                    If Not IsNothing(sm) Then
                        'sm.ActiveRespondentsCount = sm.ActiveRespondentsCount - 1
                        sm.RespondentCompletedCount = sm.RespondentCompletedCount + 1
                        sm.LastRespondentCompletedDate = Date.Now
                        db.SubmitChanges()
                        sm = Nothing
                    End If
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("RDentCompletedSurvey: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
    End Sub

    Public Sub UpdatePostingMetaData(ByVal _SurveyID As Integer)
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim sm = db.SurveyMasters.Where(Function(s) s.SurveyID = _SurveyID).FirstOrDefault
                    If Not IsNothing(sm) Then
                        sm.ResponsePostingSvcIsActive = True
                        sm.LastRespondentPostedDate = Date.Now
                        If sm.FirstRespondentPostedDate Is Nothing Then
                            sm.FirstRespondentPostedDate = Date.Now
                        End If
                        db.SubmitChanges()
                        sm = Nothing
                    End If
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("UpdatePostingMetaData: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
    End Sub

    Public Sub UpdateResltsViewedMetaData(ByVal _SurveyID As Integer, ByVal _ViewerIsActive As Boolean)
        If IamOk() Then
            Try
                Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                    Dim sm = db.SurveyMasters.Where(Function(s) s.SurveyID = _SurveyID).FirstOrDefault
                    If Not IsNothing(sm) Then
                        sm.StatisticsViewerSvcIsActive = _ViewerIsActive
                        If _ViewerIsActive Then
                            sm.LastStatisticsViewerDate = Date.Now
                        End If

                        If sm.FirstStatisticsViewerDate Is Nothing Then
                            sm.FirstStatisticsViewerDate = Date.Now
                        End If
                        db.SubmitChanges()
                        sm = Nothing
                    End If
                End Using
            Catch ex As Exception
                Using EvLog As New EventLog()
                    EvLog.Source = "CustomerDBSvc"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("UpdateResltsViewedMetaData: EptSuffix " & Me.MyEptSuffix & " Reports exception " & ex.Message, EventLogEntryType.Error)
                End Using
            End Try
        End If
    End Sub
#End Region

#Region "CnxnString Methods"
    'Public Function CnxnString_SurveyID(ByVal _SurveyID As Integer) As String
    '    Dim rslt As String = "NotSet"
    '    Dim db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
    '    Dim q = (From sm In db.SurveyMasters, sds In db.SurveyDataStores _
    '            Where sm.SurveyID = _SurveyID AndAlso sm.SurveyDataStoreID = sds.SurveyDataStoreID _
    '            Select sds.AbsolutePath).SingleOrDefault

    '    rslt = CnxnString_AbsolutePath(q)
    '    Return rslt
    'End Function
    'Public Function CnxnString_DatabaseName(ByVal _DatabaseName As String) As String
    '    Dim rslt As String = "NotSet"
    '    Dim builder As New SqlConnectionStringBuilder
    '    builder.InitialCatalog = _DatabaseName
    '    builder.IntegratedSecurity = True
    '    builder.DataSource = ".\DEVRENTS"
    '    builder.UserInstance = False
    '    builder.LoadBalanceTimeout = 30
    '    rslt = builder.ConnectionString
    '    builder = Nothing
    '    Return rslt
    'End Function
    'Public Function CnxnString_AbsolutePath(ByVal _AbsolutePath As String) As String
    '    Dim rslt As String = "NotSet"
    '    Dim builder As New SqlConnectionStringBuilder
    '    builder.AttachDBFilename = _AbsolutePath
    '    builder.IntegratedSecurity = True
    '    builder.DataSource = ".\DEVRENTS"
    '    builder.UserInstance = False
    '    builder.LoadBalanceTimeout = 30
    '    rslt = builder.ConnectionString
    '    builder = Nothing
    '    Return rslt
    'End Function
    'Public Function CnxnStringFromCSH() As String
    '    Dim rslt As String
    '    rslt = MyCSH.DataContextConnectionString
    '    Return rslt
    'End Function
    'Public Sub SetCSHCnxnString_AbsolutePath(ByVal _AbsolutePath As String)
    '    MyCSH.DataContextConnectionString = CnxnString_AbsolutePath(_AbsolutePath)
    'End Sub
    'Public Sub SetCSHCnxnString_SurveyID(ByVal _SurveyID As Integer)
    '    MyCSH.DataContextConnectionString = CnxnString_SurveyID(_SurveyID)
    'End Sub
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Me.MyCSH = Nothing
                Me.DC_ConnectionString = Nothing
                Me.MyEptSuffix = Nothing
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
