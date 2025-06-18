using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using LigaDAMtasy.Models;

namespace LigaDAMtasy.ViewsModels
{
    internal class CardService
    {
        private readonly HttpClient _httpClient;
        public string _url = "http://127.0.0.1:5000/";

        public CardService(ApiService apiService)
        {
            _httpClient = apiService.HttpClient;
        }

        public async Task<JsonNode?> ComprarSobre(string tipoSobre = "estándar")
        {
            var data = new
            {
                tipo_sobre = tipoSobre
            };

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_url + "buy_pack", data);
            if (response.IsSuccessStatusCode)
            {
                return JsonNode.Parse(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<JsonNode?> ObtenerCartasDisponibles()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_url + "cards");
            if (response.IsSuccessStatusCode)
            {
                return JsonNode.Parse(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<bool> ComprarCarta(string nombre, string apellidos)
        {
            var data = new
            {
                nombre,
                apellidos
            };

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_url + "buy_card", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> VenderCarta(Card card, int quantity)
        {
            var data = new
            {
                nombre = card.Nombre,
                apellidos = card.Apellidos,
                cantidad = quantity
            };

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_url + "sell_card", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<JsonNode?> Partida()
        {
            HttpResponseMessage response = await _httpClient.PostAsync(_url + "battle", null);
            if (response.IsSuccessStatusCode)
            {
                return JsonNode.Parse(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<JsonNode?> Batalla(List<Card> cartasSeleccionadas, string modo)
        {
            var cartasPayload = cartasSeleccionadas.ConvertAll(c => new { nombre = c.Nombre, apellidos = c.Apellidos });
            var data = new
            {
                cartas = cartasPayload,
                modo = modo.ToLower()
            };

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_url + "battle", data);
            if (response.IsSuccessStatusCode)
            {
                return JsonNode.Parse(await response.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<JsonNode?> RealizarBatalla(List<string> ids, string accion)
        {
            var body = new
            {
                cartas = ids,
                accion = accion
            };

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_url + "battle", body);
            if (response.IsSuccessStatusCode)
            {
                return JsonNode.Parse(await response.Content.ReadAsStringAsync());
            }
            return null;
        }
    }
}
