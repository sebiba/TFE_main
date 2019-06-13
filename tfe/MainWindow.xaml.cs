using Microsoft.Win32;
using NAudio.Wave;
using python;
using Requete;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;

namespace tfe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainWindow()
        {
            SplashScreen Splash = new SplashScreen(@"img\logo.png");
            Splash.Show(false);
            _log.Info("----- Start Session -----");
            if (!Directory.Exists("python")|| !Directory.Exists("Latex")|| !Directory.Exists("Lilypond"))
            {
                try { 
                    ZipFile.ExtractToDirectory("./data.zip", "./");
                    _log.Info("Data.zip has been unzipped");
                }
                catch(Exception ex)
                {
                    _log.Error("Erreur while unzip data.zip: "+ex.Message);
                    MessageBox.Show("Un élément inconnue à empèché le traitement de donner dans le dossier de l'application. Certaine fonctionnalités risque de ne pas fonctionner correctement.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            Splash.Close(new TimeSpan(5));
            InitializeComponent();
            Audio_Click();
        }

        private void Audio_Click(object sender = null, RoutedEventArgs e = null)
        {
            nav.Navigate(new input(nav, _log));
            TitleMenu.Content = "Gestion de l'audio";
        }

        private void Lily_Click(object sender, RoutedEventArgs e)
        {
            nav.Navigate(new lilypond(nav, _log));
            TitleMenu.Content = "Lilypond";
        }

        private void PDF_Click(object sender, RoutedEventArgs e)
        {
            nav.Navigate(new PdfViewer(nav, _log));
            TitleMenu.Content = "Gestion des PDF";
        }

        private void Param_Click(object sender, RoutedEventArgs e)
        {
            Config conf = new Config(_log);
            conf.ShowDialog();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://docs.google.com/document/d/1p7f-_XnWPieBqKoqPCSCYtwtNbvRD9kP0L4fHDpBh_I/edit?usp=sharing");
        }
    }
}
