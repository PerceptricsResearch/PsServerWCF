Imports System
Imports System.ServiceModel
Imports System.ComponentModel

Class Window1


    Private ThisInstance As Window1
    Public Sub IamLoaded() Handles Me.Loaded
        
        ThisInstance = Me
        AddHandler WCFSvcLibr1.ServiceMonitor.PropertyChanged, AddressOf ServiceMonitorPropertyChanged

        'SetUpRespDispatcherSvcHost() 'automatically starts service
    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        If MyRespDispatcherSvcHost IsNot Nothing Then
            If MyRespDispatcherSvcHost.State <> CommunicationState.Opened _
            AndAlso MyRespDispatcherSvcHost.State <> CommunicationState.Opening Then
                SetUpRespDispatcherSvcHost()
            End If
        Else
            SetUpRespDispatcherSvcHost()
        End If
    End Sub

    Private WithEvents MyRespDispatcherSvcHost As ServiceHost



    Private Sub SetUpRespDispatcherSvcHost()
        Try
            MyRespDispatcherSvcHost = New ServiceHost(GetType(WCFSvcLibr1.RespDispatcherSvc))
            'Dim netmqbinding As New NetMsmqBinding("NetMsmqBinding_IRespDispatcherSvc")
            'Dim endpt = MyRespDispatcherSvcHost.AddServiceEndpoint(GetType(WCFSvcLibr1.IRespDispatcherSvc), _
            '                                          netmqbinding,someaddressstring...)

            With MyRespDispatcherSvcHost
                WCFSvcLibr1.ServiceMonitor.Update_ServiceInfoColxn(.Description.Name & .GetHashCode.ToString, _
                                                               .Description.Endpoints, .State)
                WCFSvcLibr1.ServiceMonitor.Update_ServiceCalls("<<New>>" & .Description.Name & .GetHashCode, .State.ToString)
            End With

            AddHandler WCFSvcLibr1.ServiceMonitor.PropertyChanged, AddressOf ServiceMonitorPropertyChanged
            AddHandler MyRespDispatcherSvcHost.Opened, AddressOf UpdateSvcMonitorState
            AddHandler MyRespDispatcherSvcHost.Faulted, AddressOf ServiceFaultedHandler
            AddHandler MyRespDispatcherSvcHost.Closing, AddressOf UpdateSvcMonitorState
            AddHandler MyRespDispatcherSvcHost.Closed, AddressOf UpdateSvcMonitorState
            MyRespDispatcherSvcHost.Open()
            With MyRespDispatcherSvcHost
                WCFSvcLibr1.ServiceMonitor.Update_ServiceCalls("<<Open>>" & .Description.Name & .GetHashCode, .State.ToString)
            End With


        Catch ex As Exception
            MessageBox.Show("RespDispatcherContainer.SetUpRespDispatcherSvcHost reports error..." & ex.Message)
        End Try

    End Sub
    Private Sub ServiceFaultedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles MyRespDispatcherSvcHost.Faulted
        RemoveHandler WCFSvcLibr1.ServiceMonitor.PropertyChanged, AddressOf ServiceMonitorPropertyChanged
        With CType(sender, ServiceHost)
            RemoveHandler .Opened, AddressOf UpdateSvcMonitorState
            RemoveHandler .Faulted, AddressOf ServiceFaultedHandler
            RemoveHandler .Closing, AddressOf UpdateSvcMonitorState
            RemoveHandler .Closed, AddressOf UpdateSvcMonitorState
            WCFSvcLibr1.ServiceMonitor.Update_ServiceState(.Description.Name & .GetHashCode, .State)
            WCFSvcLibr1.ServiceMonitor.Update_ServiceCalls("<<<<<Faulted Should see MSmQ Retry(ies) >>>>>>" & .Description.Name & .GetHashCode, .State.ToString)
        End With
        sender = Nothing
        'SetUpRespDispatcherSvcHost()
        'sender = New ServiceHost(GetType(WCFSvcLibr1.RespDispatcherSvc))
        'With CType(sender, ServiceHost)
        '    WCFSvcLibr1.ServiceMonitor.Update_ServiceState(.Description.Name & .GetHashCode, .State)
        '    WCFSvcLibr1.ServiceMonitor.Update_ServiceCalls("<<New After Fault>>" & .Description.Name & .GetHashCode, .State)
        'End With

    End Sub

    Private Sub UpdateSvcMonitorState(ByVal sender As Object, ByVal e As System.EventArgs)

        With CType(sender, ServiceHost)
            WCFSvcLibr1.ServiceMonitor.Update_ServiceState(.Description.Name & .GetHashCode, .State)
        End With

    End Sub


    Private Sub WindowClosing() Handles Me.Closing
        If MyRespDispatcherSvcHost IsNot Nothing Then
            If MyRespDispatcherSvcHost.State <> CommunicationState.Closing AndAlso MyRespDispatcherSvcHost.State <> CommunicationState.Closed Then
                If MyRespDispatcherSvcHost.State = CommunicationState.Faulted Then
                    MyRespDispatcherSvcHost = Nothing
                Else
                    MyRespDispatcherSvcHost.Close()
                End If
            End If
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If MyRespDispatcherSvcHost IsNot Nothing Then
            If MyRespDispatcherSvcHost.State <> CommunicationState.Closing AndAlso MyRespDispatcherSvcHost.State <> CommunicationState.Closed Then
                If MyRespDispatcherSvcHost.State = CommunicationState.Faulted Then
                    With MyRespDispatcherSvcHost
                        WCFSvcLibr1.ServiceMonitor.Update_ServiceCalls("<<Stop Service Click>>" & .Description.Name & .GetHashCode, _
                                                                       .State.ToString)
                    End With
                    MyRespDispatcherSvcHost = Nothing
                Else
                    With MyRespDispatcherSvcHost
                        WCFSvcLibr1.ServiceMonitor.Update_ServiceCalls("<<Stop Service Click>>" & .Description.Name & .GetHashCode, _
                                                                       .State.ToString)
                    End With
                    MyRespDispatcherSvcHost.Close()
                End If
            End If
        End If
    End Sub


    Private Sub ServiceMonitorPropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) 'Handles WCFSvcLibr1.ServiceMonitor.PropertyChanged
        If e.PropertyName = "ServiceCalls" Then
            ThisInstance.OperationsMonitorListBox.ItemsSource = Nothing
            ThisInstance.OperationsMonitorListBox.ItemsSource = WCFSvcLibr1.ServiceMonitor.ServiceCalls
            ThisInstance.OperationsMonitorListBox.Items.MoveCurrentToLast()
            ThisInstance.OperationsMonitorListBox.SelectedItem = ThisInstance.OperationsMonitorListBox.Items.CurrentItem
            ThisInstance.OperationsMonitorListBox.ScrollIntoView(ThisInstance.OperationsMonitorListBox.SelectedItem)
        ElseIf e.PropertyName = "ServiceStateEvents" Then
            ThisInstance.ServiceListbox.ItemsSource = Nothing
            ThisInstance.ServiceListbox.ItemsSource = WCFSvcLibr1.ServiceMonitor.ServiceInfoColxn
            ThisInstance.ServiceListbox.Items.MoveCurrentToLast()
            ThisInstance.ServiceListbox.SelectedItem = ThisInstance.ServiceListbox.Items.CurrentItem
            ThisInstance.ServiceListbox.ScrollIntoView(ThisInstance.ServiceListbox.SelectedItem)

            ThisInstance.StateListbox.ItemsSource = Nothing
            ThisInstance.StateListbox.ItemsSource = From si In WCFSvcLibr1.ServiceMonitor.ServiceInfoColxn _
                                                    From sc In si.StateColxn _
                                                    Select sc
            ThisInstance.StateListbox.Items.MoveCurrentToLast()
            ThisInstance.StateListbox.SelectedItem = ThisInstance.StateListbox.Items.CurrentItem
            ThisInstance.StateListbox.ScrollIntoView(ThisInstance.StateListbox.SelectedItem)
        End If
    End Sub

    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.OperationsMonitorListBox.ItemsSource = Nothing
        Me.OperationsMonitorListBox.ItemsSource = WCFSvcLibr1.ServiceMonitor.ServiceCalls

        ThisInstance.ServiceListbox.ItemsSource = Nothing
        ThisInstance.ServiceListbox.ItemsSource = WCFSvcLibr1.ServiceMonitor.ServiceInfoColxn
        ThisInstance.ServiceListbox.Items.MoveCurrentToLast()
        ThisInstance.ServiceListbox.SelectedItem = ThisInstance.ServiceListbox.Items.CurrentItem
        ThisInstance.ServiceListbox.ScrollIntoView(ThisInstance.ServiceListbox.SelectedItem)

        ThisInstance.StateListbox.ItemsSource = Nothing
        ThisInstance.StateListbox.ItemsSource = From si In WCFSvcLibr1.ServiceMonitor.ServiceInfoColxn _
                                                From sc In si.StateColxn _
                                                Select sc
        ThisInstance.StateListbox.Items.MoveCurrentToLast()
        ThisInstance.StateListbox.SelectedItem = ThisInstance.StateListbox.Items.CurrentItem
        ThisInstance.StateListbox.ScrollIntoView(ThisInstance.StateListbox.SelectedItem)
    End Sub

    Private RespPostingCmdSvcClient As ResponsePostingContainerCommandSvc.CommandSvcClient = Nothing
    Private Sub PostingButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If RespPostingCmdSvcClient Is Nothing Then
            Try
                RespPostingCmdSvcClient = New ResponsePostingContainerCommandSvc.CommandSvcClient
                RespPostingCmdSvcClient.Open()
                Dim btn = CType(sender, Button)
                If Me.PostingContainerListbox.SelectedItem IsNot Nothing Then
                    Dim tb As TextBlock = CType(Me.PostingContainerListbox.SelectedItem, TextBlock)
                    RespPostingCmdSvcClient.IssueCommand(btn.Content.ToString, tb.Text)
                Else
                    MessageBox.Show("You must select a Survey...")
                End If
            Catch ex As Exception
                MessageBox.Show("RespDispatcherContainer.Window1.PostingButtonClick encountered error..." & ex.Message)
            End Try
        ElseIf RespPostingCmdSvcClient.State <> CommunicationState.Opened Then
            Try
                'RespPostingCmdSvcClient.Close()
                RespPostingCmdSvcClient = New ResponsePostingContainerCommandSvc.CommandSvcClient
                RespPostingCmdSvcClient.Open()
                Dim btn = CType(sender, Button)
                If Me.PostingContainerListbox.SelectedItem IsNot Nothing Then
                    Dim tb As TextBlock = CType(Me.PostingContainerListbox.SelectedItem, TextBlock)
                    RespPostingCmdSvcClient.IssueCommand(btn.Content.ToString, tb.Text)
                Else
                    MessageBox.Show("You must select a Survey...")
                End If
            Catch ex As Exception
                MessageBox.Show("RespDispatcherContainer.Window1.PostingButtonClick encountered error..." & ex.Message)
            End Try

        ElseIf RespPostingCmdSvcClient.State = CommunicationState.Opened Then
            Dim btn = CType(sender, Button)
            Try
                If Me.PostingContainerListbox.SelectedItem IsNot Nothing Then
                    Dim tb As TextBlock = CType(Me.PostingContainerListbox.SelectedItem, TextBlock)
                    RespPostingCmdSvcClient.IssueCommand(btn.Content.ToString, tb.Text)
                Else
                    MessageBox.Show("You must select a Survey...")
                End If
            Catch ex As Exception
                MessageBox.Show("RespDispatcherContainer.Window1.PostingButtonClick encountered error..." & ex.Message)
            End Try
        End If
    End Sub


End Class
