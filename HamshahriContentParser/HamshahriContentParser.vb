Imports System.Net
Imports System.IO
Imports HtmlAgilityPack
Imports Common

Public Class HamshahriContentParser

    Public Sub Extract()

        While True

            Dim i As Integer = FileSystemHelper.GetLastRawJsonContentId
            i += 1

            Console.Write(i & ": ")

            Try

                Dim content As String = FileSystemHelper.GetRawHtml(i)

                If String.IsNullOrEmpty(content) Then
                    Exit While
                End If

                Dim doc As New HtmlDocument
                doc.LoadHtml(content)

                Dim item As New NewsArticle
                With item

                    Dim contentNode = doc.DocumentNode.Descendants("div").Where(Function(x) x.GetAttributeValue("class", "").Contains("newsBodyCont")).FirstOrDefault
                    .PublishDate = ExtractPublishDate(doc)
                    .Title = ExtractTitle(contentNode)
                    .LeadPart = ExtractLeadPart(contentNode)
                    .Labels = ExtractLabels(contentNode)
                    .Publisher = ExtractPublisher(contentNode)
                    .Body = ExtractContent(contentNode)
                    .Id = i
                    .Url = HttpHelper.GetUrl(i)

                End With

                FileSystemHelper.WriteRawJson(item)

            Catch ex As Exception
                Console.Write(ex.ToString)
            Finally

                FileSystemHelper.SetLastRawJsonContentId(i)
                Console.WriteLine()

            End Try

        End While

    End Sub

    Private Function ExtractContent(htmlNode As HtmlNode) As String

        Dim content As String = ""

        Try

            Dim contentNode = htmlNode.Descendants("p")

            If contentNode.Count > 0 Then

                For Each item In contentNode
                    content &= Helper.ConvertHtmlToText(item.InnerText) & vbCrLf
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
                'out = PersianCalendarHelper.GetDate(Trim(publishDateNode.First.InnerText))
            End If

        Catch ex As Exception

        End Try

    End Function

    

End Class
