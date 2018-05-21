Imports System.ComponentModel

Partial Public Class CustomSvcHostView
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
    End Sub


    Private Sub UserControl_DataContextChanged(ByVal sender As System.Object, ByVal e As System.Windows.DependencyPropertyChangedEventArgs)

    End Sub
  
End Class
