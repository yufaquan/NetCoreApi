using System.Collections.Generic;
using System.Linq;
using Authorization;
using Bussiness;
using Bussiness.Mangement;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace NetCoreAPI.Controllers.Management
{
    /// <summary>
    /// 附件
    /// </summary>
    [ApiController]
    public class AttachmentController : ManagementApiController
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        public AttachmentController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            Current.ServerPath = _webHostEnvironment.ContentRootPath;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="name">筛选条件And拼接</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页多少条</param>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<Attachment>))]
        [HttpGet]
        public JsonResult GetList(string name,int page,int limit)
        {
            var pageCount = 0;
            var data = new Attachment();
            data.Name = name;
            var list = AttachmentBussiness.Init.GetPageList(data, page, limit, ref pageCount);
            //string errorMessage;
            var result = new {
                pageCount,
                page,
                limit,
                list = from a in list select new
                {
                    a.Name, a.Id, a.Extension,a.Enabled,a.Path,a.Size,
                    Type=a.Type.GetDisplayName(),
                    CreatedAt = a.CreatedAt.ToLongString(),
                    CreatedBy = a.CreatedBy.HasValue ? ServiceHelp.GetAttachmentService.GetById(a.CreatedBy.Value)?.Name : ""
                }
            };
            return new JsonResult(HttpResult.Success(result));
        }


        /// <summary>
        /// 创建附件
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns></returns>
        [HttpPost]
        [MyAuthorize(typeof(Create<Attachment>))]
        public JsonResult Add([FromForm]IEnumerable<IFormFile> file)
        {
            string errorMessage;
            var result = AttachmentBussiness.Init.Add(file.ToList(), out errorMessage);
            if (result==null)
            {
                return new JsonResult(HttpResult.Success( HttpResultCode.AddFail,errorMessage,result));
            }
            else
            {
                return new JsonResult(HttpResult.Success(result));
            }
        }

        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [MyAuthorize(typeof(DownLoad<Attachment>))]
        public ActionResult DownLoadAttachment(int id) {
            var attachment = ServiceHelp.GetAttachmentService.GetById(id);
            if (attachment!=null)
            {
                using (var sw = new FileStream(Current.ServerPath + attachment.Path, FileMode.Open))
                {
                    byte[] bytes = new byte[sw.Length];
                    sw.Read(bytes, 0, bytes.Length);
                    sw.Close();
                    // 把 byte[] 转换成 Stream 
                    Stream stream = new MemoryStream(bytes);
                    return File(stream, attachment.ContentType, attachment.Name);
                }
            }
            else
            {
                return new JsonResult(HttpResult.Success(HttpResultCode.SelectFail, "未查询到附件", null));
            }
        }

        /// <summary>
        /// 返回附件信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAttachment(int id)
        {
            var attachment = ServiceHelp.GetAttachmentService.GetById(id);
            if (attachment != null)
            {
                return PhysicalFile(Current.ServerPath + attachment.Path, attachment.ContentType);
            }
            else
            {
                return new JsonResult(HttpResult.Success(HttpResultCode.SelectFail, "未查询到附件", null));
            }
        }
    }
}
