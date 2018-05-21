Imports System.Xml.Serialization

' NOTE: If you change the interface name  you must also update the reference to "IPgItemColxnSvc" in App.config.
<ServiceContract()> _
Public Interface IPgItemColxnSvc
    <OperationContract()> _
    Function IamWorking() As String


    ''' <summary>
    ''' Saves  _PageItemModel to the SurveyDataStore...returns KeyValuePaier(original poco.SDS_ID,_PageITemModelID on the db...)
    ''' </summary>
    ''' <param name="_PageItemModel"></param>
    ''' <returns>KeyValuePair(of poco_SDSID, xxxID of the row returned</returns>
    ''' <remarks></remarks>
    <OperationContract()> _
    Function SavePageItemModel(ByVal _PageItemModel As PgItemModel) As KeyValuePair(Of Integer, Integer)

    <OperationContract()> _
    Function RetrievePageItemModelList(ByVal _SurveyID As Integer) As List(Of PgItemModel)

    <OperationContract()> _
    Function SavePageContentModel(ByVal _PageContentModel As PageContentModel) As KeyValuePair(Of Integer, Integer)

    <OperationContract()> _
    Function RetrievePageContentModelList(ByVal _PgItemModelID As Integer) As List(Of PageContentModel)

    <OperationContract()> _
    Function SavePageContentElement(ByVal _PageContentElement As PageContentElement) As KeyValuePair(Of Integer, Integer)

    <OperationContract()> _
    Function RetrievePageContentElementList(ByVal _PageContentModelID As Integer) As List(Of PageContentElement)

    
    <OperationContract()> _
   Function Retrieve_PageBlobInfo_WithIndex(ByVal _SurveyID As Integer, ByVal _IndexOfPage As Integer) As PageBlobInfo

    <OperationContract()> _
    Function Retrieve_PageBlob_Count(ByVal _SurveyID As Integer) As Integer

    <OperationContract()> _
    Function Retrieve_PgItemModelIDsOnly(ByVal _SurveyID As Integer) As List(Of Integer)

End Interface
<DataContract()> _
Public Class PageBlobInfo
    Private _PIM As String
    <DataMember()> _
    Public Property PIM() As String
        Get
            Return _PIM
        End Get
        Set(ByVal value As String)
            _PIM = value
        End Set
    End Property

    Private _PCM As String
    <DataMember()> _
    Public Property PCM() As String
        Get
            Return _PCM
        End Get
        Set(ByVal value As String)
            _PCM = value
        End Set
    End Property

    Private _PCE_Colxn As List(Of String)
    <DataMember()> _
    Public Property PCE_Colxn() As List(Of String)
        Get
            Return _PCE_Colxn
        End Get
        Set(ByVal value As List(Of String))
            _PCE_Colxn = value
        End Set
    End Property

End Class


'<Serializable()> _
'Public Class PageItemModel
'    'Inherits ModelCore(Of PageItemModel) 'implements Inotify....

'    'Public Sub New()
'    '    'Me.Model = Me 'this sets the Model property on the ModelCore(of T) base class
'    'End Sub

'#Region "Model Properties"
'    'Private _IgnatzDemoString As String = "This is the IgnatzDemo String"
'    '<XmlIgnore()> _
'    'Public Property IgnatzDemoString() As String
'    '    Get
'    '        Return _IgnatzDemoString
'    '    End Get
'    '    Set(ByVal value As String)
'    '        _IgnatzDemoString = value
'    '        'MyBase.My_OnPropertyChanged("IgnatzDemoString")
'    '    End Set
'    'End Property
'#End Region

'#Region "Properties"
'    Private _PageContent_Model As PageContentModel
'    <DataMember()> _
'    Property PageContent_Model() As PageContentModel
'        Get
'            Return _PageContent_Model
'        End Get
'        Set(ByVal value As PageContentModel)
'            _PageContent_Model = value
'            'My_OnPropertyChanged("PageContent_Model")
'        End Set

'    End Property

'    '<NonSerialized()> _
'    'Private _redundantPage As PageItemModel
'    '<XmlIgnore()> _
'    'Public Property Page() As PageItemModel
'    '    Get
'    '        Return Me
'    '    End Get
'    '    Set(ByVal value As PageItemModel)
'    '        '_redundantPage = Me
'    '        'My_OnPropertyChanged("Page")
'    '    End Set
'    'End Property

'    Private _SurveyName As String
'    <DataMember()> _
'    Public Property SurveyName() As String
'        Get
'            Return _SurveyName
'        End Get
'        Set(ByVal value As String)
'            _SurveyName = value
'            'My_OnPropertyChanged("SurveyName")
'        End Set
'    End Property

'    Private _Property1Value As String
'    <DataMember()> _
'    Public Property Property1Value() As String
'        Get
'            Return _Property1Value
'        End Get
'        Set(ByVal value As String)
'            _Property1Value = value
'            'My_OnPropertyChanged("Property1Value")
'        End Set
'    End Property

'    Private _PageNumber As String = "not set yet"
'    <DataMember()> _
'    Public Property PageNumber() As String
'        Get
'            Return _PageNumber
'        End Get
'        Set(ByVal value As String)
'            _PageNumber = value
'            'My_OnPropertyChanged("PageNumber")
'        End Set
'    End Property

'    Private _PageOptions As String = "no options set yet..." ' for now...would be some kind of object...
'    <DataMember()> _
'    Public Property PageOptions() As String
'        Get
'            Return _PageOptions
'        End Get
'        Set(ByVal value As String)
'            _PageOptions = value
'            'My_OnPropertyChanged("PageOptions")
'        End Set
'    End Property

'    Private _ModelofTSerialized As String = "not set yet"
'    <DataMember()> _
'    Public Property ModelofTSerialized() As String
'        Get
'            Return _ModelofTSerialized
'        End Get
'        Set(ByVal value As String)
'            _ModelofTSerialized = value
'        End Set
'    End Property

'#End Region
'End Class

