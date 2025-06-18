using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using LigaDAMtasy.Models;
using LigaDAMtasy.ViewsModels;

namespace LigaDAMtasy
{
    public partial class MiColeccion : Window
    {
        private List<Card> userCards = new List<Card>();
        private readonly ApiService _apiService;
        private readonly CardService _cardService;
        private readonly UserService _userService;
        private DispatcherTimer coinUpdateTimer;

        public MiColeccion()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _cardService = new CardService(_apiService);
            _userService = new UserService(_apiService);
            CargarColeccion();
            ActualizarMonedas();
            IntervaloActualizarMonedas();
        }

        private void IntervaloActualizarMonedas()
        {
            coinUpdateTimer = new DispatcherTimer();
            coinUpdateTimer.Interval = TimeSpan.FromSeconds(10);
            coinUpdateTimer.Tick += (s, e) => ActualizarMonedas();
            coinUpdateTimer.Start();
        }

        private async void ActualizarMonedas()
        {
            int? balance = await _userService.GetUserBalance();
            CoinBalanceLabel.Content = balance?.ToString() ?? "0";
        }

        private async void CargarColeccion()
        {
            // Llamada al endpoint /my_collection
            userCards = await _userService.GetUserCollection();
            CollectionListBox.ItemsSource = userCards;
        }


        private void InfoColeccion(object sender, SelectionChangedEventArgs e)
        {
            if (CollectionListBox.SelectedItem is Card selectedCard)
            {
                try
                {
                    SelectedCardImage.Source = new BitmapImage(new Uri(selectedCard.Imagen));
                }
                catch (Exception)
                {
                    SelectedCardImage.Source = null;
                }
                SelectedCardDetails.Text = $"Nombre: {selectedCard.Nombre} {selectedCard.Apellidos}\n" +
                                             $"Posición: {selectedCard.Posicion}\n" +
                                             $"Equipo: {selectedCard.Equipo}\n" +
                                             $"Nacionalidad: {selectedCard.Nacionalidad}\n" +
                                             $"Rareza: {selectedCard.Rareza}\n" +
                                             $"Ataque: {selectedCard.Ataque}\n" +
                                             $"Defensa: {selectedCard.Defensa}\n";


            }
        }

        private async void BotonVender(object sender, RoutedEventArgs e)
        {
            if (CollectionListBox.SelectedItem is Card selectedCard)
            {
                if (int.TryParse(SellQuantityTextBox.Text, out int quantity) && quantity > 0)
                {
                    bool sellResult = await _cardService.VenderCarta(selectedCard, quantity);
                    if (sellResult)
                    {
                        MessageBox.Show("Carta vendida exitosamente");
                        CargarColeccion();  
                        ActualizarMonedas();   
                    }
                    else
                    {
                        MessageBox.Show("Error al vender la carta. Revisa la cantidad de cartas que tienes o inténtalo nuevamente.");
                    }
                }
                else
                {
                    MessageBox.Show("Introduce una cantidad válida.");
                }
            }
            else
            {
                MessageBox.Show("Selecciona una carta para vender.");
            }
        }

        private void BotonFiltrar(object sender, RoutedEventArgs e)
        {
            string rarezaFiltro = (RarezaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString().ToLower() ?? "todas";
            string equipoFiltro = EquipoTextBox.Text.Trim().ToLower();
            string nombreFiltro = NombreTextBox.Text.Trim().ToLower();

            var filteredCards = userCards;

            if (rarezaFiltro != "todas")
            {
                filteredCards = filteredCards.FindAll(card => card.Rareza.ToLower() == rarezaFiltro);
            }
            if (!string.IsNullOrEmpty(equipoFiltro))
            {
                filteredCards = filteredCards.FindAll(card => card.Equipo.ToLower().Contains(equipoFiltro));
            }
            if (!string.IsNullOrEmpty(nombreFiltro))
            {
                filteredCards = filteredCards.FindAll(card =>
                    card.Nombre.ToLower().Contains(nombreFiltro) || card.Apellidos.ToLower().Contains(nombreFiltro));
            }

            CollectionListBox.ItemsSource = filteredCards;
        }
    }

    
   
}
