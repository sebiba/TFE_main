using Newtonsoft.Json;
using python;
using Requete;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace tfe
{
    /// <summary>
    /// Logique d'interaction pour PdfViewer.xaml
    /// </summary>
    public partial class PdfViewer : Page
    {
        private Frame _frame;
        private Pdf _pdf;
        public PdfViewer(Frame nav, string path = "about:blank")
        {
            _frame = nav;
            _pdf = new Pdf(path); 
            InitializeComponent();
            try { 
                listServer.ItemsSource = _pdf.GetPdf(ReadConf("pseudo"), ReadConf("password"));
            }
            catch (IdentificationException)
            {
                MessageBox.Show("une erreur d'authentification est survenue.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("une erreur d'accès est survenue sur le serveur.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            pdfWebViewer.Navigate(new Uri(path));
        }

        private void UploadPdf(object sender, EventArgs e)
        {
            try { 
                _pdf.UploadPdf(ReadConf("pseudo"), ReadConf("password"));
                listServer.ItemsSource = _pdf.GetPdf(ReadConf("pseudo"), ReadConf("password"));
                MessageBox.Show("Le pdf a bien été envoyé dans votre espace personnel en ligne");
            }
            catch (IdentificationException)
            {
                MessageBox.Show("Un Problème est survenu lors de la connection à votre compte en ligne", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Aucun PDF n'est actif pour l'instant", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("Une erreur est survenue", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DownloadPdf(object sender, EventArgs e)
        {
            if(listServer.SelectedItem != null) { 
                _pdf.DownloadPdf(ReadConf("pseudo"), ReadConf("password"), listServer.SelectedItem.ToString(), ReadConf("PartiFolder"));
                MessageBox.Show("Votre pdf a bien été downloadé.", "Download", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SharePdf(object sender, EventArgs e)
        {
            if(listServer.SelectedItem != null) {
                try { 
                    if (_pdf.SharePdf(ReadConf("pseudo"), ReadConf("password"), listServer.SelectedItem.ToString(), ShareTo.Text))
                    {
                        MessageBox.Show("votre fichier à bien été partagé avec: " + ShareTo.Text, "partage", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Une erreur est survenue lors du partage.", "partage", MessageBoxButton.OK, MessageBoxImage.Error);
                    };
                }
                catch
                {
                    MessageBox.Show("Une erreur est survenue lors de la connection à votre compte.", "partage", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string ReadConf(string key)
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;

                string[] arr = appSettings.GetValues(key);
                return arr[0];
            }
            catch
            {
                throw;
            }
        }
    }
}
