using Common;
using Entity;
using IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Mangement
{
   public class TemplateBussiness
    {
        private IAttachmentService _service;
        public TemplateBussiness()
        {
            _service = ServiceHelp.GetAttachmentService;
        }
        public static AttachmentBussiness Init { get => new AttachmentBussiness(); }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页多少个</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public List<User> GetPageList(Attachment filter, int page, int limit, ref int total)
        {
            System.Linq.Expressions.Expression<Func<User, bool>> where = null;
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                where = where.ExpressionAnd(x => x.Name.Contains(filter.Name));
            }
            return ServiceHelp.GetUserService.GetPageList(where, page, limit, ref total, x => x.CreatedAt, SqlSugar.OrderByType.Desc).ToList();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="data"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Null：失败；</returns>
        public Attachment Add(Attachment data,out string errorMessage)
        {
            if (!VerifyData(data,out errorMessage))
            {
                return null;
            }
            var rAttachment= _service.Add(data);
            if (rAttachment == null)
            {
                errorMessage = "创建失败。";
                return null;
            }
            //记录操作日志
            Task task = ServiceHelp.GetLogService.WriteEventLogCreateAsync(typeof(Attachment), rAttachment.Id, rAttachment.ToJsonString());
            return rAttachment;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="data"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Null：失败；</returns>
        public Attachment Edit(Attachment data,out string errorMessage)
        {
            if(!VerifyData(data,out errorMessage))
            {
                return null;
            }
            if (_service.GetById(data.Id)==null)
            {
                errorMessage = "未查询到数据！可能已被删除。";
                return null;
            }
            var rAttachment = _service.Edit(data);
            if (rAttachment == null)
            {
                errorMessage = "修改失败。";
                return null;
            }
            //记录操作日志
            Task task = ServiceHelp.GetLogService.WriteEventLogEditAsync(typeof(Attachment), rAttachment.Id, rAttachment.ToJsonString());
            return rAttachment;
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errorMessage"></param>
        /// <returns>True：成功；</returns>
        public bool Delete(int id,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (_service.GetById(id) == null)
            {
                errorMessage = "未查询到数据！可能已被删除。";
                return false;
            }
            var rb= _service.DeleteById(id);
            if (rb)
            {
                //删除成功，记录日志
                Task task = ServiceHelp.GetLogService.WriteEventLogDeleteAsync(typeof(Attachment), id);
            }
            return rb;
        }

        /// <summary>
        /// 检测数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>True：校验通过；</returns>
        private bool VerifyData(Attachment data,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (data == null)
            {
                errorMessage = "请检查是否传入数据。";
                return false;
            }
            if (_service.GetAllList(x => x.Name == data.Name).Count > 0)
            {
                errorMessage = "角色名称已存在。";
                return false;
            }
            return true;
        }
    }
}
