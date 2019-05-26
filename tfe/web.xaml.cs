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
using System.Windows.Shapes;

namespace tfe
{
    /// <summary>
    /// Logique d'interaction pour web.xaml
    /// </summary>
    public partial class web : Window
    {
        public string pdfPath { get; set; }
        public web()
        {
            InitializeComponent();
        }

        private void Envois_Click(object sender, RoutedEventArgs e)
        {
            Request.PostFile(Request.GetToken(Username.Text, Password.Text), pdfPath);
        }
    }
}
