﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.3603
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Data.Linq
Imports System.Data.Linq.Mapping
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection


<System.Data.Linq.Mapping.DatabaseAttribute(Name:="ExxonSurvey4DataStore")>  _
Partial Public Class L2S_SurveyDataStoreDataContext
	Inherits System.Data.Linq.DataContext
	
	Private Shared mappingSource As System.Data.Linq.Mapping.MappingSource = New AttributeMappingSource
	
  #Region "Extensibility Method Definitions"
  Partial Private Sub OnCreated()
  End Sub
  Partial Private Sub InsertResultsSummary(instance As ResultsSummary)
    End Sub
  Partial Private Sub UpdateResultsSummary(instance As ResultsSummary)
    End Sub
  Partial Private Sub DeleteResultsSummary(instance As ResultsSummary)
    End Sub
  Partial Private Sub InsertResultsDetail(instance As ResultsDetail)
    End Sub
  Partial Private Sub UpdateResultsDetail(instance As ResultsDetail)
    End Sub
  Partial Private Sub DeleteResultsDetail(instance As ResultsDetail)
    End Sub
  Partial Private Sub InsertSDSResponseModel(instance As SDSResponseModel)
    End Sub
  Partial Private Sub UpdateSDSResponseModel(instance As SDSResponseModel)
    End Sub
  Partial Private Sub DeleteSDSResponseModel(instance As SDSResponseModel)
    End Sub
  Partial Private Sub InsertSDSRespondentModel(instance As SDSRespondentModel)
    End Sub
  Partial Private Sub UpdateSDSRespondentModel(instance As SDSRespondentModel)
    End Sub
  Partial Private Sub DeleteSDSRespondentModel(instance As SDSRespondentModel)
    End Sub
  Partial Private Sub InsertResponse(instance As Response)
    End Sub
  Partial Private Sub UpdateResponse(instance As Response)
    End Sub
  Partial Private Sub DeleteResponse(instance As Response)
    End Sub
  Partial Private Sub InsertResultsFilter(instance As ResultsFilter)
    End Sub
  Partial Private Sub UpdateResultsFilter(instance As ResultsFilter)
    End Sub
  Partial Private Sub DeleteResultsFilter(instance As ResultsFilter)
    End Sub
  #End Region
	
	Public Sub New()
		MyBase.New(Global.RsltsProviderSvcLibr.My.MySettings.Default.ExxonSurvey4DataStoreConnectionString, mappingSource)
		OnCreated
	End Sub
	
	Public Sub New(ByVal connection As String)
		MyBase.New(connection, mappingSource)
		OnCreated
	End Sub
	
	Public Sub New(ByVal connection As System.Data.IDbConnection)
		MyBase.New(connection, mappingSource)
		OnCreated
	End Sub
	
	Public Sub New(ByVal connection As String, ByVal mappingSource As System.Data.Linq.Mapping.MappingSource)
		MyBase.New(connection, mappingSource)
		OnCreated
	End Sub
	
	Public Sub New(ByVal connection As System.Data.IDbConnection, ByVal mappingSource As System.Data.Linq.Mapping.MappingSource)
		MyBase.New(connection, mappingSource)
		OnCreated
	End Sub
	
	Public ReadOnly Property ResultsSummaries() As System.Data.Linq.Table(Of ResultsSummary)
		Get
			Return Me.GetTable(Of ResultsSummary)
		End Get
	End Property
	
	Public ReadOnly Property ResultsDetails() As System.Data.Linq.Table(Of ResultsDetail)
		Get
			Return Me.GetTable(Of ResultsDetail)
		End Get
	End Property
	
	Public ReadOnly Property SDSResponseModels() As System.Data.Linq.Table(Of SDSResponseModel)
		Get
			Return Me.GetTable(Of SDSResponseModel)
		End Get
	End Property
	
	Public ReadOnly Property SDSRespondentModels() As System.Data.Linq.Table(Of SDSRespondentModel)
		Get
			Return Me.GetTable(Of SDSRespondentModel)
		End Get
	End Property
	
	Public ReadOnly Property Responses() As System.Data.Linq.Table(Of Response)
		Get
			Return Me.GetTable(Of Response)
		End Get
	End Property
	
	Public ReadOnly Property ResultsFilters() As System.Data.Linq.Table(Of ResultsFilter)
		Get
			Return Me.GetTable(Of ResultsFilter)
		End Get
	End Property
End Class

<Table(Name:="dbo.ResultsSummary")>  _
Partial Public Class ResultsSummary
	Implements System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	
	Private Shared emptyChangingEventArgs As PropertyChangingEventArgs = New PropertyChangingEventArgs(String.Empty)
	
	Private _ResultsSummaryID As Integer
	
	Private _ResultsSummaryAddress As String
	
	Private _SurveyID As System.Nullable(Of Integer)
	
	Private _ResultsDetails As EntitySet(Of ResultsDetail)
	
    #Region "Extensibility Method Definitions"
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub
    Partial Private Sub OnResultsSummaryIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnResultsSummaryIDChanged()
    End Sub
    Partial Private Sub OnResultsSummaryAddressChanging(value As String)
    End Sub
    Partial Private Sub OnResultsSummaryAddressChanged()
    End Sub
    Partial Private Sub OnSurveyIDChanging(value As System.Nullable(Of Integer))
    End Sub
    Partial Private Sub OnSurveyIDChanged()
    End Sub
    #End Region
	
	Public Sub New()
		MyBase.New
		Me._ResultsDetails = New EntitySet(Of ResultsDetail)(AddressOf Me.attach_ResultsDetails, AddressOf Me.detach_ResultsDetails)
		OnCreated
	End Sub
	
	<Column(Storage:="_ResultsSummaryID", AutoSync:=AutoSync.OnInsert, DbType:="Int NOT NULL IDENTITY", IsPrimaryKey:=true, IsDbGenerated:=true)>  _
	Public Property ResultsSummaryID() As Integer
		Get
			Return Me._ResultsSummaryID
		End Get
		Set
			If ((Me._ResultsSummaryID = value)  _
						= false) Then
				Me.OnResultsSummaryIDChanging(value)
				Me.SendPropertyChanging
				Me._ResultsSummaryID = value
				Me.SendPropertyChanged("ResultsSummaryID")
				Me.OnResultsSummaryIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_ResultsSummaryAddress", DbType:="VarChar(50) NOT NULL", CanBeNull:=false)>  _
	Public Property ResultsSummaryAddress() As String
		Get
			Return Me._ResultsSummaryAddress
		End Get
		Set
			If (String.Equals(Me._ResultsSummaryAddress, value) = false) Then
				Me.OnResultsSummaryAddressChanging(value)
				Me.SendPropertyChanging
				Me._ResultsSummaryAddress = value
				Me.SendPropertyChanged("ResultsSummaryAddress")
				Me.OnResultsSummaryAddressChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_SurveyID", DbType:="Int")>  _
	Public Property SurveyID() As System.Nullable(Of Integer)
		Get
			Return Me._SurveyID
		End Get
		Set
			If (Me._SurveyID.Equals(value) = false) Then
				Me.OnSurveyIDChanging(value)
				Me.SendPropertyChanging
				Me._SurveyID = value
				Me.SendPropertyChanged("SurveyID")
				Me.OnSurveyIDChanged
			End If
		End Set
	End Property
	
	<Association(Name:="ResultsSummary_ResultsDetail", Storage:="_ResultsDetails", ThisKey:="ResultsSummaryID", OtherKey:="ResultsSummaryID")>  _
	Public Property ResultsDetails() As EntitySet(Of ResultsDetail)
		Get
			Return Me._ResultsDetails
		End Get
		Set
			Me._ResultsDetails.Assign(value)
		End Set
	End Property
	
	Public Event PropertyChanging As PropertyChangingEventHandler Implements System.ComponentModel.INotifyPropertyChanging.PropertyChanging
	
	Public Event PropertyChanged As PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
	
	Protected Overridable Sub SendPropertyChanging()
		If ((Me.PropertyChangingEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanging(Me, emptyChangingEventArgs)
		End If
	End Sub
	
	Protected Overridable Sub SendPropertyChanged(ByVal propertyName As [String])
		If ((Me.PropertyChangedEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
		End If
	End Sub
	
	Private Sub attach_ResultsDetails(ByVal entity As ResultsDetail)
		Me.SendPropertyChanging
		entity.ResultsSummary = Me
	End Sub
	
	Private Sub detach_ResultsDetails(ByVal entity As ResultsDetail)
		Me.SendPropertyChanging
		entity.ResultsSummary = Nothing
	End Sub
End Class

<Table(Name:="dbo.ResultsDetail")>  _
Partial Public Class ResultsDetail
	Implements System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	
	Private Shared emptyChangingEventArgs As PropertyChangingEventArgs = New PropertyChangingEventArgs(String.Empty)
	
	Private _ResultsDetailID As Integer
	
	Private _SDSResponseModelID As Integer
	
	Private _ResultsSummaryID As Integer
	
	Private _RespDentCount As Integer
	
	Private _LastCountTimestamp As System.Nullable(Of Date)
	
	Private _ResultsSummary As EntityRef(Of ResultsSummary)
	
	Private _SDSResponseModel As EntityRef(Of SDSResponseModel)
	
    #Region "Extensibility Method Definitions"
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub
    Partial Private Sub OnResultsDetailIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnResultsDetailIDChanged()
    End Sub
    Partial Private Sub OnSDSResponseModelIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnSDSResponseModelIDChanged()
    End Sub
    Partial Private Sub OnResultsSummaryIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnResultsSummaryIDChanged()
    End Sub
    Partial Private Sub OnRespDentCountChanging(value As Integer)
    End Sub
    Partial Private Sub OnRespDentCountChanged()
    End Sub
    Partial Private Sub OnLastCountTimestampChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnLastCountTimestampChanged()
    End Sub
    #End Region
	
	Public Sub New()
		MyBase.New
		Me._ResultsSummary = CType(Nothing, EntityRef(Of ResultsSummary))
		Me._SDSResponseModel = CType(Nothing, EntityRef(Of SDSResponseModel))
		OnCreated
	End Sub
	
	<Column(Storage:="_ResultsDetailID", AutoSync:=AutoSync.OnInsert, DbType:="Int NOT NULL IDENTITY", IsPrimaryKey:=true, IsDbGenerated:=true)>  _
	Public Property ResultsDetailID() As Integer
		Get
			Return Me._ResultsDetailID
		End Get
		Set
			If ((Me._ResultsDetailID = value)  _
						= false) Then
				Me.OnResultsDetailIDChanging(value)
				Me.SendPropertyChanging
				Me._ResultsDetailID = value
				Me.SendPropertyChanged("ResultsDetailID")
				Me.OnResultsDetailIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_SDSResponseModelID", DbType:="Int NOT NULL")>  _
	Public Property SDSResponseModelID() As Integer
		Get
			Return Me._SDSResponseModelID
		End Get
		Set
			If ((Me._SDSResponseModelID = value)  _
						= false) Then
				If Me._SDSResponseModel.HasLoadedOrAssignedValue Then
					Throw New System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException
				End If
				Me.OnSDSResponseModelIDChanging(value)
				Me.SendPropertyChanging
				Me._SDSResponseModelID = value
				Me.SendPropertyChanged("SDSResponseModelID")
				Me.OnSDSResponseModelIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_ResultsSummaryID", DbType:="Int NOT NULL")>  _
	Public Property ResultsSummaryID() As Integer
		Get
			Return Me._ResultsSummaryID
		End Get
		Set
			If ((Me._ResultsSummaryID = value)  _
						= false) Then
				If Me._ResultsSummary.HasLoadedOrAssignedValue Then
					Throw New System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException
				End If
				Me.OnResultsSummaryIDChanging(value)
				Me.SendPropertyChanging
				Me._ResultsSummaryID = value
				Me.SendPropertyChanged("ResultsSummaryID")
				Me.OnResultsSummaryIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_RespDentCount", DbType:="Int NOT NULL")>  _
	Public Property RespDentCount() As Integer
		Get
			Return Me._RespDentCount
		End Get
		Set
			If ((Me._RespDentCount = value)  _
						= false) Then
				Me.OnRespDentCountChanging(value)
				Me.SendPropertyChanging
				Me._RespDentCount = value
				Me.SendPropertyChanged("RespDentCount")
				Me.OnRespDentCountChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_LastCountTimestamp", DbType:="DateTime")>  _
	Public Property LastCountTimestamp() As System.Nullable(Of Date)
		Get
			Return Me._LastCountTimestamp
		End Get
		Set
			If (Me._LastCountTimestamp.Equals(value) = false) Then
				Me.OnLastCountTimestampChanging(value)
				Me.SendPropertyChanging
				Me._LastCountTimestamp = value
				Me.SendPropertyChanged("LastCountTimestamp")
				Me.OnLastCountTimestampChanged
			End If
		End Set
	End Property
	
	<Association(Name:="ResultsSummary_ResultsDetail", Storage:="_ResultsSummary", ThisKey:="ResultsSummaryID", OtherKey:="ResultsSummaryID", IsForeignKey:=true)>  _
	Public Property ResultsSummary() As ResultsSummary
		Get
			Return Me._ResultsSummary.Entity
		End Get
		Set
			Dim previousValue As ResultsSummary = Me._ResultsSummary.Entity
			If ((Object.Equals(previousValue, value) = false)  _
						OrElse (Me._ResultsSummary.HasLoadedOrAssignedValue = false)) Then
				Me.SendPropertyChanging
				If ((previousValue Is Nothing)  _
							= false) Then
					Me._ResultsSummary.Entity = Nothing
					previousValue.ResultsDetails.Remove(Me)
				End If
				Me._ResultsSummary.Entity = value
				If ((value Is Nothing)  _
							= false) Then
					value.ResultsDetails.Add(Me)
					Me._ResultsSummaryID = value.ResultsSummaryID
				Else
					Me._ResultsSummaryID = CType(Nothing, Integer)
				End If
				Me.SendPropertyChanged("ResultsSummary")
			End If
		End Set
	End Property
	
	<Association(Name:="SDSResponseModel_ResultsDetail", Storage:="_SDSResponseModel", ThisKey:="SDSResponseModelID", OtherKey:="SDSResponseModelID", IsForeignKey:=true)>  _
	Public Property SDSResponseModel() As SDSResponseModel
		Get
			Return Me._SDSResponseModel.Entity
		End Get
		Set
			Dim previousValue As SDSResponseModel = Me._SDSResponseModel.Entity
			If ((Object.Equals(previousValue, value) = false)  _
						OrElse (Me._SDSResponseModel.HasLoadedOrAssignedValue = false)) Then
				Me.SendPropertyChanging
				If ((previousValue Is Nothing)  _
							= false) Then
					Me._SDSResponseModel.Entity = Nothing
					previousValue.ResultsDetails.Remove(Me)
				End If
				Me._SDSResponseModel.Entity = value
				If ((value Is Nothing)  _
							= false) Then
					value.ResultsDetails.Add(Me)
					Me._SDSResponseModelID = value.SDSResponseModelID
				Else
					Me._SDSResponseModelID = CType(Nothing, Integer)
				End If
				Me.SendPropertyChanged("SDSResponseModel")
			End If
		End Set
	End Property
	
	Public Event PropertyChanging As PropertyChangingEventHandler Implements System.ComponentModel.INotifyPropertyChanging.PropertyChanging
	
	Public Event PropertyChanged As PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
	
	Protected Overridable Sub SendPropertyChanging()
		If ((Me.PropertyChangingEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanging(Me, emptyChangingEventArgs)
		End If
	End Sub
	
	Protected Overridable Sub SendPropertyChanged(ByVal propertyName As [String])
		If ((Me.PropertyChangedEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
		End If
	End Sub
End Class

<Table(Name:="dbo.SDSResponseModel")>  _
Partial Public Class SDSResponseModel
	Implements System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	
	Private Shared emptyChangingEventArgs As PropertyChangingEventArgs = New PropertyChangingEventArgs(String.Empty)
	
	Private _SDSResponseModelID As Integer
	
	Private _QuestID As Integer
	
	Private _Key1 As Integer
	
	Private _Key2 As Integer
	
	Private _Key3 As Integer
	
	Private _SurveyID As Integer
	
	Private _ResultsDetails As EntitySet(Of ResultsDetail)
	
	Private _Responses As EntitySet(Of Response)
	
    #Region "Extensibility Method Definitions"
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub
    Partial Private Sub OnSDSResponseModelIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnSDSResponseModelIDChanged()
    End Sub
    Partial Private Sub OnQuestIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnQuestIDChanged()
    End Sub
    Partial Private Sub OnKey1Changing(value As Integer)
    End Sub
    Partial Private Sub OnKey1Changed()
    End Sub
    Partial Private Sub OnKey2Changing(value As Integer)
    End Sub
    Partial Private Sub OnKey2Changed()
    End Sub
    Partial Private Sub OnKey3Changing(value As Integer)
    End Sub
    Partial Private Sub OnKey3Changed()
    End Sub
    Partial Private Sub OnSurveyIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnSurveyIDChanged()
    End Sub
    #End Region
	
	Public Sub New()
		MyBase.New
		Me._ResultsDetails = New EntitySet(Of ResultsDetail)(AddressOf Me.attach_ResultsDetails, AddressOf Me.detach_ResultsDetails)
		Me._Responses = New EntitySet(Of Response)(AddressOf Me.attach_Responses, AddressOf Me.detach_Responses)
		OnCreated
	End Sub
	
	<Column(Storage:="_SDSResponseModelID", AutoSync:=AutoSync.OnInsert, DbType:="Int NOT NULL IDENTITY", IsPrimaryKey:=true, IsDbGenerated:=true)>  _
	Public Property SDSResponseModelID() As Integer
		Get
			Return Me._SDSResponseModelID
		End Get
		Set
			If ((Me._SDSResponseModelID = value)  _
						= false) Then
				Me.OnSDSResponseModelIDChanging(value)
				Me.SendPropertyChanging
				Me._SDSResponseModelID = value
				Me.SendPropertyChanged("SDSResponseModelID")
				Me.OnSDSResponseModelIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_QuestID", DbType:="Int NOT NULL")>  _
	Public Property QuestID() As Integer
		Get
			Return Me._QuestID
		End Get
		Set
			If ((Me._QuestID = value)  _
						= false) Then
				Me.OnQuestIDChanging(value)
				Me.SendPropertyChanging
				Me._QuestID = value
				Me.SendPropertyChanged("QuestID")
				Me.OnQuestIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_Key1", DbType:="Int NOT NULL")>  _
	Public Property Key1() As Integer
		Get
			Return Me._Key1
		End Get
		Set
			If ((Me._Key1 = value)  _
						= false) Then
				Me.OnKey1Changing(value)
				Me.SendPropertyChanging
				Me._Key1 = value
				Me.SendPropertyChanged("Key1")
				Me.OnKey1Changed
			End If
		End Set
	End Property
	
	<Column(Storage:="_Key2", DbType:="Int NOT NULL")>  _
	Public Property Key2() As Integer
		Get
			Return Me._Key2
		End Get
		Set
			If ((Me._Key2 = value)  _
						= false) Then
				Me.OnKey2Changing(value)
				Me.SendPropertyChanging
				Me._Key2 = value
				Me.SendPropertyChanged("Key2")
				Me.OnKey2Changed
			End If
		End Set
	End Property
	
	<Column(Storage:="_Key3", DbType:="Int NOT NULL")>  _
	Public Property Key3() As Integer
		Get
			Return Me._Key3
		End Get
		Set
			If ((Me._Key3 = value)  _
						= false) Then
				Me.OnKey3Changing(value)
				Me.SendPropertyChanging
				Me._Key3 = value
				Me.SendPropertyChanged("Key3")
				Me.OnKey3Changed
			End If
		End Set
	End Property
	
	<Column(Storage:="_SurveyID", DbType:="Int NOT NULL")>  _
	Public Property SurveyID() As Integer
		Get
			Return Me._SurveyID
		End Get
		Set
			If ((Me._SurveyID = value)  _
						= false) Then
				Me.OnSurveyIDChanging(value)
				Me.SendPropertyChanging
				Me._SurveyID = value
				Me.SendPropertyChanged("SurveyID")
				Me.OnSurveyIDChanged
			End If
		End Set
	End Property
	
	<Association(Name:="SDSResponseModel_ResultsDetail", Storage:="_ResultsDetails", ThisKey:="SDSResponseModelID", OtherKey:="SDSResponseModelID")>  _
	Public Property ResultsDetails() As EntitySet(Of ResultsDetail)
		Get
			Return Me._ResultsDetails
		End Get
		Set
			Me._ResultsDetails.Assign(value)
		End Set
	End Property
	
	<Association(Name:="SDSResponseModel_Response", Storage:="_Responses", ThisKey:="SDSResponseModelID", OtherKey:="ResponseModelID")>  _
	Public Property Responses() As EntitySet(Of Response)
		Get
			Return Me._Responses
		End Get
		Set
			Me._Responses.Assign(value)
		End Set
	End Property
	
	Public Event PropertyChanging As PropertyChangingEventHandler Implements System.ComponentModel.INotifyPropertyChanging.PropertyChanging
	
	Public Event PropertyChanged As PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
	
	Protected Overridable Sub SendPropertyChanging()
		If ((Me.PropertyChangingEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanging(Me, emptyChangingEventArgs)
		End If
	End Sub
	
	Protected Overridable Sub SendPropertyChanged(ByVal propertyName As [String])
		If ((Me.PropertyChangedEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
		End If
	End Sub
	
	Private Sub attach_ResultsDetails(ByVal entity As ResultsDetail)
		Me.SendPropertyChanging
		entity.SDSResponseModel = Me
	End Sub
	
	Private Sub detach_ResultsDetails(ByVal entity As ResultsDetail)
		Me.SendPropertyChanging
		entity.SDSResponseModel = Nothing
	End Sub
	
	Private Sub attach_Responses(ByVal entity As Response)
		Me.SendPropertyChanging
		entity.SDSResponseModel = Me
	End Sub
	
	Private Sub detach_Responses(ByVal entity As Response)
		Me.SendPropertyChanging
		entity.SDSResponseModel = Nothing
	End Sub
End Class

<Table(Name:="dbo.SDSRespondentModel")>  _
Partial Public Class SDSRespondentModel
	Implements System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	
	Private Shared emptyChangingEventArgs As PropertyChangingEventArgs = New PropertyChangingEventArgs(String.Empty)
	
	Private _RespondentModelID As Integer
	
	Private _CustomField As String
	
	Private _FirstName As String
	
	Private _LastName As String
	
	Private _IPAddress As String
	
	Private _SurveyID As Integer
	
	Private _Responses As EntitySet(Of Response)
	
    #Region "Extensibility Method Definitions"
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub
    Partial Private Sub OnRespondentModelIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnRespondentModelIDChanged()
    End Sub
    Partial Private Sub OnCustomFieldChanging(value As String)
    End Sub
    Partial Private Sub OnCustomFieldChanged()
    End Sub
    Partial Private Sub OnFirstNameChanging(value As String)
    End Sub
    Partial Private Sub OnFirstNameChanged()
    End Sub
    Partial Private Sub OnLastNameChanging(value As String)
    End Sub
    Partial Private Sub OnLastNameChanged()
    End Sub
    Partial Private Sub OnIPAddressChanging(value As String)
    End Sub
    Partial Private Sub OnIPAddressChanged()
    End Sub
    Partial Private Sub OnSurveyIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnSurveyIDChanged()
    End Sub
    #End Region
	
	Public Sub New()
		MyBase.New
		Me._Responses = New EntitySet(Of Response)(AddressOf Me.attach_Responses, AddressOf Me.detach_Responses)
		OnCreated
	End Sub
	
	<Column(Storage:="_RespondentModelID", AutoSync:=AutoSync.OnInsert, DbType:="Int NOT NULL IDENTITY", IsPrimaryKey:=true, IsDbGenerated:=true)>  _
	Public Property RespondentModelID() As Integer
		Get
			Return Me._RespondentModelID
		End Get
		Set
			If ((Me._RespondentModelID = value)  _
						= false) Then
				Me.OnRespondentModelIDChanging(value)
				Me.SendPropertyChanging
				Me._RespondentModelID = value
				Me.SendPropertyChanged("RespondentModelID")
				Me.OnRespondentModelIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_CustomField", DbType:="VarChar(50) NOT NULL", CanBeNull:=false)>  _
	Public Property CustomField() As String
		Get
			Return Me._CustomField
		End Get
		Set
			If (String.Equals(Me._CustomField, value) = false) Then
				Me.OnCustomFieldChanging(value)
				Me.SendPropertyChanging
				Me._CustomField = value
				Me.SendPropertyChanged("CustomField")
				Me.OnCustomFieldChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_FirstName", DbType:="VarChar(50)")>  _
	Public Property FirstName() As String
		Get
			Return Me._FirstName
		End Get
		Set
			If (String.Equals(Me._FirstName, value) = false) Then
				Me.OnFirstNameChanging(value)
				Me.SendPropertyChanging
				Me._FirstName = value
				Me.SendPropertyChanged("FirstName")
				Me.OnFirstNameChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_LastName", DbType:="VarChar(50)")>  _
	Public Property LastName() As String
		Get
			Return Me._LastName
		End Get
		Set
			If (String.Equals(Me._LastName, value) = false) Then
				Me.OnLastNameChanging(value)
				Me.SendPropertyChanging
				Me._LastName = value
				Me.SendPropertyChanged("LastName")
				Me.OnLastNameChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_IPAddress", DbType:="VarChar(50)")>  _
	Public Property IPAddress() As String
		Get
			Return Me._IPAddress
		End Get
		Set
			If (String.Equals(Me._IPAddress, value) = false) Then
				Me.OnIPAddressChanging(value)
				Me.SendPropertyChanging
				Me._IPAddress = value
				Me.SendPropertyChanged("IPAddress")
				Me.OnIPAddressChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_SurveyID", DbType:="Int NOT NULL")>  _
	Public Property SurveyID() As Integer
		Get
			Return Me._SurveyID
		End Get
		Set
			If ((Me._SurveyID = value)  _
						= false) Then
				Me.OnSurveyIDChanging(value)
				Me.SendPropertyChanging
				Me._SurveyID = value
				Me.SendPropertyChanged("SurveyID")
				Me.OnSurveyIDChanged
			End If
		End Set
	End Property
	
	<Association(Name:="SDSRespondentModel_Response", Storage:="_Responses", ThisKey:="RespondentModelID", OtherKey:="RespondentID")>  _
	Public Property Responses() As EntitySet(Of Response)
		Get
			Return Me._Responses
		End Get
		Set
			Me._Responses.Assign(value)
		End Set
	End Property
	
	Public Event PropertyChanging As PropertyChangingEventHandler Implements System.ComponentModel.INotifyPropertyChanging.PropertyChanging
	
	Public Event PropertyChanged As PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
	
	Protected Overridable Sub SendPropertyChanging()
		If ((Me.PropertyChangingEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanging(Me, emptyChangingEventArgs)
		End If
	End Sub
	
	Protected Overridable Sub SendPropertyChanged(ByVal propertyName As [String])
		If ((Me.PropertyChangedEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
		End If
	End Sub
	
	Private Sub attach_Responses(ByVal entity As Response)
		Me.SendPropertyChanging
		entity.SDSRespondentModel = Me
	End Sub
	
	Private Sub detach_Responses(ByVal entity As Response)
		Me.SendPropertyChanging
		entity.SDSRespondentModel = Nothing
	End Sub
End Class

<Table(Name:="dbo.Responses")>  _
Partial Public Class Response
	Implements System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	
	Private Shared emptyChangingEventArgs As PropertyChangingEventArgs = New PropertyChangingEventArgs(String.Empty)
	
	Private _ResponseID As Integer
	
	Private _RespondentID As Integer
	
	Private _ResponseModelID As Integer
	
	Private _PostedTimeStamp As Date
	
	Private _SDSRespondentModel As EntityRef(Of SDSRespondentModel)
	
	Private _SDSResponseModel As EntityRef(Of SDSResponseModel)
	
    #Region "Extensibility Method Definitions"
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub
    Partial Private Sub OnResponseIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnResponseIDChanged()
    End Sub
    Partial Private Sub OnRespondentIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnRespondentIDChanged()
    End Sub
    Partial Private Sub OnResponseModelIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnResponseModelIDChanged()
    End Sub
    Partial Private Sub OnPostedTimeStampChanging(value As Date)
    End Sub
    Partial Private Sub OnPostedTimeStampChanged()
    End Sub
    #End Region
	
	Public Sub New()
		MyBase.New
		Me._SDSRespondentModel = CType(Nothing, EntityRef(Of SDSRespondentModel))
		Me._SDSResponseModel = CType(Nothing, EntityRef(Of SDSResponseModel))
		OnCreated
	End Sub
	
	<Column(Storage:="_ResponseID", AutoSync:=AutoSync.OnInsert, DbType:="Int NOT NULL IDENTITY", IsPrimaryKey:=true, IsDbGenerated:=true)>  _
	Public Property ResponseID() As Integer
		Get
			Return Me._ResponseID
		End Get
		Set
			If ((Me._ResponseID = value)  _
						= false) Then
				Me.OnResponseIDChanging(value)
				Me.SendPropertyChanging
				Me._ResponseID = value
				Me.SendPropertyChanged("ResponseID")
				Me.OnResponseIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_RespondentID", DbType:="Int NOT NULL")>  _
	Public Property RespondentID() As Integer
		Get
			Return Me._RespondentID
		End Get
		Set
			If ((Me._RespondentID = value)  _
						= false) Then
				If Me._SDSRespondentModel.HasLoadedOrAssignedValue Then
					Throw New System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException
				End If
				Me.OnRespondentIDChanging(value)
				Me.SendPropertyChanging
				Me._RespondentID = value
				Me.SendPropertyChanged("RespondentID")
				Me.OnRespondentIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_ResponseModelID", DbType:="Int NOT NULL")>  _
	Public Property ResponseModelID() As Integer
		Get
			Return Me._ResponseModelID
		End Get
		Set
			If ((Me._ResponseModelID = value)  _
						= false) Then
				If Me._SDSResponseModel.HasLoadedOrAssignedValue Then
					Throw New System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException
				End If
				Me.OnResponseModelIDChanging(value)
				Me.SendPropertyChanging
				Me._ResponseModelID = value
				Me.SendPropertyChanged("ResponseModelID")
				Me.OnResponseModelIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_PostedTimeStamp", DbType:="DateTime NOT NULL")>  _
	Public Property PostedTimeStamp() As Date
		Get
			Return Me._PostedTimeStamp
		End Get
		Set
			If ((Me._PostedTimeStamp = value)  _
						= false) Then
				Me.OnPostedTimeStampChanging(value)
				Me.SendPropertyChanging
				Me._PostedTimeStamp = value
				Me.SendPropertyChanged("PostedTimeStamp")
				Me.OnPostedTimeStampChanged
			End If
		End Set
	End Property
	
	<Association(Name:="SDSRespondentModel_Response", Storage:="_SDSRespondentModel", ThisKey:="RespondentID", OtherKey:="RespondentModelID", IsForeignKey:=true)>  _
	Public Property SDSRespondentModel() As SDSRespondentModel
		Get
			Return Me._SDSRespondentModel.Entity
		End Get
		Set
			Dim previousValue As SDSRespondentModel = Me._SDSRespondentModel.Entity
			If ((Object.Equals(previousValue, value) = false)  _
						OrElse (Me._SDSRespondentModel.HasLoadedOrAssignedValue = false)) Then
				Me.SendPropertyChanging
				If ((previousValue Is Nothing)  _
							= false) Then
					Me._SDSRespondentModel.Entity = Nothing
					previousValue.Responses.Remove(Me)
				End If
				Me._SDSRespondentModel.Entity = value
				If ((value Is Nothing)  _
							= false) Then
					value.Responses.Add(Me)
					Me._RespondentID = value.RespondentModelID
				Else
					Me._RespondentID = CType(Nothing, Integer)
				End If
				Me.SendPropertyChanged("SDSRespondentModel")
			End If
		End Set
	End Property
	
	<Association(Name:="SDSResponseModel_Response", Storage:="_SDSResponseModel", ThisKey:="ResponseModelID", OtherKey:="SDSResponseModelID", IsForeignKey:=true)>  _
	Public Property SDSResponseModel() As SDSResponseModel
		Get
			Return Me._SDSResponseModel.Entity
		End Get
		Set
			Dim previousValue As SDSResponseModel = Me._SDSResponseModel.Entity
			If ((Object.Equals(previousValue, value) = false)  _
						OrElse (Me._SDSResponseModel.HasLoadedOrAssignedValue = false)) Then
				Me.SendPropertyChanging
				If ((previousValue Is Nothing)  _
							= false) Then
					Me._SDSResponseModel.Entity = Nothing
					previousValue.Responses.Remove(Me)
				End If
				Me._SDSResponseModel.Entity = value
				If ((value Is Nothing)  _
							= false) Then
					value.Responses.Add(Me)
					Me._ResponseModelID = value.SDSResponseModelID
				Else
					Me._ResponseModelID = CType(Nothing, Integer)
				End If
				Me.SendPropertyChanged("SDSResponseModel")
			End If
		End Set
	End Property
	
	Public Event PropertyChanging As PropertyChangingEventHandler Implements System.ComponentModel.INotifyPropertyChanging.PropertyChanging
	
	Public Event PropertyChanged As PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
	
	Protected Overridable Sub SendPropertyChanging()
		If ((Me.PropertyChangingEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanging(Me, emptyChangingEventArgs)
		End If
	End Sub
	
	Protected Overridable Sub SendPropertyChanged(ByVal propertyName As [String])
		If ((Me.PropertyChangedEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
		End If
	End Sub
End Class

<Table(Name:="dbo.ResultsFilter")>  _
Partial Public Class ResultsFilter
	Implements System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	
	Private Shared emptyChangingEventArgs As PropertyChangingEventArgs = New PropertyChangingEventArgs(String.Empty)
	
	Private _ID As Integer
	
	Private _Model As String
	
	Private _SurveyID As Integer
	
    #Region "Extensibility Method Definitions"
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub
    Partial Private Sub OnIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnIDChanged()
    End Sub
    Partial Private Sub OnModelChanging(value As String)
    End Sub
    Partial Private Sub OnModelChanged()
    End Sub
    Partial Private Sub OnSurveyIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnSurveyIDChanged()
    End Sub
    #End Region
	
	Public Sub New()
		MyBase.New
		OnCreated
	End Sub
	
	<Column(Storage:="_ID", AutoSync:=AutoSync.OnInsert, DbType:="Int NOT NULL IDENTITY", IsPrimaryKey:=true, IsDbGenerated:=true)>  _
	Public Property ID() As Integer
		Get
			Return Me._ID
		End Get
		Set
			If ((Me._ID = value)  _
						= false) Then
				Me.OnIDChanging(value)
				Me.SendPropertyChanging
				Me._ID = value
				Me.SendPropertyChanged("ID")
				Me.OnIDChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_Model", DbType:="VarChar(MAX) NOT NULL", CanBeNull:=false)>  _
	Public Property Model() As String
		Get
			Return Me._Model
		End Get
		Set
			If (String.Equals(Me._Model, value) = false) Then
				Me.OnModelChanging(value)
				Me.SendPropertyChanging
				Me._Model = value
				Me.SendPropertyChanged("Model")
				Me.OnModelChanged
			End If
		End Set
	End Property
	
	<Column(Storage:="_SurveyID", DbType:="Int NOT NULL")>  _
	Public Property SurveyID() As Integer
		Get
			Return Me._SurveyID
		End Get
		Set
			If ((Me._SurveyID = value)  _
						= false) Then
				Me.OnSurveyIDChanging(value)
				Me.SendPropertyChanging
				Me._SurveyID = value
				Me.SendPropertyChanged("SurveyID")
				Me.OnSurveyIDChanged
			End If
		End Set
	End Property
	
	Public Event PropertyChanging As PropertyChangingEventHandler Implements System.ComponentModel.INotifyPropertyChanging.PropertyChanging
	
	Public Event PropertyChanged As PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
	
	Protected Overridable Sub SendPropertyChanging()
		If ((Me.PropertyChangingEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanging(Me, emptyChangingEventArgs)
		End If
	End Sub
	
	Protected Overridable Sub SendPropertyChanged(ByVal propertyName As [String])
		If ((Me.PropertyChangedEvent Is Nothing)  _
					= false) Then
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
		End If
	End Sub
End Class
