using EtacToKiSync.M3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EtacToKiSync.Services
{
    public class M3API : IM3API
    {
        public static Dictionary<string, string> CustomerQueryParams = new Dictionary<string, string>
        {
            {"dateformat", "YMD8" },
            {"excludeempty", "false" },
            {"righttrim", "true" },
            {"extendedresult", "false" },
            {"format", "PRETTY" }
        };

        public static Dictionary<string, string> AddressQueryParams = new Dictionary<string, string>
        {
            {"ADRT", "1" },
            {"dateformat", "YMD8" },
            {"excludeempty", "false" },
            {"righttrim", "true" },
            {"extendedresult", "false" },
            {"format", "PRETTY" },
            {"ADID", "C001" }
        };

        public static Dictionary<string, string> OrderInfoQueryParams = new Dictionary<string, string>
        {
            {"CONO", "100" },
            {"maxrecs", "0" },
            {"dateformat", "YMD8" },
            {"excludeempty", "false" },
            {"righttrim", "true" },
            {"returncols", "CUNO, CONO, ADID" },
            {"extendedresult", "false" },
            {"format", "PRETTY" }
        };

        public static Dictionary<string, string> PartInfoQueryParams = new Dictionary<string, string>
        {
            {"dateformat", "YMD8" },
            {"excludeempty", "false" },
            {"righttrim", "true" },
            {"returncols", "ITNO,ITDS,FUDS" },
            {"extendedresult", "false" },
            {"format", "PRETTY" }
        };

        public async Task<BackendAddressRecord> GetM3AddressData(string customerNumber, string addressID)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler(), false))
            {
                //httpClient.DefaultRequestHeaders.Remove("Content-Type");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = BuildM3AddressQueryString(customerNumber, addressID)
                };

                var headers = request.Headers;
                //  headers.Remove("Content-Type");
                headers.Add("Authorization", $"Bearer {SessionInfo.Session.Token.access_token}");
                //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return null;

                var addressInfo = JsonConvert.DeserializeObject<BackendAddressRecord>(await response.Content.ReadAsStringAsync());
                return addressInfo;

            }
        }

        public async Task<BackendOrderInfoRecord> GetM3OrderInfoData(string customerNumber)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler(), false))
            {
                //httpClient.DefaultRequestHeaders.Remove("Content-Type");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = BuildM3OrderInfoQueryString(customerNumber)
                };

                var headers = request.Headers;
                //  headers.Remove("Content-Type");
                headers.Add("Authorization", $"Bearer {SessionInfo.Session.Token.access_token}");
                //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return null;

                var orderInfo = JsonConvert.DeserializeObject<BackendOrderInfoRecord>(await response.Content.ReadAsStringAsync());
                return orderInfo;

            }
        }

        public async Task<BackendCustomerRecord> GetM3BasicCustomerData(string customerNumber)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler(), false))
            {
                //httpClient.DefaultRequestHeaders.Remove("Content-Type");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = BuildM3QueryString(customerNumber)
                };

                var headers = request.Headers;
                //  headers.Remove("Content-Type");
                headers.Add("Authorization", $"Bearer {SessionInfo.Session.Token.access_token}");
                //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return null;

                var addressInfo = JsonConvert.DeserializeObject<BackendCustomerRecord>(await response.Content.ReadAsStringAsync());
                return addressInfo;

            }
        }

        public async Task<BackendPartRecord> GetM3BasicPartData(string partNumber)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler(), false))
            {
                //httpClient.DefaultRequestHeaders.Remove("Content-Type");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = BuildM3PartInfoQueryString(partNumber)
                };

                var headers = request.Headers;
                //  headers.Remove("Content-Type");
                headers.Add("Authorization", $"Bearer {SessionInfo.Session.Token.access_token}");
                //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return null;

                var partInfo = JsonConvert.DeserializeObject<BackendPartRecord>(await response.Content.ReadAsStringAsync());
                return partInfo;

            }
        }

        private Uri BuildM3QueryString(string customerNumber)
        {
            UriBuilder builder = new UriBuilder($"{SessionInfo.Session.GetCustomerAddressEtacBaseURL}/GetBasicData")
            {
                Query = $"?CUNO={customerNumber}&" + string.Join("&", CustomerQueryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"))
            };

            return builder.Uri;
        }
        private Uri BuildM3AddressQueryString(string customerNumber, string addrNumber = null)
        {
            if ( addrNumber != null ) 
                AddressQueryParams["ADID"] = addrNumber;

            UriBuilder builder = new UriBuilder($"{SessionInfo.Session.GetCustomerAddressEtacBaseURL}/GetAddress")
            {
                Query =  $"?CUNO={customerNumber}&" + string.Join("&", AddressQueryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"))                
            };

            return builder.Uri;
        }

        private Uri BuildM3OrderInfoQueryString(string customerNumber)
        {
            UriBuilder builder = new UriBuilder($"{SessionInfo.Session.GetCustomerAddressEtacBaseURL}/GetOrderInfo")
            {
                Query = $"?CUNO={customerNumber}&" + string.Join("&", OrderInfoQueryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"))
            };

            return builder.Uri;
        }

        private Uri BuildM3PartInfoQueryString(string partNumber)
        {
            UriBuilder builder = new UriBuilder($"{SessionInfo.Session.GetM3PartBaseURL}/GetItmBasic")
            {
                Query = $"?ITNO={partNumber}&" + string.Join("&", PartInfoQueryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"))
            };

            return builder.Uri;
        }
    }
}
