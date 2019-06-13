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
        public Config()
        {
            List<string> param = new List<string>() {"WavFolder", "LilyFolder", "LatexFolder", "TabFolder", "PartiFolder", "pseudo"};
            InitializeComponent();
            param.ForEach(delegate(string x) {
                ((System.Windows.Controls.TextBox)this.FindName(x)).Text = ReadConf(x);
            });
            password.Password = ReadConf("password");
        }

        private void WavFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.RootFolder = Environment.SpecialFolder.Desktop;
            folder.Description = "Selectionner le dossier qui contiendra les fichiers audio";
            folder.ShowNewFolderButton = true;
            if(folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WavFolder.Text = folder.SelectedPath;
                WriteConf("WavFolder", folder.SelectedPath);
            }
        }

        private void LilyFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.RootFolder = Environment.SpecialFolder.Desktop;
            folder.Description = "Selectionner le dossier qui contiendra les fichiers Lilypond";
            folder.ShowNewFolderButton = true;
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LilyFolder.Text = folder.SelectedPath;
                WriteConf("LilyFolder", folder.SelectedPath);
            }
        }

        private void LatexFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.RootFolder = Environment.SpecialFolder.Desktop;
            folder.Description = "Selectionner le dossier qui contiendra les fichiers Latex";
            folder.ShowNewFolderButton = true;
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LatexFolder.Text = folder.SelectedPath;
                WriteConf("LatexFolder", folder.SelectedPath);
            }
        }

        private void TabFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.RootFolder = Environment.SpecialFolder.Desktop;
            folder.Description = "Selectionner le dossier qui contiendra les tablatures";
            folder.ShowNewFolderButton = true;
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TabFolder.Text = folder.SelectedPath;
                WriteConf("TabFolder", folder.SelectedPath);
            }
        }

        private void PartiFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.RootFolder = Environment.SpecialFolder.Desktop;
            folder.Description = "Selectionner le dossier qui contiendra les partitions";
            folder.ShowNewFolderButton = true;
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PartiFolder.Text = folder.SelectedPath;
                WriteConf("PartiFolder", folder.SelectedPath);
            }
        }

        private void Connection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Requete.Request.GetToken(pseudo.Text, password.Password);
                WriteConf("pseudo", pseudo.Text);
                WriteConf("password", password.Password);
                System.Windows.MessageBox.Show("Identification Correct, identifiant enregistré", "Validation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Requete.IdentificationException)
            {
                System.Windows.MessageBox.Show("Erreur lors du test d'authentification avec le server. Verifier Votre mot de passe et votre connection internet.\n", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\log.txt", true))
                {
                    file.WriteLine(ex.Message);
                }
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
