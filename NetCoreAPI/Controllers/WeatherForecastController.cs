using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetCoreAPI.Controllers
{
    /// <summary>
    /// 天气测试模块
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 测试天气
        /// </summary>
        /// <returns></returns>
        [MyAuthorize(typeof(Read<UserOptions>))]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WeatherForecast), 200)]
        public IEnumerable<WeatherForecast> Get(int id)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }


        //[AllowAnonymous]
        //[HttpGet]
        //public JsonResult GetJWT()
        //{
            //var token = TokenHelp.IssueJWT(new Authorization.Model.TokenModel() { Mobile = "13235601859", Sub = "Default", Uid = 1, Uname = "张珊", UNickname = "ashanasf" }, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
            //var obj = new
            //{
            //    token,
            //    fullToken = "Bearer " + token
            //};
            //return new JsonResult(HttpResult.Success(obj));
        //}
    }
}
