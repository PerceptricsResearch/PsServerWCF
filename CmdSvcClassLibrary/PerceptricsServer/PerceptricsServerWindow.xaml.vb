Imports System.Configuration
Imports System.ServiceModel.Configuration
Imports System.ServiceModel
Imports System.Collections.ObjectModel
Imports Tester
Imports System.Threading

Class PerceptricsServerWindow

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddHandler CmdInfrastructureNS.SharedEvents.OperationFailed, AddressOf OperationFailedHandler
    End Sub
    Private Sub IamLoaded() Handles Me.Loaded
        Dim ta As New Tester.Window1
        ta.Show()

        ''Dim CrossDomainHost As New ServiceHost(GetType(CrossDomainSvcNS.CrossDomainSvc))
        ''Try
        ''    CrossDomainHost.Open()
        ''    If CrossDomainHost IsNot Nothing Then
        ''        MessageBox.Show("CrossDomainSvc.State = " & CrossDomainHost.State)
        ''    End If
        ''Catch ex As Exception
        ''    MessageBox.Show("PerceptricsServer CrossDomainHost.Open reports..." & ex.Message)
        ''End Try

        'Me.MyListOfInstances = Application.AppDomainList
        'Me.Instance_IC.ItemsSource = Nothing
        'Me.Instance_IC.ItemsSource = Me.MyListOfInstances
    End Sub


    Private Sub OperationFailedHandler(ByVal sender As Object, ByVal _msg As String, ByVal e As EventArgs)
        MessageBox.Show(sender.ToString & " reports operation failed " & _msg & "   " & DateTime.Now.ToShortTimeString)
    End Sub

    'Private Sub ExamineConfig() 'this was a bunch of exploration...
    '    'Dim qx = From sg In ConfigurationManager.OpenExeConfiguration("PerceptricsServer.exe.config").SectionGroups _
    '    '         Where sg.GetType.Equals(GetType(System.ServiceModel.Configuration.ServiceModelSectionGroup)) _
    '    '        Select sg



    '    Dim q = From sg In ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).SectionGroups _
    '            Where sg.GetType.Equals(GetType(System.ServiceModel.Configuration.ServiceModelSectionGroup)) _
    '            Select sg

    '    Dim rslt = CType(q.First, System.ServiceModel.Configuration.ServiceModelSectionGroup)

    '    Dim services = From sv As System.ServiceModel.Configuration.ServiceElement In rslt.Services.Services _
    '                 Select sv

    '    'CType(Me.ServicesRegion.Child, ServicesManager).MyModel.SvcElementsFromConfig = _
    '    'New ObservableCollection(Of ServiceModel.Configuration.ServiceElement)(services.ToList)

    'End Sub
    'Private Ndx As Integer = 0
    'Private Sub NewInstance_Btn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    Dim btn = CType(sender, Button)
    '    Application.AppDomainList.Add(Application.CreateAppDomain())
    '    'Tester.Application.Main()
    '    'Dim x = Application.CreateAppDomain
    '    'x.

    '    Me.MyListOfInstances = Application.AppDomainList
    '    Me.Instance_IC.ItemsSource = Nothing
    '    Me.Instance_IC.ItemsSource = Me.MyListOfInstances
    'End Sub


    'Private _MyListOfInstances As New ObservableCollection(Of String)
    'Public Property MyListOfInstances() As ObservableCollection(Of String)
    '    Get
    '        Return _MyListOfInstances
    '    End Get
    '    Set(ByVal value As ObservableCollection(Of String))
    '        _MyListOfInstances = value
    '    End Set
    'End Property


End Class
