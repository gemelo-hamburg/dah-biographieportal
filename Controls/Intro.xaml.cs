using Gemelo.Applications.Biographieportal.Applications;
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
    /// Interaktionslogik für Intro.xaml
    /// </summary>
    public partial class Intro : UserControl
    {

        public event EventHandler<EventArgs> Proceed;

        protected void OnProceed()
        {
            Proceed?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs> OwnStory;

        protected void OnOwnStory()
        {
            OwnStory?.Invoke(this, new EventArgs());
        }


        public Intro()
        {
            InitializeComponent();
        }

        private void BtnEnglish_Click(object sender, RoutedEventArgs e)
        {
            App.Current.CurrentLanguage = "en";
            OnProceed();
        }

        private void BtnGerman_Click(object sender, RoutedEventArgs e)
        {
            App.Current.CurrentLanguage = "de";
            OnProceed();
        }

        private void BtnOwnStory_Click(object sender, RoutedEventArgs e)
        {
            OnOwnStory();
        }
    }
}
