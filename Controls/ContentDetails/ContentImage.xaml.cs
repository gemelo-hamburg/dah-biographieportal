using Gemelo.Applications.Biographieportal.Code;
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
using Pete.Components.WpfExtensions.Localization;
using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Code.Enumerations;
using System.Diagnostics;

namespace Gemelo.Applications.Biographieportal.Controls.ContentDetails
{
    /// <summary>
    /// Interaktionslogik für ContentImage.xaml
    /// </summary>
    public partial class ContentImage : UserControl
    {
        public MediaFile Media { get; private set; }

        public ContentImage(MediaFile media, bool small = false)
        {
            Media = media;
            InitializeComponent();

            m_Img.Source = media.ImageSource;

            if (string.IsNullOrWhiteSpace(media.Copyright.De)) m_Txt.Visibility = Visibility.Collapsed;
            else m_Txt.SetTextsFromLocalizationString(media.Copyright);

            if (media.Type == MediaFileType.Image)
            {
                m_BtnPlayVideo.Visibility = Visibility.Collapsed;
                m_BtnFullSize.Visibility = Visibility.Visible;
                if (small)
                {
                    m_BtnFullSize.Width = 40;
                    m_Txt.FontSize = 10;
                }
            }
            else if(media.Type== MediaFileType.Video)
            {
                m_BtnPlayVideo.Visibility = Visibility.Visible;
                m_BtnFullSize.Visibility = Visibility.Collapsed;
            }


        }

        private void BtnFullSize_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("BtnFullSize_Click");
            App.Current.MainWindow.ShowMediaFullscreen(Media);
        }
    }
}
