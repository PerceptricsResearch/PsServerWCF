Imports System.Configuration

Partial Public Class SvcElementUC
    Inherits UserControl

    Const baseaddressForMySvc = "baseaddressForMySvc"
    Private Sub mylayoutupdated() Handles Me.LayoutUpdated

        If Me.Parent IsNot Nothing AndAlso Me.Parent.GetType.Equals(GetType(Window)) Then
            CType(Me.Parent, Window).Title = Me.SvcName_Tbx.Text & " in " & CType(Me.Parent, Window).Owner.Title
        End If
    End Sub

    Private Sub LaunchSvcofT_Btn_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

        Dim txm = [GetType].Assembly.FullName
        Dim typetext = Me.SvcName_Tbx.Text

        Dim typetextfirstpart = Strings.Split(typetext, ".").First
        ' Dim trygettypetext = Me.SvcName_Tbx.Text & ", " & typetextfirstpart
        Dim refasm = Me.GetType.Assembly.GetReferencedAssemblies
        Dim asm = From ras In Me.GetType.Assembly.GetReferencedAssemblies _
                  Where ras.Name = typetextfirstpart _
                  Select ras

        Dim asmname As String = ""
        If asm.Any Then
            asmname = asm.FirstOrDefault.FullName
        End If
        Dim trygettypetext2 = Me.SvcName_Tbx.Text & ", " & asmname
        Dim svctype2 = Type.GetType(trygettypetext2, False, True)
        'Dim svctype = Type.GetType(Me.SvcName_Tbx.Text, False, True) 'this won't work
        'UNBELIEVABLE....
        'Dim svctype3 = Type.GetType(trygettypetext, False, True) 'this will work

        'Dim baseaddr = Me.BaseAddresses_Lbx.SelectedItem ' or getitfrom appconfig settings...

        Dim db As New DataClasses1DataContext
        Dim q = From wcf In db.WCFServiceInfos _
                Where wcf.Name = SvcName_Tbx.Text _
                Select wcf.BaseAddress

        Dim newbaseaddr = q.FirstOrDefault

        Dim svconfig = Me.DataContext 'has the svcconfig init...should be able to inspect contracts for each endpoint... in it...


        Dim SvcWindow As New CmdSvcClassLibrary.WPFSvcContainerWindow(svctype2, _
                                                                      newbaseaddr)
        'baseaddr.BaseAddress)
        If SvcWindow IsNot Nothing Then
            If svctype2.Equals(GetType(CustomerDBSvc.CustomerDBSvc)) Then
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.OrangeRed)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.OrangeRed)

                'ElseIf svctype2.Equals(GetType(EndPtDataCntxtSvcNS.EndPtDataCntxtSvc)) Then
                '    SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.Crimson)
                '    SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.Crimson)
            ElseIf svctype2.Equals(GetType(CrossDomainSvcNS.CrossDomainSvc)) Then
                Dim blackbrush As New SolidColorBrush(Colors.Orange)
                blackbrush.Opacity = 0.2
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.Orange)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.Orange)

            ElseIf svctype2.Equals(GetType(ImageSvcNS.ImageSvc)) Then
                Dim blackbrush As New SolidColorBrush(Colors.Teal)
                blackbrush.Opacity = 0.2
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.Teal)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.Teal)

            ElseIf svctype2.Equals(GetType(LogInSvcNS.LogInSvc)) Then
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.Turquoise)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.Turquoise)

            ElseIf svctype2.Equals(GetType(AdministratorSvcNS.AdministratorSvc)) Then
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.DeepSkyBlue)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.DeepSkyBlue)

            ElseIf svctype2.Equals(GetType(AuthoringSvcNS.AuthoringSvc)) Then
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.DarkOrchid)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.DarkOrchid)

            ElseIf svctype2.Equals(GetType(PageItemColxnServicesLibr.PgItemColxnSvc)) Then
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.SpringGreen)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.SpringGreen)

            ElseIf svctype2.Equals(GetType(RespProviderSvcLibr.RespProviderSvc)) Then
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.LightSkyBlue)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.LightSkyBlue)

            ElseIf svctype2.Equals(GetType(RsltsProviderSvcLibr.ResultsSvc)) Then
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.Maroon)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.Maroon)

            ElseIf svctype2.Equals(GetType(MasterDBSvcLibr.MasterSurveyDBSvc)) Then
                Dim blackbrush As New SolidColorBrush(Colors.Black)
                blackbrush.Opacity = 0.2
                SvcWindow.ColorSchemeBorder = blackbrush
                SvcWindow.ColorSchemeSplitters = blackbrush
            ElseIf svctype2.Equals(GetType(WCFSvcLibr1.RespDispatcherSvc)) Then
                Dim purplebrush As New SolidColorBrush(Colors.DarkViolet)
                purplebrush.Opacity = 0.3
                SvcWindow.ColorSchemeBorder = purplebrush
                SvcWindow.ColorSchemeSplitters = purplebrush

            ElseIf svctype2.Equals(GetType(ResponsePostingSvcLibr1.PostResponsetoSurveySvc)) Then
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.Purple)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.Purple)
            Else
                SvcWindow.ColorSchemeBorder = New SolidColorBrush(Colors.RoyalBlue)
                SvcWindow.ColorSchemeSplitters = New SolidColorBrush(Colors.RoyalBlue)

            End If

            Me.SvCElementUC_PlaceHolder_Grid.Children.Clear()
            Me.SvCElementUC_PlaceHolder_Grid.Children.Add(SvcWindow)

            Me.CmdWindowPlaceholder_Grid.Children.Clear()
            'Me.CmdWindowPlaceholder_Grid.Children.Add(SvcWindow.CmdWindow)
        Else
            MessageBox.Show("Could not launch this service..." & Me.SvcName_Tbx.Text)
        End If


    End Sub

    Private Sub Button_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Height += 100
        Me.InvalidateVisual()
        Me.UpdateLayout()
    End Sub
End Class
