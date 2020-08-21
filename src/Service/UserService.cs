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
    public class UserService :  IUserService
    {
        private DbContext<User> db;
        public UserService()
        {
            db = new DbContext<User>();
        }
        public User Add(User t)
        {
            return db.InsertT(t);
        }

        public bool Add(List<User> list)
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

        public bool Delete(User t)
        {
            return db.Delete(t.Id);
        }

        public bool Delete(List<User> list)
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

        public User Edit(User t)
        {
            return db.UpdateT(t);
        }

        public bool Edit(List<User> list)
        {
            return db.Update(list);
        }

        public IList<User> GetAllList(Expression<Func<User, bool>> whereExpression)
        {
            if (whereExpression==null)
            {
                return db.GetList();
            }
            return db.GetList(whereExpression);
        }

        public User GetBy(Expression<Func<User, bool>> whereExpression)
        {
            return db.GetSingle(whereExpression);
        }

        public User GetById(int id)
        {
            return db.GetById(id);
        }

        public IList<User> GetPageList(Expression<Func<User, bool>> whereExpression, int pageIndex, int pageSize, ref int pageCount, Expression<Func<User, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return db.GetPageList(whereExpression, new PageModel() { PageIndex = pageIndex, PageSize = pageSize }, ref pageCount, orderByExpression, orderByType);
        }

        public void RollbackTran()
        {
            db.RollbackTran();
        }
    }
}
