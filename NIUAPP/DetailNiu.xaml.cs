using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;

namespace NIUAPP
{
    public partial class DetailWindow : Window
    {
        public DetailWindow(DataRowView selectedRow)
        {
            InitializeComponent();
            DisplayDetails(selectedRow);
        }

        // Affiche les détails de la ligne sélectionnée dans l'ItemsControl
        private void DisplayDetails(DataRowView selectedRow)
        {
            DetailsList.Items.Clear();
            foreach (DataColumn column in selectedRow.Row.Table.Columns)
            {
                string detail = $"{column.ColumnName}: {selectedRow[column.ColumnName].ToString()}";
                DetailsList.Items.Add(detail);
            }
        }

        // Gestionnaire d'événement pour exporter les détails en PDF
        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            // Configurer la boîte de dialogue de sauvegarde pour le fichier PDF
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Fichiers PDF (*.pdf)|*.pdf",
                Title = "Exporter les détails en PDF"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Créer un document PDF
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        Document document = new Document(PageSize.A4, 10, 10, 10, 10); // Ajouter des marges
                        PdfWriter writer = PdfWriter.GetInstance(document, stream);

                        document.Open();
                        // Ajouter un titre au PDF
                        var titleFont = FontFactory.GetFont("Arial", 20, Font.BOLD);
                        var bodyFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                        document.Add(new Paragraph("Détails de l'élément", titleFont));
                        document.Add(new Paragraph("\n"));

                        // Ajouter les détails de l'élément
                        foreach (var item in DetailsList.Items)
                        {
                            document.Add(new Paragraph(item.ToString(), bodyFont));
                        }

                        // Ajouter une ligne de séparation
                        document.Add(new Paragraph("\n-------------------------------\n", bodyFont));

                        document.Close();
                    }

                    // Message de confirmation
                    MessageBox.Show("Exportation réussie !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de l'exportation : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
