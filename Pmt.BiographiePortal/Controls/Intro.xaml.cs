using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Components.Common.Wpf.UI.Transitions;
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

            if (App.HideLanguageBtn)
            {
                m_BtnLanguage.Visibility = Visibility.Collapsed;
            }
        }

        #region öffentliche Methoden

        public void Reset()
        {
            m_PanelIntro0.StopTransitionAndShow();
            m_PanelIntro1.StopTransitionAndCollapse();
            m_PanelIntro2.StopTransitionAndCollapse();
        }

        #endregion öffentliche Methoden

        #region private Methoden

        private void GotoIntro0()
        {
            m_PanelIntro0.FadeInIfNotVisible();
            m_PanelIntro1.FadeOutIfVisible();
            m_PanelIntro2.FadeOutIfVisible();
        }

        private void GotoIntro1()
        {
            m_PanelIntro0.FadeOutIfVisible();
            m_PanelIntro1.FadeInIfNotVisible();
            m_PanelIntro2.FadeOutIfVisible();
        }

        private void GotoIntro2()
        {
            m_PanelIntro0.FadeOutIfVisible();
            m_PanelIntro1.FadeOutIfVisible();
            m_PanelIntro2.FadeInIfNotVisible();
        }

        #endregion private Methoden

        //private void BtnEnglish_Click(object sender, RoutedEventArgs e)
        //{
        //    Gemelo.Components.Common.Localization.Languages.ChangeTo("en");
        //    OnProceed();
        //}

        //private void BtnGerman_Click(object sender, RoutedEventArgs e)
        //{
        //    Gemelo.Components.Common.Localization.Languages.ChangeTo("de");
        //    //App.Current.CurrentLanguage = "de";
        //    OnProceed();
        //}

        private void BtnOwnStory_Click(object sender, RoutedEventArgs e)
        {
            OnOwnStory();
        }

        private void BtnProceedToIntro2_Clicked(object sender, EventArgs e)
        {
            GotoIntro2();
        }

        private void BtnProceedToPortal_Clicked(object sender, EventArgs e)
        {
            OnProceed();
        }


        private void PanelIntro0_TouchDown(object sender, TouchEventArgs e)
        {
            GotoIntro1();
        }

        private void PanelIntro0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GotoIntro1();
        }
    }
}
