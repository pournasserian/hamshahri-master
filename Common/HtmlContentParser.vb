Imports Common
Imports HtmlAgilityPack

Public Class HtmlContentParser

    Public ReadOnly Property SourceHtmlContentPath As String
    Public ReadOnly Property DestinationHtmlContentPath As String
    Public ReadOnly Property NewsArticleContentPath As String

    Public Sub New()

        SourceHtmlContentPath = GetRootPath() & "Raw\"
        DestinationHtmlContentPath = _SourceHtmlContentPath & "Done\"
        NewsArticleContentPath = GetRootPath() & "NewsArticles\"

        If Not System.IO.Directory.Exists(DestinationHtmlContentPath) Then
            System.IO.Directory.CreateDirectory(DestinationHtmlContentPath)
        End If

        If Not System.IO.Directory.Exists(NewsArticleContentPath) Then
            System.IO.Directory.CreateDirectory(NewsArticleContentPath)
        End If

    End Sub

    Private Function GetRootPath() As String
        Return System.Configuration.ConfigurationManager.AppSettings.Item("ContentFolder")
    End Function

    Public Function GetHtmlContent(contentId As String) As String

        Dim sourceFile As String = SourceHtmlContentPath & contentId & ".txt"

        If System.IO.File.Exists(sourceFile) Then

            Dim destFile As String = DestinationHtmlContentPath & contentId & ".txt"
            System.IO.File.Move(sourceFile, destFile)
            Return System.IO.File.ReadAllText(destFile)

        Else

            Return String.Empty

        End If

    End Function

    Public Function GetHtmlContentId() As String

        If System.IO.Directory.GetFiles(SourceHtmlContentPath).Count > 0 Then
            Return System.IO.Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(SourceHtmlContentPath)(0))
        Else
            Return String.Empty
        End If

    End Function


    Public Function CreateArticle(contentId As String, htmlContent As String) As NewsArticle

        Dim doc As New HtmlDocument
        doc.LoadHtml(htmlContent)

        Dim item As New NewsArticle
        With item

            Dim contentNode = doc.DocumentNode.Descendants("div").Where(Function(x) x.GetAttributeValue("class", "").Contains("newsBodyCont")).FirstOrDefault
            .PublishDate = ExtractPublishDate(doc)
            .Title = ExtractTitle(contentNode)
            .LeadPart = ExtractLeadPart(contentNode)
            .Labels = ExtractLabels(contentNode)
            .Publisher = ExtractPublisher(contentNode)
            .Body = ExtractContent(contentNode)
            .Id = contentId

        End With


        System.Diagnostics.Debug.WriteLine(item.Id & "," & item.Publisher)

        Return item

    End Function

    Public Sub Save(article As NewsArticle)

        If String.IsNullOrEmpty(article.Title) Then
            Exit Sub
        End If

        Dim jsonString As String = Helper.Serialize(article)
        Dim fileName As String = NewsArticleContentPath & article.Id.ToString & ".json"

        System.IO.File.WriteAllText(fileName, jsonString)

    End Sub

    Private Function ExtractContent(htmlNode As HtmlNode) As String

        Dim content As String = ""

        Try

            Dim contentNode = htmlNode.Descendants("p")

            If contentNode.Count > 0 Then

                For Each item In contentNode
                    content &= Helper.ConvertHtmlToText(item.InnerText)
                Next


            End If

        Catch ex As Exception

        End Try

        Return content

    End Function

    Private Function ExtractLeadPart(htmlNode As HtmlNode) As String

        Dim leadPart As String = ""

        Try

            Dim leadingPartNode = htmlNode.Descendants("div").Where(Function(x) x.GetAttributeValue("class", "").Contains("leadContainer"))

            If leadingPartNode.Count > 0 Then

                Dim internalNodes = leadingPartNode.FirstOrDefault.ChildNodes

                If internalNodes.Count > 0 Then
                    Dim lastIndex = internalNodes.Count - 1
                    Dim iCounter As Integer = 0
                    While String.IsNullOrEmpty(Trim(leadPart))
                        leadPart = Helper.ConvertHtmlToText(internalNodes(lastIndex - iCounter).InnerText.Replace("-", "").Replace(":", ""))
                        iCounter += 1
                    End While
                End If

            End If

        Catch ex As Exception

        End Try

        Return leadPart

    End Function

    Private Function ExtractLabels(htmlNode As HtmlNode) As List(Of String)

        Dim labels As New List(Of String)

        Try

            Dim leadingPartNode = htmlNode.Descendants("div").Where(Function(x) x.GetAttributeValue("class", "").Contains("leadContainer"))

            If leadingPartNode.Count > 0 Then

                Dim labelParentNode = leadingPartNode.FirstOrDefault.Descendants("span")

                If labelParentNode.Count > 0 Then
                    Dim labelString = Helper.ConvertHtmlToText(labelParentNode.FirstOrDefault.InnerText)
                End If

            End If

        Catch ex As Exception

        End Try

        Return labels
    End Function

    Private Function ExtractPublisher(htmlNode As HtmlNode) As String

        Dim source As String = ""

        Try

            Dim leadingPartNode = htmlNode.Descendants("div").Where(Function(x) x.GetAttributeValue("class", "").Contains("leadContainer"))

            If leadingPartNode.Count > 0 Then

                Dim internalNodes = leadingPartNode.FirstOrDefault.ChildNodes

                If internalNodes.Count > 3 Then
                    source = Helper.ConvertHtmlToText(internalNodes(2).InnerText.Replace("-", "").Replace(":", ""))
                End If

            End If

        Catch ex As Exception

        End Try

        Return source
    End Function

    Private Function ExtractTitle(htmlNode As HtmlNode) As String

        Dim title As String = ""

        Try
            Dim node = htmlNode.Descendants("h3")
            If node.Count > 0 Then
                title = Helper.ConvertHtmlToText(node.FirstOrDefault.InnerText)
            End If

        Catch ex As Exception

        End Try

        Return title

    End Function

    Private Function ExtractPublishDate(doc As HtmlDocument) As DateTime?

        Dim out As DateTime?

        Try

            Dim publishDateNode = doc.DocumentNode.Descendants("span").Where(Function(x) x.GetAttributeValue("class", "").Contains("publisheDate"))

            If publishDateNode.Count > 0 Then
                out = Helper.GetDate(Trim(publishDateNode.First.InnerText))
                Return out
            End If

        Catch ex As Exception

        End Try

    End Function


End Class
