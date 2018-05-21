' NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
Public Class LoginSvc
    Implements ILoginSvc




    Public Function ChangeMyPassword(ByVal _ChangePasswordPack As ChangePasswordPackage) As ChangePasswordResult Implements ILoginSvc.ChangeMyPassword
        Dim rslt As ChangePasswordResult = Nothing
        Return rslt
    End Function

    Private MyEptDCClient As New EndPtDCSvc.EndPtDataContextSvcClient

    Public Function LogMeInPlease(ByVal _LogInPack As LogInPackage) As LogInResult Implements ILoginSvc.LogMeInPlease
        ServiceMonitor.Update_ServiceCalls("IN  " & DateTime.Now.ToLongTimeString, _
                                           "(LogIn_Email=<" & _LogInPack.LogIn_Email & ">")
        Dim spinup = SpinUpCustomerDBSvc(_LogInPack.LogIn_Email) 'function to make this wait a little..
        ServiceMonitor.Update_ServiceCalls("SpinUpCustomerDBSvc = " & spinup.ToString & " " & DateTime.Now.ToLongTimeString, "(LogIn_Email=<" & _LogInPack.LogIn_Email & ">")

        Dim LogInrslt As LogInResult = New LogInResult
        LogInrslt.LogIn_IsSuccess = False
        LogInrslt.SpiffList = New List(Of Srlzd_KVP)

        Try
            Dim eptLoginPkg = Populate_EndPtKeysList(_LogInPack.LogIn_Email) 'this comes from EptSvc.Retrieve_ListOf_EptPkg
            LogInrslt.LogIn_IsSuccess = eptLoginPkg.IsAuthenticated
            If eptLoginPkg.IsAuthenticated Then
                LogInrslt.EndpointKeysList = eptLoginPkg.ListOfEndPtPackage.ToList
                LogInrslt.SpiffList.Add(New Srlzd_KVP("Message", "Success..You have " & LogInrslt.EndpointKeysList.Count & " endpoint keys..."))
               
            Else
                If eptLoginPkg.IsLoginEmailFound Then
                    LogInrslt.SpiffList.Add(New Srlzd_KVP("Message", "The password is incorrect... "))
                Else
                    LogInrslt.SpiffList.Add(New Srlzd_KVP("Message", "Could not find the Login Name entered... "))
                End If
            End If
        Catch ex As Exception
            LogInrslt.SpiffList.Add(New Srlzd_KVP("Message", "Failure..can't connect to Server..."))
            ServiceMonitor.Update_ServiceCalls("BLOWEDUP IN  " & DateTime.Now.ToLongTimeString, "(LogIn_Email=<" & _LogInPack.LogIn_Email & ">" & ex.Message)
        End Try

        ServiceMonitor.Update_ServiceCalls(LogInrslt.LogIn_IsSuccess.ToString & " " & DateTime.Now.ToLongTimeString, "(LogIn_Email=<" & _LogInPack.LogIn_Email & ">")
        Return LogInrslt
    End Function

    Private Function Populate_EndPtKeysList(ByVal _LogIn_Email As String) As EndPtDCSvc.EptLoginPackage
        Dim rslt As New EndPtDCSvc.EptLoginPackage
        'Dim EPtDCClient As New EndPtDCSvc.EndPtDataContextSvcClient
        Try
            rslt = MyEptDCClient.Retrieve_List_of_EndPtPkg(_LogIn_Email)
            If rslt.IsAuthenticated Then
                MyEptDCClient.IssueExposeEndPtCommands(_LogIn_Email, rslt.LogIn_ID)
            End If
        Catch ex As Exception
            ServiceMonitor.Update_ServiceCalls("BLOWEDUP EndPtKeysList  " & DateTime.Now.ToLongTimeString, _
                                               "LogIn_Email=<" & _LogIn_Email & ">" & ex.Message)
        End Try
        Try
            MyEptDCClient.Close() 'should be done with it here...
        Catch ex As Exception
            ServiceMonitor.Update_ServiceCalls("BLOWEDUP TryCloseClient EndPtKeysList  " & DateTime.Now.ToLongTimeString, _
                                               "LogIn_Email=<" & _LogIn_Email & ">" & ex.Message)
            MyEptDCClient = Nothing
        End Try
        Return rslt
    End Function

    Private Function SpinUpCustomerDBSvc(ByVal _Login_Email As String) As Boolean
        Dim rslt As Boolean = False
        Dim EPtDCClient As New EndPtDCSvc.EndPtDataContextSvcClient
        Try
            rslt = EPtDCClient.SpinUpCustomerMasterDBSvc(_Login_Email)
            
        Catch ex As Exception
            ServiceMonitor.Update_ServiceCalls("BLOWEDUP SpinUpCustomerDBSvc  " & DateTime.Now.ToLongTimeString, "(LogIn_Email=<" & _Login_Email & ">" & ex.Message)
        End Try
        Try
            EPtDCClient.Close()
        Catch ex As Exception
            ServiceMonitor.Update_ServiceCalls("BLOWEDUP TryCloseClient SpinUpCustomerDBSvc " & DateTime.Now.ToLongTimeString, "(LogIn_Email=<" & _Login_Email & ">" & ex.Message)
            EPtDCClient = Nothing
        End Try
        Return rslt
    End Function


    'Private Sub IssueMSurrveyEpt_Command(ByVal _cmdString1 As String, ByVal _cmdStrin2 As String)
    '    Dim rslt As Boolean = False
    '    Dim PgItClxn_CmdsvcClient As New PgItemsColxnCommandSvc.CommandSvcClient
    '    Try
    '        PgItClxn_CmdsvcClient.Open()
    '        PgItClxn_CmdsvcClient.IssueCommand(_cmdString1, _cmdStrin2)
    '        PgItClxn_CmdsvcClient.Close()
    '        ServiceMonitor.Update_ServiceCalls("PgItemsColxn_Commandsvc " & DateTime.Now.ToLongTimeString, _cmdString1 & " " & _cmdStrin2)
    '        rslt = True
    '    Catch ex As Exception
    '        ServiceMonitor.Update_ServiceCalls("BLOWEDUP PgItemsColxn_Commandsvc " & DateTime.Now.ToLongTimeString, ex.Message)
    '    End Try
    '    'Return rslt
    'End Sub
    'Private Sub IssuePgItemsColxn_EptCommand(ByVal _cmdString1 As String, ByVal _cmdStrin2 As String)
    '    Dim rslt As Boolean = False
    '    Dim PgItClxn_CmdsvcClient As New PgItemsColxnCommandSvc.CommandSvcClient
    '    Try
    '        PgItClxn_CmdsvcClient.Open()
    '        PgItClxn_CmdsvcClient.IssueCommand(_cmdString1, _cmdStrin2)
    '        PgItClxn_CmdsvcClient.Close()
    '        ServiceMonitor.Update_ServiceCalls("PgItemsColxn_Commandsvc " & DateTime.Now.ToLongTimeString, _cmdString1 & " " & _cmdStrin2)
    '        rslt = True
    '    Catch ex As Exception
    '        ServiceMonitor.Update_ServiceCalls("BLOWEDUP PgItemsColxn_Commandsvc " & DateTime.Now.ToLongTimeString, ex.Message)
    '    End Try
    '    'Return rslt
    'End Sub

    Public Function LogMeOutPlease(ByVal _LogOutPack As LogOutPackage) As LogOutResult Implements ILoginSvc.LogMeOutPlease
        Dim rslt As LogOutResult = Nothing
        ServiceMonitor.Update_ServiceCalls("Out  " & DateTime.Now.ToLongTimeString, "(LogIn_Email=<" & _LogOutPack.LogIn_Email & ">")
        'IssuePgItemsColxn_Command("XStop", "TestLoginEPTAddr")
        rslt.SpiffList = New List(Of Srlzd_KVP)
        rslt.SpiffList.Add(New Srlzd_KVP("Message", "Thanks for being here!!"))
        Dim durationMinutes = DateDiff(DateInterval.Minute, _LogOutPack.LogIn_Result.LogIn_DateTime, DateTime.Now)
        rslt.SpiffList.Add(New Srlzd_KVP("Message", "Elapsed time: " & durationMinutes.ToString & " minutes"))
        Return rslt
    End Function

    Public Function ResetMyPassword(ByVal _ResetPasswordPack As ResetPasswordPackage) As ResetPasswordResult Implements ILoginSvc.ResetMyPassword
        Dim rslt As ResetPasswordResult = Nothing
        Return rslt
    End Function

    Public Function WhoIsHere(ByVal _WhoIsHerePack As WhoIsHerePackage) As WhoIsHereResult Implements ILoginSvc.WhoIsHere
        Dim rslt As WhoIsHereResult = Nothing
        Return rslt
    End Function
End Class
