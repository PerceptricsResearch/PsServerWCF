Imports System.ServiceModel
Imports System.ComponentModel
Imports System.Windows.Controls
Imports System.Windows
Imports System.Windows.Media
Imports CmdInfrastructureNS

Partial Public Class WPFSvcContainerWindow
    Inherits UserControl
    Implements INotifyPropertyChanged

#Region "MyServiceTypeProperties and New(s) For this Window"
    Private _MySvcType As Type
    ''' <summary>
    ''' Pass a FullyQualifedType in here...Namespace and all...make sure the App.Config in your Executable has this 
    ''' type defined as a Service with a contract and a behaviorconfig and a binding...
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MySvcType() As Type
        Get
            Return _MySvcType
        End Get
        Set(ByVal value As Type)
            _MySvcType = value
        End Set
    End Property

    Private _MyISvcType As Type
    '''' <summary>
    '''' Pass a FullyQualifedInterfaceType in here...Namespace and all...make sure the App.Config in your Executable has this 
    '''' type defined as a Service with a contract and a behaviorconfig and a binding...
    '''' </summary>
    '''' <value></value>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Property MyISvcType() As Type
    '    Get
    '        Return _MyISvcType
    '    End Get
    '    Set(ByVal value As Type)
    '        _MyISvcType = value
    '    End Set
    'End Property
    ''' <summary>
    ''' This is passed in as a parameter from SvcElementUC.LaunchSvcofT_Btn_Click...it comes from the App.Config
    ''' </summary>
    ''' <remarks></remarks>
    Private MyBaseAddress As String

    'Private MyCmdSupport As CmdSvcClassLibrary.CommandSupport = New CmdSvcClassLibrary.CommandSupport

    Public Sub New(ByVal _myServiceType As Type, ByVal _mySvcBaseAddress As String)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        If _myServiceType Is Nothing Then
            MessageBox.Show("MyServiceType is Nothing...stop and start buttons are diabled...")
            Me.Button1.IsEnabled = False
            Me.Button2.IsEnabled = False
            Me.Button3.IsEnabled = False
            Me.ButtonCSHView.IsEnabled = False
        Else
            MySvcType = _myServiceType
            Me.MyBaseAddress = _mySvcBaseAddress
            '<<<<This is REALLY REALLY IMPORTANT...it defines T for the CommandSupoort Class...if you don't do this, it no work!!
            'MyISvcType = MyCmdSupport.ToIType(MySvcType)
            'If MyISvcType IsNot Nothing Then
            '    MessageBox.Show("This Container is hosting " & MySvcType.Name & " with Interface " & MyISvcType.Name)
            '    ' Me.Title = MySvcType.Name & " Hosting Container"
            '    MyCmdSupport.MyBaseAddress = _mySvcBaseAddress
            '    Me.MyBaseAddress = _mySvcBaseAddress

            'Else
            '    MessageBox.Show("This Container is hosting nothing...could not resove Interface for " & MySvcType.Name)
            '    Me.Button1.IsEnabled = False
            '    Me.Button2.IsEnabled = False
            '    Me.Button3.IsEnabled = False
            '    Me.ButtonCSHView.IsEnabled = False
            'End If

        End If
    End Sub



#End Region '"MyServiceTypeProperties and New(s) for this Window" ...put the Service(Of T) and IService(Of T) here..

#Region "MainWindow Stuff"
    'Private ThisInstance As WPFSvcContainerWindow
    Public Sub IamLoaded() 'Handles Me.Loaded
        'SetUpRespDispatcherSvcHost() 'automatically starts service
        ' SetUpCommandSvcHost()
    End Sub

    Private WithEvents MyServiceOf_T_SvcHost As CustomSvcHost

    Private Sub SetUprSvcHost()
        Try
            MyServiceOf_T_SvcHost = New CustomSvcHost(MySvcType) 'GetType(MyServiceType))
            'Dim netmqbinding As New NetMsmqBinding("NetMsmqBinding_IRespDispatcherSvc")
            'Dim endpt = MyRespDispatcherSvcHost.AddServiceEndpoint(GetType(WCFSvcLibr1.IRespDispatcherSvc), _
            '                                          netmqbinding,someaddressstring...)

            With MyServiceOf_T_SvcHost
                .SvcMonitor = New CmdSvcClassLibrary.ServiceMonitor
                AddHandler .SvcMonitor.PropertyChanged, AddressOf ServiceMonitorPropertyChanged
                .SvcMonitor.Update_ServiceInfoColxn(.Description.Name & .GetHashCode.ToString, _
                                                               .Description.Endpoints, .State)
                .SvcMonitor.Update_ServiceCalls("<<New>>" & .Description.Name & .GetHashCode, .State.ToString)
                AddHandler .Opened, AddressOf UpdateSvcMonitorState
                AddHandler .Faulted, AddressOf ServiceFaultedHandler
                AddHandler .Closing, AddressOf UpdateSvcMonitorState
                AddHandler .Closed, AddressOf UpdateSvcMonitorState
                .Open()
                Using EvLog As New EventLog()
                    EvLog.Source = "WPF Svc Window"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Started WCF service: " & .Description.Name & "  " & .State.ToString, EventLogEntryType.SuccessAudit)
                End Using
                .SvcMonitor.Update_ServiceCalls("<<Open>>" & .Description.Name & .GetHashCode, .State.ToString)
            End With
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "WPF Svc Window"
                EvLog.Log = "Application"
                EvLog.WriteEntry("SetUpMyServiceofT reports: " & MySvcType.Name & "  " & ex.Message, EventLogEntryType.Information)
            End Using
            MessageBox.Show("WPFSvCContainerWindow.SetUpMyServiceofT reports error..." & ex.Message)
        End Try

    End Sub

    Private Sub OnNewPSHost_Opened() 'this gets called in  updatesvcmonitorstate...is just a test..
        'If MyCmdSupport.ToIType(MySvcType).Equals(GetType(ITesterSvc.ITesterSvc)) Then
        '    Try
        '        Dim factory = New ChannelFactory(Of ITesterSvc.ITesterSvc)("BasicHttpBinding_ICacheTesterSvc")
        '        Dim client As ITesterSvc.ITesterSvc = factory.CreateChannel

        '        client.GoDoWhatISaid()
        '        MyServiceOf_T_SvcHost.SvcMonitor.Update_ServiceCalls("TEST OF CommandSupport.OnNewPSHost_Opened  Client.GoDoWhatISaid() " & DateTime.Now.ToLongTimeString, _
        '                                     "MySvcType=<" & MySvcType.ToString & ">")
        '    Catch ex As Exception
        '        MyServiceOf_T_SvcHost.SvcMonitor.Update_ServiceCalls("TEST OF CommandSupport.OnNewPSHost_Opened reports " & DateTime.Now.ToLongTimeString, _
        '                                     "MySvcType=<" & MySvcType.ToString & ">)" & ex.Message)
        '    End Try
        'End If
        'If MyCmdSupport.ToIType(MySvcType).Equals(GetType(ICustomerDBSvc.ICustomerDBSvc)) Then
        '    Try
        '        Dim factory = New ChannelFactory(Of ICustomerDBSvc.ICustomerDBSvc)("wsHttpBinding_ICustomerDBSvc")
        '        Dim client As ICustomerDBSvc.ICustomerDBSvc = factory.CreateChannel

        '        client.SetCache("Aldofo")
        '        MyServiceOf_T_SvcHost.SvcMonitor.Update_ServiceCalls("TEST OF CommandSupport.OnNewPSHost_Opened  CustomerDBSvcClient.SetCache('Aldofo') " & DateTime.Now.ToLongTimeString, _
        '                                     "MySvcType=<" & MySvcType.ToString & ">")
        '    Catch ex As Exception
        '        MyServiceOf_T_SvcHost.SvcMonitor.Update_ServiceCalls("TEST OF CommandSupport.OnNewPSHost_Opened reports " & DateTime.Now.ToLongTimeString, _
        '                                     "MySvcType=<" & MySvcType.ToString & ">)" & ex.Message)
        '    End Try
        'End If
    End Sub

    Private Sub ServiceFaultedHandler(ByVal sender As Object, ByVal e As EventArgs) Handles MyServiceOf_T_SvcHost.Faulted
        RemoveHandler MyServiceOf_T_SvcHost.SvcMonitor.PropertyChanged, AddressOf ServiceMonitorPropertyChanged
        With CType(sender, ServiceHost)
            RemoveHandler .Opened, AddressOf UpdateSvcMonitorState
            RemoveHandler .Faulted, AddressOf ServiceFaultedHandler
            RemoveHandler .Closing, AddressOf UpdateSvcMonitorState
            RemoveHandler .Closed, AddressOf UpdateSvcMonitorState
            Using EvLog As New EventLog()
                EvLog.Source = "WPF Svc Window"
                EvLog.Log = "Application"
                EvLog.WriteEntry("ServiceFaultedHandler: " & .Description.Name & "  " & .State.ToString, EventLogEntryType.Warning)
            End Using
            MyServiceOf_T_SvcHost.SvcMonitor.Update_ServiceState(.Description.Name & .GetHashCode, .State)
            MyServiceOf_T_SvcHost.SvcMonitor.Update_ServiceCalls("<<<<<Faulted >>>>>>" & .Description.Name & .GetHashCode, .State.ToString)
        End With
        sender = Nothing

    End Sub

    Private Sub WindowClosing() Handles Me.Unloaded
        ' TryCloseCmdSvc()
        'If MyCmdSupport IsNot Nothing Then
        '    'have to gracefully close all of the svchosts in this dxnry...else they stay in the Http.sys as registered URL
        '    For Each kvp In MyCmdSupport.SvcHostDxnry
        '        Dim csh = kvp.Value
        '        If csh.State <> CommunicationState.Closing AndAlso csh.State <> CommunicationState.Closed Then
        '            If csh.State = CommunicationState.Faulted Then
        '                csh = Nothing
        '            Else
        '                csh.Close()
        '            End If
        '        Else
        '            csh = Nothing
        '        End If
        '    Next

        '    MyCmdSupport = Nothing
        'End If

        If MyServiceOf_T_SvcHost IsNot Nothing Then
            If MyServiceOf_T_SvcHost.SvcMonitor IsNot Nothing Then
                RemoveHandler MyServiceOf_T_SvcHost.SvcMonitor.PropertyChanged, AddressOf ServiceMonitorPropertyChanged
                MyServiceOf_T_SvcHost.SvcMonitor = Nothing
            End If
            
            With MyServiceOf_T_SvcHost
                Using EvLog As New EventLog()
                    EvLog.Source = "WPF Svc Window"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("WindowClosing: " & .Description.Name & "  " & .State.ToString, EventLogEntryType.Information)
                End Using
                RemoveHandler .Opened, AddressOf UpdateSvcMonitorState
                RemoveHandler .Faulted, AddressOf ServiceFaultedHandler
                RemoveHandler .Closing, AddressOf UpdateSvcMonitorState
                RemoveHandler .Closed, AddressOf UpdateSvcMonitorState
            End With
           
            If MyServiceOf_T_SvcHost.State <> CommunicationState.Closing AndAlso MyServiceOf_T_SvcHost.State <> CommunicationState.Closed Then
                If MyServiceOf_T_SvcHost.State = CommunicationState.Faulted Then
                    MyServiceOf_T_SvcHost = Nothing
                Else
                    MyServiceOf_T_SvcHost.Close()
                End If
          
            End If
        End If

    End Sub

#Region "ServiceMonitorStuff"
    Private Sub UpdateSvcMonitorState(ByVal sender As Object, ByVal e As System.EventArgs)

        With CType(sender, ServiceHost)
            MyServiceOf_T_SvcHost.SvcMonitor.Update_ServiceState(.Description.Name & .GetHashCode, .State)
        End With

    End Sub

    Private Sub ServiceMonitorPropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) 'Handles WCFSvcLibr1.ServiceMonitor.PropertyChanged
        If e.PropertyName = "ServiceCalls" Then
            Me.OperationsMonitorListBox.ItemsSource = Nothing
            Me.OperationsMonitorListBox.ItemsSource = MyServiceOf_T_SvcHost.SvcMonitor.ServiceCalls
            Me.OperationsMonitorListBox.Items.MoveCurrentToLast()
            Me.OperationsMonitorListBox.SelectedItem = Me.OperationsMonitorListBox.Items.CurrentItem
            Me.OperationsMonitorListBox.ScrollIntoView(Me.OperationsMonitorListBox.SelectedItem)
        ElseIf e.PropertyName = "ServiceStateEvents" Then
            Me.ServiceListbox.ItemsSource = Nothing
            Me.ServiceListbox.ItemsSource = MyServiceOf_T_SvcHost.SvcMonitor.ServiceInfoColxn
            Me.ServiceListbox.Items.MoveCurrentToLast()
            Me.ServiceListbox.SelectedItem = Me.ServiceListbox.Items.CurrentItem
            Me.ServiceListbox.ScrollIntoView(Me.ServiceListbox.SelectedItem)

            Me.StateListbox.ItemsSource = Nothing
            Me.StateListbox.ItemsSource = From si In MyServiceOf_T_SvcHost.SvcMonitor.ServiceInfoColxn _
                                                    From sc In si.StateColxn _
                                                    Select sc
            Me.StateListbox.Items.MoveCurrentToLast()
            Me.StateListbox.SelectedItem = Me.StateListbox.Items.CurrentItem
            Me.StateListbox.ScrollIntoView(Me.StateListbox.SelectedItem)
        End If
    End Sub
#End Region '"ServiceMonitorStuff"

#Region "ButtonClickHandlers"
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        If MyServiceOf_T_SvcHost IsNot Nothing Then
            If MyServiceOf_T_SvcHost.State <> CommunicationState.Opened _
            AndAlso MyServiceOf_T_SvcHost.State <> CommunicationState.Opening Then
                SetUprSvcHost()
            End If
        Else
            SetUprSvcHost()
        End If
    End Sub

    ''' <summary>
    ''' This shows a CustomServiceHost View...
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MyClickHandler(ByVal sender As Object, ByVal e As EventArgs)
        If MyServiceOf_T_SvcHost IsNot Nothing Then
            Dim btn = CType(sender, Button)
            Dim myHostWindow = New Window
            myHostWindow.ResizeMode = Windows.ResizeMode.CanResizeWithGrip
            myHostWindow.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
            myHostWindow.Width = 300
            myHostWindow.Height = 200
            'Dim vb As New Viewbox
            'vb.MinHeight = 100
            ' vb.MinWidth = 100

            myHostWindow.Title = "SvcHost " & MyServiceOf_T_SvcHost.Description.Endpoints.First.ListenUri.OriginalString & " " & MyServiceOf_T_SvcHost.State.ToString

            Dim view As New CustomSvcHostView
            Dim model As New CustomSvcHostModel With {.MyCustomSvcHost = MyServiceOf_T_SvcHost} 'btn.Tag}
            'view.MyModel = model
            view.DataContext = model

            'vb.Child = view
            myHostWindow.Content = view
            myHostWindow.WindowStyle = WindowStyle.ToolWindow
            myHostWindow.Show()
        Else
            MessageBox.Show("There is no service host active in this window...there is nothing to show...")
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If MyServiceOf_T_SvcHost IsNot Nothing Then

            If MyServiceOf_T_SvcHost.State <> CommunicationState.Closing AndAlso MyServiceOf_T_SvcHost.State <> CommunicationState.Closed Then
                If MyServiceOf_T_SvcHost.State = CommunicationState.Faulted Then
                    With MyServiceOf_T_SvcHost
                        .SvcMonitor.Update_ServiceCalls("<<Stop Service Click>>" & .Description.Name & .GetHashCode, _
                                                                       .State.ToString)
                    End With
                    MyServiceOf_T_SvcHost = Nothing
                Else
                    With MyServiceOf_T_SvcHost
                        .SvcMonitor.Update_ServiceCalls("<<Stop Service Click>>" & .Description.Name & .GetHashCode, _
                                                                       .State.ToString)
                    End With
                    Try
                        MyServiceOf_T_SvcHost.Close()
                    Catch ex As Exception
                        MessageBox.Show("StopService Click reports .." & ex.Message)
                    End Try
                End If
            End If
        End If
    End Sub

    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If MyServiceOf_T_SvcHost IsNot Nothing Then
            Me.OperationsMonitorListBox.ItemsSource = Nothing
            Me.OperationsMonitorListBox.ItemsSource = MyServiceOf_T_SvcHost.SvcMonitor.ServiceCalls

            Me.ServiceListbox.ItemsSource = Nothing
            Me.ServiceListbox.ItemsSource = MyServiceOf_T_SvcHost.SvcMonitor.ServiceInfoColxn
            Me.ServiceListbox.Items.MoveCurrentToLast()
            Me.ServiceListbox.SelectedItem = Me.ServiceListbox.Items.CurrentItem
            Me.ServiceListbox.ScrollIntoView(Me.ServiceListbox.SelectedItem)

            Me.StateListbox.ItemsSource = Nothing
            Me.StateListbox.ItemsSource = From si In MyServiceOf_T_SvcHost.SvcMonitor.ServiceInfoColxn _
                                                    From sc In si.StateColxn _
                                                    Select sc
            Me.StateListbox.Items.MoveCurrentToLast()
            Me.StateListbox.SelectedItem = Me.StateListbox.Items.CurrentItem
            Me.StateListbox.ScrollIntoView(Me.StateListbox.SelectedItem)
        Else
            MessageBox.Show("The ServiceHost is nothing...no service monitor is available for this window")
        End If
    End Sub
#End Region '"ButtonClickHandlers"

#End Region '"MainWindow Stuff"

#Region "Command Svc Stuff"
    Private WithEvents MyCommandSvcHost As CustomSvcHost
    'Private Sub SetUpCommandSvcHost()
    '    Try
    '        MyCommandSvcHost = New CustomSvcHost(GetType(CommandSvc), New Uri(MyBaseAddress & "CommandSvc")) 'sets BaseAddress

    '        Dim endpt = MyCommandSvcHost.AddServiceEndpoint(LocalToIType(GetType(CommandSvc)), _
    '                                                       New WSHttpBinding("WSHttpBinding_ICommandSvc"), _
    '                                                         MyBaseAddress & "CommandSvc")

    '        MyCommandSvcHost.CmdSvcMonitor = New CommandSvcMonitor
    '        MyCommandSvcHost.SvcMonitor = New CmdSvcClassLibrary.ServiceMonitor


    '        AddHandler MyCommandSvcHost.Opened, AddressOf UpdateCommandSvcMonitor
    '        AddHandler MyCommandSvcHost.Faulted, AddressOf UpdateCommandSvcMonitor
    '        AddHandler MyCommandSvcHost.Closing, AddressOf UpdateCommandSvcMonitor
    '        AddHandler MyCommandSvcHost.Closed, AddressOf UpdateCommandSvcMonitor

    '        MyCommandSvcHost.Open()

    '        AddHandler MyCommandSvcHost.CmdSvcMonitor.PropertyChanged, AddressOf CmdSvcMonitorPropertyChanged

    '        Me.CmdWindow.Title.Text = MyCommandSvcHost.BaseAddresses.First.OriginalString
    '        Me.CmdWindow.ColorSchemeBorder = Me.ColorSchemeBorder
    '        Me.CmdWindow.ColorSchemeSplitters = Me.ColorSchemeSplitters
    '    Catch ex As Exception
    '        MessageBox.Show("WPFSvcContainerWindow.SetUpCommandSvcHost reports " & ex.Message)
    '    End Try
    'End Sub
    'Public Function LocalToIType(ByVal _ClassType As Type) As Type
    '    Dim rslt As Type = Nothing
    '    Dim classtypename As String = ""
    '    Try
    '        Dim q = From itype In _ClassType.GetInterfaces() _
    '            Select itype
    '        rslt = q.Single
    '    Catch ex As Exception
    '        MessageBox.Show("LocalToIType reports The ClassType<" & _ClassType.Name & "> provided has more than one interface defined..." & ex.Message)
    '    End Try
    '    Return rslt
    'End Function

    'Private Sub CommandHandler(ByVal _cmdPkg As CommandPackage)
    '    With MySvcType
    '        If _cmdPkg.CmdVb = CmdVerb.ExposeEndPoint Then
    '            MyCmdSupport.StartCmd(_cmdPkg) 'this is the queueURI of the TargetPostingSvc....NEED TO LOOK AT CALLER HERE>>
    '        ElseIf _cmdPkg.CmdVb = CmdVerb.RetractEndPoint Then
    '            MyCmdSupport.StopCmd(_cmdPkg.EndPtSuffix)
    '        ElseIf _cmdPkg.CmdVb = CmdVerb.ExposeWithDataContext Then
    '            MyCmdSupport.StartCmd(_cmdPkg) 'just for now...
    '        End If
    '        Me.CmdWindow.ActivePostingSvcs_Listbox.ItemsSource = Nothing
    '        Me.CmdWindow.ActivePostingSvcs_Listbox.ItemsSource = MyCmdSupport.ActiveServicesList
    '        Me.CmdWindow.DormantPostingSvcs_Listbox.ItemsSource = Nothing
    '        Me.CmdWindow.DormantPostingSvcs_Listbox.ItemsSource = MyCmdSupport.DormantServicesList
    '    End With
    'End Sub

    'Private Sub CmdSvcMonitorPropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
    '    Me.CmdWindow.CommandsListBox.ItemsSource = Nothing
    '    Me.CmdWindow.CommandsListBox.ItemsSource = MyCommandSvcHost.CmdSvcMonitor.CommandSvcCalls
    '    Me.CmdWindow.CommandsListBox.Items.MoveCurrentToLast()
    '    Me.CmdWindow.CommandsListBox.SelectedItem = Me.CmdWindow.CommandsListBox.Items.CurrentItem
    '    Me.CmdWindow.CommandsListBox.ScrollIntoView(Me.CmdWindow.CommandsListBox.SelectedItem)

    '    Me.CommandHandler(MyCommandSvcHost.CmdSvcMonitor.CommandIssued)
    'End Sub

    'Private Sub UpdateCommandSvcMonitor(ByVal sender As Object, ByVal e As System.EventArgs)
    '    If MyCommandSvcHost.State = CommunicationState.Faulted Then
    '        'CommandSvcMonitor.Update_CommandSvcCalls("Service Faulted", "MyCommandSvcHost")
    '        Me.CmdWindow.Background = New SolidColorBrush(Colors.Red)
    '        Me.CmdWindow.CmdSvc_State_Textblock.Text = " Faulted " & DateTime.Now.ToLongTimeString
    '        'Me.CmdWindow.Show()
    '        'SetUpCommandSvcHost()
    '    End If
    '    If Me.MyCommandSvcHost.State = CommunicationState.Opened Then
    '        Me.CmdWindow.Background = New SolidColorBrush(Colors.White)
    '    End If
    '    Me.CmdWindow.CmdSvc_State_Textblock.Text = MyCommandSvcHost.State.ToString & " " & DateTime.Now.ToLongTimeString
    'End Sub

#Region "CommandWindow"
    ' Public WithEvents CmdWindow As New CommandWindow 'this is actually a usercontrol now..
    'Private Sub Button_CommandWindow_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    'If Me.CmdWindow.WindowState = Windows.WindowState.Minimized Then
    '    '    Me.CmdWindow.WindowState = Windows.WindowState.Normal
    '    'Else
    '    '    Me.CmdWindow.Show()
    '    'End If
    'End Sub
    'Private Sub CmdWindow_Closing(ByVal sender As Object, ByVal e As CancelEventArgs) Handles CmdWindow.Closing
    '    e.Cancel = True
    '    CType(sender, CommandWindow).WindowState = Windows.WindowState.Minimized
    'End Sub
#End Region 'CommandWindow
#End Region

    'Public Sub CloseThis()
    '    TryCloseCmdSvc()

    'End Sub

    'Private Sub TryCloseCmdSvc()
    '    If Me.MyCommandSvcHost IsNot Nothing Then
    '        If Me.MyCommandSvcHost.CmdSvcMonitor IsNot Nothing Then
    '            RemoveHandler MyCommandSvcHost.CmdSvcMonitor.PropertyChanged, AddressOf CmdSvcMonitorPropertyChanged
    '            MyCommandSvcHost.CmdSvcMonitor = Nothing
    '        End If
    '        RemoveHandler MyCommandSvcHost.Opened, AddressOf UpdateCommandSvcMonitor
    '        RemoveHandler MyCommandSvcHost.Faulted, AddressOf UpdateCommandSvcMonitor
    '        RemoveHandler MyCommandSvcHost.Closing, AddressOf UpdateCommandSvcMonitor
    '        RemoveHandler MyCommandSvcHost.Closed, AddressOf UpdateCommandSvcMonitor
    '        Try
    '            If Me.MyCommandSvcHost.State <> CommunicationState.Faulted AndAlso Me.MyCommandSvcHost.State <> CommunicationState.Closing _
    '          AndAlso Me.MyCommandSvcHost.State <> CommunicationState.Closed Then
    '                Me.MyCommandSvcHost.Close()
    '            Else
    '                If Me.MyCommandSvcHost.State = CommunicationState.Faulted Then
    '                    Me.MyCommandSvcHost = Nothing
    '                End If
    '            End If
    '        Catch ex As Exception
    '            MessageBox.Show("WPFSvcContainerWindow reports " & ex.Message)
    '        End Try
    '    End If
    'End Sub

#Region "Border and SplitterBrushProperties" ' Has INotify guy in here too...
    Private _ColorSchemeBorder As Brush = New SolidColorBrush(Colors.SaddleBrown)
    Public Property ColorSchemeBorder() As Brush
        Get
            Return _ColorSchemeBorder
        End Get
        Set(ByVal value As Brush)
            _ColorSchemeBorder = value
            My_OnPropertyChanged("ColorSchemeBorder")
        End Set
    End Property

    Private _ColorSchemeSplitters As Brush = New SolidColorBrush(Colors.SaddleBrown)
    Public Property ColorSchemeSplitters() As Brush
        Get
            Return _ColorSchemeSplitters
        End Get
        Set(ByVal value As Brush)
            _ColorSchemeSplitters = value
            My_OnPropertyChanged("ColorSchemeSplitters")
        End Set
    End Property

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
        'If propname = "Configuration" Then
        '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
        'End If
    End Sub
#End Region '"Border and SplitterBrushProperties" Has INotify guy in here too...
End Class

