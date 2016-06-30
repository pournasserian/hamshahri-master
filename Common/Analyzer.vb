Public Class Analyzer

    Public ReadOnly Property NewsArticleContentPath As String
    Public ReadOnly Property NewsArticleProcessedPath As String

    Private _charcterDensityFile As String = "CharDensity.tsv"
    Private _wordCountFile As String = "WordCount.tsv"
    Private seperator As Char = vbTab
    Public Property WordSpliter As String()

    Public Property CharachterDensity As Dictionary(Of Char, CharacterDensity)
    Public Property WordCounts As Dictionary(Of String, WordCount)

    Public Sub New()

        WordSpliter = {".", "<", ">", "/", ",", "]", "[", "(", ")", "-", "=", "+", "_", "!", "~", "#", "$", "%", "^", "&", "*", "{", "}", "`", "\", "|", """", "?", ";", ":", "،", "؛", ",", vbCr, vbCrLf, " ", vbTab, "«", "«", "", "–", "»", "»", " ", " ", "؟"}

        NewsArticleContentPath = GetRootPath() & "NewsArticles\"
        NewsArticleProcessedPath = NewsArticleContentPath & "Done\"

        CharachterDensity = New Dictionary(Of Char, CharacterDensity)
        WordCounts = New Dictionary(Of String, WordCount)

        If Not System.IO.Directory.Exists(NewsArticleProcessedPath) Then
            System.IO.Directory.CreateDirectory(NewsArticleProcessedPath)
        End If

        LoadCharDensity()
        LoadWordCount()

    End Sub

    Private Sub LoadCharDensity()

        If System.IO.File.Exists(GetCharachterDensityPath) Then

            For Each line In System.IO.File.ReadAllLines(GetCharachterDensityPath)

                Dim data() As String = line.Split(seperator)
                Dim charDensity As New CharacterDensity
                charDensity.Character = data(0)
                charDensity.Count = CInt(data(1))
                CharachterDensity.Add(charDensity.Character, charDensity)

            Next


        End If

    End Sub

    Private Sub LoadWordCount()

        If System.IO.File.Exists(GetWordCountPath) Then

            For Each line In System.IO.File.ReadAllLines(GetWordCountPath)

                Dim data() As String = line.Split(seperator)
                Dim wordCount As New WordCount
                wordCount.Word = data(0)
                wordCount.Count = CInt(data(1))
                WordCounts.Add(wordCount.Word, wordCount)

            Next


        End If

    End Sub

    Private Function GetCharachterDensityPath() As String
        Return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) & "\" & _charcterDensityFile
    End Function

    Private Function GetWordCountPath() As String
        Return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) & "\" & _wordCountFile
    End Function

    Private Function GetRootPath() As String
        Return System.Configuration.ConfigurationManager.AppSettings.Item("ContentFolder")
    End Function

    Public Function GetNewsArticleId() As String

        If System.IO.Directory.GetFiles(NewsArticleContentPath).Count > 0 Then
            Return System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(NewsArticleContentPath)(0))
        Else
            Return String.Empty
        End If

    End Function

    Public Function GetNewsArticle(articleId As String) As NewsArticle

        Dim sourceFile As String = NewsArticleContentPath & articleId & ".json"

        Try

            If System.IO.File.Exists(sourceFile) Then

                Dim destFile As String = NewsArticleProcessedPath & articleId & ".json"
                System.IO.File.Move(sourceFile, destFile)
                Return Helper.Deserialize(System.IO.File.ReadAllText(destFile))

            End If

        Catch ex As Exception

            Return Nothing

        End Try

        Return Nothing

    End Function

    Public Sub UpdateCharacterDensity(article As NewsArticle)
        UpdateCharDensity(article.Title)
        UpdateCharDensity(article.Body)
    End Sub

    Public Sub UpdateWordCount(article As NewsArticle)
        UpdateWordCountLocal(article.Title)
        UpdateWordCountLocal(article.Body)
    End Sub

    Private Sub UpdateCharDensity(content As String)

        For Each ch In content.ToArray

            If ch = seperator Then
                Exit Sub
            End If

            If CharachterDensity.ContainsKey(ch) Then
                CharachterDensity.Item(ch).Count += 1
            Else
                CharachterDensity.Add(ch, New CharacterDensity With {.Character = ch, .Count = 1})
            End If

        Next

    End Sub

    Private Sub UpdateWordCountLocal(content As String)

        For Each word In content.Split(WordSpliter, StringSplitOptions.RemoveEmptyEntries)

            If word = seperator Then
                Exit Sub
            End If

            If WordCounts.ContainsKey(word) Then
                WordCounts.Item(word).Count += 1
            Else
                WordCounts.Add(word, New WordCount With {.Word = word, .Count = 1})
            End If

        Next

    End Sub

    Public Sub SaveCharDensity()

        Dim contentToWrite As String = String.Empty

        For Each charDensity In CharachterDensity.Values.OrderBy(Function(x) x.Character)
            contentToWrite &= charDensity.Character & seperator & charDensity.Count & vbCrLf
        Next

        System.IO.File.WriteAllText(GetCharachterDensityPath, contentToWrite)

    End Sub

    Public Sub SaveWordCount()

        Dim contentToWrite As String = String.Empty

        For Each wordCount In WordCounts.Values
            contentToWrite &= wordCount.Word & seperator & wordCount.Count & vbCrLf
        Next

        System.IO.File.WriteAllText(GetWordCountPath, contentToWrite)

    End Sub

End Class
