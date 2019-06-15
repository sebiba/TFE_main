using Microsoft.Win32;
using NAudio.Wave;
using python;
using Requete;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace tfe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
            IsConnected();
            Version.Content = Assembly.GetExecutingAssembly().GetName().Version;
            if (ReadConf("WavFolder") == "" && ReadConf("LilyFolder") == "" && ReadConf("LatexFolder") == "" && ReadConf("TabFolder") == "" && ReadConf("PartiFolder") == "") { 
                foreach(string key in new List<string> { "WavFolder", "LilyFolder", "LatexFolder", "TabFolder", "PartiFolder" }) //init document folder
                {
                    WriteConf(key, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\DiaTab\"+key);
                    if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DiaTab\" + key))
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DiaTab\" + key);
                    }
                }
            }
            Audio_Click();
        }

        private void Audio_Click(object sender = null, RoutedEventArgs e = null)
        {
            nav.Navigate(new input(nav, TitleMenu, _log));
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
            IsConnected();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://docs.google.com/document/d/1p7f-_XnWPieBqKoqPCSCYtwtNbvRD9kP0L4fHDpBh_I/edit?usp=sharing");
        }

        private void WriteConf(string key, string value)
        {
            // Open App.Config of executable
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Add an Application Setting.
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            // Save the configuration file.
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            _log.Debug("Write New Configuration Value: " + key + "->" + value);

            ConfigurationSection section = config.GetSection("appSettings");

            if (section != null && !section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                section.SectionInformation.ForceSave = true;
                _log.Info("appSettings is now encrypted");
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("configuration");
                ResetConfigMechanism();
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

        private void IsConnected()
        {
            if(ReadConf("pseudo") != "")
            {
                identifiant.Content = ReadConf("pseudo");
                identifiant.Visibility = Visibility.Visible;
            }
            else
            {
                identifiant.Visibility = Visibility.Collapsed;
            }
        }

        private void ResetConfigMechanism()
        {
            typeof(ConfigurationManager)
                .GetField("s_initState", BindingFlags.NonPublic |
                                         BindingFlags.Static)
                .SetValue(null, 0);

            typeof(ConfigurationManager)
                .GetField("s_configSystem", BindingFlags.NonPublic |
                                            BindingFlags.Static)
                .SetValue(null, null);

            typeof(ConfigurationManager)
                .Assembly.GetTypes()
                .Where(x => x.FullName ==
                            "System.Configuration.ClientConfigPaths")
                .First()
                .GetField("s_current", BindingFlags.NonPublic |
                                       BindingFlags.Static)
                .SetValue(null, null);
        }
    }
}
