Imports System.Windows.Controls
Imports System.Windows
Imports System.Windows.Media
Imports System.ComponentModel

Partial Public Class CommandWindow
    'Implements INotifyPropertyChanged
    'Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    'Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
    '    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
    '    'If propname = "Configuration" Then
    '    '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
    '    'End If
    'End Sub

    ''Private myHostWindow As Window
    'Private Sub MyClickHandler(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim btn = CType(sender, Button)
    '    Dim myHostWindow = New Window
    '    myHostWindow.ResizeMode = Windows.ResizeMode.CanResizeWithGrip
    '    myHostWindow.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
    '    myHostWindow.Width = 300
    '    myHostWindow.Height = 200
    '    'Dim vb As New Viewbox
    '    'vb.MinHeight = 100
    '    ' vb.MinWidth = 100

    '    myHostWindow.Title = "SvcHost " & CType(btn.Tag, CustomSvcHost).EndPtSuffix & " " & CType(btn.Tag, CustomSvcHost).State.ToString

    '    Dim view As New CustomSvcHostView
    '    Dim model As New CustomSvcHostModel With {.MyCustomSvcHost = btn.Tag}
    '    'view.MyModel = model
    '    view.DataContext = model

    '    'vb.Child = view
    '    myHostWindow.Content = view
    '    myHostWindow.WindowStyle = WindowStyle.ToolWindow
    '    myHostWindow.Show()
    'End Sub


    'Private _ColorSchemeBorder As Brush = New SolidColorBrush(Colors.SaddleBrown)
    'Public Property ColorSchemeBorder() As Brush
    '    Get
    '        Return _ColorSchemeBorder
    '    End Get
    '    Set(ByVal value As Brush)
    '        _ColorSchemeBorder = value
    '        My_OnPropertyChanged("ColorSchemeBorder")
    '    End Set
    'End Property

    'Private _ColorSchemeSplitters As Brush = New SolidColorBrush(Colors.SaddleBrown)
    'Public Property ColorSchemeSplitters() As Brush
    '    Get
    '        Return _ColorSchemeSplitters
    '    End Get
    '    Set(ByVal value As Brush)
    '        _ColorSchemeSplitters = value
    '        My_OnPropertyChanged("ColorSchemeSplitters")
    '    End Set
    'End Property
End Class
