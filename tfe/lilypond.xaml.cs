﻿using Microsoft.Win32;
using python;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
        private readonly log4net.ILog _log;
        public lilypond(Frame nav, log4net.ILog logParam, List<Note> ParamNote = null)
        {
            _frame = nav;
            _log = logParam;
            _log.Info("Show Lilypond page");
            InitializeComponent();
            if (ParamNote == null)
            {  // if no notes are imported no posibility to generate a lilypond file
                _log.Info("ParamNote is null");
                ToPdf.IsEnabled = false;
                ToLaTex.IsEnabled = false;
            }
            else
            {
                _log.Info("ParamNote has " + ParamNote.Count + " elements");
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
            lilyFile.Text = Lilypond.SetNotes(_notes);
            _log.Debug("Create lilypond file: "+saveFileDialog.FileName);
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";  // only ly files
            openFileDialog.InitialDirectory = ReadConf("LilyFolder");
            if (openFileDialog.ShowDialog() != true) return;  // if no file is selected
            string script = @" --output=" + ReadConf("PartiFolder") + " " + openFileDialog.FileName;
            Cursor = Cursors.Arrow;
            _log.Debug("Lilypond arguments: "+script);
            TraitePdf(openFileDialog.SafeFileName, Lilypond, script);
        }


        private void TraitePdf(string name, string Lilypond, string script) { 
            if(ReadConf("PartiFolder")+ @"\" == @"\"){  // if param for folder of partitions is empty
                if (File.Exists(name.Split('.').First() + ".pdf"))
                {
                    File.Delete(name.Split('.').First() + ".pdf");
                }
                var process = Process.Start(Lilypond, script);
                process.WaitForExit();
                _log.Info("Lilypond compilation success: "+ System.IO.Path.GetFullPath(name.Split('.').First()));
                if (File.Exists(name.Split('.').First())) File.Delete(name.Split('.').First());
                File.Copy(name.Split('.').First() + ".pdf", name.Split('.').First());
                _frame.Navigate(new PdfViewer(_frame, _log, System.IO.Path.GetFullPath(name.Split('.').First())));
            }
            else
            {
                if (File.Exists(ReadConf("PartiFolder") + @"\" + name.Split('.').First() + ".pdf"))
                {
                    File.Delete(ReadConf("PartiFolder") + @"\" + name.Split('.').First() + ".pdf");
                }
                var process = Process.Start(Lilypond, script);
                process.WaitForExit();
                _log.Info("Lilypond compilation success: "+ System.IO.Path.GetFullPath(name.Split('.').First()));
                if (File.Exists(ReadConf("PartiFolder") + @"\" + name.Split('.').First())) {
                    try
                    {
                        File.Delete(ReadConf("PartiFolder") + @"\" + name.Split('.').First());
                    }
                    catch {
                        _log.Error("TraitePdf() -> Error delete file:"+ ReadConf("PartiFolder") + @"\" + name.Split('.').First());
                    }
                }
                File.Copy(ReadConf("PartiFolder") + @"\" + name.Split('.').First() + ".pdf", ReadConf("PartiFolder") + @"\" + name.Split('.').First());
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
        private void SvgLily(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";
            saveFileDialog.InitialDirectory = ReadConf("LilyFolder");
            if (saveFileDialog.ShowDialog() != true) return;

            Lily Lilypond = new Lily(saveFileDialog.FileName);
            List<string> Data = lilyFile.Text.Split('\n').ToList();
            lilyFile.Text = Lilypond.Save(Data);
            _log.Info("Save Lilypond file: "+ saveFileDialog.FileName);
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
            _log.Info("Load lilypond file: "+ openFileDialog.FileName);
            ToLaTex.IsEnabled = true;
            ToPdf.IsEnabled = true;
            midi.IsChecked = Lilypond.MidiGeneration;
            _LyliPath = new Uri(openFileDialog.FileName);
        }

        private void MidiChanged(object sender, RoutedEventArgs e)
        {
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
