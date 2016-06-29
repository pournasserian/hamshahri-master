Public Interface IHttpScraper

    Function Read(itemId As Integer)
    Sub Write(itemId As Integer, content As String)
    Function GetLastItemId() As Integer
    Sub SetLastItemId(itemId As Integer)

    ReadOnly Property BaseUrl As String

End Interface


