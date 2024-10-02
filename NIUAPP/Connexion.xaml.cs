using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Input;
namespace NIUAPP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           // CopierBaseDeDonneesSiNecessaire();
        }

       // private void CopierBaseDeDonneesSiNecessaire()
       // {
            // Chemin de la base de données dans le répertoire de débogage
            //string sourcePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");

            // Chemin vers le dossier AppData de l'utilisateur
            //string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //string targetPath = System.IO.Path.Combine(appDataPath, "MonApp", "database.db");

            // Vérifie si la base de données existe déjà dans AppData
            //if (!System.IO.File.Exists(targetPath))
            //{
                // Créer le répertoire dans AppData si nécessaire
                //string targetDir = System.IO.Path.GetDirectoryName(targetPath);
                //if (!System.IO.Directory.Exists(targetDir))
                //{
                  //  System.IO.Directory.CreateDirectory(targetDir);
                //}

                // Copier la base de données vers le répertoire AppData
                //System.IO.File.Copy(sourcePath, targetPath);
            //}

            // Maintenant que la base de données est en place, tu peux utiliser targetPath pour tes connexions
        //}

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close(); // Ferme la fenêtre actuelle
        }

        // Définir une liste d'utilisateurs avec emails et mots de passe
        private List<(string email, string password)> utilisateurs = new List<(string, string)>
        {
            ("admin@gmail.com", "password"),
            ("user1@gmail.com", "password1"),
            ("user2@gmail.com", "password2")
        };

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les informations d'identification
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            //string connectionString = "Data Source=database.db;Version=3;"; // Chemin relatif

            //using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            //{
            // try
            //{
            //  connection.Open(); // Ouvre la connexion

            // Créer la table Users si elle n'existe pas
            //string createTableQuery = @"
            //CREATE TABLE IF NOT EXISTS Users (
            //  id INTEGER PRIMARY KEY AUTOINCREMENT,
            //email TEXT NOT NULL,
            // password TEXT NOT NULL
            //);";

            //  SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection);
            //createTableCommand.ExecuteNonQuery();

            // Insérer un utilisateur de test si besoin
            // string insertTestUserQuery = "INSERT INTO Users (email, password) VALUES ('admin@gmail.com', 'password');";
            // SQLiteCommand insertCommand = new SQLiteCommand(insertTestUserQuery, connection);
            //insertCommand.ExecuteNonQuery();

            // Vérification des identifiants
            //string query = "SELECT * FROM Users WHERE email = @Email AND password = @Password;";
            //SQLiteCommand command = new SQLiteCommand(query, connection);
            //command.Parameters.AddWithValue("@Email", email);
            //command.Parameters.AddWithValue("@Password", password);

            //using (SQLiteDataReader reader = command.ExecuteReader())
            //{

            // Vérifier si les informations d'identification correspondent à un utilisateur dans la liste
            bool isAuthenticated = utilisateurs.Any(user => user.email == email && user.password == password);
            if (isAuthenticated)
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

            //}

            //connection.Close(); // Fermer la connexion
            //}
            //catch (Exception ex)
            //{
            //  MessageBox.Show("Erreur lors de la connexion à la base de données : " + ex.Message);
            //}
            //}
        }

    }
}
