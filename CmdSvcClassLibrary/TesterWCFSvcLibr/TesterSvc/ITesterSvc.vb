' NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in App.config.
<ServiceContract()> _
Public Interface ITesterSvc

    '<OperationContract()> _
    'Function IamWorking() As String


    <OperationContract()> _
   Sub GoDoWhatISaid()

End Interface



