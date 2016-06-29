Imports Common

Module Module1

    Sub Main()


        Dim parser As New HtmlContentParser
        Dim contentId As String = parser.GetHtmlContentId

        While Not String.IsNullOrEmpty(contentId)

            Dim htmlContent As String = parser.GetHtmlContent(contentId)
            Dim article As NewsArticle = parser.CreateArticle(contentId, htmlContent)

            contentId = parser.GetHtmlContentId

        End While






    End Sub

End Module


