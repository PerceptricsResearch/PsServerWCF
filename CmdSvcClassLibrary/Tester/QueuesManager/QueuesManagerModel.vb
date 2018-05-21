Imports System.ComponentModel
Imports System.Messaging
Imports System.Collections.ObjectModel

Public Class QueuesManagerModel
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
        'If propname = "Configuration" Then
        '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
        'End If
    End Sub

    Private _MyQueuesList As New ObservableCollection(Of TinyMQ)
    Public Property MyQueuesList() As ObservableCollection(Of TinyMQ)
        Get
            Return _MyQueuesList
        End Get
        Set(ByVal value As ObservableCollection(Of TinyMQ))
            _MyQueuesList = value
            My_OnPropertyChanged("MyQueuesList")
        End Set
    End Property

End Class
