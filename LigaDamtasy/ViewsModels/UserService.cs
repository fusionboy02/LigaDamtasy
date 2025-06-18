using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using LigaDAMtasy.Models;

namespace LigaDAMtasy.ViewsModels
{
    internal class UserService
    {
        private readonly HttpClient _httpClient;
        public string _url = "http://127.0.0.1:5000/";

        public UserService(ApiService apiService)
        {
            _httpClient = apiService.HttpClient;
            if (!string.IsNullOrEmpty(SessionManager.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SessionManager.Token);
            }
        }

        public async Task<string> GetUserName()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_url + "users/" + SessionManager.UserEmail);
            if (response.IsSuccessStatusCode)
            {
                JsonNode? json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return json?["username"]?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }

        public async Task<int?> GetUserBalance()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_url + "balance");
            if (response.IsSuccessStatusCode)
            {
                JsonNode? json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return json?["balance"]?.GetValue<int>();
            }
            return null;
        }

        public async Task<List<Card>> GetUserCollection()
        {
            List<Card> cards = new List<Card>();
            HttpResponseMessage response = await _httpClient.GetAsync(_url + "my_collection");
            if (response.IsSuccessStatusCode)
            {
                string jsonText = await response.Content.ReadAsStringAsync();
                JsonNode? root = JsonNode.Parse(jsonText);
                var array = root?["coleccion"]?.AsArray();
                if (array != null)
                {
                    foreach (var cardNode in array)
                    {
                        cards.Add(new Card
                        {
                            Nombre = cardNode["nombre"]?.ToString() ?? "",
                            Apellidos = cardNode["apellidos"]?.ToString() ?? "",
                            Posicion = cardNode["posicion"]?.ToString() ?? "",
                            Equipo = cardNode["equipo"]?.ToString() ?? "",
                            Nacionalidad = cardNode["nacionalidad"]?.ToString() ?? "",
                            Imagen = _url + (cardNode["imagen"]?.ToString() ?? ""),
                            Rareza = cardNode["rareza"]?.ToString() ?? "",
                            Ataque = cardNode["ataque"]?.GetValue<int>() ?? 0,
                            Defensa = cardNode["defensa"]?.GetValue<int>() ?? 0,

                            Cantidad = cardNode["cantidad"]?.GetValue<int>() ?? 1 // Usar el campo cantidad
                        });
                    }
                }
            }
            return cards;
        }
    }
}
