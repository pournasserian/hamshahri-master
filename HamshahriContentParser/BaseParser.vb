Imports System.Net
Imports System.IO
Imports HtmlAgilityPack

Public Class BaseParser

    Private Shared path As String = "C:\Users\a.pournasserian\Desktop\Hamshahri\Content\"
    Private Shared extractpath As String = path & "extract\"
    Private Shared lastcontentfileindex As String = path & "lastcontentfileid.txt"
    Private Shared rawpath As String = path & "raw\"
    Private Shared pCalendar As New System.Globalization.PersianCalendar

    Public Shared Sub ExtractAll()

        While True

            Dim i As Integer = GetLastContentFileId()
            i += 1
            Dim content As String = GetContent(i)

            Dim doc As New HtmlDocument
            doc.LoadHtml(content)

            Dim publishDate As DateTime? = ExtractPublishDate(doc)
            Dim publishDateString As String = ""
            If publishDate.HasValue Then
                publishDateString = publishDate.Value
            End If

            Dim contentNode = doc.DocumentNode.Descendants("div").Where(Function(x) x.GetAttributeValue("class", "").Contains("newsBodyCont")).FirstOrDefault
            Dim title As String = ExtractTitle(contentNode)
            Dim body As String = ExtractContent(contentNode)

            Write(i, title, publishDateString, body)

            SetLastContentFileId(i)

        End While

    End Sub

    Private Shared Function GetContent(fileid As Integer) As String
        Dim fullpath As String = rawpath & fileid & ".txt"
        Return System.IO.File.ReadAllText(fullpath)
    End Function

    Private Shared Function ReviseHtmlString(value As String) As String
        Dim out As String = Trim(System.Web.HttpUtility.HtmlDecode(value))
        out = Trim(out.Replace(System.Environment.NewLine, ""))
        Return out
    End Function

    Private Shared Function ExtractContent(htmlNode As HtmlNode) As String

        Dim content As String = ""
        Dim leadPart As String = ""

        Try

            Dim leadingPartNode = htmlNode.Descendants("div").Where(Function(x) x.GetAttributeValue("class", "").Contains("leadContainer"))

            If leadingPartNode.Count > 0 Then
                Dim cat = leadingPartNode.Descendants("")

                leadPart = ReviseHtmlString(leadingPartNode.FirstOrDefault.InnerText)

            End If

        Catch ex As Exception

        End Try

        Return leadPart
    End Function

    Private Shared Function ExtractTitle(htmlNode As HtmlNode) As String

        Dim title As String = ""

        Try
            Dim node = htmlNode.Descendants("h3")
            If node.Count > 0 Then
                title = ReviseHtmlString(node.FirstOrDefault.InnerText)
            End If

        Catch ex As Exception

        End Try

        Return title

    End Function

    Private Shared Function ExtractPublishDate(doc As HtmlDocument) As DateTime?

        Try

            Dim publishDateNode = doc.DocumentNode.Descendants("span").Where(Function(x) x.GetAttributeValue("class", "").Contains("publisheDate"))

            If publishDateNode.Count > 0 Then

                Dim dateTimeSting As String = Trim(publishDateNode.First.InnerText)
                Dim arr() As String = dateTimeSting.Split("-")

                If arr.Length <> 2 Then
                    Return Nothing
                End If

                Dim dateString() As String = arr(0).Split(" ")

                Dim pDay As Integer = 0
                If Not Integer.TryParse(Trim(dateString(1)), pDay) Then
                    Return Nothing
                End If

                Dim pMonth As Integer = GetMonth(dateString(2))
                If pMonth = 0 Then
                    Return Nothing
                End If

                Dim pYear As Integer = 0
                If Not Integer.TryParse(Trim(dateString(3)), pYear) Then
                    Return Nothing
                End If


                Dim hour As Integer = 0
                Dim minute As Integer = 0
                Dim second As Integer = 0
                Dim times() As String = arr(1).Split(":")

                If times.Length = 3 Then
                    Integer.TryParse(times(0), hour)
                    Integer.TryParse(times(1), minute)
                    Integer.TryParse(times(2), second)
                End If

                Dim out As New DateTime(pYear, pMonth, pDay, hour, minute, second, 0, pCalendar)
                Return out

            Else
                Return Nothing
            End If


        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Private Shared Function GetMonth(name As String) As Integer

        Select Case Trim(name)
            Case "فروردین"
                Return 1
            Case "اردیبهشت"
                Return 2
            Case "خرداد"
                Return 3
            Case "تیر"
                Return 4
            Case "مرداد"
                Return 5
            Case "شهریور"
                Return 6
            Case "مهر"
                Return 7
            Case "آبان"
                Return 8
            Case "آذر"
                Return 9
            Case "دی"
                Return 10
            Case "بهمن"
                Return 11
            Case "اسفند"
                Return 12

            Case Else
                Return 0

        End Select

    End Function

    Public Shared Sub Write(fileid As Integer, title As String, publishDate As String, body As String)

        Dim content As String = title & vbCrLf & publishDate & vbCrLf & body
        Dim fullpath As String = extractpath & fileid & ".txt"
        System.IO.File.WriteAllText(fullpath, content)

    End Sub

    Private Shared Function GetLastContentFileId() As Integer

        If Not System.IO.File.Exists(lastcontentfileindex) Then
            Return 1
        End If

        Dim id As Integer = CInt(Trim(System.IO.File.ReadAllText(lastcontentfileindex)))
        Return id

    End Function

    Private Shared Sub SetLastContentFileId(id As Integer)
        System.IO.File.WriteAllText(lastcontentfileindex, id.ToString)
    End Sub

End Class
