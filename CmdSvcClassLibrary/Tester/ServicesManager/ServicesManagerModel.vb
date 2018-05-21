Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Configuration
Imports System.ServiceModel.Configuration
Imports System.ServiceModel

Public Class ServicesManagerModel
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub My_OnPropertyChanged(ByVal propname As String) 'Overrides OnPropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propname))
        'If propname = "Configuration" Then
        '    RaiseEvent Configuration_Changed(Me, New System.Windows.RoutedEventArgs)
        'End If
    End Sub


    Public SvcElementsFromConfig As ObservableCollection(Of ServiceModel.Configuration.ServiceElement)


    Private _GlobalSurveyMasterCnxnString As String
    ''' <summary>
    ''' This comes from the app.config ultimately...this gets populated when ServicesManager loads and executes method PopulateMyModel
    ''' This is used to provide EptSvc and MasterSurveDBSvc a datacontext to the GlobalSurveyMaster when they are hosted and started
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GlobalSurveyMasterCnxnString() As String
        Get
            Return _GlobalSurveyMasterCnxnString
        End Get
        Set(ByVal value As String)
            _GlobalSurveyMasterCnxnString = value
        End Set
    End Property

    Private _MyHostedServices As New ObservableCollection(Of WCFServiceInfo)
    Public Property MyHostedServices() As ObservableCollection(Of WCFServiceInfo)
        Get
            Return _MyHostedServices
        End Get
        Set(ByVal value As ObservableCollection(Of WCFServiceInfo))
            _MyHostedServices = value
            My_OnPropertyChanged("MyHostedServices")
        End Set
    End Property

    Private _MyServicesInventory As New ObservableCollection(Of WCFServiceInfo)
    Public Property MyServicesInventory() As ObservableCollection(Of WCFServiceInfo)
        Get
            Return _MyServicesInventory
        End Get
        Set(ByVal value As ObservableCollection(Of WCFServiceInfo))
            _MyServicesInventory = value
            My_OnPropertyChanged("MyServicesInventory")
        End Set
    End Property
End Class
