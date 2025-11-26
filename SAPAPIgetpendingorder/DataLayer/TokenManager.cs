using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;

namespace SAPAPIgetpendingorder.DataLayer
{
    public static class TokenManager
    {
        private static string _token;
        private static DateTime _expiry;

        public static async Task<string> GetToken()
        {
            // Return cached token if valid
            if (!string.IsNullOrEmpty(_token) && DateTime.Now < _expiry)
                return _token;

            var tokenUrl = ConfigurationManager.AppSettings["Tvs_OAuth_Token_Url"];
            var clientId = ConfigurationManager.AppSettings["Tvs_OAuth_ClientId"];
            var clientSecret = ConfigurationManager.AppSettings["Tvs_OAuth_ClientSecret"];

            var client = new HttpClient();

            var formData = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            var response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(formData));

            if (!response.IsSuccessStatusCode)
                throw new Exception("OAuth token generation failed: " + response.ReasonPhrase);

            var json = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);

            _token = tokenResponse.access_token;
            _expiry = DateTime.Now.AddSeconds(tokenResponse.expires_in - 60); // refresh early

            return _token;
        }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}
//