Imports System.ComponentModel

Partial Public Class CustomSvcHostView
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
    End Sub


    Private Sub UserControl_DataContextChanged(ByVal sender As System.Object, ByVal e As System.Windows.DependencyPropertyChangedEventArgs)

    End Sub
    'Private WithEvents _myModel As CustomSvcHostModel
    'Public Property MyModel() As CustomSvcHostModel
    '    Get
    '        Return _myModel
    '    End Get
    '    Set(ByVal value As CustomSvcHostModel)
    '        _myModel = value
    '        Me.DataContext = _myModel
    '        My_OnPropertyChanged("MyModel")

    '    End Set
    'End Property

    'Private Sub MyModelChanged() Handles _myModel.PropertyChanged

    '    Me.DataContext = _myModel
    '    'My_OnPropertyChanged("DataContext")
    'End Sub
End Class
