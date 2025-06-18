using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LigaDAMtasy.Models;

namespace LigaDAMtasy
{
    public partial class PackWindow : Window
    {
        private readonly Func<Task<List<Card>>> _comprarSobreAsync;

        public PackWindow(List<Card> cards, Func<Task<List<Card>>> comprarSobreAsync)
        {
            InitializeComponent();
            CardsListBox.ItemsSource = cards;
            _comprarSobreAsync = comprarSobreAsync;
        }

        private void CardsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CardsListBox.SelectedItem is Card selectedCard)
            {
                CardDetailsLabel.Text = $"Nombre: {selectedCard.Nombre} {selectedCard.Apellidos}\n" +
                                             $"Posición: {selectedCard.Posicion}\n" +
                                             $"Equipo: {selectedCard.Equipo}\n" +
                                             $"Nacionalidad: {selectedCard.Nacionalidad}\n" +
                                             $"Rareza: {selectedCard.Rareza}";
                                           
            }
            else
            {
                CardDetailsLabel.Text = string.Empty;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void OpenAnotherPack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var nuevasCartas = await _comprarSobreAsync();
                if (nuevasCartas != null && nuevasCartas.Count > 0)
                {
                    var nuevoPackWindow = new PackWindow(nuevasCartas, _comprarSobreAsync);
                    nuevoPackWindow.Show();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir otro sobre: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
                var paginaPrincipal = new PaginaPrincipal();
                paginaPrincipal.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al volver al menú principal: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
