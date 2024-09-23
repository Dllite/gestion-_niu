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

namespace NIUAPP
{
    public partial class Accueil : Window
    {
        private DataTable excelDataTable; // Pour stocker les données Excel

        public Accueil()
        {
            InitializeComponent();
        }

        // Gestionnaire d'événement pour "Importer un fichier"
        private void MenuItem_Import_Click(object sender, RoutedEventArgs e)
        {
            // Ouvre une boîte de dialogue pour sélectionner un fichier Excel
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers Excel (*.xlsx)|*.xlsx|Tous les fichiers (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    // Lire le fichier Excel et l'afficher dans le DataGrid
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
                            DataGridTable.ItemsSource = excelDataTable.DefaultView; // Afficher dans le DataGrid
                        }
                    }

                    MessageBox.Show("Fichier importé : " + openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de l'importation du fichier : " + ex.Message);
                }
            }
        }

        // Gestionnaire d'événement pour "Rechercher"
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (excelDataTable != null)
            {
                string searchValue = SearchTextBox.Text;

                // Filtrer les lignes par NIU
                DataView dataView = excelDataTable.DefaultView;
                dataView.RowFilter = string.Format("Convert([Niu], 'System.String') LIKE '%{0}%'", searchValue);
                DataGridTable.ItemsSource = dataView;
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
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Fichiers PDF (*.pdf)|*.pdf";
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
            MessageBox.Show("Affichage des archives...");
        }

        // Gestionnaire d'événement pour "Préférences"
        private void MenuItem_Preferences_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ouverture des préférences...");
        }

        // Gestionnaire d'événement pour "À propos"
        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Application NIUAPP\nVersion 1.0");
        }
    }
}
