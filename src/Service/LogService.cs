﻿using Common;
using Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class LogService:DBOther
    {
        public LogService()
        {
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="log"></param>
        public void WriteErrorLog(LogError log)
        {
            Db.Insertable(log).ExecuteCommand();
        }
        /// <summary>
        /// 错误日志 （异步）
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task WriteErrorLogAsync(LogError log)
        {
            await Task.Run(() =>
            {
                Db.Insertable(log).ExecuteCommand();
            });
        }
        /// <summary>
        /// 错误日志（异步）
        /// </summary>
        /// <param name="ex">错误信息</param>
        /// <param name="logLevel">日志等级</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public async Task WriteErrorLogAsync(Exception ex,Enums.LogLevel logLevel,string errorMessage)
        {
            LogError error = new LogError();
            error.LogLevel = logLevel;
            error.MsgType = ex.GetType().Name;
            error.Message = errorMessage + "--系统提示："+ ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            error.StackTrace = ex.StackTrace.Length > 300 ? ex.StackTrace.Substring(0, 300) : ex.StackTrace;
            error.Source = ex.Source;
            error.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            error.Assembly = ex.TargetSite.Module.Assembly.FullName;
            error.Method = ex.TargetSite.Name;
            await Task.Run(() =>
            {
                Db.Insertable(error).ExecuteCommand();
            });
        }

        /// <summary>
        /// 记录API访问日志
        /// </summary>
        /// <param name="log"></param>
        public void WriteApiLog(LogAPI log)
        {
            Db.Insertable(log).ExecuteCommand();
        }

        /// <summary>
        /// 异步记录API访问日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task WriteApiLogAsync(LogAPI log)
        {
            await Task.Run(() =>
            {
                Db.Insertable(log).ExecuteCommand();
            });
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="log"></param>
        public void WriteEventLog(LogEvent log)
        {
            Db.Insertable(log).ExecuteCommand();
        }

        /// <summary>
        /// 异步记录操作日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task WriteEventLogAsync(LogEvent log)
        {
            await Task.Run(() =>
            {
                Db.Insertable(log).ExecuteCommand();
            });
        }

        /// <summary>
        /// 单条数据的操作
        /// </summary>
        /// <typeparam name="T">必须继承BaseModel的实体类</typeparam>
        /// <param name="t">实体信息</param>
        /// <param name="type">操作类型</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public async Task WriteOneEventLogAsync<T>(T t,Enums.EventType type,string remark)where T:BaseModel
        {
            var log = new LogEvent();
            log.UserId = Current.UserId.HasValue ? Current.UserId.Value : 0;
            log.UserName = Current.UserJson.JsonToModelOrDefault<User>().Name;
            log.Content = $"用户\"{log.UserName}\"{type.GetDisplayName()}了一条[{typeof(T).GetDisplayName()}]数据，数据ID：{t.Id}。";
            log.EventType = type;
            log.Remark = remark;
            log.WriteDate = DateTime.Now;
            await Task.Run(() =>
            {
                Db.Insertable(log).ExecuteCommand();
            });
        }


        /// <summary>
        /// 多条数据的操作
        /// </summary>
        /// <typeparam name="T">必须继承BaseModel的实体类</typeparam>
        /// <param name="t">实体信息</param>
        /// <param name="type">操作类型</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public async Task WriteOneEventLogAsync<T>(List<T> t, Enums.EventType type, string remark) where T : BaseModel
        {
            var log = new LogEvent();
            log.UserId = Current.UserId.HasValue ? Current.UserId.Value : 0;
            log.UserName = Current.UserJson.JsonToModelOrDefault<User>().Name;
            log.Content = $"用户\"{log.UserName}\"批量{type.GetDisplayName()}了[{typeof(T).GetDisplayName()}]数据。";
            log.EventType = type;
            log.Remark = remark;
            log.WriteDate = DateTime.Now;
            await Task.Run(() =>
            {
                Db.Insertable(log).ExecuteCommand();
            });
        }

        /// <summary>
        /// 创建操作记录
        /// </summary>
        /// <param name="log">操作信息（不能为空）</param>
        /// <returns></returns>
        public async Task WriteOneEventLogAsync(LogEvent log)
        {
            if (log!=null)
            {
                log.UserId = Current.UserId.HasValue ? Current.UserId.Value : 0;
                log.UserName = Current.UserJson.JsonToModelOrDefault<User>().Name;
                log.WriteDate = DateTime.Now;
                await Task.Run(() =>
                {
                    Db.Insertable(log).ExecuteCommand();
                });
            }
        }


    }
}