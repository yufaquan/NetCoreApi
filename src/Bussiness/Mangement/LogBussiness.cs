using Common;
using Entity;
using IService;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Mangement
{
   public class LogBussiness
    {
        private LogService _service;
        public LogBussiness()
        {
            _service = ServiceHelp.GetLogService;
        }
        public static LogBussiness Init { get => new LogBussiness(); }


        /// <summary>
        /// 获取API数据
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页多少个</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public List<LogAPI> GetAPIPageList(LogAPI filter, int page, int limit, ref int total)
        {
            System.Linq.Expressions.Expression<Func<LogAPI, bool>> where = null;
            if (!string.IsNullOrWhiteSpace(filter.From))
            {
                where = where.ExpressionAnd(x => x.From.Contains(filter.From));
            }
            return _service.GetAPIPageList(where, page, limit, ref total, x => x.StartTime, SqlSugar.OrderByType.Desc).ToList();
        }

        /// <summary>
        /// 获取事件数据
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页多少个</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public List<LogEvent> GetEventPageList(LogEvent filter, int page, int limit, ref int total)
        {
            System.Linq.Expressions.Expression<Func<LogEvent, bool>> where = null;
            if (!string.IsNullOrWhiteSpace(filter.Content))
            {
                where = where.ExpressionAnd(x => x.Content.Contains(filter.Content));
            }
            return _service.GetEventPageList(where, page, limit, ref total, x => x.WriteDate, SqlSugar.OrderByType.Desc).ToList();
        }

        /// <summary>
        /// 获取错误数据
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页多少个</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public List<LogError> GetErrorPageList(LogError filter, int page, int limit, ref int total)
        {
            System.Linq.Expressions.Expression<Func<LogError, bool>> where = null;
            if (!string.IsNullOrWhiteSpace(filter.UserName))
            {
                where = where.ExpressionAnd(x => x.UserName.Contains(filter.UserName));
            }
            return _service.GetErrorPageList(where, page, limit, ref total, x => x.Time, SqlSugar.OrderByType.Desc).ToList();
        }

    }
}
