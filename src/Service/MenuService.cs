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
    public class MenuService :  IMenuService
    {
        private DbContext<Menus> db;
        public MenuService()
        {
            db = new DbContext<Menus>();
        }
        public Menus Add(Menus t)
        {
            return db.InsertT(t);
        }

        public bool Add(List<Menus> list)
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

        public bool Delete(Menus t)
        {
            return db.Delete(t.Id);
        }

        public bool Delete(List<Menus> list)
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

        public Menus Edit(Menus t)
        {
            return db.UpdateT(t);
        }

        public bool Edit(List<Menus> list)
        {
            return db.Update(list);
        }

        public IList<Menus> GetAllList(Expression<Func<Menus, bool>> whereExpression)
        {
            if (whereExpression==null)
            {
                return db.GetList();
            }
            return db.GetList(whereExpression);
        }

        public Menus GetBy(Expression<Func<Menus, bool>> whereExpression)
        {
            return db.GetSingle(whereExpression);
        }

        public Menus GetById(int id)
        {
            return db.GetById(id);
        }

        public IList<Menus> GetPageList(Expression<Func<Menus, bool>> whereExpression, int page, int limit, ref int total, Expression<Func<Menus, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return db.GetPageList(whereExpression, new PageModel() { PageIndex = page, PageSize = limit }, ref total, orderByExpression, orderByType);
        }

        public void RollbackTran()
        {
            db.RollbackTran();
        }
    }
}
