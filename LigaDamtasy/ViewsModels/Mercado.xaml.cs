using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LigaDAMtasy.Models;
using LigaDAMtasy.ViewsModels;

namespace LigaDAMtasy
{
    public partial class Mercado : Window
    {
        private readonly CardService _cardService;
        private readonly ApiService apiService = new ApiService();
        private List<Card> allCards = new List<Card>();

        public Mercado()
        {
            InitializeComponent();
            _cardService = new CardService(apiService);
            CargarCartas();
            this.Activated += Mercado_Activated; // para actualizar las monedas al volver a la ventana
        }

        private async void Mercado_Activated(object sender, System.EventArgs e)
        {
            await ActualizarMonedasUsuario();
        }

        private async void CargarCartas()
        {
            // llamamos al endpoint /cards
            JsonNode? cardsResponse = await _cardService.ObtenerCartasDisponibles();
            if (cardsResponse != null)
            {
                List<Card> cards = ParseCards(cardsResponse);
                allCards = cards; // Guarda la lista completa para filtrar
                AvailableCardsListBox.ItemsSource = cards;
            }
            else
            {
                MessageBox.Show("Error al cargar las cartas disponibles");
            }
        }

        private List<Card> ParseCards(JsonNode cardsResponse)
        {
            List<Card> cards = new List<Card>();
            foreach (var cardNode in cardsResponse.AsArray())
            {
                cards.Add(new Card
                {
                    Nombre = cardNode["nombre"]?.ToString(),
                    Apellidos = cardNode["apellidos"]?.ToString(),
                    Posicion = cardNode["posicion"]?.ToString(),
                    Equipo = cardNode["equipo"]?.ToString(),
                    Nacionalidad = cardNode["nacionalidad"]?.ToString(),
                    Imagen = apiService._url + cardNode["imagen"]?.ToString(),
                    Rareza = cardNode["rareza"]?.ToString(),
                    Ataque = cardNode["ataque"] != null ? int.Parse(cardNode["ataque"].ToString()) : 0,
                    Defensa = cardNode["defensa"] != null ? int.Parse(cardNode["defensa"].ToString()) : 0
                });
            }
            return cards;
        }


        private async void BotonComprarCarta(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Card selectedCard)
            {
                // Confirmación de compra
                var result = MessageBox.Show(
                    $"¿Deseas comprar la carta {selectedCard.Nombre} {selectedCard.Apellidos}?",
                    "Confirmar Compra", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool buyResult = await _cardService.ComprarCarta(selectedCard.Nombre, selectedCard.Apellidos);
                    if (buyResult)
                    {
                        MessageBox.Show("Carta comprada");
                        // Actualiza las monedas después de la compra
                        await ActualizarMonedasUsuario();
                    }
                    else
                    {
                        MessageBox.Show("Error al comprar la carta. Revisa tus monedas o inténtalo de nuevo.");
                    }
                }
            }
        }

        private async Task ActualizarMonedasUsuario()
        {
            UserService userService = new UserService(apiService);
            int? balance = await userService.GetUserBalance();
            // Actualiza el label de monedas
            if (BalanceLabel != null)
            {
                BalanceLabel.Text = balance?.ToString() ?? "0";
            }
        }

        private void BotonFiltrar(object sender, RoutedEventArgs e)
        {
            string rarezaFiltro = (RarezaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todas";
            string equipoFiltro = EquipoTextBox.Text.Trim().ToLower();


            string nombreFiltro = NombreTextBox.Text.Trim().ToLower();

            var filteredCards = allCards;

            // Filtro por rareza (ignora si es "Todas")
            if (!string.Equals(rarezaFiltro, "Todas", StringComparison.OrdinalIgnoreCase))
            {
                filteredCards = filteredCards.FindAll(card =>
                    string.Equals(card.Rareza, rarezaFiltro, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por equipo
            if (!string.IsNullOrEmpty(equipoFiltro))
            {
                filteredCards = filteredCards.FindAll(card =>
                    card.Equipo != null && card.Equipo.ToLower().Contains(equipoFiltro));
            }

            // Filtro por nombre o apellidos
            if (!string.IsNullOrEmpty(nombreFiltro))
            {
                filteredCards = filteredCards.FindAll(card =>
                    (card.Nombre != null && card.Nombre.ToLower().Contains(nombreFiltro)) ||
                    (card.Apellidos != null && card.Apellidos.ToLower().Contains(nombreFiltro)));
            }

            AvailableCardsListBox.ItemsSource = filteredCards;
        }



        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CardsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AvailableCardsListBox.SelectedItem is Card selectedCard)
            {
                MessageBox.Show($"Carta seleccionada: {selectedCard.Nombre} {selectedCard.Apellidos}");
            }
        }
    }
}


