using Pete.Components.WpfExtensions.Extensions;
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

namespace Gemelo.Applications.Biographieportal.Controls
{
    /// <summary>
    /// Interaktionslogik für Impressum.xaml
    /// </summary>
    public partial class Impressum : UserControl
    {
        public Impressum()
        {
            InitializeComponent();
        }

        public void ShowUp()
        {
            this.FadeInIfNotVisible();
        }

        public void HideDown()
        {
            this.FadeOutIfVisible();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            HideDown();
        }
    }
}
