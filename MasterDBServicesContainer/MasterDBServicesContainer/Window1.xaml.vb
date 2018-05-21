Imports System
Imports System.ServiceModel
Imports System.ComponentModel
Imports MasterDBSvcLibr
Imports System.Configuration
Class Window1

    Const baseaddressMasterSurveyDBSvc = "baseaddressMasterSurveyDBSvc"
    Private Shared ThisInstance As Window1
    Public Sub IamLoaded() Handles Me.Loaded

        ThisInstance = Me
        CommandSupport.MyBaseAddress = ConfigurationManager.AppSettings(baseaddressMasterSurveyDBSvc)
        SvcMonitor = New MasterDBSvcLibr.ServiceMonitor
        'SetUpMasterSurveyDBSvcHost()
        Try
            CmdSvcMonitor = New MasterDBSvcLibr.CommandSvcMonitor
            SetUpCommandSvcHost()
        Catch ex As Exception
            MessageBox.Show("MasterDBServicesContainer.Window1.SetupCommandSvcHost reports error..." & ex.Message)
        End Try
    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        
        If MyMasterSurveyDBSvcHost IsNot Nothing Then
            If MyMasterSurveyDBSvcHost.State <> CommunicationState.Opened AndAlso MyMasterSurveyDBSvcHost.State <> CommunicationState.Opening Then
                SetUpMasterSurveyDBSvcHost()
                'If MyMasterSurveyDBSvcHost IsNot Nothing Then
                '    Me.ServiceListbox.Items.Add(MyMasterSurveyDBSvcHost.Description.Name)
                '    Me.ServiceListbox.Items.Add(MyMasterSurveyDBSvcHost.Description.Endpoints.FirstOrDefault.ListenUri.OriginalString) '& _
                '    'vbCrLf & MySvcHost.Description.Endpoints.FirstOrDefault.ListenUri.Host & _
                '    'vbCrLf & MySvcHost.Description.Endpoints.FirstOrDefault.ListenUri.Authority & _
                '    'vbCrLf & MySvcHost.Description.Endpoints.FirstOrDefault.Contract.Name


                'End If
            End If
        Else
            SetUpMasterSurveyDBSvcHost()
        End If
        
    End Sub

    Private MyMasterSurveyDBSvcHost As ServiceHost '(GetType(WCFSvcLibr1.MasterSurveyDB))

    Private SvcHostDxnry As New Dictionary(Of String, ServiceHost)

    Private Sub SetUpMasterSurveyDBSvcHost()
        Dim sh As ServiceHost = Nothing
        MyMasterSurveyDBSvcHost = New ServiceHost(GetType(MasterDBSvcLibr.MasterSurveyDBSvc))
        'If SvcHostDxnry.TryGetValue(MyMasterSurveyDBSvcHost.Description.Name, sh) Then

        'Else
        '    SvcHostDxnry.Add(MyMasterSurveyDBSvcHost.Description.Name, MyMasterSurveyDBSvcHost)
        'End If

        AddHandler MyMasterSurveyDBSvcHost.Opened, AddressOf UpdateStateTextBlock
        AddHandler MyMasterSurveyDBSvcHost.Faulted, AddressOf UpdateStateTextBlock
        AddHandler MyMasterSurveyDBSvcHost.Closing, AddressOf UpdateStateTextBlock
        AddHandler MyMasterSurveyDBSvcHost.Closed, AddressOf UpdateStateTextBlock
        MyMasterSurveyDBSvcHost.Open()

        If MyMasterSurveyDBSvcHost IsNot Nothing Then
            Dim tb As New TextBlock
            tb.Text = MyMasterSurveyDBSvcHost.Description.Name & " SetUp @ " & DateTime.Now.ToLongTimeString
            tb.Margin = New Thickness(2.0)
            Me.ServiceListbox.Items.Add(tb)
            Dim tb1 As New TextBlock
            tb1.Margin = New Thickness(2.0)
            tb1.Text = MyMasterSurveyDBSvcHost.Description.Endpoints.FirstOrDefault.ListenUri.OriginalString & " " & DateTime.Now.ToLongTimeString
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
        If MyMasterSurveyDBSvcHost IsNot Nothing Then
            Dim lbi As New ListBoxItem
            Dim tb As New TextBlock
            tb.Margin = New Thickness(2.0)
            tb.Text = MyMasterSurveyDBSvcHost.State.ToString & " " & DateTime.Now.ToShortTimeString
            lbi.Content = tb
            Me.StateListbox.Items.Add(lbi)
            Me.StateListbox.SelectedItem = lbi
            Me.StateListbox.ScrollIntoView(lbi)
        End If
        If MyMasterSurveyDBSvcHost.State = CommunicationState.Faulted Then
            SetUpMasterSurveyDBSvcHost()
        End If
    End Sub


    Private Sub WindowClosing() Handles Me.Closing
        If MyMasterSurveyDBSvcHost IsNot Nothing Then
            If MyMasterSurveyDBSvcHost.State <> CommunicationState.Closing AndAlso MyMasterSurveyDBSvcHost.State <> CommunicationState.Closed Then
                MyMasterSurveyDBSvcHost.Close()
                MyMasterSurveyDBSvcHost = Nothing
            End If
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If MyMasterSurveyDBSvcHost IsNot Nothing Then
            If MyMasterSurveyDBSvcHost.State <> CommunicationState.Closing AndAlso MyMasterSurveyDBSvcHost.State <> CommunicationState.Closed Then
                MyMasterSurveyDBSvcHost.Close()
                MyMasterSurveyDBSvcHost = Nothing
            End If
        End If
    End Sub

    Private Shared WithEvents SvcMonitor As MasterDBSvcLibr.ServiceMonitor
    Private Shared Sub ServiceMonitorPropertyChanged() Handles SvcMonitor.PropertyChanged
        ThisInstance.OperationsMonitorListBox.ItemsSource = Nothing
        ThisInstance.OperationsMonitorListBox.ItemsSource = MasterDBSvcLibr.ServiceMonitor.ServiceCalls

        ThisInstance.OperationsMonitorListBox.Items.MoveCurrentToLast()
        ThisInstance.OperationsMonitorListBox.SelectedItem = ThisInstance.OperationsMonitorListBox.Items.CurrentItem
        ThisInstance.OperationsMonitorListBox.ScrollIntoView(ThisInstance.OperationsMonitorListBox.SelectedItem)

    End Sub

    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.OperationsMonitorListBox.ItemsSource = Nothing
        Me.OperationsMonitorListBox.ItemsSource = MasterDBSvcLibr.ServiceMonitor.ServiceCalls

    End Sub


#Region "Command Svc Stuff"



    Private Shared WithEvents CmdSvcMonitor As MasterDBSvcLibr.CommandSvcMonitor
    Private Shared Sub CmdSvcMonitorPropertyChanged() Handles CmdSvcMonitor.PropertyChanged
        ThisInstance.CmdWindow.CommandsListBox.ItemsSource = Nothing
        ThisInstance.CmdWindow.CommandsListBox.ItemsSource = MasterDBSvcLibr.CommandSvcMonitor.CommandSvcCalls
        ThisInstance.CmdWindow.CommandsListBox.Items.MoveCurrentToLast()
        ThisInstance.CmdWindow.CommandsListBox.SelectedItem = ThisInstance.CmdWindow.CommandsListBox.Items.CurrentItem
        ThisInstance.CmdWindow.CommandsListBox.ScrollIntoView(ThisInstance.CmdWindow.CommandsListBox.SelectedItem)
        'Dim lofT As List(Of KeyValuePair(Of String, String)) = ThisInstance.CmdWindow.CommandsListBox.ItemsSource

        'ThisInstance.CommandHandler(lofT.LastOrDefault)
        ThisInstance.CommandHandler(MasterDBSvcLibr.CommandSvcMonitor.CommandIssued)

    End Sub

    Private Sub CommandHandler(ByVal _cmdPkg As CmdSvcClassLibrary.CommandPackage)

        If _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.ExposeEndPoint Then
            CommandSupport.StartCmd(_cmdPkg) 'this is the queueURI of the TargetPostingSvc....NEED TO LOOK AT CALLER HERE>>
        ElseIf _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.RetractEndPoint Then
            CommandSupport.StopCmd(_cmdPkg.EndPtSuffix)
        ElseIf _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.ExposeWithDataContext Then
            CommandSupport.StartCmd(_cmdPkg) 'just for now...
        End If
        'Me.CmdWindow.ActivePostingSvcs_Listbox.ItemsSource = Nothing
        Me.CmdWindow.ActivePostingSvcs_Listbox.ItemsSource = CommandSupport.ActiveServicesList
        Me.CmdWindow.DormantPostingSvcs_Listbox.ItemsSource = Nothing
        Me.CmdWindow.DormantPostingSvcs_Listbox.ItemsSource = CommandSupport.DormantServicesList
    End Sub

    Private WithEvents MyCommandSvcHost As CustomSvcHost
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
''' CommandSupport for MasterSurveyDBServices Container
''' </summary>
''' <remarks></remarks>
Public Class CommandSupport
    Public Shared SvcHostDxnry As New Dictionary(Of String, CustomSvcHost)

    Public Shared MyBaseAddress As String = "ignatz" '"http://rents/hm/MasterSurveyDBSvc/" 'this needs to get populated by ConfigMAnager...

    Public Shared Sub StartCmd(ByVal _cmdPkg As CmdSvcClassLibrary.CommandPackage) 'ByVal _EptSuffix As String)
        Dim _EptSuffix = _cmdPkg.EndPtSuffix
        If SvcHostDxnry.ContainsKey(_EptSuffix) Then 'TryGetValue(_queueUri, xxxsvchost) Then
            If SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Opening AndAlso SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Opened Then
                Try
                    Dim newPSHost = New CustomSvcHost(GetType(IMasterSurveyDBSvc))
                    newPSHost.EndPtSuffix = _EptSuffix
                    If _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.AddDataContext Or _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.ExposeWithDataContext Then
                        newPSHost.DC_Pkg = _cmdPkg.DC_Pkg
                        newPSHost.DataContextConnectionString = _cmdPkg.DC_Cnxn
                        ServiceMonitor.Update_ServiceCalls("<<Cmd Start Contains DC_Cnxn>>" & DateTime.Now.ToLongTimeString, _
                                             "_cmdPkg.DC_Cnxn=<" & _cmdPkg.DC_Cnxn & ">)")

                    End If
                    newPSHost.Description.Name = CreateSvcName(_EptSuffix)

                    Dim endpt = newPSHost.AddServiceEndpoint(GetType(IMasterSurveyDBSvc), _
                                                             New BasicHttpBinding(BasicHttpSecurityMode.Message), _
                                                             MyBaseAddress & _EptSuffix)
                    endpt.Name = newPSHost.Description.Name '& "altered"

                    'Dim tcpendpt = newPSHost.AddServiceEndpoint(GetType(IMasterSurveyDBSvc), New NetTcpBinding(), MyBaseAddress & _EptSuffix)

                    SvcHostDxnry.Item(_EptSuffix) = newPSHost
                    'AddHandler newPSHost.Faulted, AddressOf SvcFaulted_Handler

                    newPSHost.Open()
                    ServiceMonitor.Update_ServiceCalls("CMD Start Contains" & DateTime.Now.ToLongTimeString, _
                                              "EptSuffix=<" & _EptSuffix & ">)")

                Catch ex As Exception
                    MessageBox.Show("MasterSurveyDBSvcContainer.Window1.StartCmd encountered error..." & ex.Message)
                End Try
            End If
        Else
            Try
                Dim newPSHost = New CustomSvcHost(GetType(IMasterSurveyDBSvc))
                newPSHost.EndPtSuffix = _EptSuffix
                If _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.AddDataContext Or _cmdPkg.CmdVb = CmdSvcClassLibrary.CmdVerb.ExposeWithDataContext Then
                    newPSHost.DC_Pkg = _cmdPkg.DC_Pkg
                    newPSHost.LastOperationKVP = New KeyValuePair(Of String, String)(DateTime.Now.ToLongTimeString, "CMD Start")
                    newPSHost.DataContextConnectionString = _cmdPkg.DC_Cnxn
                    ServiceMonitor.Update_ServiceCalls("<<Cmd Start Add DC_Cnxn>>" & DateTime.Now.ToLongTimeString, _
                                             "_cmdPkg.DC_Cnxn=<" & _cmdPkg.DC_Cnxn & ">)")

                End If
                newPSHost.Description.Name = CreateSvcName(_EptSuffix)

                Dim endpt = newPSHost.AddServiceEndpoint(GetType(IMasterSurveyDBSvc), New BasicHttpBinding("binding1"), _
                                                         MyBaseAddress & _EptSuffix)
                endpt.Name = newPSHost.Description.Name '& "altered"

                'Dim tcpendpt = newPSHost.AddServiceEndpoint(GetType(IMasterSurveyDBSvc), New NetTcpBinding(), MyBaseAddress & _EptSuffix)

                SvcHostDxnry.Add(_EptSuffix, newPSHost)
                'AddHandler newPSHost.Faulted, AddressOf SvcFaulted_Handler

                newPSHost.Open()
                ServiceMonitor.Update_ServiceCalls("CMD Start Add to SvcDxnry" & DateTime.Now.ToLongTimeString, _
                                             "EptSuffix=<" & _EptSuffix & ">)")

            Catch ex As Exception
                MessageBox.Show("MasterSurveyDBSvcContainer.Window1.StartCmd encountered error..." & ex.Message)
            End Try
        End If
    End Sub

    Public Shared Sub StopCmd(ByVal _EptSuffix As String)
        'Dim xxxsvchost As ServiceHost = Nothing
        If SvcHostDxnry.ContainsKey(_EptSuffix) Then 'TryGetValue(_queueUri, xxxsvchost) Then
            Try
                ServiceMonitor.Update_ServiceCalls("CMD Stop " & DateTime.Now.ToLongTimeString, _
                                             "EptSuffix=<" & _EptSuffix & ">)")

                If SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Closing AndAlso SvcHostDxnry.Item(_EptSuffix).State <> CommunicationState.Closed Then
                    SvcHostDxnry.Item(_EptSuffix).Close()
                End If
            Catch ex As Exception
                MessageBox.Show("MasterSurveyDBSvcContainer.Window1.StopCmd encountered error..." & ex.Message)
            End Try

        End If

    End Sub
    Public Shared ReadOnly Property ActiveServicesList() As List(Of KeyValuePair(Of String, CustomSvcHost))
        Get
            Return FnActiveServicesList()
        End Get
    End Property


    Public Shared Function FnActiveServicesList() As List(Of KeyValuePair(Of String, CustomSvcHost))
        Dim rslt = From kvp In SvcHostDxnry _
                   Where kvp.Value.State = CommunicationState.Opened Or kvp.Value.State = CommunicationState.Opening _
                   Select kvp 'New KeyValuePair(Of String, CustomSvcHost)(kvp.Key & " " & kvp.Value.State.ToString, _
        'kvp.Value)
        'Select New CSHButton With {.Content = kvp.Key & " " & kvp.Value.State.ToString, _
        '                                                           .Tag = kvp.Value}
        'Select New KeyValuePair(Of CSHButton, String)(New CSHButton With {.Content = kvp.Key, _
        '                                                            .Tag = kvp.Value}, kvp.Value.State.ToString)
        rslt.DefaultIfEmpty(New KeyValuePair(Of String, CustomSvcHost)("", Nothing))
        'New KeyValuePair(Of CSHButton, String)(New CSHButton With {.Content = "NotSet", _
        '                                                                       .Tag = "NotSet"}, ""))
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
        'sv.Update_CommandSvcCalls("ServiceHostFaulted " & svchost.Description.Endpoints.FirstOrDefault.Name, DateTime.Now.ToShortTimeString)
        'RemoveHandler svchost.Faulted, AddressOf SvcFaulted_Handler 'not sure if I need this....
        Dim _EptSuffix As String = svchost.EndPtSuffix
        ServiceMonitor.Update_ServiceCalls("<<Faulted>>" & DateTime.Now.ToLongTimeString, _
                                             "EptSuffix=<" & _EptSuffix & ">)")

        Dim _Dc_Cnxn As String = svchost.DataContextConnectionString
        Dim _DC_Pkg = svchost.DC_Pkg
        Dim _FaultsColxn = svchost.FaultsColxn
        If SvcHostDxnry.TryGetValue(_EptSuffix, svchost) Then
            Try
                'svchost = New CustomSvcHost(GetType(MasterSurveyDBSvc))
                'svchost.EndPtSuffix = _EptSuffix
                'svchost.DataContextConnectionString = _Dc_Cnxn
                'svchost.DC_Pkg = _DC_Pkg
                'svchost.FaultsColxn = _FaultsColxn

                'svchost.Description.Name = CreateSvcName(_EptSuffix)
                'Dim endpt = svchost.AddServiceEndpoint(GetType(IMasterSurveyDBSvc), _
                '                                       New BasicHttpBinding(BasicHttpSecurityMode.Message), MyBaseAddress & _EptSuffix)
                'endpt.Name = svchost.Description.Name '& "altered"
                'Dim tcpendpt = svchost.AddServiceEndpoint(GetType(IMasterSurveyDBSvc), New NetTcpBinding(), MyBaseAddress & _EptSuffix)

                'SvcHostDxnry.Item(_EptSuffix) = svchost
                'AddHandler svchost.Faulted, AddressOf SvcFaulted_Handler

                'svchost.Open()

            Catch ex As Exception
                MessageBox.Show("MasterSurveyDBSvcContainer.Window1.SvcFaulterHandler encountered error..." & ex.Message)
            End Try
        End If
    End Sub


End Class
'Public Class CSHButton
'    Inherits Button

'    Public Sub New()
'        AddHandler Me.Click, AddressOf MyClickHandler
'        Me.Background = New SolidColorBrush(Colors.Teal)
'    End Sub

'    Private myHostWindow As Window
'    Private Sub MyClickHandler()
'        myHostWindow = New Window
'        myHostWindow.Title = "SvcHost " & CType(Me.Tag, CustomSvcHost).EndPtSuffix & " " & CType(Me.Tag, CustomSvcHost).State.ToString
'        myHostWindow.SizeToContent = SizeToContent.WidthAndHeight
'        Dim view As New CustomSvcHostView
'        Dim model As New CustomSvcHostModel With {.MyCustomSvcHost = Me.Tag}
'        view.MyModel = model
'        myHostWindow.Content = view
'        myHostWindow.WindowStyle = WindowStyle.ToolWindow
'        myHostWindow.Show()
'    End Sub
'End Class