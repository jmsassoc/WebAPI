Imports System.Net.Http
Imports System.Web.Http
Imports Microsoft.Owin.Security.OAuth
Imports Newtonsoft.Json.Serialization

Public Module WebApiConfig
    Public Sub Register(config As HttpConfiguration)
        ' Web API 配置和服务
        '' 将 Web API 配置为仅使用不记名令牌身份验证。
        'config.SuppressDefaultHostAuthentication()
        'config.Filters.Add(New HostAuthenticationFilter(OAuthDefaults.AuthenticationType))

        ' Web API 路由
        config.MapHttpAttributeRoutes()

        '''http://localhost:4230/api/Order
        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="api/{controller}/{id}",
            defaults:=New With {.id = RouteParameter.Optional}
        )

        ''http://localhost:4230/testapi/Order/aa
        config.Routes.MapHttpRoute(
            name:="Api",
            routeTemplate:="api/{controller}/{id}/{facetsid}/{id2}",
            defaults:=New With {.id = RouteParameter.Optional, .facetsid = RouteParameter.Optional, .id2 = RouteParameter.Optional}
        )
        'defaults:=New With {.id = RouteParameter.Optional, .ordertype = "facetsid", .id2 = RouteParameter.Optional}
        'routes.MapHttpRoute("AllRows", "api/{controller}/{action}/{userName}/{tableName}",

        ''http://localhost:4230/actionapi/Order/GetAll
        config.Routes.MapHttpRoute(
            name:="ActionApi",
            routeTemplate:="actionapi/{controller}/{action}/{id}",
            defaults:=New With {.id = RouteParameter.Optional}
        )


        config.Filters.Add(New HttpBasicAuthenticationFilter())
    End Sub



End Module
