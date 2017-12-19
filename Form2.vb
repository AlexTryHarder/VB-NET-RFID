Public Class Form2
    Sub load(ByVal Fcolor As Color, text As String, Bcolor As Color)
        Me.Show()
        Label1.Size = New Size(Me.Width / 3, Me.Height / 3)
        Label1.Font = New Font("Arial", Me.Height / 20)
        Dim p As New Point
        p.X = (Me.Width / 2) - (Label1.Width / 2)
        p.Y = (Me.Height / 2) - (Label1.Height / 2)
        Label1.Location = p
        Label1.Text = text
        Label1.ForeColor = Fcolor
        Me.BackColor = Bcolor





        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        Me.Close()
    End Sub

End Class