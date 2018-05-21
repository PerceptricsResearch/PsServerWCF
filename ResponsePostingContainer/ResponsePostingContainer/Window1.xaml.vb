Imports ResponsePostingSvcLibr1
Imports System.ServiceModel
Imports System.ServiceModel.Dispatcher
Imports System.ServiceModel.Description
Imports IPostResponsetoSurveySvcNS

Class Window1
    Private Shared ThisInstance As Window1
    Public Sub IamLoaded() Handles Me.Loaded

        ThisInstance = Me
        SvcMonitor = New ResponsePostingSvcLibr1.CommandSvcMonitor

        Try
            SetUpCommandSvcHost()
        Catch ex As Exception
            MessageBox.Show("ResponsePostingContainer.Window1.SetupCommandSvcHost reports error..." & ex.Message)
        End Try

    End Sub

    Private Shared WithEvents SvcMonitor As ResponsePostingSvcLibr1.CommandSvcMonitor
    Private Shared Sub ServiceMonitorPropertyChanged() Handles SvcMonitor.PropertyChanged
        ThisInstance.CommandsListBox.ItemsSource = Nothing
        ThisInstance.CommandsListBox.ItemsSource = ResponsePostingSvcLibr1.CommandSvcMonitor.CommandSvcCalls
        ThisInstance.CommandsListBox.Items.MoveCurrentToLast()
        ThisInstance.CommandsListBox.SelectedItem = ThisInstance.CommandsListBox.Items.CurrentItem
        ThisInstance.CommandsListBox.ScrollIntoView(ThisInstance.CommandsListBox.SelectedItem)
        Dim lofT As List(Of KeyValuePair(Of String, String)) = ThisInstance.CommandsListBox.ItemsSource
        'ThisInstance.RespondtoCommand(lofT.LastOrDefault)
        ThisInstance.CommandHandler(lofT.LastOrDefault) 'this replaces respond to command
    End Sub

    Private ActiveSvcDxnry As New Dictionary(Of String, String)
    Private DormantSvcDxnry As New Dictionary(Of String, String)

    Private Sub CommandHandler(ByVal _commandobject As KeyValuePair(Of String, String))
        Dim cmd As String = _commandobject.Key.Split("/").FirstOrDefault.ToLower
        If cmd.StartsWith("start") Then
            CommandSupport.StartCmd(_commandobject.Value) 'this is the queueURI of the TargetPostingSvc 
        ElseIf cmd.StartsWith("stop") Then
            CommandSupport.StopCmd(_commandobject.Value)
        End If
        Me.ActivePostingSvcs_Listbox.ItemsSource = Nothing
        Me.ActivePostingSvcs_Listbox.ItemsSource = CommandSupport.ActiveServicesList
        Me.DormantPostingSvcs_Listbox.ItemsSource = Nothing
        Me.DormantPostingSvcs_Listbox.ItemsSource = CommandSupport.DormantServicesList
    End Sub




    Private MyCommandSvcHost As ServiceHost
    Private Sub SetUpCommandSvcHost()

        Try
            MyCommandSvcHost = New ServiceHost(GetType(ResponsePostingSvcLibr1.CommandSvc))

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
            CommandSvcMonitor.Update_CommandSvcCalls("Service Faulted", "MyCommandSvcHost")
            SetUpCommandSvcHost()
        End If
        CommandSvcMonitor.Update_CommandSvcCalls("CommandSvcHost", MyCommandSvcHost.State.ToString)
    End Sub
End Class


Public Class CommandSupport
    Public Shared SvcHostDxnry As New Dictionary(Of String, ServiceHost)

    Public Shared SvcHost As ServiceHost
    Public Shared Sub StartCmd(ByVal _queueUri As String)
        'Dim xxxsvchost As ServiceHost = Nothing
        If SvcHostDxnry.ContainsKey(_queueUri) Then 'TryGetValue(_queueUri, xxxsvchost) Then
            If SvcHostDxnry.Item(_queueUri).State <> CommunicationState.Opening AndAlso SvcHostDxnry.Item(_queueUri).State <> CommunicationState.Opened Then
                Try
                    Dim newPSHost = New ServiceHost(GetType(PostResponsetoSurveySvc))
                    newPSHost.Description.Name = CreateSvcName(_queueUri)
                    'newPSHost.Description.Endpoints.RemoveAt(0)
                    Dim endpt = newPSHost.AddServiceEndpoint(GetType(IPostResponsetoSurveySvc), New NetMsmqBinding(NetMsmqSecurityMode.None), _queueUri)
                    endpt.Name = newPSHost.Description.Name '& "altered"

                    SvcHostDxnry.Item(_queueUri) = newPSHost
                    AddHandler newPSHost.Faulted, AddressOf SvcFaulted_Handler

                    newPSHost.Open()

                Catch ex As Exception
                    MessageBox.Show("ResponsePostingContainer.Window1.StartCmd encountered error..." & ex.Message)
                End Try
            End If
        Else
            Try
                Dim newPSHost = New ServiceHost(GetType(PostResponsetoSurveySvc))
                newPSHost.Description.Name = CreateSvcName(_queueUri)

                'newPSHost.Description.Endpoints.RemoveAt(0) ' instead just removed it from the app.config
                Dim endpt = newPSHost.AddServiceEndpoint(GetType(IPostResponsetoSurveySvc), New NetMsmqBinding(NetMsmqSecurityMode.None), _queueUri)
                endpt.Name = newPSHost.Description.Name '& "altered"

                SvcHostDxnry.Add(_queueUri, newPSHost)
                AddHandler newPSHost.Faulted, AddressOf SvcFaulted_Handler

                newPSHost.Open()

            Catch ex As Exception
                MessageBox.Show("ResponsePostingContainer.Window1.StartCmd encountered error..." & ex.Message)
            End Try
        End If
    End Sub

    Public Shared Sub StopCmd(ByVal _queueUri As String)
        'Dim xxxsvchost As ServiceHost = Nothing
        If SvcHostDxnry.ContainsKey(_queueUri) Then 'TryGetValue(_queueUri, xxxsvchost) Then
            Try

                If SvcHostDxnry.Item(_queueUri).State <> CommunicationState.Closing AndAlso SvcHostDxnry.Item(_queueUri).State <> CommunicationState.Closed Then
                    SvcHostDxnry.Item(_queueUri).Close()
                End If
            Catch ex As Exception
                MessageBox.Show("ResponsePostingContainer.Window1.StopCmd encountered error..." & ex.Message)
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
    ''' Returns the last part of a net.msmq uri delimitted by "/"
    ''' </summary>
    ''' <param name="_queueURI"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CreateSvcName(ByVal _queueURI As String) As String
        Return Strings.Split(_queueURI, "/").Last
    End Function


    Private Shared Sub SvcFaulted_Handler(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim svchost As ServiceHost = CType(sender, ServiceHost)
        ' CommandSvcMonitor.Update_CommandSvcCalls("ServiceHostFaulted " & svchost.Description.Endpoints.FirstOrDefault.Name, DateTime.Now.ToShortTimeString)
        'RemoveHandler svchost.Faulted, AddressOf SvcFaulted_Handler 'not sure if I need this....
        Dim queueURI As String = svchost.Description.Endpoints.Last.ListenUri.OriginalString
        'If SvcHostDxnry.TryGetValue(queueURI, svchost) Then
        '    Try
        '        svchost = New ServiceHost(GetType(PostResponsetoSurveySvc))
        '        svchost.Description.Name = CreateSvcName(queueURI)
        '        Dim endpt = svchost.AddServiceEndpoint(GetType(IPostResponsetoSurveySvc), New NetMsmqBinding(NetMsmqSecurityMode.None), queueURI)
        '        endpt.Name = svchost.Description.Name '& "altered"

        '        SvcHostDxnry.Item(queueURI) = svchost
        '        AddHandler svchost.Faulted, AddressOf SvcFaulted_Handler

        '        svchost.Open()

        '    Catch ex As Exception
        '        MessageBox.Show("ResponsePostingContainer.Window1.SvcFaulterHandler encountered error..." & ex.Message)
        '    End Try
        'End If
    End Sub
End Class


'this is the old respondtocommand...
'Private Sub RespondtoCommand(ByVal _commandobject As KeyValuePair(Of String, String))
'    Dim addxstr As String = Nothing
'    Dim removexstr As String = Nothing
'    Dim cmd As String = _commandobject.Key.Split("/").FirstOrDefault.ToLower
'    If cmd.StartsWith("start") Then

'        If ActiveSvcDxnry.TryGetValue(_commandobject.Value, addxstr) Then
'            addxstr += " +" & MyPostingSvcHost.State.ToString
'            ActiveSvcDxnry.Item(_commandobject.Value) = addxstr
'        Else
'            SetUpPostingSvcHost(_commandobject.Value)
'            Me.ActiveSvcDxnry.Add(_commandobject.Value, DateTime.Now.ToLongTimeString & " " & MyPostingSvcHost.State.ToString)
'        End If
'        If DormantSvcDxnry.TryGetValue(_commandobject.Value, removexstr) Then
'            Me.DormantSvcDxnry.Remove(_commandobject.Value)
'        End If
'    End If
'    If cmd.StartsWith("stop") Then
'        If DormantSvcDxnry.TryGetValue(_commandobject.Value, addxstr) Then
'            addxstr += " +" & MyPostingSvcHost.State.ToString
'            DormantSvcDxnry.Item(_commandobject.Value) = addxstr
'        Else

'            Try
'                If ClosePostingSvcHost(_commandobject.Value) Then
'                    Me.DormantSvcDxnry.Add(_commandobject.Value, DateTime.Now.ToLongTimeString & " " & MyPostingSvcHost.State.ToString)
'                Else
'                    Me.DormantSvcDxnry.Add(_commandobject.Value, DateTime.Now.ToLongTimeString & " Is Nothing")

'                End If


'            Catch ex As Exception
'                MessageBox.Show("ResponsePostingContainer.Window1.RespondtoCommand.Stop encountered error..." & ex.Message)
'            End Try

'        End If

'        If ActiveSvcDxnry.TryGetValue(_commandobject.Value, removexstr) Then
'            Me.ActiveSvcDxnry.Remove(_commandobject.Value)
'        End If
'    End If
'    If cmd.StartsWith("pause") Then

'    End If
'    If cmd.StartsWith("resume") Then

'    End If
'    Me.ActivePostingSvcs_Listbox.ItemsSource = Nothing
'    Me.ActivePostingSvcs_Listbox.ItemsSource = ActiveSvcDxnry.ToList
'    Me.DormantPostingSvcs_Listbox.ItemsSource = Nothing
'    Me.DormantPostingSvcs_Listbox.ItemsSource = DormantSvcDxnry.ToList
'End Sub


'Private MyPostingSvcHost As ServiceHost
'Private Sub SetUpPostingSvcHost(ByVal _queueuri As String)
'    Try
'        MyPostingSvcHost = New ServiceHost(GetType(PostResponsetoSurveySvc))
'        MyPostingSvcHost.AddServiceEndpoint(GetType(IPostResponsetoSurveySvc), New NetMsmqBinding(NetMsmqSecurityMode.None), _queueuri)
'        MyPostingSvcHost.Open()
'        'MyPostingSvcHost = New ServiceHost(GetType(PostResponsetoSurveySvc))
'        'MyPostingSvcHost.Open()
'    Catch ex As Exception
'        MessageBox.Show("ResponsePostingContainer.Window1.SetUpPostingSvcHost encountered error..." & ex.Message)
'    End Try

'End Sub

'Private Function ClosePostingSvcHost(ByVal _queuename As String) As Boolean
'    Dim rslt As Boolean = False
'    Try
'        MyPostingSvcHost.Close()
'        rslt = True
'    Catch ex As Exception
'        MyPostingSvcHost = Nothing
'        MessageBox.Show("ClosePostingSvcHost encountered error..." & ex.Message)
'    End Try
'    Return rslt
'End Function