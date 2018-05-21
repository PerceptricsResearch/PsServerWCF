Imports System.ServiceModel

<ServiceContract()> _
Public Interface ITesterSvc

    '<OperationContract()> _
    'Function IamWorking() As String


    <OperationContract(IsOneWay:=True)> _
   Sub GoDoWhatISaid()

End Interface
