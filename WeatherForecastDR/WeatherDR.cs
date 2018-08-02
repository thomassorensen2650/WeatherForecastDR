using Newtonsoft.Json.Linq;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace WeatherForecastDR
{
    [Guid("04D0C230-80DE-4FD4-8F54-C147C7FCA747")]
    [Serializable]
    [Description("Weather;Get Weather Info")]
    public class WeatherDR : AFDataReference
    {
        public float Lng { get; set; }
        public float Lat { get; set; }
        public string SearchString { get; set; }

        const string weatherBaseUrl = @"https://api.weather.gov/points/";

        public override string ConfigString
        {
            get
            {
                return $"lng={Lng};lan={Lat};s={SearchString}";
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    var tokens = value.Split(';');
                    foreach (var token in tokens)
                    {
                        var keyValue = token.Split('=');
                        switch (keyValue[0].ToLower())
                        {
                            case "lng":
                                float.TryParse(keyValue[1], out float ln);
                                Lng = ln;
                                break;
                            case "lan":
                                float.TryParse(keyValue[1], out float la);
                                Lat = la;
                                break;
                            case "s":
                                break;
                            default:
                                break;
                        }
                    }
                }
                SaveConfigChanges();
            }
        }

        public override AFValue GetValue(object context, object timeContext, AFAttributeList inputAttributes, AFValues inputValues)
        {
            // I could not find a free API that supports historial data. so I'm just getting newest forecast data
            AFValue result = new AFValue();

            using (var webClient = new WebClient())
            {
                webClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                webClient.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";

                var url = new Uri($"{weatherBaseUrl}{Lat},{Lng}/forecast");
                var json = webClient.DownloadString(url);
                var j = JObject.Parse(json);

                var results = j["properties"]["periods"].Select(sf => (JObject)sf).Select(sf =>
                    new WeatherForecastPeriod()
                    {
                        Temperature = sf["temperature"].Value<int>(),
                        TemperatureUnit = sf["temperatureUnit"].Value<string>(),
                        StartTime = sf["startTime"].Value<DateTime>(),
                        EndTime = sf["endTime"].Value<DateTime>()
                    });
                result.Value = results.First().Temperature;
                result.UOM = this.PISystem.UOMDatabase.UOMs.First(x => x.Abbreviation == $"°{results.First().TemperatureUnit}");
            }
            return result;
        }

        public override Type EditorType => typeof(FindLocationForm);

        public override AFDataMethods SupportedDataMethods => AFDataMethods.None;

        public override AFDataReferenceMethod SupportedMethods => AFDataReferenceMethod.GetValue;

        public override AFDataReferenceContext SupportedContexts => AFDataReferenceContext.None; 

    }
}
