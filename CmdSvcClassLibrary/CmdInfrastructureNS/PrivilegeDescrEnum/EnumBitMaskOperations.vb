Imports System.Reflection
Public Class EnumBitMaskOperations


    Public Shared Function BmskToList(Of T)(ByVal _Tvalue As T) As List(Of String)
        Dim rslt As List(Of String) = Nothing

        Dim rawstring = (CType(_Tvalue, T).ToString)
        Dim q = From elem In Strings.Split(rawstring, ",") _
                Select elem

        If q.Any Then
            rslt = q.ToList
        End If
        Return rslt
    End Function

    Public Shared Function BmskToValues(Of T)(ByVal _Tvalue As T) As List(Of ULong)
        Dim rslt As New List(Of ULong)
        Dim stringlist = BmskToList(Of T)(_Tvalue)
        For Each enumstr In stringlist
            rslt.Add(GetValueFromEnumStr(Of T)(enumstr))
        Next
        Return rslt
    End Function
    Public Shared Function GetValueFromEnumStr(Of T)(ByVal _Tvalue As String) As ULong
        Dim rslt As ULong
        rslt = [Enum].Parse(GetType(T), _Tvalue, True)
        Return rslt
    End Function

    Public Shared Function EvaluatePrivilege(ByVal _EnumValue As Integer) As ULong
        Dim rslt As ULong = 0

        If (_EnumValue And PriviligeDescrEnum.AuthorRead) > 0 _
        Or (_EnumValue And PriviligeDescrEnum.AuthorWrite) > 0 _
        Or (_EnumValue And PriviligeDescrEnum.ReadAny) > 0 _
        Or (_EnumValue And PriviligeDescrEnum.WriteAny) > 0 _
        Or (_EnumValue And PriviligeDescrEnum.RDispatcherSvc) > 0 _
        Or (_EnumValue And PriviligeDescrEnum.RPostingSvc) > 0 Then
            rslt = rslt Or ServiceTypeBMskEnum.AuthoringSvc
        End If

        If (_EnumValue And PriviligeDescrEnum.RsltsViewer) > 0 _
           Then
            rslt = rslt Or ServiceTypeBMskEnum.ResultsProviderSvc
        End If

        If (_EnumValue And PriviligeDescrEnum.Respondent) > 0 Then
            rslt = rslt Or ServiceTypeBMskEnum.RespProviderSvc
        End If

        If (_EnumValue And PriviligeDescrEnum.PerceptricsAdministrator) > 0 Then
            rslt = rslt Or ServiceTypeBMskEnum.AdministratorSvc
        End If
        Return rslt
    End Function

    Public Shared Function EvaluatePrivilegeUsingListofUlong(ByVal _EnumValueList As List(Of ULong)) As ULong
        Dim rslt As ULong = 0
        For Each _EnumValue In _EnumValueList
            If (_EnumValue And PriviligeDescrEnum.AuthorRead) > 0 _
            Or (_EnumValue And PriviligeDescrEnum.AuthorWrite) > 0 _
            Or (_EnumValue And PriviligeDescrEnum.ReadAny) > 0 _
            Or (_EnumValue And PriviligeDescrEnum.WriteAny) > 0 _
            Or (_EnumValue And PriviligeDescrEnum.RDispatcherSvc) > 0 _
            Or (_EnumValue And PriviligeDescrEnum.RPostingSvc) > 0 Then
                rslt = rslt Or ServiceTypeBMskEnum.AuthoringSvc
            End If

            If (_EnumValue And PriviligeDescrEnum.RsltsViewer) > 0 _
            Or (_EnumValue And PriviligeDescrEnum.RsltsViewerAny) > 0 Then
                rslt = rslt Or ServiceTypeBMskEnum.ResultsProviderSvc Or ServiceTypeBMskEnum.AuthoringSvc
            End If

            If (_EnumValue And PriviligeDescrEnum.Respondent) > 0 Then
                rslt = rslt Or ServiceTypeBMskEnum.RespProviderSvc
            End If

            If (_EnumValue And PriviligeDescrEnum.PerceptricsAdministrator) > 0 Then
                rslt = rslt Or ServiceTypeBMskEnum.AdministratorSvc
            End If


            If (_EnumValue And PriviligeDescrEnum.RPostingSvc) > 0 Then
                rslt = rslt Or ServiceTypeBMskEnum.PostResponsetoSurveySvc
            End If

        Next
        Return rslt
    End Function
    'Not used...but will for client admin and perceptricsadministrator...
    Public Shared Function GetValues(ByVal _enum As Type) As List(Of ULong)
        Dim rslt As New List(Of ULong)
        If _enum.IsEnum Then
            Dim fieldinfos = From fi In _enum.GetFields _
                         Where fi.IsLiteral _
                         Select fi
            For Each fi In fieldinfos
                Dim val = fi.GetValue(_enum)
                rslt.Add(val)
            Next


            'For Each fi As FieldInfo In _enum.GetType().GetFields(BindingFlags.Static Or BindingFlags.Public)
            '    rslt.Add([Enum].Parse(_enum.GetType(), fi.Name, False))
            'Next
        End If
        Return rslt
    End Function

    Public Shared Function GetNames(ByVal _enum As Type) As List(Of String)
        Dim rslt As New List(Of String)
        If _enum.IsEnum Then
            Dim fieldinfos = From fi In _enum.GetFields _
                         Where fi.IsLiteral _
                         Select fi
            For Each fi In fieldinfos
                Dim val = fi.Name
                rslt.Add(val)
            Next

            'For Each fi As FieldInfo In _enum.GetType().GetFields(BindingFlags.Static Or BindingFlags.Public)
            '    rslt.Add([Enum].Parse(_enum.GetType(), fi.Name, False))
            'Next
        End If
        Return rslt
    End Function

    Public Shared Function GetServiceEnumByFullName(ByVal fullName As String) As ULong
        Dim retval As ULong = 0
        Select Case (fullName)
            Case "LogInSvcNS.LogInSvc" '"Perceptrics.Business.Services.LogInSvc"
                retval = ServiceTypeBMskEnum.LoginSvc
            Case "AuthoringSvcNS.AuthoringSvc" '"Perceptrics.Business.Services.AuthoringSvc"
                retval = ServiceTypeBMskEnum.AuthoringSvc
            Case "RespProviderSvcLibr.RespProviderSvc" '"Perceptrics.Business.Services.RespProviderSvc"
                retval = ServiceTypeBMskEnum.RespProviderSvc
            Case "RsltsProviderSvcLibr.ResultsSvc"
                retval = ServiceTypeBMskEnum.ResultsProviderSvc
            Case "Perceptrics.Business.Services.PerceptricsProviderSvc"
                retval = ServiceTypeBMskEnum.PerceptricsProviderSvc
            Case "AdministratorSvcNS.AdministratorSvc" '"Perceptrics.Business.Services.AdministratorSvc"
                retval = ServiceTypeBMskEnum.AdministratorSvc
            Case "ResponsePostingSvcLibr1.PostResponsetoSurveySvc" '"Perceptrics.Business.Services.AdministratorSvc"
                retval = ServiceTypeBMskEnum.PostResponsetoSurveySvc
            Case "WCFSvcLibr1.RespDispatcherSvc" '"Perceptrics.Business.Services.AdministratorSvc"
                retval = ServiceTypeBMskEnum.RespDispatcherSvc
            Case "CrossDomainSvcNS.CrossDomainSvc" '"Perceptrics.Business.Services.LogInSvc"
                retval = ServiceTypeBMskEnum.CrossDomainSvc
            Case "ImageSvcNS.ImageSvc" '"Perceptrics.Business.Services.LogInSvc"
                retval = ServiceTypeBMskEnum.CrossDomainSvc
        End Select
        Return retval
    End Function
End Class
