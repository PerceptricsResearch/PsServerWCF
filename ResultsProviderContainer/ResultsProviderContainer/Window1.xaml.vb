Imports System.ServiceModel
'Imports RsltsProviderSvcLibr


Class Window1


    Private Shared ThisInstance As Window1
    Public Sub IamLoaded() Handles Me.Loaded
        'Me.StateListbox.Items.Add("I am Loaded")
        'Me.TextBlock1.Background = New SolidColorBrush(Colors.YellowGreen)
        ThisInstance = Me
        SvcMonitor = New RsltsProviderSvcLibr.ServiceMonitor
        SetUpResultsSvcHost()
    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click

        If MyResultsSvcHost IsNot Nothing Then
            If MyResultsSvcHost.State <> CommunicationState.Opened AndAlso MyResultsSvcHost.State <> CommunicationState.Opening Then
                SetUpResultsSvcHost()
                'If MyMasterSurveyDBSvcHost IsNot Nothing Then
                '    Me.ServiceListbox.Items.Add(MyMasterSurveyDBSvcHost.Description.Name)
                '    Me.ServiceListbox.Items.Add(MyMasterSurveyDBSvcHost.Description.Endpoints.FirstOrDefault.ListenUri.OriginalString) '& _
                '    'vbCrLf & MySvcHost.Description.Endpoints.FirstOrDefault.ListenUri.Host & _
                '    'vbCrLf & MySvcHost.Description.Endpoints.FirstOrDefault.ListenUri.Authority & _
                '    'vbCrLf & MySvcHost.Description.Endpoints.FirstOrDefault.Contract.Name


                'End If
            End If
        Else
            SetUpResultsSvcHost()
        End If

    End Sub

    Private MyResultsSvcHost As ServiceHost '(GetType(WCFSvcLibr1.MasterSurveyDB))

    Private SvcHostDxnry As New Dictionary(Of String, ServiceHost)

    Private Sub SetUpResultsSvcHost()
        Dim sh As ServiceHost = Nothing
        MyResultsSvcHost = New ServiceHost(GetType(RsltsProviderSvcLibr.ResultsSvc))
        'If SvcHostDxnry.TryGetValue(MyMasterSurveyDBSvcHost.Description.Name, sh) Then

        'Else
        '    SvcHostDxnry.Add(MyMasterSurveyDBSvcHost.Description.Name, MyMasterSurveyDBSvcHost)
        'End If

        AddHandler MyResultsSvcHost.Opened, AddressOf UpdateStateTextBlock
        AddHandler MyResultsSvcHost.Faulted, AddressOf UpdateStateTextBlock
        AddHandler MyResultsSvcHost.Closing, AddressOf UpdateStateTextBlock
        AddHandler MyResultsSvcHost.Closed, AddressOf UpdateStateTextBlock
        MyResultsSvcHost.Open()

        If MyResultsSvcHost IsNot Nothing Then
            Dim tb As New TextBlock
            tb.Text = MyResultsSvcHost.Description.Name & " SetUp @ " & DateTime.Now.ToLongTimeString
            tb.Margin = New Thickness(2.0)
            Me.ServiceListbox.Items.Add(tb)
            Dim tb1 As New TextBlock
            tb1.Margin = New Thickness(2.0)
            tb1.Text = MyResultsSvcHost.Description.Endpoints.FirstOrDefault.ListenUri.OriginalString & " " & DateTime.Now.ToLongTimeString
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
        If MyResultsSvcHost IsNot Nothing Then
            Dim lbi As New ListBoxItem
            Dim tb As New TextBlock
            tb.Margin = New Thickness(2.0)
            tb.Text = MyResultsSvcHost.State.ToString & " " & DateTime.Now.ToShortTimeString
            lbi.Content = tb
            Me.StateListbox.Items.Add(lbi)
            Me.StateListbox.SelectedItem = lbi
            Me.StateListbox.ScrollIntoView(lbi)
        End If
        If MyResultsSvcHost.State = CommunicationState.Faulted Then
            SetUpResultsSvcHost()
        End If
    End Sub


    Private Sub WindowClosing() Handles Me.Closing
        If MyResultsSvcHost IsNot Nothing Then
            If MyResultsSvcHost.State <> CommunicationState.Closing AndAlso MyResultsSvcHost.State <> CommunicationState.Closed Then
                MyResultsSvcHost.Close()
                MyResultsSvcHost = Nothing
            End If
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If MyResultsSvcHost IsNot Nothing Then
            If MyResultsSvcHost.State <> CommunicationState.Closing AndAlso MyResultsSvcHost.State <> CommunicationState.Closed Then
                MyResultsSvcHost.Close()
                MyResultsSvcHost = Nothing
            End If
        End If
    End Sub

    Private Shared WithEvents SvcMonitor As RsltsProviderSvcLibr.ServiceMonitor
    Private Shared Sub ServiceMonitorPropertyChanged() Handles SvcMonitor.PropertyChanged
        ThisInstance.OperationsMonitorListBox.ItemsSource = Nothing
        ThisInstance.OperationsMonitorListBox.ItemsSource = RsltsProviderSvcLibr.ServiceMonitor.ServiceCalls

        ThisInstance.OperationsMonitorListBox.Items.MoveCurrentToLast()
        ThisInstance.OperationsMonitorListBox.SelectedItem = ThisInstance.OperationsMonitorListBox.Items.CurrentItem
        ThisInstance.OperationsMonitorListBox.ScrollIntoView(ThisInstance.OperationsMonitorListBox.SelectedItem)

    End Sub

    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button3.Click
        Me.OperationsMonitorListBox.ItemsSource = Nothing
        Me.OperationsMonitorListBox.ItemsSource = RsltsProviderSvcLibr.ServiceMonitor.ServiceCalls

    End Sub
End Class
