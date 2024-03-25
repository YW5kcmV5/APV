using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using APV.Pottle.Toolkit.Navigation.Entities;
using APV.Pottle.Toolkit.Navigation.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APV.Pottle.Toolkit.Navigation.GeoLocators.Google
{
    public class GoogleGeoLocator : IGeoLocator
    {
        private readonly Settings _settings;

        internal class JsonResponse
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum AddressType
            {
                [EnumMember(Value = "street_address")]
                StreetAddress,

                [EnumMember(Value = "locality")]
                Locality,

                [EnumMember(Value = "political")]
                Political,

                [EnumMember(Value = "street_number")]
                StreetNumber,

                [EnumMember(Value = "route")]
                Route,

                [EnumMember(Value = "administrative_area_level_3")]
                AdministrativeAreaLevel3,

                [EnumMember(Value = "administrative_area_level_2")]
                AdministrativeAreaLevel2,

                [EnumMember(Value = "administrative_area_level_1")]
                AdministrativeAreaLevel1,

                [EnumMember(Value = "country")]
                Country,

                [EnumMember(Value = "postal_code")]
                PostalCode,

                [EnumMember(Value = "post_box")]
                PostBox,

                /// <summary>
                /// Этаж
                /// </summary>
                [EnumMember(Value = "floor")]
                Floor,

                /// <summary>
                /// Номер квартиры
                /// </summary>
                [EnumMember(Value = "room")]
                Room,

                /// <summary>
                /// Крупные перекрестки, обычно двух главных дорог
                /// </summary>
                [EnumMember(Value = "intersection")]
                Intersection,

                /// <summary>
                /// Альтернативное название для политической или административной единицы
                /// </summary>
                [EnumMember(Value = "colloquial_area")]
                ColloquialArea,

                /// <summary>
                /// Район города
                /// </summary>
                [EnumMember(Value = "sublocality")]
                Sublocality,

                /// <summary>
                /// Округ
                /// </summary>
                [EnumMember(Value = "sublocality_level_1")]
                SublocalityLevel1,

                [EnumMember(Value = "sublocality_level_2")]
                SublocalityLevel3,

                /// <summary>
                /// Микрорайоны и квартал с собственным названием
                /// </summary>
                [EnumMember(Value = "neighborhood")]
                Neighborhood,

                /// <summary>
                /// Поименованное местоположения, обычно строение или группа строений с общим именем
                /// </summary>
                [EnumMember(Value = "premise")]
                Premise,

                /// <summary>
                /// Единица, на которые делится предыдущая категория Premise(), обычно отдельные строения внутри групп строений с общим названием
                /// </summary>
                [EnumMember(Value = "subpremise")]
                Subpremise,

                /// <summary>
                /// Главные природные достопримечательности
                /// </summary>
                [EnumMember(Value = "natural_feature")]
                NaturalFeature,

                /// <summary>
                /// Аэропорт
                /// </summary>
                [EnumMember(Value = "airport")]
                Airport,

                /// <summary>
                /// Парк
                /// </summary>
                [EnumMember(Value = "park")]
                Park,

                /// <summary>
                /// Достопримечательности, имеющие названия
                /// </summary>
                [EnumMember(Value = "point_of_interest")]
                PointOfInterest,
            }

            public class AddressComponent
            {
                [JsonProperty(PropertyName = "long_name", Required = Required.Always)]
                public string LongName { get; set; }

                [JsonProperty(PropertyName = "short_name", Required = Required.Always)]
                public string ShortName { get; set; }

                [JsonProperty(PropertyName = "types", Required = Required.Always)]
                public AddressType[] Types { get; set; }
            }

            public class Location
            {
                [JsonProperty(PropertyName = "lat", Required = Required.Always)]
                public float LAT { get; set; }

                [JsonProperty(PropertyName = "lng", Required = Required.Always)]
                public float LON { get; set; }
            }

            public class Geometry
            {
                [JsonProperty(PropertyName = "location")]
                public Location Location { get; set; }
            }

            public class Result
            {
                [JsonProperty(PropertyName = "address_components")]
                public AddressComponent[] AddressComponents { get; set; }

                [JsonProperty(PropertyName = "formatted_address")]
                public string FormattedAddress { get; set; }

                [JsonProperty(PropertyName = "geometry")]
                public Geometry Geometry { get; set; }
            }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty(PropertyName = "results")]
            public Result[] Results { get; set; }
        }

        private JsonResponse GetResponse(string url, out string jsonData)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Headers.Add("Accept-Language", "ru,en");
            request.Headers.Add("Accept-Charset", "utf-8");

            jsonData = string.Empty;
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
            return JsonConvert.DeserializeObject<JsonResponse>(jsonData);
        }

        private static string FormatAddress(JsonResponse.Result result)
        {
            var items = new string[6];
            string cityName = null;
            foreach (JsonResponse.AddressComponent component in result.AddressComponents)
            {
                if (component.Types.Any(type => type == JsonResponse.AddressType.PostalCode))
                {
                    //Код города
                    items[0] = component.LongName;
                }
                else if (component.Types.Any(type => type == JsonResponse.AddressType.Country))
                {
                    //Страна
                    items[1] = component.LongName;
                }
                else if (component.Types.Any(type => type == JsonResponse.AddressType.AdministrativeAreaLevel1))
                {
                    //Область (может совпадать с название города, например "город Санкт-Петербург")
                    string value = component.LongName;
                    if (value != cityName)
                    {
                        cityName = value;
                        items[2] = value;
                    }
                }
                else if (component.Types.Any(type => type == JsonResponse.AddressType.Locality))
                {
                    //Город
                    string value = (!component.LongName.StartsWith("город ")) ? "город " + component.LongName : component.LongName;
                    if (value != cityName)
                    {
                        cityName = value;
                        items[3] = value;
                    }
                }
                else if (component.Types.Any(type => type == JsonResponse.AddressType.Route))
                {
                    //Улица
                    items[4] = component.LongName;
                }
                else if (component.Types.Any(type => type == JsonResponse.AddressType.StreetNumber))
                {
                    //Номер улицы
                    string value = (!component.LongName.StartsWith("дом ")) ? "дом " + component.LongName : component.LongName;
                    if (value.Contains(" корпус "))
                    {
                        value = value.Replace(" корпус ", ", корпус ");
                    }
                    items[5] = value;
                }
            }

            var sb = new StringBuilder();
            foreach (string item in items)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    sb.AppendFormat("{0}, ", item);
                }
            }
            sb.Length -= 2;
            return sb.ToString();
        }

        private GeoAddress InvokeGetGeoAddress(string address)
        {
            //Request example: https://maps.googleapis.com/maps/api/geocode/json?address=1600+Amphitheatre+Parkway,+Mountain+View,+CA&key=API_KEY

            string serviceUrl = _settings.ServiceUrl;
            while (serviceUrl.EndsWith("/"))
            {
                serviceUrl = serviceUrl.Substring(0, serviceUrl.Length - 1);
            }
            string url = string.Format("{0}/json?address={1}&key={2}&language=ru", serviceUrl, HttpUtility.UrlEncode(address), _settings.ApplicationKey);
            
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Headers.Add("Accept-Language", "ru,en");
            request.Headers.Add("Accept-Charset", "utf-8");

            string message;
            string jsonData;
            JsonResponse response;
            try
            {
                response = GetResponse(url, out jsonData);
            }
            catch (Exception ex)
            {
                message = string.Format("Google get geo location response failed. See inner exception for more details.\r\nRequested address: \"{0}\"\r\nUrl: \"{1}\"\r\n", address, url);
                throw new InvalidOperationException(message, ex);
            }

            bool success = (response.Status == "OK") && (response.Results != null) && (response.Results.Length == 1) &&
                           (response.Results[0] != null) && (response.Results[0].Geometry != null) &&
                           (response.Results[0].Geometry.Location != null) &&
                           (response.Results[0].FormattedAddress != null);

            if (success)
            {
                return new GeoAddress
                    {
                        Address = address,
                        FormattedAddress = FormatAddress(response.Results[0]),
                        Location = new GeoLocation
                            {
                                LAT = response.Results[0].Geometry.Location.LAT,
                                LON = response.Results[0].Geometry.Location.LON,
                            }
                    };
            }

            if (response.Status == "ZERO_RESULTS")
            {
                return null;
            }

            message = string.Format("Google get geo location response could no be parsed.\r\nRequested address: \"{0}\"\r\nUrl: \"{1}\"\r\nJSON Response: \"{2}\"", address, url, jsonData);
            throw new InvalidOperationException(message);
        }

        private GeoAddress InvokeGetGeoAddress(GeoLocation location)
        {
            //Request example: https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key=API_KEY

            string serviceUrl = _settings.ServiceUrl;
            while (serviceUrl.EndsWith("/"))
            {
                serviceUrl = serviceUrl.Substring(0, serviceUrl.Length - 1);
            }
            
            var nfi = new NumberFormatInfo {NumberDecimalSeparator = "."};
            string url = string.Format("{0}/json?&latlng={1},{2}&key={3}&language=ru", serviceUrl, location.LAT.ToString(nfi), location.LON.ToString(nfi), _settings.ApplicationKey);

            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Headers.Add("Accept-Language", "ru,en");
            request.Headers.Add("Accept-Charset", "utf-8");

            string message;
            string jsonData;
            JsonResponse response;
            try
            {
                response = GetResponse(url, out jsonData);
            }
            catch (Exception ex)
            {
                message = string.Format("Google get geo location response failed. See inner exception for more details.\r\nRequested LAT: \"{0}\"\r\nRequested LON: \"{1}\"\r\nUrl: \"{2}\"\r\n", location.LAT, location.LON, url);
                throw new InvalidOperationException(message, ex);
            }

            bool success = (response.Status == "OK") && (response.Results != null) && (response.Results.Length >= 1) &&
                           (response.Results[0] != null) && (response.Results[0].Geometry != null) &&
                           (response.Results[0].Geometry.Location != null) &&
                           (response.Results[0].FormattedAddress != null);

            if (success)
            {
                return new GeoAddress
                    {
                        Address = response.Results[0].FormattedAddress,
                        FormattedAddress = FormatAddress(response.Results[0]),
                        Location = location,
                    };
            }

            if (response.Status == "ZERO_RESULTS")
            {
                return null;
            }

            message = string.Format("Google get geo location response could no be parsed.\r\n.\r\nRequested LAT: \"{0}\"\r\nRequested LON: \"{1}\"\r\nUrl: \"{1}\"\r\nJSON Response: \"{2}\"", location.LAT, location.LON, jsonData);
            throw new InvalidOperationException(message);
        }

        public class Settings
        {
            public string ServiceUrl { get; set; }

            public string ApplicationKey { get; set; }

            public int LimitRequestsPerSecond { get; set; }

            public int LimitRequestsPerDay { get; set; }
        }

        public GoogleGeoLocator()
            : this(DefaultSettings)
        {
        }

        public GoogleGeoLocator(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            _settings = DefaultSettings;
        }

        #region IGeoLocator

        public GeoAddress GetGeoAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            return InvokeGetGeoAddress(address);
        }

        public GeoAddress GetGeoAddress(GeoLocation location)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            return InvokeGetGeoAddress(location);
        }

        #endregion

        public static readonly Settings DefaultSettings = new Settings
            {
                ApplicationKey = "AIzaSyAdffcBnwuayY8Mz5YinbSfqNxgVXmL6ZE",
                ServiceUrl = @"https://maps.googleapis.com/maps/api/geocode/",
                LimitRequestsPerSecond = 5,
                LimitRequestsPerDay = 2500,
            };
    }
}