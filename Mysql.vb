Imports MySql.Data.MySqlClient
Module Mysql
    Public conn As New MySqlConnection
    Public cmd As New MySqlCommand
    Public dadapter As New MySqlDataAdapter
    Public datardr As MySqlDataReader
    Public strSql As String
    Public Sub Connection()
        conn.ConnectionString = "server=127.0.0.1;user id=admin;password=admin;database=test"
        conn.Open()
    End Sub



    Public Function Insertdata(ByVal TagId As String, Time As String, date2 As String, check As String) As Boolean
        Connection()
        Using cmd As New MySqlCommand()
            Try
                strSql = "INSERT INTO " & TagId & " (`RFID`,`Time`,`Date`,`Check In/Out`) values (@TagID, @Time, @Date, @Check)"
                cmd.CommandText = strSql
                cmd.Connection = conn
                dadapter.SelectCommand = cmd
                With cmd
                    .CommandType = CommandType.Text
                    .Parameters.AddWithValue("@TagID", TagId)
                    .Parameters.AddWithValue("@Time", Time)
                    .Parameters.AddWithValue("@Date", date2)
                    .Parameters.AddWithValue("@Check", check)
                End With
                cmd.ExecuteNonQuery()
                conn.Close()
            Catch ex As Exception
            End Try
        End Using

        Return Nothing
    End Function

    Public Function EditData(status As String) As Boolean

        Dim iReturn As Boolean
        Using SQLConnection As New MySqlConnection("server=127.0.0.1;user id=admin;password=admin;database=test")
            Using sqlCommand As New MySqlCommand()
                With sqlCommand
                    .CommandText = "UPDATE workers SET status = '" & status & "' WHERE RFID='" & Form1.txtTag.Text & "' ;"
                    .Connection = SQLConnection
                    .CommandType = CommandType.Text

                End With

                SQLConnection.Open()
                sqlCommand.ExecuteNonQuery()
                iReturn = True

            End Using
        End Using

        Return iReturn
    End Function

    'Public Function Online() As Boolean
    '    Dim iReturn As Boolean
    '    Using SQLConnection As New MySqlConnection("server=127.0.0.1;user id=admin;password=admin;database=test")
    '        Using sqlCommand As New MySqlCommand()
    '            With sqlCommand
    '                .CommandText = "UPDATE workers SET status = '  ' WHERE RFID='" & Form1.txtTag.Text & "' ;"
    '                .Connection = SQLConnection
    '                .CommandType = CommandType.Text

    '            End With

    '            SQLConnection.Open()
    '            sqlCommand.ExecuteNonQuery()
    '            iReturn = True

    '        End Using
    '    End Using
    '    Return iReturn
    'End Function

    Public Function Mysql(command As String) As String
        Call Connection()
        Dim x As String
        Try

            strSql = command
            cmd.CommandText = strSql
            cmd.Connection = conn
            dadapter.SelectCommand = cmd

            datardr = cmd.ExecuteReader
            If datardr.HasRows Then
                datardr.Read()
                x = datardr("Name")
            End If

        Catch ex As Exception

        End Try
        conn.Close()
        Return x
    End Function
End Module
