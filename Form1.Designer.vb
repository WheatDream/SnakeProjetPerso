<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        timerMvt = New Timer(components)
        lblTime = New Label()
        lblNbrSnake = New Label()
        lblPlayer2 = New Label()
        SuspendLayout()
        ' 
        ' timerMvt
        ' 
        ' 
        ' lblTime
        ' 
        lblTime.AutoSize = True
        lblTime.Location = New Point(1442, 279)
        lblTime.Name = "lblTime"
        lblTime.Size = New Size(0, 20)
        lblTime.TabIndex = 2
        ' 
        ' lblNbrSnake
        ' 
        lblNbrSnake.AutoSize = True
        lblNbrSnake.Location = New Point(1442, 175)
        lblNbrSnake.Name = "lblNbrSnake"
        lblNbrSnake.Size = New Size(0, 20)
        lblNbrSnake.TabIndex = 3
        ' 
        ' lblPlayer2
        ' 
        lblPlayer2.AutoSize = True
        lblPlayer2.Location = New Point(1340, 272)
        lblPlayer2.Name = "lblPlayer2"
        lblPlayer2.Size = New Size(0, 20)
        lblPlayer2.TabIndex = 4
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.DarkGray
        ClientSize = New Size(1482, 1053)
        Controls.Add(lblPlayer2)
        Controls.Add(lblNbrSnake)
        Controls.Add(lblTime)
        Name = "Form1"
        Text = "Form1"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents timerMvt As Timer
    Friend WithEvents lblTime As Label
    Friend WithEvents lblNbrSnake As Label
    Friend WithEvents lblPlayer2 As Label

End Class
