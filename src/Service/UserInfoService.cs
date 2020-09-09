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
    public class UserInfoService :  IUserInfoService
    {
        private DbContext<UserInfo> db;
        public UserInfoService()
        {
            db = new DbContext<UserInfo>();
        }
        public UserInfo Add(UserInfo t)
        {
            return db.InsertT(t);
        }

        public bool Add(List<UserInfo> list)
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

        public bool Delete(UserInfo t)
        {
            return db.Delete(t.Id);
        }

        public bool Delete(List<UserInfo> list)
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

        public UserInfo Edit(UserInfo t)
        {
            return db.UpdateT(t);
        }

        public bool Edit(List<UserInfo> list)
        {
            return db.Update(list);
        }

        public IList<UserInfo> GetAllList(Expression<Func<UserInfo, bool>> whereExpression)
        {
            if (whereExpression==null)
            {
                return db.GetList();
            }
            return db.GetList(whereExpression);
        }

        public UserInfo GetBy(Expression<Func<UserInfo, bool>> whereExpression)
        {
            return db.GetSingle(whereExpression);
        }

        public UserInfo GetById(int id)
        {
            return db.GetById(id);
        }

        public IList<UserInfo> GetPageList(Expression<Func<UserInfo, bool>> whereExpression, int page, int limit, ref int total, Expression<Func<UserInfo, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return db.GetPageList(whereExpression, new PageModel() { PageIndex = page, PageSize = limit }, ref total, orderByExpression, orderByType);
        }

        public void RollbackTran()
        {
            db.RollbackTran();
        }
    }
}
