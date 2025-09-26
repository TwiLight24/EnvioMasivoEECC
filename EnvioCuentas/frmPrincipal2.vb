Imports System.IO
Imports System.Net.Mail
Imports System.Net
Imports System.Net.Mime
Imports System.Text
Imports System.Threading.Tasks
Imports System.Text.RegularExpressions
Imports OpenQA.Selenium
Imports System.Web

Public Class frmPrincipal2
    Dim DatosCuentas As DataView
    Dim t As Integer
    'Dim leer As New StreamReader("C:\EnviarEstadoCuenta\cod.ini", False)
    Private driver As IWebDriver

    Private Sub frmPrincipal2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Label4.Text = "C000001" 'leer.ReadLine() '"C000001"

        'leer.Close()
        ComboBox1.SelectedIndex = 0

        CargarGrilla()
        Generar_CorreoPDF()
        EnviarEECC_Correo2()
        EnviarMsgWsp()
        Application.Exit()
    End Sub

    Sub CargarGrilla()

        DataGridView1.DataSource = Variables.damestockpmega1(Label4.Text)

        alineargrilla()
        anchocolumnas()
        'cambiando de fuente
        DataGridView1.DefaultCellStyle.Font = New Font("Arial", 7)
        'interlineado de grilla
        DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke
        Label3.Text = "Total de Registros .: " & DataGridView1.Rows.Count.ToString()

    End Sub
#Region "AGRUPAR"
    Sub alineargrilla()

        'alinear encabezado 1ER DATAGRID
        Try
            Dim h, g As Integer
            For h = 0 To 7
                Dim col As DataGridViewColumn = Me.DataGridView1.Columns(h)

                ' Asignamos la alineación del encabezado de la columna 
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

                For g = 0 To 7
                    Dim colum As DataGridViewColumn = Me.DataGridView1.Columns(g)
                    colum.HeaderCell.Style.Font = New Font("Arial", 9, FontStyle.Bold)
                    col.HeaderCell.Value = UCase(col.HeaderCell.Value)
                Next
            Next
        Catch
        End Try
    End Sub

    Sub anchocolumnas()
        'cambiando ancho de columnas 1ER DATAGRIDVIEW
        For Each column As DataGridViewColumn _
       In DataGridView1.Columns
            If column.Index = 0 Then column.Width = 65
            If column.Index = 1 Then column.Width = 150
            If column.Index = 2 Then column.Width = 100
            If column.Index = 3 Then column.Width = 125
            If column.Index = 4 Then column.Width = 70
            If column.Index = 5 Then column.Width = 0
            If column.Index = 6 Then column.Width = 100
            If column.Index = 6 Then column.Width = 100
            'If Me.DataGridView1.Columns(2).Name = "Saldo" Then e.CellStyle.Format = "#,#.0 €"

        Next
    End Sub
#End Region

    Private Sub Generar_CorreoPDF()
        For Contador = 0 To DataGridView1.RowCount - 2
            Dim rd As New CrystalDecisions.CrystalReports.Engine.ReportDocument
            rd.Load("\\JUPITER\Aplicativos\accesos_aplicativos_finanzas\EnviarEECC\CUENTA2.rpt")

            rd.SetDataSource(DameReporte(DataGridView1(0, Contador).Value.ToString).Tables(0))


            Dim oStrem As New System.IO.MemoryStream

            Dim exportStream As System.IO.Stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            exportStream.CopyTo(oStrem)

            Dim ArchivoXLS As New System.IO.FileStream("\\JUPITER\Aplicativos\accesos_aplicativos_finanzas\EnviarEECC\reportecobranza_" & DataGridView1(0, Contador).Value.ToString & ".pdf", IO.FileMode.Create)
            ArchivoXLS.Write(oStrem.ToArray, 0, oStrem.ToArray.Length)
            ArchivoXLS.Flush()
            ArchivoXLS.Close()
            rd.Close()
            rd.Dispose()
        Next
    End Sub

    Private Sub EnviarEECC_Correo2()

        Dim Contador As Integer
        For Contador = 0 To DataGridView1.RowCount - 2
            Try
                System.Threading.Thread.Sleep(3000)
                Label7.Text = DataGridView1(0, Contador).Value.ToString

                Dim miCorreo As New System.Net.Mail.MailMessage
                miCorreo.IsBodyHtml = False
                miCorreo.From = New System.Net.Mail.MailAddress("creditosycobranzas@gmail.com.pe") 'mail desde donde se envía
                miCorreo.Bcc.Add("squispe@gmail.com.pe")
                miCorreo.To.Add("aherrera@gmail.com.pe")
                miCorreo.To.Add("jflores@gmail.com.pe")
                miCorreo.To.Add("lchavez@gmail.com.pe")

                miCorreo.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess
                miCorreo.Headers.Add("Enviar_Respuesta", "creditosycobranzas@gmail.com.pe")

                If DataGridView1(7, Contador).Value Is DBNull.Value Then
                Else
                    miCorreo.To.Add(DataGridView1(7, Contador).Value.ToString) 'Mail del vendedor
                End If


                If DataGridView1(3, Contador).Value Is DBNull.Value Then
                Else
                    miCorreo.To.Add(DataGridView1(3, Contador).Value.ToString) 'Mail del cliente
                End If

                miCorreo.Subject = "Estado de Cuenta RUMI IMPORT S.A. " & DataGridView1(1, Contador).Value.ToString


                Dim html As String = "<p>Estimado cliente: " & DataGridView1(1, Contador).Value.ToString & "</p>" &
               "</br><p>CREDITOS Y COBRANZAS: Adjunta estado de cuenta RUMI </p>" &'<a href='" & DataGridView1(12, Contador).Value & "' target=_blank>Presione aquí, para ver su estado de cuenta.</a>
               "</br><p>Coméntenos sobre nuestra atención:</p>" &
               "<a href='https://docs.google.com/forms/d/e/1FAIpQLSd1NO3PbL8ugO-85l3ks52TannzVzEtKm6kgVQo8JgYDKyyKg/viewform' target=_blank><img src='cid:imagen'/></a></br><p>Ante cualquier duda y/o consulta, por favor comunicarse con los siguientes números: +51 948 487 304  -  +51 963 913 820</p>"

                Dim htmlView As AlternateView = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html)
                'Colocar hubicacion en la cual se desea poner la imagen-------------------------------------------------------
                Dim img As LinkedResource = New LinkedResource("\\JUPITER\Aplicativos\accesos_aplicativos_finanzas\EnviarEECC\Encuesta.png", MediaTypeNames.Image.Jpeg)
                img.ContentId = "imagen"

                htmlView.LinkedResources.Add(img)
                miCorreo.AlternateViews.Add(htmlView)

                Dim archivoAdjunto As New Attachment("\\JUPITER\Aplicativos\accesos_aplicativos_finanzas\EnviarEECC\reportecobranza_" & DataGridView1(0, Contador).Value.ToString & ".pdf") ' Cambia "ruta_de_la_imagen.jpg" por la ruta de tu imagen.
                archivoAdjunto.Name = "reportecobranza_" & DataGridView1(0, Contador).Value.ToString & ".pdf"
                miCorreo.Attachments.Add(archivoAdjunto)

                Dim smtp As New SmtpClient
                smtp.Host = "mail.gmail.com.pe"
                smtp.EnableSsl = False
                smtp.Credentials = New System.Net.NetworkCredential("creditosycobranzas@gmail.com.pe", "#####")
                smtp.Send(miCorreo)

                miCorreo.Dispose()
                smtp.Dispose()


            Catch ex As Exception

            End Try

            ProgressBar1.Minimum = 0
            ProgressBar1.Maximum = CInt(Label3.Text.Replace("Total de Registros .: ", "").Trim().ToString())
            ProgressBar1.Value = Contador

            Label6.Text = Contador.ToString
        Next


    End Sub

    Sub GenerarArchivo(codigocli As String)

        Dim fs As FileStream
        Dim ruta As String = "C:\EnviarEstadoCuenta\"
        Dim archivo As String = "cod.ini"
        Directory.CreateDirectory(ruta)
        fs = File.Create(ruta + archivo)
        fs.Close()

        Dim escribir As New StreamWriter(ruta + archivo)
        Try
            escribir.WriteLine(codigocli)
            escribir.Close()
        Catch ex As Exception
            MsgBox("Se presento un problema al escribir en el archivo: " & ex.Message, MsgBoxStyle.Critical, ":::Aprendamos de Programación:::")
        End Try


    End Sub

    Function NumeroValido(phone1 As String, phone2 As String, phone3 As String) As String

        If phone1.Length = 9 And phone1.StartsWith("9") Then
            Return phone1
        ElseIf phone2.Length = 9 And phone2.StartsWith("9") Then
            Return phone2
        ElseIf phone3.Length = 9 And phone3.StartsWith("9") Then
            Return phone3
        End If

        Return "0"
    End Function


    Private Sub EnviarMsgWsp()

        Try
            For contador = 0 To DataGridView1.RowCount - 2

                Dim phone1 As String
                Dim phone2 As String
                Dim phone3 As String

                If DataGridView1(9, contador).Value Is DBNull.Value Then
                    phone1 = "0"
                Else
                    phone1 = Regex.Replace(DataGridView1(9, contador).Value.ToString, "[^\d]", "")
                End If

                If DataGridView1(10, contador).Value Is DBNull.Value Then

                    phone2 = "0"
                Else
                    phone2 = Regex.Replace(DataGridView1(10, contador).Value.ToString, "[^\d]", "")
                End If

                If DataGridView1(11, contador).Value Is DBNull.Value Then
                    phone3 = "0"
                Else
                    phone3 = Regex.Replace(DataGridView1(11, contador).Value.ToString, "[^\d]", "")
                End If

                Dim numero As String = NumeroValido(phone1, phone2, phone3)

                If numero <> "0" Then
                    'numero = "969834193"

                    Task.Run(Sub() EnviarAPI(numero, DataGridView1(0, contador).Value.ToString(), DataGridView1(1, contador).Value.ToString(), DataGridView1(12, contador).Value.ToString())).Wait()
                    System.Threading.Thread.Sleep(15000)
                End If

                'Application.Exit()
            Next
        Catch ex As Exception
            GenerarArchivo("")
            Application.Exit()
        End Try

    End Sub

    Public Shared Async Function EnviarAPI(ByVal numero As String, ByVal codigoCli As String, ByVal nombre As String, ByVal link As String) As Task

        Try
            Dim fecha As String = Now.ToString("dd-MM-yyyy")
            nombre = nombre.Replace("&", "Y")
            Dim WebRequest As HttpWebRequest
            Dim bytes As Byte() = IO.File.ReadAllBytes("\\JUPITER\Aplicativos\accesos_aplicativos_finanzas\EnviarEECC\reportecobranza_" + codigoCli + ".pdf")
            Dim file As String = Convert.ToBase64String(bytes)
            WebRequest = CType(HttpWebRequest.Create("https://api.ultramsg.com/####/messages/document"), HttpWebRequest)
            Dim postdata As String = "token=#####&to=" & numero & "&document=" & HttpUtility.UrlEncode(file) & "&filename=" & codigoCli & ".pdf&caption=Hola estimado/a Cliente " & nombre & ". 🌟! CREDITOS Y COBRANZAS - RUMI IMPORT S.A. 🚀 te compartimos el Estado de Cuenta actualizado 📊 " & fecha & ". Si tienes alguna duda o consulta, no dudes en comunicarte con nosotros a los siguientes números ☎️: 000 000 000 📞 - 000 000 000 📱. ¡Estamos aquí para ayudarte y escucharte, que tenga un buen día!"
            Dim enc As UTF8Encoding = New System.Text.UTF8Encoding()
            Dim postdatabytes As Byte() = enc.GetBytes(postdata)
            WebRequest.Method = "POST"
            WebRequest.ContentType = "application/x-www-form-urlencoded"
            WebRequest.GetRequestStream().Write(postdatabytes, 0, postdatabytes.Length)

            Dim ret As New System.IO.StreamReader(WebRequest.GetResponse().GetResponseStream())
            Console.WriteLine(ret.ReadToEnd())

        Catch ex As Exception
            'MessageBox.Show(ex.Message)
            Application.Exit()
        End Try
    End Function

End Class