using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Authorization;
using Common.Cache;
using System.Linq.Expressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreAPI.Controllers.Portal
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ICacheManager _redisCache;

        public ValuesController(ICacheManager redisCacheManager)
        {
            _redisCache = redisCacheManager;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        // GET api/<ValuesController>/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public string GetById(int id)
        {
            _redisCache.Set("abcde", new { a = 1, b = 2 },TimeSpan.FromMinutes(5), new TimeSpan(0,5,0));
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            throw new Exception("zdy");
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [AllowAnonymous]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            throw new Exception("zdyDelete");
        }
    }
}
