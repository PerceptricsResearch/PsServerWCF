Imports CmdInfrastructureNS
Imports System.Data.SqlClient
Imports System.Text
Imports System.Configuration

Public Class Manager
    Implements IDisposable

    Public DC_ConnectionString As String
    Public PrivBitMask As ULong

#Region "PrivilegeServiceMappings"
    ''' <summary>
    ''' Returns a Survey_Priv_DC_Colxn_Pkg that contains the ServiceTypeEnumBitMask and the contents of a DC_Package(surveyPrivList and Survey_DC_list)
    ''' </summary>
    ''' <param name="_LoginID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPrivServiceMappings(ByVal _LoginID As Integer) As Survey_Priv_DC_Colxn_Pkg
        Dim rslt As New Survey_Priv_DC_Colxn_Pkg
        Try
            Dim conn = New SqlConnection(DC_ConnectionString)
            Dim da = New SqlDataAdapter("proc_GetSurveyIDsAndPrivilegeIDsByLoginID", conn)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Add("@LoginID", SqlDbType.Int)
            da.SelectCommand.Parameters("@LoginID").Value = _LoginID
            conn.Open()
            Dim ds = New DataSet()
            da.Fill(ds)


            'da.Fill(_surveyPrivilegeMappings)
            ' Dim rslt As New Survey_Priv_DC_Colxn_Pkg

            'ServiceEnumBitMsk
            ' Dim ServiceEnumBitMask As ULong = 0


            'Note...distinct PrivilegeEnumBitmasks for all surveys, imply a serviceEnumBitMask(i.e. what Services should be
            'available to a particular LoginID...is a function of what privileges the login has with surveys...

            If ds IsNot Nothing Then
                If ds.Tables.Count > 0 Then
                    Dim _surveyPrivilegeMappings = ds.Tables(0)

                    Dim pbm = From dr In ds.Tables(2).DefaultView _
                            Select dr("PrivBitMask")
                    Me.PrivBitMask = pbm.FirstOrDefault

                    Dim privEnumlist As New List(Of ULong)
                    privEnumlist.Add(Me.PrivBitMask)
                    If _surveyPrivilegeMappings.Rows.Count > 0 Then
                        For Each dr In _surveyPrivilegeMappings.DefaultView.ToTable(True, "PrivEnumBitMask").Rows
                            privEnumlist.Add(dr("PrivEnumBitMask"))
                        Next
                        Dim SurvPrivKvps = From dr1 In _surveyPrivilegeMappings.DefaultView _
                                                      Where dr1("PrivEnumBitMask") > 0 _
                                                      Select New Srlzd_KVP With {.Key = dr1("SurveyID"), _
                                                                               .Valu = dr1("PrivEnumBitMask")}
                        If SurvPrivKvps.Any Then
                            rslt.Survey_Privilege_List = SurvPrivKvps.ToList
                            'Let Cnxstr = CnxnString_DatabaseName(dr("DatabaseName"))....cnxnstring isin the database now...
                            rslt.Survey_DC_List = (From kvp In rslt.Survey_Privilege_List, dr In ds.Tables(1).DefaultView _
                                        Where kvp.Key = dr("SurveyID") _
                                        Let Cnxstr = dr("CnxnString") _
                                        Let SurvID = dr("SurveyID") _
                                        Select New Srlzd_KVP With {.Key = SurvID, _
                                                                   .Valu = Cnxstr}).ToList
                        End If
                    End If
                    If privEnumlist.Count > 0 Then
                        rslt.ServiceEnumBitMsk = EnumBitMaskOperations.EvaluatePrivilegeUsingListofUlong(privEnumlist)
                    End If
                End If
            End If

            'Dim _surveyID_DbName = ds.Tables(1)

            'rslt.Survey_DC_List = (From dr In _surveyID_DbName.DefaultView _
            '                                                    Let Cnxstr = CnxnString_DatabaseName(dr("DatabaseName")) _
            '                                                    Let SurvID = dr("SurveyID") _
            '                                                    Select New Srlzd_KVP With {.Key = SurvID, _
            '                                                                               .Valu = Cnxstr}).ToList


            conn.Close()
            conn.Dispose()
            da.Dispose()
            conn = Nothing
            da = Nothing
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "CustomerDBOperations"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Manager.GetPrivServiceMappings: reports error... " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

        Return rslt
    End Function
#End Region

#Region "Initialize New Subscriber Database Tables"
    Public Function UpdateNewSubscriberTables(ByVal _CustomerName As String, _
                                               ByVal _NormalizedEmailAddress As String, _
                                               ByVal _DataStorePath As String, _
                                               ByVal _gsmID As Integer, _
                                               ByVal _DataStore_databaseName As String) As Boolean
        Dim rslt As Boolean = False
        Try
            'Dim db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
            Using db As New L2S_CustomerSurveyMasterDBDataContext(DC_ConnectionString)
                db.Customers.FirstOrDefault.CustomerName = _CustomerName
                db.LoginInfos.FirstOrDefault.LoginEmail = _NormalizedEmailAddress
                db.SurveyDataStores.FirstOrDefault.AbsolutePath = _DataStorePath
                db.SurveyDataStores.FirstOrDefault.DatabaseName = _DataStore_databaseName
                db.SubmitChanges()
            End Using
            rslt = True
        Catch ex As Exception
            Using EvLog As New EventLog()
                EvLog.Source = "CustomerDBOperations"
                EvLog.Log = "Application"
                EvLog.WriteEntry("Manager.UpdateNewSubscriberTables: reports error... " & ex.Message, EventLogEntryType.Error)
            End Using
        End Try

        Return rslt
    End Function
#End Region

    'Public Function CnxnString_DatabaseName(ByVal _DatabaseName As String) As String
    '    Dim rslt As String = "NotSet"
    '    'Dim builder As New SqlConnectionStringBuilder
    '    ''builder.InitialCatalog = _DatabaseName
    '    'builder.Item("Database") = _DatabaseName
    '    'builder.IntegratedSecurity = True
    '    'builder.DataSource = ".\DEVRENTS"
    '    'builder.UserInstance = False
    '    'builder.LoadBalanceTimeout = 30
    '    'rslt = builder.ConnectionString
    '    'builder = Nothing

    '    'this needs to get the right database instance...e.g. LEASES\DEVRENTS.....\DEVRENTS doesn't work when we separate the database machine from this service...

    '    Dim sb = New StringBuilder()
    '    sb.Append("Data Source=LEASES\DEVRENTS;Database=")
    '    sb.Append(_DatabaseName)
    '    sb.Append(";Integrated Security=True;Connect Timeout=30;User Instance=False")
    '    rslt = sb.ToString()
    '    Return rslt
    'End Function

    'Private Function GetValueFromAppConfig(ByVal _ConfigKey) As String
    '    Dim rslt As String = Nothing
    '    Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
    '    rslt = config.AppSettings.Settings(_ConfigKey).Value
    '    'rslt = ConfigurationManager.AppSettings(_ConfigKey)
    '    Return rslt
    'End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Me.DC_ConnectionString = Nothing
                Me.PrivBitMask = Nothing
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
