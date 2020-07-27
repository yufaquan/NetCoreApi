using System;

namespace NetCoreAPI
{
    /// <summary>
    /// 天气
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// aa
        /// </summary>
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// 天气
        /// </summary>
        public string Summary { get; set; }
    }
}
