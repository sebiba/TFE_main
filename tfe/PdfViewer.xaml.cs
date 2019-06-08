﻿using Newtonsoft.Json;
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
            listServer.ItemsSource = _pdf.GetPdf(ReadConf("pseudo"), ReadConf("password"));
            if(listServer.ItemsSource == new List<string>()) MessageBox.Show("une erreur de connection avec le serveur est survenue", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            pdfWebViewer.Navigate(new Uri(path));
        }

        private void UploadPdf(object sender, EventArgs e)
        {
            _pdf.UploadPdf(ReadConf("pseudo"), ReadConf("password"));
            listServer.ItemsSource = _pdf.GetPdf(ReadConf("pseudo"), ReadConf("password"));
            MessageBox.Show("Le pdf a bien été envoyé dans votre espace personnel enligne");
        }

        private void DownloadPdf(object sender, EventArgs e)
        {
            _pdf.DownloadPdf(ReadConf("pseudo"), ReadConf("password"), listServer.SelectedItem.ToString(), ReadConf("PartiFolder"));
        }

        private void SharePdf(object sender, EventArgs e)
        {
            if (_pdf.SharePdf(ReadConf("pseudo"), ReadConf("password"), listServer.SelectedItem.ToString(), ShareTo.Text))
            {
                MessageBox.Show("votre fichier à bien été partagé avec: " + ShareTo.Text, "partage", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Une erreur est survenue lors du partage.", "partage", MessageBoxButton.OK, MessageBoxImage.Error);
            };
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
