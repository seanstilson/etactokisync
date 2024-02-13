using EtacToKiSync.M3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EtacToKiSync.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<TokenResponse> Authenticate(string azureSecret)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler(), false))
            {
                //httpClient.DefaultRequestHeaders.Remove("Content-Type");
                //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(SessionInfo.Session.EtacAuthURL),
                    Content = new StringContent(azureSecret, Encoding.UTF8, "application/x-www-form-urlencoded")
                };
               // var headers = request.Headers;
              //  headers.Remove("Content-Type");
             //   headers.Add("Content-Type", "application/x-www-form-urlencoded");
                //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return null;

                var token = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
                return token;

            }
        }
    }
}
