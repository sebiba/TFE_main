using Newtonsoft.Json;
using Requete;
using System;
using System.Collections.Generic;
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
            return JsonConvert.DeserializeObject<List<string>>(Request.Post(Request.GetToken("sebiba@gmail.com", "Sebiba1330#"), "http://localhost:51727/api/ApiApp/GetFiles"));
        }

        private void UploadPdf(object sender, EventArgs e)
        {
            Request.PostFile(Request.GetToken("sebiba@gmail.com", "Sebiba1330#"), _pathPdf);
            listServer.ItemsSource = GetPdf();
        }

        private void DownloadPdf(object sender, EventArgs e)
        {
            var test = Request.DownloadFile(Request.GetToken("sebiba@gmail.com", "Sebiba1330#"), listServer.SelectedItem.ToString());
        }
    }
}
