using Microsoft.Win32;
using NAudio.Wave;
using python;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private Frame _frame;

        public input(Frame nav)
        {
            _frame = nav;
            InitializeComponent();
        }

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

            Impfile.Text = openFileDialog.FileName;
            try { 
            _notes = PyUtils.Getfreq(openFileDialog.FileName);  // get frequency
            }
            catch
            {
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

            _frame.Navigate(new lilypond( _frame, _notes));
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
