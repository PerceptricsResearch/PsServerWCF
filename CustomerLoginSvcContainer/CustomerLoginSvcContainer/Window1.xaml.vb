Imports System.ServiceModel
Imports System.ComponentModel

Imports CustLoginServicesLibr


Class Window1


    Private Shared ThisInstance As Window1
    Public Sub IamLoaded() Handles Me.Loaded
        ThisInstance = Me
        LogInSvc_SvcMonitor = New ServiceMonitor
        SetUp_LogInSvc_SvcHost()
        'Try
        '    SetUpCommandSvcHost()
        'Catch ex As Exception
        '    MessageBox.Show("PageItemColxnContainer.Window1.SetupCommandSvcHost reports error..." & ex.Message)
        'End Try
    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click

        If MyLogInSvcHost IsNot Nothing Then
            If MyLogInSvcHost.State <> CommunicationState.Opened AndAlso MyLogInSvcHost.State <> CommunicationState.Opening Then
                SetUp_LogInSvc_SvcHost()
            End If
        Else
            SetUp_LogInSvc_SvcHost()
        End If

    End Sub

    Private MyLogInSvcHost As CustomSvcHost

    'Private SvcHostDxnry As New Dictionary(Of String, ServiceHost)

    Private Sub SetUp_LogInSvc_SvcHost()
        MyLogInSvcHost = New CustomSvcHost(GetType(CustLoginServicesLibr.LoginSvc))
        AddHandler MyLogInSvcHost.Opened, AddressOf UpdateStateTextBlock
        AddHandler MyLogInSvcHost.Faulted, AddressOf UpdateStateTextBlock
        AddHandler MyLogInSvcHost.Closing, AddressOf UpdateStateTextBlock
        AddHandler MyLogInSvcHost.Closed, AddressOf UpdateStateTextBlock
        MyLogInSvcHost.Open()

        If MyLogInSvcHost IsNot Nothing Then
            Dim tb As New TextBlock
            tb.Text = MyLogInSvcHost.Description.Name & " SetUp @ " & DateTime.Now.ToLongTimeString
            tb.Margin = New Thickness(2.0)
            Me.ServiceListbox.Items.Add(tb)
            Dim tb1 As New TextBlock
            tb1.Margin = New Thickness(2.0)
            tb1.Text = MyLogInSvcHost.Description.Endpoints.FirstOrDefault.ListenUri.OriginalString & " " & DateTime.Now.ToLongTimeString
            Dim lbitem1 As New ListBoxItem
            lbitem1.Content = tb1
            Me.ServiceListbox.Items.Add(lbitem1) '& _
            'vbCrLf & MySvcHost.Description.Endpoints.FirstOrDefault.ListenUri.Host & _
            'vbCrLf & MySvcHost.Description.Endpoints.FirstOrDefault.ListenUri.Authority & _
            'vbCrLf & MySvcHost.Description.Endpoints.FirstOrDefault.Contract.Name
            Me.ServiceListbox.SelectedItem = lbitem1
            Me.ServiceListbox.ScrollIntoView(lbitem1)


        End If
    End Sub
    Private Sub UpdateStateTextBlock()
        If MyLogInSvcHost IsNot Nothing Then
            Dim lbi As New ListBoxItem
            Dim tb As New TextBlock
            tb.Margin = New Thickness(2.0)
            tb.Text = MyLogInSvcHost.State.ToString & " " & DateTime.Now.ToShortTimeString
            lbi.Content = tb
            Me.StateListbox.Items.Add(lbi)
            Me.StateListbox.SelectedItem = lbi
            Me.StateListbox.ScrollIntoView(lbi)
        End If
        If MyLogInSvcHost.State = CommunicationState.Faulted Then
            SetUp_LogInSvc_SvcHost()
        End If
    End Sub

    Private Sub WindowClosing() Handles Me.Closing
        If MyLogInSvcHost IsNot Nothing Then
            If MyLogInSvcHost.State <> CommunicationState.Closing AndAlso MyLogInSvcHost.State <> CommunicationState.Closed Then
                MyLogInSvcHost.Close()
                MyLogInSvcHost = Nothing
            End If
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If MyLogInSvcHost IsNot Nothing Then
            If MyLogInSvcHost.State <> CommunicationState.Closing AndAlso MyLogInSvcHost.State <> CommunicationState.Closed Then
                MyLogInSvcHost.Close()
                MyLogInSvcHost = Nothing
            End If
        End If
    End Sub

    Private Shared WithEvents LogInSvc_SvcMonitor As ServiceMonitor
    Private Shared Sub LogInSvc_SvcMonitorPropertyChanged() Handles LogInSvc_SvcMonitor.PropertyChanged
        ThisInstance.OperationsMonitorListBox.ItemsSource = Nothing
        ThisInstance.OperationsMonitorListBox.ItemsSource = ServiceMonitor.ServiceCalls

        ThisInstance.OperationsMonitorListBox.Items.MoveCurrentToLast()
        ThisInstance.OperationsMonitorListBox.SelectedItem = ThisInstance.OperationsMonitorListBox.Items.CurrentItem
        ThisInstance.OperationsMonitorListBox.ScrollIntoView(ThisInstance.OperationsMonitorListBox.SelectedItem)

    End Sub

    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.OperationsMonitorListBox.ItemsSource = Nothing
        Me.OperationsMonitorListBox.ItemsSource = ServiceMonitor.ServiceCalls

    End Sub
End Class
