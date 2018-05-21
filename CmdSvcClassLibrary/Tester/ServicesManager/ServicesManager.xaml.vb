Imports System.Collections.ObjectModel
Imports System.ComponentModel

Partial Public Class ServicesManager

    Public WithEvents MyModel As New ServicesManagerModel
    Public MyOwnerWindow As Tester.Window1


    Private Sub IamLoaded() Handles Me.Loaded
        PopulateMyModel()
        Me.DataContext = MyModel
    End Sub

    Private Sub PopulateMyModel()
        Dim db As New DataClasses1DataContext
        MyModel.GlobalSurveyMasterCnxnString = db.Connection.ConnectionString

        Dim svcelems = From selem In MyModel.SvcElementsFromConfig _
                       Select selem.Name

        Dim q = From si In db.WCFServiceInfos _
                Where svcelems.Contains(si.Name) _
                Select si
        MyModel.MyServicesInventory = New ObservableCollection(Of WCFServiceInfo)(q.ToList)
        Me.MyModel.MyServicesInventory.Add(New WCFServiceInfo With {.Name = "ImageSvcNS.ImageSvc", _
                                                                    .BaseAddress = "https://leases/ImageSvc/"})
    End Sub

    Private Sub MyModelPropertyChanged() Handles MyModel.PropertyChanged
        Me.DataContext = Nothing
        Me.DataContext = MyModel
    End Sub

    Private Sub HostSvc_btn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim wcfinfo As WCFServiceInfo = CType(sender, Button).Tag
        MyModel.MyServicesInventory.Remove(wcfinfo)
        MyModel.MyHostedServices.Add(wcfinfo)
        Dim q = From selem In MyModel.SvcElementsFromConfig _
                Where selem.Name = wcfinfo.Name _
                Select New SvcElementUC With {.DataContext = selem}
        If q.Any Then
            PutEminaWindow(q.Single)
        Else
            MessageBox.Show("Cannot find this Service under this name in the App.Config...it is not 'hostable' here...")
        End If
    End Sub

    Private Sub PutEminaWindow(ByVal Seuc As SvcElementUC)
        Dim win As New Window
        AddHandler win.Closing, AddressOf SvcElementWindowClosing
        win.Owner = Me.MyOwnerWindow
        win.Height = 400
        win.Width = 600
        'win.Title = Seuc.SvcName_Tbx.Text & " in " & win.Owner.Title...this actually gets set in SEUC...
        win.Content = Seuc
        win.WindowStyle = Windows.WindowStyle.SingleBorderWindow
        win.ResizeMode = Windows.ResizeMode.CanResizeWithGrip
        win.SizeToContent = Windows.SizeToContent.Height
        win.Show()
    End Sub

    Private Sub SvcElementWindowClosing(ByVal sender As Object, ByVal e As CancelEventArgs)
        Dim win = CType(sender, Window)
        Dim seuc As SvcElementUC = win.Content
        Dim svcelem As ServiceModel.Configuration.ServiceElement = seuc.DataContext
        Dim q = From wcfinfo In MyModel.MyHostedServices _
                Where svcelem.Name = wcfinfo.Name _
                Select wcfinfo
        Dim winfo = q.First
        MyModel.MyHostedServices.Remove(winfo) 'this window's content is the SEUC the window displays 
        MyModel.MyServicesInventory.Add(winfo) 'it's now back in the ServiceInventory
        Dim svcwindow = seuc.SvCElementUC_PlaceHolder_Grid.Children(0)
        'CType(svcwindow, CmdSvcClassLibrary.WPFSvcContainerWindow).CloseThis()

    End Sub
End Class
