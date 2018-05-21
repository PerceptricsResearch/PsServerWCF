Imports System
Imports System.ServiceModel
Imports System.ComponentModel
Imports RespProviderSvcLibr
Imports System.Configuration


Class Window1

    Const baseaddressRespProviderSvc = "baseaddressRespProviderSvc"

    Private Shared ThisInstance As Window1
    Public Sub IamLoaded() Handles Me.Loaded
        ThisInstance = Me
        CommandSupport.MyBaseAddress = ConfigurationManager.AppSettings(baseaddressRespProviderSvc)
        RespProviderSvcMonitor = New RespProviderSvcLibr.ServiceMonitor
        'SetUpRespProviderSvcHost()
        Try
            CmdSvcMonitor = New RespProviderSvcLibr.CommandSvcMonitor
            SetUpCommandSvcHost()
        Catch ex As Exception
            MessageBox.Show("RespProviderSvcContainer.Window1.SetupCommandSvcHost reports error..." & ex.Message)
        End Try
    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click

        If MyRespProviderSvcHost IsNot Nothing Then
            If MyRespProviderSvcHost.State <> CommunicationState.Opened AndAlso MyRespProviderSvcHost.State <> CommunicationState.Opening Then
                SetUpRespProviderSvcHost()
            End If
        Else
            SetUpRespProviderSvcHost()
        End If

    End Sub

    Private MyRespProviderSvcHost As CustomSvcHost

    'Private SvcHostDxnry As New Dictionary(Of String, ServiceHost)

    Private Sub SetUpRespProviderSvcHost()
        MyRespProviderSvcHost = New RespProviderSvcLibr.CustomSvcHost(GetType(RespProviderSvcLibr.RespProviderSvc))
        AddHandler MyRespProviderSvcHost.Opened, AddressOf UpdateStateTextBlock
        AddHandler MyRespProviderSvcHost.Faulted, AddressOf UpdateStateTextBlock
        AddHandler MyRespProviderSvcHost.Closing, AddressOf UpdateStateTextBlock
        AddHandler MyRespProviderSvcHost.Closed, AddressOf UpdateStateTextBlock
        MyRespProviderSvcHost.Open()

        If MyRespProviderSvcHost IsNot Nothing Then
            Dim tb As New TextBlock
            tb.Text = MyRespProviderSvcHost.Description.Name & " SetUp @ " & DateTime.Now.ToLongTimeString
            tb.Margin = New Thickness(2.0)
            Me.ServiceListbox.Items.Add(tb)
            Dim tb1 As New TextBlock
            tb1.Margin = New Thickness(2.0)
            tb1.Text = MyRespProviderSvcHost.Description.Endpoints.FirstOrDefault.ListenUri.OriginalString & " " & DateTime.Now.ToLongTimeString
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
        If MyRespProviderSvcHost IsNot Nothing Then
            Dim lbi As New ListBoxItem
            Dim tb As New TextBlock
            tb.Margin = New Thickness(2.0)
            tb.Text = MyRespProviderSvcHost.State.ToString & " " & DateTime.Now.ToShortTimeString
            lbi.Content = tb
            Me.StateListbox.Items.Add(lbi)
            Me.StateListbox.SelectedItem = lbi
            Me.StateListbox.ScrollIntoView(lbi)
        End If
        If MyRespProviderSvcHost.State = CommunicationState.Faulted Then
            SetUpRespProviderSvcHost()
        End If
    End Sub

    Private Sub WindowClosing() Handles Me.Closing
        If MyRespProviderSvcHost IsNot Nothing Then
            If MyRespProviderSvcHost.State <> CommunicationState.Closing AndAlso MyRespProviderSvcHost.State <> CommunicationState.Closed Then
                MyRespProviderSvcHost.Close()
                MyRespProviderSvcHost = Nothing
            End If
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If MyRespProviderSvcHost IsNot Nothing Then
            If MyRespProviderSvcHost.State <> CommunicationState.Closing AndAlso MyRespProviderSvcHost.State <> CommunicationState.Closed Then
                MyRespProviderSvcHost.Close()
                MyRespProviderSvcHost = Nothing
            End If
        End If
    End Sub

    Private Shared WithEvents RespProviderSvcMonitor As RespProviderSvcLibr.ServiceMonitor
    Private Shared Sub PgItemsColxnSvcMonitorPropertyChanged() Handles RespProviderSvcMonitor.PropertyChanged
        ThisInstance.OperationsMonitorListBox.ItemsSource = Nothing
        ThisInstance.OperationsMonitorListBox.ItemsSource = RespProviderSvcLibr.ServiceMonitor.ServiceCalls

        ThisInstance.OperationsMonitorListBox.Items.MoveCurrentToLast()
        ThisInstance.OperationsMonitorListBox.SelectedItem = ThisInstance.OperationsMonitorListBox.Items.CurrentItem
        ThisInstance.OperationsMonitorListBox.ScrollIntoView(ThisInstance.OperationsMonitorListBox.SelectedItem)

    End Sub

    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.OperationsMonitorListBox.ItemsSource = Nothing
        Me.OperationsMonitorListBox.ItemsSource = RespProviderSvcLibr.ServiceMonitor.ServiceCalls

    End Sub

#Region "Command Svc Stuff"



    Private Shared WithEvents CmdSvcMonitor As RespProviderSvcLibr.CommandSvcMonitor
    Private Shared Sub ServiceMonitorPropertyChanged() Handles CmdSvcMonitor.PropertyChanged
        ThisInstance.CmdWindow.CommandsListBox.ItemsSource = Nothing
        ThisInstance.CmdWindow.CommandsListBox.ItemsSource = RespProviderSvcLibr.CommandSvcMonitor.CommandSvcCalls
        ThisInstance.CmdWindow.CommandsListBox.Items.MoveCurrentToLast()
        ThisInstance.CmdWindow.CommandsListBox.SelectedItem = ThisInstance.CmdWindow.CommandsListBox.Items.CurrentItem
        ThisInstance.CmdWindow.CommandsListBox.ScrollIntoView(ThisInstance.CmdWindow.CommandsListBox.SelectedItem)
        'Dim lofT As List(Of KeyValuePair(Of String, String)) = ThisInstance.CmdWindow.CommandsListBox.ItemsSource

        'ThisInstance.CommandHandler(lofT.LastOrDefault)
        ThisInstance.CommandHandler(RespProviderSvcLibr.CommandSvcMonitor.CommandIssued)
    End Sub

    Private Sub CommandHandler(ByVal _cmdPkg As CmdSvcClassLibrary.CommandPackage) 'ByVal _commandobject As KeyValuePair(Of String, String))

        If _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.ExposeEndPoint Then
            CommandSupport.StartCmd(_cmdPkg) 'this is the queueURI of the TargetPostingSvc....NEED TO LOOK AT CALLER HERE>>
        ElseIf _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.RetractEndPoint Then
            CommandSupport.StopCmd(_cmdPkg.EndPtSuffix)
        ElseIf _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.ExposeWithDataContext Then
            CommandSupport.StartCmd(_cmdPkg) 'just for now...
        End If
        'Dim cmd As String = _commandobject.Key.Split("/").FirstOrDefault.ToLower
        'If cmd.StartsWith("start") Then
        '    CommandSupport.StartCmd(_commandobject.Value) 'this is the queueURI of the TargetPostingSvc....NEED TO LOOK AT CALLER HERE>>
        'ElseIf cmd.StartsWith("stop") Then
        '    CommandSupport.StopCmd(_commandobject.Value)
        'End If
        Me.CmdWindow.ActivePostingSvcs_Listbox.ItemsSource = Nothing
        Me.CmdWindow.ActivePostingSvcs_Listbox.ItemsSource = CommandSupport.ActiveServicesList
        Me.CmdWindow.DormantPostingSvcs_Listbox.ItemsSource = Nothing
        Me.CmdWindow.DormantPostingSvcs_Listbox.ItemsSource = CommandSupport.DormantServicesList
    End Sub

    Private MyCommandSvcHost As CustomSvcHost
    Private Sub SetUpCommandSvcHost()

        Try
            MyCommandSvcHost = New CustomSvcHost(GetType(CommandSvc))

            AddHandler MyCommandSvcHost.Opened, AddressOf UpdateCommandSvcMonitor
            AddHandler MyCommandSvcHost.Faulted, AddressOf UpdateCommandSvcMonitor
            AddHandler MyCommandSvcHost.Closing, AddressOf UpdateCommandSvcMonitor
            AddHandler MyCommandSvcHost.Closed, AddressOf UpdateCommandSvcMonitor
            MyCommandSvcHost.Open()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub UpdateCommandSvcMonitor(ByVal sender As Object, ByVal e As System.EventArgs)
        If MyCommandSvcHost.State = CommunicationState.Faulted Then
            'CommandSvcMonitor.Update_CommandSvcCalls("Service Faulted", "MyCommandSvcHost")
            Me.CmdWindow.Background = New SolidColorBrush(Colors.Red)
            Me.CmdWindow.CmdSvc_State_Textblock.Text = " Faulted " & DateTime.Now.ToLongTimeString
            Me.CmdWindow.Show()
            SetUpCommandSvcHost()
        End If
        If Me.MyCommandSvcHost.State = CommunicationState.Opened Then
            Me.CmdWindow.Background = New SolidColorBrush(Colors.White)
        End If
        'CommandSvcMonitor.Update_CommandSvcCalls("CommandSvcHost", MyCommandSvcHost.State.ToString)
        Me.CmdWindow.CmdSvc_State_Textblock.Text = MyCommandSvcHost.State.ToString & " " & DateTime.Now.ToLongTimeString
    End Sub

    Private WithEvents CmdWindow As New CommandWindow
    Private Sub Button_CommandWindow_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If Me.CmdWindow.WindowState = Windows.WindowState.Minimized Then
            Me.CmdWindow.WindowState = Windows.WindowState.Normal
        Else
            Me.CmdWindow.Show()
        End If


    End Sub
    Private Sub CmdWindow_Closing(ByVal sender As Object, ByVal e As CancelEventArgs) Handles CmdWindow.Closing
        e.Cancel = True
        CType(sender, CommandWindow).WindowState = Windows.WindowState.Minimized
    End Sub
#End Region
End Class

''' <summary>
''' CommandSupport for RespProvider Services Container
''' </summary>
''' <remarks></remarks>
Public Class CommandSupport
    Public Shared SvcHostDxnry As New Dictionary(Of String, CustomSvcHost)

    Public Shared MyBaseAddress As String = "ignatz" '"http://rents/hm/PgItemColxnSvc/"

    Public Shared Sub StartCmd(ByVal _cmdPkg As CmdSvcClassLibrary.CommandPackage) 'ByVal _EptSuffix As String)
        Dim _EptSuffix = _cmdPkg.EndPtSuffix
        If SvcHostDxnry.ContainsKey(_EptSuffix) Then 'TryGetValue(_queueUri, xxxsvchost) Then
            If SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Opening AndAlso SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Opened Then
                Try
                    Dim newPSHost = New CustomSvcHost(GetType(RespProviderSvc))
                    newPSHost.EndPtSuffix = _EptSuffix
                    If _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.AddDataContext Or _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.ExposeWithDataContext Then
                        newPSHost.DC_Pkg = _cmdPkg.DC_Pkg
                        newPSHost.DataContextConnectionString = _cmdPkg.DC_Cnxn
                    End If
                    newPSHost.Description.Name = CreateSvcName(_EptSuffix)

                    Dim endpt = newPSHost.AddServiceEndpoint(GetType(IRespProviderSvc), _
                                                             New BasicHttpBinding("binding1"), _
                                                             MyBaseAddress & _EptSuffix)
                    endpt.Name = newPSHost.Description.Name '& "altered"

                    Dim tcpendpt = newPSHost.AddServiceEndpoint(GetType(IRespProviderSvc), New NetTcpBinding(), MyBaseAddress & _EptSuffix)

                    SvcHostDxnry.Item(_EptSuffix) = newPSHost
                    AddHandler newPSHost.Faulted, AddressOf SvcFaulted_Handler

                    newPSHost.Open()

                Catch ex As Exception
                    MessageBox.Show("RespProviderSvcContainer.Window1.StartCmd encountered error..." & ex.Message)
                End Try
            End If
        Else
            Try
                Dim newPSHost = New CustomSvcHost(GetType(RespProviderSvc))
                newPSHost.EndPtSuffix = _EptSuffix
                If _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.AddDataContext Or _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.ExposeWithDataContext Then
                    newPSHost.DC_Pkg = _cmdPkg.DC_Pkg
                    newPSHost.DataContextConnectionString = _cmdPkg.DC_Cnxn
                End If
                newPSHost.Description.Name = CreateSvcName(_EptSuffix)

                Dim endpt = newPSHost.AddServiceEndpoint(GetType(IRespProviderSvc), New BasicHttpBinding("binding1"), _
                                                         MyBaseAddress & _EptSuffix)
                endpt.Name = newPSHost.Description.Name '& "altered"

                'Dim tcpendpt = newPSHost.AddServiceEndpoint(GetType(IPgItemColxnSvc), New NetTcpBinding(), MyBaseAddress & _EptSuffix)

                SvcHostDxnry.Add(_EptSuffix, newPSHost)
                AddHandler newPSHost.Faulted, AddressOf SvcFaulted_Handler

                newPSHost.Open()

            Catch ex As Exception
                MessageBox.Show("RespProviderSvcContainer.Window1.StartCmd encountered error..." & ex.Message)
            End Try
        End If
    End Sub

    Public Shared Sub StopCmd(ByVal _EptSuffix As String)
        'Dim xxxsvchost As ServiceHost = Nothing
        If SvcHostDxnry.ContainsKey(_EptSuffix) Then 'TryGetValue(_queueUri, xxxsvchost) Then
            Try

                If SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Closing AndAlso SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Closed Then
                    SvcHostDxnry.Item(_EptSuffix).Close()
                End If
            Catch ex As Exception
                MessageBox.Show("RespProviderSvcContainer.Window1.StopCmd encountered error..." & ex.Message)
            End Try

        End If

    End Sub

    Public Shared Function ActiveServicesList() As List(Of KeyValuePair(Of String, String))
        Dim rslt = From kvp In SvcHostDxnry _
                   Where kvp.Value.State = CommunicationState.Opened Or kvp.Value.State = CommunicationState.Opening _
                   Select New KeyValuePair(Of String, String)(kvp.Key, kvp.Value.State.ToString)
        rslt.DefaultIfEmpty(New KeyValuePair(Of String, String)("", ""))
        Return rslt.ToList
    End Function

    Public Shared Function DormantServicesList() As List(Of KeyValuePair(Of String, String))
        Dim rslt = From kvp In SvcHostDxnry _
                   Where kvp.Value.State = CommunicationState.Closed Or kvp.Value.State = CommunicationState.Closing _
                   Select New KeyValuePair(Of String, String)(kvp.Key, kvp.Value.State.ToString)
        rslt.DefaultIfEmpty(New KeyValuePair(Of String, String)("", ""))
        Return rslt.ToList
    End Function
    ''' <summary>
    ''' Returns the _EptSuffix...not necessary to do this as a function...left it this way for consistency with prior code...
    ''' </summary>
    ''' <param name="_EptSuffix"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CreateSvcName(ByVal _EptSuffix As String) As String
        Return _EptSuffix
        'Return Strings.Split(_EptSuffix, "/").Last
    End Function


    Private Shared Sub SvcFaulted_Handler(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim svchost As CustomSvcHost = CType(sender, CustomSvcHost)
        svchost.FaultsColxn.Add(DateTime.Now)
        'CommandSvcMonitor.Update_CommandSvcCalls("ServiceHostFaulted " & svchost.Description.Endpoints.FirstOrDefault.Name, DateTime.Now.ToShortTimeString)
        'RemoveHandler svchost.Faulted, AddressOf SvcFaulted_Handler 'not sure if I need this....
        Dim _EptSuffix As String = svchost.EndPtSuffix
        Dim _Dc_Cnxn As String = svchost.DataContextConnectionString
        Dim _DC_Pkg = svchost.DC_Pkg
        Dim _FaultsColxn = svchost.FaultsColxn
        If SvcHostDxnry.TryGetValue(_EptSuffix, svchost) Then
            Try
                svchost = New CustomSvcHost(GetType(RespProviderSvc))
                svchost.EndPtSuffix = _EptSuffix
                svchost.DataContextConnectionString = _Dc_Cnxn
                svchost.DC_Pkg = _DC_Pkg
                svchost.FaultsColxn = _FaultsColxn

                svchost.Description.Name = CreateSvcName(_EptSuffix)
                Dim endpt = svchost.AddServiceEndpoint(GetType(IRespProviderSvc), _
                                                       New BasicHttpBinding("binding1"), MyBaseAddress & _EptSuffix)
                endpt.Name = svchost.Description.Name '& "altered"
                Dim tcpendpt = svchost.AddServiceEndpoint(GetType(IRespProviderSvc), New NetTcpBinding(), MyBaseAddress & _EptSuffix)

                SvcHostDxnry.Item(_EptSuffix) = svchost
                AddHandler svchost.Faulted, AddressOf SvcFaulted_Handler

                svchost.Open()

            Catch ex As Exception
                MessageBox.Show("RespProviderSvcContainer.Window1.SvcFaulterHandler encountered error..." & ex.Message)
            End Try
        End If
    End Sub
End Class
