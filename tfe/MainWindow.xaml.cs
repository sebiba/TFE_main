using Microsoft.Win32;
using NAudio.Wave;
using python;
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
        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        public List<Note> _notes = new List<Note>();
        private Uri _LyliPath;
        public MainWindow()
        {
            InitializeComponent();
            pdfWebViewer.Navigate(new Uri("about:blank"));
        }
        #region record
        private void StartBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Wav Files Only (*.wav)|*.wav";
            saveFileDialog.InitialDirectory = @"D:\programmation\python\TFE\";
            if (saveFileDialog.ShowDialog() != true) return;

            StartBtn.IsEnabled = false;
            StopBtn.IsEnabled = true;

            waveSource = new WaveIn();
            waveSource.WaveFormat = new WaveFormat(44100, 1);

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            waveFile = new WaveFileWriter(saveFileDialog.FileName, waveSource.WaveFormat);

            waveSource.StartRecording();
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            StopBtn.IsEnabled = false;

            waveSource.StopRecording();
        }

        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();
            }
        }

        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }

            StartBtn.IsEnabled = true;
        }
        #endregion record

        /// <summary>
        /// importe a wav file an get all frequency, notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImporteBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Wav Files Only (*.wav)|*.wav";  // only wav files
            //openFileDialog.InitialDirectory = @"D:\programmation\python\TFE\";
            if (openFileDialog.ShowDialog() != true) return;  // if no file is selected

            Impfile.Text = openFileDialog.FileName;
            _notes = PyUtils.Getfreq(openFileDialog.FileName);  // get frequency
            for(int i= 0;i < _notes.Count;i++)  //remove alone note
            {
                try
                {
                    if (_notes[i].value != _notes[i + 1].value)
                    {
                        _notes.RemoveAt(i + 1);
                    }
                }
                catch
                {

                }
            }

            //frequence.Text = freq;
            notes.Text = string.Join("\n", _notes.Select(note => note.value).ToList());
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
            //saveFileDialog.InitialDirectory = @"D:\programmation\c#\TFE\python\Lily\";
            if (saveFileDialog.ShowDialog() != true) return;

            Lily Lilypond = new Lily(saveFileDialog.FileName);
            Lilypond.Customise(titre.Text, sTitre.Text, piece.Text, pdPage.Text);
            lilypond.Text = Lilypond.SetNotes(_notes, midi.IsChecked.Value);
            MessageBox.Show("Fichier lilypond généré avec succes","Génération Lylipond",MessageBoxButton.OK, MessageBoxImage.Information);
            ToLaTex.IsEnabled = true;
            ToPdf.IsEnabled = true;
            _LyliPath = new Uri(saveFileDialog.FileName);
        }

        /// <summary>
        /// generate a pdf with lilypond on base of ly file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToPdf_Click(object sender, RoutedEventArgs e) {
#if DEBUG
            string Lilypond = @"D:\Programme file(x86)\LilyPond\usr\bin\lilypond.exe";
#else
            string Lilypond = @"LilyPond\usr\bin\lilypond.exe";
#endif
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";  // only ly files
            //openFileDialog.InitialDirectory = @"D:\programmation\c#\TFE\python\Lily\";
            if (openFileDialog.ShowDialog() != true) return;  // if no file is selected
            string script = @" --output=D:\jsp\partition "+openFileDialog.FileName;
            if (File.Exists(openFileDialog.SafeFileName.Split('.').First()+".pdf"))
            {
                File.Delete(openFileDialog.SafeFileName.Split('.').First() + ".pdf");
            }
            var process = Process.Start(Lilypond, script);
            process.WaitForExit();
#if DEBUG
            if(File.Exists(@"D:\jsp\partition\" + openFileDialog.SafeFileName.Split('.').First() + ".pdf")) File.Delete(@"D:\jsp\partition\" + openFileDialog.SafeFileName.Split('.').First() + ".pdf");
            File.Move(Path.GetFullPath(openFileDialog.SafeFileName.Split('.').First() + ".pdf"), @"D:\jsp\partition\"+ openFileDialog.SafeFileName.Split('.').First() + ".pdf");
            if(File.Exists(@"D:\jsp\" + openFileDialog.SafeFileName.Split('.').First())) File.Delete(@"D:\jsp\" + openFileDialog.SafeFileName.Split('.').First());
            File.Copy(@"D:\jsp\partition\" + openFileDialog.SafeFileName.Split('.').First() + ".pdf", @"D:\jsp\"+ openFileDialog.SafeFileName.Split('.').First());
            pdfWebViewer.Navigate(@"D:\jsp\" + openFileDialog.SafeFileName.Split('.').First());  // display the new pdf on the screen
#else
            if(File.Exists(@"partition\"+ openFileDialog.SafeFileName.Split('.').First() + ".pdf")) File.Delete(@"partition\"+ openFileDialog.SafeFileName.Split('.').First() + ".pdf");
            File.Move(Path.GetFullPath(openFileDialog.SafeFileName.Split('.').First() + ".pdf"), @"partition\"+ openFileDialog.SafeFileName.Split('.').First() + ".pdf");
            if(File.Exists(@"../temp/temp."+ openFileDialog.SafeFileName.Split('.').First()) File.Delete(@"../temp/temp."+ openFileDialog.SafeFileName.Split('.').First());
            File.Copy(@"D:\jsp\partition\" + openFileDialog.SafeFileName.Split('.').First() + ".pdf", @"../temp/temp."+ openFileDialog.SafeFileName.Split('.').First());
            pdfWebViewer.Navigate(@"../temp/temp."+ openFileDialog.SafeFileName.Split('.').First());  // display the new pdf on the screen
#endif
        }

        private void ToLaTex_Click (object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "LaTex Files (*.tex)|*.tex";
            //saveFileDialog.InitialDirectory = @"D:\programmation\c#\TFE\python\Lily\";
            if (saveFileDialog.ShowDialog() != true) return;

            //try { 
            Latex latex = new Latex(saveFileDialog.FileName, _LyliPath);
            latex.BuildRow();
            latex.BuildLaTex();
            /*}
            catch(Exception exeption)
            {
                MessageBox.Show(exeption.Message.ToString(), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
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
            //saveFileDialog.InitialDirectory = @"D:\programmation\c#\TFE\python\Lily\";
            if (saveFileDialog.ShowDialog() != true) return;

            Lily Lilypond = new Lily(saveFileDialog.FileName);
            List<string> Data = lilypond.Text.Split('\n').ToList();
            lilypond.Text = Lilypond.Save(Data);
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
            //openFileDialog.InitialDirectory = @"D:\programmation\c#\TFE\python\Lily\";
            if (openFileDialog.ShowDialog() != true) return;

            Lily Lilypond = new Lily(openFileDialog.FileName);
            lilypond.Text = string.Join("\n",Lilypond.ReadFile());
            MessageBox.Show("Fichier lilypond sauvegardé avec succes", "Génération Lylipond", MessageBoxButton.OK, MessageBoxImage.Information);
            ToLaTex.IsEnabled = true;
            ToPdf.IsEnabled = true;
            midi.IsChecked = Lilypond.MidiGeneration;
            _LyliPath = new Uri(openFileDialog.FileName);
        }

        private void Midichanged(object sender, RoutedEventArgs e)
        {
            if (!midi.IsChecked.Value) {
                string temp = lilypond.Text;
                temp = temp.Replace("\\midi{}", "");
                temp = temp.Replace("\\layout{}", "");
                lilypond.Text = temp;
            }
            else
            {
                if (!lilypond.Text.Contains(@"\layout{}") && !lilypond.Text.Contains(@"\midi{}"))
                {
                    string temp = lilypond.Text;
                    temp = temp.TrimEnd(Environment.NewLine.ToCharArray());
                    temp += @"
\midi{}
\layout{}";
                    lilypond.Text = temp;
                }
            }
        }

    }
}
