using Common;
using Entity;
using IService;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Service
{
    public class SysConfigurationService :  ISysConfigurationService
    {
        private DbContext<SysConfiguration> db;
        public SysConfigurationService()
        {
            db = new DbContext<SysConfiguration>();
        }
        public SysConfiguration Add(SysConfiguration t)
        {
            return db.InsertT(t);
        }

        public bool Add(List<SysConfiguration> list)
        {
            if (list!=null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].CreatedAt = DateTime.Now;
                    list[i].CreatedBy = Current.UserId;
                }
                return db.Insert(list);
            }
            return false;
        }

        public void BeginTran()
        {
            db.BeginTran();
        }

        public void CommitTran()
        {
            db.CommitTran();
        }

        public bool Delete(SysConfiguration t)
        {
            return db.Delete(t.Id);
        }

        public bool Delete(List<SysConfiguration> list)
        {
            return db.Delete(list);
        }

        public bool Delete(List<int> idList)
        {
            return db.Delete(idList);
        }

        public bool DeleteById(int id)
        {
            return db.Delete(id);
        }

        public SysConfiguration Edit(SysConfiguration t)
        {
            return db.UpdateT(t);
        }

        public bool Edit(List<SysConfiguration> list)
        {
            return db.Update(list);
        }

        public IList<SysConfiguration> GetAllList(Expression<Func<SysConfiguration, bool>> whereExpression)
        {
            if (whereExpression==null)
            {
                return db.GetList();
            }
            return db.GetList(whereExpression);
        }

        public SysConfiguration GetBy(Expression<Func<SysConfiguration, bool>> whereExpression)
        {
            return db.GetSingle(whereExpression);
        }

        public SysConfiguration GetById(int id)
        {
            return db.GetById(id);
        }

        public IList<SysConfiguration> GetPageList(Expression<Func<SysConfiguration, bool>> whereExpression, int page, int limit, ref int total, Expression<Func<SysConfiguration, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return db.GetPageList(whereExpression, new PageModel() { PageIndex = page, PageSize = limit }, ref total, orderByExpression, orderByType);
        }

        public void RollbackTran()
        {
            db.RollbackTran();
        }
    }
}
