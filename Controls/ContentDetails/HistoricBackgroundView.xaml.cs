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
    /// Interaktionslogik für HistoricBackgroundView.xaml
    /// </summary>
    public partial class HistoricBackgroundView : UserControl
    {
        public HistoricBackgroundView(Code.Data.HistoricBackground hb)
        {
            InitializeComponent();

            m_TxtTitle.SetTextsFromLocalizationString(hb.Headline);
            m_TxtText.SetTextsFromLocalizationString(hb.Text);
        }
    }
}
