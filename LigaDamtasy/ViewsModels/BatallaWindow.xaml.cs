using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using LigaDAMtasy.ViewsModels;
using System.Windows.Media.Imaging;

namespace LigaDAMtasy
{
    public partial class BattleWindow : Window
    {
        private readonly CardService _cardService;
        private readonly ApiService _apiService;

        public BattleWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _cardService = new CardService(_apiService);

            // Establecer la ubicación de inicio de la ventana (asi evitamos spamear batallas)
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = 100; 
            Top = 100;  

            EmpezarPartida();
        }

        private async void EmpezarPartida()
        {
            JsonNode? battleResponse = await _cardService.Partida();
            if (battleResponse != null)
            {
                string resultado = battleResponse["mensaje"].ToString();
                BattleResultTextBlock.Text = resultado;

                var userCard = battleResponse["user_card"];
                var opponentCard = battleResponse["random_card"];

                string userCardImageUrl = _apiService._url + userCard["imagen"].ToString();
                if (!string.IsNullOrEmpty(userCardImageUrl))
                {
                    UserCardImage.Source = new BitmapImage(new Uri(userCardImageUrl));
                }

                UserCardInfoTextBlock.Text = $"Nombre: {userCard["nombre"]} {userCard["apellidos"]}\n" +
                                             $"Posición: {userCard["posicion"]}\n" +
                                             $"Equipo: {userCard["equipo"]}\n";

                string opponentCardImageUrl = _apiService._url + opponentCard["imagen"].ToString();
                if (!string.IsNullOrEmpty(opponentCardImageUrl))
                {
                    OpponentCardImage.Source = new BitmapImage(new Uri(opponentCardImageUrl));
                }

                OpponentCardInfoTextBlock.Text = $"Nombre: {opponentCard["nombre"]} {opponentCard["apellidos"]}\n" +
                                                 $"Posición: {opponentCard["posicion"]}\n" +
                                                 $"Equipo: {opponentCard["equipo"]}\n";
                                              

                // Mostrar mensaje de resultado
                if (resultado.ToLower().Contains("ganado"))
                {
                    MessageBox.Show("¡Has ganado!");
                }
                else if (resultado.ToLower().Contains("perdido"))
                {
                    MessageBox.Show("Has perdido...");
                }
            }
            else
            {
                MessageBox.Show("Error al iniciar la batalla.");
                Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

