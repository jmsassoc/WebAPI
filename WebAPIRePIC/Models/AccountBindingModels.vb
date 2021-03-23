Imports System.ComponentModel.DataAnnotations
Imports Newtonsoft.Json

' 用作 AccountController 操作的参数的模型。

Public Class AddExternalLoginBindingModel
    <Required>
    <Display(Name := "外部访问令牌")>
    Public Property ExternalAccessToken As String
End Class

Public Class ChangePasswordBindingModel
    <Required>
    <DataType(DataType.Password)>
    <Display(Name := "当前密码")>
    Public Property OldPassword As String

    <Required>
    <StringLength(100, ErrorMessage := "{0} 必须至少包含 {2} 个字符。", MinimumLength := 6)>
    <DataType(DataType.Password)>
    <Display(Name := "新密码")>
    Public Property NewPassword As String

    <DataType(DataType.Password)>
    <Display(Name := "确认新密码")>
    <Compare("NewPassword", ErrorMessage := "新密码和确认密码不匹配。")>
    Public Property ConfirmPassword As String
End Class

Public Class RegisterBindingModel
    <Required>
    <Display(Name := "电子邮件")>
    Public Property Email As String

    <Required>
    <StringLength(100, ErrorMessage := "{0} 必须至少包含 {2} 个字符。", MinimumLength := 6)>
    <DataType(DataType.Password)>
    <Display(Name := "密码")>
    Public Property Password As String

    <DataType(DataType.Password)>
    <Display(Name := "确认密码")>
    <Compare("Password", ErrorMessage := "密码和确认密码不匹配。")>
    Public Property ConfirmPassword As String
End Class

Public Class RegisterExternalBindingModel
    <Required>
    <Display(Name := "电子邮件")>
    Public Property Email As String
End Class

Public Class RemoveLoginBindingModel
    <Required>
    <Display(Name := "登录提供程序")>
    Public Property LoginProvider As String

    <Required>
    <Display(Name := "提供程序密钥")>
    Public Property ProviderKey As String
End Class

Public Class SetPasswordBindingModel
    <Required>
    <StringLength(100, ErrorMessage := "{0} 必须至少包含 {2} 个字符。", MinimumLength := 6)>
    <DataType(DataType.Password)>
    <Display(Name := "新密码")>
    Public Property NewPassword As String

    <DataType(DataType.Password)>
    <Display(Name := "确认新密码")>
    <Compare("NewPassword", ErrorMessage := "新密码和确认密码不匹配。")>
    Public Property ConfirmPassword As String
End Class
