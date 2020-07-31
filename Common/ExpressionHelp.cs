using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Common
{
    public static class ExpressionHelp
    {
        #region 拉姆达表达式
        private static Expression<T> Combine<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            MyExpressionVisitor visitor = new MyExpressionVisitor(first.Parameters[0]);
            Expression bodyone = visitor.Visit(first.Body);
            Expression bodytwo = visitor.Visit(second.Body);
            return Expression.Lambda<T>(merge(bodyone, bodytwo), first.Parameters[0]);
        }
        /// <summary>
        /// 拉姆达表达式 and 合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ExpressionAnd<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.And);
        }
        /// <summary>
        /// 拉姆达表达式 or 合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ExpressionOr<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.Or);
        }

        #endregion


        #region http
        /// <summary>
        /// 权限验证失败时返回
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public static void AuthFailed(this HttpResponse httpResponse)
        {
            //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换
            var payload = HttpResult.NotAuth.ToJsonString();
            //自定义返回的数据类型
            httpResponse.ContentType = "application/json";
            //自定义返回状态码，默认为401 我这里改成 200
            httpResponse.StatusCode = StatusCodes.Status401Unauthorized;
            //输出Json数据结果
            httpResponse.WriteAsync(payload);
        }

        public static void AuthFailed(this HttpContext httpContext)
        {
            //httpContext.Response.Clear();
            httpContext.Response.AuthFailed();
        }
        #endregion

        /// <summary>
        /// 转化为json字符串 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将json字符串转化为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T JsonToModel<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// 获取日期格式为yyyy/MM/dd HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToLongString(this DateTime dt)
        {
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }


      

    }

    public class MyExpressionVisitor : ExpressionVisitor
    {
        public ParameterExpression _Parameter { get; set; }

        public MyExpressionVisitor(ParameterExpression Parameter)
        {
            _Parameter = Parameter;
        }
        protected override Expression VisitParameter(ParameterExpression p)
        {
            return _Parameter;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);//Visit会根据VisitParameter()方法返回的Expression修改这里的node变量
        }
    }


    /// <summary>
    /// 枚举扩展方法
    /// </summary>
    public static class EnumExtension
    {
        private static Dictionary<string, Dictionary<string, string>> _enumCache;

        /// <summary>
        /// 缓存
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> EnumCache
        {
            get { return _enumCache ?? (_enumCache = new Dictionary<string, Dictionary<string, string>>()); }
            set { _enumCache = value; }
        }

        /// <summary>
        /// 获取枚举描述信息
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GetDisplayName(this System.Enum en)
        {
            string enString = string.Empty;
            if (null == en) return enString;

            Type type = en.GetType();
            enString = en.ToString();
            if (!EnumCache.ContainsKey(type.FullName+"name"))
            {
                System.Reflection.FieldInfo[] fields = type.GetFields();
                Dictionary<string, string> temp = new Dictionary<string, string>();
                foreach (FieldInfo item in fields)
                {
                    object[] attrs = item.GetCustomAttributes(typeof(DisplayAttribute), false);
                    if (attrs.Length == 1)
                    {
                        string v = ((DisplayAttribute)attrs[0]).Name;
                        temp.Add(item.Name, v);
                    }
                }
                EnumCache.Add(type.FullName + "name", temp);
            }
            if (EnumCache[type.FullName + "name"].ContainsKey(enString))
            {
                return EnumCache[type.FullName + "name"][enString];
            }
            return enString;
        }

        /// <summary>
        /// 遍历枚举对象的所有元素
        /// </summary>
        /// <typeparam name="T">枚举对象</typeparam>
        /// <returns>Dictionary：枚举值-描述</returns>
        public static Dictionary<int, string> GetEnumValues<T>()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            foreach (var code in System.Enum.GetValues(typeof(T)))
            {
                ////获取名称
                //string strName = System.Enum.GetName(typeof(T), code);

                object[] objAttrs = code.GetType().GetField(code.ToString()).GetCustomAttributes(typeof(DisplayAttribute), true);
                if (objAttrs.Length > 0)
                {
                    DisplayAttribute descAttr = objAttrs[0] as DisplayAttribute;
                    if (!dictionary.ContainsKey((int)code))
                    {
                        if (descAttr != null) dictionary.Add((int)code, descAttr.Name);
                    }
                    //Console.WriteLine(string.Format("[{0}]", descAttr.Value));
                }
                //Console.WriteLine(string.Format("{0}={1}", code.ToString(), Convert.ToInt32(code)));
            }
            return dictionary;
        }

    }




}
