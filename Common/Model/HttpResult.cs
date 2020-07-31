using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text;

/// <summary>
/// httpResult
/// </summary>
public class HttpResult
{
    public string Message { get; set; }
    public int Code { get; set; }

    public object Data { get; set; }

    public static HttpResult Error = new HttpResult() { Code = 500, Message = "啊哦，一不小心走丢了，试着返回再来哦，多次走丢记得联系管理员呀。" };
    public static HttpResult NotAuth = new HttpResult() { Code = 401, Message = "您暂时没有权限访问哦！" };
    public static HttpResult Success(object data)
    {
        return new HttpResult() { Code = (int)HttpResultCode.Success, Message = "OK!", Data= data };
    }
}

/// <summary>
/// HTTP返回的Code枚举
/// </summary>
public enum HttpResultCode
{
    [Display(Name = "成功")]
    Success =0
}