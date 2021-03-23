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
Public Class HttpBasicAuthenticationFilter
    Inherits AuthorizationFilterAttribute

    Public Overrides Sub OnAuthorization(ByVal actionContext As System.Web.Http.Controllers.HttpActionContext)
        If actionContext.ActionDescriptor.GetCustomAttributes(Of AllowAnonymousAttribute)().Count > 0 Then
            MyBase.OnAuthorization(actionContext)
            Return
        End If

        If Thread.CurrentPrincipal IsNot Nothing AndAlso Thread.CurrentPrincipal.Identity.IsAuthenticated Then
            MyBase.OnAuthorization(actionContext)
            Return
        End If

        Dim authParameter As String = Nothing
        Dim authValue = actionContext.Request.Headers.Authorization

        If authValue IsNot Nothing AndAlso authValue.Scheme = "Basic" Then
            authParameter = authValue.Parameter
        End If

        If String.IsNullOrEmpty(authParameter) Then
            Challenge(actionContext)
            Return
        End If

        authParameter = Encoding.[Default].GetString(Convert.FromBase64String(authParameter))
        Dim authToken = authParameter.Split(":"c)

        If authToken.Length < 2 Then
            Challenge(actionContext)
            Return
        End If

        If Not ValidateUser(authToken(0), authToken(1)) Then
            Challenge(actionContext)
            Return
        End If

        Dim principal = New GenericPrincipal(New GenericIdentity(authToken(0)), Nothing)
        Thread.CurrentPrincipal = principal

        If HttpContext.Current IsNot Nothing Then
            HttpContext.Current.User = principal
        End If

        MyBase.OnAuthorization(actionContext)
    End Sub
    Private Sub Challenge(ByVal actionContext As HttpActionContext)
        Dim host = actionContext.Request.RequestUri.DnsSafeHost
        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "请求未授权，拒绝访问。")
        actionContext.Response.Headers.WwwAuthenticate.Add(New AuthenticationHeaderValue("Basic", String.Format("realm=""{0}""", host)))
    End Sub

    Protected Overridable Function ValidateUser(ByVal userName As String, ByVal password As String) As Boolean
        Dim strUser As String = "", strPass As String = ""
        Dim rsRowLine() As DataRow
        Dim strSql As String = "SELECT  KeyName, KeyValue  FROM      tbl_Apps_AppSettings  WHERE   (KeyName = 'api.username' or KeyName='api.password' )"
        Dim SqlCnn2 As New SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("dbConn"))
        Dim sqlDa2 As New SqlClient.SqlDataAdapter(strSql, SqlCnn2)
        ' Dim sqlDs2 As New DataSet
        Dim rsTblData As DataTable = New DataTable("AuthTable")
        sqlDa2.Fill(rsTblData)
        rsRowLine = rsTblData.Select("KeyName = 'api.username'")
        If rsRowLine.Length >= 1 Then
            strUser = rsRowLine(0)("KeyValue") & ""
        End If

        rsRowLine = rsTblData.Select("KeyName = 'api.password'")
        If rsRowLine.Length >= 1 Then
            strPass = rsRowLine(0)("KeyValue") & ""
        End If
        sqlDa2.Dispose()
        SqlCnn2.Dispose()

        If userName.Equals(strUser, StringComparison.OrdinalIgnoreCase) AndAlso password.Equals(strPass) Then
            Return True
        End If
        Return False
    End Function






    'Public Overrides Sub OnAuthorization(ByVal actionContext As HttpActionContext)
    '        Dim authHeader = actionContext.Request.Headers.Authorization

    '        If authHeader IsNot Nothing Then
    '            Dim authenticationToken = actionContext.Request.Headers.Authorization.Parameter
    '            Dim decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken))
    '            Dim usernamePasswordArray = decodedAuthenticationToken.Split(":"c)
    '            Dim userName = usernamePasswordArray(0)
    '            Dim password = usernamePasswordArray(1)
    '            Dim isValid = userName = "andy" AndAlso password = "password"

    '            If isValid Then
    '                Dim principal = New GenericPrincipal(New GenericIdentity(userName), Nothing)
    '                Thread.CurrentPrincipal = principal
    '                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, "User " & userName & " successfully authenticated")
    '                Return
    '            End If
    '        End If

    '        HandleUnathorized(actionContext)
    '    End Sub

    '    Private Shared Sub HandleUnathorized(ByVal actionContext As HttpActionContext)
    '        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized)
    '        actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='Data' location = 'http://localhost:")
    '    End Sub


End Class
