using Microsoft.Win32;
using python;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace tfe
{
    /// <summary>
    /// Logique d'interaction pour lilypond.xaml
    /// </summary>
    public partial class lilypond : Page
    {
        public List<Note> _notes = new List<Note>();
        public Label TitleMenu;

        private Uri _LyliPath;
        private Frame _frame;
        private readonly log4net.ILog _log;
        public lilypond(Frame nav, Label TitleMenuParam, log4net.ILog logParam, List<Note> ParamNote = null, bool NeedHelp = false)
        {
            TitleMenu = TitleMenuParam;
            _frame = nav;
            _log = logParam;
            _log.Info("Show Lilypond page");
            InitializeComponent();
            if (ParamNote == null)
            {  // if no notes are imported no posibility to generate a lilypond file
                _log.Info("ParamNote is null");
                GenLily.IsEnabled = false;
            }
            else
            {
                _log.Info("ParamNote has " + ParamNote.Count + " elements");
                _notes = ParamNote;
            }
            ToPdf.IsEnabled = false;
            ToLaTex.IsEnabled = false;
            if (NeedHelp)
            {
                help.Visibility = Visibility.Visible;
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
            lilyFile.Text = Lilypond.SetNotes(_notes);
            _log.Info("Ask for midi génération");
            if (midi.IsChecked.Value) { 
                if (!lilyFile.Text.Contains(@"\layout{}") && !lilyFile.Text.Contains(@"\midi{}"))
                {
                    List<string> temp = lilyFile.Text.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
                    if (!lilyFile.Text.Contains(@"\score{"))
                    {
                        temp.Insert(temp.FindIndex(x => x.Contains(@"\relative")) - 1, @"\score{");
                        temp.AddRange(new List<string> { @"\layout{}", @"\midi{}", "}" });
                    }
                    else
                    {
                        temp.InsertRange(temp.Count - 1, new List<string> { @"\layout{}", @"\midi{}" });
                    }
                    lilyFile.Text = string.Join("\n", temp);
                }
            }  // if midi is checked
            _log.Debug("Create lilypond file: "+saveFileDialog.FileName);
            MessageBox.Show("Fichier lilypond généré avec succès", "Génération Lylipond", MessageBoxButton.OK, MessageBoxImage.Information);
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
            Cursor = Cursors.Wait;
#if DEBUG
            string Lilypond = @"D:\Programme file(x86)\LilyPond\usr\bin\lilypond.exe";
#else
            if (!Directory.Exists("Lilypond")) {
                try { 
                    ZipFile.ExtractToDirectory("./data.zip", "./");
                    _log.Info("Data.zip has been unzipped");
                }
                catch(Exception ex)
                {
                    _log.Error("ToPdf_Click() -> Erreur while unzip data.zip: "+ex.Message);
                    MessageBox.Show("Un élément inconnue à empèché le traitement de donner dans le dossier de l'application. Certaine fonctionnalités risque de ne pas fonctionner correctement.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            string Lilypond = @"LilyPond\usr\bin\lilypond.exe";
#endif
            string script = @" --output=" + ReadConf("PartiFolder") + " " + _LyliPath.LocalPath;

            if (lilyFile.Text.Replace("\r", "") != File.ReadAllText(_LyliPath.LocalPath))
            {
                switch (MessageBox.Show("Voulez-vous enregistrer les modifications effectuées sur le fichier lilypond avant d'en faire un pdf?", "Modification", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes: SvgLily();
                        break;
                    case MessageBoxResult.No: _log.Info("don't save change before making the pdf");
                        break;
                    default: Cursor = Cursors.Arrow;
                        return;
                };
            }
            Cursor = Cursors.Arrow;
            _log.Debug("Lilypond arguments: "+script);
            TraitePdf(Path.GetFileName(_LyliPath.LocalPath), Lilypond, script);
        }

        private void TraitePdf(string name, string Lilypond, string script) { 
            if(ReadConf("PartiFolder")+ @"\" == @"\"){  // if param for folder of partitions is empty
                if (File.Exists(name.Split('.').First() + ".pdf"))
                {
                    File.Delete(name.Split('.').First() + ".pdf");
                }
                var process = Process.Start(Lilypond, script);
                process.WaitForExit();
                _log.Info("Lilypond compilation success: "+ Path.GetFullPath(name.Split('.').First()));
                try { 
                if (File.Exists(name.Split('.').First())) File.Delete(name.Split('.').First());
                }
                catch
                {
                    _log.Error("TraitePdf() -> Error delete file:" + Path.GetFullPath(name.Split('.').First()));
                    MessageBox.Show("Le fichier \"" + Path.GetFullPath(name.Split('.').First()) + "\" n'a pas pu être supprimé. vérifier qu'il n'est pas utilisé par un autre programme au redémarré Diatab.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                File.Copy(name.Split('.').First() + ".pdf", name.Split('.').First());
                TitleMenu.Content = "Gestion des PDF";
                _frame.Navigate(new PdfViewer(_frame, _log, Path.GetFullPath(name.Split('.').First())));
            }
            else
            {
                if (File.Exists(ReadConf("PartiFolder") + @"\" + name.Split('.').First() + ".pdf"))
                {
                    File.Delete(ReadConf("PartiFolder") + @"\" + name.Split('.').First() + ".pdf");
                }
                var process = Process.Start(Lilypond, script);
                process.WaitForExit();
                _log.Info("Lilypond compilation success: "+ Path.GetFullPath(name.Split('.').First()));
                if (File.Exists(ReadConf("PartiFolder") + @"\" + name.Split('.').First())) {
                    try
                    {
                        File.Delete(ReadConf("PartiFolder") + @"\" + name.Split('.').First());
                    }
                    catch {
                        _log.Error("TraitePdf() -> Error delete file:"+ ReadConf("PartiFolder") + @"\" + name.Split('.').First());
                        MessageBox.Show("Le fichier \"" + ReadConf("PartiFolder") + @"\" + name.Split('.').First() + "\" n'a pas pu être supprimé. vérifier qu'il n'est pas utilisé par un autre programme au redémarré Diatab.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                File.Copy(ReadConf("PartiFolder") + @"\" + name.Split('.').First() + ".pdf", ReadConf("PartiFolder") + @"\" + name.Split('.').First());
                TitleMenu.Content = "Gestion des PDF";
                _frame.Navigate(new PdfViewer(_frame, _log, ReadConf("PartiFolder") + @"\" + name.Split('.').First()));
            }            
        }

        private void ToLaTex_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "LaTex Files (*.tex)|*.tex";
            saveFileDialog.InitialDirectory = ReadConf("LatexFolder");
            if (saveFileDialog.ShowDialog() != true) return;

            try
            {
                Latex latex = new Latex(saveFileDialog.FileName, _LyliPath);
                latex.BuildRow();
                latex.BuildLaTex();
                _log.Debug("Create and Compile latex file: " + saveFileDialog.FileName);
                MessageBox.Show("Votre tablature à bien été créée dans le dossier: "+ReadConf("TabFolder")+@"\","Tablature", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exeption)
            {
                MessageBox.Show(exeption.Message.ToString(), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                _log.Error("Error Latex: "+exeption.Message);
            }
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// save changes effected in the textbox of lilypond
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SvgLily(object sender=null, RoutedEventArgs e=null)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";
            saveFileDialog.InitialDirectory = ReadConf("LilyFolder");
            if (saveFileDialog.ShowDialog() != true) return;

            Lily Lilypond = new Lily(saveFileDialog.FileName);
            List<string> Data = lilyFile.Text.Split('\n').ToList();
            lilyFile.Text = Lilypond.Save(Data);
            _log.Info("Save Lilypond file: "+ saveFileDialog.FileName);
            _LyliPath = new Uri(saveFileDialog.FileName);
            MessageBox.Show("Fichier lilypond sauvegardé avec succès", "Génération Lylipond", MessageBoxButton.OK, MessageBoxImage.Information);
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
            _log.Info("Load lilypond file: "+ openFileDialog.FileName);
            ToLaTex.IsEnabled = true;
            ToPdf.IsEnabled = true;
            titre.Text = Lilypond.getTitre();
            sTitre.Text = Lilypond.getSousTitre();
            piece.Text = Lilypond.getPiece();
            pdPage.Text = Lilypond.getTagLine();
            midi.IsChecked = Lilypond.MidiGeneration;
            _LyliPath = new Uri(openFileDialog.FileName);
        }

        #region update lilypond
        private void MidiChanged(object sender, RoutedEventArgs e)
        {
            if (lilyFile.Text == "") return;
            if (!midi.IsChecked.Value)
            {
                List<string> temp = lilyFile.Text.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
                lilyFile.Text = string.Join("\n", temp.Where(x => !x.Contains("\\midi{}") && !x.Contains("\\layout{}")).ToList());
                _log.Info("Don't ask for midi génération");
            }
            else
            {
                _log.Info("Ask for midi génération");
                if (!lilyFile.Text.Contains(@"\layout{}") && !lilyFile.Text.Contains(@"\midi{}"))
                {
                    List<string> temp = lilyFile.Text.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
                    if (!lilyFile.Text.Contains(@"\score{"))
                    {
                        temp.Insert(temp.FindIndex(x => x.Contains(@"\relative"))-1, @"\score{");
                        temp.AddRange(new List<string> { @"\layout{}", @"\midi{}", "}"});
                    }
                    else
                    {
                        temp.InsertRange(temp.Count-1, new List<string> { @"\layout{}", @"\midi{}"});
                    }
                    lilyFile.Text = string.Join("\n", temp);
                }
            }
        }

        private void TitleUpdate(object sender, RoutedEventArgs e)
        {
            List<string> temp = lilyFile.Text.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
            for(int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Contains("title")) {
                    temp[i] = "title = \""+titre.Text+"\"";
                    lilyFile.Text = string.Join("\n", temp);
                    return;
                }

            }
        }

        private void STitleUpdate(object sender, RoutedEventArgs e)
        {
            List<string> temp = lilyFile.Text.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Contains("subtitle"))
                {
                    temp[i] = "subtitle = \"" + sTitre.Text + "\"";
                    lilyFile.Text = string.Join("\n", temp);
                    return;
                }

            }
        }

        private void PieceUpdate(object sender, RoutedEventArgs e)
        {
            List<string> temp = lilyFile.Text.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Contains("piece"))
                {
                    temp[i] = "piece = \"" + piece.Text + "\"";
                    lilyFile.Text = string.Join("\n", temp);
                    return;
                }

            }
        }

        private void PdPageUpdate(object sender, RoutedEventArgs e)
        {
            List<string> temp = lilyFile.Text.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Contains("tagline"))
                {
                    temp[i] = "tagline = \"" + pdPage.Text + "\"";
                    lilyFile.Text = string.Join("\n", temp);
                    return;
                }

            }
        }

        #endregion update lilypond
        private string ReadConf(string key)
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;

                string[] arr = appSettings.GetValues(key);
                return arr[0];
            }
            catch(Exception ex)
            {
                _log.Error("Error key not found:" + key+"\tMessage:"+ ex.Message);
                throw;
            }
        }

    }
}
