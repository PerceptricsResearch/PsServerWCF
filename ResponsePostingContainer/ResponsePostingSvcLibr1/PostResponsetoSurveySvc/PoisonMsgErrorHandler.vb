Imports System.Transactions
Imports System.ServiceModel.Dispatcher
Imports System.ServiceModel.Channels
Imports System.messaging

Public Class PoisonMsgErrorHandler
    Implements IErrorHandler

    Public SvcQueueName As String 'these guys are set by the PoisonErrorBehaviorAttribute at run time...
    Public SvcPoisonQueueName As String
    

    Public Sub ProvideFault(ByVal [error] As Exception, ByVal _version As Channels.MessageVersion, ByRef _fault As Channels.Message) Implements IErrorHandler.ProvideFault

    End Sub

    Public Function HandleError(ByVal [error] As Exception) As Boolean Implements IErrorHandler.HandleError
        Dim rslt = False
        If Not MessageQueue.Exists(SvcPoisonQueueName) Then
            Try
                MessageQueue.Create(SvcPoisonQueueName, True)
            Catch ex As Exception
                Dim yyy As Integer = 2
                'CommandSvcMonitor.Update_CommandSvcCalls("PoisonMsgErrorHandler reports exception when creating PoisonQueue..." & ex.Message, SvcPoisonQueueName)
            End Try
        End If
        Dim poisonException As MsmqPoisonMessageException = TryCast([error], MsmqPoisonMessageException)
        If poisonException IsNot Nothing Then
            Dim lookupID = poisonException.MessageLookupId
            Dim SvcQueue = New MessageQueue(SvcQueueName)
            Dim PoisonQueue = New MessageQueue(SvcPoisonQueueName)
            Dim pmsg As System.Messaging.Message = Nothing
            Dim IsMoved As Boolean = False
            'Try
            '    pmsg = SvcQueue.ReceiveByLookupId(lookupID)
            'Catch ex As Exception
            '    Dim zzz As Integer = 2
            'End Try
            'If pmsg Is Nothing Then
            '    IsMoved = True
            'Else
            Using TxScpe As New TransactionScope(TransactionScopeOption.RequiresNew)
                Dim retrycount As Integer = 0
                While retrycount < 3 AndAlso Not IsMoved
                    retrycount += 1
                    Try
                        pmsg = SvcQueue.ReceiveByLookupId(lookupID)
                        PoisonQueue.Send(pmsg, MessageQueueTransactionType.Automatic)
                        TxScpe.Complete()
                        IsMoved = True
                        rslt = True
                    Catch ex As Exception
                        If retrycount < 3 Then
                            Threading.Thread.Sleep(TimeSpan.FromSeconds(5))
                        Else
                            'CommandSvcMonitor.Update_CommandSvcCalls("PoisonMsgErrorHandler tried but failed to move message...", SvcPoisonQueueName)
                            Dim xxx As Integer = 2
                        End If
                    End Try


                End While
            End Using
            'End If

        Else
            'CommandSvcMonitor.Update_CommandSvcCalls("PoisonMsgErrorHandler says poisonException is nothing...", SvcPoisonQueueName)
            Dim xyz As Integer = 2
        End If

        Return rslt
    End Function


End Class
