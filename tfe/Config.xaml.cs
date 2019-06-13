using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows;
using System.Windows.Forms;

namespace tfe
{
    /// <summary>
    /// Logique d'interaction pour Config.xaml
    /// </summary>
    public partial class Config : Window
    {
        private readonly log4net.ILog _log;

        public Config(log4net.ILog logParam)
        {
            _log = logParam;
            List<string> param = new List<string>() {"WavFolder", "LilyFolder", "LatexFolder", "TabFolder", "PartiFolder", "pseudo"};
            InitializeComponent();
            param.ForEach(delegate(string x) {
                ((System.Windows.Controls.TextBox)this.FindName(x)).Text = ReadConf(x);
            });
            password.Password = ReadConf("password");
            _log.Info("Show Configuration window");
        }

        private void WavFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                Description = "Selectionner le dossier qui contiendra les fichiers audio",
                ShowNewFolderButton = true
            };
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WavFolder.Text = folder.SelectedPath;
                WriteConf("WavFolder", folder.SelectedPath);
                _log.Debug("Change configuration value: WavFolder->"+ folder.SelectedPath);
            }
        }

        private void LilyFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                Description = "Selectionner le dossier qui contiendra les fichiers Lilypond",
                ShowNewFolderButton = true
            };
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LilyFolder.Text = folder.SelectedPath;
                WriteConf("LilyFolder", folder.SelectedPath);
                _log.Debug("Change configuration value: LilyFolder->"+ folder.SelectedPath);
            }
        }

        private void LatexFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                Description = "Selectionner le dossier qui contiendra les fichiers Latex",
                ShowNewFolderButton = true
            };
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LatexFolder.Text = folder.SelectedPath;
                WriteConf("LatexFolder", folder.SelectedPath);
                _log.Debug("Change configuration value: LatexFolder->"+ folder.SelectedPath);
            }
        }

        private void TabFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                Description = "Selectionner le dossier qui contiendra les tablatures",
                ShowNewFolderButton = true
            };
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TabFolder.Text = folder.SelectedPath;
                WriteConf("TabFolder", folder.SelectedPath);
                _log.Debug("Change configuration value: TabFolder->"+ folder.SelectedPath);
            }
        }

        private void PartiFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                Description = "Selectionner le dossier qui contiendra les partitions",
                ShowNewFolderButton = true
            };
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PartiFolder.Text = folder.SelectedPath;
                WriteConf("PartiFolder", folder.SelectedPath);
                _log.Debug("Change configuration value: PartFolder->"+ folder.SelectedPath);
            }
        }

        private void Connection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Requete.Request.GetToken(pseudo.Text, password.Password);
                WriteConf("pseudo", pseudo.Text);
                WriteConf("password", password.Password);
                _log.Debug("Test connection successfull pseudo:"+pseudo.Text);
                System.Windows.MessageBox.Show("Identification Correct, identifiant enregistré", "Validation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Requete.IdentificationException)
            {
                _log.Warn("Test connection error: Wrong password");
                System.Windows.MessageBox.Show("Erreur lors du test d'authentification avec le server. Verifier Votre mot de passe et votre connection internet.\n", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _log.Error("Error test connection: " + ex.Message);
                System.Windows.MessageBox.Show("Erreur est survenue.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            };            
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Pour vous créer un compte, il faut vous rendre à cette addresse https://tfe.moovego.be/Account/Register", "connection", MessageBoxButton.OK, MessageBoxImage.Hand);
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
            _log.Debug("Write New Configuration Value: "+key+"->"+value);
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
