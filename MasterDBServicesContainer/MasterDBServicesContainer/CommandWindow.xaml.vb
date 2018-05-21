Imports MasterDBSvcLibr

Partial Public Class CommandWindow
    'Private myHostWindow As Window
    Private Sub MyClickHandler(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn = CType(sender, Button)
        Dim myHostWindow = New Window
        myHostWindow.ResizeMode = Windows.ResizeMode.CanResizeWithGrip
        myHostWindow.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
        myHostWindow.Width = 300
        myHostWindow.Height = 200
        'Dim vb As New Viewbox
        'vb.MinHeight = 100
        ' vb.MinWidth = 100

        myHostWindow.Title = "SvcHost " & CType(btn.Tag, CustomSvcHost).EndPtSuffix & " " & CType(btn.Tag, CustomSvcHost).State.ToString

        Dim view As New CustomSvcHostView
        Dim model As New CustomSvcHostModel With {.MyCustomSvcHost = btn.Tag}
        'view.MyModel = model
        view.DataContext = model

        'vb.Child = view
        myHostWindow.Content = view
        myHostWindow.WindowStyle = WindowStyle.ToolWindow
        myHostWindow.Show()
    End Sub
End Class
