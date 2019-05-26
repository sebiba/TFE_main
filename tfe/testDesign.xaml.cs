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
    /// Logique d'interaction pour testDesign.xaml
    /// </summary>
    public partial class testDesign : Window
    {
        public testDesign()
        {
            InitializeComponent();
            Audio_Click();
        }

        private void Audio_Click(object sender=null, RoutedEventArgs e=null)
        {
            nav.Navigate(new input(nav));
            Audio.IsEnabled = false;
            Lilypond.IsEnabled = true;
            PDF.IsEnabled = true;
        }

        private void Lily_Click(object sender, RoutedEventArgs e)
        {
            nav.Navigate(new lilypond(nav));
            Audio.IsEnabled = true;
            Lilypond.IsEnabled = false;
            PDF.IsEnabled = true;
        }

        private void PDF_Click(object sender, RoutedEventArgs e)
        {
            nav.Navigate(new PdfViewer(nav));
            Audio.IsEnabled = true;
            Lilypond.IsEnabled = true;
            PDF.IsEnabled = false;
        }
    }
}
