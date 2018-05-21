Imports System
'Imports System.Threading
'Imports System.Reflection

Partial Public Class Application

    '' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    '' can be handled in this file.
    '<STAThread()> _
    '<LoaderOptimization(LoaderOptimization.MultiDomainHost)> _
    'Public Shared Sub Main()

    '    Dim newdomain As AppDomain = AppDomain.CreateDomain("TesterDomain " & DateTime.Now.ToShortTimeString)
    '    newdomain.DoCallBack(AddressOf myCallback)
    '    'Is there still a "default domain created here...?

    'End Sub
    'Public Shared Sub myCallback()
    '    Dim thr As New Thread(AddressOf myThreadCallback)
    '    thr.SetApartmentState(ApartmentState.STA)
    '    thr.Start()
    'End Sub
    'Private Shared Sub myThreadCallback()
    '    Dim app As New Application
    '    'this is necessary because it was not loaded thru ExecuteAssembly...is documented in MSDN...have to load this yourself.
    '    Application.ResourceAssembly = Assembly.GetAssembly(GetType(Application))
    '    app.InitializeComponent()
    '    app.Run()
    'End Sub

    'Public Sub InitializeComponent()
    '    Me.StartupUri = New System.Uri("Window1.xaml", System.UriKind.Relative)
    '    Me.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose
    'End Sub


    
End Class
