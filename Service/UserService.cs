using Entity;
using IService;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Service
{
    public class UserService : DbContext<User>, IUserService
    {
        public User Add(User t)
        {
            throw new NotImplementedException();
        }

        public bool Add(List<User> list)
        {
            throw new NotImplementedException();
        }

        public bool Delete(User t)
        {
            throw new NotImplementedException();
        }

        public bool Delete(List<User> list)
        {
            throw new NotImplementedException();
        }

        public bool Delete(List<dynamic> idList)
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(dynamic id)
        {
            throw new NotImplementedException();
        }

        public User Edit(User t)
        {
            throw new NotImplementedException();
        }

        public bool Edit(List<User> list)
        {
            throw new NotImplementedException();
        }

        public User GetBy(Expression<Func<User, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }

        public User GetById(dynamic id)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetPageList(Expression<Func<User, bool>> whereExpression, int pageIndex, int pageSize, ref int pageCount, Expression<Func<User, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            throw new NotImplementedException();
        }

        IList<User> IServiceBase<User>.GetList(Expression<Func<User, bool>> whereExpression)
        {
            throw new NotImplementedException();
        }
    }
}
