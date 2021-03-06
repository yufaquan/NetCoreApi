﻿using Common;
using Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        #region 操作日志
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
        /// 异步记录登录操作日志
        /// </summary>
        /// <returns></returns>
        public async Task WriteEventLogToLoginAsync()
        {
            LogEvent log = new LogEvent();
            log.EventType = Enums.EventType.Login;
            User user = Current.UserJson.JsonToModel<User>();
            var from = string.Empty;
            Config.VisitTos.TryGetValue(Current.VisitToKey, out from);
            log.Content = $"用户{user.Name}({user.NickName})在[{from}]登录了系统。";
            log.UserId = user.Id;
            log.UserName = user.Name;
            log.WriteDate = DateTime.Now;
            await Task.Run(() =>
            {
                Db.Insertable(log).ExecuteCommand();
            });
        }
        public async Task WriteEventLogToLogOutAsync()
        {
            LogEvent log = new LogEvent();
            log.EventType = Enums.EventType.LogOut;
            User user = Current.UserJson.JsonToModel<User>();
            var from = string.Empty;
            Config.VisitTos.TryGetValue(Current.VisitToKey, out from);
            log.Content = $"用户{user.Name}({user.NickName})在[{from}]登出了系统。";
            log.UserId = user.Id;
            log.UserName = user.Name;
            log.WriteDate = DateTime.Now;
            await Task.Run(() =>
            {
                Db.Insertable(log).ExecuteCommand();
            });
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        /// <param name="t">创建数据的实体</param>
        /// <param name="id">创建的数据ID</param>
        /// <param name="data">创建的数据json</param>
        /// <returns></returns>
        public async Task WriteEventLogCreateAsync(Type t,int id,string data)
        {
            LogEvent log = new LogEvent();
            log.EventType = Enums.EventType.Insert;
            User user = Current.UserJson.JsonToModel<User>();
            var from = string.Empty;
            Config.VisitTos.TryGetValue(Current.VisitToKey, out from);
            log.Content = $"用户{user.Name}[{user.NickName}]在[{from}]创建了[{t.GetDisplayName()}]数据，数据ID为：{id}。";
            log.UserId = user.Id;
            log.UserName = user.Name;
            log.Remark = data;
            log.WriteDate = DateTime.Now;
            await Task.Run(() =>
            {
                Db.Insertable(log).ExecuteCommand();
            });
        }


        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="t">修改数据的实体</param>
        /// <param name="id">修改的数据ID</param>
        /// <param name="data">修改后的数据json</param>
        /// <returns></returns>
        public async Task WriteEventLogEditAsync(Type t, int id, string data)
        {
            LogEvent log = new LogEvent();
            log.EventType = Enums.EventType.Update;
            User user = Current.UserJson.JsonToModel<User>();
            var from = string.Empty;
            Config.VisitTos.TryGetValue(Current.VisitToKey, out from);
            log.Content = $"用户{user.Name}[{user.NickName}]在[{from}]修改了[{t.GetDisplayName()}]数据，数据ID为：{id}。(记录修改后的数据)";
            log.UserId = user.Id;
            log.UserName = user.Name;
            log.Remark = data;
            log.WriteDate = DateTime.Now;
            await Task.Run(() =>
            {
                Db.Insertable(log).ExecuteCommand();
            });
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="t">删除数据的实体</param>
        /// <param name="id">删除的数据ID</param>
        /// <returns></returns>
        public async Task WriteEventLogDeleteAsync(Type t, int id)
        {
            LogEvent log = new LogEvent();
            log.EventType = Enums.EventType.Delete;
            User user = Current.UserJson.JsonToModel<User>();
            var from = string.Empty;
            Config.VisitTos.TryGetValue(Current.VisitToKey, out from);
            log.Content = $"用户{user.Name}[{user.NickName}]在[{from}]删除了[{t.GetDisplayName()}]数据，数据ID为：{id}。";
            log.UserId = user.Id;
            log.UserName = user.Name;
            log.WriteDate = DateTime.Now;
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
        public async Task WriteOneEventLogAsync<T>(T t, Enums.EventType type, string remark) where T : BaseModel
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
            if (log != null)
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


        #endregion


        /// <summary>
        /// 获取API日志分页list
        /// </summary>
        /// <param name="whereExpression">筛选条件</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页条数</param>
        /// <param name="total">筛选后总数据条数</param>
        /// <param name="orderByExpression">排序字段</param>
        /// <param name="orderByType">排序方式;Default:OrderByType.Asc</param>
        /// <returns></returns>
        public IList<LogAPI> GetAPIPageList(Expression<Func<LogAPI, bool>> whereExpression, int page, int limit, ref int total, Expression<Func<LogAPI, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            if (whereExpression==null)
            {
                return Db.Queryable<LogAPI>().OrderBy(orderByExpression, orderByType).ToPageList(page, limit, ref total);
            }
            return Db.Queryable<LogAPI>().Where(whereExpression).OrderBy(orderByExpression,orderByType).ToPageList(page, limit, ref total);
        }

        /// <summary>
        /// 获取错误日志分页list
        /// </summary>
        /// <param name="whereExpression">筛选条件</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页条数</param>
        /// <param name="total">筛选后总数据条数</param>
        /// <param name="orderByExpression">排序字段</param>
        /// <param name="orderByType">排序方式;Default:OrderByType.Asc</param>
        /// <returns></returns>
        public IList<LogError> GetErrorPageList(Expression<Func<LogError, bool>> whereExpression, int page, int limit, ref int total, Expression<Func<LogError, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            if (whereExpression == null)
            {
                return Db.Queryable<LogError>().OrderBy(orderByExpression, orderByType).ToPageList(page, limit, ref total);
            }
            return Db.Queryable<LogError>().Where(whereExpression).OrderBy(orderByExpression, orderByType).ToPageList(page, limit, ref total);
        }

        /// <summary>
        /// 获取操作日志分页list
        /// </summary>
        /// <param name="whereExpression">筛选条件</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页条数</param>
        /// <param name="total">筛选后总数据条数</param>
        /// <param name="orderByExpression">排序字段</param>
        /// <param name="orderByType">排序方式;Default:OrderByType.Asc</param>
        /// <returns></returns>
        public IList<LogEvent> GetEventPageList(Expression<Func<LogEvent, bool>> whereExpression, int page, int limit, ref int total, Expression<Func<LogEvent, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            if (whereExpression == null)
            {
                return Db.Queryable<LogEvent>().OrderBy(orderByExpression, orderByType).ToPageList(page, limit, ref total);
            }
            return Db.Queryable<LogEvent>().Where(whereExpression).OrderBy(orderByExpression, orderByType).ToPageList(page, limit, ref total);
        }

    }
}
