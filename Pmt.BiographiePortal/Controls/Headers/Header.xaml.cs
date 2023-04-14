using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Code.Enumerations;
using Gemelo.Components.ClosedXml.Code.EventArguments;
using Gemelo.Components.Common.Exhibits.Settings;
using Gemelo.Components.Common.Wpf.Threading;
using Gemelo.Components.Common.Wpf.UI.Transitions.Appearance;
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

namespace Gemelo.Applications.Biographieportal.Controls.Headers
{
    /// <summary>
    /// Interaktionslogik für Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        #region Events


        public event EventHandler<EventArgs> BackClicked;

        protected void OnBackClicked()
        {
            BackClicked?.Invoke(this, new EventArgs());
        }


        public event EventHandler<EventArgs> HomeClicked;

        protected void OnHomeClicked()
        {
            HomeClicked?.Invoke(this, new EventArgs());
        }


        public event EventHandler<EventArgs> OwnStory;

        protected void OnOwnStory()
        {
            OwnStory?.Invoke(this, new EventArgs());
        }


        #endregion Events

        public Header()
        {
            InitializeComponent();

            if (App.Current != null)
            {
                App.Current.BiographyReader.ReadingFinished += BiographyReader_ReadingFinished;
            }
        }



        #region öffentliche Methoden

        public void UpdateByState(GameState state, BasePartialData currentFilter, int historyCount)
        {
            HideSliders();

            if (historyCount > 0) m_BtnBack.FadeInIfNotVisible();
            else m_BtnBack.FadeOutIfVisible();

            //m_BtnBack.FadeInOutByFlagAndVisible(historyCount > 0);

            //switch (state)
            //{
            //    case GameState.ContentList:
            //        if(currentFilter==null) m_BtnBack.FadeOutIfVisible();
            //        else m_BtnBack.FadeInIfNotVisible();
            //        break;
            //    case GameState.ContentDetail:
            //        m_BtnBack.FadeInIfNotVisible();
            //        break;
            //}
        }

        #endregion öffentliche Methoden

        #region private Methoden

        private void ToggleSlider(CategoriesSlider toggleSlider)
        {
            RestartTimer.Restart();
            foreach (UIElement ui in m_GridCategorySliders.Children)
            {
                if (ui is CategoriesSlider cs)
                {
                    if (cs == toggleSlider) cs.Toggle();
                    else cs.Hide();
                }
            }
        }

        private void HideSliders()
        {
            RestartTimer.Restart();
            foreach (UIElement ui in m_GridCategorySliders.Children)
            {
                if (ui is CategoriesSlider cs)
                {
                    cs.Hide();
                }
            }
        }



        #endregion private Methoden

        #region EventHandler

        private CategoriesSlider m_CategoriesSlider_TimeRanges;
        private CategoriesSlider m_CategoriesSlider_MigrationEffects;
        private CategoriesSlider m_CategoriesSlider_MigrationTypes;
        private CategoriesSlider m_CategoriesSlider_MigrationReasons;

        private void CreateAllElements()
        {
            m_GridCategorySliders.Children.Clear();

            m_CategoriesSlider_TimeRanges = new CategoriesSlider();
            m_CategoriesSlider_TimeRanges.SetContent(App.Current.BiographyStore.GetAllTimeRanges());
            m_GridCategorySliders.Children.Add(m_CategoriesSlider_TimeRanges);

            m_CategoriesSlider_MigrationEffects = new CategoriesSlider();
            m_CategoriesSlider_MigrationEffects.SetContent(App.Current.BiographyStore.GetAllMigrationEffects());
            m_GridCategorySliders.Children.Add(m_CategoriesSlider_MigrationEffects);

            m_CategoriesSlider_MigrationTypes = new CategoriesSlider();
            m_CategoriesSlider_MigrationTypes.SetContent(App.Current.BiographyStore.GetAllMigrationTypes());
            m_GridCategorySliders.Children.Add(m_CategoriesSlider_MigrationTypes);

            m_CategoriesSlider_MigrationReasons = new CategoriesSlider();
            m_CategoriesSlider_MigrationReasons.SetContent(App.Current.BiographyStore.GetAllMigrationReasons());
            m_GridCategorySliders.Children.Add(m_CategoriesSlider_MigrationReasons);
        }


        #endregion EventHandler

        private void BiographyReader_ReadingFinished(object sender, ReadingFinishedEventArgs e)
        {
            Dispatcher.BeginInvokeWithCheckAccess(() =>
            {
                CreateAllElements();
            });
        }

        private void BtnTimeRanges_Clicked(object sender, EventArgs e)
        {
            ToggleSlider(m_CategoriesSlider_TimeRanges);
        }

        private void BtnMigrationTypes_Clicked(object sender, EventArgs e)
        {
            ToggleSlider(m_CategoriesSlider_MigrationTypes);
        }


        private void BtnMigrationReasons_Clicked(object sender, EventArgs e)
        {
            ToggleSlider(m_CategoriesSlider_MigrationReasons);
        }

        private void BtnMigrationEffects_Clicked(object sender, EventArgs e)
        {
            ToggleSlider(m_CategoriesSlider_MigrationEffects);
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            OnHomeClicked();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            OnBackClicked();
        }

        private void BtnOwnStory_Click(object sender, RoutedEventArgs e)
        {
            OnOwnStory();
        }

    }
}
