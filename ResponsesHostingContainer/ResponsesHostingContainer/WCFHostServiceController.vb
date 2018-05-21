Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel
Imports System.Configuration
Imports System.Runtime.Serialization
Imports System.ServiceModel.Configuration
Imports System.Reflection
Imports System.Xml
Imports System.ServiceModel.Channels
Imports System.IO
Imports System.ServiceModel.Web

''' <summary>
''' IWCFHostServiceController.  Interface for the WCF management service that is being exposed 
''' from the windows service.
''' </summary>
''' <remarks></remarks>
<ServiceContract()> _
Public Interface IWCFHostServiceController
    

    <OperationContract()> _
    <FaultContract(GetType(WCFHostServiceException))> _
 Sub OpenService(ByVal serviceName As String)

    <OperationContract()> _
    <FaultContract(GetType(WCFHostServiceException))> _
 Sub CloseService(ByVal serviceName As String)

    'I'm putting the client access policy stuff in here in case I want to look at this from SL 3...not needed otherwise
    <OperationContract()> _
    <WebGet(UriTemplate:="/clientaccesspolicy.xml")> _
     Function GetClientAccessPolicy() As Message
End Interface
Public Class WCFHostServiceController
    Implements IWCFHostServiceController

    Private m_hostConfigurationManager As HostConfigurationManager = Nothing
    Private ReadOnly Property MyHostConfigurationManager() As HostConfigurationManager
        Get
            If (m_hostConfigurationManager Is Nothing) Then
                m_hostConfigurationManager = New HostConfigurationManager()
            End If
            Return m_hostConfigurationManager
        End Get
    End Property

#Region "Client Access Policy stuff"
    Public Function GetClientAccessPolicy() As System.ServiceModel.Channels.Message Implements IWCFHostServiceController.GetClientAccessPolicy

        Dim capstring = <?xml version="1.0" encoding="utf-8"?>
                        <access-policy>
                            <cross-domain-access>
                                <policy>
                                    <allow-from http-request-headers="*">
                                        <domain uri="*"/>
                                    </allow-from>
                                    <grant-to>
                                        <resource path="/*" include-subpaths="true"/>
                                    </grant-to>
                                </policy>
                            </cross-domain-access>
                        </access-policy>
        Dim reader = New StringReader(capstring.ToString)

        Dim myxmlReader = XmlReader.Create(reader)
        Return Message.CreateMessage(MessageVersion.None, "", myxmlReader)
    End Function
#End Region

    ''' <summary>
    ''' StartServices.  Starts all of the WCF Services that are defined in the configuration.
    ''' </summary>
    Public Sub StartServices()
        Dim serviceNames = MyHostConfigurationManager.GetServiceNames()
        If (serviceNames IsNot Nothing AndAlso serviceNames.Count > 0) Then

            CreateServices(serviceNames)

        End If
    End Sub

    Public Sub OpenService(ByVal serviceName As String) Implements IWCFHostServiceController.OpenService
        Try
            Dim serviceHost = HostWindowsService.ServiceHosts(serviceName)
            If (serviceHost IsNot Nothing) Then
                If serviceHost.State <> CommunicationState.Opening AndAlso serviceHost.State <> CommunicationState.Opened Then
                    Dim theType = MyHostConfigurationManager.FindType(serviceName)
                    Dim serviceHost2 = New ServiceHost(theType)
                    serviceHost2.Open()
                    AddHandler serviceHost2.Faulted, AddressOf serviceHost_Faulted
                    'serviceHost2.Faulted += new EventHandler(serviceHost_Faulted);
                    serviceHost = Nothing
                    HostWindowsService.ServiceHosts(serviceName) = serviceHost2
                End If
            Else
                Throw New WCFHostServiceException("Invalid service name")
            End If
        Catch ex As Exception
            Dim message = "Could not open service: " + serviceName + " " + ex.Message
            Throw New WCFHostServiceException(message)
        End Try
    End Sub

    Public Sub CloseService(ByVal serviceName As String) Implements IWCFHostServiceController.CloseService
        Dim serviceHost = HostWindowsService.ServiceHosts(serviceName)
        If (serviceHost IsNot Nothing) Then
            If serviceHost.State <> CommunicationState.Closing AndAlso serviceHost.State <> CommunicationState.Closed Then
                serviceHost.Close()
            End If
        Else
            Throw New WCFHostServiceException("Invalid service name: " + serviceName)
        End If
    End Sub

    Private Function CreateService(ByVal serviceName As String) As ServiceHost
        Dim theType = MyHostConfigurationManager.FindType(serviceName)
        Dim serviceHost As ServiceHost = Nothing
        If (theType IsNot Nothing) Then

            serviceHost = New ServiceHost(theType)
            serviceHost.Open()
            AddHandler serviceHost.Faulted, AddressOf serviceHost_Faulted
            'serviceHost.Faulted += new EventHandler(serviceHost_Faulted);

        Else

            Throw New ApplicationException("Could not create service: " + serviceName + ", the class could not be found.")

        End If
        Return serviceHost
    End Function

    Private Sub serviceHost_Faulted(ByVal sender As Object, ByVal e As EventArgs)
        Dim serviceHost As ServiceHost = CType(sender, ServiceHost)
        Dim serviceName = serviceHost.Description.Name
        OpenService(serviceName)
    End Sub

    Private Sub CreateServices(ByVal serviceNames As List(Of String))
        Dim enumerator = serviceNames.GetEnumerator()
        Dim serviceHost As ServiceHost = Nothing
        While (enumerator.MoveNext())
            Try
                serviceHost = CreateService(enumerator.Current)
                HostWindowsService.ServiceHosts.Add(enumerator.Current, serviceHost)

            Catch ex As Exception
                ' // TODO: Add logging here
                Using EvLog As New EventLog()
                    EvLog.Source = "HMResponses WCFHostServiceController CreateServices "
                    EvLog.Log = "Application"
                    EvLog.WriteEntry(ex.Message + "\n" + ex.StackTrace, EventLogEntryType.Error)
                End Using
            End Try
        End While
    End Sub
End Class

Public Class HostConfigurationManager
    Private configuration As System.Configuration.Configuration
    Private serviceModelSectionGroup As ServiceModelSectionGroup

    Public Sub New()
        Me.configuration = HostWindowsService.Configuration
        Me.serviceModelSectionGroup = configuration.GetSectionGroup("system.serviceModel")
    End Sub
    ''' <summary>
    ''' FindType. Finds a Type by its name as a string in the .dll assembly files in the current directory.
    ''' </summary>
    Public Function FindType(ByVal name As String) As Type
       
        Dim theType As Type = Nothing
        theType = Type.GetType(name)
       
        If (theType Is Nothing) Then

            Dim assembly As Assembly = Nothing
            Dim files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory)
            For Each File In files

                If ((File.EndsWith(".dll") Or File.EndsWith(".DLL"))) Then

                    Try

                        assembly = assembly.LoadFrom(File)
                        theType = assembly.GetType(name)
                        
                        If (theType IsNot Nothing) Then
                            Using EvLog As New EventLog()
                                EvLog.Source = "HMResponses WCFHostServiceController FindType "
                                EvLog.Log = "Application"
                                EvLog.WriteEntry("ReturnedAssembyTypeName = " & theType.Name, EventLogEntryType.Information)
                            End Using
                            Return theType
                            'break()
                        End If

                    Catch ex As Exception
                        'do something with the exception
                    End Try
                End If
            Next
        End If
        Using EvLog As New EventLog()
            EvLog.Source = "HMResponses WCFHostServiceController FindType "
            EvLog.Log = "Application"
            If theType IsNot Nothing Then
                EvLog.WriteEntry("Type.Name = " & theType.Name & " namestring is " & name, EventLogEntryType.Information)
            Else
                EvLog.WriteEntry("Type.Name is nothing for string " & name, EventLogEntryType.Information)
            End If
        End Using
        Return theType

    End Function

    Public Function GetServiceNames() As List(Of String)

        Dim list = New List(Of String)()
        Dim enumerator As IEnumerator = serviceModelSectionGroup.Services.Services.GetEnumerator()
        Dim serviceElement As ServiceElement = Nothing
        While (enumerator.MoveNext())

            serviceElement = enumerator.Current
            list.Add(serviceElement.Name)

        End While
        Return list

    End Function
End Class

<Serializable()> _
    Public Class WCFHostServiceException
    Inherits ApplicationException
    Implements ISerializable

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

End Class