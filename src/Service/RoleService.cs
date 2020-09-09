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
    public class RoleService :  IRoleService
    {
        private DbContext<Role> db;
        public RoleService()
        {
            db = new DbContext<Role>();
        }
        public Role Add(Role t)
        {
            return db.InsertT(t);
        }

        public bool Add(List<Role> list)
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

        public bool Delete(Role t)
        {
            return db.Delete(t.Id);
        }

        public bool Delete(List<Role> list)
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

        public Role Edit(Role t)
        {
            return db.UpdateT(t);
        }

        public bool Edit(List<Role> list)
        {
            return db.Update(list);
        }

        public IList<Role> GetAllList(Expression<Func<Role, bool>> whereExpression)
        {
            if (whereExpression==null)
            {
                return db.GetList();
            }
            return db.GetList(whereExpression);
        }

        public Role GetBy(Expression<Func<Role, bool>> whereExpression)
        {
            return db.GetSingle(whereExpression);
        }

        public Role GetById(int id)
        {
            return db.GetById(id);
        }

        public IList<Role> GetPageList(Expression<Func<Role, bool>> whereExpression, int page, int limit, ref int total, Expression<Func<Role, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return db.GetPageList(whereExpression, new PageModel() { PageIndex = page, PageSize = limit }, ref total, orderByExpression, orderByType);
        }

        public void RollbackTran()
        {
            db.RollbackTran();
        }
    }
}
