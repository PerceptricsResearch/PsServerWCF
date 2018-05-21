Imports CmdInfrastructureNS
Imports CmdSvcClassLibrary
Imports System.Configuration
Imports System.IO
Imports System.Text
Imports System.ServiceModel.Configuration
Imports DataContextPackageNS
Imports System.ServiceModel
Imports System.ServiceModel.Channels

Public Class WCFServiceManager
    Public Shared ServicesDxnry As Dictionary(Of String, CustomSvcHost)
    Private Shared _lock As Object
    Private Shared IdleSvcsDxnry As Dictionary(Of String, Dictionary(Of String, TimeSpan)) 'each suffix is the key to this dictionary, the second dxnry is keyed by address
    Private Shared CleanUpSvcsTimer As System.Windows.Threading.DispatcherTimer
    Shared Sub New()
        _lock = New Object
        ServicesDxnry = New Dictionary(Of String, CustomSvcHost)
        IdleSvcsDxnry = New Dictionary(Of String, Dictionary(Of String, TimeSpan))
        'AddHandler SharedEvents.SvcHostIsIdle, AddressOf SvcIsIdleHandler

        'CleanUpSvcsTimer = New System.Windows.Threading.DispatcherTimer(Windows.Threading.DispatcherPriority.Background)
        'AddHandler CleanUpSvcsTimer.Tick, AddressOf CleanSvcsTimerTickHandler
        'CleanUpSvcsTimer.Interval = New TimeSpan(1, 3, 1) 'this should be in app.config...or configurable with a webdialogue...
        'CleanUpSvcsTimer.Start()
    End Sub

#Region "IdleSvcHostTimer methods...not used now...make sure to go to SharedEvents to uncomment idleevent.."
    Public Shared Sub CleanSvcsTimerTickHandler(ByVal sender As Object, ByVal e As EventArgs)
        'get the keys as strings...can't modify the dictionary while enumerating it...other functions might be accessing...SYNCHLOCK?
        'this TotalMinutes parameter should be configurable too..
        Dim dxnryKeys = From kvp In IdleSvcsDxnry _
                             Where kvp.Value.Values.All(Function(ts) ts.TotalMinutes > 60) _
                             Select kvp.Value.Keys
        If dxnryKeys.Any Then

            Dim listofEndPtSuffixs = New List(Of String)
            For Each dxnrykeyxColxn In dxnryKeys 'this is a colxn of KeyCollection instances...
                'make sure that the LastOperationTime ON ALL THE SERVICEHOSTs for this Suffix is still outside the elapsed time boundary for doing any svchost.close stuff.
                Dim svchosts_lastOptTimeStillOutside As Boolean = False
                For Each cshkey In dxnrykeyxColxn
                    Dim svchost As CustomSvcHost = Nothing
                    If ServicesDxnry.TryGetValue(cshkey, svchost) Then
                        If DateTime.UtcNow.Subtract(svchost.LastOperationDateTime).TotalMinutes > 3 Then
                            svchosts_lastOptTimeStillOutside = True
                        Else
                            svchosts_lastOptTimeStillOutside = False
                        End If
                    End If
                Next
                'if they are all outside the window, then close the svcn host...
                If svchosts_lastOptTimeStillOutside Then
                    Dim EndPtSuffix As String = Nothing
                    For Each key In dxnrykeyxColxn
                        Dim svchost As CustomSvcHost = Nothing
                        If ServicesDxnry.TryGetValue(key, svchost) Then
                            Try
                                EndPtSuffix = svchost.EndPtSuffix.ToString
                                svchost.Close()
                                ServicesDxnry.Remove(key)
                                svchost = Nothing
                            Catch ex As Exception
                                SharedEvents.RaiseOperationFailed(dxnrykeyxColxn, "WCFServiceManager.CleanSvcsTimerTickHandler reports... " & ex.Message)
                            End Try
                        Else
                            SharedEvents.RaiseOperationFailed(dxnrykeyxColxn, "WCFServiceManager.CleanSvcsTimerTickHandler reports... can't find svc in ServicesDxnry...key= " & key)
                        End If
                    Next
                    listofEndPtSuffixs.Add(EndPtSuffix)
                End If
            Next
            'this takes the suffixes out of the IdleSvcsDxnry...PROBABLY NEED A SYNCHLOCK!!!...
            For Each endptsuffix In listofEndPtSuffixs
                Dim listofdxnryaddr As Dictionary(Of String, TimeSpan) = Nothing
                If IdleSvcsDxnry.TryGetValue(endptsuffix, listofdxnryaddr) Then
                    IdleSvcsDxnry.Remove(endptsuffix)
                    listofdxnryaddr.Clear()
                    listofdxnryaddr = Nothing
                End If
            Next
            listofEndPtSuffixs.Clear()
            listofEndPtSuffixs = Nothing
        End If
        dxnryKeys = Nothing
    End Sub

    'this guy puts timespans in the idlesvcsdxnry..CleanSvcsTimerTickHandler periodically looks at this dxnry and close the services...
    'the timespan param represents the time elapsed since the ServiceHost's last operation....is reported by the ServiceHost...
    Public Shared Sub SvcIsIdleHandler(ByVal _timespan As TimeSpan, ByVal _DxnryKey As String, ByVal _suffix As String, ByVal e As EventArgs)
        If _DxnryKey IsNot Nothing Then
            'this dxnry is orginally populated at the same time the ServicsDxnry is populated
            Dim idledxnryitem As Dictionary(Of String, TimeSpan) = Nothing
            If IdleSvcsDxnry.TryGetValue(_suffix, idledxnryitem) Then
                Dim tsobj As TimeSpan = Nothing
                If idledxnryitem.TryGetValue(_DxnryKey, tsobj) Then
                    idledxnryitem(_DxnryKey) = _timespan
                    tsobj = Nothing
                End If
            End If

        End If
    End Sub

    Public Shared Sub AddThisSvcHostToIdleSvcsDxnry(ByVal _Suffix As String, ByVal _DxnryAddress As String)
        Dim idledxnryitem As Dictionary(Of String, TimeSpan) = Nothing
        If IdleSvcsDxnry.TryGetValue(_Suffix, idledxnryitem) Then
            'suffix level exists, and so does the idledxnrtyitem..
            Dim tsobj As TimeSpan = Nothing
            If Not idledxnryitem.TryGetValue(_DxnryAddress, tsobj) Then
                idledxnryitem.Add(_DxnryAddress, New TimeSpan)
            Else

                Dim x = 2 'this is a problem
            End If
        Else
            idledxnryitem = New Dictionary(Of String, TimeSpan)
            idledxnryitem.Add(_DxnryAddress, New TimeSpan)
            IdleSvcsDxnry.Add(_Suffix, idledxnryitem)
        End If
        idledxnryitem = Nothing
    End Sub
#End Region
 

    Public Shared Function StartServicesV2(ByVal serviceStartPackages As List(Of ServiceStartPackage)) _
    As List(Of IEndPtDataCntxtSvcNS.EndPtPackage)
        '_logger.EnterMethod()
        Dim serviceHost As CmdSvcClassLibrary.CustomSvcHost
        Dim theType As Type
        Dim endPtPackage As IEndPtDataCntxtSvcNS.EndPtPackage
        Dim startedServices As List(Of IEndPtDataCntxtSvcNS.EndPtPackage)
        Dim address As String

        serviceHost = Nothing
        theType = Nothing
        startedServices = New List(Of IEndPtDataCntxtSvcNS.EndPtPackage)()

        SyncLock (_lock)
            For Each ssp As ServiceStartPackage In serviceStartPackages
                theType = FindType(ssp.Name)
                If (theType IsNot Nothing) Then
                    '_logger.Debug("Ready to start WCF service: " + ssp.Name)
                    serviceHost = BuildSvcHost(ssp.Name, ssp.EndpointSuffix)
                    If (serviceHost IsNot Nothing) Then
                        'serviceHost.CmdSvcMonitor = New CmdSvcClassLibrary.CommandSvcMonitor()
                        '// TODO: See whether we need a Clone() method here

                        serviceHost.DC_Pkg = ssp.DC_Pkg
                        If serviceHost.Description.Endpoints.Count = 0 Then
                            Dim MyBaseAddress = serviceHost.BaseAddresses.First.AbsoluteUri
                            'Dim endpt = serviceHost.AddServiceEndpoint(ToIType(theType), New BasicHttpBinding("basicHttpBinding_open"), _
                            '                                     MyBaseAddress)

                            Dim endpt = serviceHost.AddServiceEndpoint(ToIType(theType), New CustomBinding("binaryHttpSBinding"), _
                                                                 MyBaseAddress)
                            'Dim endptweb = serviceHost.AddServiceEndpoint(ToIType(theType), New WebHttpBinding("securewebhttpbinding"), _
                            '                                     MyBaseAddress & "web/")
                            'endptweb.Behaviors.Add(New Description.WebHttpBehavior With {.AutomaticFormatSelectionEnabled = True})
                        End If
                        serviceHost.LastOperationKVP = New KeyValuePair(Of String, String)(DateTime.Now.ToLongTimeString(), "CMD Start")
                        serviceHost.DataContextConnectionString = ssp.DataContextConnectionString
                        serviceHost.EndPtSuffix = ssp.EndpointSuffix
                        Try
                            serviceHost.Open()
                            If (ssp.RestartonFault) Then
                                ' serviceHost.Faulted += new EventHandler(serviceHost_Faulted_Restart);
                            Else
                                'serviceHost.Faulted += new EventHandler(serviceHost_Faulted_Close);
                                For Each uri As Uri In serviceHost.BaseAddresses
                                    endPtPackage = New IEndPtDataCntxtSvcNS.EndPtPackage()
                                    endPtPackage.Address = uri.AbsoluteUri
                                    endPtPackage.BaseAddress = uri.AbsoluteUri
                                    endPtPackage.Name = ssp.Name
                                    endPtPackage.ServerName = uri.Host
                                    endPtPackage.Suffix = ssp.EndpointSuffix
                                    endPtPackage.WCFSvcID = 0
                                    startedServices.Add(endPtPackage)
                                Next
                            End If
                            ServicesDxnry.Add(serviceHost.BaseAddresses(0).AbsoluteUri, serviceHost)
                            'AddThisSvcHostToIdleSvcsDxnry(serviceHost.EndPtSuffix, serviceHost.DxnryAddress)
                            '_logger.Debug("Started WCF service: " + ssp.Name)
                            Using EvLog As New EventLog()
                                EvLog.Source = "WCFSvcManager"
                                EvLog.Log = "Application"
                                EvLog.WriteEntry("Started WCF service: " & ssp.Name & " " & ssp.EndpointSuffix, EventLogEntryType.SuccessAudit)
                            End Using
                        Catch ex As Exception
                            Using EvLog As New EventLog()
                                EvLog.Source = "WCFSvcManager"
                                EvLog.Log = "Application"
                                EvLog.WriteEntry("StartServicesV2: " & ssp.Name & " " & ssp.EndpointSuffix & " " & ex.Message, EventLogEntryType.FailureAudit)
                            End Using
                        End Try
                    Else
                        address = BuildEndpointAddress(ssp.Name, ssp.EndpointSuffix)
                        ServicesDxnry.TryGetValue(address, serviceHost)
                        If (serviceHost IsNot Nothing) Then
                            serviceHost.DC_Pkg = ssp.DC_Pkg
                            serviceHost.LastOperationKVP = New KeyValuePair(Of String, String)(DateTime.Now.ToLongTimeString(), "CMD Start")
                            serviceHost.DataContextConnectionString = ssp.DataContextConnectionString
                            For Each uri As Uri In serviceHost.BaseAddresses
                                endPtPackage = New IEndPtDataCntxtSvcNS.EndPtPackage()
                                endPtPackage.Address = uri.AbsoluteUri
                                endPtPackage.BaseAddress = uri.AbsoluteUri
                                endPtPackage.Name = ssp.Name
                                endPtPackage.ServerName = uri.Host
                                endPtPackage.Suffix = ssp.EndpointSuffix
                                endPtPackage.WCFSvcID = 0
                                startedServices.Add(endPtPackage)
                            Next
                        End If
                    End If
                Else
                    Using EvLog As New EventLog()
                        EvLog.Source = "WCFSvcManager"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("WCFSvcManager.Could not create service : " & ssp.Name & ssp.EndpointSuffix, EventLogEntryType.Warning)
                    End Using
                    Throw New ApplicationException("Could not create service : " + ssp.Name & ssp.EndpointSuffix)
                End If
            Next
        End SyncLock
        '_logger.ExitMethod();
        Return startedServices
    End Function

    Public Shared Function FindType(ByVal name As String) As Type

        Dim theType As Type = Nothing
        theType = Type.GetType(name)

        If (theType Is Nothing) Then

            Dim assembly As Reflection.Assembly = Nothing
            Dim files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory)
            For Each File In files

                If ((File.EndsWith(".dll") Or File.EndsWith(".DLL"))) Then

                    Try

                        assembly = Reflection.Assembly.LoadFrom(File)
                        theType = assembly.GetType(name)

                        If (theType IsNot Nothing) Then
                            'Using EvLog As New EventLog()
                            '    EvLog.Source = "PerceptricsServer WCFServiceManager FindType "
                            '    EvLog.Log = "Application"
                            '    EvLog.WriteEntry("ReturnedAssembyTypeName = " & theType.Name, EventLogEntryType.Information)
                            'End Using
                            Return theType
                            'break()
                        End If

                    Catch ex As Exception
                        SharedEvents.RaiseOperationFailed(ex.ToString, "  WCFServiceManager.FindType")
                    End Try
                End If
            Next
            files = Nothing
        End If
        'Using EvLog As New EventLog()
        '    EvLog.Source = "PerceptricsServer WCFServiceManager FindType "
        '    EvLog.Log = "Application"
        '    'If theType IsNot Nothing Then
        '    '    EvLog.WriteEntry("Type.Name = " & theType.Name & " namestring is " & name, EventLogEntryType.Information)
        '    'Else
        '    '    EvLog.WriteEntry("Type.Name is nothing for string " & name, EventLogEntryType.Information)
        '    'End If
        'End Using
        Return theType

    End Function

    Private Shared Function BuildSvcHost(ByVal serviceName As String, ByVal endpointSuffix As String) As CmdSvcClassLibrary.CustomSvcHost
        '_logger.EnterMethod();

        Dim theType As Type
        Dim Configuration As Configuration
        Dim address As String
        Dim serviceHost As CmdSvcClassLibrary.CustomSvcHost
        Dim smsg As ServiceModelSectionGroup

        Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        smsg = New ServiceModelSectionGroup()
        smsg = CType(Configuration.GetSectionGroup("system.serviceModel"), ServiceModelSectionGroup)
        serviceHost = Nothing
        theType = Nothing

        '// TODO: Add other URI's for other binding types...only basicHttpBinding works this way
        For Each se As ServiceElement In smsg.Services.Services
            If (se.Name = serviceName) Then
                address = BuildEndpointAddress(serviceName, endpointSuffix)
                If (Not (ServicesDxnry.ContainsKey(address))) Then
                    theType = FindType(serviceName)
                    '// TODO: This should be a collection...need to overload the constructor again
                    serviceHost = New CmdSvcClassLibrary.CustomSvcHost(theType, New Uri(address, UriKind.Absolute))
                    serviceHost.DxnryAddress = address
                    ' _logger.Debug("Created CustomSvcHost with baseaddress: " + address);
                    'Using EvLog As New EventLog()
                    '    EvLog.Source = "PerceptricsServer"
                    '    EvLog.Log = "Application"
                    '    EvLog.WriteEntry("Created CustomSvcHost with baseaddress: " & address, EventLogEntryType.SuccessAudit)
                    'End Using
                Else
                    ' _logger.Debug("Already exists with baseaddress: " + address);
                End If
                'break;
            End If
        Next
        smsg = Nothing
        theType = Nothing
        '_logger.ExitMethod();
        Return serviceHost
    End Function

    Private Shared Function BuildEndpointAddress(ByVal serviceName As String, ByVal endpointSuffix As String) As String
        '_logger.EnterMethod();
        Dim configuration As Configuration
        Dim sb As StringBuilder
        Dim addressPrefix As String

        configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        addressPrefix = configuration.AppSettings.Settings("httpBaseAddress").Value
        sb = New StringBuilder()
        sb.Append(addressPrefix)
        sb.Append("/")
        sb.Append(serviceName)
        If (endpointSuffix IsNot Nothing) Then
            sb.Append("/")
            sb.Append(endpointSuffix)
        End If
        '_logger.ExitMethod();
        Return sb.ToString()
    End Function


    Public Shared Function StartPostingService(ByVal _ssp As ServiceStartPackage, ByVal _QueueCnxnString As String) As String
        Dim rslt As String = "NotSet"
        If Not IsNothing(_ssp) AndAlso Not IsNothing(_QueueCnxnString) Then
            If Not ServicesDxnry.ContainsKey(_ssp.EndpointSuffix) Then
                Dim theType As Type = FindType(_ssp.Name)
                If theType IsNot Nothing Then
                    Dim newPSHost = New CustomSvcHost(theType)
                    newPSHost.SvcMonitor = New ServiceMonitor

                    newPSHost.Description.Name = _ssp.EndpointSuffix
                    newPSHost.DC_Pkg = _ssp.DC_Pkg
                    newPSHost.EndPtSuffix = _ssp.EndpointSuffix

                    'this sets up the posting service to use NetMsmqBinding using the msmq for this subscriber...
                    newPSHost.DataContextConnectionString = _ssp.DataContextConnectionString
                    Dim endpt As Description.ServiceEndpoint = Nothing
                    Dim newbinding = New NetMsmqBinding("NetMsmqBinding_IPostResponsetoSurveySvc")
                    endpt = newPSHost.AddServiceEndpoint(ToIType(theType), newbinding, _QueueCnxnString)
                    endpt.Name = newPSHost.Description.Name
                    ServicesDxnry.Add(_ssp.EndpointSuffix, newPSHost)
                    Try
                        newPSHost.Open()
                        rslt = "Posting Service Start Success..." & _ssp.EndpointSuffix
                        Using EvLog As New EventLog()
                            EvLog.Source = "WCFServiceManager"
                            EvLog.Log = "Application"
                            EvLog.WriteEntry(rslt, EventLogEntryType.SuccessAudit)
                        End Using
                    Catch ex As Exception
                        rslt = "WCFServiceManager.StartPostingService.ServiceHost.Open Failed.... " & _ssp.EndpointSuffix
                        Using EvLog As New EventLog()
                            EvLog.Source = "WCFServiceManager"
                            EvLog.Log = "Application"
                            EvLog.WriteEntry(rslt & " " & ex.Message, EventLogEntryType.Error)
                        End Using
                        'SharedEvents.RaiseOperationFailed("WCFServiceManager", ".StartPostingService.ServiceHost.Open <" & _ssp.EndpointSuffix & ">  " & ex.Message)
                    End Try
                Else
                    Using EvLog As New EventLog()
                        EvLog.Source = "WCFServiceManager"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("StartPostingService could not create PostingService : theType is nothing... " & _ssp.Name, EventLogEntryType.Error)
                    End Using
                    Throw New ApplicationException("Could not create PostingService : " & _ssp.Name)
                End If
            Else
                rslt = "PostingServiceAlready started " & _ssp.EndpointSuffix
            End If
        Else
            Using EvLog As New EventLog()
                EvLog.Source = "WCFServiceManager"
                EvLog.Log = "Application"
                EvLog.WriteEntry("StartPostingService ssp or queuecnxnstring Is Nothing...", EventLogEntryType.Warning)
            End Using
        End If

        Return rslt
    End Function

    Private Shared Function ToIType(ByVal _ClassType As Type) As Type
        Dim rslt As Type = Nothing
        Dim classtypename As String = ""
        Try
            Dim q = From itype In _ClassType.GetInterfaces() _
                Select itype
            rslt = q.Single
            'MySvcType = _ClassType
            'TName = MySvcType.Name
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "WCFSvcManager"
                EvLog.Log = "Application"
                EvLog.WriteEntry("WCFSvcManager.ToIType : " & _ClassType.Name & ex.Message, EventLogEntryType.Warning)
            End Using
            '_logger.Debug("The ClassType<" & _ClassType.Name & "> provided has more than one interface defined..." & ex.Message)
            'MessageBox.Show("The ClassType<" & _ClassType.Name & "> provided has more than one interface defined..." & ex.Message)
        End Try
        Return rslt
    End Function

    'Public Function GetServiceNames() As List(Of String)

    '    Dim list = New List(Of String)()
    '    Dim enumerator As IEnumerator = serviceModelSectionGroup.Services.Services.GetEnumerator()
    '    Dim serviceElement As ServiceElement = Nothing
    '    While (enumerator.MoveNext())

    '        serviceElement = enumerator.Current
    '        list.Add(serviceElement.Name)

    '    End While
    '    Return list

    'End Function

    Public Shared Sub StopTheseServices(ByVal _ListofEndpts As List(Of IEndPtDataCntxtSvcNS.EndPtPackage))
        For Each endptpkg In _ListofEndpts
            Dim dxnrykey As String = ""
            dxnrykey = BuildEndpointAddress(endptpkg.Name, endptpkg.Suffix)
            Dim svchost As CustomSvcHost = Nothing

            If ServicesDxnry.TryGetValue(dxnrykey, svchost) Then
                Try
                    If ServicesDxnry.Remove(dxnrykey) Then
                        Using EvLog As New EventLog()
                            EvLog.Source = "WCFSvcManager"
                            EvLog.Log = "Application"
                            EvLog.WriteEntry("Stopping WCF service: " & dxnrykey, EventLogEntryType.SuccessAudit)
                        End Using
                        svchost.Close()
                        ' svchost.CmdSvcMonitor = Nothing
                        svchost.DC_Pkg = Nothing
                        svchost = Nothing
                        'this takes the logged out servicehosts out of the IdleSvcsDxnry...
                        'Dim listofdxnryaddr As Dictionary(Of String, TimeSpan) = Nothing
                        'If IdleSvcsDxnry.TryGetValue(endptpkg.Suffix, listofdxnryaddr) Then
                        '    listofdxnryaddr.Clear()
                        '    IdleSvcsDxnry.Remove(endptpkg.Suffix)
                        '    listofdxnryaddr = Nothing
                        'End If
                        'Else
                        '    Dim x = 2
                    Else
                        Using EvLog As New EventLog()
                            EvLog.Source = "WCFSvcManager"
                            EvLog.Log = "Application"
                            EvLog.WriteEntry("Stop WCF service: ServicesDxnry.Remove is false. Key= " & dxnrykey, EventLogEntryType.Warning)
                        End Using
                    End If

                Catch ex As Exception
                    Using EvLog As New EventLog()
                        EvLog.Source = "WCFSvcManager"
                        EvLog.Log = "Application"
                        EvLog.WriteEntry("Stop WCF service: Reports Exception. Key= " & dxnrykey & " " & ex.Message, EventLogEntryType.Error)
                    End Using
                    'SharedEvents.RaiseOperationFailed(endptpkg.Suffix, "WCFServiceManager.StopTheseServices reports... " & ex.Message)
                End Try
            Else
                Using EvLog As New EventLog()
                    EvLog.Source = "WCFSvcManager"
                    EvLog.Log = "Application"
                    EvLog.WriteEntry("Stop WCF service: ServicesDxnry.TryGetValue is false. Key= " & dxnrykey, EventLogEntryType.Warning)
                End Using
                'SharedEvents.RaiseOperationFailed(_ListofEndpts, "WCFServiceManager.StopTheseServices reports... can't find svc in ServicesDxnry...key= " & dxnrykey)
            End If
            dxnrykey = ""
        Next
    End Sub


End Class
