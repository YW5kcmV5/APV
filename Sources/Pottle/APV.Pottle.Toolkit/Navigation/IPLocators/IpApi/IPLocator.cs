using System;
using System.IO;
using System.Net;
using APV.Pottle.Toolkit.Navigation.Entities;
using APV.Pottle.Toolkit.Navigation.Interfaces;
using Newtonsoft.Json;

namespace APV.Pottle.Toolkit.Navigation.IPLocators.IpApi
{
    public class IPLocator : IIPLocator
    {
        private readonly Settings _settings;

        internal class JsonResponse
        {
            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "country")]
            public string Country { get; set; }

            [JsonProperty(PropertyName = "countryCode")]
            public string CountryCode { get; set; }

            [JsonProperty(PropertyName = "region")]
            public string Region { get; set; }

            [JsonProperty(PropertyName = "regionName")]
            public string RegionName { get; set; }

            [JsonProperty(PropertyName = "city")]
            public string City { get; set; }

            [JsonProperty(PropertyName = "zip")]
            public string Zip { get; set; }

            [JsonProperty(PropertyName = "lat")]
            public float? Lat { get; set; }

            [JsonProperty(PropertyName = "lon")]
            public float? Lon { get; set; }

            [JsonProperty(PropertyName = "timezone")]
            public string Timezone { get; set; }

            [JsonProperty(PropertyName = "isp")]
            public string Isp { get; set; }

            [JsonProperty(PropertyName = "org")]
            public string Org { get; set; }

            [JsonProperty(PropertyName = "as")]
            public string As { get; set; }

            [JsonProperty(PropertyName = "query")]
            public string Query { get; set; }
        }

        private IPLocation InvokeGetIPLocation(IPAddress ip)
        {
            //Request example: http://ip-api.com/json/208.80.152.201

            string serviceUrl = _settings.ServiceUrl;
            while (serviceUrl.EndsWith("/"))
            {
                serviceUrl = serviceUrl.Substring(0, serviceUrl.Length - 1);
            }
            string url = string.Format("{0}/json/{1}", serviceUrl, ip);
            
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Headers.Add("Accept-Language", "ru,en");
            request.Headers.Add("Accept-Charset", "utf-8");

            string message;
            string jsonData = string.Empty;
            JsonResponse response;
            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream stream = webResponse.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            jsonData = reader.ReadToEnd();
                        }
                    }
                }
                response = JsonConvert.DeserializeObject<JsonResponse>(jsonData);
            }
            catch (Exception ex)
            {
                message = string.Format("InfoDb get ip location response failed. See inner exception for more details.\r\nRequested address: \"{0}\"\r\nUrl: \"{1}\"\r\n", ip, url);
                throw new InvalidOperationException(message, ex);
            }

            bool success = (response.Status == "success") && (response.Lat != null) &&
                           (response.Lon != null) && (!string.IsNullOrEmpty(response.CountryCode));

            if (success)
            {
                return new IPLocation
                    {
                        IP = ip,
                        City = response.City,
                        CountryCode = response.CountryCode,
                        CountryName = response.Country,
                        Location = new GeoLocation
                            {
                                LAT = response.Lat.Value,
                                LON = response.Lon.Value,
                            },
                    };
            }

            if (response.Status == "fail")
            {
                return null;
            }

            message = string.Format("InfoDb get ip location response could no be parsed.\r\nRequested address: \"{0}\"\r\nUrl: \"{1}\"\r\nJSON Response: \"{2}\"", ip, url, jsonData);
            throw new InvalidOperationException(message);
        }

        public class Settings
        {
            public string ServiceUrl { get; set; }
        }

        public IPLocator()
            : this(DefaultSettings)
        {
        }

        public IPLocator(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            _settings = DefaultSettings;
        }

        #region IIPLocator

        public IPLocation GetIPLocation(IPAddress ip)
        {
            if (ip == null)
                throw new ArgumentNullException("ip");

            return InvokeGetIPLocation(ip);
        }

        #endregion

        public static readonly Settings DefaultSettings = new Settings
            {
                ServiceUrl = @"http://ip-api.com/",
            };
    }
}