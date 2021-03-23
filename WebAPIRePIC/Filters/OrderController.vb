Imports System
Imports System.Net
Imports System.Text
Imports System.Web
Imports System.Web.Http.Controllers
Imports System.Web.Http.Filters
Imports System.Net.Http
Imports System.Web.Http
Imports System.Security.Principal
Imports System.Threading
Imports System.Net.Http.Headers
Imports System.IO
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Linq

Public Class OrderController
    Public Class OrderController
        Inherits ApiController



        <HttpGet>
        Public Function GetAll() As Object

            Dim strReValue As String = ""
            strReValue = HttpContext.Current.Request.UserHostAddress 'HttpContext.Current.Request.ServerVariables("REMOTE_ADDR") ' ' HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            Return strReValue & "Success"
        End Function

        <HttpGet>
        Public Function GetAll(ByVal id As String) As Object
            'Dim strReValue As String = ""
            'strReValue = HttpContext.Current.Request.UserHostAddress 'HttpContext.Current.Request.ServerVariables("REMOTE_ADDR") ' ' HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            'Return strReValue & "Success"

            Dim imgPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" & id & ".jpg")
            Dim imgByte = File.ReadAllBytes(imgPath)
            Dim imgStream = New MemoryStream(File.ReadAllBytes(imgPath))
            Dim resp = New HttpResponseMessage(HttpStatusCode.OK) With {
                .Content = New StreamContent(imgStream)
            }
            resp.Content.Headers.ContentType = New MediaTypeHeaderValue("image/jpg")
            Return resp

        End Function
        <HttpGet>
        Public Function GetAll(ByVal id As String, ByVal facetsid As String, ByVal id2 As String) As Object
            Dim strReValue As String = ""
            strReValue = HttpContext.Current.Request.UserHostAddress 'HttpContext.Current.Request.ServerVariables("REMOTE_ADDR") ' ' HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            Return strReValue & "Success"
        End Function

        <HttpGet>
        Public Function GetById(ByVal id As String, ByVal facetsid As String) As HttpResponseMessage
            Dim result As HttpResponseMessage
            Dim serializer As JavaScriptSerializer = New JavaScriptSerializer()

            Dim imgPath = System.Web.Hosting.HostingEnvironment.MapPath("~/") '  System.Web.Hosting.HostingEnvironment.MapPath("~/" & id & ".jpg")
            ' Dim str As String = serializer.Serialize(imgPath)
            Dim jsonStr As String
            Dim dict As New Dictionary(Of String, String)
            dict.Add("ImagePath", imgPath)
            jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(dict)

            result = New HttpResponseMessage With {
        .Content = New StringContent(jsonStr, Encoding.GetEncoding("UTF-8"), "application/json")
     }






            Return result
        End Function

        <HttpPost>
        Public Function PostById(ByVal id As String, ByVal facetsid As String) As HttpResponseMessage
            Dim result As HttpResponseMessage
            Dim serializer As JavaScriptSerializer = New JavaScriptSerializer()

            Dim imgPath = System.Web.Hosting.HostingEnvironment.MapPath("~/") '  System.Web.Hosting.HostingEnvironment.MapPath("~/" & id & ".jpg")
            ' Dim str As String = serializer.Serialize(imgPath)
            Dim jsonStr As String
            Dim dict As New Dictionary(Of String, String)
            dict.Add("ImagePath Post", imgPath)
            jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(dict)

            result = New HttpResponseMessage With {
        .Content = New StringContent(jsonStr, Encoding.GetEncoding("UTF-8"), "application/json")
     }
            Return result
        End Function



        <HttpPost>
        Public Function AddAnewdocument(ByVal Value As Object) As Int64
            Dim result As Int64 = 0
            Dim strError As String = ""
            Dim ddDocType As String = ""
            Dim objJson As JObject = New JObject()
            Dim indexNo As Integer = 0

            Dim dtIndex As DataTable
            Dim drIndex As DataRow
            Dim intDCN As Int64
            Dim strFileName As String
            dtIndex = IndexDt()
            objJson = JObject.Parse(Value.ToString())
            Dim jarA As JArray = New JArray()
            jarA = JArray.Parse(objJson("indexing").ToString())
            For Each StrValue In jarA
                indexNo = indexNo + 1
                drIndex = dtIndex.NewRow()
                drIndex("FieldNamer") = StrValue("key").ToString
                drIndex("Value") = StrValue("value").ToString
                drIndex("IndexNO") = indexNo
                dtIndex.Rows.Add(drIndex)
            Next

            'tbl_Web_Documents 的插入
            strFileName = objJson("fileType").ToString
            ddDocType = objJson("documentType").ToString
            intDCN = ADDDocumen(strFileName, objJson("fileContents"), 1, ddDocType, dtIndex, strError)

            Return intDCN
        End Function


        Private Function ADDDocumen(ByVal Filename As String, ByVal ImageData As String, ByVal ddQueue As Integer, ByVal ddDocType As Integer, ByVal dtIndex As DataTable, ByRef errString As String) As Int64
            Dim intId As Int64 = 0
            Dim sqlCmd2 As New SqlClient.SqlCommand
            Dim SqlCnn2 As New SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("dbConn"))
            Dim sqlDa2 As New SqlClient.SqlDataAdapter
            Dim sqlDs2 As New DataSet
            Dim strSql As String = ""

            'Dim b As Byte() = System.Text.Encoding.Default.GetBytes(ImageData)
            'Dim ImageBlob64 As String
            'ImageBlob64 = Convert.ToBase64String(b)
            'Dim ImageBlob As Byte() = System.Text.Encoding.Default.GetBytes(ImageBlob64)

            Dim strbase64 As String = ImageData.Trim().Substring(ImageData.IndexOf(",") + 1)
            Dim stream As MemoryStream = New MemoryStream(Convert.FromBase64String(strbase64))
            Dim b As Byte() = stream.ToArray()

            sqlCmd2.CommandTimeout = 0
            sqlCmd2.Connection = SqlCnn2
            sqlCmd2.CommandText = "prc_uploadDocument"
            sqlCmd2.CommandType = CommandType.StoredProcedure

            Filename = Replace(Filename, " ", "_")
            Filename = Replace(Filename, "'", "")
            sqlCmd2.Parameters.AddWithValue("@FileName", Filename)
            sqlCmd2.Parameters.AddWithValue("@ImageBlob", b)
            sqlCmd2.Parameters.AddWithValue("@QueueID", ddQueue)
            sqlCmd2.Parameters.AddWithValue("@DocType", ddDocType)
            sqlCmd2.Parameters.AddWithValue("@DocStatus", 1)
            sqlCmd2.Parameters.AddWithValue("@Priority", 2)

            sqlCmd2.Parameters.AddWithValue("@UploadedBy", "API")

            Try
                SqlCnn2.Open()
                sqlDa2.SelectCommand = sqlCmd2
                sqlDa2.Fill(sqlDs2)
                If sqlDs2.Tables(0).Rows.Count > 0 Then
                    Dim dr1 As DataRow
                    Dim dt1 As DataTable
                    dt1 = sqlDs2.Tables(0)
                    dr1 = dt1.Rows(0)
                    intId = dr1(0)
                    '更新文件名 用DCN
                    strSql = "UPDATE tbl_Web_Documents  SET           FileName ='" & intId & "." & Filename & "'   where dcn=" & intId
                    sqlCmd2 = New SqlClient.SqlCommand
                    sqlCmd2.CommandTimeout = 0
                    sqlCmd2.Connection = SqlCnn2
                    sqlCmd2.CommandText = strSql
                    sqlCmd2.ExecuteNonQuery()


                    Call UpdateValues(dtIndex, intId)
                    '''tbl_Web_DocumentAttributes 表的插入
                    ''Dim rsRowLineIndex() As DataRow
                    ''Dim strSql As String = "", strValueList As String = "", strFieldList As String = ""
                    ''rsRowLineIndex = dtIndex.Select("1=1")
                    ''For Each row In rsRowLineIndex
                    ''    strFieldList = strFieldList & row("FieldNamer") & ","
                    ''    strValueList = "'" & strValueList & row("Value") & "',"
                    ''Next
                    ''strFieldList = strFieldList & "DCN, QueueID, DocType"
                    ''strValueList = strValueList & intId & ",'" & ddQueue & "','" & ddDocType & "'"
                    ''strSql = "INSERT INTO tbl_Web_DocumentAttributes " & "(" & strFieldList & ") " & " VALUES (" & strValueList & ")"
                    ''sqlCmd2 = New SqlClient.SqlCommand
                    ''sqlCmd2.CommandTimeout = 0
                    ''sqlCmd2.Connection = SqlCnn2
                    ''sqlCmd2.CommandText = strSql
                    ''sqlCmd2.ExecuteNonQuery()


                    errString = ""
                End If

            Catch ex As SqlClient.SqlException
                errString = ex.Message
            Finally
                sqlCmd2.Dispose()
                sqlCmd2 = Nothing
                SqlCnn2.Close()
                SqlCnn2.Dispose()
                SqlCnn2 = Nothing
            End Try
            Return intId
        End Function



        Private Function UpdateValues(ByVal dtIndex As DataTable, ByVal intDcn As Int64) As Integer
            Dim intCount As Integer = 0
            Dim sqlCmd2 As New SqlClient.SqlCommand
            Dim SqlCnn2 As New SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("dbConn"))
            Dim strSql As String = String.Empty
            Dim rsRowLineIndex() As DataRow
            Dim strSQLWhere As String = ""
            Try
                rsRowLineIndex = dtIndex.Select("1=1")
                For Each row In rsRowLineIndex
                    strSQLWhere = strSQLWhere & row("FieldNamer") & "='" & row("Value") & "',"
                    intCount = intCount + 1
                Next

                SqlCnn2.Open()

                sqlCmd2 = New SqlClient.SqlCommand
                If strSQLWhere.Length >= 1 Then
                    strSQLWhere = Strings.Left(strSQLWhere, strSQLWhere.Length - 1)
                    strSql = " UPDATE tbl_Web_DocumentAttributes SET " & strSQLWhere & " WHERE dcn=" & intDcn
                    sqlCmd2.CommandTimeout = 0
                    sqlCmd2.Connection = SqlCnn2
                    sqlCmd2.CommandText = strSql
                    sqlCmd2.ExecuteNonQuery()
                End If
            Catch ex As Exception
                intCount = -1
            Finally
                sqlCmd2.Dispose()
                sqlCmd2 = Nothing
                SqlCnn2.Close()
                SqlCnn2.Dispose()
                SqlCnn2 = Nothing
            End Try
            Return intCount
        End Function


        'Public Function Base64StringToFile(ByVal base64String As String, ByVal fileName As String) As Boolean
        '    Dim opResult As Boolean = False

        '    Try
        '        Dim strDate As String = DateTime.Now.ToString("yyyyMMdd")
        '        Dim fileFullPath As String = "K:\images\test\b\UpFile\" & strDate

        '        If Not Directory.Exists(fileFullPath) Then
        '            Directory.CreateDirectory(fileFullPath)
        '        End If

        '        Dim strbase64 As String = base64String.Trim().Substring(base64String.IndexOf(",") + 1)
        '        Dim stream As MemoryStream = New MemoryStream(Convert.FromBase64String(strbase64))
        '        Dim fs As FileStream = New FileStream(fileFullPath & "\" & fileName, FileMode.OpenOrCreate, FileAccess.Write)
        '        Dim b As Byte() = stream.ToArray()
        '        fs.Write(b, 0, b.Length)
        '        fs.Close()
        '        opResult = True
        '    Catch e As Exception
        '    End Try

        '    Return opResult
        'End Function

        Private Function IndexDt() As DataTable
            Dim dtIndex As DataTable
            dtIndex = New DataTable("IndexTb")
            Dim dcIndex As DataColumn
            dcIndex = dtIndex.Columns.Add("FieldNamer", Type.GetType("System.String"))
            dcIndex = dtIndex.Columns.Add("Value", Type.GetType("System.String"))
            dcIndex = dtIndex.Columns.Add("IndexNO", Type.GetType("System.Int32"))
            Return dtIndex
        End Function

        Public Shared Function GetConfig(ByVal key As String, ByVal defaultValue As String) As String
            Dim val As String = defaultValue
            If ConfigurationManager.AppSettings.AllKeys.Contains(key) Then val = ConfigurationManager.AppSettings(key)
            If val Is Nothing Then val = defaultValue
            Return val
        End Function

        Private Sub CreatLog(ByVal strFolder As String, ByVal strFileName As String, ByVal strLog As String)
            Dim strLogFolder As String
            Try
                strLogFolder = strFolder & "\Log\"
                If Not My.Computer.FileSystem.FileExists(strLogFolder) Then
                    My.Computer.FileSystem.CreateDirectory(strLogFolder)
                End If
                Dim sw As StreamWriter = New StreamWriter(strLogFolder & strFileName, True) 'true is add 
                sw.WriteLine(strLog & vbTab & Now)
                sw.Flush()
                sw.Close()
                sw = Nothing
            Catch ex As Exception

            End Try

        End Sub

    End Class
End Class
