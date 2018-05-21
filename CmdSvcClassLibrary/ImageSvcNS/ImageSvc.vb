Imports MasterDBSvcLibr
Imports IImageSvcNS
Imports WCFServiceManagerNS
Imports System.IO
Imports IResultsProviderSvcNS
Imports CmdSvcClassLibrary
Imports RsltsProviderSvcLibr
Imports AuthoringSvcNS
Imports CmdInfrastructureNS
Imports System.ServiceModel.web

Public Class ImageSvc
    Implements IImageSvc

#Region "ImageManagerMethods"
    Public Function ManagerGuidStringsPlease(ByVal email As String) As List(Of String) Implements IImageSvc.ManagerGuidStringsPlease
        Dim rslt As List(Of String) = Nothing
        Try
            If UserCacheManager.IsLoggedIn(email) Then
                If Me.SetUpCustomerDB(email) Then
                    rslt = MyCustomerDBSvc.RetrieveManagerGuidStrList(3)
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ImageSvc"
                EvLog.Log = ""
                EvLog.WriteEntry("ManagerGuidStringsPlease: email=" & email & " Reports exception " & ex.Message, EventLogEntryType.Warning)
            End Using
        End Try

        Return rslt
    End Function
    Public Function ManagerImagePlease(ByVal email As String, ByVal guidstr As String) As Stream Implements IImageSvc.ManagerImagePlease
        Dim rslt As Stream = Nothing
        Try
            If UserCacheManager.IsLoggedIn(email) Then
                If Me.SetUpCustomerDB(email) Then
                    rslt = New MemoryStream(MyCustomerDBSvc.RetrieveManagerImage(guidstr, 0))
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ImageSvc"
                EvLog.Log = ""
                EvLog.WriteEntry("ManagerImagePlease: email=" & email & " sid=" & guidstr & " Reports exception " & ex.Message, EventLogEntryType.Warning)
            End Using
        End Try

        Return rslt
    End Function
    Public Function ManagerLargeImagePlease(ByVal email As String, ByVal guidstr As String) As Stream Implements IImageSvc.ManagerLargeImagePlease
        Dim rslt As Stream = Nothing
        Try
            If UserCacheManager.IsLoggedIn(email) Then
                If Me.SetUpCustomerDB(email) Then
                    rslt = New MemoryStream(MyCustomerDBSvc.RetrieveManagerImage(guidstr, 1))
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ImageSvc"
                EvLog.Log = ""
                EvLog.WriteEntry("ManagerLargeImagePlease: email=" & email & " sid=" & guidstr & " Reports exception " & ex.Message, EventLogEntryType.Warning)
            End Using
        End Try

        Return rslt
    End Function
    Public Function ManagerSmallImagePlease(ByVal email As String, ByVal guidstr As String) As Stream Implements IImageSvc.ManagerSmallImagePlease
        Dim rslt As Stream = Nothing
        Try
            If UserCacheManager.IsLoggedIn(email) Then
                If Me.SetUpCustomerDB(email) Then
                    rslt = New MemoryStream(MyCustomerDBSvc.RetrieveManagerImage(guidstr, 2))
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ImageSvc"
                EvLog.Log = ""
                EvLog.WriteEntry("ManagerSmallImagePlease: email=" & email & " sid=" & guidstr & " Reports exception " & ex.Message, EventLogEntryType.Warning)
            End Using
        End Try

        Return rslt
    End Function
    Public Function ManagerThumbnailImagePlease(ByVal email As String, ByVal guidstr As String) As Stream Implements IImageSvc.ManagerThumbnailImagePlease
        Dim rslt As Stream = Nothing
        Try
            If UserCacheManager.IsLoggedIn(email) Then
                If Me.SetUpCustomerDB(email) Then
                    rslt = New MemoryStream(MyCustomerDBSvc.RetrieveManagerImage(guidstr, 3))
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ImageSvc"
                EvLog.Log = ""
                EvLog.WriteEntry("ManagerThumbnailImagePlease: email=" & email & " sid=" & guidstr & " Reports exception " & ex.Message, EventLogEntryType.Warning)
            End Using
        End Try

        Return rslt
    End Function
#End Region


#Region "Prelim Concept tests"
    Public Function TestMe() As String Implements IImageSvc.TestMe
        Return String.Format("You tested me... " & Date.Now.ToLongTimeString)
    End Function

    Public Function LogInPlease(ByVal email As String, ByVal sid As String, ByVal sqn As String) As String Implements IImageSvc.LogInPlease
        Dim rslt As String = "Not Logged In " & Date.Now.ToString
        If UserCacheManager.IsLoggedIn(email) Then
            rslt = "yes...logged in.." & Date.Now.ToString
            If Me.SetUpCustomerDB(email) Then
                rslt += " has image = " & MyCustomerDBSvc.RetrieveSurveyImagesPackageSequenceNumber(sid, sqn).ImageStorePkgList.Count.ToString()
            End If

        End If
        Return rslt
    End Function

    Public Function ImagePlease(ByVal email As String, ByVal sid As String, ByVal sqn As String) As Stream Implements IImageSvc.ImagePlease
        Dim rslt As Stream = Nothing
        Try
            If UserCacheManager.IsLoggedIn(email) Then
                If Me.SetUpCustomerDB(email) Then
                    rslt = New MemoryStream(MyCustomerDBSvc.RetrieveSurveyImagesPackageSequenceNumber(sid, sqn).ImageStorePkgList.FirstOrDefault.ByteArray)
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ImageSvc"
                EvLog.Log = ""
                EvLog.WriteEntry("ImagePlease: email=" & email & " sid=" & sid & " sqn=" & sqn & " Reports exception " & ex.Message, EventLogEntryType.Warning)
            End Using
        End Try

        Return rslt
    End Function

    Function ImagePCElemPlease(ByVal email As String, ByVal sid As String, ByVal pcmid As String, ByVal pcelemid As String) As Stream Implements IImageSvc.ImagePCElemPlease
        Dim rslt As Stream = Nothing
        Try
            If UserCacheManager.IsLoggedIn(email) Then
                If Me.SetUpCustomerDB(email) Then
                    rslt = New MemoryStream(MyCustomerDBSvc.RetrieveImagePCElem(sid, pcmid, pcelemid).ImageStorePkgList.FirstOrDefault.ByteArray)
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ImageSvc"
                EvLog.Log = ""
                EvLog.WriteEntry("ImagePCElemPlease: email=" & email & " sid=" & sid & " pcmid=" & pcmid & " Reports exception " & ex.Message, EventLogEntryType.Warning)
            End Using
        End Try

        Return rslt
    End Function

    Function ImagePCElemIDsList(ByVal email As String, ByVal sid As String, ByVal pcmid As String) As List(Of Integer) Implements IImageSvc.ImagePCElemIDsList
        Dim rslt As List(Of Integer) = Nothing
        Try
            If UserCacheManager.IsLoggedIn(email) Then
                If Me.SetUpCustomerDB(email) Then
                    rslt = MyCustomerDBSvc.RetrieveImagePCElemIDsList(sid, pcmid)
                End If
            End If
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "ImageSvc"
                EvLog.Log = ""
                EvLog.WriteEntry("ImagePCElemIDsList: email=" & email & " sid=" & sid & " pcmid=" & pcmid & " Reports exception " & ex.Message, EventLogEntryType.Warning)
            End Using
        End Try

        Return rslt
    End Function

#End Region


#Region "ResultsSVC methods for webhttp"
    Private MyCSHResultsService As CustomSvcHost
    Private MyResultsService As ResultsSvc
    Public Function Retrieve_SDSResponseModels(email As String, ByVal sid As String) As List(Of SDSResponseModelObject) Implements IImageSvc.Retrieve_SDSResponseModels
        Dim rslt As List(Of SDSResponseModelObject) = Nothing
        Dim surveyid As Integer = Nothing
        If Integer.TryParse(sid, surveyid) Then
            Try
                If UserCacheManager.IsLoggedIn(email) Then
                    Me.MyCSHResultsService = Me.SetUpMyCSH(email, "RSLT")
                    If Not IsNothing(Me.MyCSHResultsService) Then
                        Me.MyResultsService = New ResultsSvc
                        With Me.MyResultsService
                            .MyCSH = Me.MyCSHResultsService
                            rslt = .Retrieve_SDSResponseModels(surveyid)
                        End With
                    Else
                        Throw New WebFaultException(Of String)("Retrieve_SDSResponseModels No ResultsSvcHost " & email, Net.HttpStatusCode.NotFound)
                    End If
                Else
                    Throw New WebFaultException(Of String)("Retrieve_SDSResponseModels email IsLoggedIn=False " & email, Net.HttpStatusCode.Unauthorized)
                End If
            Catch ex As Exception
                Throw New WebFaultException(Of String)("Retrieve_SDSResponseModels encountered " & ex.Message, Net.HttpStatusCode.BadRequest)
            End Try
        Else
            Throw New WebFaultException(Of String)("Retrieve_SDSResponseModels sid does not parse to integer " & email, Net.HttpStatusCode.NotFound)
        End If
        Return rslt
    End Function

    Public Function RetrieveResults(email As String, ByVal sid As String) As List(Of ResultsProviderSummaryObject) Implements IImageSvc.RetrieveResults
        Dim rslt As List(Of ResultsProviderSummaryObject) = Nothing
        Dim surveyid As Integer = Nothing
        If Integer.TryParse(sid, surveyid) Then
            Try
                If UserCacheManager.IsLoggedIn(email) Then
                    Me.MyCSHResultsService = Me.SetUpMyCSH(email, "RSLT")
                    If Not IsNothing(Me.MyCSHResultsService) Then
                        Me.MyResultsService = New ResultsSvc
                        With Me.MyResultsService
                            .MyCSH = Me.MyCSHResultsService
                            rslt = .RetrieveResults(surveyid)
                        End With
                    Else
                        Throw New WebFaultException(Of String)("RetrieveResults No ResultsSvcHost " & email, Net.HttpStatusCode.NotFound)
                    End If
                Else
                    Throw New WebFaultException(Of String)("RetrieveResults email IsLoggedIn=False " & email, Net.HttpStatusCode.Unauthorized)
                End If
            Catch ex As Exception
                Throw New WebFaultException(Of String)("RetrieveResults encountered " & ex.Message, Net.HttpStatusCode.BadRequest)
            End Try
        Else
            Throw New WebFaultException(Of String)("RetrieveResults sid does not parse to integer " & email, Net.HttpStatusCode.NotFound)
        End If
        Return rslt

    End Function

    Public Function Retrieve_ResultsFilters(email As String, ByVal sid As String) As List(Of ResultsFilterModelObject) Implements IImageSvc.Retrieve_ResultsFilters
        Dim rslt As List(Of ResultsFilterModelObject) = Nothing
        Dim surveyid As Integer = Nothing
        If Integer.TryParse(sid, surveyid) Then
            Try
                If UserCacheManager.IsLoggedIn(email) Then
                    Me.MyCSHResultsService = Me.SetUpMyCSH(email, "RSLT")
                    If Not IsNothing(Me.MyCSHResultsService) Then
                        Me.MyResultsService = New ResultsSvc
                        With Me.MyResultsService
                            .MyCSH = Me.MyCSHResultsService
                            rslt = .Retrieve_ResultsFilters(surveyid)
                        End With
                    Else
                        Throw New WebFaultException(Of String)("RetrieveResults No ResultsSvcHost " & email, Net.HttpStatusCode.NotFound)
                    End If
                Else
                    Throw New WebFaultException(Of String)("RetrieveResults email IsLoggedIn=False " & email, Net.HttpStatusCode.Unauthorized)
                End If
            Catch ex As Exception
                Throw New WebFaultException(Of String)("RetrieveResults encountered " & ex.Message, Net.HttpStatusCode.BadRequest)
            End Try
        Else
            Throw New WebFaultException(Of String)("RetrieveResults sid does not parse to integer " & email, Net.HttpStatusCode.NotFound)
        End If
        Return rslt
    End Function

    Public Function RetrieveFilteredResults_with_RFMID(ByVal email As String, ByVal sid As String, ByVal rfmid As String) _
    As List(Of ResultsProviderSummaryObject) Implements IImageSvc.RetrieveFilteredResults_with_RFMID
        Dim rslt As List(Of ResultsProviderSummaryObject) = Nothing
        Dim surveyid As Integer = Nothing
        Dim resultsfiltermodelid As Integer = Nothing
        If Integer.TryParse(sid, surveyid) AndAlso Integer.TryParse(rfmid, resultsfiltermodelid) Then
            Try
                If UserCacheManager.IsLoggedIn(email) Then
                    Me.MyCSHResultsService = Me.SetUpMyCSH(email, "RSLT")
                    If Not IsNothing(Me.MyCSHResultsService) Then
                        Me.MyResultsService = New ResultsSvc
                        With Me.MyResultsService
                            .MyCSH = Me.MyCSHResultsService
                            'this needs to call a method that does this...but it gets its list of rfgo from the resultsfiltermodel implied by rfmid...
                            rslt = .RetrieveFilteredResults_with_ListofResultFilterGroupObject(surveyid, New List(Of ResultsFilterGroupObject))
                        End With
                    Else
                        Throw New WebFaultException(Of String)("RetrieveFilteredResults_with_RFMID No ResultsSvcHost " & email, Net.HttpStatusCode.NotFound)
                    End If
                Else
                    Throw New WebFaultException(Of String)("RetrieveFilteredResults_with_RFMID email IsLoggedIn=False " & email, Net.HttpStatusCode.Unauthorized)
                End If
            Catch ex As Exception
                Throw New WebFaultException(Of String)("RetrieveFilteredResults_with_RFMID encountered " & ex.Message, Net.HttpStatusCode.BadRequest)
            End Try
        Else
            Throw New WebFaultException(Of String)("RetrieveFilteredResults_with_RFMID sid does not parse to integer " & email, Net.HttpStatusCode.NotFound)
        End If

        Return rslt
    End Function
#End Region


#Region "AuthoringSVC Methods for webhttp"
    Private MyCSHAuthoringService As CustomSvcHost
    Private MyAuthoringService As AuthoringSvc
    Public Function Retrieve_SurveyMetaData_List(ByVal email As String) As List(Of SurveyMetadataPackage) Implements IImageSvc.Retrieve_SurveyMetaData_List
        Dim rslt As List(Of SurveyMetadataPackage) = Nothing
        Try
            If UserCacheManager.IsLoggedIn(email) Then
                Me.MyCSHAuthoringService = Me.SetUpMyCSH(email, "AUTH")
                If Not IsNothing(Me.MyCSHAuthoringService) Then
                    MyAuthoringService = New AuthoringSvc
                    With Me.MyAuthoringService
                        .MyCSH = MyCSHAuthoringService
                        rslt = .Retrieve_SurveyMetaData_List(email)
                    End With

                End If
            Else
                Throw New WebFaultException(Of String)("SurveyMetaData_List email IsLoggedIn=False " & email, Net.HttpStatusCode.Unauthorized)
            End If
        Catch ex As Exception
            Throw New WebFaultException(Of String)("SurveyMetaData_List encountered " & ex.Message, Net.HttpStatusCode.InternalServerError)
        End Try

        Return rslt

    End Function

    Public Function Retrieve_SurveyMetaData(ByVal email As String, ByVal sid As String) As SurveyMetadataPackage Implements IImageSvc.Retrieve_SurveyMetaData
        Dim rslt As SurveyMetadataPackage = Nothing
        Dim surveyid As Integer = Nothing
        If Integer.TryParse(sid, surveyid) Then
            Try
                If UserCacheManager.IsLoggedIn(email) Then
                    Me.MyCSHAuthoringService = Me.SetUpMyCSH(email, "AUTH")
                    If Not IsNothing(Me.MyCSHAuthoringService) Then
                        MyAuthoringService = New AuthoringSvc
                        With Me.MyAuthoringService
                            .MyCSH = MyCSHAuthoringService
                            rslt = .Retrieve_SurveyMetaData(surveyid)
                        End With
                    Else
                        Throw New WebFaultException(Of String)("Retrieve_SurveyMetaData No AuthoringSvcHost " & email, Net.HttpStatusCode.NotFound)
                    End If
                Else
                    Throw New WebFaultException(Of String)("Retrieve_SurveyMetaData email IsLoggedIn=False " & email, Net.HttpStatusCode.Unauthorized)
                End If
            Catch ex As Exception
                Throw New WebFaultException(Of String)("Retrieve_SurveyMetaData encountered " & ex.Message, Net.HttpStatusCode.BadRequest)
            End Try
        Else
            Throw New WebFaultException(Of String)("Retrieve_SurveyMetaData sid does not parse to integer " & email, Net.HttpStatusCode.NotFound)
        End If


        Return rslt

    End Function

    Public Function Retrieve_PageBlob_Count(ByVal email As String, ByVal sid As String) _
    As Integer Implements IImageSvc.Retrieve_PageBlob_Count
        Dim rslt As Integer = Nothing
        Dim surveyid As Integer = Nothing
        If Integer.TryParse(sid, surveyid) Then
            Try
                If UserCacheManager.IsLoggedIn(email) Then
                    Me.MyCSHAuthoringService = Me.SetUpMyCSH(email, "AUTH")
                    If Not IsNothing(Me.MyCSHAuthoringService) Then
                        MyAuthoringService = New AuthoringSvc
                        With Me.MyAuthoringService
                            .MyCSH = MyCSHAuthoringService
                            rslt = .Retrieve_PageBlob_Count(surveyid)
                        End With
                    Else
                        Throw New WebFaultException(Of String)("Retrieve_PageBlob_Count No AuthoringSvcHost " & email, Net.HttpStatusCode.NotFound)
                    End If
                Else
                    Throw New WebFaultException(Of String)("Retrieve_PageBlob_Count email IsLoggedIn=False " & email, Net.HttpStatusCode.Unauthorized)
                End If
            Catch ex As Exception
                Throw New WebFaultException(Of String)("Retrieve_PageBlob_Count encountered " & ex.Message, Net.HttpStatusCode.BadRequest)
            End Try
        Else
            Throw New WebFaultException(Of String)("Retrieve_PageBlob_Count sid does not parse to integer " & email, Net.HttpStatusCode.NotFound)
        End If

        Return rslt
    End Function

    Public Function Retrieve_Page(ByVal email As String, ByVal sid As String, ByVal ndx As String) _
    As Page_Package Implements IImageSvc.Retrieve_Page
        Dim rslt As Page_Package = Nothing
        Dim surveyid As Integer = Nothing
        Dim indexofpage As Integer = Nothing
        If Integer.TryParse(sid, surveyid) AndAlso Integer.TryParse(ndx, indexofpage) Then
            Try
                If UserCacheManager.IsLoggedIn(email) Then
                    Me.MyCSHAuthoringService = Me.SetUpMyCSH(email, "AUTH")
                    If Not IsNothing(Me.MyCSHAuthoringService) Then
                        MyAuthoringService = New AuthoringSvc
                        With Me.MyAuthoringService
                            .MyCSH = MyCSHAuthoringService
                            rslt = .Retrieve_Page(surveyid, indexofpage)
                        End With
                    Else
                        Throw New WebFaultException(Of String)("Retrieve_Page No AuthoringSvcHost " & email, Net.HttpStatusCode.NotFound)
                    End If
                Else
                    Throw New WebFaultException(Of String)("Retrieve_Page email IsLoggedIn=False " & email, Net.HttpStatusCode.Unauthorized)
                End If
            Catch ex As Exception
                Throw New WebFaultException(Of String)("Retrieve_Pageencountered " & ex.Message, Net.HttpStatusCode.BadRequest)
            End Try
        Else
            Throw New WebFaultException(Of String)("Retrieve_Page/ sid or ndx does not parse to integer " & email, Net.HttpStatusCode.NotFound)
        End If


        Return rslt
    End Function
#End Region


#Region "SetUpMyCSHxxxxxSvc"
    Private Const AuthDxnryKey As String = "https://perceptricsresearch.com/hm/AuthoringSvcNS.AuthoringSvc/"
    Private Const ResultsDxnryKey As String = "https://perceptricsresearch.com/hm/RsltsProviderSvcLibr.ResultsSvc/"

    Private Function SetUpMyCSH(_email As String, _AuthorResults As String) As CustomSvcHost
        Dim mycsh As CustomSvcHost = Nothing
        Dim dxnrykey = Nothing
        Select Case _AuthorResults
            Case Is = "AUTH"
                dxnrykey = ImageSvc.AuthDxnryKey & _email
            Case Is = "RSLT"
                dxnrykey = ImageSvc.ResultsDxnryKey & _email
        End Select
        If Not IsNothing(dxnrykey) Then
            WCFServiceManager.ServicesDxnry.TryGetValue(dxnrykey, mycsh)
        End If


        Return mycsh
    End Function
#End Region


    'Private Property MyGlobalMasterDataResult_Pkg As GlobalMasterDataByLoginNameResult_Package
    Private MyCustomerDBSvc As CustomerDBSvc.CustomerDBSvc = Nothing
    Private Function SetUpCustomerDB(ByVal _email As String) As Boolean
        Dim mycsh = Nothing
        Dim dxnrykey = "https://perceptricsresearch.com/hm/AuthoringSvcNS.AuthoringSvc/" & _email
        WCFServiceManager.ServicesDxnry.TryGetValue(dxnrykey, mycsh)
        If Not IsNothing(mycsh) Then
            Dim dcnxnstring = mycsh.DataContextConnectionString
            If dcnxnstring IsNot Nothing Then
                MyCustomerDBSvc = New CustomerDBSvc.CustomerDBSvc
                MyCustomerDBSvc.DC_ConnectionString = dcnxnstring
                MyCustomerDBSvc.MyCSH = mycsh
                Return True
            Else
                MyCustomerDBSvc = Nothing
                Return False
            End If
        Else
            Return False
        End If
        'MyGlobalMasterDataResult_Pkg = UserCacheManager.GetGMDbpkg(_email).GMDBPkg
        'If Not IsNothing(Me.MyGlobalMasterDataResult_Pkg) Then
        '    If Not IsNothing(Me.MyGlobalMasterDataResult_Pkg.CustomerDatabase) Then

        '    End If
        'End If
    End Function
End Class
