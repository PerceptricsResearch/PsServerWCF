Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Web
'Namespace StatisticsService
<ServiceContract()> _
Public Interface IStatisticsService
    <OperationContract()> _
    Function GetMyStatus() As String
End Interface
Public Class StatisticsService
    Implements IStatisticsService


    Public Function GetMyStatus() As String Implements IStatisticsService.GetMyStatus
        Return "StatisticsService says it is functioning at " & DateAndTime.Now.ToShortTimeString
    End Function
End Class
'End Namespace