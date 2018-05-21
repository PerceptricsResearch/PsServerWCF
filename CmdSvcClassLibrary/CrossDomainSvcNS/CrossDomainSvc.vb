Imports ICrossDomainNS
Imports System.IO
Imports System.Xml
Imports System.ServiceModel.Channels

Public Class CrossDomainSvc
    Implements ICrossDomainNS.ICrossDomain

#Region "Client Access Policy stuff"
    Public Function GetClientAccessPolicy() As System.ServiceModel.Channels.Message Implements ICrossDomain.GetClientAccessPolicy

        Dim capstring = <?xml version="1.0" encoding="utf-8"?>
                        <access-policy>
                            <cross-domain-access>
                                <policy>
                                    <allow-from http-request-headers="*">
                                        <domain uri="http://*"/>
                                        <domain uri="https://*"/>
                                    </allow-from>
                                    <grant-to>
                                        <resource path="/" include-subpaths="true"/>
                                    </grant-to>
                                </policy>
                            </cross-domain-access>
                        </access-policy>
        Dim reader = New StringReader(capstring.ToString)

        Dim myxmlReader = XmlReader.Create(reader)
        Return Message.CreateMessage(MessageVersion.None, "", myxmlReader)
    End Function
#End Region
    '<domain uri="*"/>
End Class
