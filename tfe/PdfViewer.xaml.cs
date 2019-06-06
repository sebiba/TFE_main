using Newtonsoft.Json;
using Requete;
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
    /// Logique d'interaction pour PdfViewer.xaml
    /// </summary>
    public partial class PdfViewer : Page
    {
        private Frame _frame;
        private string _pathPdf;
        public PdfViewer(Frame nav, string path = "about:blank")
        {
            _frame = nav;
            _pathPdf = path;
            InitializeComponent();
            listServer.ItemsSource = GetPdf();
            pdfWebViewer.Navigate(new Uri(path));
        }

        public List<string> GetPdf()
        {
            try { 
                return JsonConvert.DeserializeObject<List<string>>(Request.Post(Request.GetToken(ReadConf("pseudo"), ReadConf("password")), "http://tfe.moovego.be/api/ApiApp/GetFiles"));
            }
            catch
            {
                MessageBox.Show("une erreur de connection avec le serveur est survenue", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<string>();
            }
        }

        private void UploadPdf(object sender, EventArgs e)
        {
            Request.PostFile(Request.GetToken(ReadConf("pseudo"), ReadConf("password")), _pathPdf);
            listServer.ItemsSource = GetPdf();
            MessageBox.Show("Le pdf a bien été envoyé dans votre espace personnel enligne");
        }

        private void DownloadPdf(object sender, EventArgs e)
        {
            var test = Request.DownloadFile(Request.GetToken(ReadConf("pseudo"), ReadConf("password")), listServer.SelectedItem.ToString(), ReadConf("PartiFolder"));
        }

        private void SharePdf(object sender, EventArgs e)
        {
            if ("\"True\"" == Request.Get(Request.GetToken(ReadConf("pseudo"), ReadConf("password")), "http://tfe.moovego.be/api/ApiApp/Share", new Dictionary<string, string> { { "toShare", listServer.SelectedItem.ToString() }, { "dest", ShareTo.Text } }))
            {
                MessageBox.Show("votre fichier à bien été partagé avec: " + ShareTo.Text, "partage", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Une erreur est survenue lors du partage.", "partage", MessageBoxButton.OK, MessageBoxImage.Error);
            };
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
