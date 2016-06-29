Imports Common

Module Module1

    Sub Main()


        Dim reader As New BaseHttpScraper

        While True

            Dim i As Integer = reader.GetLastItemId
            i += 1

            Dim rawcontent As String = ""
            Console.Write(i & ": ")

            Try

                rawcontent = reader.Read(i)
                Console.Write("OK!")

            Catch ex As Exception

                Console.Write(ex.ToString)

            Finally

                reader.SetLastItemId(i)
                reader.Write(i, rawcontent)

                Console.WriteLine()

            End Try

        End While


    End Sub

End Module