Imports System.Numerics
Imports System.Reflection
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Logging

Public Class Form1
    Dim GridSize As Integer                             'Nombre de carreaux
    Dim CellSize As Integer                             'Taille de chaque carreau
    Private PointPosition As Point = New Point(0, 0)    'Position initiale du serpent
    Private PointPosition2 As Point = New Point(GridSize - 1, GridSize - 1)   'Le -1 est important sinon le serpent commence hors de la limite ???
    Private PointFruit As Point                         'definition d'un point
    Dim random As New Random                            'definition d'un nombre aléatoire
    Dim Direction As Integer = 1                        'direction (haut, bas, gauche, droite) de la tête du serpent
    Dim Direction2 As Integer = 3                       'Direction serpent 2
    Dim LenghtSnake As Integer = 1                      'longeur du serpent
    Dim LenghtSnake2 As Integer = 1                     'Longeur serpent 2
    Dim ListPos As New List(Of Point)                   'Liste des positions de chaque partie du serpent
    Dim ListPos2 As New List(Of Point)                  'position des parties du serpent 2
    Dim ListFruit As New List(Of Point)                 'Liste de chaque fruit
    Dim NbrPartie As Integer = 0                        'Nombre de parties qui composent le serpent
    Dim CodeError As Integer = 0                        'Type d'erreur/victoire en cas de fin de partie
    Dim Speed As String                                 'Vitesse du serpent *boite de dialogue*
    Dim SizeOf As String                                'Taille de la zone de jeu *boite de dialogue*
    Dim TimerTime As Integer                            'Juste tenir compte du nombre de pas du jeu *Non utilisé*
    Dim NbrFruits As Integer                            'defini le nombre de fruits simultanés dans la partie
    Dim NbrFruitsSTR As String = ""                     'Nombre de fruits *boite de dialogue*
    Dim GrilleBool As Boolean = True                    'Afficher la grille du jeu ou non
    Dim ColorChoice As Boolean = True                   'Choisir si le serpent est vert ou de couleur aléatoire
    Dim Player2 As Boolean = False                      'false si un joueur, true si 2 joueur, giga important
    Dim FirstGame As Boolean = True                     'Permet de garder en mémoire les paramètres de la partie précedente
    Dim GameMode As Boolean = False
    Private PointObst As Point
    Dim ListPosObs As New List(Of Point)
    Dim NbrObst As Integer
    Dim boost As Boolean = False
    Dim SpeedBoost As Integer
    Dim NormalSpeed As Integer
    Dim ModeInf As Boolean = True
    Dim NbrEat As Integer = 0

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Defini le form, demarre la boucle de depart et met en plein écran
        Me.Text = "Snake"
        Me.DoubleBuffered = True ' Pour éviter le scintillement lors du dessin
        StartAgain()
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        Me.WindowState = FormWindowState.Maximized
    End Sub

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        'dessine à chaque changement de position le serpent et les fruits
        Dim g As Graphics = e.Graphics

        ' Dessiner la grille ou juste les bords externes
        If GrilleBool = True Then
            For i As Integer = 0 To GridSize - 1
                For j As Integer = 0 To GridSize - 1
                    g.DrawRectangle(Pens.Black, i * CellSize, j * CellSize, CellSize, CellSize)
                Next
            Next
        Else
            g.DrawRectangle(Pens.Black, 0, 0, GridSize * CellSize, GridSize * CellSize)
        End If

        'Dessiner les fruits
        For Each fruit As Point In ListFruit
            Dim fruitRect As New Rectangle(fruit.X * CellSize, fruit.Y * CellSize, CellSize, CellSize)
            g.FillEllipse(Brushes.Red, fruitRect)
        Next

        'Dessiner le serpent et choisir sa couleur
        For Each segment As Point In ListPos.Skip(1)
            Dim segmentRect As New Rectangle(segment.X * CellSize, segment.Y * CellSize, CellSize, CellSize)
            If ColorChoice = True Then
                g.FillEllipse(Brushes.DarkOliveGreen, segmentRect)
            Else
                g.FillEllipse(RandomColor(), segmentRect)
            End If
        Next

        'Definir la couleur de la tête du serpent
        Dim segmentRectTete As New Rectangle(ListPos.First.X * CellSize, ListPos.First.Y * CellSize, CellSize, CellSize)
        g.FillEllipse(Brushes.DarkGreen, segmentRectTete)

        'dessiner le 2eme serpent
        If Player2 = True Then
            For Each segment2 As Point In ListPos2.Skip(1)
                Dim segmentRect2 As New Rectangle(segment2.X * CellSize, segment2.Y * CellSize, CellSize, CellSize)
                If ColorChoice = True Then
                    g.FillEllipse(Brushes.Coral, segmentRect2)
                Else
                    g.FillEllipse(RandomColor(), segmentRect2)
                End If
            Next
            'Definir la couleur de la tête du serpent 2
            Dim segmentRectTete2 As New Rectangle(ListPos2.First.X * CellSize, ListPos2.First.Y * CellSize, CellSize, CellSize)
            g.FillEllipse(Brushes.DarkRed, segmentRectTete2)
        End If

        'dessiner les obstacles
        If GameMode = True Then
            For Each elem As Point In ListPosObs
                Dim segmentRect3 As New Rectangle(elem.X * CellSize, elem.Y * CellSize, CellSize, CellSize)
                g.FillRectangle(Brushes.Black, segmentRect3)
            Next
        End If
    End Sub

    Public Function RandomColor()
        'Fonction qui retourne un pinceau avec une couleur aléatoire foncée
        Dim rdmColor As Color = Color.FromArgb(255, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255))
        Dim rdmBrush As New SolidBrush(rdmColor)
        Return rdmBrush
    End Function

    Public Function FruitPos()
        'place le bon nombre de fruits sur la grille et met leurs positions dans une liste
        While ListFruit.Count < NbrFruits
            Dim newFruit As New Point(random.Next(0, GridSize), random.Next(0, GridSize))
            If (Not ListPos.Contains(newFruit) And Not ListPos2.Contains(newFruit) And Not ListPosObs.Contains(newFruit)) AndAlso Not ListFruit.Contains(newFruit) Then
                ListFruit.Add(newFruit)
            End If
        End While
    End Function

    Public Function ObstPos()
        While ListPosObs.Count < NbrObst
            Dim newObst As New Point(random.Next(0, GridSize), random.Next(1, GridSize - 1))
            If newObst <> PointPosition And newObst <> PointPosition2 Then
                ListPosObs.Add(newObst)
            End If
        End While
    End Function

    Public Function CheckFruitInSnake()
        'Vérifie que le fruit ne se trouve ni sur le serpent ni sur un autre fruit, sinon repositionne les fruits
        'Bizzarement fonctionne que 1x sur 2
        For Each elem As Point In ListPos
            For Each elem2 As Point In ListFruit
                If elem = elem2 Then
                    If ModeInf = True Then
                        Return FruitPos()
                    End If
                End If
            Next
        Next

        If Player2 = True Then
            For Each elem As Point In ListPos2
                For Each elem2 As Point In ListFruit
                    If elem = elem2 Then
                        If ModeInf = True Then
                            Return FruitPos()
                        End If
                    End If
                Next
            Next
        End If
    End Function

    Public Function CheckSnakeInObst()
        Dim tempList = ListPosObs.ToList()
        For Each elem As Point In tempList
            If elem = PointPosition Then
                CodeError = 8
                EndOfGame()
            End If
        Next

        If Player2 = True Then
            For Each elem As Point In tempList
                If elem = PointPosition Then
                    CodeError = 8
                    EndOfGame()
                End If
            Next
        End If
    End Function

    Public Sub CheckEaten()
        'vérifie que la tête du serpent arrive sur une case avec un fruit et si oui augmente le serpent
        'cas possible ou un fruit est generé sur la tete du serpent et il ne detecte pas que ça compte comme un cheackeaten à cause de l'ordre des opérations
        For i As Integer = ListFruit.Count - 1 To 0 Step -1
            If PointPosition = ListFruit(i) Then
                ListFruit.RemoveAt(i) 'Enlever le fruit mangé
                If ModeInf = True Then
                    FruitPos()
                End If
                'Ajouter un nouveau fruit à l'endroit de la liste ou le fruit a été retiré
                LenghtSnake += 1 'Augmenter la longueur du serpent
                NbrEat += 1
            End If
        Next
        If Player2 = True Then
            For i As Integer = ListFruit.Count - 1 To 0 Step -1
                If PointPosition2 = ListFruit(i) Then
                    ListFruit.RemoveAt(i) 'Enlever le fruit mangé
                    If ModeInf = True Then
                        FruitPos()
                    End If
                    'Ajouter un nouveau fruit à l'endroit de la liste ou le fruit a été retiré
                    LenghtSnake2 += 1 'Augmenter la longueur du serpent
                    NbrEat += 1
                End If
            Next
        End If

        If ModeInf = False Then
            If NbrEat = NbrFruits Then
                CodeError = 9
                EndOfGame()
            End If
        End If
    End Sub



    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        'Donne la direction de la tête du serpent en fonction de la touche préssée
        Select Case e.KeyCode
            Case Keys.Up
                If Direction <> 2 Then Direction = 4   'permet d'éviter un maximum que le serpent se retourne totalement et se bouffe
            Case Keys.Down
                If Direction <> 4 Then Direction = 2
            Case Keys.Left
                If Direction <> 1 Then Direction = 3
            Case Keys.Right
                If Direction <> 3 Then Direction = 1
        End Select

        If Player2 = True Then
            Select Case e.KeyCode
                Case Keys.Z
                    If Direction2 <> 2 Then Direction2 = 4
                Case Keys.S
                    If Direction2 <> 4 Then Direction2 = 2
                Case Keys.Q
                    If Direction2 <> 1 Then Direction2 = 3
                Case Keys.D
                    If Direction2 <> 3 Then Direction2 = 1
            End Select
        End If

        If e.KeyCode = Keys.Space Then
            boost = True
            timerMvt.Interval = 100
            'Math.Ceiling(NormalSpeed * 2)
        ElseIf e.KeyCode <> Keys.Space Then
            boost = False
            timerMvt.Interval = NormalSpeed
        End If
    End Sub


    Public Function SnakeList(fruit As Boolean)
        'defini et remplit la liste des positions de chaque partie du serpent
        'juste une liste de laquelle on supprime le dernier element et on rajoute un element (tete) au début
        If fruit = True Then
            ListPos.Insert(0, PointPosition)
        ElseIf fruit = False Then                'distingue si le serpent a mangé (et ne pas supprimer le dernier elem) ou non
            ListPos.Insert(0, PointPosition)
            ListPos.Remove(ListPos.Last)
        End If
        Return ListPos
    End Function

    Public Function SnakeList2(fruit As Boolean)
        If fruit = True Then
            ListPos2.Insert(0, PointPosition2)
        ElseIf fruit = False Then
            ListPos2.Insert(0, PointPosition2)
            ListPos2.Remove(ListPos2.Last)
        End If
        Return ListPos2
    End Function

    Public Function CheckHimself()
        'vérifie que la tête du serpent ne passe jamais sur un element du serpent sinon il se bouffe lui même
        Dim tempList = ListPos
        Dim tempList2 = ListPos2
        For Each elem As Point In tempList.Skip(1)
            If PointPosition = elem Then
                CodeError = 1
                EndOfGame()
            End If
        Next

        If Player2 = True Then
            For Each elem As Point In tempList2.Skip(1)
                If PointPosition2 = elem Then
                    CodeError = 4
                    EndOfGame()
                End If
            Next

            For Each elem As Point In tempList
                If PointPosition2 = elem Then     'Verifier si les deux sepents se rentre l'un dans l'autre
                    CodeError = 5
                    EndOfGame()
                End If
            Next

            For Each elem As Point In tempList2
                If PointPosition = elem Then
                    CodeError = 6
                    EndOfGame()
                End If
            Next
        End If
    End Function

    Public Function CheckWin()
        'Verifie si le serpent remplit toutes les cases (sauf une par soucis de facilité à pas devoir gérer les timings des opérations)
        Dim nbrObs As Integer
        nbrObs = ListPosObs.Count + 1
        If Player2 = False Then
            If ListPos.Count = GridSize * GridSize - nbrObs Then
                CodeError = 3
                EndOfGame()
            End If
        Else
            If ListPos.Count + ListPos2.Count = GridSize * GridSize - nbrObs - 1 Then    'victoire si les deux serpents survivent mdr
                CodeError = 3
                EndOfGame()
            End If
        End If
    End Function

    Private Sub timerMvt_Tick(sender As Object, e As EventArgs) Handles timerMvt.Tick
        'Toutes les opérations qui doivent arriver à chaque pas
        TimerTime += 1
        lblTime.Text = TimerTime.ToString
        lblNbrSnake.Text = ListPos.Count.ToString
        lblPlayer2.Text = ListPos2.Count.ToString
        CheckWin()

        'Déplacement du serpent et verification de l'erreur/victoire
        Select Case Direction
            Case 4
                If PointPosition.Y > 0 Then
                    PointPosition.Y -= 1
                Else
                    CodeError = 2
                    EndOfGame()
                End If
            Case 2
                If PointPosition.Y < GridSize - 1 Then
                    PointPosition.Y += 1
                Else
                    CodeError = 2
                    EndOfGame()
                End If
            Case 3
                If PointPosition.X > 0 Then
                    PointPosition.X -= 1
                Else
                    CodeError = 2
                    EndOfGame()
                End If
            Case 1
                If PointPosition.X < GridSize - 1 Then
                    PointPosition.X += 1
                Else
                    CodeError = 2
                    EndOfGame()
                End If
        End Select

        If Player2 = True Then
            Select Case Direction2
                Case 4
                    If PointPosition2.Y > 0 Then
                        PointPosition2.Y -= 1
                    Else
                        CodeError = 2
                        EndOfGame()
                    End If
                Case 2
                    If PointPosition2.Y < GridSize - 1 Then
                        PointPosition2.Y += 1
                    Else
                        CodeError = 2
                        EndOfGame()
                    End If
                Case 3
                    If PointPosition2.X > 0 Then
                        PointPosition2.X -= 1
                    Else
                        CodeError = 2
                        EndOfGame()
                    End If
                Case 1
                    If PointPosition2.X < GridSize - 1 Then
                        PointPosition2.X += 1
                    Else
                        CodeError = 2
                        EndOfGame()
                    End If
            End Select
        End If

        CheckHimself()   'on verifie apres si il fait de la merde
        CheckEaten()
        If GameMode = True Then
            CheckSnakeInObst()
        End If

        'Mise à jour de la liste du serpent
        'c'est juste le check si il doit s'aggrandir ou non
        If LenghtSnake > ListPos.Count Then
            SnakeList(True)
        Else
            SnakeList(False)
        End If

        If Player2 = True Then
            If LenghtSnake2 > ListPos2.Count Then
                SnakeList2(True)
            Else
                SnakeList2(False)
            End If
        End If

        'ça refresh la grille mais .refresh() fonctionnera surement pas vu qu'ici on supprime tout pour tout redessiner par dessus
        Me.Invalidate()
    End Sub

    Public Function EndOfGame()

        Dim message1 As String
        Dim message2 As String
        Dim message3 As String = "Vous avez gagné !!!"
        Dim message4 As String
        Dim message5 As String
        Dim message6 As String
        Dim message7 As String
        Dim message8 As String = "Vous vous êtes pris un obstacle !"
        Dim message9 As String = "Vous avez gagné vous avez mangé toutes les pommes !"

        If Player2 = False Then
            message1 = "Vous vous êtes mangé... Oupsi ?"
            message2 = "Vous vous êtes pris un mur !"
            message4 = "Ce message ne devrait pas apparaitre, en parler à Louis"
            message5 = "Ce message ne devrait pas apparaitre, en parler à Louis"
            message6 = "Ce message ne devrait pas apparaitre, en parler à Louis"
            message7 = "Ce message ne devrait pas apparaitre, en parler à Louis"
        Else
            message1 = "Joueur vert s'est mangé la queue, victoire du joueur rouge"
            message2 = "Joueur vert s'est pris un mur, victoire du joueur rouge"
            message4 = "Joueur rouge s'est mangé la queue, victoire du joueur vert"
            message5 = "Joueur rouge s'est fait écrasé la tête par le joueur vert"
            message6 = "Joueur vert s'est fait écrasé la tête par le joueur rouge"
            message7 = "Joueur rouge s'est pris un mur, victoire du joueur vert"
        End If

        timerMvt.Stop()

        NbrPartie = ListPos.Count()
        Dim Menu As Integer
        If CodeError = 1 Then
            Menu = MessageBox.Show("Voulez vous refaire une partie de Snake ?" + vbCr + "Vous avez un score de : " + NbrPartie.ToString + vbCr + message1, "Snake", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
        ElseIf CodeError = 2 Then
            Menu = MessageBox.Show("Voulez vous refaire une partie de Snake ?" + vbCr + "Vous avez un score de : " + NbrPartie.ToString + vbCr + message2, "Snake", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
        ElseIf CodeError = 3 Then
            Menu = MessageBox.Show("Voulez vous refaire une partie de Snake ?" + vbCr + "Vous avez un score de : " + NbrPartie.ToString + vbCr + message3, "Snake", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
        ElseIf CodeError = 4 Then
            Menu = MessageBox.Show("Voulez vous refaire une partie de Snake ?" + vbCr + "Vous avez un score de : " + NbrPartie.ToString + vbCr + message4, "Snake", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
        ElseIf CodeError = 5 Then
            Menu = MessageBox.Show("Voulez vous refaire une partie de Snake ?" + vbCr + "Vous avez un score de : " + NbrPartie.ToString + vbCr + message5, "Snake", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
        ElseIf CodeError = 6 Then
            Menu = MessageBox.Show("Voulez vous refaire une partie de Snake ?" + vbCr + "Vous avez un score de : " + NbrPartie.ToString + vbCr + message6, "Snake", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
        ElseIf CodeError = 7 Then
            Menu = MessageBox.Show("Voulez vous refaire une partie de Snake ?" + vbCr + "Vous avez un score de : " + NbrPartie.ToString + vbCr + message7, "Snake", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
        ElseIf CodeError = 8 Then
            Menu = MessageBox.Show("Voulez vous refaire une partie de Snake ?" + vbCr + "Vous avez un score de : " + NbrPartie.ToString + vbCr + message8, "Snake", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
        ElseIf CodeError = 9 Then
            Menu = MessageBox.Show("Voulez vous refaire une partie de Snake ?" + vbCr + "Vous avez un score de : " + NbrPartie.ToString + vbCr + message9, "Snake", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
        End If
        Select Case Menu
            Case DialogResult.Yes
                ListPos.Clear()
                ListFruit.Clear()
                ListPos2.Clear()
                ListPosObs.Clear()

                'ListPosObs.Clear()
                NbrEat = 0
                StartAgain()
            Case Else
                Close()
        End Select
    End Function

    Public Function StartAgain()
        'fonction de debut de partie
        TimerTime = 0
        If FirstGame = False Then
            Dim Menu2 As Integer
            Menu2 = MessageBox.Show("Voulez-vous continer une partie de Snake avec les paramètres actuels ?" + vbCr + "Cliquez sur Continuer pour changer les paramètres" + vbCr + "réessayer pour garder les paramètres actuels", "Snake", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
            Select Case Menu2
                Case DialogResult.TryAgain
                    'Re Initialisation du jeu
                    BeginGame()
                Case DialogResult.Continue
                    FirstLauch()
                Case DialogResult.Cancel
                    Close()
            End Select
        Else
            Dim Menu2 As Integer
            Menu2 = MessageBox.Show("Voulez-vous jouer une partie de Snake ?", "Snake", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
            Select Case Menu2
                Case DialogResult.Yes
                    'Re Initialisation du jeu
                    FirstLauch()
                Case Else
                    Close()
                    Me.Close()
            End Select
        End If
        FirstGame = False
        Return 1
    End Function

    Public Function FirstLauch()
        SizeOf = "15"
        Speed = "750"
        NbrFruitsSTR = "1"

        ' Configuration des paramètres du jeu
        SizeOf = InputBox("Quelle taille de jeu voulez-vous ?" + vbCr + "Attention, plus le chiffre est haut, plus le jeu sera grand !" + vbCr + "Max 200 et min 3 !", "Snake", SizeOf)
        Speed = InputBox("Quelle vitesse souhaitez-vous ?" + vbCr + "Attention 0 signifie Lent et 1000 extremement rapide !", "Snake", Speed)
        NbrFruitsSTR = InputBox("Combien de fruits voulez-vous sur la grille ?" + vbCr + "Si vous voulez un max de fruit mettez 999999 !", "Snake", NbrFruitsSTR)
        If IsNumeric(Val(SizeOf)) And Val(SizeOf) > 3 And Val(SizeOf) <= 200 Then
            GridSize = Val(SizeOf)
        Else
            GridSize = 3            'J'ai eu un bug avec les while donc pour les petits malins qui mettent des valeurs qui
        End If                      'fonctionnent pas bah ils auront un 3x3, même chose pour le reste
        If IsNumeric(Val(Speed)) And Val(Speed) >= 0 And Val(Speed) <= 1000 Then
            timerMvt.Interval = (1001 - Val(Speed))
        Else
            timerMvt.Interval = Math.Ceiling(500 / (GridSize / 3))
        End If
        If IsNumeric(Val(NbrFruitsSTR)) And Val(NbrFruitsSTR) > 0 Then
            If Val(NbrFruitsSTR) > Math.Ceiling((GridSize * GridSize) * 0.9) Then
                NbrFruits = Math.Ceiling((GridSize * GridSize) * 0.9)
            Else
                NbrFruits = Val(NbrFruitsSTR)
            End If
        Else
            NbrFruitsSTR = "1"
            NbrFruits = 1
        End If
        CellSize = Math.Ceiling(1100 / GridSize)
        NormalSpeed = timerMvt.Interval()
        'Me.ClientSize = New Size(GridSize * CellSize, GridSize * CellSize)

        Dim choice1 As Integer
        choice1 = MessageBox.Show("Voulez-vous un serpent vert ?", "Snake", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        Select Case choice1
            Case DialogResult.No
                ColorChoice = False
            Case DialogResult.Yes
                ColorChoice = True
        End Select
        Dim choice2 As Integer
        choice2 = MessageBox.Show("Voulez-vous afficher la gille ?", "Snake", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        Select Case choice2
            Case DialogResult.No
                GrilleBool = False
            Case DialogResult.Yes
                GrilleBool = True
        End Select
        Dim choice3 As Integer
        choice3 = MessageBox.Show("Voulez vous jouer solo ?", "Snake", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        Select Case choice3
            Case DialogResult.Yes
                Player2 = False
            Case DialogResult.No
                Player2 = True
        End Select
        If GridSize > 6 Then
            Dim choice4 As Integer
            choice4 = MessageBox.Show("Voulez vous jouer en mode normal, sans obstacles ?", "Snake", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            Select Case choice4
                Case DialogResult.Yes
                    GameMode = False
                Case DialogResult.No
                    Dim ObstSTR As String = "1"
                    GameMode = True
                    ObstSTR = InputBox("Quelle difficultée voulez vous ?" + vbCr + "Attention, plus vous mettez un chiffre haut plus il y en aura !" + vbCr + "1 pour peu, 10 pour beaucoup", "Snake", ObstSTR)
                    If IsNumeric(Val(ObstSTR)) And Val(ObstSTR) >= 0 And Val(ObstSTR) <= 10 Then
                        NbrObst = Math.Ceiling(((GridSize - 6) * Val(ObstSTR)) - 5)
                    Else
                        NbrObst = (GridSize * 2) - 3
                    End If
            End Select
            Dim choice5 As Integer
            choice5 = MessageBox.Show("Voulez vous jouer en mode infini ?" + vbCr + "Si non vous gagnerez en attrapant toutes les pommes !", "Snake", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            Select Case choice5
                Case DialogResult.Yes
                    ModeInf = True
                Case DialogResult.No
                    ModeInf = False
            End Select
        End If
        BeginGame()
        'Initialisation du jeu
    End Function

    Public Function BeginGame()
        PointPosition = New Point(0, 0)
        ListPos.Add(PointPosition)
        Direction = 1
        LenghtSnake = 1
        If Player2 = True Then
            PointPosition2 = New Point(GridSize - 1, GridSize - 1)
            ListPos2.Add(PointPosition2)
            Direction2 = 3
            LenghtSnake2 = 1
        End If
        'Dim FirstObst As New Point(random.Next(0, GridSize), random.Next(1, GridSize - 1))
        '
        'ListPosObs.Add(FirstObst)
        'ObstPos()
        If GameMode = True Then
            ObstPos()
        End If
        FruitPos()
        timerMvt.Start()
    End Function
    'Vraiment horrible maintenant que je l'ai relu, beaucoup qui pourrait etre reduit mais flemme
    'La prochaine fois que je passerai dessus je vais changer :
    '-Faire une liste pour les listes de serpents pour pouvoir jouer à un nombre illimité mdr
    '-Trouver pourquoi ça crash si on change le nombre d'obstacle avec un serpent multicolore ???   'je crois que c'est réparé
    '-Faire les paramètres sur un form à part pour plus devoir cliquer 8x pour arriver au jeu
    '-Faire un mode de jeu qui ajouer un niveau de difficulté pour le gamemode à chaque fois qu'on gagne automatiquement
    '-donner les points pour chaque joueur à la fin de partie
    '-Créer une methode pour que le serpent ait un mode automatique (évite les obstacles, cherche les fruits)
    '-ajouter des points de vie et les retirer si on touche un mur, reset la position du serpent
End Class
