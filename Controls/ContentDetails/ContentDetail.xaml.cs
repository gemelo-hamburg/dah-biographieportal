using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Code.Enumerations;
using Pete.Components.WpfExtensions.Extensions;
using Pete.Components.WpfExtensions.Localization;
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

namespace Gemelo.Applications.Biographieportal.Controls.ContentDetails
{
    /// <summary>
    /// Interaktionslogik für ContentDetail.xaml
    /// </summary>
    public partial class ContentDetail : UserControl
    {
        private const double ConstDistanceToNextControl = 20.0;

        private BiographyPart m_TeaserOrFirstPart;
        private Style m_TextStyleText;

        public event EventHandler<EventArgs> OwnStory;

        protected void OnOwnStory()
        {
            OwnStory?.Invoke(this, new EventArgs());
        }

        public ContentDetail()
        {
            InitializeComponent();

            m_TextStyleText = (Style)FindResource("TextStyle_Text");
        }

        #region öffentliche Methoden

        public void ShowUp()
        {
            this.FadeInIfNotVisible();
            m_Scrollviewer.ScrollToVerticalOffset(0);
        }

        public void ShowDetails(Biography bio)
        {
            m_StackColumn0.Children.Clear();
            m_StackColumn1.Children.Clear();

            BiographyPart teaser = bio.GetTeaserOrFirst();
            if (teaser != null)
            {
                m_TeaserOrFirstPart = teaser;
                m_TxtHeader.SetTextsFromLocalizationString(teaser.Title);

                TextBlock txtText = new TextBlock();
                txtText.Style = m_TextStyleText;
                txtText.SetTextsFromLocalizationString(teaser.Text);
                txtText.Margin = new Thickness(0, 0, 0, ConstDistanceToNextControl);
                m_StackColumn1.Children.Add(txtText);

                if (teaser.MediaFile.Type != MediaFileType.NotExist)
                {
                    ContentImage ci = new ContentImage(teaser.MediaFile);
                    ci.Margin = new Thickness(0, 0, 0, ConstDistanceToNextControl);
                    m_StackColumn0.Children.Add(ci);
                }
            }

            foreach (BiographyPart additional in bio.Parts)
            {
                if (additional != teaser)
                {
                    AdditionalParts addView = new AdditionalParts(additional);
                    addView.Margin = new Thickness(0, 0, 0, ConstDistanceToNextControl);
                    m_StackColumn0.Children.Add(addView);
                }
            }

            CategoriesDisplay cd = new CategoriesDisplay(bio);
            m_StackColumn0.Children.Add(cd);


            DahButton btnSimilarBios = new DahButton();
            btnSimilarBios.FireClickAtTouchDown = true;
            btnSimilarBios.ButtonBackground = new SolidColorBrush(MigrationType.StaticTypeColor);
            btnSimilarBios.TextDe = "Ähnliche Biographien";
            btnSimilarBios.TextEn = "Similar Biographies";
            btnSimilarBios.Clicked += BtnSimilarBios_Clicked;
            btnSimilarBios.Margin = new Thickness(0, 0, 0, ConstDistanceToNextControl);
            m_StackColumn1.Children.Add(btnSimilarBios);

            foreach (HistoricBackground hb in bio.HistoricBackgrounds)
            {
                HistoricBackgroundView hbv = new HistoricBackgroundView(hb);
                hbv.Margin = new Thickness(0, 0, 0, ConstDistanceToNextControl);
                m_StackColumn1.Children.Add(hbv);
            }

            ButtonOwnStory buttonOwnStory = new ButtonOwnStory();
            buttonOwnStory.Margin = new Thickness(0, 50, 0, 50);
            buttonOwnStory.ButtonClicked += ButtonOwnStory_ButtonClicked;
            m_StackColumn1.Children.Add(buttonOwnStory);

        }

        #endregion öffentliche Methoden

        #region EventHandler

        private void BtnSimilarBios_Clicked(object sender, EventArgs e)
        {
            App.Current.MainWindow.SetContentListFilter(m_TeaserOrFirstPart, addToHistory: true);
        }

        private void Control_TouchDown(object sender, TouchEventArgs e)
        {
            App.Current.ExhibitMainWindow.ResetRestartTimer();
        }

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.ExhibitMainWindow.ResetRestartTimer();
        }


        private void Scrollviewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void Scrollviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (App.Current != null)
            {
                App.Current.ExhibitMainWindow.ResetRestartTimer();
            }
        }

        private void ButtonOwnStory_ButtonClicked(object sender, EventArgs e)
        {
            OnOwnStory();
        }


        #endregion EventHandler

    }
}
