using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussiness;
using Entity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreAPI.Controllers.Management
{
    [Route("api/management/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        public JsonResult Get()
        {
            var list = ServiceHelp.GetUserService.GetAllList(null);
            return new JsonResult(HttpResult.Success(new { list=list }));
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string GetById(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] User user)
        {
            //ServiceHelp.GetUserService.Add(new Entity.User() { Email = "13235601859@163.com", Area = "中国-湖北-武汉", Mobile = "13235601859", Sex = Entity.Enums.Sex.man, Name = "yufaquan", NickName = "单逸" });
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
