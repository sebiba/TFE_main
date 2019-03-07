using Microsoft.Win32;
using NAudio.Wave;
using python;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public List<string> _notes = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
        }
        #region record
        private void StartBtn_Click(object sender, EventArgs e)
        {
            StartBtn.IsEnabled = false;
            StopBtn.IsEnabled = true;

            waveSource = new WaveIn();
            waveSource.WaveFormat = new WaveFormat(44100, 1);

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            waveFile = new WaveFileWriter(@"D:\programmation\python\TFE\Test0001.wav", waveSource.WaveFormat);

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

        private void ImporteBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Wav Files Only (*.wav)|*.wav";
            openFileDialog.InitialDirectory = @"D:\programmation\python\TFE\";
            if (openFileDialog.ShowDialog() != true) return;
            Impfile.Text = openFileDialog.FileName;
            List<string> data = PyUtils.getfreq(openFileDialog.FileName).Replace("||", "\n").Split('\n').ToList();
            data.RemoveAt(data.Count - 1);
            string freq = "";
            foreach (string input in data) {
                freq += input.Split(':').Last() + "\n";
                _notes.Add(PyUtils.freqToNote(Math.Abs(float.Parse(input.Split(':').Last(), CultureInfo.InvariantCulture))));
            }

            for(int i= 0;i < _notes.Count;i++)
            {
                try
                {
                    if (_notes[i] != _notes[i + 1])
                    {
                        _notes.RemoveAt(i + 1);
                    }
                }
                catch
                {

                }
            }
            frequence.Text = freq;
            notes.Text = string.Join("\n", _notes);
        }
        
        private void ToLilypond_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";
            saveFileDialog.InitialDirectory = @"D:\programmation\c#\TFE\python\Lily\";
            if (saveFileDialog.ShowDialog() != true) return;
            Lily Lilypond = new Lily(saveFileDialog.FileName);
            Lilypond.Customise(titre.Text, sTitre.Text, piece.Text, pdPage.Text);
            Lilypond.SetNotes(_notes);
            MessageBox.Show("Fichier lilypond généré avec succes","Génération Lylipond",MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
