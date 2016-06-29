Imports System.Net
Imports System.IO
Imports Newtonsoft.Json

Public Class Helper

    Public Shared Function ConvertHtmlToText(value As String) As String
        Dim out As String = Trim(System.Web.HttpUtility.HtmlDecode(value))
        out = Trim(out.Replace(System.Environment.NewLine, ""))
        out = Trim(out.Replace(vbCr, ""))
        out = Trim(out.Replace(vbLf, ""))
        out = Trim(out.Replace(vbCrLf, ""))
        Return out
    End Function

    Public Shared Function GetContent(url As String) As String

        Dim webRequest As HttpWebRequest
        Dim responseReader As StreamReader
        Dim responseData As String = ""

        Try
            webRequest = HttpWebRequest.Create(url)
            responseReader = New StreamReader(webRequest.GetResponse().GetResponseStream())
            responseData = responseReader.ReadToEnd()
            responseReader.Close()
            webRequest.GetResponse().Close()

        Catch ex As Exception

        End Try

        Return responseData

    End Function

#Region " SerializationHelper "

    Public Shared Function Serialize(value As Object) As String
        Return JsonConvert.SerializeObject(value)
    End Function

    Public Shared Function Deserialize(value As String) As Object
        Dim obj = JsonConvert.DeserializeObject(Of NewsArticle)(value)
        Return obj
    End Function

#End Region

#Region " PersianCalendarHelper "

    Private Shared pCalendar As New System.Globalization.PersianCalendar

    Public Shared Function GetDate(dateTimeString As String) As DateTime?

        Try

            Dim tempString As String = dateTimeString

            tempString = tempString.Replace("جمعه", "")
            tempString = tempString.Replace("پنجشنبه", "")
            tempString = tempString.Replace("پنج شنبه", "")
            tempString = tempString.Replace("چهارشنبه", "")
            tempString = tempString.Replace("چهار شنبه", "")
            tempString = tempString.Replace("سه شنبه", "")
            tempString = tempString.Replace("دوشنبه", "")
            tempString = tempString.Replace("دو شنبه", "")
            tempString = tempString.Replace("یکشنبه", "")
            tempString = tempString.Replace("یک شنبه", "")
            tempString = tempString.Replace("شنبه", "")


            Dim arr() As String = tempString.Split("-")

            If arr.Length <> 2 Then
                Return Nothing
            End If

            Dim dateString() As String = arr(0).Split({" "}, StringSplitOptions.RemoveEmptyEntries)

            Dim pDay As Integer = 0
            If Not Integer.TryParse(Trim(dateString(0)), pDay) Then
                Return Nothing
            End If

            Dim pMonth As Integer = GetMonth(dateString(1))
            If pMonth = 0 Then
                Return Nothing
            End If

            Dim pYear As Integer = 0
            If Not Integer.TryParse(Trim(dateString(2)), pYear) Then
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

        Catch ex As Exception
            Return Nothing
        End Try


    End Function

    Public Shared Function GetMonth(name As String) As Integer

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

#End Region

End Class
