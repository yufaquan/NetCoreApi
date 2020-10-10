using Common;
using Entity;
using IService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Mangement
{
   public class ConfigurationBussiness
    {
        private ISysConfigurationService _service;
        public ConfigurationBussiness()
        {
            _service = ServiceHelp.GetSysConfigurationService;
        }
        public static ConfigurationBussiness Init { get => new ConfigurationBussiness(); }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public Configuration Get(HttpContext httpContext)
        {
            var result = new Configuration();
            //获取list
            var list = _service.GetAllList(null);
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (var item in list)
            {
                data.Add(item.Key, item.Value);
            }
            Type t = typeof(Configuration);
            foreach (PropertyInfo p in t.GetProperties())
            {
                if (data.ContainsKey(p.Name))
                {
                    #region 根据类型进行赋值
                    if (p.PropertyType.FullName == "System.Int32")
                    {
                        p.SetValue(result, Convert.ToInt32(data[p.Name]));
                    }else if (p.PropertyType.FullName == "System.Boolean")
                    {
                        p.SetValue(result, Convert.ToBoolean(data[p.Name]));
                    }
                    else if (p.PropertyType.FullName == "System.Double")
                    {
                        p.SetValue(result, Convert.ToDouble(data[p.Name]));
                    }
                    else if (p.PropertyType.FullName == "System.Decimal")
                    {
                        p.SetValue(result, Convert.ToDecimal(data[p.Name]));
                    }
                    else if (p.PropertyType.FullName == "System.DateTime")
                    {
                        p.SetValue(result, Convert.ToDateTime(data[p.Name]));
                    }
                    else if (p.PropertyType.FullName == "System.Single")
                    {
                        p.SetValue(result, Convert.ToSingle(data[p.Name]));
                    }
                    else
                    {
                        try
                        {
                            p.SetValue(result, data[p.Name]);
                        }
                        catch (Exception ex)
                        {
                            Task task1 = ServiceHelp.GetLogService.WriteErrorLogAsync(new LogError(ex, httpContext));
                        }
                    }
                    #endregion
                }
            }
            //记录操作日志
            Task task = ServiceHelp.GetLogService.WriteOneEventLogAsync(new LogEvent() {Content= $"用户\"{Current.UserJson.JsonToModelOrDefault<User>().Name}\"查看了系统配置。" });
            return result;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public bool Set(Configuration configuration, HttpContext httpContext)
        {
            var result = new Configuration();
            Dictionary<string, object> data = new Dictionary<string, object>();
            Type t = typeof(Configuration);
            foreach (PropertyInfo p in t.GetProperties())
            {
                data.Add(p.Name, p.GetValue(configuration));
            }
            var list = _service.GetAllList(null);
            var addlist = new List<SysConfiguration>();
            var editlist= new List<SysConfiguration>();
            DateTime dt = DateTime.Now;
            foreach (var item in data)
            {
                var flist = list.ToList().Find(x => x.Key == item.Key);
                if (flist!=null)
                {
                    flist.Value = item.Value;
                    flist.ModifiedAt = dt;
                    flist.ModifiedBy = Current.UserId;
                    editlist.Add(flist);
                }
                else
                {
                    var c = new SysConfiguration();
                    c.Key = item.Key;
                    c.Value = item.Value;
                    c.CreatedAt = dt;
                    c.CreatedBy = Current.UserId;
                    addlist.Add(c);
                }
            }
            try
            {
                _service.BeginTran();
                if (addlist.Count > 0)
                {
                    _service.Add(addlist);
                }
                if (editlist.Count > 0)
                {
                    _service.Edit(editlist);
                }
                _service.CommitTran();
                //记录操作日志
                Task task = ServiceHelp.GetLogService.WriteOneEventLogAsync(addlist.Concat(editlist).ToList(), Enums.EventType.Update,"");
                return true;
            }
            catch (Exception ex)
            {
                _service.RollbackTran();
                Task task = ServiceHelp.GetLogService.WriteErrorLogAsync(new LogError(ex, httpContext));
                return false;
            }
        }
    }
}
