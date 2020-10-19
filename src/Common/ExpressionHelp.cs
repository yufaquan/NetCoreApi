using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
            if (first == null && second == null)
            {
                return null;
            }
            else if (first == null && second != null)
            {
                return second;
            }
            else if (first != null && second == null)
            {
                return first;
            }
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
            if (first == null && second == null)
            {
                return null;
            }
            else if (first == null && second != null)
            {
                return second;
            }
            else if (first != null && second == null)
            {
                return first;
            }
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


        #region json
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
        /// 将json字符串转化为实体,如果字符串为空或者转换失败，则返回默认类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T JsonToModelOrDefault<T>(this string str) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return new T();
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception ex)
            {
                return new T();
            }
        }

        #endregion

        #region 时间

        /// <summary>
        /// 获取日期格式为yyyy/MM/dd HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToLongString(this DateTime dt)
        {
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        /// 获取日期格式为yyyy/MM/dd HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToLongString(this DateTime? dt)
        {
            if (dt.HasValue)
            {
                return dt.Value.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取日期格式为yyyy/MM/dd
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToShotString(this DateTime dt)
        {
            return dt.ToString("yyyy/MM/dd");
        }

        #endregion


        #region 字符串

        /// <summary>
        /// 判断字符串中是否存在某些字符
        /// </summary>
        /// <param name="str">要判断的字符串</param>
        /// <param name="arr">查询的数组字符</param>
        /// <param name="isqfdx">是否考虑大小写，默认不考虑</param>
        /// <returns>有一个存在则返回 True.</returns>
        public static bool IsHaveStr(this string str,string[] arr,bool isqfdx=false)
        {
            if (isqfdx)
            {
                foreach (var item in arr)
                {
                    if (str.Contains(item))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var item in arr)
                {
                    if (str.ToLower().Contains(item.ToLower()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断字符串中是否全部包含数组中的字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="arr"></param>
        /// <param name="isqfdx">师傅区分大小写</param>
        /// <returns></returns>
        public static bool IsHaveAllStr(this string str,string[] arr,bool isqfdx = false)
        {
            bool result=false;
            if (isqfdx)
            {
                foreach (var item in arr)
                {
                    if (str.Contains(item))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            else
            {
                foreach (var item in arr)
                {
                    if (str.ToLower().Contains(item.ToLower()))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 判断字符串是否等于其中一个字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="arr">要判断的字符串</param>
        /// <param name="isqfdx">是否区分大小写，默认不区分</param>
        /// <returns>只要有一个相等则返回 True.</returns>
        public static bool AsStrs(this string str, string[] arr, bool isqfdx = false)
        {
            if (isqfdx)
            {
                foreach (var item in arr)
                {
                    if (str == item)
                    {
                        return true;
                    }

                }
            }
            else
            {
                foreach (var item in arr)
                {
                    if (str.ToLower() == item.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 将字符串按特定字符切割成字符串数组
        /// </summary>
        /// <param name="str">需要切割的字符串</param>
        /// <param name="s">指定字符</param>
        /// <returns></returns>
        public static string[] ToArr(this string str, char s)
        {
            return str.Split(new char[] { s }, StringSplitOptions.RemoveEmptyEntries);
        }


        /// <summary>
        /// 将字符串按特定字符切割成字符串数组
        /// </summary>
        /// <param name="str">需要切割的字符串</param>
        /// <param name="s">指定字符</param>
        /// <returns></returns>
        public static List<string> ToList(this string str, char s)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return new List<string>();
            }
            return str.Split(new char[] { s }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }


        #endregion


        #region 集合
        /// <summary>
        /// 判断是否有交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="list2"></param>
        /// <returns>True：有交集；</returns>
        public static bool IsHaveSame<T>(this List<T> list,List<T> list2)
        {
            //查看两个集合是否有交集
            var newList=list.Intersect(list2);
            if (newList!=null && newList.Count()>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否包含集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="list2"></param>
        /// <returns>True：包含；</returns>
        public static bool IsSubset<T>(this List<T> list, List<T> list2)
        {
            bool result = false;
            foreach (var item in list2)
            {
                if (list.Contains(item))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }


        #endregion
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
