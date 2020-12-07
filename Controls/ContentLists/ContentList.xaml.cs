using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Code.Enumerations;
using Gemelo.Components.ClosedXml.Code.EventArguments;

using Pete.Components.Extensions.Classes;
using Pete.Components.WpfExtensions.Extensions;
using Pete.Components.WpfExtensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Gemelo.Applications.Biographieportal.Controls.ContentLists
{
    /// <summary>
    /// Interaktionslogik für ContentList.xaml
    /// </summary>
    public partial class ContentList : UserControl
    {
        #region private Member

        private List<ContentElement> m_AllElements;

        #endregion private Member

        public Biography SelectedItem { get; private set; }

        #region Events

        #region Event ContentSelected

        public event EventHandler<ContentSelectedEventArgs> ContentSelected;

        public class ContentSelectedEventArgs : EventArgs
        {
            public Biography Biography { get; private set; }

            public ContentSelectedEventArgs(Biography _Biography)
            {
                Biography = _Biography;
            }
        }

        protected void OnContentSelected(Biography _Biography)
        {
            ContentSelected?.Invoke(this, new ContentSelectedEventArgs(_Biography));
        }

        #endregion Event ContentSelected     

        #endregion Events

        #region ctor

        public ContentList()
        {
            m_AllElements = new List<ContentElement>();

            InitializeComponent();

            if (App.Current != null)
            {
                App.Current.BiographyReader.ReadingFinished += BiographyReader_ReadingFinished;
            }
        }

        #endregion ctor

        #region öffentliche Methoden

        public void ShowUp()
        {
            this.FadeInIfNotVisible();
            m_Scrollviewer.ScrollToVerticalOffset(0);
        }

        public void Reset()
        {
            m_Scrollviewer.ScrollToVerticalOffset(0);
            FilterBy(null);
        }

        public void SetFilter(BasePartialData data)
        {
            m_Scrollviewer.ScrollToVerticalOffset(0);
            if (data is BiographyPart bioPart) FilterBySimilarBio(bioPart.ParentBio);
            else FilterBy(data);
            //else if (data is MigrationEffect mf) FilterBy(mf);
            //else if (data is MigrationReason mr) FilterBy(mr);
            //else if (data is MigrationType mt) FilterBy(mt);
            //else if (data is TimeRange rt) FilterBy(rt);
        }



        #endregion öffentliche Methoden

        #region private Methoden

        private void CreateAllElements()
        {
            m_AllElements.Clear();
            m_PanelListItems.Children.Clear();
            m_PanelListPersons.Children.Clear();

            foreach (Biography bio in App.Current.BiographyStore)
            {
                CreateElement(bio, expectedMediaType: MediaContentType.ImagePerson, m_PanelListPersons);
            }
            foreach (Biography bio in App.Current.BiographyStore)
            {
                CreateElement(bio, expectedMediaType: MediaContentType.ImageItem, m_PanelListItems);
            }
            foreach (Biography bio in App.Current.BiographyStore)
            {
                CreateElement(bio, expectedMediaType: MediaContentType.Unknown, m_PanelListItems);
            }
        }

        private void CreateElement(Biography bio, MediaContentType expectedMediaType, UniformGrid grid)
        {
            var teaser = bio.GetTeaserOrFirst();
            if (teaser.MediaContentType == expectedMediaType)
            {
                ContentElement element = new ContentElement(bio);
                element.Margin = new Thickness(0, 40, 0, 40);
                element.Selected += Element_Selected;
                grid.Children.Add(element);
                m_AllElements.Add(element);
            }
        }

        #endregion private Methoden

        #region FilterMethoden

        private void FilterBy(BasePartialData categorie)
        {
            int similarCount = 0;

            foreach (ContentElement element in m_AllElements)
            {
                if (categorie == null || element.Bio.HasCategorie(categorie))
                {
                    similarCount++;
                    element.FadeInIfNotVisible(); //.StopWpfEffectAndShow();//
                }
                else element.StopWpfEffectAndCollapse();//.FadeOutIfVisible();
            }

            if (categorie == null)
            {
                m_PanelFilterName.Visibility = Visibility.Collapsed;
            }
            else
            {
                m_PanelFilterName.Visibility = Visibility.Visible;
                m_TxtFilterName.SetTextsFromLocalizationString(categorie.Text);

                //m_TxtFilterType.SetTextsFromLocalizationString(GetCategorieName(categorie));

                m_BtnMigrationEffect.Visibility = (categorie is MigrationEffect).ToVisibility();
                m_BtnMigrationReason.Visibility = (categorie is MigrationReason).ToVisibility();
                m_BtnMigrationType.Visibility = (categorie is MigrationType).ToVisibility();
                m_BtnTimeRange.Visibility = (categorie is TimeRange).ToVisibility();

                m_TxtFilterType.Visibility = Visibility.Collapsed;
                m_BtnSimilarBiographies.Visibility = Visibility.Collapsed;

                m_TxtNoOneFound.Visibility = (similarCount == 0).ToVisibility();
            }
        }

        private LocalizationString GetCategorieName(BasePartialData categorie)
        {
            if (categorie is TimeRange) return LocalizationString.Create("Zeitraum: ", "Time Period: ");
            else if (categorie is MigrationEffect) return LocalizationString.Create("Folge: ", "Effect: ");
            else if (categorie is MigrationReason) return LocalizationString.Create("Migrationsgrund: ", "Migration Reason: ");
            else if (categorie is MigrationType) return LocalizationString.Create("Migrationsart: ", "Migration Type: ");
            else if (categorie is HistoricBackground) return LocalizationString.Create("Historischer Hintergrund: ", "Historic Background: ");
            else return LocalizationString.Create("Sonstiges: ", "Others: ");
        }


        //private void FilterBy(MigrationType mt)
        //{
        //}

        //private void FilterBy(MigrationReason mr)
        //{
        //}

        //private void FilterBy(MigrationEffect mf)
        //{
        //}

        private void FilterBySimilarBio(Biography parentBio)
        {
            // alle einblenden, für die zwei oder mehr Kategorien übereinstimmen
            int similarCount = 0;
            foreach (ContentElement element in m_AllElements)
            {
                if (element.Bio.HasSimilarCategories(parentBio, 2))
                {
                    similarCount++;
                    element.FadeInIfNotVisible(); //.StopWpfEffectAndShow();//
                }
                else element.StopWpfEffectAndCollapse();//.FadeOutIfVisible();
            }

            m_PanelFilterName.Visibility = Visibility.Visible;

            m_TxtNoOneFound.Visibility = (similarCount == 0).ToVisibility();

            m_TxtFilterName.SetTextsFromLocalizationString(parentBio.GetTeaserOrFirst().Title);

            m_TxtFilterType.Visibility = Visibility.Collapsed;
            //m_TxtFilterType.SetTextsFromLocalizationString(LocalizationString.Create("ähnliche Biografien zu: ", "similar biographies to: "));

            m_BtnMigrationEffect.Visibility = Visibility.Collapsed;
            m_BtnMigrationReason.Visibility = Visibility.Collapsed;
            m_BtnMigrationType.Visibility = Visibility.Collapsed;
            m_BtnTimeRange.Visibility = Visibility.Collapsed;
            m_BtnSimilarBiographies.Visibility = Visibility.Visible;

        }

        #endregion FilterMethoden

        #region EventHandler

        private void BiographyReader_ReadingFinished(object sender, ReadingFinishedEventArgs e)
        {
            Dispatcher.BeginInvokeWithCheckAccess(() =>
            {
                CreateAllElements();
            });
        }

        private void Element_Selected(object sender, ContentElement.SelectedEventArgs e)
        {
            SelectedItem = e.Bio;
            OnContentSelected(e.Bio);
        }


        #endregion EventHandler

        //private void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        //{
        //    e.Handled = true;
        //}

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
    }
}
