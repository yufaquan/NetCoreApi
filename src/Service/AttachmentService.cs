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
    public class AttachmentService :  IAttachmentService
    {
        private DbContext<Attachment> db;
        public AttachmentService()
        {
            db = new DbContext<Attachment>();
        }
        public Attachment Add(Attachment t)
        {
            var r= db.InsertT(t);
            return r;
        }

        public bool Add(List<Attachment> list)
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

        public bool Delete(Attachment t)
        {
            return db.Delete(t.Id);
        }

        public bool Delete(List<Attachment> list)
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

        public Attachment Edit(Attachment t)
        {
            return db.UpdateT(t);
        }

        public bool Edit(List<Attachment> list)
        {
            return db.Update(list);
        }

        public IList<Attachment> GetAllList(Expression<Func<Attachment, bool>> whereExpression)
        {
            if (whereExpression==null)
            {
                return db.GetList();
            }
            return db.GetList(whereExpression);
        }

        public Attachment GetBy(Expression<Func<Attachment, bool>> whereExpression)
        {
            return db.GetSingle(whereExpression);
        }

        public Attachment GetById(int id)
        {
            return db.GetById(id);
        }

        public IList<Attachment> GetPageList(Expression<Func<Attachment, bool>> whereExpression, int page, int limit, ref int total, Expression<Func<Attachment, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return db.GetPageList(whereExpression, new PageModel() { PageIndex = page, PageSize = limit }, ref total, orderByExpression, orderByType);
        }

        public void RollbackTran()
        {
            db.RollbackTran();
        }
    }
}
