using Microsoft.Win32;
using python;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace tfe
{
    /// <summary>
    /// Logique d'interaction pour lilypond.xaml
    /// </summary>
    public partial class lilypond : Page
    {
        public List<Note> _notes = new List<Note>();
        private Uri _LyliPath;

        private Frame _frame;
        public lilypond(Frame nav, List<Note> ParamNote = null)
        {
            _frame = nav;
            InitializeComponent();
            if (ParamNote == null)
            {  // if no notes are imported no posibility to generate a lilypond file
                ToPdf.IsEnabled = false;
                ToLaTex.IsEnabled = false;
            }
            else
            {
                _notes = ParamNote;
                ToPdf.IsEnabled = true;
                ToLaTex.IsEnabled = true;
            }
        }

        /// <summary>
        /// select a lilypond file and save note into it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToLilypond_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";
            saveFileDialog.InitialDirectory = ReadConf("LilyFolder");
            if (saveFileDialog.ShowDialog() != true) return;

            Lily Lilypond = new Lily(saveFileDialog.FileName);
            Lilypond.Customise(titre.Text, sTitre.Text, piece.Text, pdPage.Text);
            lilyFile.Text = Lilypond.SetNotes(_notes, midi.IsChecked.Value);
            MessageBox.Show("Fichier lilypond généré avec succes", "Génération Lylipond", MessageBoxButton.OK, MessageBoxImage.Information);
            ToLaTex.IsEnabled = true;
            ToPdf.IsEnabled = true;
            _LyliPath = new Uri(saveFileDialog.FileName);
        }

        /// <summary>
        /// generate a pdf with lilypond on base of ly file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToPdf_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            string Lilypond = @"D:\Programme file(x86)\LilyPond\usr\bin\lilypond.exe";
#else
            string Lilypond = @"LilyPond\usr\bin\lilypond.exe";
#endif
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";  // only ly files
            openFileDialog.InitialDirectory = ReadConf("LilyFolder");
            if (openFileDialog.ShowDialog() != true) return;  // if no file is selected
            string script = @" --output="+ReadConf("PartiFolder")+" " + openFileDialog.FileName;
            if (File.Exists(ReadConf("PartiFolder") + @"\" + openFileDialog.SafeFileName.Split('.').First() + ".pdf"))
            {
                File.Delete(ReadConf("PartiFolder") + @"\" + openFileDialog.SafeFileName.Split('.').First() + ".pdf");
            }
            var process = Process.Start(Lilypond, script);
            process.WaitForExit();
            if (File.Exists(ReadConf("PartiFolder") + @"\" + openFileDialog.SafeFileName.Split('.').First())) File.Delete(ReadConf("PartiFolder") + @"\" + openFileDialog.SafeFileName.Split('.').First());
            File.Copy(ReadConf("PartiFolder") +@"\"+ openFileDialog.SafeFileName.Split('.').First() + ".pdf", ReadConf("PartiFolder") + @"\" + openFileDialog.SafeFileName.Split('.').First());
            _frame.Navigate(new PdfViewer(_frame, ReadConf("PartiFolder") + @"\" + openFileDialog.SafeFileName.Split('.').First()));
        }

        private void ToLaTex_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "LaTex Files (*.tex)|*.tex";
            saveFileDialog.InitialDirectory = ReadConf("LatexFolder");
            if (saveFileDialog.ShowDialog() != true) return;

            try
            {
                Latex latex = new Latex(saveFileDialog.FileName, _LyliPath);
                latex.BuildRow();
                latex.BuildLaTex();
            }
            catch (Exception exeption)
            {
                MessageBox.Show(exeption.Message.ToString(), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// save changes effected in the textbox of lilypond
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SvgLily(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";
            saveFileDialog.InitialDirectory = ReadConf("LilyFolder");
            if (saveFileDialog.ShowDialog() != true) return;

            Lily Lilypond = new Lily(saveFileDialog.FileName);
            List<string> Data = lilyFile.Text.Split('\n').ToList();
            lilyFile.Text = Lilypond.Save(Data);
            MessageBox.Show("Fichier lilypond sauvegardé avec succes", "Génération Lylipond", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// load a lilypond file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenLily(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";
            openFileDialog.InitialDirectory = ReadConf("LilyFolder");
            if (openFileDialog.ShowDialog() != true) return;

            Lily Lilypond = new Lily(openFileDialog.FileName);
            lilyFile.Text = string.Join("\n", Lilypond.ReadFile());
            MessageBox.Show("Fichier lilypond sauvegardé avec succes", "Génération Lylipond", MessageBoxButton.OK, MessageBoxImage.Information);
            ToLaTex.IsEnabled = true;
            ToPdf.IsEnabled = true;
            midi.IsChecked = Lilypond.MidiGeneration;
            _LyliPath = new Uri(openFileDialog.FileName);
        }

        private void Midichanged(object sender, RoutedEventArgs e)
        {
            if (!midi.IsChecked.Value)
            {
                string temp = lilyFile.Text;
                temp = temp.Replace("\\midi{}", "");
                temp = temp.Replace("\\layout{}", "");
                lilyFile.Text = temp;
            }
            else
            {
                if (!lilyFile.Text.Contains(@"\layout{}") && !lilyFile.Text.Contains(@"\midi{}"))
                {
                    string temp = lilyFile.Text;
                    temp = temp.TrimEnd(Environment.NewLine.ToCharArray());
                    temp += @"
\midi{}
\layout{}";
                    lilyFile.Text = temp;
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
