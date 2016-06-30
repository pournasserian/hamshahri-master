Module Module1

    Sub Main()

        Dim analyzer As New Common.Analyzer
        Dim articleId As String = analyzer.GetNewsArticleId

        Dim iCounter As Integer = 0

        While Not String.IsNullOrEmpty(articleId)

            iCounter += 1

            Try

                Dim article As Common.NewsArticle = analyzer.GetNewsArticle(articleId)

                analyzer.UpdateCharacterDensity(article)
                analyzer.UpdateWordCount(article)

                Console.WriteLine(articleId & ": OK!")

                If iCounter / 1000 = CInt(iCounter / 1000) Then

                    Console.WriteLine("--------------------------------------------------")
                    analyzer.SaveCharDensity()
                    analyzer.SaveWordCount()
                    Console.WriteLine("Write completed: " & iCounter.ToString)
                    Console.WriteLine("--------------------------------------------------")

                End If

            Catch ex As Exception
                Console.WriteLine(articleId & ": Error!")
            End Try


            articleId = analyzer.GetNewsArticleId

        End While
    End Sub

End Module
