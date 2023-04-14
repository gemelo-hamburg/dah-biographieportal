using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Pmt.BiographiePortal.Code.Misc;
using Gemelo.Components.Common.Exhibits.Settings;
using Gemelo.Components.Common.Wpf.Controls.Buttons;
using Gemelo.Components.Common.Wpf.UI;
using Gemelo.Components.Common.Wpf.UI.Transitions.Appearance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Gemelo.Applications.Biographieportal.Controls
{
    public partial class BioMediaplayer : UserControl
    {
        #region Konstanten

        private static readonly TimeSpan ConstPositionUpdateEventInterval = TimeSpan.FromMilliseconds(500.0);
        private static readonly TimeSpan ConstSecureEndThreshold = TimeSpan.FromMilliseconds(150.0);

        #endregion Konstanten

        #region Private Variablen

        private bool m_BorderTouchMouseDown = false;
        private bool m_AutoPlayIfOpened = false;

        private IEnumerable<TouchButton> m_Buttons;

        private TimeSpan m_TimespanSkip;

        private DateTime m_LastPositionUpdateEventTime = DateTime.Now;
        private TimeSpan? m_LastPosition;
        private DispatcherTimer m_TimerFadeoutControls;
        private Action m_ActionDismissMediaControlsByTimer;

        #endregion Private Variablen

        #region Öffentliche Properties


        private double m_Volume = 1.0;

        public double Volume
        {
            get { return m_Volume; }
            set { SetVolume(value); }
        }



        private bool m_IsMediaControlsVisible = true;

        public bool IsMediaControlsVisible
        {
            get { return m_IsMediaControlsVisible; }
            set
            {
                if (value != m_IsMediaControlsVisible)
                {
                    if (value) FadeInMediaControls();
                    else FadeOutMediaControls();
                }
            }
        }

        public bool ShowMediaControlsOnTouch { get; set; } = true;

        #region DependencyProperty MediaControlsFadeoutTime

        public static readonly DependencyProperty MediaControlsFadeoutTimeProperty = DependencyProperty.Register(
           "MediaControlsFadeoutTime", typeof(TimeSpan?), typeof(BioMediaplayer),
           new PropertyMetadata(null, new PropertyChangedCallback(OnMediaControlsFadeoutTimeChanged)));

        [Description("MediaControlsFadeoutTime")]
        [Category("MediaControlsFadeoutTime Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TimeSpan? MediaControlsFadeoutTime
        {
            get { return ((TimeSpan?)(GetValue(BioMediaplayer.MediaControlsFadeoutTimeProperty))); }
            set { SetValue(BioMediaplayer.MediaControlsFadeoutTimeProperty, value); }
        }

        private static void OnMediaControlsFadeoutTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BioMediaplayer ctrl)
            {
                TimeSpan? newValue = (TimeSpan?)e.NewValue;
                if (newValue.HasValue) ctrl.m_TimerFadeoutControls.Interval = newValue.Value;
                else ctrl.m_TimerFadeoutControls.Stop();
            }
        }

        #endregion DependencyProperty MediaControlsFadeoutTime

        #region DependencyProperty IsStopButtonVisible

        public static readonly DependencyProperty IsStopButtonVisibleProperty = DependencyProperty.Register(
           "IsStopButtonVisible", typeof(bool), typeof(BioMediaplayer),
           new PropertyMetadata(true, new PropertyChangedCallback(OnIsStopButtonVisibleChanged)));

        [Description("IsStopButtonVisible")]
        [Category("IsStopButtonVisible Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsStopButtonVisible
        {
            get { return ((bool)(GetValue(BioMediaplayer.IsStopButtonVisibleProperty))); }
            set { SetValue(BioMediaplayer.IsStopButtonVisibleProperty, value); }
        }

        private static void OnIsStopButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BioMediaplayer ctrl)
            {
                bool newValue = (bool)e.NewValue;
                ctrl.m_BtnStop.Visibility = newValue.ToVisibility();
            }
        }



        #endregion DependencyProperty IsStopButtonVisible

        #region DependencyProperty Source

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
           "Source", typeof(Uri), typeof(BioMediaplayer),
           new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        [Description("Source")]
        [Category("Source Category")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Uri Source
        {
            get { return ((Uri)(GetValue(BioMediaplayer.SourceProperty))); }
            set { SetValue(BioMediaplayer.SourceProperty, value); }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BioMediaplayer ctrl)
            {
                Uri newValue = (Uri)e.NewValue;
                Uri oldValue = (Uri)e.OldValue;

                if (newValue != oldValue)
                {
                    ctrl.SetControlButtonsEnabled(newValue != null);
                }
            }
        }

        #endregion DependencyProperty Source

        #endregion Öffentliche Properties

        #region Events


        public event EventHandler<EventArgs> StopClicked;

        protected void OnStopClicked()
        {
            StopClicked?.Invoke(this, new EventArgs());
        }


        public event EventHandler<EventArgs> MediaEnded;

        protected void OnMediaEnded()
        {
            MediaEnded?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs> MediaOpened;

        protected void OnMediaOpened()
        {
            MediaOpened?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs> CloseClicked;

        protected void OnCloseClicked()
        {
            CloseClicked?.Invoke(this, new EventArgs());
        }

        public event EventHandler PositionChanged;

        protected void OnPositionChanged()
        {
            PositionChanged?.Invoke(this, null);
        }

        #endregion Events

        #region Konstruktur und Initialisierungen

        public BioMediaplayer()
        {
            InitializeComponent();

            InitializeThings();
            if (App.Current != null) CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void InitializeThings()
        {
#if(DEBUG)
            m_TxtDebug.Visibility = Visibility.Collapsed;
            m_BorderTouch.Visibility = Visibility.Collapsed;
#endif
            m_TxtDebug.Visibility = Visibility.Collapsed;
            m_BorderTouch.Visibility = Visibility.Collapsed;


            //m_MediaElement.FadeOutIfVisible();

            Binding b = new Binding("Source")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(BioMediaplayer), 1)
            };
            BindingOperations.SetBinding(m_MediaElement, MediaElement.SourceProperty, b);

            m_TimerFadeoutControls = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            m_TimerFadeoutControls.Tick += TimerFadeoutControls_Tick;

            m_MediaElement.LoadedBehavior = MediaState.Manual;
            m_MediaElement.UnloadedBehavior = MediaState.Manual;
            m_MediaElement.MediaOpened += MediaElement_MediaOpened;
            m_MediaElement.MediaEnded += MediaElement_MediaEnded;
            m_MediaElement.MouseDown += MediaElement_MouseDown;

            m_BorderTouch.PreviewMouseDown += BorderTouch_PreviewMouseDown;
            m_BorderTouch.PreviewMouseMove += BorderTouch_PreviewMouseMove;
            m_BorderTouch.PreviewMouseUp += BorderTouch_PreviewMouseUp;
            m_BorderTouch.MouseLeave += BorderTouch_MouseLeave;

            m_Buttons = this.FindLogicalChildren<TouchButton>();

            SetControlButtonsEnabled(enabled: false);

            SetVolume(m_Volume);
        }

        #endregion Konstruktur und Initialisierungen

        #region Öffentliche Methoden

        public void HideDown()
        {
            Stop();
        }

        public async Task ShowUp(MediaFile media)
        {
            this.FadeInIfNotVisible();
            Source = new Uri(media.FileInfo.FullName);
            FadeInMediaControls();
            m_AutoPlayIfOpened = true;
            //await Task.Delay(500);
            //Play();
        }


        public void Stop()
        {
            RestartTimer.Restart();
            m_AutoPlayIfOpened = false;
            m_MediaElement.Stop();
            Source = null;
            this.FadeOutIfVisible();
            RestartMediaControlFadeoutTimer();
            OnStopClicked();
        }


        public void Backward()
        {
            RestartTimer.Restart();
            m_MediaElement.Position = m_MediaElement.Position - m_TimespanSkip;
            RestartMediaControlFadeoutTimer();
        }

        public void Forward()
        {
            RestartTimer.Restart();
            m_MediaElement.Position = m_MediaElement.Position + m_TimespanSkip;
            RestartMediaControlFadeoutTimer();
        }

        public void Close()
        {
            RestartTimer.Restart();
            Stop();
            OnCloseClicked();
            RestartMediaControlFadeoutTimer();
        }

        public void Play()
        {
            //m_MediaElement.FadeInIfNotVisible();
            RestartTimer.Restart();
            m_MediaElement.Play();
            RestartMediaControlFadeoutTimer();
        }

        public void Pause()
        {
            RestartTimer.Restart();
            m_MediaElement.Pause();
            RestartMediaControlFadeoutTimer();
        }

        public void SetControlButtonsEnabled(bool enabled)
        {
            //foreach (TouchButton btn in m_Buttons)
            //{
            //    if (btn.ButtonType != Code.Enumerations.ButtonType.Close) btn.IsEnabled = enabled;
            //}
            //m_PanelControlButtons.Opacity = enabled ? 1.0 : 0.5;
        }

        public void SlideInMediaControls()
        {
            m_PanelMediaControlsAndProgressBar.SlideIn(SlideDirection.Bottom);
            m_BtnClose.FadeIn();
            m_IsMediaControlsVisible = true;
            m_ActionDismissMediaControlsByTimer = SlideOutMediaControls;
            RestartMediaControlFadeoutTimer();
        }

        public void SlideOutMediaControls()
        {
            m_PanelMediaControlsAndProgressBar.SlideOut(SlideDirection.Bottom);
            m_BtnClose.FadeOutIfVisible();
            m_IsMediaControlsVisible = false;
            m_TimerFadeoutControls.Stop();
        }

        public void FadeInMediaControls()
        {
            m_PanelMediaControlsAndProgressBar.FadeInIfNotVisible();
            m_BtnClose.FadeInIfNotVisible();
            m_IsMediaControlsVisible = false;
            m_ActionDismissMediaControlsByTimer = FadeOutMediaControls;
            RestartMediaControlFadeoutTimer();
        }

        public void FadeOutMediaControls()
        {
            m_PanelMediaControlsAndProgressBar.FadeOutIfVisible();
            m_BtnClose.FadeOutIfVisible();
            m_IsMediaControlsVisible = false;
            m_TimerFadeoutControls.Stop();
        }

        #endregion Öffentliche Methoden

        #region Private Methoden


        private void RestartMediaControlFadeoutTimer()
        {
            if (MediaControlsFadeoutTime.HasValue)
            {
                m_TimerFadeoutControls.Stop();
                m_TimerFadeoutControls.Start();
            }
        }

        private void UpdatePosition()
        {
            if (!m_BorderTouchMouseDown)
            {
                Duration duration = m_MediaElement.NaturalDuration;
                if (duration != Duration.Automatic && duration.HasTimeSpan)
                {
                    if (IsVisible)
                    {
                        RestartTimer.Restart();
                    }

                    double pos = m_MediaElement.Position.TotalMilliseconds;
                    double length = duration.TimeSpan.TotalMilliseconds;

                    double percentage = pos / length;

                    double barLength = m_PanelProgressBar.ActualWidth - m_ImgProgressDot.ActualWidth;
                    double relativePos = percentage * barLength;

                    m_TranslateDot.X = relativePos;
                    //m_TranslateTouch.X = MathEx.MinMax(
                    //    value: relativePos - m_BorderTouch.ActualWidth / 2.0 + m_ImgProgressDot.ActualWidth / 2.0,
                    //    min: 0,
                    //    max: m_PanelProgressBar.ActualWidth - m_BorderTouch.ActualWidth);

                    TimeSpan t = m_MediaElement.Position;
                    m_TxtCurrentPosition.Text = $"{t.TotalMinutes:0}:{t.Seconds:00}";
                }
            }
        }

        private void CheckForPositionChangeEvent()
        {
            DateTime now = DateTime.Now;
            if (now > m_LastPositionUpdateEventTime + ConstPositionUpdateEventInterval)
            {
                TimeSpan newPosition = m_MediaElement.Position;
                if (!m_LastPosition.HasValue || m_LastPosition.Value != newPosition)
                {
                    m_LastPosition = newPosition;
                    m_LastPositionUpdateEventTime = now;
                    OnPositionChanged();
                }
            }
        }

        private void CheckForSecureEnd()
        {
            if (m_MediaElement.NaturalDuration.HasTimeSpan && m_MediaElement.Position > TimeSpan.Zero)
            {
                if (m_MediaElement.Position > m_MediaElement.NaturalDuration.TimeSpan - ConstSecureEndThreshold)
                {
                    Stop();
                    OnMediaEnded();
                }
            }
        }

        private void BorderTouchMouseDownStart(MouseDevice mouseDevice)
        {
            m_BorderTouchMouseDown = true;
            mouseDevice.Capture(m_BorderTouch);
            m_First = true;
        }

        private bool m_First;
        private void BorderTouchMouseMove(Point point2Panel, Point point2Border, MouseDevice mouseDevice)
        {
            RestartMediaControlFadeoutTimer();
            if (m_BorderTouchMouseDown && m_MediaElement.NaturalDuration.HasTimeSpan)
            {
                double shiftX = point2Panel.X;
                if (m_First)
                {
                    m_First = false;
                    shiftX -= point2Border.X;
                }
                //m_TranslateDot.X = shiftX;
                m_TranslateTouch.X = shiftX;

                double percentage = shiftX / m_PanelProgressBar.ActualWidth;

                TimeSpan newPosition = TimeSpan.FromMilliseconds(m_MediaElement.NaturalDuration.TimeSpan.TotalMilliseconds * percentage);
                m_TxtDebug.Text = $"{newPosition}";
                //m_MediaElement.Position = newPosition;
            }
        }

        private void BorderTouchMouseDownEnded(MouseDevice mouseDevice)
        {
            m_BorderTouchMouseDown = false;
            mouseDevice.Capture(null);
        }

        private void SetVolume(double value)
        {
            if (value != m_Volume)
            {
                m_Volume = value;
                if (m_MediaElement != null) m_MediaElement.Volume = m_Volume;
            }
        }

        #endregion Private Methoden

        #region Eventhandler

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (IsVisible)
            {
                UpdatePosition();
                CheckForPositionChangeEvent();
                CheckForSecureEnd();
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(m_MediaElement.NaturalDuration);
            Debug.WriteLine(m_MediaElement.Position);
            if (m_MediaElement.NaturalDuration.HasTimeSpan)
            {
                m_TimespanSkip = TimeSpan.FromMilliseconds(m_MediaElement.NaturalDuration.TimeSpan.TotalMilliseconds / 20.0);

                TimeSpan t = m_MediaElement.NaturalDuration.TimeSpan;
                m_TxtTotalLength.Text = $"{t.TotalMinutes:0}:{t.Seconds:00}";
            }

            if (m_AutoPlayIfOpened) Play();
            OnMediaOpened();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            Stop();
            OnMediaEnded();
        }

        private void TimerFadeoutControls_Tick(object sender, EventArgs e)
        {
            m_TimerFadeoutControls.Stop();
            if (m_ActionDismissMediaControlsByTimer == null) FadeOutMediaControls();
            else m_ActionDismissMediaControlsByTimer.Invoke();
        }

        private void MediaElement_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ShowMediaControlsOnTouch && !IsMediaControlsVisible) SlideInMediaControls();
        }

        private void BorderTouch_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BorderTouchMouseDownStart(e.MouseDevice);
        }

        private void BorderTouch_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BorderTouchMouseMove(e.GetPosition(m_PanelProgressBar), e.GetPosition(m_BorderTouch), e.MouseDevice);
        }

        private void BorderTouch_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BorderTouchMouseDownEnded(e.MouseDevice);
        }

        private void BorderTouch_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BorderTouchMouseDownEnded(e.MouseDevice);
        }



        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnBackward_Click(object sender, RoutedEventArgs e)
        {
            Backward();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            Play();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            Pause();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }


        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            Forward();
        }

        #endregion Eventhandler

    }

}
