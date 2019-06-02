using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace tfe
{
    /// <summary>
    /// Logique d'interaction pour Config.xaml
    /// </summary>
    public partial class Config : Window
    {
        public Config()
        {
            List<string> param = new List<string>() {"WavFolder", "LilyFolder", "LatexFolder", "TabFolder", "PartiFolder", "pseudo", "password"};
            InitializeComponent();
            param.ForEach(delegate(string x) {
                ((System.Windows.Controls.TextBox)this.FindName(x)).Text = ReadConf(x);
            });
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
            if (Requete.Request.GetToken(pseudo.Text, password.Text) == "invalid_grant") {
                System.Windows.MessageBox.Show("Erreur lors du test d'authentification à l'api. Verifier Votre mot de passe et votre connection internet.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            WriteConf("pseudo", pseudo.Text);
            WriteConf("password", password.Text);
            System.Windows.MessageBox.Show("Identification Correct, identifiant enregistré", "Validation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Pour vous créer un compte, il faut vous rendre à cette addresse https://localhost:44371", "connection", MessageBoxButton.OK, MessageBoxImage.Hand);
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
