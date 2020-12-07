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
using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Applications;

namespace Gemelo.Applications.Biographieportal.Controls
{
    /// <summary>
    /// Interaktionslogik für DahButton.xaml
    /// </summary>
    public partial class DahButton : UserControl
    {

        #region öffentliche Properties


        private bool m_FireClickAtTouchDown = false;

        public bool FireClickAtTouchDown
        {
            get { return m_FireClickAtTouchDown; }
            set { SetFireClickAtTouchDown(value); }
        }


        public BasePartialData Data { get; private set; }

        #region DependencyProperty ButtonBackground

        public static readonly DependencyProperty ButtonBackgroundProperty =
           DependencyProperty.Register(
           "ButtonBackground", typeof(Brush),
           typeof(DahButton),
           new PropertyMetadata(null, new PropertyChangedCallback(OnButtonBackgroundChanged)));

        [System.ComponentModel.Description("ButtonBackground")]
        [System.ComponentModel.Category("gemelo")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public Brush ButtonBackground
        {
            get { return ((Brush)(GetValue(DahButton.ButtonBackgroundProperty))); }
            set { SetValue(DahButton.ButtonBackgroundProperty, value); }
        }

        private static void OnButtonBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DahButton ctrl = d as DahButton;
            if (ctrl != null)
            {
                Brush newValue = (Brush)e.NewValue;
                Brush oldValue = (Brush)e.OldValue;

                //TODO: Changed implementieren
            }
        }

        #endregion DependencyProperty ButtonBackground

        #region DependencyProperty Text

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register(
           "Text", typeof(BiographyLocalizationString),
           typeof(DahButton),
           new PropertyMetadata(new BiographyLocalizationString(), new PropertyChangedCallback(OnTextChanged)));

        [System.ComponentModel.Description("Text")]
        [System.ComponentModel.Category("gemelo")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public BiographyLocalizationString Text
        {
            get { return ((BiographyLocalizationString)(GetValue(DahButton.TextProperty))); }
            set { SetValue(DahButton.TextProperty, value); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DahButton ctrl = d as DahButton;
            if (ctrl != null)
            {
                BiographyLocalizationString newValue = (BiographyLocalizationString)e.NewValue;
                BiographyLocalizationString oldValue = (BiographyLocalizationString)e.OldValue;

                ctrl.m_Txt.SetTextsFromLocalizationString(newValue);
            }
        }

        #endregion DependencyProperty Text

        #region DependencyProperty TextDe

        public static readonly DependencyProperty TextDeProperty =
           DependencyProperty.Register(
           "TextDe", typeof(string),
           typeof(DahButton),
           new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextDeChanged)));

        [System.ComponentModel.Description("TextDe")]
        [System.ComponentModel.Category("gemelo")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public string TextDe
        {
            get { return ((string)(GetValue(DahButton.TextDeProperty))); }
            set { SetValue(DahButton.TextDeProperty, value); }
        }

        private static void OnTextDeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DahButton ctrl = d as DahButton;
            if (ctrl != null)
            {
                string newValue = (string)e.NewValue;
                string oldValue = (string)e.OldValue;

                Pete.Components.WpfExtensions.Localization.Localization.SetTextDe(ctrl.m_Txt, newValue);
                ctrl.m_Txt.Text = newValue;
            }
        }

        #endregion DependencyProperty TextDe

        #region DependencyProperty TextEn

        public static readonly DependencyProperty TextEnProperty =
           DependencyProperty.Register(
           "TextEn", typeof(string),
           typeof(DahButton),
           new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextEnChanged)));

        [System.ComponentModel.Description("TextEn")]
        [System.ComponentModel.Category("gemelo")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public string TextEn
        {
            get { return ((string)(GetValue(DahButton.TextEnProperty))); }
            set { SetValue(DahButton.TextEnProperty, value); }
        }

        private static void OnTextEnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DahButton ctrl = d as DahButton;
            if (ctrl != null)
            {
                string newValue = (string)e.NewValue;
                string oldValue = (string)e.OldValue;

                Pete.Components.WpfExtensions.Localization.Localization.SetTextEn(ctrl.m_Txt, newValue);
            }
        }

        #endregion DependencyProperty TextEn

        #endregion öffentliche Properties

        #region Events


        public event EventHandler<EventArgs> Clicked;

        protected void OnClicked()
        {
            Clicked?.Invoke(this, new EventArgs());
        }


        #endregion Events

        public DahButton()
        {
            BorderThickness = new Thickness(4);
            FontSize = 18.0;
            HorizontalAlignment = HorizontalAlignment.Left;
            InitializeComponent();
        }

        #region öffentliche Methoden

        public void SetData(BasePartialData data)
        {
            Data = data;
            TextDe = data.Text.De;
            TextEn = data.Text.En;
            ButtonBackground = new SolidColorBrush(data.TypeColor);
        }


        #endregion öffentliche Methoden

        private void SetFireClickAtTouchDown(bool value)
        {
            if (value != m_FireClickAtTouchDown)
            {
                m_FireClickAtTouchDown = value;
                m_Btn.FireClickAtTouchDown = value;
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            OnClicked();

            if (Data != null)
            {
                App.Current.MainWindow.SetContentListFilter(Data, addToHistory: true);
            }
        }
    }
}
