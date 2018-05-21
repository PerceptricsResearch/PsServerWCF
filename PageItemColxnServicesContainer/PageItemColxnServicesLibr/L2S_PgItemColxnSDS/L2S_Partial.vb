Partial Public Class L2S_PgItemColxnSDSDataContext
    Inherits System.Data.Linq.DataContext

    Public Sub New()
        MyBase.New("Data Source=.\SQLEXPRESS;AttachDbFilename=""C:\Program Files\Microsoft SQL Server\" & _
    "MSSQL.2\MSSQL\Data\Survey4DataStore.mdf"";Integrated Security=True;Connect Timeou" & _
    "t=30;User Instance=True", mappingSource)
        OnCreated()

    End Sub
End Class
