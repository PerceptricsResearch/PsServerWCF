﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.225
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


<Global.System.Data.Linq.Mapping.DatabaseAttribute(Name:="Survey4DataStore")>  _
Partial Public Class L2S_SurveyDSDataContext
	Inherits System.Data.Linq.DataContext
	
	Private Shared mappingSource As System.Data.Linq.Mapping.MappingSource = New AttributeMappingSource()
	
  #Region "Extensibility Method Definitions"
  Partial Private Sub OnCreated()
  End Sub
  Partial Private Sub InsertSurveyMaster(instance As SurveyMaster)
    End Sub
  Partial Private Sub UpdateSurveyMaster(instance As SurveyMaster)
    End Sub
  Partial Private Sub DeleteSurveyMaster(instance As SurveyMaster)
    End Sub
  #End Region
	
	Public Sub New()
		MyBase.New(Global.TesterWCFSvcLibr.My.MySettings.Default.Survey4DataStoreConnectionString, mappingSource)
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
	
	Public ReadOnly Property SurveyMasters() As System.Data.Linq.Table(Of SurveyMaster)
		Get
			Return Me.GetTable(Of SurveyMaster)
		End Get
	End Property
End Class

<Global.System.Data.Linq.Mapping.TableAttribute(Name:="dbo.SurveyMaster")>  _
Partial Public Class SurveyMaster
	Implements System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	
	Private Shared emptyChangingEventArgs As PropertyChangingEventArgs = New PropertyChangingEventArgs(String.Empty)
	
	Private _SurveyID As Integer
	
	Private _SurveyDescription As String
	
	Private _QueueURI As String
	
	Private _QueueName As String
	
	Private _QueueComputer As String
	
	Private _SurveyDataStoreID As Integer
	
	Private _SurveyDataStoreComputer As String
	
	Private _LoginID As Integer
	
	Private _CreatedDate As System.Nullable(Of Date)
	
	Private _LastModifiedDate As System.Nullable(Of Date)
	
	Private _FirstRespondentPostedDate As System.Nullable(Of Date)
	
	Private _LastRespondentPostedDate As System.Nullable(Of Date)
	
	Private _RespondenStartedtCount As Integer
	
	Private _RespondentCompletedCount As Integer
	
	Private _LastRespondentStartedDate As System.Nullable(Of Date)
	
	Private _LastRespondentCompletedDate As System.Nullable(Of Date)
	
	Private _ActiveRespondentsCount As Integer
	
	Private _FirstPublishedDate As System.Nullable(Of Date)
	
	Private _LastPublishedDate As System.Nullable(Of Date)
	
	Private _SurveyType As Integer
	
	Private _ResponsePostingSvcIsActive As Boolean
	
	Private _StatisticsViewerSvcIsActive As Boolean
	
	Private _FirstStatisticsViewerDate As System.Nullable(Of Date)
	
	Private _LastStatisticsViewerDate As System.Nullable(Of Date)
	
	Private _SurveyStateID As Integer
	
	Private _Model As String
	
    #Region "Extensibility Method Definitions"
    Partial Private Sub OnLoaded()
    End Sub
    Partial Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
    End Sub
    Partial Private Sub OnCreated()
    End Sub
    Partial Private Sub OnSurveyIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnSurveyIDChanged()
    End Sub
    Partial Private Sub OnSurveyDescriptionChanging(value As String)
    End Sub
    Partial Private Sub OnSurveyDescriptionChanged()
    End Sub
    Partial Private Sub OnQueueURIChanging(value As String)
    End Sub
    Partial Private Sub OnQueueURIChanged()
    End Sub
    Partial Private Sub OnQueueNameChanging(value As String)
    End Sub
    Partial Private Sub OnQueueNameChanged()
    End Sub
    Partial Private Sub OnQueueComputerChanging(value As String)
    End Sub
    Partial Private Sub OnQueueComputerChanged()
    End Sub
    Partial Private Sub OnSurveyDataStoreIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnSurveyDataStoreIDChanged()
    End Sub
    Partial Private Sub OnSurveyDataStoreComputerChanging(value As String)
    End Sub
    Partial Private Sub OnSurveyDataStoreComputerChanged()
    End Sub
    Partial Private Sub OnLoginIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnLoginIDChanged()
    End Sub
    Partial Private Sub OnCreatedDateChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnCreatedDateChanged()
    End Sub
    Partial Private Sub OnLastModifiedDateChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnLastModifiedDateChanged()
    End Sub
    Partial Private Sub OnFirstRespondentPostedDateChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnFirstRespondentPostedDateChanged()
    End Sub
    Partial Private Sub OnLastRespondentPostedDateChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnLastRespondentPostedDateChanged()
    End Sub
    Partial Private Sub OnRespondenStartedtCountChanging(value As Integer)
    End Sub
    Partial Private Sub OnRespondenStartedtCountChanged()
    End Sub
    Partial Private Sub OnRespondentCompletedCountChanging(value As Integer)
    End Sub
    Partial Private Sub OnRespondentCompletedCountChanged()
    End Sub
    Partial Private Sub OnLastRespondentStartedDateChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnLastRespondentStartedDateChanged()
    End Sub
    Partial Private Sub OnLastRespondentCompletedDateChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnLastRespondentCompletedDateChanged()
    End Sub
    Partial Private Sub OnActiveRespondentsCountChanging(value As Integer)
    End Sub
    Partial Private Sub OnActiveRespondentsCountChanged()
    End Sub
    Partial Private Sub OnFirstPublishedDateChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnFirstPublishedDateChanged()
    End Sub
    Partial Private Sub OnLastPublishedDateChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnLastPublishedDateChanged()
    End Sub
    Partial Private Sub OnSurveyTypeChanging(value As Integer)
    End Sub
    Partial Private Sub OnSurveyTypeChanged()
    End Sub
    Partial Private Sub OnResponsePostingSvcIsActiveChanging(value As Boolean)
    End Sub
    Partial Private Sub OnResponsePostingSvcIsActiveChanged()
    End Sub
    Partial Private Sub OnStatisticsViewerSvcIsActiveChanging(value As Boolean)
    End Sub
    Partial Private Sub OnStatisticsViewerSvcIsActiveChanged()
    End Sub
    Partial Private Sub OnFirstStatisticsViewerDateChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnFirstStatisticsViewerDateChanged()
    End Sub
    Partial Private Sub OnLastStatisticsViewerDateChanging(value As System.Nullable(Of Date))
    End Sub
    Partial Private Sub OnLastStatisticsViewerDateChanged()
    End Sub
    Partial Private Sub OnSurveyStateIDChanging(value As Integer)
    End Sub
    Partial Private Sub OnSurveyStateIDChanged()
    End Sub
    Partial Private Sub OnModelChanging(value As String)
    End Sub
    Partial Private Sub OnModelChanged()
    End Sub
    #End Region
	
	Public Sub New()
		MyBase.New
		OnCreated
	End Sub
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_SurveyID", DbType:="Int NOT NULL", IsPrimaryKey:=true)>  _
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
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_SurveyDescription", DbType:="VarChar(50) NOT NULL", CanBeNull:=false)>  _
	Public Property SurveyDescription() As String
		Get
			Return Me._SurveyDescription
		End Get
		Set
			If (String.Equals(Me._SurveyDescription, value) = false) Then
				Me.OnSurveyDescriptionChanging(value)
				Me.SendPropertyChanging
				Me._SurveyDescription = value
				Me.SendPropertyChanged("SurveyDescription")
				Me.OnSurveyDescriptionChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_QueueURI", DbType:="VarChar(50) NOT NULL", CanBeNull:=false)>  _
	Public Property QueueURI() As String
		Get
			Return Me._QueueURI
		End Get
		Set
			If (String.Equals(Me._QueueURI, value) = false) Then
				Me.OnQueueURIChanging(value)
				Me.SendPropertyChanging
				Me._QueueURI = value
				Me.SendPropertyChanged("QueueURI")
				Me.OnQueueURIChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_QueueName", DbType:="VarChar(50) NOT NULL", CanBeNull:=false)>  _
	Public Property QueueName() As String
		Get
			Return Me._QueueName
		End Get
		Set
			If (String.Equals(Me._QueueName, value) = false) Then
				Me.OnQueueNameChanging(value)
				Me.SendPropertyChanging
				Me._QueueName = value
				Me.SendPropertyChanged("QueueName")
				Me.OnQueueNameChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_QueueComputer", DbType:="VarChar(50) NOT NULL", CanBeNull:=false)>  _
	Public Property QueueComputer() As String
		Get
			Return Me._QueueComputer
		End Get
		Set
			If (String.Equals(Me._QueueComputer, value) = false) Then
				Me.OnQueueComputerChanging(value)
				Me.SendPropertyChanging
				Me._QueueComputer = value
				Me.SendPropertyChanged("QueueComputer")
				Me.OnQueueComputerChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_SurveyDataStoreID", DbType:="Int NOT NULL")>  _
	Public Property SurveyDataStoreID() As Integer
		Get
			Return Me._SurveyDataStoreID
		End Get
		Set
			If ((Me._SurveyDataStoreID = value)  _
						= false) Then
				Me.OnSurveyDataStoreIDChanging(value)
				Me.SendPropertyChanging
				Me._SurveyDataStoreID = value
				Me.SendPropertyChanged("SurveyDataStoreID")
				Me.OnSurveyDataStoreIDChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_SurveyDataStoreComputer", DbType:="VarChar(50) NOT NULL", CanBeNull:=false)>  _
	Public Property SurveyDataStoreComputer() As String
		Get
			Return Me._SurveyDataStoreComputer
		End Get
		Set
			If (String.Equals(Me._SurveyDataStoreComputer, value) = false) Then
				Me.OnSurveyDataStoreComputerChanging(value)
				Me.SendPropertyChanging
				Me._SurveyDataStoreComputer = value
				Me.SendPropertyChanged("SurveyDataStoreComputer")
				Me.OnSurveyDataStoreComputerChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_LoginID", DbType:="Int NOT NULL")>  _
	Public Property LoginID() As Integer
		Get
			Return Me._LoginID
		End Get
		Set
			If ((Me._LoginID = value)  _
						= false) Then
				Me.OnLoginIDChanging(value)
				Me.SendPropertyChanging
				Me._LoginID = value
				Me.SendPropertyChanged("LoginID")
				Me.OnLoginIDChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_CreatedDate", DbType:="DateTime")>  _
	Public Property CreatedDate() As System.Nullable(Of Date)
		Get
			Return Me._CreatedDate
		End Get
		Set
			If (Me._CreatedDate.Equals(value) = false) Then
				Me.OnCreatedDateChanging(value)
				Me.SendPropertyChanging
				Me._CreatedDate = value
				Me.SendPropertyChanged("CreatedDate")
				Me.OnCreatedDateChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_LastModifiedDate", DbType:="DateTime")>  _
	Public Property LastModifiedDate() As System.Nullable(Of Date)
		Get
			Return Me._LastModifiedDate
		End Get
		Set
			If (Me._LastModifiedDate.Equals(value) = false) Then
				Me.OnLastModifiedDateChanging(value)
				Me.SendPropertyChanging
				Me._LastModifiedDate = value
				Me.SendPropertyChanged("LastModifiedDate")
				Me.OnLastModifiedDateChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_FirstRespondentPostedDate", DbType:="DateTime")>  _
	Public Property FirstRespondentPostedDate() As System.Nullable(Of Date)
		Get
			Return Me._FirstRespondentPostedDate
		End Get
		Set
			If (Me._FirstRespondentPostedDate.Equals(value) = false) Then
				Me.OnFirstRespondentPostedDateChanging(value)
				Me.SendPropertyChanging
				Me._FirstRespondentPostedDate = value
				Me.SendPropertyChanged("FirstRespondentPostedDate")
				Me.OnFirstRespondentPostedDateChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_LastRespondentPostedDate", DbType:="DateTime")>  _
	Public Property LastRespondentPostedDate() As System.Nullable(Of Date)
		Get
			Return Me._LastRespondentPostedDate
		End Get
		Set
			If (Me._LastRespondentPostedDate.Equals(value) = false) Then
				Me.OnLastRespondentPostedDateChanging(value)
				Me.SendPropertyChanging
				Me._LastRespondentPostedDate = value
				Me.SendPropertyChanged("LastRespondentPostedDate")
				Me.OnLastRespondentPostedDateChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_RespondenStartedtCount", DbType:="Int NOT NULL")>  _
	Public Property RespondenStartedtCount() As Integer
		Get
			Return Me._RespondenStartedtCount
		End Get
		Set
			If ((Me._RespondenStartedtCount = value)  _
						= false) Then
				Me.OnRespondenStartedtCountChanging(value)
				Me.SendPropertyChanging
				Me._RespondenStartedtCount = value
				Me.SendPropertyChanged("RespondenStartedtCount")
				Me.OnRespondenStartedtCountChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_RespondentCompletedCount", DbType:="Int NOT NULL")>  _
	Public Property RespondentCompletedCount() As Integer
		Get
			Return Me._RespondentCompletedCount
		End Get
		Set
			If ((Me._RespondentCompletedCount = value)  _
						= false) Then
				Me.OnRespondentCompletedCountChanging(value)
				Me.SendPropertyChanging
				Me._RespondentCompletedCount = value
				Me.SendPropertyChanged("RespondentCompletedCount")
				Me.OnRespondentCompletedCountChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_LastRespondentStartedDate", DbType:="DateTime")>  _
	Public Property LastRespondentStartedDate() As System.Nullable(Of Date)
		Get
			Return Me._LastRespondentStartedDate
		End Get
		Set
			If (Me._LastRespondentStartedDate.Equals(value) = false) Then
				Me.OnLastRespondentStartedDateChanging(value)
				Me.SendPropertyChanging
				Me._LastRespondentStartedDate = value
				Me.SendPropertyChanged("LastRespondentStartedDate")
				Me.OnLastRespondentStartedDateChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_LastRespondentCompletedDate", DbType:="DateTime")>  _
	Public Property LastRespondentCompletedDate() As System.Nullable(Of Date)
		Get
			Return Me._LastRespondentCompletedDate
		End Get
		Set
			If (Me._LastRespondentCompletedDate.Equals(value) = false) Then
				Me.OnLastRespondentCompletedDateChanging(value)
				Me.SendPropertyChanging
				Me._LastRespondentCompletedDate = value
				Me.SendPropertyChanged("LastRespondentCompletedDate")
				Me.OnLastRespondentCompletedDateChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_ActiveRespondentsCount", DbType:="Int NOT NULL")>  _
	Public Property ActiveRespondentsCount() As Integer
		Get
			Return Me._ActiveRespondentsCount
		End Get
		Set
			If ((Me._ActiveRespondentsCount = value)  _
						= false) Then
				Me.OnActiveRespondentsCountChanging(value)
				Me.SendPropertyChanging
				Me._ActiveRespondentsCount = value
				Me.SendPropertyChanged("ActiveRespondentsCount")
				Me.OnActiveRespondentsCountChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_FirstPublishedDate", DbType:="DateTime")>  _
	Public Property FirstPublishedDate() As System.Nullable(Of Date)
		Get
			Return Me._FirstPublishedDate
		End Get
		Set
			If (Me._FirstPublishedDate.Equals(value) = false) Then
				Me.OnFirstPublishedDateChanging(value)
				Me.SendPropertyChanging
				Me._FirstPublishedDate = value
				Me.SendPropertyChanged("FirstPublishedDate")
				Me.OnFirstPublishedDateChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_LastPublishedDate", DbType:="DateTime")>  _
	Public Property LastPublishedDate() As System.Nullable(Of Date)
		Get
			Return Me._LastPublishedDate
		End Get
		Set
			If (Me._LastPublishedDate.Equals(value) = false) Then
				Me.OnLastPublishedDateChanging(value)
				Me.SendPropertyChanging
				Me._LastPublishedDate = value
				Me.SendPropertyChanged("LastPublishedDate")
				Me.OnLastPublishedDateChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_SurveyType", DbType:="Int NOT NULL")>  _
	Public Property SurveyType() As Integer
		Get
			Return Me._SurveyType
		End Get
		Set
			If ((Me._SurveyType = value)  _
						= false) Then
				Me.OnSurveyTypeChanging(value)
				Me.SendPropertyChanging
				Me._SurveyType = value
				Me.SendPropertyChanged("SurveyType")
				Me.OnSurveyTypeChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_ResponsePostingSvcIsActive", DbType:="Bit NOT NULL")>  _
	Public Property ResponsePostingSvcIsActive() As Boolean
		Get
			Return Me._ResponsePostingSvcIsActive
		End Get
		Set
			If ((Me._ResponsePostingSvcIsActive = value)  _
						= false) Then
				Me.OnResponsePostingSvcIsActiveChanging(value)
				Me.SendPropertyChanging
				Me._ResponsePostingSvcIsActive = value
				Me.SendPropertyChanged("ResponsePostingSvcIsActive")
				Me.OnResponsePostingSvcIsActiveChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_StatisticsViewerSvcIsActive", DbType:="Bit NOT NULL")>  _
	Public Property StatisticsViewerSvcIsActive() As Boolean
		Get
			Return Me._StatisticsViewerSvcIsActive
		End Get
		Set
			If ((Me._StatisticsViewerSvcIsActive = value)  _
						= false) Then
				Me.OnStatisticsViewerSvcIsActiveChanging(value)
				Me.SendPropertyChanging
				Me._StatisticsViewerSvcIsActive = value
				Me.SendPropertyChanged("StatisticsViewerSvcIsActive")
				Me.OnStatisticsViewerSvcIsActiveChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_FirstStatisticsViewerDate", DbType:="DateTime")>  _
	Public Property FirstStatisticsViewerDate() As System.Nullable(Of Date)
		Get
			Return Me._FirstStatisticsViewerDate
		End Get
		Set
			If (Me._FirstStatisticsViewerDate.Equals(value) = false) Then
				Me.OnFirstStatisticsViewerDateChanging(value)
				Me.SendPropertyChanging
				Me._FirstStatisticsViewerDate = value
				Me.SendPropertyChanged("FirstStatisticsViewerDate")
				Me.OnFirstStatisticsViewerDateChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_LastStatisticsViewerDate", DbType:="DateTime")>  _
	Public Property LastStatisticsViewerDate() As System.Nullable(Of Date)
		Get
			Return Me._LastStatisticsViewerDate
		End Get
		Set
			If (Me._LastStatisticsViewerDate.Equals(value) = false) Then
				Me.OnLastStatisticsViewerDateChanging(value)
				Me.SendPropertyChanging
				Me._LastStatisticsViewerDate = value
				Me.SendPropertyChanged("LastStatisticsViewerDate")
				Me.OnLastStatisticsViewerDateChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_SurveyStateID", DbType:="Int NOT NULL")>  _
	Public Property SurveyStateID() As Integer
		Get
			Return Me._SurveyStateID
		End Get
		Set
			If ((Me._SurveyStateID = value)  _
						= false) Then
				Me.OnSurveyStateIDChanging(value)
				Me.SendPropertyChanging
				Me._SurveyStateID = value
				Me.SendPropertyChanged("SurveyStateID")
				Me.OnSurveyStateIDChanged
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Model", DbType:="VarChar(MAX)")>  _
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
