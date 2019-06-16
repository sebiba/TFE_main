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
        private readonly log4net.ILog _log;
        public PdfViewer(Frame nav, log4net.ILog logParam,  string path = "about:blank")
        {
            _frame = nav;
            _pdf = new Pdf(path);
            _log = logParam;
            InitializeComponent();
            try { 
                listServer.ItemsSource = _pdf.GetPdf(ReadConf("pseudo"), ReadConf("password"));
            }
            catch (IdentificationException)
            {
                _log.Warn("Failed to connect to the session on server");
                MessageBox.Show("une erreur d'authentification est survenue. Connectez vous depuis la page paramètres.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                _log.Error("L'utilisateur n'a pas encore de bibliotheque enligne. "+ex.Message);
                MessageBox.Show("Vous n'avez pas encore de pdf enregistré enligne", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            _log.Info("Show PDF page with the pdf: "+path);
            pdfWebViewer.Navigate(new Uri(path));
        }

        private void UploadPdf(object sender, EventArgs e)
        {
            try { 
                _pdf.UploadPdf(ReadConf("pseudo"), ReadConf("password"));
                listServer.ItemsSource = _pdf.GetPdf(ReadConf("pseudo"), ReadConf("password"));
                MessageBox.Show("Le pdf a bien été envoyé dans votre espace personnel en ligne");
                _log.Info("pdf uploaded to the server: " + _pdf._path);
            }
            catch (IdentificationException)
            {
                _log.Warn("Failed to connect to the session on server");
                MessageBox.Show("une erreur d'authentification est survenue. Connectez vous depuis la page paramètres.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (NullReferenceException)
            {
                _log.Warn("no pdf is used");
                MessageBox.Show("Aucun PDF n'est actif pour l'instant", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(Exception ex)
            {
                _log.Error("Error failed to upload a pdf: "+ex.Message);
                MessageBox.Show("Une erreur est survenue", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DownloadPdf(object sender, EventArgs e)
        {
            if(listServer.SelectedItem != null) { 
                _pdf.DownloadPdf(ReadConf("pseudo"), ReadConf("password"), listServer.SelectedItem.ToString(), ReadConf("PartiFolder"));
                _log.Debug("Download a pdf from the server: " + listServer.SelectedItem.ToString());
                MessageBox.Show("Votre pdf a bien été downloadé.", "Download", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SharePdf(object sender, EventArgs e)
        {
            if(listServer.SelectedItem != null) {
                try { 
                    if (_pdf.SharePdf(ReadConf("pseudo"), ReadConf("password"), listServer.SelectedItem.ToString(), ShareTo.Text))
                    {
                        _log.Debug("The pdf '"+ listServer.SelectedItem.ToString() + "' has been shared with the user: " + ShareTo.Text);
                        MessageBox.Show("votre fichier à bien été partagé avec: " + ShareTo.Text, "partage", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        _log.Error("Failed to share the pdf '"+ listServer.SelectedItem.ToString() + "' with the user: "+ ShareTo.Text);
                        MessageBox.Show("Une erreur est survenue lors du partage.", "partage", MessageBoxButton.OK, MessageBoxImage.Error);
                    };
                }
                catch
                {
                    _log.Error("Failed to share the pdf due to connection problem");
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
            catch (Exception ex)
            {
                _log.Error("Error key not found:" + key + "\tMessage:" + ex.Message);
                throw;
            }
        }
    }
}
