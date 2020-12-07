using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Code.Enumerations;
using Gemelo.Components.ClosedXml.Code.EventArguments;
using Gemelo.Components.ClosedXml.Code.ExcelReader;
using Gemelo.Components.Exhibits.Controls;
using Pete.Components.Extensions.Tracing;
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

namespace Gemelo.Applications.Biographieportal.Windows
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LocalizationExhibitMainWindow
    {
        #region private Member

        private BasePartialData m_CurrentFilter;
        private bool m_IsFirstStart = true;
        private object m_HistoryHome = new object();

        private Stack<object> m_History = new Stack<object>();

        #endregion private Member

        #region öffentliche Properties

        public GameState State { get; private set; }
        public static MainWindow CurrentMainWindow { get; private set; }

        #endregion öffentliche Properties

        #region ctor

        public MainWindow()
        {
            // Protokollieren
            TraceX.WriteLine(TraceXLevel.Informational, TraceMessages.CreateMainWindowStart);

            // XAML abarbeiten
            CurrentMainWindow = this;
            InitializeComponent();

            // Hauptgrid setzen, damit das Scaling funktionert.
            GridForScaling = m_GridMain;

            RestartTimerInterval = App.Current.RestartInterval;

            if (App.Current != null)
            {
                App.Current.BiographyReader.ReadingFinished += BiographyReader_ReadingFinished;
                m_ExcelReaderProgress.AddReader(App.Current.BiographyReader);
                m_ExcelReaderProgress.AutoHideIfNoError = true;
                m_ExcelReaderProgress.AutoHideIfError = !App.Current.WaitIfContentReadingError;
                m_ExcelReaderProgress.Closing += ExcelReaderProgress_Closing;

                m_Intro.Proceed += Intro_Proceed;
                m_Intro.OwnStory += Control_OwnStory;
                m_ContentList.ContentSelected += ContentList_ContentSelected;

                m_Header.HomeClicked += Header_HomeClicked;
                m_Header.BackClicked += Header_BackClicked;
                m_Header.OwnStory += Control_OwnStory;

                m_ContentDetail.OwnStory += Control_OwnStory;

                m_Mediaplayer.MediaControlsFadeoutTime = TimeSpan.FromSeconds(5);

                this.ManipulationBoundaryFeedback += MainWindow_ManipulationBoundaryFeedback;
            }

            // Protokollieren
            TraceX.WriteLine(TraceXLevel.Verbose, TraceMessages.CreateMainWindowEnd);
        }




        #endregion ctor

        #region öffentliche Methoden

        private object m_LatestAddUserHistory = null;

        public void AddUserHistory(object o)
        {
            if (m_History.Count == 0) m_History.Push(m_HistoryHome);
            m_LatestAddUserHistory = o;
            m_History.Push(o);
            UpdateHistoryUi();
        }

        private void ClearHistory()
        {
            m_History.Clear();
            m_LatestAddUserHistory = null;
        }


        public void ShowImpressum()
        {
            ResetRestartTimer();
            m_Impressum.ShowUp();
        }

        public void ShowMediaFullscreen(MediaFile media)
        {
            ResetRestartTimer();

            if (media.Type == MediaFileType.Image) m_MediaFullscreen.ShowUp(media);
            else if (media.Type == MediaFileType.Video) m_Mediaplayer.ShowUp(media);
        }


        public void SetContentListFilter(BasePartialData data, bool addToHistory)
        {
            m_CurrentFilter = data;
            if (State == GameState.ContentDetail) FromContentDetailToContentList();
            m_Header.UpdateByState(State, m_CurrentFilter, m_History.Count);
            m_ContentList.SetFilter(data);

            if (addToHistory) AddUserHistory(data);
        }


        #endregion öffentliche Methoden

        #region Protected Methoden

        /// <summary>
        /// Wird unter anderem aufgerufen, wenn der Restart-Timer abläuft
        /// </summary>
        protected override void Restart()
        {
            // Protokollieren
            StopRestartTimer();
            TraceX.WriteInfo(TraceMessages.Restart);
            RestartTimerInterval = App.Current.RestartInterval;
            m_ContentList.Reset();
            ToIntro();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.F2:
                    m_BorderTraceLiveView.ToggleVisibility();
                    break;
                case Key.F4:
                    Restart();
                    break;
                case Key.F5:
                    App.Current.ContentAutoUpdater.DoUpdate();
                    break;

            }
        }

        #endregion Protected Methoden

        #region private Methoden

        private void GotoLastHistoryView()
        {
            if (m_History.Count > 0)
            {
                object history = m_History.Pop();
                if (history == m_LatestAddUserHistory)
                {
                    m_LatestAddUserHistory = null;
                    GotoLastHistoryView();
                }
                else if (history is Biography bio)
                {
                    m_ContentDetail.ShowDetails(bio);
                    ToContentDetail();
                }
                else if (history is BasePartialData bpd)
                {
                    SetContentListFilter(bpd, addToHistory: false);
                    ToContentList();
                }
                else if (history == m_HistoryHome)
                {
                    SetContentListFilter(null, addToHistory: false);
                    ToContentList();
                }
            }
        }


        private void UpdateHistoryUi()
        {
            m_Header.UpdateByState(GameState.Unknown, null, m_History.Count);
        }

        #endregion private Methoden

        #region From To Methoden

        private void ToIntro()
        {
            TraceX.WriteInfo($"{nameof(ToIntro)}() ...");
            State = GameState.Intro;
            m_ExcelReaderProgress.FadeOutIfVisible();
            m_Intro.FadeInIfNotVisible();
            m_Impressum.HideDown();
            m_MediaFullscreen.HideDown();
            m_Mediaplayer.HideDown();
            m_SendOwnStory.FadeOutIfVisible();
            m_ContentDetail.FadeOutIfVisible();
            m_ContentList.FadeInIfNotVisible();
            ClearHistory();
        }

        private void FromIntroToContentList()
        {
            TraceX.WriteInfo($"{nameof(FromIntroToContentList)}() ...");
            State = GameState.ContentList;
            ResetRestartTimer();
            m_Intro.FadeOutIfVisible();
            m_ContentList.ShowUp();
            m_Header.UpdateByState(State, m_CurrentFilter, m_History.Count);
        }

        private void FromContentDetailToContentList()
        {
            ToContentList();
            //TraceX.WriteInfo($"{nameof(FromContentDetailToContentList)}() ...");
            //State = GameState.ContentList;
            //ResetRestartTimer();
            //m_ContentDetail.FadeOutIfVisible();
            //m_ContentList.ShowUp();
            //m_Header.UpdateByState(State, m_CurrentFilter);
        }

        private void ToContentList()
        {
            TraceX.WriteInfo($"{nameof(FromContentDetailToContentList)}() ...");
            State = GameState.ContentList;
            ResetRestartTimer();
            m_ContentDetail.FadeOutIfVisible();
            m_ContentList.ShowUp();
            m_Header.UpdateByState(State, m_CurrentFilter, m_History.Count);
        }

        private void ToContentDetail()
        {
            TraceX.WriteInfo($"FromContentListToContentDetail() ...");
            State = GameState.ContentDetail;
            ResetRestartTimer();
            m_ContentList.FadeOutIfVisible();
            m_ContentDetail.ShowUp();
            m_Header.UpdateByState(State, m_CurrentFilter, m_History.Count);
        }

        #endregion From To Methoden

        #region EventHandler

        private void BiographyReader_ReadingFinished(object sender, ReadingFinishedEventArgs e)
        {
            Dispatcher.BeginInvokeWithCheckAccess(() =>
            {
                if (!m_IsFirstStart) Restart();
            });
        }

        private void ExcelReaderProgress_Closing(object sender, EventArgs e)
        {
            m_IsFirstStart = false;
            Restart();
        }

        private void Intro_Proceed(object sender, EventArgs e)
        {
            if (this.State == GameState.Intro) FromIntroToContentList();
        }

        private void ContentList_ContentSelected(object sender, Controls.ContentLists.ContentList.ContentSelectedEventArgs e)
        {
            if (this.State == GameState.ContentList)
            {
                m_ContentDetail.ShowDetails(m_ContentList.SelectedItem);
                AddUserHistory(m_ContentList.SelectedItem);
                ToContentDetail();
            }
        }

        private void Header_BackClicked(object sender, EventArgs e)
        {
            GotoLastHistoryView();

            //if (this.State == GameState.ContentDetail) FromContentDetailToContentList();
            //else if (this.State == GameState.ContentList) SetFilter(null);
        }

      
        private void Header_HomeClicked(object sender, EventArgs e)
        {
            ClearHistory();
            SetContentListFilter(null, addToHistory: false);
            ToContentList();
            UpdateHistoryUi();
        }



        private void Control_OwnStory(object sender, EventArgs e)
        {
            ResetRestartTimer();
            m_SendOwnStory.ShowUp();
        }


        private void MainWindow_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }


        #endregion EventHandler

    }
}
