using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LigaDAMtasy.Models;

namespace LigaDAMtasy
{
    public partial class PackWindow : Window
    {
        public PackWindow(List<Card> cards)
        {
            InitializeComponent();
            CardsListBox.ItemsSource = cards;
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

        private void OpenAnotherPack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cerrar la ventana actual
                this.Close();

                // Volver a la página principal donde están los botones de sobres
                var paginaPrincipal = new PaginaPrincipal();
                paginaPrincipal.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al abrir la página principal: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cerrar la ventana actual
                this.Close();

                // Volver a la página principal
                var paginaPrincipal = new PaginaPrincipal();
                paginaPrincipal.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al volver al menú principal: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}