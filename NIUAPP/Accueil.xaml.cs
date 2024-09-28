using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ExcelDataReader;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Threading.Tasks;

namespace NIUAPP
{
    public partial class Accueil : Window
    {
        private DataTable excelDataTable; // Pour stocker les données Excel
        private string archivesFolderPath;

        public Accueil()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événement pour "Importer un fichier"
        private async void MenuItem_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Fichiers Excel (*.xlsx)|*.xlsx|Tous les fichiers (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                if (string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show("Le chemin du fichier est vide.");
                    return;
                }

                // Chemin du dossier Archives
                archivesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Archives");

                // Vérifier si le dossier Archives existe, sinon le créer
                if (!Directory.Exists(archivesFolderPath))
                {
                    Directory.CreateDirectory(archivesFolderPath);
                }

                // Afficher le ProgressBar et désactiver les interactions utilisateur
                LoadingIndicator.Visibility = Visibility.Visible;
                DataGridTable.IsEnabled = false;

                try
                {
                    await ImportExcelFileAsync(filePath);

                    // Afficher les données dans le DataGrid
                    DataGridTable.ItemsSource = excelDataTable.DefaultView;
                    MessageBox.Show("Fichier importé et copié dans les archives : " + openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de l'importation du fichier : " + ex.Message);
                }
                finally
                {
                    // Cacher le ProgressBar et réactiver les interactions utilisateur
                    LoadingIndicator.Visibility = Visibility.Collapsed;
                    DataGridTable.IsEnabled = true;
                }
            }
        }

        // Méthode pour importer le fichier Excel de manière asynchrone
        private async Task ImportExcelFileAsync(string filePath)
        {
            await Task.Run(() =>
            {
                try
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
                            excelDataTable = result.Tables[0]; // Charger la première feuille de calcul
                        }
                    }

                    // Copier le fichier importé dans le dossier Archives
                    string fileName = Path.GetFileName(filePath);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        throw new ArgumentNullException(nameof(fileName), "Le nom du fichier est vide.");
                    }

                    string destinationPath = Path.Combine(archivesFolderPath, fileName);
                    File.Copy(filePath, destinationPath, true); // true pour écraser si existe
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => MessageBox.Show("Erreur lors de la lecture du fichier : " + ex.Message));
                    throw;
                }
            });
        }

        // Gestionnaire d'événement pour "Rechercher"
        // Gestionnaire d'événement pour "Rechercher"
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (excelDataTable != null)
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
                    DataView dataView = excelDataTable.DefaultView;
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
                    MessageBox.Show("Veuillez entrer une ou plusieurs valeurs à rechercher.");
                }
            }
            else
            {
                MessageBox.Show("Veuillez d'abord importer un fichier.");
            }
        }



        // Gestionnaire d'événement pour "Voir les détails"
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

        // Gestionnaire d'événement pour "Exporter en PDF"
        private void ExportToPDF_Click(object sender, RoutedEventArgs e)
        {
            if (excelDataTable != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Fichiers PDF (*.pdf)|*.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        Document pdfDoc = new Document();
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        // Ajout d'un titre
                        pdfDoc.Add(new Paragraph("Données Exportées"));
                        pdfDoc.Add(new Paragraph(DateTime.Now.ToString()));

                        // Création d'un tableau pour les données
                        PdfPTable pdfTable = new PdfPTable(excelDataTable.Columns.Count);
                        foreach (DataColumn column in excelDataTable.Columns)
                        {
                            pdfTable.AddCell(column.ColumnName);
                        }
                        foreach (DataRow row in excelDataTable.Rows)
                        {
                            foreach (var item in row.ItemArray)
                            {
                                pdfTable.AddCell(item.ToString());
                            }
                        }
                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                    }

                    MessageBox.Show("Données exportées en PDF : " + saveFileDialog.FileName);
                }
            }
            else
            {
                MessageBox.Show("Veuillez d'abord importer un fichier.");
            }
        }

        // Gestionnaire d'événement pour "Voir les archives"
        private void MenuItem_Archives_Click(object sender, RoutedEventArgs e)
        {
            ArchivesPage archivesPage = new ArchivesPage();
            archivesPage.ShowDialog();
        }

        // Gestionnaire d'événement pour "Préférences"
        private void MenuItem_Preferences_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ouverture des préférences...");
        }

        // Gestionnaire d'événement pour "À propos"
        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Application Dufe_Pat\nVersion 1.0");
        }
    }
}
