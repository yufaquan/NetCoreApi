using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IService
{
    public interface IServiceBase<T>where T:class
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="t"></param>
        /// <returns>返回为空则表示新增失败</returns>
        T Add(T t);

        /// <summary>
        /// 批量新增 需要自己插入创建时间和创建人
        /// </summary>
        /// <param name="list"></param>
        /// <returns>False：失败；</returns>
        bool Add(List<T> list);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="t"></param>
        /// <returns>返回为空则表示修改失败</returns>
        T Edit(T t);

        /// <summary>
        /// 批量修改 需要自己添加修改时间和修改人
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool Edit(List<T> list);

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true：成功</returns>
        bool Delete(T t);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool Delete(List<T> list);

        /// <summary>
        /// 根据Id逻辑删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true：成功</returns>
        bool DeleteById(int id);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        bool Delete(List<int> idList);

        /// <summary>
        /// 根据主键id获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(int id);

        /// <summary>
        /// 根据条件获取
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        T GetBy(Expression<Func<T, bool>> whereExpression);

        /// <summary>
        /// 获取list
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        IList<T> GetAllList(Expression<Func<T, bool>> whereExpression);

        /// <summary>
        /// 获取分页list
        /// </summary>
        /// <param name="whereExpression">筛选条件</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页条数</param>
        /// <param name="total">筛选后总数据条数</param>
        /// <param name="orderByExpression">排序字段</param>
        /// <param name="orderByType">排序方式;Default:OrderByType.Asc</param>
        /// <returns></returns>
        IList<T> GetPageList(Expression<Func<T, bool>> whereExpression, int page, int limit, ref int total, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);

        /// <summary>
        /// 事务开始
        /// </summary>
        void BeginTran();
        /// <summary>
        /// 事务执行
        /// </summary>
        void CommitTran();
        /// <summary>
        /// 事务回滚
        /// </summary>
        void RollbackTran();

    }
}
