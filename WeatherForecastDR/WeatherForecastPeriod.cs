using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecastDR
{
    internal class WeatherForecastPeriod
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public float Temperature { get; set; }
        public string TemperatureUnit { get; set; }
    }
}
