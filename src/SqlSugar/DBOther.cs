using Common;
using System;
using System.Linq;

namespace SqlSugar
{
    public class DBOther
    {
        public DBOther()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.MysqlConnectionStrng,
                DbType = DbType.MySql,
                InitKeyType = InitKeyType.SystemTable,
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了

            });
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                var text = sql + "\r\n" +
                    Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value));
                Console.WriteLine(text);
                //Console.WriteLine();
            };

        }
        //注意：不能写成静态的
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作


        public static DBOther Init { get => new DBOther(); }

        

    }
}
