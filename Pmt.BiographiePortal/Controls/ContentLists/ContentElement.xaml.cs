using Gemelo.Applications.Biographieportal.Code.Data;
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

namespace Gemelo.Applications.Biographieportal.Controls.ContentLists
{
    /// <summary>
    /// Interaktionslogik für ContentElement.xaml
    /// </summary>
    public partial class ContentElement : UserControl
    {
        #region private Member

        private BiographyPart m_BiographyTeaser;

        #endregion private Member

        #region öffentliche Properties

        public Biography Bio { get; set; }

        #endregion öffentliche Properties

        #region Events


        #region Event Selected

        public event EventHandler<SelectedEventArgs> Selected;

        public class SelectedEventArgs : EventArgs
        {
            public Biography Bio { get; private set; }

            public SelectedEventArgs(Biography _Bio)
            {
                Bio = _Bio;
            }
        }

        protected void OnSelected(Biography _Bio)
        {
            Selected?.Invoke(this, new SelectedEventArgs(_Bio));
        }

        #endregion Event Selected

        #endregion Events

        #region ctor

        public ContentElement(Biography bio)
        {
            Bio = bio;
            InitializeComponent();
            m_BiographyTeaser = bio.GetTeaserOrFirst();
            if (m_BiographyTeaser != null && m_BiographyTeaser.MediaFile != null && m_BiographyTeaser.MediaFile.ImageSource != null)
            {
                m_BorderImageMissing.Visibility = Visibility.Collapsed;
                m_Img.Source = m_BiographyTeaser.MediaFile.ImageSource;
            }
            else
            {
                m_BorderImageMissing.Visibility = Visibility.Visible;
                m_Img.Source = null;
                m_Img.Visibility = Visibility.Collapsed;
            }
        }

        #endregion ctor

        #region öffentliche Methoden


        #endregion öffentliche Methoden

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnSelected(Bio);
        }

    }
}
