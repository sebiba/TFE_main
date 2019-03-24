﻿using Microsoft.Win32;
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
        public List<string> _notes = new List<string>();
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
            openFileDialog.InitialDirectory = @"D:\programmation\python\TFE\";
            if (openFileDialog.ShowDialog() != true) return;  // if no file is selected

            Impfile.Text = openFileDialog.FileName;
            List<string> data = PyUtils.Getfreq(openFileDialog.FileName).Replace("||", "\n").Split('\n').ToList();  // get frequency
            data.RemoveAt(data.Count - 1);
            string freq = "";
            foreach (string input in data) {  // loop on all notes
                freq += input.Split(':').Last() + "\n";
                _notes.Add(PyUtils.FreqToNote(Math.Abs(float.Parse(input.Split(':').Last(), CultureInfo.InvariantCulture))));  // get note
            }
            for(int i= 0;i < _notes.Count;i++)  //remove alone note
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
        
        /// <summary>
        /// select a lilypond file and save note into it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToLilypond_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";
            saveFileDialog.InitialDirectory = @"D:\programmation\c#\TFE\python\Lily\";
            if (saveFileDialog.ShowDialog() != true) return;

            Lily Lilypond = new Lily(saveFileDialog.FileName);
            Lilypond.Customise(titre.Text, sTitre.Text, piece.Text, pdPage.Text);
            lilypond.Text = Lilypond.SetNotes(_notes);
            MessageBox.Show("Fichier lilypond généré avec succes","Génération Lylipond",MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// generate a pdf with lilypond on base of ly file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToPdf_Click(object sender, RoutedEventArgs e) {
            string Lilypond = @"D:\Programme file(x86)\LilyPond\usr\bin\lilypond.exe";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Lilypond Files (*.ly)|*.ly";  // only ly files
            openFileDialog.InitialDirectory = @"D:\programmation\c#\TFE\python\Lily\";
            if (openFileDialog.ShowDialog() != true) return;  // if no file is selected
            string script = openFileDialog.FileName;
            if (File.Exists(@"D:\programmation\c#\TFE\tfe\bin\Debug\"+ openFileDialog.SafeFileName.Split('.').First()+".pdf"))
            {
                File.Delete(@"D:\programmation\c#\TFE\tfe\bin\Debug\" + openFileDialog.SafeFileName.Split('.').First() + ".pdf");
            }
            var process = Process.Start(Lilypond, script);
            process.WaitForExit();
            pdfWebViewer.Navigate(@"D:\programmation\c#\TFE\tfe\bin\Debug\"+openFileDialog.SafeFileName.Split('.').First()+".pdf");  // display the new pdf on the screen
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
            saveFileDialog.InitialDirectory = @"D:\programmation\c#\TFE\python\Lily\";
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
            openFileDialog.InitialDirectory = @"D:\programmation\c#\TFE\python\Lily\";
            if (openFileDialog.ShowDialog() != true) return;

            Lily Lilypond = new Lily(openFileDialog.FileName);
            lilypond.Text = string.Join("\n",Lilypond.ReadFile());
            MessageBox.Show("Fichier lilypond sauvegardé avec succes", "Génération Lylipond", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}