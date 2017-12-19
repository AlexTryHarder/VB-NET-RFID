Imports MySql.Data.MySqlClient

Public Class Form1
    Dim myport As Array
    Dim statusX As Boolean
    Public retornoSerial, retornoLinha As String
    Dim serialBuffer, serialLinha As String
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CarregarTaxaEPortas()
        connect()
        'TabPage1.Enabled = False

    End Sub
    Private Sub SerialPort1_DataReceived(sender As Object, e As IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Try
            If SerialPort1.IsOpen Then
                serialLinha = serialLinha & SerialPort1.ReadExisting() 'imp
                If InStr(1, serialLinha, vbCr) > 0 _
                  Or InStr(1, serialLinha, vbLf) > 0 Then
                    serialBuffer = serialLinha
                    serialLinha = ""
                Else
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MsgBox("Error: " & vbCrLf & ex.Message, MsgBoxStyle.Information)
        End Try
    End Sub

    Sub connect() 'Łączeni się z Arduino
        Try
            SerialPort1.Open()
            'btnConectar.Enabled = False
            'btnDesconectar.Enabled = True
            'cboPorta.Enabled = False
            'cboTaxa.Enabled = False
            Timer1.Enabled = True
        Catch
        End Try
    End Sub

    Private Sub btnConectar_Click(sender As Object, e As EventArgs)
        connect()
    End Sub

    Private Sub btnDesconectar_Click(sender As Object, e As EventArgs)
        SerialPort1.Close()

        btnConectar.Enabled = True
        btnDesconectar.Enabled = False
        cboPorta.Enabled = True
        cboTaxa.Enabled = True
        Timer1.Enabled = False
    End Sub

    Private Sub tmrControle_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        txtTag.Text = serialBuffer
        online()
        Mysql.Connection()
        Dim status As String
        strSql = "SELECT Status FROM workers WHERE RFID='" & txtTag.Text.ToLower & "';"
        cmd.CommandText = strSql
        cmd.Connection = conn
        dadapter.SelectCommand = cmd
        datardr = cmd.ExecuteReader
        If datardr.HasRows Then
            datardr.Read()
            status = datardr("Status")
        End If
        If txtTag.Text = "" Then
            Button1.Enabled = False
        Else
            Button1.Enabled = True
        End If

        If status = "True" Then
            statusX = True
            Button1.Text = "Check Out"
        Else
            statusX = False
            Button1.Text = "Check IN"
        End If
        If txtTag.Text = "AaAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" Then
            TabPage1.Enabled = True
        Else
            ' TabPage1.Enabled = False
        End If

        conn.Close()
    End Sub


    Private Sub CarregarTaxaEPortas() 'Wykrywanie COM portu i resetowanie indexów
        Try
            myport = IO.Ports.SerialPort.GetPortNames
            For i = 0 To UBound(myport)
                cboPorta.Items.Add(myport(i))
            Next
            cboPorta.SelectedIndex = 0
            cboTaxa.SelectedIndex = 0
        Catch
        End Try


    End Sub

    Sub czystka()
        TextBox2.Clear()
        TextBox4.Clear()
        TextBox5.Text = "+212 "
        txtTag.Clear()

    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim x As String = (Mysql.Mysql("SELECT Name FROM workers WHERE RFID='" & Me.txtTag.Text & "';"))

        If statusX = False Then
            Dim clock As String = My.Computer.Clock.LocalTime.TimeOfDay.ToString
            Dim date2 As String = My.Computer.Clock.LocalTime.Date
            Mysql.EditData("True")
            Mysql.Insertdata(txtTag.Text.ToLower, clock, date2, "Check In")
            Form2.load(Color.White, "Welcome " & x, Color.Lime)


        Else
            Dim clock As String = My.Computer.Clock.LocalTime.TimeOfDay.ToString
            Dim date2 As String = My.Computer.Clock.LocalTime.Date
            Mysql.EditData("False")
            Mysql.Insertdata(txtTag.Text.ToLower, clock, date2, "Check Out")
            Form2.load(Color.White, "Bay Bay " & x, Color.Red)

        End If
        serialBuffer = ""
        led(2)
        led(5)

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Mysql.Connection()

        Try
            strSql = "SELECT * FROM workers WHERE RFID='" & txtTag.Text & "';"
            cmd.CommandText = strSql
            cmd.Connection = conn
            dadapter.SelectCommand = cmd
            datardr = cmd.ExecuteReader
            If datardr.HasRows Then
                datardr.Read()
                txtTag.Text = datardr("RFID")
                TextBox2.Text = datardr("RFID")
                TextBox4.Text = datardr("Name")
                TextBox5.Text = datardr("Phone Number")
                TextBox3.Text = datardr("Status")
            End If
        Catch ex As Exception
        End Try
        conn.Close()
    End Sub


    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Mysql.Connection()
        Using cmd As New MySqlCommand()
            Try
                strSql = "INSERT INTO workers (`Name`,`RFID`,`Phone Number`,`Status`) values (@2Name, @RFID, @Phone, @status)"
                cmd.CommandText = strSql
                cmd.Connection = conn
                dadapter.SelectCommand = cmd
                With cmd
                    .CommandType = CommandType.Text
                    .Parameters.AddWithValue("@2name", TextBox4.Text)
                    .Parameters.AddWithValue("@RFID", txtTag.Text)
                    .Parameters.AddWithValue("@Phone", TextBox5.Text)
                    .Parameters.AddWithValue("@status", TextBox3.Text)
                End With
                cmd.ExecuteNonQuery()
            Catch ex As Exception
            End Try
            conn.Close()
            czystka()
        End Using
    End Sub




    Sub online()
        ListBox1.Items.Clear()
        ListBox2.Items.Clear()
        Mysql.Connection()
        With cmd
            .Connection = conn
            .CommandText = "SELECT Name, Status FROM workers"
        End With
        Dim myReader As MySqlDataReader
        myReader = cmd.ExecuteReader()
        While (myReader.Read())
            Dim name As String = myReader.GetString("Name")
            Dim status As String = myReader.GetString("Status")
            If status = True Then
                ListBox1.Items.Add(name & "         " & status)
            Else
                ListBox2.Items.Add(name & "         " & status)
            End If
        End While
        conn.Close()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        TabPage1.Enabled = False
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If TextBox1.Text = "admin" AndAlso TextBox6.Text = "alex9819" Then
            TabPage1.Enabled = True
            TextBox1.Clear()
            TextBox6.Clear()
        Else
            MsgBox("Wrong Password!")
        End If
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        '1 - włącz zielone '
        '2 - włącz czerwone
        '3 - wyłacz czerwone
        '0 - wyłacz zielone'
        '4 -buzzer error
        '5 - buzzer OK
        '6 - czyszczenie serialportu
        '7 - buzzer OK SHORT
        led(1)
        led(3)

        led(2)

        led(0)


        led(4)
        led(5)


        led(7)







    End Sub

    Private Sub txtTag_TextChanged_1(sender As Object, e As EventArgs) Handles txtTag.TextChanged
        If Not txtTag.Text = "" Then
            led(7)
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Mysql.Connection()
        Try
            strSql = "DELETE FROM workers WHERE RFID=@id"
            cmd.CommandText = strSql
            cmd.Connection = conn
            dadapter.SelectCommand = cmd
            With cmd
                .Parameters.AddWithValue("@id", txtTag.Text)
                .ExecuteNonQuery()
            End With
        Catch ex As Exception
        End Try
        conn.Close()
        czystka()
    End Sub

    Function led(ByVal x As Integer)
        '1 - włącz zielone '
        '2 - włącz czerwone
        '3 - wyłacz czerwone
        '0 - wyłacz zielone'
        '4 -buzzer error
        '5 - buzzer OK
        '6 - czyszczenie serialportu
        '7 - buzzer OK SHORT
        If x = 1 Then
            SerialPort1.Write("1")
            Threading.Thread.Sleep(800)
            SerialPort1.Write("0")
        ElseIf x = 2 Then
            SerialPort1.Write("2")
            Threading.Thread.Sleep(800)
            SerialPort1.Write("3")
        ElseIf x = 4 Then
            SerialPort1.Write("4")
        ElseIf x = 5 Then
            SerialPort1.Write("5")
        ElseIf x = 6 Then
            SerialPort1.Write("6")
            txtTag.Clear()
        ElseIf x = 7 Then
            SerialPort1.Write("7")
        End If
        Return Nothing
    End Function

End Class
