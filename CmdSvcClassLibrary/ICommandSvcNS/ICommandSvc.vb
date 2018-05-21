Imports System.ServiceModel
Imports CmdInfrastructureNS

<ServiceContract()> _
Public Interface ICommandSvc
    '<OperationContract(IsOneWay:=True)> _
    'Sub IssueCommand(ByVal _CommandPkg As CommandPackage)

End Interface
