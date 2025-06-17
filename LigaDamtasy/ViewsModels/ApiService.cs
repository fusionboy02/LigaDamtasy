using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using LigaDAMtasy.Models;

namespace LigaDAMtasy.ViewsModels
{
    internal class ApiService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public string _url = "http://127.0.0.1:5000/";

        public ApiService()
        {
            if (!string.IsNullOrEmpty(SessionManager.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SessionManager.Token);
            }
        }

        public HttpClient HttpClient => _httpClient;
        public string Url => _url;

        public async Task<bool> Register(string username, string email, string password)
        {
            var data = new
            {
                username,
                email,
                password
            };

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync(_url + "register", data);
            return httpResponseMessage.IsSuccessStatusCode;
        }

        public async Task<bool> Login(string email, string pass)
        {
            var data = new
            {
                email,
                password = pass
            };

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync(_url + "login", data);
            string loginDataResponse = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                JsonNode? response = JsonNode.Parse(loginDataResponse);
                string token = response?["token"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    SessionManager.Token = token;
                    SessionManager.UserEmail = email;
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> Logout()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(_url + "logout", null);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                SessionManager.Token = null;
                SessionManager.UserEmail = null;
                return true;
            }
            return false;
        }
    }
}
