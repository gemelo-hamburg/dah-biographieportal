using Gemelo.Components.Common.Wpf.Localization;
using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Code.Enumerations;
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
using Gemelo.Components.Common.Wpf.UI;
using Gemelo.Components.Common.Wpf.UI.Transitions.Appearance;

namespace Gemelo.Applications.Biographieportal.Controls
{
    /// <summary>
    /// Interaktionslogik für MediaFullscreen.xaml
    /// </summary>
    public partial class MediaFullscreen : UserControl
    {
        public MediaFullscreen()
        {
            InitializeComponent();
        }

        public void ShowUp(MediaFile media)
        {
            this.FadeInIfNotVisible();
            m_Img.Source = media.ImageSource;

            if (string.IsNullOrWhiteSpace(media.Copyright.De)) m_Txt.Visibility = Visibility.Collapsed;
            else m_Txt.SetLocalizedText(media.Copyright);

            m_Img.Visibility = (media.Type == MediaFileType.Image).ToVisibility();

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
