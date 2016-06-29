Public Class BaseHttpScraper
    Implements IHttpScraper

    Private _rawHtmlFileIndex As String = "RawHtmlId.txt"
    Private _rawHtmlPath As String

    Public Sub New()

        _rawHtmlPath = GetRootPath() & "Raw\"

        'check raw folder 
        If Not System.IO.Directory.Exists(_rawHtmlPath) Then
            System.IO.Directory.CreateDirectory(_rawHtmlPath)
        End If

    End Sub

    Private Function GetRootPath() As String
        Return System.Configuration.ConfigurationManager.AppSettings.Item("ContentFolder")
    End Function

    Private Function GetRawHtmlPath() As String

        Dim out As String = GetRootPath() & "RawHtml\"

        If Not System.IO.Directory.Exists(out) Then
            System.IO.Directory.CreateDirectory(out)
        End If

        Return out

    End Function

    Public Function GetLastItemId() As Integer Implements IHttpScraper.GetLastItemId

        Dim filePath As String = GetIndexFilePath()

        If System.IO.File.Exists(filePath) Then
            Return CInt(Trim(System.IO.File.ReadAllText(filePath)))
        Else
            Return 1
        End If

    End Function

    Public Function GetIndexFilePath() As String
        Return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) & "\" & _rawHtmlFileIndex
    End Function

    Public Function Read(itemId As Integer) As Object Implements IHttpScraper.Read
        Return Helper.GetContent(String.Format(Me.BaseUrl, itemId))
    End Function

    Public Sub SetLastItemId(itemId As Integer) Implements IHttpScraper.SetLastItemId
        System.IO.File.WriteAllText(GetIndexFilePath, itemId)
    End Sub

    Public Sub Write(itemId As Integer, content As String) Implements IHttpScraper.Write
        Dim fullpath As String = _rawHtmlPath & itemId & ".txt"
        System.IO.File.WriteAllText(fullpath, content)
    End Sub

    Public ReadOnly Property BaseUrl As String Implements IHttpScraper.BaseUrl
        Get
            Return System.Configuration.ConfigurationManager.AppSettings.Item("BaseUrl")
        End Get
    End Property

End Class