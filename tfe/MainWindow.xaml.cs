using NAudio.Wave;
using python;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        public MainWindow()
        {
            InitializeComponent();
        }

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
        
        private void TestBtn_Click(object sender, EventArgs e)
        {
            List<string> data = PyUtils.getfreq(Impfile.Text).Replace("||", "\n").Split('\n').ToList();
            data.RemoveAt(data.Count - 1);
            string freq = "";
            string not = "";
            foreach (string input in data) {
                freq += input.Split(':').Last() + "\n";
                string test = input.Split(':').Last();
                float jsp = float.Parse(test, CultureInfo.InvariantCulture);
                not += PyUtils.freqToNote(Math.Abs(jsp));
            }
            frequence.Text = freq;
            notes.Text = not;
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
    }
}
