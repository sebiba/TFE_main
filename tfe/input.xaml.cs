using Microsoft.Win32;
using NAudio.Wave;
using python;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace tfe
{
    /// <summary>
    /// Logique d'interaction pour input.xaml
    /// </summary>
    public partial class input : Page
    {
        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        public List<Note> _notes = new List<Note>();
        public Label TitleMenu;

        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, Object value);

        private Frame _frame;
        private readonly log4net.ILog _log;

        public input(Frame nav, Label TitleMenuParam ,log4net.ILog logParam)
        {
            TitleMenu = TitleMenuParam;
            _frame = nav;
            _log = logParam;
            _log.Info("Show Audio page");
            InitializeComponent();
            Progression.Value = 0;
        }
#region record
        private void StartBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Wav Files Only (*.wav)|*.wav";
            saveFileDialog.InitialDirectory = ReadConf("WavFolder");
            if (saveFileDialog.ShowDialog() != true) return;

            StartBtn.IsEnabled = false;
            StopBtn.IsEnabled = true;

            waveSource = new WaveIn();
            waveSource.WaveFormat = new WaveFormat(44100, 1);

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            waveFile = new WaveFileWriter(saveFileDialog.FileName, waveSource.WaveFormat);

            waveSource.StartRecording();
            _log.Debug("Start recording in file: " + saveFileDialog.FileName);
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            waveSource.StopRecording();
            _log.Debug("End of recording in file");
            Cursor = Cursors.Wait;
            StopBtn.IsEnabled = false;
            try
            {
                _notes = PyUtils.Getfreq(waveFile.Filename);  // get frequency
                _log.Debug(_notes.Count + " Notes get from the file: '" + waveFile.Filename);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors du traitement de votre fichier audio.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                _log.Error("Error while getting frequences of the audio file: "+ex.Message);
            }
            for (int i = 0; i < _notes.Count; i++)  //remove alone note
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
            Cursor = Cursors.Arrow;
            _frame.Navigate(new lilypond(_frame, _log, _notes));
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
            openFileDialog.InitialDirectory = ReadConf("WavFolder");
            if (openFileDialog.ShowDialog() != true) return;  // if no file is selected
            Cursor = Cursors.Wait;
            Impfile.Text = openFileDialog.FileName;
            UpdateProgress(10);
            try { 
                _notes = PyUtils.Getfreq(openFileDialog.FileName);  // get frequency
                UpdateProgress(70);
                _log.Info("Get frequencies of the audio file: " + openFileDialog.FileName);
            }
            catch(Exception ex)
            {
                _log.Error("Error while trying to get frequencies of the audio file: " + openFileDialog.FileName + "\tMessage: " + ex.Message);
                MessageBox.Show("Une erreur est survenue lors du traitement de votre fichier audio.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            for (int i = 0; i < _notes.Count; i++)  //remove alone note
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
            UpdateProgress(80);
            Cursor = Cursors.Arrow;
            UpdateProgress(100);
            TitleMenu.Content = "Lilypond";
            _frame.Navigate(new lilypond( _frame, _log, _notes, true));
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

        private void UpdateProgress(double status)
        {
            Progression.Dispatcher.Invoke(new UpdateProgressBarDelegate(Progression.SetValue),
                DispatcherPriority.Background,
                new object[] { ProgressBar.ValueProperty, status });
            ProgressionNumber.Content = status + " %";
        }
    }
}
