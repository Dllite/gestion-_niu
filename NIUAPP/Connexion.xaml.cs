using MaterialDesignThemes.Wpf;
using System;
using System.Data.SQLite;
using System.Windows;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Windows.Input;
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
            string connectionString = "Data Source=database.db;Version=3;"; // Chemin relatif

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open(); // Ouvre la connexion
                    
                    // Créer la table Users si elle n'existe pas
                    string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    email TEXT NOT NULL,
                    password TEXT NOT NULL
                );";

                    SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection);
                    createTableCommand.ExecuteNonQuery();

                    // Insérer un utilisateur de test si besoin
                    string insertTestUserQuery = "INSERT INTO Users (email, password) VALUES ('admin@gmail.com', 'password');";
                    SQLiteCommand insertCommand = new SQLiteCommand(insertTestUserQuery, connection);
                    insertCommand.ExecuteNonQuery();

                    // Vérification des identifiants
                    string query = "SELECT * FROM Users WHERE email = @Email AND password = @Password;";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // Identifiants corrects
                            Accueil accueil = new Accueil();
                            accueil.Show();
                            this.Close();
                        }
                        else
                        {
                            // Identifiants incorrects
                            ErrorMessage.Visibility = Visibility.Visible;
                        }
                    }

                    connection.Close(); // Fermer la connexion
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la connexion à la base de données : " + ex.Message);
                }
            }
        }

    }
}
