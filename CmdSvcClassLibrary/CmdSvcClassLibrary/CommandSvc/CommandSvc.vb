Imports System.ServiceModel
Imports ICommandSvcNS
Imports CmdInfrastructureNS

Public Class CommandSvc
    Implements ICommandSvc





    'Private MyCSH As CmdSvcClassLibrary.CustomSvcHost = OperationContext.Current.Host

    'Public Sub IssueCommand(ByVal _CommandPkg As CmdInfrastructureNS.CommandPackage) Implements ICommandSvcNS.ICommandSvc.IssueCommand
    '    Try
    '        MyCSH.SvcMonitor.Update_ServiceCalls("IssueCommand" & DateTime.Now.ToLongTimeString, CmdInfrastructureNS.CommandPackage.ToText(_CommandPkg))
    '        MyCSH.CmdSvcMonitor.Update_CommandSvcCalls(_CommandPkg)
    '    Catch ex As Exception
    '        Dim x = 2
    '    End Try
    'End Sub
End Class
