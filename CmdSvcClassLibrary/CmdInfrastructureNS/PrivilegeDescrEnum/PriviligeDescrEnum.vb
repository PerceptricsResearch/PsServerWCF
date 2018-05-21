<Flags()> _
Public Enum PriviligeDescrEnum As ULong
    None = 0
    AuthorRead = 1
    AuthorWrite = 2
    RsltsViewer = 4
    Respondent = 8
    RDispatcherSvc = 16
    RPostingSvc = 32
    PublishSurvey = 64
    LoginAdministrator = 128
    ReadAny = 256
    WriteAny = 512
    RsltsViewerAny = 1024
    Create = 2048
    DeleteAny = 4096
    PerceptricsAdministrator = 8192

End Enum
<Flags()> _
Public Enum ServiceTypeBMskEnum As ULong
    None = 0
    LoginSvc = 1
    AuthoringSvc = 2
    RespProviderSvc = 4
    ResultsProviderSvc = 8
    PerceptricsProviderSvc = 16
    AdministratorSvc = 64
    RespDispatcherSvc = 128
    PostResponsetoSurveySvc = 256
    CrossDomainSvc = 512
    ImageSvc = 1024
End Enum