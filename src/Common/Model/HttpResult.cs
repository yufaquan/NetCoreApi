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

    /// <summary>
    /// 系统错误
    /// </summary>
    public static HttpResult Error = new HttpResult() { Code = (int)HttpResultCode.Error, Message = "小单跑丢了，您可以尝试返回重试哦。一直找不到记得联系管理员呢。" };
    /// <summary>
    /// 没有权限
    /// </summary>
    public static HttpResult NotAuth = new HttpResult() { Code = (int)HttpResultCode.NotAuth, Message = "您暂时没有权限访问哦！" };
    /// <summary>
    /// 调用成功
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static HttpResult Success(object data)
    {
        return new HttpResult() { Code = (int)HttpResultCode.Success, Message = "OK!", Data= data };
    }
    public static HttpResult Success( string message, object data)
    {
        return new HttpResult() { Code = (int)HttpResultCode.Success, Message = message, Data = data };
    }
    public static HttpResult Success(HttpResultCode code,object data)
    {
        return new HttpResult() { Code = (int)code, Message = code.GetDisplayName(), Data = data };
    }
    public static HttpResult Success(HttpResultCode code,string message, object data)
    {
        return new HttpResult() { Code = (int)code, Message = message, Data = data };
    }
    /// <summary>
    /// 用户token校验失败
    /// </summary>
    /// <param name="data"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static HttpResult UserTokenFail(object data,string errorMessage)
    {
        return new HttpResult() { Code = (int)HttpResultCode.Success, Message = errorMessage, Data = data };
    }
}

/// <summary>
/// HTTP返回的Code枚举
/// </summary>
public enum HttpResultCode
{
    [Display(Name = "成功")]
    Success =0,
    [Display(Name = "用户Token校验失败")]
    UserTokenError = 0,
    [Display(Name = "无权访问")]
    NotAuth = 401,
    [Display(Name = "系统错误")]
    Error = 500,
    /// <summary>
    /// 新增失败
    /// </summary>
    [Display(Name = "新增失败")]
    AddFail = 1
}