using Common;
using Entity;
using IService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Mangement
{
   public class AttachmentBussiness
    {
        private IAttachmentService _service;

        /// <summary>
        /// 上传的大小限制
        /// </summary>
        private int LimitUpFileSize;
        public AttachmentBussiness()
        {
            _service = ServiceHelp.GetAttachmentService;
            LimitUpFileSize = ConfigurationBussiness.Init.Get().LimitUpFileSize;
        }
        public static AttachmentBussiness Init { get => new AttachmentBussiness(); }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<Attachment> GetPageList(Attachment filter, int page, int limit, ref int total)
        {
            System.Linq.Expressions.Expression<Func<Attachment, bool>> where = null;
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                where = where.ExpressionAnd(x => x.Name.Contains(filter.Name));
            }
            return ServiceHelp.GetAttachmentService.GetPageList(where, page, limit, ref total, x => x.CreatedAt, SqlSugar.OrderByType.Desc).ToList();
        }


        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="files">文件集</param>
        /// <param name="errorMessage"></param>
        /// <returns>Null：失败；</returns>
        public Attachment Add(List<Microsoft.AspNetCore.Http.IFormFile> files, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (files == null || files.Count == 0)
            {
                errorMessage = "请上传文件。"; return null;
            }

            if (LimitUpFileSize>0)
            {
                var limitFiles = files.Where(x => x.Length > LimitUpFileSize * 1024 * 1024).Select(s => s.FileName);
                if (limitFiles.Count() > 0)
                {
                    errorMessage = $"文件大小不能超过：{ LimitUpFileSize}M,{string.Join(",", limitFiles)}";
                    return null;
                }
            }
            string filePath = $"/Upload/Attachment/{DateTime.Now.ToString("yyyy")}/{DateTime.Now.ToString("MM")}/{DateTime.Now.ToString("dd")}/";
            string fullPath = Current.ServerPath+filePath;

            //创建Attachment
            var att = new Attachment();
            int i = 0;
            try
            {
                if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                for (i = 0; i < files.Count; i++)
                {
                    // 文件大小
                    long size =files[i].Length;
                    //原文件名（包括路径）
                    string fullName = files[i].FileName;
                    // 扩展名
                    var extName = fullName.Substring(fullName.LastIndexOf('.')).Replace("\"", "");
                    // 文件名
                    var filename = fullName.Substring(0, fullName.LastIndexOf('.'));
                    // 新文件名
                    string shortfilename = $"{Guid.NewGuid()}{extName}";
                    // 新文件名（包括路径）
                    string newFullName = fullPath + "/" + shortfilename;
                    using (var stream = new FileStream(newFullName, FileMode.Create))
                    {
                        files[i].CopyTo(stream);
                        att.Base64 = Commons.FileToBase64(stream);
                    }

                    att.Extension = extName;
                    att.Name = filename;
                    att.Path = filePath+shortfilename;
                    att.Size = size;
                    att.ContentType = files[i].ContentType;
                    att.GetFileType();
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"上传文件失败：路径：{filePath},失败文件:{files[i]},{ex.Message + ex.StackTrace}";
                return null;
            }
            
            var rAttachment = _service.Add(att);
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
        /// <param name="role"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Null：失败；</returns>
        public Attachment Edit(Attachment role,out string errorMessage)
        {
            if(!VerifyData(role,out errorMessage))
            {
                return null;
            }
            if (_service.GetById(role.Id)==null)
            {
                errorMessage = "未查询到数据！";
                return null;
            }
            var rAttachment = _service.Edit(role);
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
        /// <param name="Id"></param>
        /// <param name="errorMessage"></param>
        /// <returns>True：成功；</returns>
        public bool Delete(int Id,out string errorMessage)
        {
            errorMessage = string.Empty;
            var rb= _service.DeleteById(Id);
            if (rb)
            {
                //删除成功，记录日志
                Task task = ServiceHelp.GetLogService.WriteEventLogDeleteAsync(typeof(Attachment), Id);
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
