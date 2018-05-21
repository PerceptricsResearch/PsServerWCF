' NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
Public Class TesterSvc
    Implements ITesterSvc.ITesterSvc

    Private MyCSH As CmdSvcClassLibrary.CustomSvcHost = OperationContext.Current.Host
    Private InstanceTracker As New CmdSvcClassLibrary.InstanceTracker(OperationContext.Current.Host)


    Public Sub GoDoWhatISaid() Implements ITesterSvc.ITesterSvc.GoDoWhatISaid
        InstanceTracker.TrackMethod("GoDoWhatISaid", 0)

        Dim db As New L2S_SurveyDSDataContext
        Dim q = From sm In db.SurveyMasters _
                Select sm.SurveyID, sm.SurveyDescription, sm.QueueURI

        MyCSH.Cache_Pkg = New CmdSvcClassLibrary.Cache_Package(q.ToList, 0, 0)

        MyCSH.DataContextConnectionString = db.Connection.ConnectionString

        MyCSH.LastSurveyID = 100000

        MyCSH.SvcMonitor.Update_ServiceCalls("GoDoWhatISaid " & DateTime.Now.ToLongTimeString, db.Connection.Database)

        'CType(OperationContext.Current.Host, CmdSvcClassLibrary.CustomSvcHost).DataContextConnectionString = _
        'DateTime.Now.ToLongTimeString & vbCrLf & "ConnectionString provided " & _
        'vbCrLf & " by CommandSvc from EndPtDataContextService "

        'CType(OperationContext.Current.Host, CmdSvcClassLibrary.CustomSvcHost).Cache = DateTime.Now.ToLongTimeString & vbCrLf & "This is a PgItemBlob " & _
        'vbCrLf & " or a List(of tiny surveyrows withSurveyItemModel"
        'I actually want this Operation to 
        ' do a query for either
        ' in the case of the PageItemsService... a PgItemBlob...the first page...This is Just for RDENT's taking a survey...
        ' and put the PageBlob in this CustomServiceHost's PgItemBlobCache
        ' or in the case of the CustomerDBSvc... get a List of TinySurveyRows with SurveyItemModel...this is for AuthorViewers...
        ' and store it in the CustomerServiceHost's TinySurveyRowwithcSurveyItemModelCache
    End Sub
End Class
