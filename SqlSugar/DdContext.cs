using Common;
using Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SqlSugar
{
    public class DbContext<T> where T : BaseModel, new()
    {
        public DbContext()
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
        public SimpleClient<T> CurrentDb { get { return new SimpleClient<T>(Db); } }//用来处理T表的常用操作


        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetList()
        {
            return CurrentDb.GetList(x=>x.IsDeleted==false);
        }
        public virtual List<T> GetList(Expression<Func<T, bool>> whereExpression)
        {
            return CurrentDb.GetList(whereExpression.ExpressionAnd(x => x.IsDeleted == false));
        }


        /// <summary>
        /// 根据表达式查询分页
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel pageModel)
        {
            return CurrentDb.GetPageList(whereExpression.ExpressionAnd( x => x.IsDeleted == false), pageModel);
        }

        /// <summary>
        /// 根据表达式查询分页并排序
        /// </summary>
        /// <param name="whereExpression">it</param>
        /// <param name="pageModel"></param>
        /// <param name="orderByExpression">it=>it.id或者it=>new{it.id,it.name}</param>
        /// <param name="orderByType">OrderByType.Desc</param>
        /// <returns></returns>
        public virtual List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel pageModel, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return CurrentDb.GetPageList(whereExpression.ExpressionAnd( x => x.IsDeleted == false), pageModel, orderByExpression, orderByType);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">条件筛选</param>
        /// <param name="pageModel">分页数据</param>
        /// <param name="totalCount">筛选后的总数据</param>
        /// <param name="orderByExpression">排序字段 it=>it.id或者it=>new{it.id,it.name}</param>
        /// <param name="orderByType">排序分类 默认OrderByType.Asc</param>
        /// <returns></returns>
        public virtual List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel pageModel, ref int totalCount, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return Db.Queryable<T>()
                .Where(whereExpression.ExpressionAnd(x => x.IsDeleted == false))
                .OrderBy(orderByExpression, orderByType).ToPageList(pageModel.PageIndex, pageModel.PageSize, ref totalCount);
        }

        public virtual T GetById(int id)
        {
            return CurrentDb.GetSingle(x => x.Id == id && x.IsDeleted==false);
        }

        public virtual T GetSingle(Expression<Func<T, bool>> whereExpression)
        {
            var exp = whereExpression.ExpressionAnd(x => x.IsDeleted == false);
            return CurrentDb.GetSingle(exp);
        }


        /// <summary>
        /// 根据主键删除 物理删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete_(decimal id)
        {
            return CurrentDb.DeleteById(id);
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(int id)
        {
            var t = GetById(id);
            t.IsDeleted = true;
            return CurrentDb.Update(t);
        }


        /// <summary>
        /// 根据实体更新，实体需要有主键
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Update(T obj)
        {
            obj.ModifiedAt = DateTime.Now;
            obj.ModifiedBy = Current.CurrentUserId;
            return CurrentDb.Update(obj);
        }

        /// <summary>
        ///批量更新 需要自己添加修改时间和修改人
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Update(List<T> objs)
        {
            return CurrentDb.UpdateRange(objs);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int Insert(T obj)
        {
            obj.CreatedAt = DateTime.Now;
            obj.CreatedBy = Current.CurrentUserId;
            return CurrentDb.InsertReturnIdentity(obj);
        }


        /// <summary>
        /// 批量 需要自己插入创建时间和创建人
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Insert(List<T> objs)
        {
            return CurrentDb.InsertRange(objs);
        }

        /// <summary>
        /// 事务开始
        /// </summary>
        public void BeginTran()
        {
            Db.Ado.BeginTran();
        }
        /// <summary>
        /// 事务执行
        /// </summary>
        public void CommitTran()
        {
            Db.Ado.CommitTran();
        }
        /// <summary>
        /// 事务回滚
        /// </summary>
        public void RollbackTran()
        {
            Db.Ado.RollbackTran();
        }

    }


}