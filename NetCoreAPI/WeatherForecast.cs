using System;

namespace NetCoreAPI
{
    /// <summary>
    /// ����
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// ʱ��
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// aa
        /// </summary>
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// ����
        /// </summary>
        public string Summary { get; set; }
    }
}
