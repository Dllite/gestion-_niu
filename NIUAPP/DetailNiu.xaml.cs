using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32; // Ajoutez cette ligne


namespace NIUAPP
{
    public partial class DetailWindow : Window
    {
        public DetailWindow(DataRowView selectedRow)
        {
            InitializeComponent();
            DisplayDetails(selectedRow);
        }

        private void DisplayDetails(DataRowView selectedRow)
        {
            // Affiche les détails dans l'ItemsControl
            foreach (DataColumn column in selectedRow.Row.Table.Columns)
            {
                string detail = $"{column.ColumnName}: {selectedRow[column.ColumnName].ToString()}";
                DetailsList.Items.Add(detail);
            }
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Fichiers PDF (*.pdf)|*.pdf",
                Title = "Exporter en PDF"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    Document document = new Document();
                    PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    document.Open();

                    document.Add(new Paragraph("Détails de l'élément", FontFactory.GetFont("Arial", 20, Font.BOLD)));

                    foreach (var item in DetailsList.Items)
                    {
                        document.Add(new Paragraph(item.ToString()));
                    }

                    document.Close();
                    MessageBox.Show("Exportation réussie !");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de l'exportation : " + ex.Message);
                }
            }
        }
    }
}
