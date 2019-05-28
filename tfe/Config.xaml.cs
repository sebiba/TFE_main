using System;
using System.Collections.Generic;
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
            InitializeComponent();
        }

        private void WavFolder_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.RootFolder = Environment.SpecialFolder.Desktop;
            folder.Description = "Selectionner un Dossier";
            folder.ShowNewFolderButton = true;
            if(folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WavFolder.Text = folder.SelectedPath;
            }
        }

        private void LilyFolder_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void LatexFolder_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TabFolder_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void PartiFolder_GotFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
