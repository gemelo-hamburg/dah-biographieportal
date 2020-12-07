using Gemelo.Applications.Biographieportal.Code;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gemelo.Applications.Biographieportal.Controls.SendOwnStories
{
    /// <summary>
    /// Interaktionslogik für CheckContentMessage.xaml
    /// </summary>
    /// 
    [ContentProperty("TextString")]
    public partial class CheckContentMessage : UserControl
    {


        private string m_TextString = string.Empty;

        public string TextString
        {
            get { return m_TextString; }
            set { SetTextString(value); }
        }

       


        private LocalizationString m_Text = new LocalizationString();

        public LocalizationString Text
        {
            get { return m_Text; }
            set { SetText(value); }
        }

      


        public CheckContentMessage()
        {
            InitializeComponent();
        }

        private void SetText(LocalizationString value)
        {
            if (value != m_Text)
            {
                m_Text = value;
                m_Txt.Text = value.LocalizedStrings["de"];
                m_Txt.SetTextsFromLocalizationString(value);
            }
        }

        private void SetTextString(string value)
        {
            if (value != m_TextString)
            {
                m_TextString = value;

                SetText(LocalizationString.Parse(value));
            }
        }

        internal void Hide()
        {
            m_Txt.Visibility = Visibility.Collapsed;
        }

        public void ShowUp()
        {
            m_Txt.SlideIn(SlideDirection.Left);
        }
        public void HideDown()
        {
            m_Txt.FadeOutIfVisible();
        }

        
    }
}
