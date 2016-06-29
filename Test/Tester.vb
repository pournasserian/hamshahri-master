Imports System.Text

Public Class Tester

    Public Property Path As String
    Public Property ExtractPath As String
    Public Property WordSpliter As String()
    Public Property SentenceSpliter As String()

    Public Sub New()

        Path = System.Configuration.ConfigurationManager.AppSettings.Item("ContentFolder")
        ExtractPath = Path & "extract\"
        WordSpliter = {".", "<", ">", "/", ",", "]", "[", "(", ")", "-", "=", "+", "_", "!", "~", "#", "$", "%", "^", "&", "*", "{", "}", "`", "\", "|", """", "?", ";", ":", "،", "؛", ",", vbCr, vbCrLf, " ", vbTab, "«", "«", "", "–", "»", "»", " ", " ", "؟"}
        SentenceSpliter = {".", "!", "*", "{", "}", """", "?", ";", ":", "؛", vbCr, vbCrLf, vbTab, "؟"}

    End Sub

    Public Sub Analyse()

        For Each file In System.IO.Directory.GetFiles(ExtractPath)

            Console.Write(file)

            Dim content As String = System.IO.File.ReadAllText(file)
            content = Revise(content)
            AnalayseCharDensity(content)
            'AnalayseWordsCount(content)
            'AnalayseSequence(content)
            Console.WriteLine(" : OK!")

        Next

    End Sub

    Private Function Revise(content As String) As String
        Dim out As String = content.Replace("ي", "ی")

        Return out
    End Function

    Private Sub AnalayseCharDensity(content As String)

        For i As Integer = 0 To content.Length - 1
            Dim ch = content.Chars(i)
            If Not Me.CharDensity.ContainsKey(ch) Then
                Me.CharDensity.Add(ch, New CharacterDensity With {.Count = 0, .Character = ch})
            End If
            Me.CharDensity.Item(ch).Count += 1
        Next

    End Sub

    Private Sub AnalayseSequence(content As String)

        For Each sentence In SplitSentences(content)

            Dim prevWord As String = ""
            Dim curWord As String = ""

            For Each word In SplitWords(sentence)

                If prevWord = "" Then
                    prevWord = word
                    Continue For
                End If

                curWord = word

                Dim key As String = prevWord & " " & curWord

                If Not Me.Sequence.ContainsKey(key) Then
                    Me.Sequence.Add(key, New NGram With {.Count = 0, .WordSequence = key})
                End If

                Me.Sequence.Item(key).Count += 1

                prevWord = word

            Next

        Next

    End Sub

    Private Sub AnalayseWordsCount(content As String)

        Dim words() As String = Me.SplitWords(content)

        For Each word In words
            If Not Me.Counter.ContainsKey(word) Then
                Me.Counter.Add(word, New WordCount With {.Count = 0, .Word = word})
            End If
            Me.Counter.Item(word).Count += 1
        Next

    End Sub

    Public Sub SaveCharDensity()

        Dim contentToWrite As New StringBuilder

        For Each item In Me.CharDensity.Values.OrderByDescending(Function(x) x.Count)
            contentToWrite.Append(AscW(item.Character))
            contentToWrite.Append(vbTab)
            contentToWrite.Append(item.Count.ToString)
            contentToWrite.Append(vbTab)
            contentToWrite.Append(item.Character)
            contentToWrite.AppendLine()
        Next

        System.IO.File.WriteAllText(Path & "CharacterDensity.txt", contentToWrite.ToString)

    End Sub

    Public Sub SaveCounter()

        Dim contentToWrite As New StringBuilder

        For Each wordcount In Me.Counter.Values.OrderByDescending(Function(x) x.Count)
            contentToWrite.Append(wordcount.Word)
            contentToWrite.Append(vbTab)
            contentToWrite.AppendLine(wordcount.Count.ToString)
        Next

        System.IO.File.WriteAllText(Path & "wordcount.txt", contentToWrite.ToString)

    End Sub

    Public Sub SaveSequence()

        Dim contentToWrite As New StringBuilder

        For Each seq In Me.Sequence.Values.OrderByDescending(Function(x) x.Count)
            contentToWrite.Append(seq.WordSequence)
            contentToWrite.Append(vbTab)
            contentToWrite.AppendLine(seq.Count.ToString)
        Next

        System.IO.File.WriteAllText(Path & "sequence2.txt", contentToWrite.ToString)

    End Sub

    Public Function SplitWords(value As String) As String()

        Dim out As String()

        out = value.Split(Me.WordSpliter, StringSplitOptions.RemoveEmptyEntries)

        Return out

    End Function

    Public Function SplitSentences(value As String) As String()

        Dim out As String()

        out = value.Split(Me.SentenceSpliter, StringSplitOptions.RemoveEmptyEntries)

        Return out

    End Function

    Public Property CharDensity As New Dictionary(Of Char, CharacterDensity)
    Public Property Counter As New Dictionary(Of String, WordCount)
    Public Property Sequence As New Dictionary(Of String, NGram)

End Class

Public Class CharacterDensity

    Public Property Character As Char
    Public Property Count As Integer

End Class

Public Class WordCount

    Public Property Word As String
    Public Property Count As Integer

End Class

Public Class NGram

    Public Property WordSequence As String
    Public Property Count

End Class
