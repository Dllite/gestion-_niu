using System;
using System.Linq;
using System.Collections.Generic;

using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ExcelDataReader;
using System.Threading.Tasks;

namespace NIUAPP
{
    public partial class ArchivesPage : Window
    {
        private DataTable selectedFileDataTable;
        private string archivesFolderPath;

        public ArchivesPage()
        {
            InitializeComponent();
            archivesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Archives");
            LoadArchives();
        }

        // Chargement des fichiers dans le dossier "Archives"
        private void LoadArchives()
        {
            if (!Directory.Exists(archivesFolderPath))
            {
                MessageBox.Show("Le dossier des archives est introuvable.");
                return;
            }

            var archiveFiles = Directory.GetFiles(archivesFolderPath, "*.xlsx");

            // Lister les fichiers d'archives dans le ListBox ou DataGrid
            ArchiveFilesListBox.Items.Clear(); // Ou DataGrid
            foreach (var file in archiveFiles)
            {
                ArchiveFilesListBox.Items.Add(Path.GetFileName(file)); // Ajouter le nom du fichier
            }
        }

        // Gestionnaire d'événement pour sélectionner et afficher le contenu d'un fichier
        private async void ArchiveFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArchiveFilesListBox.SelectedItem == null)
                return;

            string selectedFileName = ArchiveFilesListBox.SelectedItem.ToString();
            string filePath = Path.Combine(archivesFolderPath, selectedFileName);

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Le fichier sélectionné est introuvable.");
                return;
            }

            // Afficher le ProgressBar et désactiver les interactions utilisateur
            LoadingIndicator.Visibility = Visibility.Visible;
            DataGridTable.IsEnabled = false;

            try
            {
                await Task.Run(() =>
                {
                    // Lire le fichier Excel
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true // Utiliser la première ligne comme en-tête
                                }
                            });
                            selectedFileDataTable = result.Tables[0]; // Charger la première feuille de calcul
                        }
                    }
                });

                // Afficher les données dans le DataGrid
                DataGridTable.ItemsSource = selectedFileDataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement du fichier : " + ex.Message);
            }
            finally
            {
                // Cacher le ProgressBar et réactiver les interactions utilisateur
                LoadingIndicator.Visibility = Visibility.Collapsed;
                DataGridTable.IsEnabled = true;
            }
        }

        // Gestionnaire d'événement pour rechercher dans le fichier sélectionné
        // Gestionnaire d'événement pour rechercher dans le fichier sélectionné
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFileDataTable != null)
            {
                string searchValue = SearchTextBox.Text;

                if (!string.IsNullOrEmpty(searchValue))
                {
                    // Séparer les valeurs de recherche par une virgule (ex: "1234,5678,91011")
                    string[] searchValues = searchValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                       .Select(val => val.Trim()).ToArray();

                    // Créer un filtre qui cherche chaque valeur dans la colonne "Niu"
                    string filter = string.Join(" OR ", searchValues.Select(val => $"Convert([Niu], 'System.String') LIKE '%{val}%'"));

                    // Appliquer le filtre
                    DataView dataView = selectedFileDataTable.DefaultView;
                    dataView.RowFilter = filter;

                    // Mettre à jour la source de données du DataGrid
                    DataGridTable.ItemsSource = dataView;

                    // Obtenir les valeurs réellement trouvées après filtrage
                    List<string> foundValues = new List<string>();
                    foreach (DataRowView row in dataView)
                    {
                        foundValues.Add(row["Niu"].ToString());
                    }

                    // Identifier les valeurs non trouvées
                    var notFoundValues = searchValues.Where(val => !foundValues.Contains(val)).ToList();

                    // Afficher un message des résultats
                    if (notFoundValues.Count > 0)
                    {
                        MessageBox.Show($"Les éléments suivants n'ont pas été trouvés : {string.Join(", ", notFoundValues)}", "Résultats de la recherche", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Tous les éléments recherchés ont été trouvés.", "Résultats de la recherche", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    // Si le champ de recherche est vide, afficher tous les éléments
                    DataView dataView = selectedFileDataTable.DefaultView;
                    dataView.RowFilter = ""; // Supprimer le filtre pour afficher toutes les lignes
                    DataGridTable.ItemsSource = dataView;

                    MessageBox.Show("Aucun critère de recherche fourni. Affichage de tous les éléments.", "Recherche", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            else
            {
                MessageBox.Show("Veuillez d'abord sélectionner un fichier dans les archives.");
            }
        }


        // Gestionnaire d'événement pour afficher les détails d'une ligne
        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridTable.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)DataGridTable.SelectedItem;
                DetailWindow detailWindow = new DetailWindow(selectedRow);
                detailWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une ligne dans le tableau.");
            }
        }
    }
}
