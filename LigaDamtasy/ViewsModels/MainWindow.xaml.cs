using System.Windows;
using LigaDAMtasy.ViewsModels;

namespace LigaDAMtasy
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _apiService;

        public MainWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string email = emailTextBox.Text;
            string password = passwordBox.Password;

            bool registerResult = await _apiService.Register(username, email, password);
            if (registerResult)
            {
                MessageBox.Show("Usuario registrado correctamente");
            }
            else
            {
                MessageBox.Show("Error en el registro");
            }
        }

        private void CerrarApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextBox.Text;
            string password = passwordBox.Password;

            bool loginResult = await _apiService.Login(email, password);
            if (loginResult)
            {
                PaginaPrincipal paginaPrincipal = new PaginaPrincipal();
                paginaPrincipal.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Email o contraseña incorrectos");
            }
        }
    }
}
