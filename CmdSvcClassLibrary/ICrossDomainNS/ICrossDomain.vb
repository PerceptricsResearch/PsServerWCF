Imports System.Runtime.Serialization
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Web
Imports System.ServiceModel

<ServiceContract()> _
Public Interface ICrossDomain

    <OperationContract()> _
   <WebGet(UriTemplate:="/clientaccesspolicy.xml")> _
    Function GetClientAccessPolicy() As Message
End Interface
