using Gemelo.Applications.Pmt.BiographiePortal.Code.Misc;
using Gemelo.Components.Common.Exhibits.Settings;
using Gemelo.Components.Common.Wpf.UI.Transitions.Appearance;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Gemelo.Applications.Biographieportal.Controls.SendOwnStories
{
    /// <summary>
    /// Bildschirmtastatur
    /// </summary>
    public partial class ScreenKeyboard : UserControl
    {
        #region Konstanten

        private static readonly Dictionary<ScreenKeyboardMode, List<string>> ConstForbiddenKeysFor =
            new Dictionary<ScreenKeyboardMode, List<string>>
            {
                [ScreenKeyboardMode.All] = new List<string> { },
                [ScreenKeyboardMode.Mail] = new List<string> { "ä", "ö", "ü", "ß", "[space]" },
                [ScreenKeyboardMode.Name] = new List<string> { "~", "|", "=", "%", "^" },
            };

        #endregion Konstanten

        #region Felder und Eigenschaften

        private List<Button> m_Buttons = new List<Button>();

        public EditBox ConnectedEditBox { get; private set; }
        public TextBox ConnectedTextBox { get; private set; }

        private ScreenKeyboardMode m_Mode = ScreenKeyboardMode.All;

        public ScreenKeyboardMode Mode
        {
            get => m_Mode;
            set => SetMode(value);
        }

        public CaseMode CaseMode { get; set; } = CaseMode.Lower;

        #endregion Felder und Eigenschaften

        #region Ereignisse

        public event EventHandler EnterClick;

        protected void OnEnterClick()
        {
            EnterClick?.Invoke(this, null);
        }

        #endregion Ereignisse

        #region Konstruktor und Initialisierung

        public ScreenKeyboard()
        {
            InitializeComponent();
            CreateButtonListAndBindEvents();
        }


        private void CreateButtonListAndBindEvents()
        {
            foreach (StackPanel panel in m_StackMain.Children)
            {
                foreach (UIElement element in panel.Children)
                {
                    if (element is Button) AddButtonAndBindEvent((Button)element);
                    else if (element is Canvas)
                    {
                        foreach (Button subButton in ((Canvas)element).Children) AddButtonAndBindEvent(subButton);
                    }
                }
            }
        }

        private void AddButtonAndBindEvent(Button button)
        {
            m_Buttons.Add(button);
            button.Click += Button_Click;
        }

        #endregion Konstruktor und Initialisierung

        #region Öffentliche Methoden

        public void ShowUp()
        {
            //this.SlideIn(SlideDirection.Bottom);
            //m_BorderBackground.StopWpfEffectAndShow();
            //m_BorderBackground.DelayedFadeOut(delayInMs: 400, durationInMs: 500);
            //m_BorderBackground.HideWithEffect(HideEffect.FadeOut, startDelayInMilliseconds: 400, durationInMilliseconds: 500);
        }

        public void HideDown()
        {
            //this.SlideOut(SlideDirection.Bottom);
            //m_BorderBackground.StopWpfEffectAndHide();
            //m_BorderBackground.FadeIn();

        }


        public void SetConnectedTextBox(TextBox textBox, EditBox editBox)
        {
            if (textBox != ConnectedTextBox)
            {
                ConnectedEditBox = editBox;
                ConnectedTextBox = textBox;
                ConnectedTextBox.Select(ConnectedTextBox.Text.Length, 0);
            }
        }

        #endregion Öffentliche Methoden

        #region Private Methoden

        private void SetMode(ScreenKeyboardMode value)
        {
            m_Mode = value;
            List<string> forbiddenKeys = ConstForbiddenKeysFor[value];
            foreach (Button button in m_Buttons)
            {
                string buttonContent = button.Tag != null ? button.Tag.ToString() : button.Content.ToString();
                button.IsEnabled = !forbiddenKeys.Contains(buttonContent.ToLowerInvariant());
            }
        }

        private void AddTextToTextBox(string text)
        {
            switch (CaseMode)
            {
                case CaseMode.Lower:
                    text = text.ToLowerInvariant();
                    break;
                case CaseMode.Upper:
                    text = text.ToUpperInvariant();
                    break;
            }
            if (ConnectedTextBox != null &&
                (ConnectedTextBox.MaxLength == 0 || ConnectedTextBox.Text.Length < ConnectedTextBox.MaxLength))
            {
                ConnectedTextBox.SelectedText = text;
                ConnectedTextBox.SelectionLength = 0;
                ConnectedTextBox.SelectionStart++;
                ConnectedTextBox.Focus();
            }
        }

        private void OnButtonClick(Button button)
        {
            if (button.Tag != null)
            {
                switch (button.Tag.ToString())
                {
                    case "[backspace]":
                        OnBackSpaceButtonClick();
                        break;
                    case "[enter]":
                        OnEnterButtonClick();
                        break;
                    case "[space]":
                        OnSpaceButtonClick();
                        break;
                }
            }
            else OnCharButtonClick(button);
        }

        private void OnEnterButtonClick()
        {
            if (ConnectedEditBox != null && ConnectedEditBox.AllowEnter)
            {
                AddTextToTextBox("\n");
            }

            OnEnterClick();
        }

        private void OnCharButtonClick(Button charButton)
        {
            AddTextToTextBox(charButton.Content.ToString().ToUpper());
        }

        private void OnBackSpaceButtonClick()
        {
            if (ConnectedTextBox != null)
            {
                if (ConnectedTextBox.SelectionLength > 0)
                {
                    ConnectedTextBox.SelectedText = string.Empty;
                }
                else if (ConnectedTextBox.SelectionStart > 0)
                {
                    int start = ConnectedTextBox.SelectionStart;
                    string currentText = ConnectedTextBox.Text;
                    ConnectedTextBox.Text = currentText.Substring(0, start - 1) + currentText.Substring(start);
                    ConnectedTextBox.SelectionStart = start - 1;
                }
                ConnectedTextBox.Focus();
            }
        }

        private void OnSpaceButtonClick()
        {
            AddTextToTextBox(" ");
        }

        //protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        //{
        //    base.OnRenderSizeChanged(sizeInfo);
        //    if (this.HasActualSize() && m_StackMain.HasActualSize())
        //    {
        //        m_ScaleBorderMain.ScaleX = m_ScaleBorderMain.ScaleY = ActualWidth / m_StackMain.ActualWidth;
        //    }
        //}

        #endregion Private Methoden

        #region Ereignishandler

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button typedSender = (Button)sender;
            OnButtonClick(typedSender);
            RestartTimer.Restart();
        }

        #endregion Ereignishandler
    }

    public enum ScreenKeyboardMode
    {
        All,
        Mail,
        Name
    }

    public enum CaseMode
    {
        Lower,
        Upper
    }
}
