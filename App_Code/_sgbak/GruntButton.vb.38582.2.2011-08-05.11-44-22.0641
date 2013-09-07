Imports Microsoft.VisualBasic

Public Class GruntButton
    Inherits ImageButton

    Private _isGrunt As Boolean
    Private _isLeftOver As Boolean
    Private _hitCounter As Integer
    Private _text As String

    Public Sub New()

        Me.BackColor = Drawing.ColorTranslator.FromHtml("#595959")
        Me.BorderWidth = 2
        Me.BorderStyle = BorderStyle.Solid
        Me.BorderColor = Drawing.ColorTranslator.FromHtml("#8d8d8d")
        Me.ForeColor = Drawing.ColorTranslator.FromHtml("#ffd52b")
        Me.ImageAlign = WebControls.ImageAlign.Middle
        Me.ImageUrl = "~/notGrunt.gif"

    End Sub

    Public Property IsGrunt() As Boolean
        Get
            Return _isGrunt
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                Me.BackColor = Drawing.ColorTranslator.FromHtml("#2c9aa1")
                Me.BorderWidth = 2
                Me.BorderStyle = BorderStyle.Solid
                Me.BorderColor = Drawing.ColorTranslator.FromHtml("#3dd6e0")
                Me.ForeColor = Drawing.ColorTranslator.FromHtml("#3dd6e0")
                Me.Style.Add("font-weight", "bold")
                Me.ImageUrl = "~/gruntSleeping.gif"
            Else
                Me.BackColor = Drawing.ColorTranslator.FromHtml("#595959")
                Me.BorderWidth = 2
                Me.BorderStyle = BorderStyle.Solid
                Me.BorderColor = Drawing.ColorTranslator.FromHtml("#8d8d8d")
                Me.ForeColor = Drawing.ColorTranslator.FromHtml("#ffd52b")
                Me.ImageUrl = "~/notGrunt.gif"
            End If
            _isGrunt = value
        End Set
    End Property

    Public Property IsLeftOverGrunt() As Boolean
        Get
            Return _isLeftOver
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                Me.BackColor = Drawing.ColorTranslator.FromHtml("#a12c2c")
                Me.BorderWidth = 2
                Me.BorderStyle = BorderStyle.Solid
                Me.BorderColor = Drawing.ColorTranslator.FromHtml("#ee4141")
                Me.ForeColor = Drawing.ColorTranslator.FromHtml("#ee4141")
                Me.ImageUrl = "~/gruntAwake.gif"
            End If
            _isLeftOver = value
        End Set
    End Property

    Public Property HitCounter() As Integer
        Get
            Return _hitCounter
        End Get
        Set(ByVal value As Integer)
            Select Case value
                Case 1
                    Me.BackColor = Drawing.ColorTranslator.FromHtml("#893b3b")
                    Me.BorderWidth = 2
                    Me.BorderStyle = BorderStyle.Solid
                    Me.BorderColor = Drawing.ColorTranslator.FromHtml("#ce5a5a")
                    Me.ForeColor = Drawing.ColorTranslator.FromHtml("#ce5a5a")
                    Me.ImageUrl = "~/gruntHit1.gif"
                Case 2
                    Me.BackColor = Drawing.ColorTranslator.FromHtml("#714a4a")
                    Me.BorderWidth = 2
                    Me.BorderStyle = BorderStyle.Solid
                    Me.BorderColor = Drawing.ColorTranslator.FromHtml("#ad7474")
                    Me.ForeColor = Drawing.ColorTranslator.FromHtml("#ad7474")
                    Me.ImageUrl = "~/gruntHit2.gif"
            End Select
            _hitCounter = value
        End Set
    End Property

End Class
