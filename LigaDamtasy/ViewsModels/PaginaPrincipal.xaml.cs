using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LigaDAMtasy.Models;
using LigaDAMtasy.ViewsModels;

namespace LigaDAMtasy
{
    public partial class PaginaPrincipal : Window
    {
        private readonly UserService _userService;
        private readonly CardService _cardService;
        ApiService apiService = new ApiService();

        public PaginaPrincipal()
        {
            InitializeComponent();
            _userService = new UserService(apiService);
            _cardService = new CardService(apiService);
            CargarDatosUsuario();
            this.Activated += PaginaPrincipal_Activated; //esto es para actualizar las monedas al volver a la ventana
        }

        private async void PaginaPrincipal_Activated(object sender, System.EventArgs e)
        {
            await ActualizarMonedas();
        }
        private void CerrarApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void CargarDatosUsuario()
        {
            string userName = await _userService.GetUserName();
            int? userBalance = await _userService.GetUserBalance();

            if (!string.IsNullOrEmpty(userName) && userBalance.HasValue)
            {
                UserNameTextBlock.Text = userName;
                CoinsTextBlock.Text = userBalance.Value.ToString();
            }
            else
            {
                MessageBox.Show("Usuario no encontrado");
            }
        }

        private async Task ActualizarMonedas()
        {
            int? balance = await _userService.GetUserBalance();
            CoinsTextBlock.Text = balance?.ToString() ?? "0";
        }

        private async void BotonSobreNormal(object sender, RoutedEventArgs e)
        {
            var packResponse = await _cardService.ComprarSobre("estándar");
            if (packResponse != null)
            {
                List<Card> cards = ParsePackCards(packResponse);
                PackWindow packWindow = new PackWindow(cards);
                packWindow.ShowDialog();

                await ActualizarMonedas();
            }
            else
            {
                MessageBox.Show("Error al abrir el sobre.");
            }
        }

        private async void BotonSobreMejorado(object sender, RoutedEventArgs e)
        {
            var packResponse = await _cardService.ComprarSobre("mejorado");
            if (packResponse != null)
            {
                List<Card> cards = ParsePackCards(packResponse);
                PackWindow packWindow = new PackWindow(cards);
                packWindow.ShowDialog();

                // Actualiza las monedas después de la compra
                await ActualizarMonedas();
            }
            else
            {
                MessageBox.Show("Error al abrir el sobre.");
            }
        }

        private List<Card> ParsePackCards(JsonNode packResponse)
        {
            List<Card> cards = new List<Card>();
            var cardsArray = packResponse["cartas"].AsArray();
            foreach (var cardNode in cardsArray)
            {
                cards.Add(new Card
                {
                    Nombre = cardNode["nombre"].ToString(),
                    Apellidos = cardNode["apellidos"].ToString(),
                    Posicion = cardNode["posicion"].ToString(),
                    Equipo = cardNode["equipo"].ToString(),
                    Nacionalidad = cardNode["nacionalidad"].ToString(),
                    Imagen = apiService._url + cardNode["imagen"].ToString(),
                    Rareza = cardNode["rareza"].ToString()
                });
            }
            return cards;
        }

        private void BotonMercado(object sender, RoutedEventArgs e)
        {
            Mercado mercado = new Mercado();
            mercado.Show();
        }

        private void MiColeccionButton_Click(object sender, RoutedEventArgs e)
        {
            MiColeccion miColeccionWindow = new MiColeccion();
            miColeccionWindow.Show();
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            bool result = await apiService.Logout();
            if (result)
            {
                MessageBox.Show("Sesión cerrada exitosamente.");
                // Volver al login
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al cerrar sesión.");
            }
        }

        private void GoToBattleButton_Click(object sender, RoutedEventArgs e)
        {
            BattleWindow battleWindow = new BattleWindow();
            battleWindow.Show();
        }
    }
}


