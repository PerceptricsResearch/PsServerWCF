Imports System.ServiceModel
Imports IPostResponsetoSurveySvcNS


<ServiceContract()> _
Public Interface IRespDispatcherSvc

    <OperationContract(IsOneWay:=True)> _
    Sub DispatchResponses(ByVal _RDentModel As ResponDENTModel)


End Interface
