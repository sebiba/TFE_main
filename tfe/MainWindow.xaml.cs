using Microsoft.Win32;
using NAudio.Wave;
using python;
using Requete;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace tfe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Audio_Click();
        }

        private void Audio_Click(object sender = null, RoutedEventArgs e = null)
        {
            nav.Navigate(new input(nav));
            TitleMenu.Content = "Gestion de l'audio";
        }

        private void Lily_Click(object sender, RoutedEventArgs e)
        {
            nav.Navigate(new lilypond(nav));
            TitleMenu.Content = "Lilypond";
        }

        private void PDF_Click(object sender, RoutedEventArgs e)
        {
            nav.Navigate(new PdfViewer(nav));
            TitleMenu.Content = "Gestion des PDF";
        }

        private void Param_Click(object sender, RoutedEventArgs e)
        {
            Config conf = new Config();
            conf.ShowDialog();
        }
    }
}
