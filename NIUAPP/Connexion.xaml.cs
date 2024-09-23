using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace NIUAPP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les informations d'identification
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            // Exemple simple de vérification des identifiants (à remplacer par une vraie validation)
            if (email == "admin@gmail.com" && password == "password")
            {
                Accueil accueil = new Accueil();
                accueil.Show();

                // Fermer la fenêtre actuelle (MainWindow)
                this.Close();
                // Code pour rediriger vers une autre fenêtre ou effectuer d'autres actions
            }
            else
            {
                ErrorMessage.Visibility = Visibility.Visible; // Affiche le message d'erreur
            }
        }
    }
}

